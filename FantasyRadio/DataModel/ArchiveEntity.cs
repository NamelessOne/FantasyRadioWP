using System.ComponentModel;
using Windows.UI.Xaml;

namespace FantasyRadio.DataModel
{
    class ArchiveEntity : INotifyPropertyChanged
    {
        public string URL { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        private bool downloadInProgress;
        public bool DownloadInProgress
        {
            get
            {
                return downloadInProgress;
            }
            set
            {
                downloadInProgress = value;
                Notify("ProgressVisibility");
                Notify("DownloadVisibility");
                Notify("DownloadInProgress");
            }
        }
        public Visibility ProgressVisibility
        {
            get
            {
                if (downloadInProgress)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            private set
            {
                
            }
        }

        public Visibility DownloadVisibility
        {
            get
            {
                if (downloadInProgress)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
            }
            private set
            {

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
