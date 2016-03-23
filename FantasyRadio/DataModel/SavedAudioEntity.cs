using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
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

        public double PBMaximum { get; set; }
        public double PBValue { get; set; }

        public Visibility ProgressVisibility
        {
            get
            {
                if (playing)
                    return Visibility.Visible;
                else
                    return Visibility.Collapsed;
            }
            private set
            {

            }
        }
        private bool playing;
        public bool Playing
        {
            get
            {
                return playing;
            }
            set
            {
                if (value)
                {
                    playTimer = new DispatcherTimer();
                    playTimer.Interval = TimeSpan.FromMilliseconds(1000); //one second  
                    playTimer.Tick += new EventHandler<object>(playTimer_Tick);
                    playTimer.Start();
                    PBMaximum = BackgroundMediaPlayer.Current.NaturalDuration.TotalSeconds;
                }
                else
                {
                    if (playTimer != null)
                    {
                        playTimer.Stop();
                    }
                    PBMaximum = 0;
                }
                if (playing != value)
                {
                    playing = value;
                    Notify("Image");
                    Notify("ProgressVisibility");
                    Notify("PBMaximum");
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

        DispatcherTimer playTimer;

        private void playTimer_Tick(object sender, object e)
        {
            if (BackgroundMediaPlayer.Current.CurrentState == MediaPlayerState.Playing)
            {
                PBValue = BackgroundMediaPlayer.Current.Position.TotalSeconds;
                Notify("PBValue");
                Debug.WriteLine("PBValue = " + PBValue);
                try
                {
                    //CurrentTime.Text = String.Format(@"{0:hh\:mm\:ss}",
                    //BackgroundAudioPlayer.Instance.Position).Remove(8);
                }
                catch
                {
                    //CurrentTime.Text = String.Format(@"{0:hh\:mm\:ss}",
                    //BackgroundAudioPlayer.Instance.Position);
                }
            }
        }
    }
}
