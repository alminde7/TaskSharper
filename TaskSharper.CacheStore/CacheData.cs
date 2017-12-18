using System;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.CacheStore
{
    /// <summary>
    /// CacheData used to hold data in cache. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CacheData<T>
    {
        /// <summary>
        /// Is the data to be cache, can be any type of data.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Last time the data was updated
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Determines if the data need to be updated.
        /// </summary>
        public bool ForceUpdate { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="updated"></param>
        /// <param name="forceUpdate"></param>
        public CacheData(T data, DateTime updated, bool forceUpdate)
        {
            Data = data;
            Updated = updated;
            ForceUpdate = forceUpdate;
        }
    }
}