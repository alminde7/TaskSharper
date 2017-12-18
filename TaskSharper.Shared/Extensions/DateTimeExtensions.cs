using System;
using System.Runtime.CompilerServices;

namespace TaskSharper.Shared.Extensions
{
    /// <summary>
    /// DateTime extensions
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Get end of day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// Get start of day
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime StartOfDay(this DateTime date)
        {
            return date.Date;
        }


        // https://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week
        /// <summary>
        /// Get start of week
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// Get end of week
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfWeek(this DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Sunday - 7;

            return dt.AddDays(-1 * diff).Date;
        }
    }
}
