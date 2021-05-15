using System;
using System.IO;

namespace VoxIA.ZerocIce.Core.Server
{
    public class ConsoleIceServer : IIceServer
    {
        private Action ShutdownCommunicatorAction { get; set; }

        public ConsoleIceServer()
        {
        }

        /// <summary>
        /// Start the ICE communicator, run the server and wait for a shutdown
        /// command. Note that this call is blocking.
        /// </summary>
        /// <param name="args">Parameters (i.e. Console Application 'args')</param>
        public void Start(string[] args)
        {
            using var communicator = Ice.Util.initialize(ref args, "config.server");
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
            //using var communicator = Ice.Util.initialize(ref args, configurationFile);
            //RunIceServer(communicator);
            Console.WriteLine("[DEBUG] UserName: {0}", Environment.UserName);
            using var communicator = Ice.Util.initialize(ref args);
            RunIceGrid(communicator);
        }

        private void RunIceServer(Ice.Communicator communicator)
        {
            // Create the upload area directory.
            Directory.CreateDirectory("./upload-area");

            var adapter = communicator.createObjectAdapter("MediaServer");
            adapter.add(new MediaServer(), Ice.Util.stringToIdentity("MediaServer"));
            adapter.activate();

            // Ensure that communicator is notified of the stop signal.
            ShutdownCommunicatorAction = () =>
            {
                communicator.destroy();
            };

            communicator.waitForShutdown();
        }

        private void RunIceGrid(Ice.Communicator communicator)
        {
            // Create the upload area directory.
            Directory.CreateDirectory("./upload-area");

            var adapter = communicator.createObjectAdapter("MediaServerAdapter");
            var properties = communicator.getProperties();
            var id = Ice.Util.stringToIdentity(properties.getProperty("Identity"));
            adapter.add(new MediaServer(), id);
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
