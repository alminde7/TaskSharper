using System;
using System.Runtime.CompilerServices;

namespace TaskSharper.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime EndOfDay(this DateTime date)
        {
            return date.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return date.Date;
        }

        // https://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week
        public static DateTime StartOfWeek(this DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt)
        {
            int diff = dt.DayOfWeek - DayOfWeek.Sunday - 7;

            return dt.AddDays(-1 * diff).Date;
        }
    }
}
