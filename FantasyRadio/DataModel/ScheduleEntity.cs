using System;

namespace FantasyRadio.DataModel
{
    public class ScheduleEntity
    {
        public DateTime StartDate { get; set;}
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string Text { get; set; }
    }
}
