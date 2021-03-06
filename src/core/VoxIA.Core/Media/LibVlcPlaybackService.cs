using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using VoxIA.Core.Streaming;

namespace VoxIA.Core.Media
{
    public class LibVlcPlaybackService : IDisposable
    {
        private const string MediaFolder = @"/app/tracks/";

        private readonly LibVLC _vlc;
        private MediaPlayer _player;
        //private readonly MediaList _medias;
        private LibVLCSharp.Shared.Media _media;

        public int Port { get; set; }

        public bool IsAvailable =>
            _player?.State != VLCState.Buffering &&
            _player?.State != VLCState.Opening &&
            _player?.State != VLCState.Paused &&
            _player?.State != VLCState.Playing;

        public event EventHandler<EventArgs> TimeChanged;
        public event EventHandler<EventArgs> PositionChanged;
        public event EventHandler<MediaPlayerLengthChangedEventArgs> LengthChanged;
        public event EventHandler<EventArgs> EndReached;
        public event EventHandler<EventArgs> Paused;
        public event EventHandler<EventArgs> Stopped;
        public event EventHandler<EventArgs> Playing;

        private bool disposedValue;

        //public bool IsPlaying => _player.IsPlaying;

        public LibVlcPlaybackService() : this(false, "--no-video")
        {
        }

        public LibVlcPlaybackService(bool enableDebugLogs, params string[] options)
        {
            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC(enableDebugLogs, options);

            if (!enableDebugLogs)
            {
                _vlc.Log += (sender, e) => { /* Do nothing! Don't log in the console... */ };
            }
            //_medias = new MediaList();

            _player = new MediaPlayer(_vlc);

            //_player.TimeChanged += TimeChanged;
            //_player.PositionChanged += (sender, e) => Console.WriteLine("Position: " + e.Position);
            _player.LengthChanged += (sender, e) => LengthChanged?.Invoke(sender, e);
            //_player.EndReached += EndReached;
            _player.Playing += (sender, e) => Playing?.Invoke(sender, e);
            _player.Paused += (sender, e) => Paused?.Invoke(sender, e);
            _player.Stopped += (sender, e) => Stopped?.Invoke(sender, e);
        }

        public async Task<string> PlayAsync(string filename)
        {
            string mediaPath = MediaFolder + filename;
            if (!File.Exists(mediaPath))
            {
                return null;
            }

            if (_player == null)
            {
                //TODO: Log an error and recommendation to call InitializeAsync() first!
                return null;
            }

            // BUG in LibVLCSharp:
            // After parsing a media to¸extract its details, you are no longer
            // able to play it through an HTTP stream. There are no errors or
            // logs, but the clients are never able to connect to the stream.
            //
            // This is the last item in the VLC debug logs when this happens:
            //   main debug: using sout stream module "stream_out_transcode"
            //   main debug: using timeshift granularity of 50 MiB
            //   main debug: using timeshift path: <...>\AppData\Local\Temp
            //   main debug: `file:///<...>/local-file.mp3' 
            var media = new LibVLCSharp.Shared.Media(_vlc, mediaPath, FromType.FromPath);
            await media.Parse(MediaParseOptions.ParseLocal);

            // Dispose of any existing media from previous playbacks.
            if (_media != null)
            {
                _media.Dispose();
            }

            _media = new LibVLCSharp.Shared.Media(
                _vlc,
                mediaPath,
                FromType.FromPath,
                $":sout=#transcode{{vcodec=none,acodec=mp3,ab=128,channels=2,samplerate=44100,scodec=none}}:http{{dst=:{Port}/stream.mp3}}",
                ":no-sout-all",
                ":sout-keep"
            );
            _player.Media = _media;
            var playing = _player?.Play();

            //using var media1 = new LibVLCSharp.Shared.Media(_vlc, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4"));
            //using var media2 = new LibVLCSharp.Shared.Media(_vlc, new Uri("https://archive.org/download/ImagineDragons_201410/imagine%20dragons.mp4"));
            //_medias.AddMedia(media2);
            //_medias.AddMedia(media1);
            //_medias.SetMedia(media1);
            //using var m = new Media(_medias);

            return (playing == true ? $"http://{Environment.GetEnvironmentVariable("DOCKER_HOST_IP")}:{Port}/stream.mp3" : null);
        }

        public void Pause()
        {
            if (_player == null)
            {
                //TODO: Log an error and recommendation to call InitializeAsync() first!
                return;
            }

            _player?.Pause();
        }

        public void Stop()
        {
            if (_player == null)
            {
                //TODO: Log an error and recommendation to call InitializeAsync() first!
                return;
            }

            _player.Stop();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_media != null) _media.Dispose();
                    //if (_medias != null) _medias.Dispose();
                    if (_player != null) _player.Dispose();
                    if (_vlc != null) _vlc.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
