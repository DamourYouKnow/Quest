using System;
using System.Text;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

using Quest.Core;
using Utils.Networking;

namespace Quest.Utils.Networking {
    public class QuestMessageHandler : WebSocketHandler {
        private GameController gc;
        private Dictionary<string, Action<JToken>> eventHandlers;

        public QuestMessageHandler(WebSocketConnectionManager connectionManager) : base(connectionManager) {
            eventHandlers = new Dictionary<string, Action<JToken>>();
            gc = new GameController(this);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer) {
            string message = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("Message received: " + message);
            JObject jqe = JObject.Parse(message);

            string eventName = (string)jqe["event"];
            if (eventHandlers.ContainsKey(eventName)) {
                eventHandlers[eventName](jqe["data"]);
            }
        }

        public void On(string eventName, Action<JToken> handler) {
            eventHandlers.Add(eventName, handler);
        }

        // TODO: Send handler.
    }

    public class QuestEvent{
        public string name;
        public string data;

        public QuestEvent(string name, string data){
            this.name = name;
            this.data = data;
        }
    }
}
