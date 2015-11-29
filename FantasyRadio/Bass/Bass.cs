using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasyRadio.Bass
{
    public class BASS
    {
        public const int BASSVERSION = 0x204;    // API version
        public const string BASSVERSIONTEXT = "2.4";

        // Error codes returned by BASS_ErrorGetCode
        public const int BASS_OK = 0;    // all is OK
        public const int BASS_ERROR_MEM = 1; // memory error
        public const int BASS_ERROR_FILEOPEN = 2;    // can't open the file
        public const int BASS_ERROR_DRIVER = 3;  // can't find a free/valid driver
        public const int BASS_ERROR_BUFLOST = 4; // the sample buffer was lost
        public const int BASS_ERROR_HANDLE = 5;  // invalid handle
        public const int BASS_ERROR_FORMAT = 6;  // unsupported sample format
        public const int BASS_ERROR_POSITION = 7;    // invalid position
        public const int BASS_ERROR_INIT = 8;    // BASS_Init has not been successfully called
        public const int BASS_ERROR_START = 9;   // BASS_Start has not been successfully called
        public const int BASS_ERROR_ALREADY = 14;    // already initialized/paused/whatever
        public const int BASS_ERROR_NOCHAN = 18; // can't get a free channel
        public const int BASS_ERROR_ILLTYPE = 19;    // an illegal type was specified
        public const int BASS_ERROR_ILLPARAM = 20;   // an illegal parameter was specified
        public const int BASS_ERROR_NO3D = 21;   // no 3D support
        public const int BASS_ERROR_NOEAX = 22;  // no EAX support
        public const int BASS_ERROR_DEVICE = 23; // illegal device number
        public const int BASS_ERROR_NOPLAY = 24; // not playing
        public const int BASS_ERROR_FREQ = 25;   // illegal sample rate
        public const int BASS_ERROR_NOTFILE = 27;    // the stream is not a file stream
        public const int BASS_ERROR_NOHW = 29;   // no hardware voices available
        public const int BASS_ERROR_EMPTY = 31;  // the MOD music has no sequence data
        public const int BASS_ERROR_NONET = 32;  // no internet connection could be opened
        public const int BASS_ERROR_CREATE = 33; // couldn't create the file
        public const int BASS_ERROR_NOFX = 34;   // effects are not available
        public const int BASS_ERROR_NOTAVAIL = 37;   // requested data is not available
        public const int BASS_ERROR_DECODE = 38; // the channel is a "decoding channel"
        public const int BASS_ERROR_DX = 39; // a sufficient DirectX version is not installed
        public const int BASS_ERROR_TIMEOUT = 40;    // connection timedout
        public const int BASS_ERROR_FILEFORM = 41;   // unsupported file format
        public const int BASS_ERROR_SPEAKER = 42;    // unavailable speaker
        public const int BASS_ERROR_VERSION = 43;    // invalid BASS version (used by add-ons)
        public const int BASS_ERROR_CODEC = 44;  // codec is not available/supported
        public const int BASS_ERROR_ENDED = 45;  // the channel/file has ended
        public const int BASS_ERROR_BUSY = 46;   // the device is busy
        public const int BASS_ERROR_UNKNOWN = -1;    // some other mystery problem

        public const int BASS_ERROR_JAVA_CLASS = 2000;   // object class problem

        // BASS_SetConfig options
        public const int BASS_CONFIG_BUFFER = 0;
        public const int BASS_CONFIG_UPDATEPERIOD = 1;
        public const int BASS_CONFIG_GVOL_SAMPLE = 4;
        public const int BASS_CONFIG_GVOL_STREAM = 5;
        public const int BASS_CONFIG_GVOL_MUSIC = 6;
        public const int BASS_CONFIG_CURVE_VOL = 7;
        public const int BASS_CONFIG_CURVE_PAN = 8;
        public const int BASS_CONFIG_FLOATDSP = 9;
        public const int BASS_CONFIG_3DALGORITHM = 10;
        public const int BASS_CONFIG_NET_TIMEOUT = 11;
        public const int BASS_CONFIG_NET_BUFFER = 12;
        public const int BASS_CONFIG_PAUSE_NOPLAY = 13;
        public const int BASS_CONFIG_NET_PREBUF = 15;
        public const int BASS_CONFIG_NET_PASSIVE = 18;
        public const int BASS_CONFIG_REC_BUFFER = 19;
        public const int BASS_CONFIG_NET_PLAYLIST = 21;
        public const int BASS_CONFIG_MUSIC_VIRTUAL = 22;
        public const int BASS_CONFIG_VERIFY = 23;
        public const int BASS_CONFIG_UPDATETHREADS = 24;
        public const int BASS_CONFIG_DEV_BUFFER = 27;
        public const int BASS_CONFIG_DEV_DEFAULT = 36;
        public const int BASS_CONFIG_NET_READTIMEOUT = 37;
        public const int BASS_CONFIG_HANDLES = 41;
        public const int BASS_CONFIG_SRC = 43;
        public const int BASS_CONFIG_SRC_SAMPLE = 44;

        // BASS_SetConfigPtr options
        public const int BASS_CONFIG_NET_AGENT = 16;
        public const int BASS_CONFIG_NET_PROXY = 17;

        // BASS_Init flags
        public const int BASS_DEVICE_8BITS = 1;  // 8 bit resolution, else 16 bit
        public const int BASS_DEVICE_MONO = 2;   // mono, else stereo
        public const int BASS_DEVICE_3D = 4; // enable 3D functionality
        public const int BASS_DEVICE_LATENCY = 0x100;    // calculate device latency (BASS_INFO struct)
        public const int BASS_DEVICE_SPEAKERS = 0x800; // force enabling of speaker assignment
        public const int BASS_DEVICE_NOSPEAKER = 0x1000; // ignore speaker arrangement
        public const int BASS_DEVICE_FREQ = 0x4000; // set device sample rate

        // Device info structure
        public struct BASS_DEVICEINFO
        {
            public string name; // description
            public string driver;   // driver
            public int flags;
        }

        // BASS_DEVICEINFO flags
        public const int BASS_DEVICE_ENABLED = 1;
        public const int BASS_DEVICE_DEFAULT = 2;
        public const int BASS_DEVICE_INIT = 4;

        public struct BASS_INFO
        {
            public int flags;   // device capabilities (DSCAPS_xxx flags)
            public int hwsize;  // size of total device hardware memory
            public int hwfree;  // size of free device hardware memory
            public int freesam; // number of free sample slots in the hardware
            public int free3d;  // number of free 3D sample slots in the hardware
            public int minrate; // min sample rate supported by the hardware
            public int maxrate; // max sample rate supported by the hardware
            public int eax;     // device supports EAX? (always FALSE if BASS_DEVICE_3D was not used)
            public int minbuf;  // recommended minimum buffer length in ms (requires BASS_DEVICE_LATENCY)
            public int dsver;   // DirectSound version
            public int latency; // delay (in ms) before start of playback (requires BASS_DEVICE_LATENCY)
            public int initflags; // BASS_Init "flags" parameter
            public int speakers; // number of speakers available
            public int freq;        // current output rate
        }

        // Recording device info structure
        public struct BASS_RECORDINFO
        {
            public int flags;   // device capabilities (DSCCAPS_xxx flags)
            public int formats; // supported standard formats (WAVE_FORMAT_xxx flags)
            public int inputs;  // number of inputs
            public bool singlein;    // TRUE = only 1 input can be set at a time
            public int freq;        // current input rate
        }

        // Sample info structure
        public struct BASS_SAMPLE
        {
            public int freq;        // default playback rate
            public float volume;    // default volume (0-1)
            public float pan;       // default pan (-1=left, 0=middle, 1=right)
            public int flags;   // BASS_SAMPLE_xxx flags
            public int length;  // length (in bytes)
            public int max;     // maximum simultaneous playbacks
            public int origres; // original resolution bits
            public int chans;   // number of channels
            public int mingap;  // minimum gap (ms) between creating channels
            public int mode3d;  // BASS_3DMODE_xxx mode
            public float mindist;   // minimum distance
            public float maxdist;   // maximum distance
            public int iangle;  // angle of inside projection cone
            public int oangle;  // angle of outside projection cone
            public float outvol;    // delta-volume outside the projection cone
            public int vam;     // voice allocation/management flags (BASS_VAM_xxx)
            public int priority;    // priority (0=lowest, 0xffffffff=highest)
        }

        public const int BASS_SAMPLE_8BITS = 1;      // 8 bit
        public const int BASS_SAMPLE_FLOAT = 256;    // 32-bit floating-point
        public const int BASS_SAMPLE_MONO = 2;       // mono
        public const int BASS_SAMPLE_LOOP = 4;       // looped
        public const int BASS_SAMPLE_3D = 8;         // 3D functionality
        public const int BASS_SAMPLE_SOFTWARE = 16;  // not using hardware mixing
        public const int BASS_SAMPLE_MUTEMAX = 32;   // mute at max distance (3D only)
        public const int BASS_SAMPLE_VAM = 64;       // DX7 voice allocation & management
        public const int BASS_SAMPLE_FX = 128;       // old implementation of DX8 effects
        public const int BASS_SAMPLE_OVER_VOL = 0x10000; // override lowest volume
        public const int BASS_SAMPLE_OVER_POS = 0x20000; // override longest playing
        public const int BASS_SAMPLE_OVER_DIST = 0x30000; // override furthest from listener (3D only)

        public const int BASS_STREAM_PRESCAN = 0x20000;  // enable pin-point seeking/length (MP3/MP2/MP1)
        public const int BASS_MP3_SETPOS = BASS_STREAM_PRESCAN;
        public const int BASS_STREAM_AUTOFREE = 0x40000; // automatically free the stream when it stop/ends
        public const int BASS_STREAM_RESTRATE = 0x80000; // restrict the download rate of internet file streams
        public const int BASS_STREAM_BLOCK = 0x100000;   // download/play internet file stream in small blocks
        public const int BASS_STREAM_DECODE = 0x200000;  // don't play the stream, only decode (BASS_ChannelGetData)
        public const int BASS_STREAM_STATUS = 0x800000;  // give server status info (HTTP/ICY tags) in DOWNLOADPROC

        public const int BASS_MUSIC_FLOAT = BASS_SAMPLE_FLOAT;
        public const int BASS_MUSIC_MONO = BASS_SAMPLE_MONO;
        public const int BASS_MUSIC_LOOP = BASS_SAMPLE_LOOP;
        public const int BASS_MUSIC_3D = BASS_SAMPLE_3D;
        public const int BASS_MUSIC_FX = BASS_SAMPLE_FX;
        public const int BASS_MUSIC_AUTOFREE = BASS_STREAM_AUTOFREE;
        public const int BASS_MUSIC_DECODE = BASS_STREAM_DECODE;
        public const int BASS_MUSIC_PRESCAN = BASS_STREAM_PRESCAN;   // calculate playback length
        public const int BASS_MUSIC_CALCLEN = BASS_MUSIC_PRESCAN;
        public const int BASS_MUSIC_RAMP = 0x200;    // normal ramping
        public const int BASS_MUSIC_RAMPS = 0x400;   // sensitive ramping
        public const int BASS_MUSIC_SURROUND = 0x800;    // surround sound
        public const int BASS_MUSIC_SURROUND2 = 0x1000;  // surround sound (mode 2)
        public const int BASS_MUSIC_FT2MOD = 0x2000; // play .MOD as FastTracker 2 does
        public const int BASS_MUSIC_PT1MOD = 0x4000; // play .MOD as ProTracker 1 does
        public const int BASS_MUSIC_NONINTER = 0x10000;  // non-interpolated sample mixing
        public const int BASS_MUSIC_SINCINTER = 0x800000; // sinc interpolated sample mixing
        public const int BASS_MUSIC_POSRESET = 0x8000;   // stop all notes when moving position
        public const int BASS_MUSIC_POSRESETEX = 0x400000; // stop all notes and reset bmp/etc when moving position
        public const int BASS_MUSIC_STOPBACK = 0x80000;  // stop the music on a backwards jump effect
        public const int BASS_MUSIC_NOSAMPLE = 0x100000; // don't load the samples

        // Speaker assignment flags
        public const int BASS_SPEAKER_FRONT = 0x1000000; // front speakers
        public const int BASS_SPEAKER_REAR = 0x2000000;  // rear/side speakers
        public const int BASS_SPEAKER_CENLFE = 0x3000000;    // center & LFE speakers (5.1)
        public const int BASS_SPEAKER_REAR2 = 0x4000000; // rear center speakers (7.1)
        public static int BASS_SPEAKER_N(int n) { return n << 24; } // n'th pair of speakers (max 15)
        public const int BASS_SPEAKER_LEFT = 0x10000000; // modifier: left
        public const int BASS_SPEAKER_RIGHT = 0x20000000;    // modifier: right
        public const int BASS_SPEAKER_FRONTLEFT = BASS_SPEAKER_FRONT | BASS_SPEAKER_LEFT;
        public const int BASS_SPEAKER_FRONTRIGHT = BASS_SPEAKER_FRONT | BASS_SPEAKER_RIGHT;
        public const int BASS_SPEAKER_REARLEFT = BASS_SPEAKER_REAR | BASS_SPEAKER_LEFT;
        public const int BASS_SPEAKER_REARRIGHT = BASS_SPEAKER_REAR | BASS_SPEAKER_RIGHT;
        public const int BASS_SPEAKER_CENTER = BASS_SPEAKER_CENLFE | BASS_SPEAKER_LEFT;
        public const int BASS_SPEAKER_LFE = BASS_SPEAKER_CENLFE | BASS_SPEAKER_RIGHT;
        public const int BASS_SPEAKER_REAR2LEFT = BASS_SPEAKER_REAR2 | BASS_SPEAKER_LEFT;
        public const int BASS_SPEAKER_REAR2RIGHT = BASS_SPEAKER_REAR2 | BASS_SPEAKER_RIGHT;

        public const int BASS_RECORD_PAUSE = 0x8000; // start recording paused

        public const uint BASS_UNICODE = 0x80000000;

        // Channel info structure
        public class BASS_CHANNELINFO
        {
            public int freq;        // default playback rate
            public int chans;   // channels
            public int flags;   // BASS_SAMPLE/STREAM/MUSIC/SPEAKER flags
            public int ctype;   // type of channel
            public int origres; // original resolution
            public int plugin;  // plugin
            public int sample; // sample
            public string filename; // filename
        }

        // BASS_CHANNELINFO types
        public const int BASS_CTYPE_SAMPLE = 1;
        public const int BASS_CTYPE_RECORD = 2;
        public const int BASS_CTYPE_STREAM = 0x10000;
        public const int BASS_CTYPE_STREAM_OGG = 0x10002;
        public const int BASS_CTYPE_STREAM_MP1 = 0x10003;
        public const int BASS_CTYPE_STREAM_MP2 = 0x10004;
        public const int BASS_CTYPE_STREAM_MP3 = 0x10005;
        public const int BASS_CTYPE_STREAM_AIFF = 0x10006;
        public const int BASS_CTYPE_STREAM_CA = 0x10007;
        public const int BASS_CTYPE_STREAM_MF = 0x10008;
        public const int BASS_CTYPE_STREAM_WAV = 0x40000; // WAVE flag, LOWORD=codec
        public const int BASS_CTYPE_STREAM_WAV_PCM = 0x50001;
        public const int BASS_CTYPE_STREAM_WAV_FLOAT = 0x50003;
        public const int BASS_CTYPE_MUSIC_MOD = 0x20000;
        public const int BASS_CTYPE_MUSIC_MTM = 0x20001;
        public const int BASS_CTYPE_MUSIC_S3M = 0x20002;
        public const int BASS_CTYPE_MUSIC_XM = 0x20003;
        public const int BASS_CTYPE_MUSIC_IT = 0x20004;
        public const int BASS_CTYPE_MUSIC_MO3 = 0x00100; // MO3 flag

        public struct BASS_PLUGINFORM
        {
            public int ctype;       // channel type
            public String name; // format description
            public String exts; // file extension filter (*.ext1;*.ext2;etc...)
        }

        public struct BASS_PLUGININFO
        {
            public int version;                 // version (same form as BASS_GetVersion)
            public int formatc;                 // number of formats
            public BASS_PLUGINFORM[] formats;   // the array of formats
        }

        // 3D vector (for 3D positions/velocities/orientations)
        public class BASS_3DVECTOR //static
        {
            public BASS_3DVECTOR() { }
            public BASS_3DVECTOR(float _x, float _y, float _z) { x = _x; y = _y; z = _z; }
            public float x; // +=right, -=left
            public float y; // +=up, -=down
            public float z; // +=front, -=behind
        }

        // 3D channel modes
        public const int BASS_3DMODE_NORMAL = 0; // normal 3D processing
        public const int BASS_3DMODE_RELATIVE = 1;   // position is relative to the listener
        public const int BASS_3DMODE_OFF = 2;    // no 3D processing

        // software 3D mixing algorithms (used with BASS_CONFIG_3DALGORITHM)
        public const int BASS_3DALG_DEFAULT = 0;
        public const int BASS_3DALG_OFF = 1;
        public const int BASS_3DALG_FULL = 2;
        public const int BASS_3DALG_LIGHT = 3;

        public interface STREAMPROC
        {
            int STREAMPROC(int handle, MemoryStream buffer, int length, Object user);
            /* User stream callback function. NOTE: A stream function should obviously be as quick
            as possible, other streams (and MOD musics) can't be mixed until it's finished.
            handle : The stream that needs writing
            buffer : Buffer to write the samples in
            length : Number of bytes to write
            user   : The 'user' parameter value given when calling BASS_StreamCreate
            RETURN : Number of bytes written. Set the BASS_STREAMPROC_END flag to end
                     the stream. */
        }

        public const uint BASS_STREAMPROC_END = 0x80000000;   // end of user stream flag

        // special STREAMPROCs
        public const int STREAMPROC_DUMMY = 0;       // "dummy" stream
        public const int STREAMPROC_PUSH = -1;       // push stream

        // BASS_StreamCreateFileUser file systems
        public const int STREAMFILE_NOBUFFER = 0;
        public const int STREAMFILE_BUFFER = 1;
        public const int STREAMFILE_BUFFERPUSH = 2;

        public interface BASS_FILEPROCS
        {
            // User file stream callback functions
            void FILECLOSEPROC(Object user);
            long FILELENPROC(Object user);
            int FILEREADPROC(MemoryStream buffer, int length, Object user);
            bool FILESEEKPROC(long offset, Object user);
        }

        // BASS_StreamPutFileData options
        public const int BASS_FILEDATA_END = 0;  // end & close the file

        // BASS_StreamGetFilePosition modes
        public const int BASS_FILEPOS_CURRENT = 0;
        public const int BASS_FILEPOS_DECODE = BASS_FILEPOS_CURRENT;
        public const int BASS_FILEPOS_DOWNLOAD = 1;
        public const int BASS_FILEPOS_END = 2;
        public const int BASS_FILEPOS_START = 3;
        public const int BASS_FILEPOS_CONNECTED = 4;
        public const int BASS_FILEPOS_BUFFER = 5;
        public const int BASS_FILEPOS_SOCKET = 6;

        public interface DOWNLOADPROC
        {
            Delegate DOWNLOADPROC(IntPtr buffer, int length, IntPtr user);
            /* Internet stream download callback function.
            buffer : Buffer containing the downloaded data... NULL=end of download
            length : Number of bytes in the buffer
            user   : The 'user' parameter value given when calling BASS_StreamCreateURL */
        }

        // BASS_ChannelSetSync types
        public const int BASS_SYNC_POS = 0;
        public const int BASS_SYNC_END = 2;
        public const int BASS_SYNC_META = 4;
        public const int BASS_SYNC_SLIDE = 5;
        public const int BASS_SYNC_STALL = 6;
        public const int BASS_SYNC_DOWNLOAD = 7;
        public const int BASS_SYNC_FREE = 8;
        public const int BASS_SYNC_SETPOS = 11;
        public const int BASS_SYNC_MUSICPOS = 10;
        public const int BASS_SYNC_MUSICINST = 1;
        public const int BASS_SYNC_MUSICFX = 3;
        public const int BASS_SYNC_OGG_CHANGE = 12;
        public const uint BASS_SYNC_MIXTIME = 0x40000000; // FLAG: sync at mixtime, else at playtime
        public const uint BASS_SYNC_ONETIME = 0x80000000; // FLAG: sync only once, else continuously

        public interface SYNCPROC
        {
            void SYNCPROC(int handle, int channel, int data, Object user);
            /* Sync callback function. NOTE: a sync callback function should be very
            quick as other syncs can't be processed until it has finished. If the sync
            is a "mixtime" sync, then other streams and MOD musics can't be mixed until
            it's finished either.
            handle : The sync that has occured
            channel: Channel that the sync occured in
            data   : Additional data associated with the sync's occurance
            user   : The 'user' parameter given when calling BASS_ChannelSetSync */
        }

        public interface DSPPROC
        {
            void DSPPROC(int handle, int channel, MemoryStream buffer, int length, Object user);
            /* DSP callback function. NOTE: A DSP function should obviously be as quick as
            possible... other DSP functions, streams and MOD musics can not be processed
            until it's finished.
            handle : The DSP handle
            channel: Channel that the DSP is being applied to
            buffer : Buffer to apply the DSP to
            length : Number of bytes in the buffer
            user   : The 'user' parameter given when calling BASS_ChannelSetDSP */
        }

        public interface RECORDPROC
        {
            bool RECORDPROC(int handle, MemoryStream buffer, int length, Object user);
            /* Recording callback function.
            handle : The recording handle
            buffer : Buffer containing the recorded sample data
            length : Number of bytes
            user   : The 'user' parameter value given when calling BASS_RecordStart
            RETURN : true = continue recording, false = stop */
        }

        // BASS_ChannelIsActive return values
        public const int BASS_ACTIVE_STOPPED = 0;
        public const int BASS_ACTIVE_PLAYING = 1;
        public const int BASS_ACTIVE_STALLED = 2;
        public const int BASS_ACTIVE_PAUSED = 3;

        // Channel attributes
        public const int BASS_ATTRIB_FREQ = 1;
        public const int BASS_ATTRIB_VOL = 2;
        public const int BASS_ATTRIB_PAN = 3;
        public const int BASS_ATTRIB_EAXMIX = 4;
        public const int BASS_ATTRIB_NOBUFFER = 5;
        public const int BASS_ATTRIB_CPU = 7;
        public const int BASS_ATTRIB_SRC = 8;
        public const int BASS_ATTRIB_MUSIC_AMPLIFY = 0x100;
        public const int BASS_ATTRIB_MUSIC_PANSEP = 0x101;
        public const int BASS_ATTRIB_MUSIC_PSCALER = 0x102;
        public const int BASS_ATTRIB_MUSIC_BPM = 0x103;
        public const int BASS_ATTRIB_MUSIC_SPEED = 0x104;
        public const int BASS_ATTRIB_MUSIC_VOL_GLOBAL = 0x105;
        public const int BASS_ATTRIB_MUSIC_VOL_CHAN = 0x200; // + channel #
        public const int BASS_ATTRIB_MUSIC_VOL_INST = 0x300; // + instrument #

        // BASS_ChannelGetData flags
        public const int BASS_DATA_AVAILABLE = 0;            // query how much data is buffered
        public const int BASS_DATA_FLOAT = 0x40000000;   // flag: return floating-point sample data
        public const uint BASS_DATA_FFT256 = 0x80000000;  // 256 sample FFT
        public const uint BASS_DATA_FFT512 = 0x80000001;  // 512 FFT
        public const uint BASS_DATA_FFT1024 = 0x80000002; // 1024 FFT
        public const uint BASS_DATA_FFT2048 = 0x80000003; // 2048 FFT
        public const uint BASS_DATA_FFT4096 = 0x80000004; // 4096 FFT
        public const uint BASS_DATA_FFT8192 = 0x80000005; // 8192 FFT
        public const uint BASS_DATA_FFT16384 = 0x80000006;    // 16384 FFT
        public const int BASS_DATA_FFT_INDIVIDUAL = 0x10;    // FFT flag: FFT for each channel, else all combined
        public const int BASS_DATA_FFT_NOWINDOW = 0x20;  // FFT flag: no Hanning window
        public const int BASS_DATA_FFT_REMOVEDC = 0x40;  // FFT flag: pre-remove DC bias

        // BASS_ChannelGetTags types : what's returned
        public const int BASS_TAG_ID3 = 0;   // ID3v1 tags : TAG_ID3
        public const int BASS_TAG_ID3V2 = 1; // ID3v2 tags : ByteBuffer
        public const int BASS_TAG_OGG = 2;   // OGG comments : String array
        public const int BASS_TAG_HTTP = 3;  // HTTP headers : String array
        public const int BASS_TAG_ICY = 4;   // ICY headers : String array
        public const int BASS_TAG_META = 5;  // ICY metadata : String
        public const int BASS_TAG_APE = 6;   // APE tags : String array
        public const int BASS_TAG_MP4 = 7;   // MP4/iTunes metadata : String array
        public const int BASS_TAG_VENDOR = 9;    // OGG encoder : String
        public const int BASS_TAG_LYRICS3 = 10;  // Lyric3v2 tag : String
        public const int BASS_TAG_WAVEFORMAT = 14;   // WAVE format : ByteBuffer containing WAVEFORMATEEX structure
        public const int BASS_TAG_RIFF_INFO = 0x100; // RIFF "INFO" tags : String array
        public const int BASS_TAG_RIFF_BEXT = 0x101; // RIFF/BWF "bext" tags : TAG_BEXT
        public const int BASS_TAG_RIFF_CART = 0x102; // RIFF/BWF "cart" tags : TAG_CART
        public const int BASS_TAG_RIFF_DISP = 0x103; // RIFF "DISP" text tag : String
        public const int BASS_TAG_APE_BINARY = 0x1000;   // + index #, binary APE tag : TAG_APE_BINARY
        public const int BASS_TAG_MUSIC_NAME = 0x10000;  // MOD music name : String
        public const int BASS_TAG_MUSIC_MESSAGE = 0x10001;   // MOD message : String
        public const int BASS_TAG_MUSIC_ORDERS = 0x10002;    // MOD order list : ByteBuffer
        public const int BASS_TAG_MUSIC_INST = 0x10100;  // + instrument #, MOD instrument name : String
        public const int BASS_TAG_MUSIC_SAMPLE = 0x10300;    // + sample #, MOD sample name : String

        // ID3v1 tag structure
        public struct TAG_ID3
        {
            public String id;
            public String title;
            public String artist;
            public String album;
            public String year;
            public String comment;
            public byte genre;
            public byte track;
        }

        // Binary APE tag structure
        public struct TAG_APE_BINARY
        {
            public String key;
            public MemoryStream data;
            public int length;
        }

        // BASS_ChannelGetLength/GetPosition/SetPosition modes
        public const int BASS_POS_BYTE = 0;      // byte position
        public const int BASS_POS_MUSIC_ORDER = 1;       // order.row position, MAKELONG(order,row)
        public const int BASS_POS_DECODE = 0x10000000; // flag: get the decoding (not playing) position
        public const int BASS_POS_DECODETO = 0x20000000; // flag: decode to the position instead of seeking

        // DX8 effect types, use with BASS_ChannelSetFX
        public const int BASS_FX_DX8_CHORUS = 0;
        public const int BASS_FX_DX8_COMPRESSOR = 1;
        public const int BASS_FX_DX8_DISTORTION = 2;
        public const int BASS_FX_DX8_ECHO = 3;
        public const int BASS_FX_DX8_FLANGER = 4;
        public const int BASS_FX_DX8_GARGLE = 5;
        public const int BASS_FX_DX8_I3DL2REVERB = 6;
        public const int BASS_FX_DX8_PARAMEQ = 7;
        public const int BASS_FX_DX8_REVERB = 8;

        public struct BASS_DX8_CHORUS
        {
            public float fWetDryMix;
            public float fDepth;
            public float fFeedback;
            public float fFrequency;
            public int lWaveform;   // 0=triangle, 1=sine
            public float fDelay;
            public int lPhase;      // BASS_DX8_PHASE_xxx
        }

        public struct BASS_DX8_DISTORTION
        {
            public float fGain;
            public float fEdge;
            public float fPostEQCenterFrequency;
            public float fPostEQBandwidth;
            public float fPreLowpassCutoff;
        }

        public struct BASS_DX8_ECHO
        {
            public float fWetDryMix;
            public float fFeedback;
            public float fLeftDelay;
            public float fRightDelay;
            public bool lPanDelay;
        }

        public struct BASS_DX8_FLANGER
        {
            public float fWetDryMix;
            public float fDepth;
            public float fFeedback;
            public float fFrequency;
            public int lWaveform;   // 0=triangle, 1=sine
            public float fDelay;
            public int lPhase;      // BASS_DX8_PHASE_xxx
        }

        public struct BASS_DX8_PARAMEQ
        {
            public float fCenter;
            public float fBandwidth;
            public float fGain;
        }

        public struct BASS_DX8_REVERB
        {
            public float fInGain;
            public float fReverbMix;
            public float fReverbTime;
            public float fHighFreqRTRatio;
        }

        public const int BASS_DX8_PHASE_NEG_180 = 0;
        public const int BASS_DX8_PHASE_NEG_90 = 1;
        public const int BASS_DX8_PHASE_ZERO = 2;
        public const int BASS_DX8_PHASE_90 = 3;
        public const int BASS_DX8_PHASE_180 = 4;

        public class Asset
        {
            //public Asset() { }
            //public Asset(AssetManager m, string f) { manager = m; file = f; }
            //public AssetManager manager;
            public string file;
        }
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SetConfig(int option, int value);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_GetConfig(int option);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SetConfigPtr(uint option, string ptr);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern Object BASS_GetConfigPtr(int option);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_GetVersion();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ErrorGetCode();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_GetDeviceInfo(int device, BASS_DEVICEINFO info);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Init(int device, uint freq, uint flags/*, IntPtr win, IntPtr clsid*/);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SetDevice(int device);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_GetDevice();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Free();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_GetInfo(BASS_INFO info);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Update(int length);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern float BASS_GetCPU();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Start();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Stop();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Pause();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SetVolume(float volume);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern float BASS_GetVolume();

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_PluginLoad(String file, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_PluginFree(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern BASS_PLUGININFO BASS_PluginGetInfo(int handle);

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Set3DFactors(float distf, float rollf, float doppf);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Get3DFactors(float distf, float rollf, float doppf);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Set3DPosition(BASS_3DVECTOR pos, BASS_3DVECTOR vel, BASS_3DVECTOR front, BASS_3DVECTOR top);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_Get3DPosition(BASS_3DVECTOR pos, BASS_3DVECTOR vel, BASS_3DVECTOR front, BASS_3DVECTOR top);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern void BASS_Apply3D();

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_SampleLoad(string file, long offset, int length, int max, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_SampleLoad(MemoryStream file, long offset, int length, int max, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_SampleLoad(Asset file, long offset, int length, int max, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_SampleCreate(int length, int freq, int chans, int max, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SampleFree(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SampleSetData(int handle, MemoryStream buffer);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SampleGetData(int handle, MemoryStream buffer);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SampleGetInfo(int handle, BASS_SAMPLE info);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SampleSetInfo(int handle, BASS_SAMPLE info);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_SampleGetChannel(int handle, bool onlynew);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_SampleGetChannels(int handle, int[] channels);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_SampleStop(int handle);

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_MusicLoad(String file, long offset, int length, int flags, int freq);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_MusicLoad(MemoryStream file, long offset, int length, int flags, int freq);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_MusicLoad(Asset asset, long offset, int length, int flags, int freq);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_MusicFree(int handle);

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamCreate(int freq, int chans, int flags, STREAMPROC proc, Object user);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamCreate(int freq, int chans, int flags, int proc, Object user);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamCreateFile(string file, long offset, long length, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamCreateFile(MemoryStream file, long offset, long length, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamCreateFile(Asset asset, long offset, long length, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamCreateURL(string url, int offset, int flags, DOWNLOADPROC proc, int user);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamCreateFileUser(int system, int flags, BASS_FILEPROCS procs, Object user);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_StreamFree(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern long BASS_StreamGetFilePosition(int handle, int mode);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamPutData(int handle, MemoryStream buffer, int length);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_StreamPutFileData(int handle, MemoryStream buffer, int length);

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_RecordGetDeviceInfo(int device, BASS_DEVICEINFO info);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_RecordInit(int device);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_RecordSetDevice(int device);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_RecordGetDevice();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_RecordFree();
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_RecordGetInfo(BASS_RECORDINFO info);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern string BASS_RecordGetInputName(int input);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_RecordSetInput(int input, int flags, float volume);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_RecordGetInput(int input, float volume);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_RecordStart(int freq, int chans, int flags, RECORDPROC proc, Object user);

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern double BASS_ChannelBytes2Seconds(int handle, long pos);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern long BASS_ChannelSeconds2Bytes(int handle, double pos);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ChannelGetDevice(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelSetDevice(int handle, int device);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ChannelIsActive(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelGetInfo(int handle, BASS_CHANNELINFO info);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern IntPtr BASS_ChannelGetTags(int handle, int tags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern long BASS_ChannelFlags(int handle, int flags, int mask);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelUpdate(int handle, int length);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelLock(int handle, bool lockb);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelPlay(int handle, bool restart);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelStop(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelPause(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelSetAttribute(int handle, int attrib, float value);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelGetAttribute(int handle, int attrib, float value);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelSlideAttribute(int handle, int attrib, float value, int time);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelIsSliding(int handle, int attrib);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelSet3DAttributes(int handle, int mode, float min, float max, int iangle, int oangle, float outvol);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelGet3DAttributes(int handle, int mode, float min, float max, int iangle, int oangle, float outvol);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelSet3DPosition(int handle, BASS_3DVECTOR pos, BASS_3DVECTOR orient, BASS_3DVECTOR vel);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelGet3DPosition(int handle, BASS_3DVECTOR pos, BASS_3DVECTOR orient, BASS_3DVECTOR vel);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern long BASS_ChannelGetLength(int handle, int mode);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelSetPosition(int handle, long pos, int mode);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern long BASS_ChannelGetPosition(int handle, int mode);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ChannelGetLevel(int handle);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ChannelGetData(int handle, MemoryStream buffer, int length);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ChannelSetSync(int handle, int type, long param, SYNCPROC proc, uint user);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelRemoveSync(int handle, int sync);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ChannelSetDSP(int handle, DSPPROC proc, Object user, int priority);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelRemoveDSP(int handle, int dsp);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelSetLink(int handle, int chan);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelRemoveLink(int handle, int chan);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_ChannelSetFX(int handle, int type, int priority);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_ChannelRemoveFX(int handle, int fx);

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_FXSetParameters(int handle, Object paramso);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_FXGetParameters(int handle, Object paramso);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern bool BASS_FXReset(int handle);

        public static class Utils
        {
            public static int LOBYTE(int n) { return n & 0xff; }
            public static int HIBYTE(int n) { return (n >> 8) & 0xff; }
            public static int LOWORD(int n) { return n & 0xffff; }
            public static int HIWORD(int n) { return (n >> 16) & 0xffff; }
            public static int MAKEWORD(int a, int b) { return (a & 0xff) | ((b & 0xff) << 8); }
            public static int MAKELONG(int a, int b) { return (a & 0xffff) | (b << 16); }
        }
   }
    /*public class BASS_AAC
    {
        // Additional BASS_SetConfig options
        public const int BASS_CONFIG_MP4_VIDEO = 0x10700; // play the audio from MP4 videos
        public const int BASS_CONFIG_AAC_MP4 = 0x10701; // support MP4 in BASS_AAC_StreamCreateXXX functions (no need for BASS_MP4_StreamCreateXXX)

        public const int BASS_AAC_STEREO = 0x400000; // downmatrix to stereo

        // BASS_CHANNELINFO type
        public const int BASS_CTYPE_STREAM_AAC = 0x10b00; // AAC
        public const int BASS_CTYPE_STREAM_MP4 = 0x10b01; // MP4

        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_AAC_StreamCreateFile(string file, long offset, long length, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_AAC_StreamCreateFile(MemoryStream file, long offset, long length, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_AAC_StreamCreateURL(string url, int offset, int flags, BASS.DOWNLOADPROC proc, object user);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_AAC_StreamCreateFileUser(int system, int flags, BASS.BASS_FILEPROCS procs, object user);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_MP4_StreamCreateFile(string file, long offset, long length, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_MP4_StreamCreateFile(MemoryStream file, long offset, long length, int flags);
        [System.Runtime.InteropServices.DllImport("bass.dll", SetLastError = true)]
        public static extern int BASS_MP4_StreamCreateFileUser(int system, int flags, BASS.BASS_FILEPROCS procs, object user);
    }*/
}
