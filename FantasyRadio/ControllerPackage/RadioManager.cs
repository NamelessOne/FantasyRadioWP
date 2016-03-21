using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FantasyRadio
{
    public class RadioManager : INotifyPropertyChanged
    {
        private string currentTitle = "";
        public string CurrentTitle
        {
            get
            {
                return currentTitle;
            }
            set
            {
                if (value != CurrentTitle)
                {
                    currentTitle = value;
                    Notify("CurrentTitle");
                }
            }
        }

        private ImageSource playImage;
        private ImageSource pauseImage;
        private ImageSource pressedPlayImage;
        private ImageSource pressedPauseImage;

        public ImageSource PressedPlayPauseImage
        {
            get
            {
                if (CurrentPlayStatus)
                    return pressedPauseImage;
                else
                    return pressedPlayImage;
            }
        }

        public ImageSource PlayPauseImage
        {
            get
            {
                if (CurrentPlayStatus)
                    return pauseImage;
                else
                    return playImage;
            }
        }

        private bool currentPlayStatus;
        public bool CurrentPlayStatus
        {
            get
            {
                return currentPlayStatus;
            }
            set
            {
                currentPlayStatus = value;
                Notify("PlayPauseImage");
                Notify("PressedPlayPauseImage");
                if (value==false)
                {
                    CurrentTitle = "";
                }
                else
                {
                    Controller.getInstance().CurrentSavedManager.CurrentPlayStatus = SavedManager.PlayStatus.Stop;
                }
            }
        }

        private SolidColorBrush BITRATE_ELEMENT_COLOR = new SolidColorBrush(Color.FromArgb(255, 91, 91, 91));
        private SolidColorBrush BITRATE_ELEMENT_COLOR_ACTIVE = new SolidColorBrush(Color.FromArgb(255, 12, 100, 140));

        public SolidColorBrush Bitrate1Color
        {
            get
            {
                if (CurrentBitrate == Bitrates.MP332)
                    return BITRATE_ELEMENT_COLOR_ACTIVE;
                else
                    return BITRATE_ELEMENT_COLOR;
            }
        }
        public SolidColorBrush Bitrate2Color
        {
            get
            {
                if (CurrentBitrate == Bitrates.MP396)
                    return BITRATE_ELEMENT_COLOR_ACTIVE;
                else
                    return BITRATE_ELEMENT_COLOR;
            }
        }

        private Bitrates currentBitrate = Bitrates.MP332;
        public Bitrates CurrentBitrate
        {
            get
            {
                return currentBitrate;
            }
            set
            {
                currentBitrate = value;
                Notify("Bitrate1Color");
                Notify("Bitrate2Color");
            }
        }

        public string getCurrentBitrateUrl()
        {
            switch(CurrentBitrate)
            {
                case Bitrates.MP332:
                    return Constants.streamURLS[0];
                case Bitrates.MP396:
                    return Constants.streamURLS[1];
                default:
                    return Constants.streamURLS[0];
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

        public RadioManager()
        {
            playImage = new BitmapImage(new Uri("ms-appx:/Assets/play.png", UriKind.Absolute));
            pauseImage = new BitmapImage(new Uri("ms-appx:/Assets/pause.png", UriKind.Absolute));
            pressedPlayImage = new BitmapImage(new Uri("ms-appx:/Assets/play_pressed.png", UriKind.Absolute));
            pressedPauseImage = new BitmapImage(new Uri("ms-appx:/Assets/pause_pressed.png", UriKind.Absolute));
        }
    }
}
