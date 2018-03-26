using System;
using System.Net.WebSockets;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Quest.Utils.Networking;
using Quest.Core.Players;

namespace Quest.Core {
    public class GameController {
        private Dictionary<WebSocket, Player> playerSockets;
        private QuestMessageHandler messageHandler;

        public GameController(QuestMessageHandler messageHandler) {
            this.playerSockets = new Dictionary<WebSocket, Player>();
            this.messageHandler = messageHandler;
            this.InitEventHandlers();
        }


        private void InitEventHandlers() {
            // Link all events to a function with a JToken parameter.
            messageHandler.On("player_join", OnPlayerJoined);

            messageHandler.On("test", (data) => {
                Console.WriteLine(data.ToString());
            });
        }

        private static void OnPlayerJoined(JToken data) {
            Console.WriteLine("Event: PlayerJoined");
        }
    }
}
