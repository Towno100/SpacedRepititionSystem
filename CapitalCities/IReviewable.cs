using System;

namespace ReviewEngine
{
    public interface IReviewable
    {
        int Score { get; set; }
        int StudyOrder { get; set; }
        bool IsCurrent { get; set; }
        DateTime LastStudied { get; set; }
    }
}
