using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSharper.Calender.WPF.Properties;
using TaskSharper.Domain.Calendar;
using TaskSharper.Shared.Constants;
using TaskSharper.WPF.Common.Properties;
using Settings = TaskSharper.WPF.Common.Properties.Settings;

namespace TaskSharper.Calender.WPF.Helpers.EventLocation
{
    public class EventLocationData
    {
        public string EventId { get; set; }
        public Event Event { get; set; }
        public double Column { get; set; }
        public double TotalColumns { get; set; }
        public int ColumnSpan { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
        public double Height { get; set; }
    }

    public class EventLocation
    {
        public static List<List<EventLocationData>> FindLayout(List<Event> events)
        {
            var columns = new List<List<EventLocationData>>();
            foreach (var @event in events.OrderBy(o => o.Start).ThenBy(o => o.End))
            {
                if (columns.Count == 0)
                {
                    columns.Add(new List<EventLocationData>()
                    {
                        new EventLocationData()
                        {
                            Event = @event,
                            Column = 0,
                            PosY = FindStartPositionY(@event),
                            Height = FindHeight(@event) 
                        } 
                    });
                }
                else
                {
                    var columnsCount = columns.Count;
                    int columnToAdd = 0;
                    for (int column = 0; column < columnsCount; column++)
                    {
                        bool toBreak = true;
                        for (int row = 0; row < columns[column].Count; row++)
                        {
                            if (IsOverlapping(columns[column][row].Event, @event))
                            {
                                if (columns.Count <= column + 1)
                                {
                                    columns.Add(new List<EventLocationData>());
                                    columnsCount++;
                                }
                                columnToAdd++;
                                toBreak = false;
                                break;
                            }
                        }
                        if (toBreak)
                        {
                            break;
                        }
                    }
                    columns[columnToAdd].Add(new EventLocationData()
                    {
                        Event = @event,
                        Column = columnToAdd,
                        PosY = FindStartPositionY(@event),
                        Height = FindHeight(@event)
                    });
                }
            }

            //TODO: ColumnSpan, TotalColumns
            var totalColumns = columns.Count;
            for (var column = 0; column < columns.Count; column++)
            {
                for (var row = 0; row < columns[column].Count; row++)
                {
                    int columnSpan = 0;
                    var eventData = columns[column][row];
                    if (totalColumns > column)
                    {
                        for (int i = column + 1; i < columns.Count; i++)
                        {
                            bool eventCanSpan = true;
                            for (int j = 0; j < columns[i].Count; j++)
                            {
                                if (IsOverlapping(eventData.Event, columns[i][j].Event))
                                {
                                    eventCanSpan = false;
                                    break;
                                }
                            }
                            if (eventCanSpan)
                                columnSpan++;
                            else
                                break;
                        }
                    }
                    eventData.ColumnSpan = columnSpan;
                    eventData.TotalColumns = totalColumns;
                }
            }


            return columns;
        }

        private static bool IsOverlapping(Event event1, Event event2)
        {
            return event1.Id != event2.Id && event1.Start < event2.End && event2.Start < event1.End;
        }

        private static double FindStartPositionY(Event eventObj)
        {
            return eventObj.Start.Value.Hour / TimeConstants.HoursInADay *
                   Settings.Default.CalendarStructure_Height_1200 + eventObj.Start.Value.Minute /
                   TimeConstants.MinutesInAnHour / TimeConstants.HoursInADay *
                   Settings.Default.CalendarStructure_Height_1200;
        }

        private static double FindHeight(Event eventObj)
        {
            return (eventObj.End.Value - eventObj.Start.Value).TotalMinutes / TimeConstants.MinutesInAnHour /
                   TimeConstants.HoursInADay * Settings.Default.CalendarStructure_Height_1200;
        }
    }

}
