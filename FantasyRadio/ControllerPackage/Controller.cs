using Windows.UI.Xaml;

namespace FantasyRadio
{
    class Controller
    {
        private static Controller instance;
        private readonly static object syncLock = new object();
        public RadioManager CurrentRadioManager { get; } = new RadioManager();
        public BassManager CurrentBassManager { get; } = new BassManager();
        public ScheduleManager CurrentScheduleManager { get; } = new ScheduleManager();
        public ArchiveManager CurrentArchiveManager { get; } = new ArchiveManager();
        public SavedManager CurrentSavedManager { get; } = new SavedManager();
        public ResourceDictionary ResourceDict { get; set; }

        private Controller() { }

        public static Controller getInstance()
        {
            if (instance == null)
            {
                lock (syncLock)
                {
                    if (instance == null)
                    {
                        instance = new Controller();
                    }
                }
            }
            return instance;
        }

        public bool IsPlaying
        {
            get
            {
                //return CurrentSavedManager.CurrentPlayStatus == SavedManager.PlayStatus.Play || CurrentRadioManager.CurrentPlayStatus;
                return Bass.BASS.BASS_ChannelIsActive(CurrentBassManager.Chan) == Bass.BASS.BASS_ACTIVE_PLAYING | Bass.BASS.BASS_ChannelIsActive(CurrentBassManager.Chan) == Bass.BASS.BASS_ACTIVE_STALLED;
            }
        }
    }
}
