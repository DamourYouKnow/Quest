using System;
using System.Net.WebSockets;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Quest.Utils.Networking;
using Quest.Core.Players;

namespace Quest.Core {
    public class GameController {
        private Dictionary<string, Player> players; // TODO: We may want to map sockets to players instead.
        private QuestMessageHandler messageHandler;
        private QuestMatch game; // TODO: If we have time make this a dictionary to support multiple games.

        public GameController(QuestMessageHandler messageHandler) {
            this.game = new QuestMatch(new Logger("ServerGame"));
            this.players = new Dictionary<string, Player>();
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

        private void OnPlayerJoined(JToken data) {
            string username = (string)data["player"]["username"];
            Player newPlayer = new Player(username, this.game);
            this.players.Add(username, newPlayer);
        }
    }
}
