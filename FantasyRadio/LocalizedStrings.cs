using Windows.ApplicationModel.Resources;

namespace FantasyRadio
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static readonly LocalizedStrings instance = new LocalizedStrings();

        private LocalizedStrings() { }

        public static LocalizedStrings Instance
        {
            get
            {
                return instance;
            }
        }

        ResourceLoader rl = new ResourceLoader();

        public string getString(string key)
        {
            return rl.GetString(key);
        }

    }
}
