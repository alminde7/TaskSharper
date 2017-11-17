using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Domain.Calendar;

namespace TaskSharper.Calender.WPF.Helpers.EventLocation
{
    public class EventLocationData
    {
        public string EventId { get; set; }
        public Event Event { get; set; }
        //public double Column { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double WidthModifier { get; set; } // Because Width is set in "OnLoaded", the Width cannot be set, but we can set what the modifier should be (eg. 0.5 is half the width of the container)
        public double Height { get; set; }
    }

    public class EventLocation
    {
        public static List<List<EventLocationData>> FindLayout(List<Event> events)
        {
            var columns = new List<List<EventLocationData>>();
            foreach (var @event in events)
            {
                if (columns.Count == 0)
                {
                    columns.Add(new List<EventLocationData>(){new EventLocationData(){Event = @event}});
                }
                else
                {
                    var columnsCount = columns.Count;
                    for (int i = 0; i < columnsCount; i++)
                    {
                        bool toBreak = false;
                        int columnToAdd = 0;
                        for (int j = 0; j < columns[i].Count; j++)
                        {
                            if (columns[i][j].Event.Id != @event.Id && columns[i][j].Event.Start < @event.End && @event.Start < columns[i][j].Event.End)
                            {
                                if (columns.Count <= i+1)
                                {
                                    columns.Add(new List<EventLocationData>());
                                    columnsCount++;
                                }
                                toBreak = true;
                                columnToAdd = 1;
                            }
                        }
                        columns[i + columnToAdd].Add(new EventLocationData() { Event = @event });
                        if (toBreak)
                        {
                            break;
                        }
                    }
                }
            }



            return columns;
        }
    }
}
