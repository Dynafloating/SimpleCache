using System;
using System.Threading.Tasks;

namespace SimpleCache
{
    /// <summary>
    /// An Asp.net core naming style interface of SimpleCacheStorage for dependency injection.
    /// </summary>
    public interface ISimpleCacheService
    {
        /// <summary>
        /// Interval of timer which will check and clear overdue data periodically.
        /// Default value is 10000ms (10 secands).
        /// </summary>
        int ClearInterval { get; set; }

        /// <summary>
        /// Determines whether specified key existed in cache.
        /// </summary>
        /// <param name="key">The key of the data to check.</param>
        bool ContainsKey(string key);

        /// <summary>
        /// Determines whether specified key was over due, which will be remove in next clear period.
        /// </summary>
        /// <param name="key">The key of the data to check.</param>
        bool? IsOverDue(string key);

        /// <summary>
        /// Attempts to get the data associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the data to get.</param>
        /// <returns>Return data in object form if key was found; otherwise return null.</returns>
        object Get(string key);

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <returns>Return data in specified type if key was found; otherwise return default value of type.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or add/replace data with output of specified func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="replaceFunc">Func that return data when key was not found.</param>
        /// <returns></returns>
        T SmartGet<T>(string key, Func<T> replaceFunc);

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or add/replace data with output of specified async func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="replaceFunc">Async func that return data when key was not found.</param>
        /// <returns></returns>
        Task<T> SmartGetAsync<T>(string key, Func<Task<T>> replaceFunc);

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or add/replace data with output of specified func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="lifeSpan">Lifespan of the data.</param>
        /// <param name="replaceFunc">Func that return data when key was not found.</param>
        /// <returns></returns>
        T SmartGet<T>(string key, TimeSpan lifeSpan, Func<T> replaceFunc);

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or add/replace data with output of specified async func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="lifeSpan">Lifespan of the data.</param>
        /// <param name="replaceFunc">Async func that return data when key was not found.</param>
        /// <returns></returns>
        Task<T> SmartGetAsync<T>(string key, TimeSpan lifeSpan, Func<Task<T>> replaceFunc);

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or add/replace data with output of specified func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="due">Due of the data.</param>
        /// <param name="replaceFunc">Func that return data when key was not found.</param>
        /// <returns></returns>
        T SmartGet<T>(string key, DateTime due, Func<T> replaceFunc);

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or add/replace data with output of specified async func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="due">Due of the data.</param>
        /// <param name="replaceFunc">Async func that return data when key was not found.</param>
        /// <returns></returns>
        Task<T> SmartGetAsync<T>(string key, DateTime due, Func<Task<T>> replaceFunc);

        /// <summary>
        /// Attempts to add specified key and data to cache. If specified key exists, then replace it.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <returns></returns>
        bool Add<T>(string key, T data);

        /// <summary>
        /// Attempts to add specified key and data to cache with specified lifespan. If specified key exists, then replace it.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <param name="lifeSpan">Lifespan of the data.</param>
        /// <returns></returns>
        bool Add<T>(string key, T data, TimeSpan lifeSpan);

        /// <summary>
        /// Attempts to add specified key and data to cache with specified due. If specified key exists, then replace it.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <param name="due">Due of the data.</param>
        /// <returns></returns>
        bool Add<T>(string key, T data, DateTime due);

        /// <summary>
        /// Attempts to add specified key and data to cache.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <returns></returns>
        bool AddOrReplace<T>(string key, T data);

        /// <summary>
        /// Attempts to add specified key and data to cache with specified lifespan.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <param name="lifeSpan">Lifespan of the data.</param>
        /// <returns></returns>
        bool AddOrReplace<T>(string key, T data, TimeSpan lifeSpan);

        /// <summary>
        /// Attempts to add specified key and data to cache with specified due.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <param name="due">Due of the data.</param>
        /// <returns></returns>
        bool AddOrReplace<T>(string key, T data, DateTime due);

        /// <summary>
        /// Attempts to replace data with specified key.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to replace.</param>
        /// <param name="data">New data to replace.</param>
        /// <returns></returns>
        bool Replace<T>(string key, T data);

        /// <summary>
        /// Attempts to extend due of data use existing lifespan.
        /// </summary>
        /// <param name="key">Key of the data to extend.</param>
        /// <returns></returns>
        bool Extend(string key);

        /// <summary>
        /// Attempts to extend due of data use specified lifespan. New lifespan will be saved.
        /// </summary>
        /// <param name="key">Key of the data to extend.</param>
        /// <param name="lifeSpan">New lifespan.</param>
        /// <returns></returns>
        bool Extend(string key, TimeSpan lifeSpan);

        /// <summary>
        /// Attempts to extend then replace data with specified key.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to renew.</param>
        /// <param name="data">New data to renew.</param>
        /// <returns></returns>
        bool Renew<T>(string key, T data);

        /// <summary>
        /// Attempts to update due of data with specified key.
        /// </summary>
        /// <param name="key">Key of the data to update.</param>
        /// <param name="due">New due of data.</param>
        /// <param name="updateLifeSpan">Recalculate lifespan of data.</param>
        /// <returns></returns>
        bool UpdateDue(string key, DateTime due, bool updateLifeSpan = true);

        /// <summary>
        /// Attempts to remove data with the specified key.
        /// </summary>
        /// <param name="key">The key of the data to remove.</param>
        /// <returns></returns>
        bool Remove(string key);

        /// <summary>
        /// Removes all overdue datas.
        /// </summary>
        void ClearOverDues();

        /// <summary>
        /// Removes all keys and datas.
        /// </summary>
        void Clear();

        /// <summary>
        /// Check if specified key exists. If not, execute replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="replaceFunc">Replace func to generate.</param>
        /// <returns></returns>
        bool CheckAndAddOrReplace<T>(string key, Func<T> replaceFunc);

        /// <summary>
        /// Check if specified key exists. If not, execute async replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="replaceFunc">Async replace func to generate.</param>
        /// <returns></returns>
        Task<bool> CheckAndAddOrReplaceAsync<T>(string key, Func<Task<T>> replaceFunc);

        /// <summary>
        /// Check if specified key exists. If not, execute replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="lifeSpan">Lifespan of the data.</param>
        /// <param name="replaceFunc">Replace func to generate.</param>
        /// <returns></returns>
        bool CheckAndAddOrReplace<T>(string key, TimeSpan lifeSpan, Func<T> replaceFunc);

        /// <summary>
        /// Check if specified key exists. If not, execute async replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="lifeSpan">Lifespan of the data.</param>
        /// <param name="replaceFunc">Async replace func to generate.</param>
        /// <returns></returns>
        Task<bool> CheckAndAddOrReplaceAsync<T>(string key, TimeSpan lifeSpan, Func<Task<T>> replaceFunc);

        /// <summary>
        /// Check if specified key exists. If not, execute replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="due">Due of the data.</param>
        /// <param name="replaceFunc">Replace func to generate.</param>
        /// <returns></returns>
        bool CheckAndAddOrReplace<T>(string key, DateTime due, Func<T> replaceFunc);

        /// <summary>
        /// Check if specified key exists. If not, execute async replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="due">Due of the data.</param>
        /// <param name="replaceFunc">Async replace func to generate.</param>
        /// <returns></returns>
        Task<bool> CheckAndAddOrReplaceAsync<T>(string key, DateTime due, Func<Task<T>> replaceFunc);
    }
}
