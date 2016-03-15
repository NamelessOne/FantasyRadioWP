using System;
using System.IO;
using System.Runtime.InteropServices;
using Windows.Storage;

namespace FantasyRadio
{
    class BassManager
    {
        private int chan;
        private readonly string[] reservedChars = {@"\", @"/", "?", ":", "*", @"\", ">", "<", "|" };
        public int Chan
        {
            get
            {
                return chan;
            }
            set
            {
                chan = value;
            }
        }

        private Bass.BASS.DOWNLOADPROC downloadProc;

        public Bass.BASS.DOWNLOADPROC DownloadProc
        {
            get
            {
                if (downloadProc == null)
                    downloadProc = new Bass.BASS.DOWNLOADPROC(MyDownload);
                return downloadProc;
            }
            private set
            {
                downloadProc = value;
            }
        }

        private void MyDownload(IntPtr buffer, int length, IntPtr user)
        {
            //TODO
            if (Controller.getInstance().CurrentRadioManager.CurrentRecStatus)
            {
                byte[] ba = new byte[length];
                try
                {
                    Marshal.Copy(buffer, ba, 0, length);
                    var storageFolder = ApplicationData.Current.LocalFolder;   
                    var createStorageFolderTask = storageFolder.CreateFolderAsync(SavedManager.SAVED_FOLDER_NAME, CreationCollisionOption.OpenIfExists);
                    createStorageFolderTask.AsTask().Wait();
                    var mp3sFolder = createStorageFolderTask.GetResults();
                    string fileName = Controller.getInstance().CurrentRadioManager.CurrentTitle;
                    if (fileName == null || fileName.Length < 3)
                        fileName = "rec";
                    foreach (string c in reservedChars)
                    {
                        fileName = fileName.Replace(c, "_");
                    }
                    var createFileTask = mp3sFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                    createFileTask.AsTask().Wait();
                    var file = createFileTask.GetResults();
                    var openStreamTask = file.OpenStreamForWriteAsync();
                    openStreamTask.Wait();
                    using (var outstream = openStreamTask.Result)
                    {
                        outstream.Write(ba, 0, ba.Length);
                    }
                }
                catch (Exception e1)
                {

                }
            }
        }

        private Bass.BASS.BASS_FILEPROCS bassFileproc;

        public Bass.BASS.BASS_FILEPROCS FileProc
        {
            get
            {
                if (bassFileproc == null)
                    bassFileproc = new Bass.BASS.BASS_FILEPROCS(new Bass.BASS.FILECLOSEPROC(MyFileCloseProc), new Bass.BASS.FILELENPROC(MyFileLenProc), new Bass.BASS.FILEREADPROC(MyFileReadProc), new Bass.BASS.FILESEEKPROC(MyFileSeekProc));
                return bassFileproc;
            }
            private set
            {
                bassFileproc = value;
            }
        }

        private void MyFileCloseProc(IntPtr user)
        {

        }

        private long MyFileLenProc(IntPtr user)
        {
            return 0;
        }

        private int MyFileReadProc(IntPtr buffer, int length, IntPtr user)
        {
            return 0;
        }

        private bool MyFileSeekProc(long offset, IntPtr user)
        {
            return true;
        }
    }
}
