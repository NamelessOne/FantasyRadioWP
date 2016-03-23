using FantasyRadio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.Storage;
using System.Linq;
using FFmpegInterop;
using Windows.Media.Core;
using Windows.Media;

namespace FantasyRadioPlaylistManager
{
    public enum PlayerSource
    {
        Stream = 0,
        File = 1,
    }
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
        private PlayerSource _currentSource;
        public PlayerSource CurrentSource
        {
            get
            {
                return _currentSource;
            }
            private set
            {
                _currentSource = value;
                //var systemmediatransportcontrol = SystemMediaTransportControls.GetForCurrentView();
                //if (_currentSource==PlayerSource.Stream)
                //{
                //    systemmediatransportcontrol.IsNextEnabled = false;
                //    systemmediatransportcontrol.IsPreviousEnabled = false;
                //}
                //else
                //{
                //    systemmediatransportcontrol.IsNextEnabled = true;
                //    systemmediatransportcontrol.IsPreviousEnabled = true;
                //}
            }
        }
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

        int CurrentStreamId = -1;

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


        private string currentStreamName = Constants.streamURLS[0];
        /// <summary>
        /// Get the current track name
        /// </summary>
        public string CurrentTrackName
        {
            get
            {
                if (CurrentSource == PlayerSource.File)
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
                else
                    return currentStreamName;
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
            //TODO 
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
            if (CurrentSource == PlayerSource.File)
            {
                SkipToNext();
            }
            else
            {
                mediaPlayer.Pause();
            }
        }

        private void mediaPlayer_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            Debug.WriteLine("Failed with error code " + args.ExtendedErrorCode.ToString());
            //TODO обрабатывать зацикливания
            if (CurrentSource == PlayerSource.File)
            {
                SkipToNext();
            }
            else
            {
                mediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Starts track at given position in the track list
        /// </summary>
        public void StartTrackAt(int id)
        {
            CurrentSource = PlayerSource.File;
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
            CurrentSource = PlayerSource.File;
            CurrentTrackId = Tracks.ToList().FindIndex(x => x.Equals(TrackName));
            mediaPlayer.AutoPlay = false;
            mediaPlayer.SetFileSource(getFileByName(TrackName));
        }

        public void StartStream(string streamUrl)
        {
            CurrentSource = PlayerSource.Stream;
            CurrentStreamId = Constants.streamURLS.ToList().FindIndex(x => x.Equals(streamUrl));
            mediaPlayer.AutoPlay = false;
            try
            {
                currentStreamName = streamUrl;
                var FFmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromUri(streamUrl, true, false);
                MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();
                mediaPlayer.SetMediaSource(mss);
            }
            catch (Exception e)
            {
                mediaPlayer.Pause();
            }
        }

        public void StartStreamAt(int index)
        {
            CurrentSource = PlayerSource.Stream;
            if (index >= Constants.streamURLS.Length)
            {
                throw new ArgumentOutOfRangeException("streamURLs.Length = " + Constants.streamURLS.Length + ", id = " + index);
            }
            CurrentStreamId = index;
            var URL = Constants.streamURLS[index];
            mediaPlayer.AutoPlay = false;
            try
            {
                currentStreamName = URL;
                var FFmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromUri(URL, true, false);
                MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();
                mediaPlayer.SetMediaSource(mss);
            }
            catch (Exception e)
            {
                mediaPlayer.Pause();
            }
        }

        /// <summary>
        /// Starts a given track by finding its name and at desired position
        /// </summary>
        public void StartTrackAt(string TrackName, TimeSpan position)
        {
            CurrentSource = PlayerSource.File;
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
            CurrentSource = PlayerSource.Stream;
            StartTrackAt(0);
        }

        /// <summary>
        /// Skip to next track
        /// </summary>
        public void SkipToNext()
        {
            if (CurrentSource == PlayerSource.File)
            {
                StartTrackAt((CurrentTrackId + 1) % Tracks.Length);
            }
            else
            {
                StartStreamAt((CurrentStreamId + 1) % Constants.streamURLS.Length);
            }
        }

        /// <summary>
        /// Skip to next track
        /// </summary>
        public void SkipToPrevious()
        {
            if (CurrentSource == PlayerSource.File)
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
            else
            {
                if (CurrentStreamId == 0)
                {
                    StartStreamAt(CurrentStreamId);
                }
                else
                {
                    StartStreamAt(CurrentTrackId - 1);
                }
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
