using FantasyRadio.DataModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace FantasyRadio
{
    class SavedManager : INotifyPropertyChanged
    {       
        private ObservableCollection<SavedAudioEntity> savedItems = new ObservableCollection<SavedAudioEntity>();
        public ObservableCollection<SavedAudioEntity> Items
        {
            get
            {
                return savedItems;
            }
            set
            {
                savedItems.Clear();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        savedItems.Add(item);
                    }
                }
                Notify("Items");
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

        public void ReloadItems()
        {
            var storageFolder = ApplicationData.Current.LocalFolder; //мб RoamingFolder?   
            var createStorageFolderTask = storageFolder.CreateFolderAsync(Constants.SAVED_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
            createStorageFolderTask.AsTask().Wait();
            Items = ListFilesInFolder(createStorageFolderTask.GetResults(), 1);
        }

        // Continue recursive enumeration of files and folders.
        private ObservableCollection<SavedAudioEntity> ListFilesInFolder(StorageFolder folder, int indentationLevel)
        {
            var result = new ObservableCollection<SavedAudioEntity>();

            // Get the files in the current folder.
            var getFilesTask = folder.GetFilesAsync().AsTask();
            getFilesTask.Wait();
            var filesInFolder = getFilesTask.Result;
            foreach (StorageFile currentFile in filesInFolder)
            {
                var savedEntity = new SavedAudioEntity(currentFile.Name, currentFile.Path, currentFile.Path, currentFile.Name);
                result.Add(savedEntity);
            }

            return result;
        }

        public string CurrentMP3Entity { get; set; }

        private ImageSource playImage;
        private ImageSource pauseImage;

        public ImageSource PlayPauseImage
        {
            get
            {
                if (CurrentPlayStatus==PlayStatus.Play)
                    return pauseImage;
                else
                    return playImage;
            }
        }

        private PlayStatus currentPlayStatus = PlayStatus.Stop;
        public PlayStatus CurrentPlayStatus
        {
            get
            {
                return currentPlayStatus;
            }
            set
            {
                currentPlayStatus = value;
                Notify("PlayPauseImage");
                //Notify("PressedPlayPauseImage");
                switch(value)
                {
                    case PlayStatus.Stop:
                        CurrentMP3Entity = null;
                        break;
                    case PlayStatus.Pause:
                        break;
                    case PlayStatus.Play:
                        Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = false;
                        break;
                }
            }
        }

        public SavedManager()
        {
            playImage = new BitmapImage(new Uri("ms-appx:/Assets/play.png", UriKind.Absolute));
            pauseImage = new BitmapImage(new Uri("ms-appx:/Assets/pause.png", UriKind.Absolute));
        }

        public enum PlayStatus
        {
            Play,
            Stop,
            Pause
        }
    }
}
