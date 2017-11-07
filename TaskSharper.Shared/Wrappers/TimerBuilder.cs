using System;
using System.Timers;

namespace TaskSharper.Shared.Wrappers
{
    public class TimerBuilder
    {
        public Timer timer;

        public TimerBuilder()
        {
            timer = new Timer();
        }
    }

    public static class TimerBuilderExtensions
    {
        //public static TimerBuilder Create(this TimerBuilder timerBuilder)
        //{
        //    timerBuilder.timer = new Timer();
        //    timerBuilder.timer.Enabled = true;
        //    return timerBuilder;
        //}

        public static TimerBuilder SetInterval(this TimerBuilder timerBuilder, double interval)
        {
            timerBuilder.timer.Interval = interval;
            return timerBuilder;
        }

        public static TimerBuilder AutoReset(this TimerBuilder timerBuilder, bool autoreset = true)
        {
            timerBuilder.timer.AutoReset = autoreset;
            return timerBuilder;
        }

        public static TimerBuilder Start(this TimerBuilder timerBuilder)
        {
            timerBuilder.timer.Start();
            return timerBuilder;
        }

        public static TimerBuilder AddCallback<T>(this TimerBuilder timerBuilder, T obj, Action<T> callback)
        {
            timerBuilder.timer.Elapsed += (sender, args) =>
            {
                callback(obj);
            };
            return timerBuilder;
        }

        public static TimerBuilder AddCallback(this TimerBuilder timerBuilder, Action callback)
        {
            timerBuilder.timer.Elapsed += (sender, args) =>
            {
                callback();
            };
            return timerBuilder;
        }

        public static TimerBuilder SetDailyScheduler(this TimerBuilder timerBuilder, TimeSpan timeOfDay, Action callback)
        {
            timerBuilder.timer.Interval = IntervalInMilliseconds(timeOfDay);
            timerBuilder.timer.Elapsed += (sender, args) =>
            {
                callback();
                timerBuilder.SetDailyScheduler(timeOfDay, callback);
            };

            return timerBuilder;
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