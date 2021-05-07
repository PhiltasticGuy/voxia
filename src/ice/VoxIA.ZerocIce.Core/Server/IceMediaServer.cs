using System;
using System.IO;

namespace VoxIA.ZerocIce.Core.Server
{
    public class IceMediaServer : IMediaServer
    {
        private Action ShutdownCommunicatorAction { get; set; }

        public IceMediaServer()
        {
        }

        /// <summary>
        /// Start the ICE communicator, run the server and wait for a shutdown
        /// command. Note that this call is blocking.
        /// </summary>
        /// <param name="args">Parameters (i.e. Console Application 'args')</param>
        public void Start(string[] args)
        {
            using var communicator = Ice.Util.initialize(ref args);
            RunIceServer(communicator);
        }

        /// <summary>
        /// Start the ICE communicator, run the server and wait for a shutdown
        /// command. Note that this call is blocking.
        /// </summary>
        /// <param name="args">Parameters (i.e. Console Application 'args')</param>
        /// <param name="configurationFile">Server configuration file path</param>
        public void Start(string[] args, string configurationFile)
        {
            using var communicator = Ice.Util.initialize(ref args, configurationFile);
            RunIceServer(communicator);
        }

        private void RunIceServer(Ice.Communicator communicator)
        {
            // Create the upload area directory.
            Directory.CreateDirectory("./upload-area");

            var adapter =
                communicator.createObjectAdapterWithEndpoints("SimplePrinterAdapter", "default -h ice.server -p 10000");
            var server = new MediaServer();
            adapter.add(server, Ice.Util.stringToIdentity("SimplePrinter"));
            adapter.activate();

            // Ensure that communicator is notified of the stop signal.
            ShutdownCommunicatorAction = () =>
            {
                communicator.destroy();
            };

            communicator.waitForShutdown();
        }

        public void Stop()
        {
            // Trigger the communicator to shutdown by invoking the action.
            ShutdownCommunicatorAction?.Invoke();
        }
    }
}
