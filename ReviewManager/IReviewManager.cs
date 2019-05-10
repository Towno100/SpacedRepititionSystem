using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReviewEngine
{
    public interface IReviewManager<T> where T : IReviewable
    {

        /// <summary>
        /// Fills the bucket of current items to study
        /// </summary>
        /// <param name="reviewItems">The full set of study items</param>
        /// <returns>List of current items</returns>
        IEnumerable<T> GetCurrent(IQueryable<T> reviewItems);

        /// <summary>
        /// Parses the results of a test of study items
        /// </summary>
        /// <typeparam name="R">The Type of the answer</typeparam>
        /// <param name="testItems">The items tested</param>
        /// <param name="resultItems">The answers given in the test</param>
        /// <param name="parseFunction">The function to determine if the answer is correct</param>
        /// <returns></returns>
        int ParseTestResults<R>(IEnumerable<T> testItems, IEnumerable<R> resultItems, Func<R, T, bool> parseFunction);
    }
}
