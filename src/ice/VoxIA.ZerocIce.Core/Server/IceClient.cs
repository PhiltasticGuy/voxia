using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxIA.ZerocIce.Core.Server
{
    public class IceClient
    {
        public string Id { get; }

        public string Url { get; }

        public int Port { get; }

        public IceClient()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Url = "127.0.0.1";
            //TODO: Port must be assigned from server after registering!
            this.Port = 6000;
        }
    }
}
