using System;
using System.Text;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using Quest.Core;
using Quest.Core.Players;
using Utils.Networking;

namespace Quest.Utils.Networking {
    public class QuestMessageHandler : WebSocketHandler {
        private GameController gc;
        private Dictionary<string, Action<Player, JToken>> eventHandlers;
        private Dictionary<WebSocket, Player> socket_player;
        private Dictionary<Player, WebSocket> player_socket;

        public QuestMessageHandler(WebSocketConnectionManager connectionManager) : base(connectionManager) {
            socket_player = new Dictionary<WebSocket, Player>();
            player_socket = new Dictionary<Player, WebSocket>();
            eventHandlers = new Dictionary<string, Action<Player, JToken>>();

            if (!(this is NullQuestMessageHandler)) {
                this.gc = new GameController(this);
            }
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
                    Player newPlayer = new Player(username, null);
                    socket_player.Add(socket, newPlayer);
                    player_socket.Add(newPlayer, socket);
                }
            }

            if (eventHandlers.ContainsKey(eventName)) {
                eventHandlers[eventName](socket_player[socket], jqe["data"]);
            }
        }

        public virtual async Task SendToPlayerAsync(Player player, string message){
            await this.SendMessageAsync(player_socket[player], message);
        }

        public virtual async Task SendToMatchAsync(QuestMatch match, string message) {
            foreach (Player player in match.Players) {
                await this.SendToPlayerAsync(player, message);
            }
        }

        public void On(string eventName, Action<Player, JToken> handler) {
            eventHandlers.Add(eventName, handler);
        }
    }


    public class NullQuestMessageHandler : QuestMessageHandler {
        public NullQuestMessageHandler() : base(null) {

        }

        public override async Task OnConnected(WebSocket socket) {
            return;
        }

        public override async Task OnDisconnected(WebSocket socket) {
            return;
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer) {
            return;
        }

        public override async Task SendToPlayerAsync(Player player, string message) {
            return;
        }
    }
}
