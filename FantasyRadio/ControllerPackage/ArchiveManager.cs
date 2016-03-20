using FantasyRadio.DataModel;
using FantasyRadio.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using System;

namespace FantasyRadio
{
    class ArchiveManager : INotifyPropertyChanged
    {
        private ArchiveParser parser = new ArchiveParser();
        private ObservableCollectionEx<ArchiveEntity> archiveItems = new ObservableCollectionEx<ArchiveEntity>();
        public string Login { get; set; }
        public string Password { get; set; }
        private bool isParsingActive;
        public bool IsParsingActive
        {
            get
            {
                return isParsingActive;
            }
            set
            {
                isParsingActive = value;
                Notify("IsParsingActive");
            }
        }
        public ObservableCollection<ArchiveEntity> Items
        {
            get
            {
                return archiveItems;
            }
            set
            {
                archiveItems.Clear();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        archiveItems.Add(item);
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

        public Task<List<ArchiveEntity>> ParseArchiveAsync()
        {
            return parser.ParseArchiveAsync(Login, Password);
        }

        public Task<bool> saveMp3Async(string url)
        {
            return Task.Run(() => saveMp3(url));
        }

        private bool saveMp3(string url)
        {
            try
            {
                var urlParts = url.Split('/');
                string filename = urlParts[urlParts.Length - 1];
                using (HttpClient http = new HttpClient())
                {
                    http.DefaultRequestHeaders.Add("User-Agent",
                                     "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0) Gecko/20100101 Firefox/5.0");
                    Task<Stream> response = http.GetStreamAsync(url);
                    response.Wait();
                    using (var data = response.Result)
                    {
                        var storageFolder = ApplicationData.Current.LocalFolder; //мб RoamingFolder?   
                        var createStorageFolderTask = storageFolder.CreateFolderAsync(Constants.SAVED_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
                        createStorageFolderTask.AsTask().Wait();
                        var mp3sFolder = createStorageFolderTask.GetResults();
                        var createFileTask = mp3sFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                        createFileTask.AsTask().Wait();
                        var file = createFileTask.GetResults();
                        var openStreamTask = file.OpenStreamForWriteAsync();
                        openStreamTask.Wait();
                        using (var outstream = openStreamTask.Result)
                        {
                            var copyTask = data.CopyToAsync(outstream);
                            copyTask.Wait();
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public List<string> RunningDownloads { get; set; } = new List<string>();
    }
}
