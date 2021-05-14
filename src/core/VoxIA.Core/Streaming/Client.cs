using System;

namespace VoxIA.Core.Streaming
{
    public class Client
    {
        public string Id { get; }

        public int Port { get; }

        public Client()
        {
            this.Id = Guid.NewGuid().ToString();
            //TODO: Port must be assigned from server after registering!
            this.Port = 6000;
        }
    }
}
