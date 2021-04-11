using LibVLCSharp;
using LibVLCSharp.Shared;
using System;
using System.IO;
using System.Threading.Tasks;
using VoxIA.Mobile.Models;

namespace VoxIA.ZerocIce.Core.Server
{
    public class LibVlcPlaybackService : IDisposable
    {
        private const string MediaFolder = @"D:\sources\uapv2101376\m2\m2-archi-distribuee\mp3\";

        private readonly LibVLC _vlc;
        private MediaPlayer _player;
        //private readonly MediaList _medias;
        private Media _media;

        private bool disposedValue;

        //public bool IsPlaying => _player.IsPlaying;

        //public event EventHandler<EventArgs> Playing
        //{
        //    add => _player.Playing += value;
        //    remove => _player.Playing -= value;
        //}

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
        }

        private string BuildVlcStreamingOptions(IceClient client) =>
            $":sout=#transcode{{vcodec=none,acodec=mp3,ab=128,channels=2,samplerate=44100,scodec=none}}:http{{dst=:{client.Port}/stream.mp3}}";

        public async Task<bool> InitializeAsync(IceClient client, Song song)
        {
            string mediaPath = MediaFolder + song.Url;
            if (!File.Exists(mediaPath))
            {
                return false;
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
            var media = new Media(_vlc, mediaPath, FromType.FromPath);
            await media.Parse(MediaParseOptions.ParseLocal);
            song.Title = media.Meta(MetadataType.Title);
            song.ArtistName = media.Meta(MetadataType.Artist);

            _player = new MediaPlayer(_vlc);
            _media = new Media(_vlc, mediaPath, FromType.FromPath, BuildVlcStreamingOptions(client), ":no-sout-all", ":sout-keep");
            _player.Media = _media;

            //_player.TimeChanged += TimeChanged;
            //_player.PositionChanged += PositionChanged;
            //_player.LengthChanged += LengthChanged;
            //_player.EndReached += EndReached;
            //_player.Playing += Playing;
            //_player.Paused += Paused;
            _player.PositionChanged += (sender, e) =>
            {
                Console.WriteLine("Position: " + e.Position);
            };
            _player.Stopped += (sender, e) =>
            {
                Console.WriteLine("STOPPED!");
            };

            return true;
        }

        public bool Play()
        {
            if (_player == null)
            {
                //TODO: Log an error and recommendation to call InitializeAsync() first!
                return false;
            }

            //using var media1 = new Media(_vlc, new Uri("http://commondatastorage.googleapis.com/gtv-videos-bucket/sample/ElephantsDream.mp4"));
            //using var media2 = new Media(_vlc, new Uri("https://archive.org/download/ImagineDragons_201410/imagine%20dragons.mp4"));
            //_medias.AddMedia(media2);
            //_medias.AddMedia(media1);
            //_medias.SetMedia(media1);
            //using var m = new Media(_medias);

            return (_player?.Play() == true);
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
                    if (_media != null ) _media.Dispose();
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
