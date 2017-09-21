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
    }
}
