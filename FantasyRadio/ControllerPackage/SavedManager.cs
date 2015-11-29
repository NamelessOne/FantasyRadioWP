using FantasyRadio.DataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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
            var createStorageFolderTask = storageFolder.CreateFolderAsync("saved", CreationCollisionOption.OpenIfExists);
            createStorageFolderTask.AsTask().Wait();
            Items = ListFilesInFolder(createStorageFolderTask.GetResults(), 1);
        }

        private const string FOLDER_PREFIX = "";
        private const int PADDING_FACTOR = 3; //TODO вроде и нафиг не надо
        private const char SPACE = ' ';
        private static StringBuilder folderContents = new StringBuilder();

        // Continue recursive enumeration of files and folders.
        private ObservableCollection<SavedAudioEntity> ListFilesInFolder(StorageFolder folder, int indentationLevel)
        {
            return new ObservableCollection<SavedAudioEntity>();
            /*string indentationPadding = String.Empty.PadRight(indentationLevel * PADDING_FACTOR, SPACE);

            // Get the subfolders in the current folder.
            var foldersInFolder = await folder.GetFoldersAsync();
            // Increase the indentation level of the output.
            int childIndentationLevel = indentationLevel + 1;
            // For each subfolder, call this method again recursively.
            foreach (StorageFolder currentChildFolder in foldersInFolder)
            {
                folderContents.AppendLine(indentationPadding + FOLDER_PREFIX + currentChildFolder.Name);
                await ListFilesInFolder(currentChildFolder, childIndentationLevel);
            }

            // Get the files in the current folder.
            var filesInFolder = await folder.GetFilesAsync();
            foreach (StorageFile currentFile in filesInFolder)
            {
                folderContents.AppendLine(indentationPadding + currentFile.Name);
            }*/
        }
    }
}
