using System;
using System.Timers;

namespace TaskSharper.Shared.Extensions
{
    public static class TimerExtensions
    {
        public static Timer StartTimer(this Timer timer)
        {
            timer.Start();
            return timer;
        }

        public static Timer Interval(this Timer timer, double intervalInMs)
        {
            timer.Interval = intervalInMs;
            return timer;
        }

        public static Timer Interval(this Timer timer, TimeSpan interval)
        {
            timer.Interval = interval.TotalMilliseconds;
            return timer;
        }

        public static Timer Loop(this Timer timer, bool isLooping = true)
        {
            timer.AutoReset = isLooping;
            return timer;
        }

        public static Timer Enable(this Timer timer, bool enable = true)
        {
            timer.Enabled = enable;
            return timer;
        }

        public static Timer Callback(this Timer timer, Action callback)
        {
            timer.Elapsed += (sender, args) =>
            {
                callback();
            };
            return timer;
        }

        public static Timer Callback<T>(this Timer timer, T obj, Action<T> callback)
        {
            timer.Elapsed += (sender, args) =>
            {
                callback(obj);
            };
            return timer;
        }

        public static Timer SetDailyScheduler(this Timer timer, TimeSpan timeOfDay, Action callback)
        {
            timer.Interval = IntervalInMilliseconds(timeOfDay);
            timer.AutoReset = true;

            timer.Elapsed += (sender, args) =>
            {
                callback();
                timer = new Timer().SetDailyScheduler(timeOfDay, callback);
            };

            return timer;
        }

        //// Inspiration from here: https://social.technet.microsoft.com/wiki/contents/articles/37252.c-timer-schedule-a-task.aspx
        //public static Timer StartDailyScheduler(this Timer timer, TimeSpan timeOfDay, Action callback)
        //{
        //    timer = new Timer();
        //    timer.Interval = IntervalInMilliseconds(timeOfDay);
        //    timer.AutoReset = false;
        //    timer.Enabled = true;
        //    timer.Elapsed += (sender, args) =>
        //    {
        //        callback();
        //        timer.StartDailyScheduler(timeOfDay, callback);
        //    };

        //    timer.Start();
        //    return timer;
        //}

        private static double IntervalInMilliseconds(TimeSpan timeOfDay)
        {
            var interval = timeOfDay.Subtract(DateTime.Now.TimeOfDay);

            double intervalInMs = interval.TotalMilliseconds;

            if (interval.TotalMilliseconds < 0)
            {
                // Event is in the past.... make it the future
                intervalInMs = (new TimeSpan(24, 0, 0)).Add(interval).TotalMilliseconds;
            }

            return intervalInMs;
        }
    }
}
