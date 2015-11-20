namespace FantasyRadio
{
    class Controller
    {
        private static Controller instance;
        private readonly static object syncLock = new object();
        public RadioManager RadioManager { get; } = new RadioManager();
        public BassManager BassManager { get; } = new BassManager();

        private Controller()
        {
        }

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
