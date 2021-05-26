using Serilog;
using System;
using System.IO;

namespace VoxIA.ZerocIce.Core.Server
{
    public class ConsoleIceServer : IIceServer
    {
        private const string LOGGER_TAG = "Ice.Console";
        private readonly ILogger _logger;

        private Action ShutdownCommunicatorAction { get; set; }

        public ConsoleIceServer(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Start the ICE communicator, run the server and wait for a shutdown
        /// command. Note that this call is blocking.
        /// </summary>
        /// <param name="args">Parameters (i.e. Console Application 'args')</param>
        public void Start(string[] args)
        {
            Start(args, null);
        }

        /// <summary>
        /// Start the ICE communicator, run the server and wait for a shutdown
        /// command. Note that this call is blocking.
        /// </summary>
        /// <param name="args">Parameters (i.e. Console Application 'args')</param>
        /// <param name="configurationFile">Server configuration file path</param>
        public void Start(string[] args, string configurationFile)
        {
            _logger.Information($"[{LOGGER_TAG}] Server initializing...");
            _logger.Information($"[{LOGGER_TAG}] Server running as '{Environment.UserName}'");

            // Create the upload area directory.
            Directory.CreateDirectory("./upload-area");

            if (string.IsNullOrEmpty(configurationFile))
            {
                using var communicator = Ice.Util.initialize(ref args);
                RunIceGrid(communicator);
            }
            else
            {
                using var communicator = Ice.Util.initialize(ref args, configurationFile);
                RunIceServer(communicator);
            }
        }

        private void RunIceServer(Ice.Communicator communicator)
        {
            var id = Ice.Util.stringToIdentity("MediaServer");

            RunIce(communicator, id);
        }

        private void RunIceGrid(Ice.Communicator communicator)
        {
            var properties = communicator.getProperties();
            var id = Ice.Util.stringToIdentity(properties.getProperty("Identity"));

            RunIce(communicator, id);
        }

        private void RunIce(Ice.Communicator communicator, Ice.Identity identity)
        {
            var adapter = communicator.createObjectAdapter("MediaServerAdapter");
            adapter.add(new MediaServer(_logger), identity);
            adapter.activate();

            // Ensure that communicator is notified of the stop signal.
            ShutdownCommunicatorAction = () =>
            {
                communicator.destroy();
                _logger.Information($"[{LOGGER_TAG}] Communicator destroyed.");
            };

            _logger.Information($"[{LOGGER_TAG}] Server running.");
            communicator.waitForShutdown();
            _logger.Information($"[{LOGGER_TAG}] Server shutdown.");
        }

        public void Stop()
        {
            // Trigger the communicator to shutdown by invoking the action.
            ShutdownCommunicatorAction?.Invoke();
        }
    }
}
