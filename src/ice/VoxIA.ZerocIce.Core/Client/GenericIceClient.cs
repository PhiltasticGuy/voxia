using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxIA.ZerocIce.Core.Client
{
    public class GenericIceClient : IIceClient, IDisposable
    {
        private readonly LibVLC _vlc;
        private readonly MediaPlayer _player;

        private bool disposedValue;
        private Ice.Communicator _communicator;
        public MediaServerPrx _mediaServer;

        public GenericIceClient() : this(false, "--no-video")
        {
        }

        public GenericIceClient(bool enableDebugLogs, params string[] options)
        {
            LibVLCSharp.Shared.Core.Initialize();

            _vlc = new LibVLC(enableDebugLogs, options);

            if (!enableDebugLogs)
            {
                _vlc.Log += (sender, e) => { /* Do nothing! Don't log in the console... */ };
            }

            _player = new MediaPlayer(_vlc);
        }

        public void Start(string[] args)
        {
            try
            {
                _communicator = Ice.Util.initialize(ref args);
                //TODO: The IP Address and Port should come from configurations!
                var obj = _communicator.stringToProxy("SimplePrinter:tcp -h 192.168.0.11 -p 10000");
                _mediaServer = MediaServerPrxHelper.checkedCast(obj);

                if (_mediaServer == null)
                {
                    throw new ApplicationException("Invalid proxy");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Stop()
        {
            _communicator.destroy();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_communicator != null)
                    {
                        _communicator.destroy();
                        _communicator.Dispose();
                    }
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
