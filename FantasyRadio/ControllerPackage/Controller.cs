namespace FantasyRadio
{
    class Controller
    {
        private static Controller instance;
        private readonly static object syncLock = new object();
        public RadioManager CurrentRadioManager { get; } = new RadioManager();
        public BassManager CurrentBassManager { get; } = new BassManager();
        public ScheduleManager CurrentScheduleManager { get; } = new ScheduleManager();

        private Controller() {}

        public static Controller getInstance()
        {
            if(instance==null)
            {
                lock(syncLock)
                {
                    if(instance==null)
                    {
                        instance = new Controller();
                    }
                }
            }
            return instance;
        } 
    }
}
