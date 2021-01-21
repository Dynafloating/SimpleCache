using System;

namespace SimpleCache
{
    public interface ISimpleCacheData
    {
        DateTime Create { get; }
        object Data { get; set; }
        DateTime? Due { get; set; }
        TimeSpan? LifeSpan { get; set; }

        void Extend();
        bool IsOverDue();
    }
}