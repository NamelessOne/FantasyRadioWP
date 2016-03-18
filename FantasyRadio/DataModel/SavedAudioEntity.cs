using System.ComponentModel;
using Windows.UI.Xaml.Media;

namespace FantasyRadio.DataModel
{
    class SavedAudioEntity : INotifyPropertyChanged
    {
        public static ImageSource PlayImage { get; set; }
        public static ImageSource PauseImage { get; set; }
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public string Time { get; private set; }
        public string Directory { get; private set; }
        private bool playing;
        public bool Playing
        {
            get
            {
                return playing;
            }
            set
            {
                if (playing != value)
                {
                    playing = value;
                    Notify("Image");
                }
            }
        } 
        public ImageSource Image
        {
            get
            {
                if (Playing)
                {
                    return PauseImage;
                }
                else
                {
                    return PlayImage;
                }                 
            }
            private set
            {

            }
        }
        public SavedAudioEntity(string title, string artist, string time, string directory)
        {
            Artist = artist;
            Directory = directory;
            Time = time;
            Title = title;
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
