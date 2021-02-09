using System;

namespace SimpleCache
{
    /// <summary>
    /// Packaged data, contains some useful properties like LifeSpan and Due. 
    /// Datas need package into CacheData in order to store in storage.
    /// </summary>
    public interface ISimpleCacheData
    {
        /// <summary>
        /// Create time of this object.
        /// </summary>
        DateTime Create { get; }

        /// <summary>
        /// Data which store in this object.
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// Due time of this object.
        /// </summary>
        DateTime? Due { get; set; }

        /// <summary>
        /// Life span of this object.
        /// </summary>
        TimeSpan? LifeSpan { get; set; }

        /// <summary>
        /// Extend due time by re-apply life span property again.
        /// </summary>
        void Extend();

        /// <summary>
        /// Determine if due time was greater than now.
        /// </summary>
        /// <returns></returns>
        bool IsOverDue();
    }
}