using System;
using System.Text;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Quest.Core;
using Utils.Networking;

namespace Quest.Utils.Networking {
    public class QuestMessageHandler : WebSocketHandler {
        private GameController gc;
        private Dictionary<string, Action<string, JToken>> eventHandlers;
        private Dictionary<WebSocket, string> socket_player;
        private Dictionary<string, WebSocket> player_socket;

        public QuestMessageHandler(WebSocketConnectionManager connectionManager) : base(connectionManager) {
            eventHandlers = new Dictionary<string, Action<string, JToken>>();
            gc = new GameController(this);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer) {
            string message = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("Message received: " + message);
            JObject jqe = JObject.Parse(message);

            string eventName = (string)jqe["event"];
            //Register players to socket/player maps on join
            if(eventName=="player_join"){
                if(!socket_player.ContainsKey(socket)){
                    string username = (string)jqe["data"]["username"];
                    socket_player.Add(socket, username);
                    player_socket.Add(username, socket);
                }
            }
            if (eventHandlers.ContainsKey(eventName)) {
                eventHandlers[eventName](socket_player[socket], jqe["data"]);
            }
        }

        public async Task SendToAsync(string player, JObject message){
            await this.SendMessageAsync(player_socket[player], JsonConvert.SerializeObject(message));
        }

        public void On(string eventName, Action<string, JToken> handler) {
            eventHandlers.Add(eventName, handler);
        }
    }
}
