using System;
using System.Collections.Generic;
using System.Linq;

namespace ReviewEngine
{
    public class ReviewManager<T> : IReviewManager<T> where T : IReviewable
    {
        private readonly int _numberToReturn;
        private readonly int _numberOfNewItemsToStudyBeforeStartingReview;

        public ReviewManager(int numberToReturn, int numberOfNewItemsToStudyBeforeStartingReview)
        {
            _numberToReturn = numberToReturn;
            _numberOfNewItemsToStudyBeforeStartingReview = numberOfNewItemsToStudyBeforeStartingReview;
        }

        public IEnumerable<T> GetCurrent(IQueryable<T> reviewItems)
        {
            var currentItems = reviewItems.Where(r => r.IsCurrent);
            var currentItemsCount = currentItems.Count();
            var addedCurrentItems = SetNewAndReviewItemsToCurrent(reviewItems.Where(r => !r.IsCurrent), _numberToReturn - currentItemsCount);
            return currentItems.Concat(addedCurrentItems);
        }

        private IEnumerable<T> SetNewAndReviewItemsToCurrent(IQueryable<T> studyItems, int numberToSet)
        {
            var allStudiedBeforeItems = studyItems.Where(r => r.Score > 0);

            var numberOfNewItemsToSet = allStudiedBeforeItems.Count() > _numberOfNewItemsToStudyBeforeStartingReview
                ? numberToSet / 2
                : numberToSet;

            var newItemsToReturn = studyItems
                .Where(r => r.Score == 0)
                .Take(numberOfNewItemsToSet)
                .ToList();
            newItemsToReturn.ForEach(x => x.Score = 1);

            // Get count of newItems to cater for once all items have been seen
            var reviewItemsToReturn = AddStudiedBeforeItems(allStudiedBeforeItems, numberToSet - newItemsToReturn.Count(), allStudiedBeforeItems.Min(x => x.Score));

            var itemsToReturn = newItemsToReturn.Concat(reviewItemsToReturn).ToList();
            itemsToReturn.ForEach(r => r.IsCurrent = true);
            return itemsToReturn;
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

        public int ParseTestResults<R>(IEnumerable<T> testItems, IEnumerable<R> resultItems, Func<R, T, bool> parseFunction)
        {
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
            return correctCount;
        }
    }
}
