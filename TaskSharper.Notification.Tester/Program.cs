using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Notification.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            double date = (DateTime.Now - DateTime.Now.AddMinutes(5)).TotalMilliseconds;


            var data = (DateTime.Now - DateTime.Now.AddMinutes(-5)).TotalMilliseconds;

            var notification = new EventNotification();

            var caleEvent = new Event();
            caleEvent.Title = "asdasd";
            caleEvent.Status = Event.EventStatus.Confirmed;
            caleEvent.Start = DateTime.Now.AddSeconds(5);

            notification.Attach(caleEvent, x =>
            {
                Console.WriteLine(x.Title);
            });
            

            //caleEvent.Status = Event.EventStatus.Completed;

            Console.ReadKey();

            notification.CleanUp();

            Console.ReadKey();
        }
    }
}
