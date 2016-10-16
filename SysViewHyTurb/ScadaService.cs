using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysViewHyTurb
{
    using System.Xml;

    using AustinHarris.JsonRpc;

    public delegate void NotifyDelegate(KeyValuePair<int, float> keyValuePair);

    public class Client
    {
        public int Id;

        public List<string> VarNames;

        public NotifyDelegate Notify;
    }
    public class ScadaService
    {
        [JsonRpcMethod]
        public int Add(int num1, int num2)
        {
            return num1 + num2;
        }

        [JsonRpcMethod]
        public int RegisterClient(NotifyDelegate callBack)
        {
            var client = new Client() { Id = this.curClientId > 32 ? 0 : this.curClientId++, Notify = callBack };
            this.clients.Add(client);
            return client.Id;
        }

        [JsonRpcMethod]
        public int Subscribe(int clientId, string[] varNames)
        {
            return 1;
        }

        [JsonRpcMethod]
        public int Unsubscribe(int clientId)
        {
            return 0;
        }

        [JsonRpcMethod]
        public object[] Read(string[] varNames)
        {
            return null;
        }

        [JsonRpcMethod]
        public object Write(KeyValuePair<string, object> nameValues)
        {
            return null;
        }

        private readonly List<Client> clients = new List<Client>();

        private int curClientId = 1;

    }
}
