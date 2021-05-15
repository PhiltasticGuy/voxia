using IceSSL;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace VoxIA.ZerocIce.Core.Client
{
    public class GenericIceClient : IIceClient, IDisposable
    {
        private readonly LibVLC _vlc;
        private readonly MediaPlayer _player;
        private readonly Dictionary<string, string> _properties;

        private bool disposedValue;
        private Ice.Communicator _communicator;
        public MediaServerPrx _mediaServer;
        public GenericIceClient(Dictionary<string, string> properties) : this(false, "--no-video")
        {
            _properties = properties;
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

        //class Verifier : IceSSL.CertificateVerifier
        //{
        //    bool CertificateVerifier.verify(ConnectionInfo info)
        //    {
        //        if (info.certs != null)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //}

        public void SetServerUrl(string ipAddress, int port)
        {
            _properties["MediaServer.Proxy"] = 
                $"MediaServer:tcp -p {port}:ssl -p 10001";
            _properties["Ice.Default.Host"] = ipAddress;
        }

        public void Start(string[] args)
        {
            try
            {
                Ice.InitializationData initData = new Ice.InitializationData();
                initData.properties = Ice.Util.createProperties();
                foreach(var pair in _properties) {
                    initData.properties.setProperty(pair.Key, pair.Value);
                }
                _communicator = Ice.Util.initialize(ref args, initData);

                //Ice.PluginManager pluginMgr = _communicator.getPluginManager();
                //Ice.Plugin plugin = pluginMgr.getPlugin("IceSSL");
                //IceSSL.Plugin sslPlugin = (IceSSL.Plugin)plugin;
                //sslPlugin.setCertificates(_certs);
                //sslPlugin.setCACertificates(_certsCA);
                //sslPlugin.setCertificateVerifier(new Verifier());
                //pluginMgr.initializePlugins();

                //TODO: The IP Address and Port should come from configurations!
                //var obj = _communicator.propertyToProxy("MediaServer.Proxy");

                var obj = _communicator.stringToProxy("MediaServerId@SimpleServer.MediaServerAdapter");
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
            if (_communicator != null)
            {
                try
                {
                    _communicator.destroy();
                    _communicator.Dispose();
                }
                finally
                {
                    _communicator = null;
                }
            }
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
