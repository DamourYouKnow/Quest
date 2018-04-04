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
        private QuestMessageHandler messageHandler;
        private QuestMatch match; // TODO: If we have time make this a dictionary to support multiple games.

        public GameController(QuestMessageHandler messageHandler) {
            this.match = new QuestMatch(new Logger("ServerGame"), this);
            this.messageHandler = messageHandler;
            this.InitEventHandlers();
        }

        public QuestMatch Match {
            get { return this.match; }
        }

        private void InitEventHandlers() {
            // Link all events to a function with a JToken parameter.
            messageHandler.On("player_join", OnPlayerJoined);

            messageHandler.On("test", (username, data) => {
                Console.WriteLine(data.ToString());
                match.Log("Test Event!!!");
            });
        }

        private void OnPlayerJoined(Player player, JToken data) {
            // As of right now this on is handled in on player joined.
            this.match.Log(player.Username + " connected");
        }

        private void OnPlayCards(Player player, JToken data) {
            List<string> cardNames = Jsonify.ArrayToList<string>(data["cards"]);
            player.Play(player.Hand.GetCards<BattleCard>(cardNames));
        }

        private void OnDiscardCards(Player player, JToken data) {
            List<string> cardNames = Jsonify.ArrayToList<string>(data["cards"]);
            player.Discard(player.Hand.GetCards<Card>(cardNames));
        }

        public async void UpdatePlayers() {
            JObject data = new JObject();
            JArray playerArray = new JArray();
            this.match.Players.ForEach((p) => playerArray.Add(p.Converter.Json.ToJObject(p)));
            data["players"] = playerArray;
            EventData evn = new EventData("update_players", data);
            await this.messageHandler.SendAllAsync(evn.ToString());
        }

        public async void UpdateHand(Player player) {
            JObject data = new JObject();
            JArray cardArray = new JArray();
            player.Hand.Cards.ForEach((c) => cardArray.Add(c.Converter.Json.ToJObject(c)));
            data["cards"] = cardArray;
            EventData evn = new EventData("update_hand", data);
            await this.messageHandler.SendToAsync(player, evn.ToString());
        }

        public async void PromptPlayer(Player player, string type, string message, string image=null) {
            JObject data = new JObject();
            data["message"] = message;
            data["image"] = image;
            EventData evn = new EventData(type.ToLower(), data);
            await this.messageHandler.SendToAsync(player, evn.ToString());
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
