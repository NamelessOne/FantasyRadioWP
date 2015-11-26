using FantasyRadio.DataModel;
using FantasyRadio.Utils;
using System.Collections.Generic;
using System.ComponentModel;

namespace FantasyRadio
{
    class ArchiveManager : INotifyPropertyChanged
    {
        public ArchiveParser Parser { get; private set; } = new ArchiveParser();
        private List<ArchiveEntity> archiveItems = new List<ArchiveEntity>();
        public List<ArchiveEntity> Items
        {
            get
            {
                return archiveItems;
            }
            set
            {
                archiveItems = value;
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
