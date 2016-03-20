using FantasyRadio.DataModel;
using FantasyRadio.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace FantasyRadio
{
    class SavedManager : INotifyPropertyChanged
    {
        private ObservableCollection<SavedAudioEntity> savedItems = new ObservableCollectionEx<SavedAudioEntity>();
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
            //TODO сохранять у старых итемов свойство Playing
            string nowPlaying = "";
            foreach (var entity in Items)
            {
                if (entity.Playing)
                {
                    nowPlaying = entity.Title;
                    break;
                }
            }
            var storageFolder = ApplicationData.Current.LocalFolder; //мб RoamingFolder?   
            var createStorageFolderTask = storageFolder.CreateFolderAsync(Constants.SAVED_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
            createStorageFolderTask.AsTask().Wait();
            Items = ListFilesInFolder(createStorageFolderTask.GetResults(), 1);
            foreach(var item in Items)
            {
                if(item.Title.Equals(nowPlaying))
                {
                    item.Playing = true;
                    break;
                }
            }
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

        private PlayStatus currentPlayStatus = PlayStatus.Stop;
        public PlayStatus CurrentPlayStatus
        {
            get
            {
                return currentPlayStatus;
            }
            set
            {
                //TODO
                currentPlayStatus = value;
                foreach (var item in Items)
                {
                    item.Playing = false;
                }
                switch (value)
                {
                    case PlayStatus.Stop:
                        //CurrentMP3Entity = null;
                        break;
                    case PlayStatus.Pause:
                        break;
                    case PlayStatus.Play:
                        Controller.getInstance().CurrentRadioManager.CurrentPlayStatus = false;
                        foreach (var entity in savedItems)
                        {
                            if (entity.Title.Equals(CurrentMP3Entity))
                            {
                                entity.Playing = true;
                                break;
                            }
                        }
                        break;
                }
                Notify("Items");
            }
        }

        public SavedManager()
        {
            SavedAudioEntity.PlayImage = new BitmapImage(new Uri("ms-appx:/Assets/play.png", UriKind.Absolute));
            SavedAudioEntity.PauseImage = new BitmapImage(new Uri("ms-appx:/Assets/pause.png", UriKind.Absolute));
        }

        public enum PlayStatus
        {
            Play,
            Stop,
            Pause
        }
    }
}
