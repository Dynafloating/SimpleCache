namespace SimpleCache
{
    /// <summary>
    /// An Asp.net core naming style class of SimpleCacheStorage for dependency injection.
    /// </summary>
    public class SimpleCacheService : SimpleCacheStorage, ISimpleCacheService
    {
        /// <summary>
        /// SimpleCacheService
        /// </summary>
        public SimpleCacheService(): base() { }
    }
}
