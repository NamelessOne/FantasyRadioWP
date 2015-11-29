using FantasyRadio.DataModel;
using FantasyRadio.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FantasyRadio
{
    public class ScheduleManager : INotifyPropertyChanged
    {
        private ScheduleParser parser = new ScheduleParser();
        private ObservableCollection<KeyedList<string, ScheduleEntity>> scheduleItems = new ObservableCollection<KeyedList<string, ScheduleEntity>>();
        private bool isParsingActive;
        public bool IsParsingActive
        {
            get
            {
                return isParsingActive;
            }
            set
            {
                isParsingActive = value;
                Notify("IsParsingActive");
            }
        }

        public ObservableCollection<KeyedList<string, ScheduleEntity>> Items {
            get
            {
                return scheduleItems;
            }
            set
            {
                scheduleItems.Clear();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        scheduleItems.Add(item);
                    }
                }
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

        public Task<ObservableCollection<KeyedList<string, ScheduleEntity>>> ParseScheduleAsync()
        {
            return parser.ParseScheduleAsync();
        }
    }
}
