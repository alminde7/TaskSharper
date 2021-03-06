﻿using TaskSharper.Domain.Calendar;
using TaskSharper.Domain.Models;

namespace TaskSharper.WPF.Common.Media
{
    public class CategoryToIconConverter
    {
        public static string ConvertToFontAwesomeIcon(string category, EventType type)
        {
            switch (category)
            {
                case "Medication":
                    return "Medkit";
                case "Hygiene":
                    return "Shower";
                case "Social":
                    return "Users";
                default:
                    switch (type)
                    {
                        case EventType.None:
                            return "Info";
                        case EventType.Appointment:
                            return "ListUl";
                        case EventType.Task:
                            return "Tasks";
                        default:
                            return "Info";
                    }
            }
        }
    }
}
