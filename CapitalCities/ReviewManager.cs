using System;
using System.Collections.Generic;
using System.Linq;

namespace ReviewEngine
{
    public class ReviewManager<T>: IReviewManager<T> where T : IReviewable
    {
        private readonly int _numberToReturn;
        private readonly int _numberOfNewItemsToStudyBeforeStartingReview;

        public ReviewManager(int numberToReturn, int numberOfNewItemsToStudyBeforeStartingReview)
        {
            _numberToReturn = numberToReturn;
            _numberOfNewItemsToStudyBeforeStartingReview = numberOfNewItemsToStudyBeforeStartingReview;
        }

        public IEnumerable<T> GetCurrent(IEnumerable<T> reviewItems)
        {
            var currentItemsCount = reviewItems.Count(r => r.IsCurrent);
            SetNewAndReviewItemsToCurrent(reviewItems.Where(r => !r.IsCurrent), _numberToReturn - currentItemsCount);
            return reviewItems.Where(x => x.IsCurrent);
        }

        private void SetNewAndReviewItemsToCurrent(IEnumerable<T> reviewItems, int numberToSet)
        {
            var allStudiedBeforeItems = reviewItems.Where(r => r.Score > 0 && !r.IsCurrent);

            IEnumerable<T> newItemsToReturn;
            IEnumerable<T> reviewItemsToReturn;
            if (allStudiedBeforeItems.Count() > _numberOfNewItemsToStudyBeforeStartingReview)
            {
                newItemsToReturn = reviewItems
                .Where(r => !r.IsCurrent && r.Score == 0)
                .Take(numberToSet / 2);

                reviewItemsToReturn = AddStudiedBeforeItems(allStudiedBeforeItems, numberToSet - newItemsToReturn.Count(), allStudiedBeforeItems.Min(x => x.Score));
            }
            else
            {
                newItemsToReturn = reviewItems
                .Where(r => !r.IsCurrent && r.Score == 0)
                .Take(numberToSet);
                reviewItemsToReturn = new List<T>();
            }

            var itemsToReturn = reviewItemsToReturn.Concat(newItemsToReturn).ToList();

            itemsToReturn.ForEach(r => r.IsCurrent = true);
        }

        private IEnumerable<T> AddStudiedBeforeItems(IEnumerable<T> studiedBeforeItems, int numberRemaining, int score)
        {
            if (numberRemaining == 0)
            {
                return new List<T>();
            }

            var scoreToGet = score > studiedBeforeItems.Max(x => x.Score)
                ? 1
                : score;
            
            var numberToAdd = numberRemaining == 1 ? 1 : numberRemaining / 2;
            var itemsToReview = studiedBeforeItems
                .Where(sbi => sbi.Score == scoreToGet)
                .OrderBy(sbi => sbi.LastStudied)
                .Take(numberToAdd);
            return itemsToReview.Concat(AddStudiedBeforeItems(studiedBeforeItems.Except(itemsToReview), numberRemaining - itemsToReview.Count(), scoreToGet + 1));
        }

        public int ParseTestResults<R>(IEnumerable<T> studyItems, IEnumerable<R> resultItems, Func<R, T, bool> parseFunction)
        {
            var testItems = studyItems.Where(ti => ti.IsCurrent);
            foreach (var t in testItems)
            {
                var isCorrect = resultItems.Any(r => parseFunction(r, t));
                t.IsCurrent = !isCorrect;
                t.Score = isCorrect
                    ? t.Score + 1
                    : 1;
                t.LastStudied = isCorrect ? DateTime.UtcNow : t.LastStudied;
            }
            var correctCount = testItems.Count(t => !t.IsCurrent);
            SetNewAndReviewItemsToCurrent(studyItems.Except(testItems), _numberToReturn - correctCount);
            return correctCount;
        }
    }
}
