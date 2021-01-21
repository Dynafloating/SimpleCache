using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SimpleCache
{
    /// <summary>
    /// SimpleCacheStorage
    /// </summary>
    public class SimpleCacheStorage : IDisposable
    {
        private readonly ConcurrentDictionary<string, ISimpleCacheData> _storage = new ConcurrentDictionary<string, ISimpleCacheData>();
        private Timer _timer;
        private int _clearInterval = 10000;

        /// <summary>
        /// Interval of timer which will check and clear overdue data periodically.
        /// Default value is 10000ms (10 secands).
        /// </summary>
        public int ClearInterval
        {
            get => _clearInterval;
            set
            {
                _clearInterval = value;
                _timer.Interval = _clearInterval;
            }
        }

        /// <summary>
        /// SimpleCacheStorage
        /// </summary>
        public SimpleCacheStorage()
        {
            SetupClearTimer();
        }

        #region [Check & Get]

        /// <summary>
        /// Determines whether specified key existed in cache.
        /// </summary>
        /// <param name="key">The key of the data to check.</param>
        public virtual bool ContainsKey(string key)
        {
            return _storage.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether specified key was over due, which will be remove in next clear period.
        /// </summary>
        /// <param name="key">The key of the data to check.</param>
        public virtual bool? IsOverDue(string key)
        {
            return GetCacheData(key, out ISimpleCacheData cacheData) ? (bool?)cacheData.IsOverDue() : null;
        }

        /// <summary>
        /// Attempts to get the data associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the data to get.</param>
        /// <returns>Return data in object form if key was found; otherwise return null.</returns>
        public virtual object Get(string key)
        {
            return GetCacheData(key, out ISimpleCacheData cacheData) && !cacheData.IsOverDue() ? cacheData.Data : default;
        }

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <returns>Return data in specified type if key was found; otherwise return default value of type.</returns>
        public virtual T Get<T>(string key)
        {
            var value = Get(key);
            return value != default ? (T)value : default;
        }

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or return output of specified func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="replaceFunc">Func that return data when key was not found.</param>
        /// <returns></returns>
        public T Get<T>(string key, Func<T> replaceFunc)
        {
            return CheckAndReplace(key, replaceFunc) ? Get<T>(key) : default;
        }

        /// <summary>
        /// Attempts to get the data in specified type associated with the specified key; 
        /// or return output of specified async func when key was not found.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to get.</param>
        /// <param name="replaceFunc">Async func that return data when key was not found.</param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string key, Func<Task<T>> replaceFunc)
        {
            return await CheckAndReplaceAsync(key, replaceFunc) ? Get<T>(key) : default;
        }

        #endregion

        #region [Add & Update]

        /// <summary>
        /// Attempts to add specified key and data to cache.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <returns></returns>
        public virtual bool Add<T>(string key, T data)
        {
            return Remove(key) && _storage.TryAdd(key, PackageData(data));
        }

        /// <summary>
        /// Attempts to add specified key and data to cache with specified lifespan.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <param name="lifeSpan">Lifespan of the data.</param>
        /// <returns></returns>
        public virtual bool Add<T>(string key, T data, TimeSpan lifeSpan)
        {
            return Remove(key) && _storage.TryAdd(key, PackageData(data, lifeSpan));
        }

        /// <summary>
        /// Attempts to add specified key and data to cache with specified due.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to add.</param>
        /// <param name="data">Data to add.</param>
        /// <param name="due">Due of the data.</param>
        /// <returns></returns>
        public virtual bool Add<T>(string key, T data, DateTime due)
        {
            return Remove(key) && _storage.TryAdd(key, PackageData(data, due));
        }

        /// <summary>
        /// Attempts to replace data with specified key.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to replace.</param>
        /// <param name="data">New data to replace.</param>
        /// <returns></returns>
        public virtual bool Replace<T>(string key, T data)
        {
            return ProcessCacheData(key, cacheData =>
            {
                cacheData.Data = data;
                return true;
            });
        }

        /// <summary>
        /// Attempts to extend due of data use existing lifespan.
        /// </summary>
        /// <param name="key">Key of the data to extend.</param>
        /// <returns></returns>
        public virtual bool Extend(string key)
        {
            return ProcessCacheData(key, cacheData =>
            {
                cacheData.Extend();
                return true;
            });
        }

        /// <summary>
        /// Attempts to extend due of data use specified lifespan. New lifespan will be saved.
        /// </summary>
        /// <param name="key">Key of the data to extend.</param>
        /// <param name="lifeSpan">New lifespan.</param>
        /// <returns></returns>
        public virtual bool Extend(string key, TimeSpan lifeSpan)
        {
            return ProcessCacheData(key, cacheData =>
            {
                cacheData.LifeSpan = lifeSpan;
                cacheData.Extend();
                return true;
            });
        }

        /// <summary>
        /// Attempts to extend then replace data with specified key.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">Key of the data to renew.</param>
        /// <param name="data">New data to renew.</param>
        /// <returns></returns>
        public virtual bool Renew<T>(string key, T data)
        {
            return ProcessCacheData(key, cacheData =>
            {
                cacheData.Data = data;
                cacheData.Extend();
                return true;
            });
        }

        /// <summary>
        /// Attempts to update due of data with specified key.
        /// </summary>
        /// <param name="key">Key of the data to update.</param>
        /// <param name="due">New due of data.</param>
        /// <param name="updateLifeSpan">Recalculate lifespan of data.</param>
        /// <returns></returns>
        public virtual bool UpdateDue(string key, DateTime due, bool updateLifeSpan = true)
        {
            return ProcessCacheData(key, cacheData =>
            {
                if (updateLifeSpan)
                {
                    cacheData.LifeSpan = due - cacheData.Create;
                }

                cacheData.Due = due;
                return true;
            });
        }

        #endregion

        #region [Remove & Clear]

        /// <summary>
        /// Attempts to remove data with the specified key.
        /// </summary>
        /// <param name="key">The key of the data to remove.</param>
        /// <returns></returns>
        public virtual bool Remove(string key)
        {
            if (ContainsKey(key))
            {
                return _storage.TryRemove(key, out _);
            }

            return true;
        }

        /// <summary>
        /// Removes all overdue datas.
        /// </summary>
        public virtual void ClearOverDues()
        {
            var keys = _storage.Keys.ToArray();
            foreach (var key in keys)
            {
                var isOverDue = IsOverDue(key);
                if (isOverDue.HasValue && isOverDue.Value)
                {
                    Remove(key);
                }
            }
        }

        /// <summary>
        /// Removes all keys and datas.
        /// </summary>
        public virtual void Clear()
        {
            _storage.Clear();
        }

        #endregion

        #region [Others]

        /// <summary>
        /// Check if specified key exists. If not, execute replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="replaceFunc">Replace func to generate.</param>
        /// <returns></returns>
        public virtual bool CheckAndReplace<T>(string key, Func<T> replaceFunc)
        {
            var isOverDue = IsOverDue(key);
            if (!isOverDue.HasValue)
            {
                return Add(key, replaceFunc());
            }
            else if (isOverDue.Value)
            {
                return Replace(key, replaceFunc());
            }

            return true;
        }

        /// <summary>
        /// Check if specified key exists. If not, execute async replace func to generate a new one.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="key">The key of the data to check.</param>
        /// <param name="replaceFunc">Async replace func to generate.</param>
        /// <returns></returns>
        public virtual async Task<bool> CheckAndReplaceAsync<T>(string key, Func<Task<T>> replaceFunc)
        {
            var isOverDue = IsOverDue(key);
            if (!isOverDue.HasValue)
            {
                return Add(key, await replaceFunc());
            }
            else if (isOverDue.Value)
            {
                return Replace(key, await replaceFunc());
            }

            return true;
        }

        /// <summary>
        /// Remove all keys and datas.
        /// </summary>
        public virtual void Dispose()
        {
            Clear();
        }

        #endregion

        #region [Protected]

        protected virtual bool GetCacheData(string key, out ISimpleCacheData cacheData)
        {
            var success = _storage.TryGetValue(key, out ISimpleCacheData value);
            if (success)
            {
                cacheData = value;
            }
            else
            {
                cacheData = null;
            }

            return success;
        }

        protected virtual bool ProcessCacheData(string key, Func<ISimpleCacheData, bool> func)
        {
            var exists = GetCacheData(key, out ISimpleCacheData cacheData);
            return exists && func(cacheData);
        }

        protected virtual ISimpleCacheData PackageData(object data)
        {
            return new SimpleCacheData(data);
        }

        protected virtual ISimpleCacheData PackageData(object data, TimeSpan lifeSpan)
        {
            return new SimpleCacheData(data, lifeSpan);
        }

        protected virtual ISimpleCacheData PackageData(object data, DateTime due)
        {
            return new SimpleCacheData(data, due);
        }

        protected virtual void SetupClearTimer()
        {
            _timer = new Timer() { Interval = _clearInterval };
            _timer.Elapsed += new ElapsedEventHandler(OnClearTimerTicking);
        }

        protected virtual void OnClearTimerTicking(object _, ElapsedEventArgs eventArgs)
        {
            ClearOverDues();
        }

        #endregion
    }
}
