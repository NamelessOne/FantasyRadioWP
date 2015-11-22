using FantasyRadio.DataModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace FantasyRadio
{
    public class ScheduleManager : INotifyPropertyChanged
    {
        public ScheduleParser Parser { get; private set; } = new ScheduleParser();
        private List<ScheduleEntity> scheduleItems = new List<ScheduleEntity>();

        public List<ScheduleEntity> Items {
            get
            {
                return scheduleItems;
            }
            set
            {
                scheduleItems = value;
                Notify("Items");
            }
        }

        public void clearEntityes()
        {
            scheduleItems.Clear();
            Notify("Items");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
