namespace FantasyRadio
{
    internal class Constants
    {
        public const string SAVED_FOLDER_NAME = "saved";
        public const string CurrentTrack = "trackname";
        public const string BackgroundTaskStarted = "BackgroundTaskStarted";
        public const string BackgroundTaskRunning = "BackgroundTaskRunning";
        public const string BackgroundTaskCancelled = "BackgroundTaskCancelled";
        public const string AppSuspended = "appsuspend";
        public const string AppResumed = "appresumed";
        public const string StartPlayback = "startplayback";
        public const string SkipNext = "skipnext";
        public const string Position = "position";
        public const string AppState = "appstate";
        public const string BackgroundTaskState = "backgroundtaskstate";
        public const string SkipPrevious = "skipprevious";
        public const string Trackchanged = "songchanged";
        public const string BufferingStarted = "bufstart";
        public const string BufferingEnded = "bufend";

        public const string Statechanged = "statechanged";
        public const string ForegroundAppActive = "Active";
        public const string ForegroundAppSuspended = "Suspended";
        public const string PlayFileByName = "playfilebyname";
        public const string PlayFileById = "playfilebyid";
        public const string PlayStream = "playstream";
        public static readonly string[] streamURLS = {
            "http://fantasyradioru.no-ip.biz:8008", //Через shoutcast работает (лолчто)
            "http://stream0.radiostyle.ru:8000/fantasy-radio",
            "http://fantasyradioru.no-ip.biz:8002/live", //Работает:/
        };
    }
}
