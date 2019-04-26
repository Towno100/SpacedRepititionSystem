using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewEngine
{
    public interface IReviewManager<T> where T : IReviewable
    {
        IEnumerable<T> GetCurrent(IEnumerable<T> reviewItems);

        int ParseTestResults<R>(IEnumerable<T> testItems, IEnumerable<R> resultItems, Func<R, T, bool> parseFunction);
    }
}
