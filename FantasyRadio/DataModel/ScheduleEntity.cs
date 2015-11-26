using System;
using System.Globalization;

namespace FantasyRadio.DataModel
{
    public class ScheduleEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string Text { get; set; }
        public string Day
        {
            get
            {
                //TODO    
                return DateTimeFormatInfo.CurrentInfo.GetDayName(StartDate.DayOfWeek) + ", " + StartDate.Day + "." + StartDate.Month;
            }
        }
        public string Time
        {
            get
            {
                return StartDate.ToString("hh:mm") + " - " + EndDate.ToString("hh:mm");
            }
        }
    }
}
