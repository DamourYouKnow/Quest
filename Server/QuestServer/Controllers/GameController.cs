using System;
using System.Net.WebSockets;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Quest.Utils;
using Quest.Utils.Networking;
using Quest.Core.Cards;
using Quest.Core.Players;

namespace Quest.Core {
    public class GameController {
        private Dictionary<string, Player> players; // TODO: We may want to map sockets to players instead.
        private QuestMessageHandler messageHandler;
        private QuestMatch game; // TODO: If we have time make this a dictionary to support multiple games.

        public GameController(QuestMessageHandler messageHandler) {
            this.game = new QuestMatch(new Logger("ServerGame"), this);
            this.players = new Dictionary<string, Player>();
            this.messageHandler = messageHandler;
            this.InitEventHandlers();
        }


        private void InitEventHandlers() {
            // Link all events to a function with a JToken parameter.
            messageHandler.On("player_join", OnPlayerJoined);

            messageHandler.On("test", (data) => {
                Console.WriteLine(data.ToString());
                game.Log("Test Event!!!");
            });
        }

        private void OnPlayerJoined(JToken data) {
            string username = (string)data["player"]["username"];
            Player newPlayer = new Player(username, this.game);
            this.players.Add(username, newPlayer);
        }

        private void OnPlayCards(JToken data) {
            Player player = players[(string)data["username"]];
            List<string> cardNames = Jsonify.ArrayToList<string>(data["cards"]);
            player.Play(player.Hand.GetCards<BattleCard>(cardNames));
        }

        private void OnDiscardCards(JToken data) {
            Player player = players[(string)data["username"]];
            List<string> cardNames = Jsonify.ArrayToList<string>(data["cards"]);
            player.Discard(player.Hand.GetCards<Card>(cardNames));
        }

        public void UpdatePlayers() {
            JObject data = new JObject();
            JArray playerArray = new JArray();
            this.game.Players.ForEach((p) => playerArray.Add(p.Converter.Json.ToJObject(p)));
            data["players"] = playerArray;
            EventData ev = new EventData("update_players", data);
            this.messageHandler.SendAllAsync(ev.ToString());
        }
    }

    public class EventData {
        private string eventName;
        private JObject data;

        public EventData(string message) {
            JObject eventJson = JObject.Parse(message);
            this.eventName = (string)eventJson["event"];
            this.data = (JObject)eventJson["data"];
        }

        public EventData(string eventName, JObject data) {
            this.eventName = eventName;
            this.data = data;
        }

        public override string ToString() {
            JObject eventJson = new JObject();
            eventJson["event"] = this.eventName;
            eventJson["data"] = data;
            return eventJson.ToString();
        }
    }
}
