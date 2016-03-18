using FantasyRadio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Storage;
using System.Linq;
using FantasyRadioPlaylistManager.Tools;
using Windows.Media.Core;

namespace FantasyRadioPlaylistManager
{
    public sealed class FantasyRadioPlayListManager
    {
        private static FRPlaylist instance;

        public FRPlaylist Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new FRPlaylist();
                }
                return instance;
            }
        }

        /// <summary>
        /// Clears playlist for re-initialization
        /// </summary>
        public void ClearPlaylist()
        {
            instance = null;
        }
    }

    /// <summary>
    /// Implement a playlist of tracks. 
    /// If instantiated in background task, it will keep on playing once app is suspended
    /// </summary>
    public sealed class FRPlaylist
    {
        static string[] Tracks
        {
            get
            {
                //TODO вероятно, излишне делать это каждый раз. Кроме приложения файлы эти менять вроде всё равно никто не сможет:/
                var storageFolder = ApplicationData.Current.LocalFolder; //мб RoamingFolder?   
                var createStorageFolderTask = storageFolder.CreateFolderAsync(Constants.SAVED_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
                createStorageFolderTask.AsTask().Wait();
                return ListFilesInFolder(createStorageFolderTask.GetResults());
            }
        }

        int CurrentTrackId = -1;
        private MediaPlayer mediaPlayer;
        private TimeSpan startPosition = TimeSpan.FromSeconds(0);
        internal FRPlaylist()
        {
            mediaPlayer = BackgroundMediaPlayer.Current;
            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            mediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
            mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
        }


        /// <summary>
        /// Get the current track name
        /// </summary>
        public string CurrentTrackName
        {
            get
            {
                if (CurrentTrackId == -1)
                {
                    return String.Empty;
                }
                if (CurrentTrackId < Tracks.Length)
                {
                    return Tracks[CurrentTrackId];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Track Id is higher than total number of tracks");
                }
            }
        }
        /// <summary>
        /// Invoked when the media player is ready to move to next track
        /// </summary>
        public event TypedEventHandler<FRPlaylist, object> TrackChanged;

        /// <summary>
        /// Handler for state changed event of Media Player
        /// </summary>
        void mediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {

            if (sender.CurrentState == MediaPlayerState.Playing && startPosition != TimeSpan.FromSeconds(0))
            {
                // if the start position is other than 0, then set it now
                sender.Position = startPosition;
                sender.Volume = 1.0;
                startPosition = TimeSpan.FromSeconds(0);
                sender.PlaybackMediaMarkers.Clear();
            }
        }

        /// <summary>
        /// Fired when MediaPlayer is ready to play the track
        /// </summary>
        void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            // wait for media to be ready
            sender.Play();
            Debug.WriteLine("New Track" + this.CurrentTrackName);
            TrackChanged.Invoke(this, CurrentTrackName);
        }

        /// <summary>
        /// Handler for MediaPlayer Media Ended
        /// </summary>
        private void MediaPlayer_MediaEnded(MediaPlayer sender, object args)
        {
            SkipToNext();
        }

        private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine("Failed with error code " + args.ExtendedErrorCode.ToString());
            //TODO обрабатывать зацикливания
            //SkipToNext();
        }

        /// <summary>
        /// Starts track at given position in the track list
        /// </summary>
        public void StartTrackAt(int id)
        {
            if (id >= Tracks.Length)
            {
                throw new ArgumentOutOfRangeException("tracks.Length = " + Tracks.Length + ", id = " + id);
            }
            CurrentTrackId = id;
            var TrackName = Tracks[id];
            mediaPlayer.AutoPlay = false;
            mediaPlayer.SetFileSource(getFileByName(TrackName));
        }

        /// <summary>
        /// Starts a given track by finding its name
        /// </summary>Failed with error code
        public void StartTrackAt(string TrackName)
        {
            CurrentTrackId = Tracks.ToList().FindIndex(x => x.Equals(TrackName));
            mediaPlayer.AutoPlay = false;
            //mediaPlayer.SetFileSource(getFileByName(TrackName));
            //var s = new ShoutcastStream();
            //var conenctTask = s.ConnectAsync(new Uri("http://fantasyradioru.no-ip.biz:8002/live"));
            //conenctTask.Wait();
            //mediaPlayer.SetStreamSource(s);
            mediaPlayer.SetUriSource(new Uri("http://fantasyradioru.no-ip.biz:8002/live"));
        }

        /// <summary>
        /// Starts a given track by finding its name and at desired position
        /// </summary>
        public void StartTrackAt(string TrackName, TimeSpan position)
        {
            for (int i = 0; i < Tracks.Length; i++)
            {
                if (Tracks[i].Contains(TrackName))
                {
                    CurrentTrackId = i;
                    break;
                }
            }

            mediaPlayer.AutoPlay = false;

            // Set the start position, we set the position once the state changes to playing, 
            // it can be possible for a fraction of second, playback can start before we are 
            // able to seek to new start position
            mediaPlayer.Volume = 0;
            startPosition = position;
            mediaPlayer.SetFileSource(getFileByName(Tracks[CurrentTrackId]));
        }

        /// <summary>
        /// Play all tracks in the list starting with 0 
        /// </summary>
        public void PlayAllTracks()
        {
            StartTrackAt(0);
        }

        /// <summary>
        /// Skip to next track
        /// </summary>
        public void SkipToNext()
        {
            StartTrackAt((CurrentTrackId + 1) % Tracks.Length);
        }

        /// <summary>
        /// Skip to next track
        /// </summary>
        public void SkipToPrevious()
        {
            if (CurrentTrackId == 0)
            {
                StartTrackAt(CurrentTrackId);
            }
            else
            {
                StartTrackAt(CurrentTrackId - 1);
            }
        }

        private IStorageFile getFileByName(string name)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;
            var createStorageFolderTask = storageFolder.CreateFolderAsync(Constants.SAVED_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
            createStorageFolderTask.AsTask().Wait();
            var folder = createStorageFolderTask.GetResults();
            var getFileTask = folder.GetFileAsync(name).AsTask();
            getFileTask.Wait();
            var result = getFileTask.Result;
            return result;
        }

        private static string[] ListFilesInFolder(StorageFolder folder)
        {
            var result = new List<string>();
            // Get the files in the current folder.
            var getFilesTask = folder.GetFilesAsync().AsTask();
            getFilesTask.Wait();
            var filesInFolder = getFilesTask.Result;
            foreach (StorageFile currentFile in filesInFolder)
            {
                result.Add(currentFile.Name);
            }
            return result.ToArray();
        }
    }
}
