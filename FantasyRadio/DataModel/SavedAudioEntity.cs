namespace FantasyRadio.DataModel
{
    class SavedAudioEntity
    {
        public string Title { get; private set; }
        public string Artist { get; private set; }
        public string Time { get; private set; }
        public string Directory { get; private  set; }
        public SavedAudioEntity(string title, string artist, string time, string directory)
        {
            Artist = artist;
            Directory = directory;
            Time = time;
            Title = title;
        }
    }
}
