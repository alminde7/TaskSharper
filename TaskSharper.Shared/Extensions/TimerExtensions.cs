using System;
using System.Timers;

namespace TaskSharper.Shared.Extensions
{
    public static class TimerExtensions
    {
        // Inspiration from here: https://social.technet.microsoft.com/wiki/contents/articles/37252.c-timer-schedule-a-task.aspx

        public static Timer StartDailyScheduler(this Timer timer, TimeSpan timeOfDay, Action callback)
        {
            timer = new Timer();
            timer.Interval = IntervalInMilliseconds(timeOfDay);
            timer.AutoReset = false;
            timer.Enabled = true;
            timer.Elapsed += (sender, args) =>
            {
                callback();
                timer.StartDailyScheduler(timeOfDay, callback);
            };

            timer.Start();
            return timer;
        }

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
