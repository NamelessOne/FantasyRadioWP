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
                if(value==false)
                {
                    CurrentTitle = "";
                    if (CurrentRecStatus)
                        CurrentRecStatus = false;
                }
            }
        }

        private ImageSource recImage;
        private ImageSource recActiveImage;

        public ImageSource RecImage
        {
            get
            {
                if (CurrentRecStatus)
                    return recActiveImage;
                else
                    return recImage;
            }
        }

        private bool currentRecStatus;
        public bool CurrentRecStatus
        {
            get
            {
                return currentRecStatus;
            }
            set
            {
                currentRecStatus = value;
                Notify("RecImage");
            }
        }

        private SolidColorBrush BITRATE_ELEMENT_COLOR = new SolidColorBrush(Color.FromArgb(255, 91, 91, 91));
        private SolidColorBrush BITRATE_ELEMENT_COLOR_ACTIVE = new SolidColorBrush(Color.FromArgb(255, 12, 100, 140));

        public SolidColorBrush Bitrate1Color {
            get
            {
                if (CurrentBitrate == Bitrates.AAC16)
                    return BITRATE_ELEMENT_COLOR_ACTIVE;
                else
                    return BITRATE_ELEMENT_COLOR;
            }
        }
        public SolidColorBrush Bitrate2Color
        {
            get
            {
                if (CurrentBitrate == Bitrates.MP332)
                    return BITRATE_ELEMENT_COLOR_ACTIVE;
                else
                    return BITRATE_ELEMENT_COLOR;
            }
        }
        public SolidColorBrush Bitrate3Color
        {
            get
            {
                if (CurrentBitrate == Bitrates.MP364)
                    return BITRATE_ELEMENT_COLOR_ACTIVE;
                else
                    return BITRATE_ELEMENT_COLOR;
            }
        }
        public SolidColorBrush Bitrate4Color
        {
            get
            {
                if (CurrentBitrate == Bitrates.AAC112)
                    return BITRATE_ELEMENT_COLOR_ACTIVE;
                else
                    return BITRATE_ELEMENT_COLOR;
            }
        }
        public SolidColorBrush Bitrate5Color
        {
            get
            {
                if (CurrentBitrate == Bitrates.MP396)
                    return BITRATE_ELEMENT_COLOR_ACTIVE;
                else
                    return BITRATE_ELEMENT_COLOR;
            }
        }

        private Bitrates currentBitrate = Bitrates.AAC16;
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
                Notify("Bitrate3Color");
                Notify("Bitrate4Color");
                Notify("Bitrate5Color");
            }
        }

        public string getCurrentBitrateUrl()
        {
            switch(CurrentBitrate)
            {
                case Bitrates.AAC16:
                    return LocalizedStrings.Instance.getString("stream_url_AAC16");
                case Bitrates.MP332:
                    return LocalizedStrings.Instance.getString("stream_url_MP332");
                case Bitrates.MP364:
                    return LocalizedStrings.Instance.getString("stream_url_MP364");
                case Bitrates.AAC112:
                    return LocalizedStrings.Instance.getString("stream_url_AAC112");
                case Bitrates.MP396:
                    return LocalizedStrings.Instance.getString("stream_url_MP396");
                default:
                    return LocalizedStrings.Instance.getString("stream_url_AAC16");
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
            recImage = new BitmapImage(new Uri("ms-appx:/Assets/rec.png", UriKind.Absolute));
            recActiveImage = new BitmapImage(new Uri("ms-appx:/Assets/rec_active.png", UriKind.Absolute));
        }
    }
}
