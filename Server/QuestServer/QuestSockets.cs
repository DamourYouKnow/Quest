using System;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Quest.Core;
using Utils.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Quest.Utils.Networking {
    public class QuestMessageHandler : WebSocketHandler {
        private GameController gc;
        public QuestMessageHandler(WebSocketConnectionManager connectionManager) : base(connectionManager) {
            gc = new GameController(this);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer) {
            string message = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("Message received: " + message);
            JObject jqe = JObject.Parse(message);
            gc.handle_event(socket, jqe.ToObject<QuestEvent>());
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
