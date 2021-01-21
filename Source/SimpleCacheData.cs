using System;

namespace SimpleCache
{
    public class SimpleCacheData : ISimpleCacheData
    {
        public object Data { get; set; }
        public DateTime? Due { get; set; }
        public TimeSpan? LifeSpan { get; set; }
        public DateTime Create { get; } = DateTime.Now;

        public SimpleCacheData(object data)
        {
            Data = data;
        }

        public SimpleCacheData(object data, DateTime due)
        {
            Data = data;
            Due = due;
            LifeSpan = Due - DateTime.Now;
        }

        public SimpleCacheData(object data, TimeSpan lifeSpan)
        {
            Data = data;
            LifeSpan = lifeSpan;
            Due = DateTime.Now.Add(lifeSpan);
        }

        public bool IsOverDue()
        {
            return DateTime.Now > Due;
        }

        public void Extend()
        {
            if (LifeSpan.HasValue)
            {
                Due = DateTime.Now.Add(LifeSpan.Value);
            }
        }
    }
}
