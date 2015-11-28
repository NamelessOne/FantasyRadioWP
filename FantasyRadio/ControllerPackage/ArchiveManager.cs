using FantasyRadio.DataModel;
using FantasyRadio.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FantasyRadio
{
    class ArchiveManager : INotifyPropertyChanged
    {
        public ArchiveParser Parser { get; private set; } = new ArchiveParser();
        private ObservableCollection<ArchiveEntity> archiveItems = new ObservableCollection<ArchiveEntity>();
        public string Login { get; set; } = "NamelessOne";
        public string Password { get; set; } = "pen3souin";
        public ObservableCollection<ArchiveEntity> Items
        {
            get
            {
                return archiveItems;
            }
            set
            {
                archiveItems.Clear();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        archiveItems.Add(item);
                    }
                }
                Notify("Items");
            }
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
