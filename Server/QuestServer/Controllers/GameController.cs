using System;
using System.Linq;
using System.Net.WebSockets;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using Quest.Utils;
using Quest.Utils.Networking;
using Quest.Core.Cards;
using Quest.Core.Players;
using Quest.Core.Scenarios;

namespace Quest.Core {
    public class GameController {
        private QuestMessageHandler messageHandler;
        private Dictionary<Player, QuestMatch> matches;

        public GameController(QuestMessageHandler messageHandler, QuestMatch match=null) {
            if (match == null) {
                this.matches = new Dictionary<Player, QuestMatch>();
            }
            else {
                this.matches = new Dictionary<Player, QuestMatch>();

            }

            this.messageHandler = messageHandler;
            this.InitEventHandlers();
        }

        public QuestMatch GetMatch(Player player) {
            return this.matches[player];
        }

        private void InitEventHandlers() {
            // Link all events to a function with a JToken parameter.
            messageHandler.On("player_join", OnPlayerJoined);
            messageHandler.On("request_games", OnRequestGames);
            messageHandler.On("create_game", OnCreateGame);
            messageHandler.On("join_game", OnJoinGame);
            messageHandler.On("add_ai", OnAddAI);
            messageHandler.On("start_game", OnStartGame);
            messageHandler.On("request_players", OnRequestPlayers);
            messageHandler.On("round_end", OnRoundEnd);
            messageHandler.On("play_cards", OnPlayCards);
            messageHandler.On("confirm_cards", OnConfirmCards);
            messageHandler.On("play_cards_stage", OnPlayCardStage);
            messageHandler.On("confirm_stage", OnConfirmStage);
            messageHandler.On("discard", OnDiscard);
            messageHandler.On("participation_response", OnParticipationResponse);
            messageHandler.On("quest_sponsor_response", OnQuestSponsorResponse);
            messageHandler.On("simulate_game", OnSimulateGame);
        }

        private void OnPlayerJoined(Player player, JToken data) {
            // As of right now this on is handled in on player joined.
        }

        private void OnRequestGames(Player player, JToken data) {
            this.UpdateGames(player);
        }

        private void OnCreateGame(Player player, JToken data) {
            QuestMatch match = ScenarioCreator.EmptyGame(this);

            int scenario = (int)data["scenario"];
            if (scenario == 1) match = ScenarioCreator.Scenario1(this);
            if (scenario == 2) match = ScenarioCreator.Scenario2(this);

            player.Behaviour = new HumanPlayer();
            match.AddPlayer(player);
            this.matches.Add(player, match);

            this.UpdatePlayers(this.matches[player]);
        }

        private void OnJoinGame(Player player, JToken data) {
            QuestMatch game = this.gameWithId((int)data["game_id"]);
            player.Behaviour = new HumanPlayer();
            game.AddPlayer(player);
            this.matches.Add(player, game);
            this.UpdatePlayers(this.matches[player]);
        }

        private void OnAddAI(Player player, JToken data) {
            int strat = (int)data["strategy"];
            QuestMatch match = this.matches[player];

            Player aiPlayer = new Player("AI Player " + match.Players.Count);
            if (strat == 1) aiPlayer.Behaviour = new Strategy1();
            if (strat == 2) aiPlayer.Behaviour = new Strategy2();
            if (strat == 3) aiPlayer.Behaviour = new Strategy3();

            match.AddPlayer(aiPlayer);
            this.UpdatePlayers(this.matches[player]);
        }

        private void OnStartGame(Player player, JToken data) {
            if (player.Match.Players.Count < 2) return;
            this.matches[player].Setup(shuffleDecks:false);
            this.matches[player].RunGame();
        }

        private void OnRequestPlayers(Player player, JToken data) {
            this.UpdatePlayers(this.matches[player]);
        }

        private void OnRoundEnd(Player player, JToken data) {
            if (player.Hand.Count > 12) {
                this.Message(this.GetMatch(player), "You have too many cards ("+(player.Hand.Cards.Count)+"), please discard");
                this.matches[player].Log("Rejected play by " + player.Username + " too many cards in hand("+(player.Hand.Cards.Count)+")");

                this.UpdateHand(player);
            }
            else {
                this.matches[player].RoundEndResponse(player);
            }
        }

        private void OnPlayCards(Player player, JToken data) {
            List<string> cardNames = Jsonify.ArrayToList<string>(data["cards"]);
            player.Play(player.Hand.GetCards<BattleCard>(cardNames));
        }

        private void OnConfirmCards(Player player, JToken data) {
            if (player.Hand.Count > 12) {
                this.UpdateHand(player);
                this.Message(this.GetMatch(player), "You have too many cards ("+(player.Hand.Cards.Count)+"), please discard");
                this.matches[player].Log("Rejected play by " + player.Username + " too many cards in hand("+(player.Hand.Cards.Count)+")");
                return;
            }

            InteractiveStoryCard story = this.matches[player].CurrentStory as InteractiveStoryCard;
            story.AddPlayed(player);
            this.UpdateHand(player);
            this.UpdatePlayerArea(player);
        }

        private void OnPlayCardStage(Player player, JToken data) {
            QuestCard quest = this.matches[player].CurrentStory as QuestCard;
            QuestArea area = quest.StageBuilder;

            List<string> cardNames = Jsonify.ArrayToList<string>(data["cards"]);
            List<FoeCard> foe = player.Hand.GetCards<FoeCard>(cardNames);
            List<WeaponCard> weapons = player.Hand.GetCards<WeaponCard>(cardNames);
            List<TestCard> test = player.Hand.GetCards<TestCard>(cardNames);

            player.Hand.Transfer(area, test.Cast<Card>().ToList());
            player.Hand.Transfer(area, foe.Cast<Card>().ToList());
            player.Hand.Transfer(area, weapons.Cast<Card>().ToList());

            this.UpdateHand(player);
            this.UpdateOtherArea(player, area.Cards);
            this.UpdatePlayerArea(player);
        }

        private void OnConfirmStage(Player player, JToken data) {
            QuestCard quest = this.matches[player].CurrentStory as QuestCard;
            QuestArea area = quest.StageBuilder;

            if (player.Hand.Count > 12) {
                this.UpdateHand(player);
                this.Message(this.GetMatch(player), "You have too many cards ("+(player.Hand.Cards.Count)+"), please discard");
                this.matches[player].Log("Rejected play by " + player.Username + " too many cards in hand("+(player.Hand.Cards.Count)+")");
                return;
            }

            if (area.MainCard != null && quest.Stages.Count < quest.StageCount) {
                quest.AddStage(area);
                quest.StageResponse();
            }

            this.UpdateOtherArea(player, new List<Card>());
        }

        private void OnDiscard(Player player, JToken data) {
            if (player.Hand.Count <= 12) {
                this.UpdateHand(player);
                return;
            }

            List<string> cardNames = Jsonify.ArrayToList<string>(data["cards"]);
            player.Discard(player.Hand.GetCards<Card>(cardNames));
        }

        private void OnParticipationResponse(Player player, JToken data) {
            InteractiveStoryCard story = this.matches[player].CurrentStory as InteractiveStoryCard;
            story.ParticipationResponse(player, (bool)data["participating"]);
        }

        private void OnQuestSponsorResponse(Player player, JToken data) {
            QuestCard quest = this.matches[player].CurrentStory as QuestCard;
            quest.SponsorshipResponse(player, (bool)data["sponsoring"]);
        }

        private void OnSimulateGame(Player player, JToken data){
          ScenarioCreator.AISimGame(3).RunGame();
        }

        public async void UpdateGames(Player player) {
            List<int> ids = new List<int>();
            foreach (QuestMatch match in this.matches.Values) {
                if (!ids.Contains(match.Id)) ids.Add(match.Id);
            }

            JObject data = new JObject();
            data["game_ids"] = Jsonify.ListToArray(ids);
            EventData evn = new EventData("update_games", data);
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());
        }

        public async void UpdatePlayers(QuestMatch match) {
            JObject data = new JObject();
            JArray playerArray = new JArray();
            match.Players.ForEach((p) => playerArray.Add(p.Converter.Json.ToJObject(p)));
            data["players"] = playerArray;
            EventData evn = new EventData("update_players", data);
            await this.messageHandler.SendToMatchAsync(match, evn.ToString());
        }

        public async void UpdateStory(QuestMatch match) {
            JObject data = new JObject();
            data["card"] = match.CurrentStory.Converter.Json.ToJObject(match.CurrentStory);
            EventData evn = new EventData("update_story", data);
            await this.messageHandler.SendToMatchAsync(match, evn.ToString());
        }

        public async void EndStory(QuestMatch match) {
            EventData evn = new EventData("end_story", new JObject());
            await this.messageHandler.SendToMatchAsync(match, evn.ToString());
        }

        public async void UpdateHand(Player player) {
            JObject data = new JObject();
            JArray cardArray = new JArray();
            player.Hand.Cards.ForEach((c) => cardArray.Add(c.Converter.Json.ToJObject(c)));
            data["cards"] = cardArray;
            EventData evn = new EventData("update_hand", data);
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());

            if (this.matches.ContainsKey(player)) {
                this.UpdatePlayers(this.matches[player]);
            }
        }

        public async void UpdatePlayerArea(Player player) {
            JObject data = new JObject();
            JArray cardArray = new JArray();
            player.BattleArea.Cards.ForEach((c) => cardArray.Add(c.Converter.Json.ToJObject(c)));
            data["cards"] = cardArray;
            data["battle_points"] = player.BattleArea.BattlePoints();
            EventData evn = new EventData("update_player_area", data);
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());
        }

        public async void UpdateOtherArea(Player player, List<Card> cards) {
            JObject data = new JObject();

            string areaName = "other";
            if (matches[player].CurrentStory is QuestCard) areaName = "quest";
            if (matches[player].CurrentStory is TournamentCard) areaName = "tournament";
            data["area_name"] = areaName;

            JArray cardArray = new JArray();
            cards.ForEach((c) => cardArray.Add(c.Converter.Json.ToJObject(c)));
            data["cards"] = cardArray;
            EventData evn = new EventData("update_other_area", data);
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());
        }

        public async void UpdateOtherArea(QuestMatch match, List<Card> cards) {
            foreach (Player player in match.Players) {
                if (this.matches.ContainsKey(player)) {
                    this.UpdateOtherArea(player, cards);
                }
            }
        }

        public async void PlayerWait(Player player) {
            EventData evn = new EventData("wait", new JObject());
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());
        }

        public async void RequestDiscard(Player player) {
            EventData evn = new EventData("request_discard", new JObject());
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());
        }

        public async void RequestStage(Player player) {
            EventData evn = new EventData("request_stage", new JObject());
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());
        }

        public async void PromptPlayer(Player player, string type, string message, string image=null) {
            JObject data = new JObject();
            data["message"] = message;
            data["image"] = image;
            EventData evn = new EventData(type.ToLower(), data);
            await this.messageHandler.SendToPlayerAsync(player, evn.ToString());
        }

        public async void Message(QuestMatch match, string message) {
            JObject data = new JObject();
            data["message"] = message;
            EventData evn = new EventData("message", data);
            await this.messageHandler.SendToMatchAsync(match, evn.ToString());
        }

        private QuestMatch gameWithId(int id) {
            foreach (QuestMatch match in this.matches.Values) {
                if (id == match.Id) return match;
            }
            return null;
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
