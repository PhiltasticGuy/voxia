using System;

namespace VoxIA.Core.Streaming
{
    public class Client
    {
        public string Id { get; }

        public string Url { get; }

        public int Port { get; }

        public Client()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Url = "127.0.0.1";
            //TODO: Port must be assigned from server after registering!
            this.Port = 6000;
        }
    }
}
