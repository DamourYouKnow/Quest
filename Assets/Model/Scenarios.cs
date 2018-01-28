using System;
using Quest.Core.Players;

namespace Quest.Core.Scenarios {
    public class ScenarioCreator {
        public static QuestMatch GameNoDeal(int playerCount) {
            QuestMatch game = new QuestMatch();
            for (int i = 1; i <= playerCount; i++) {
                game.AddPlayer(new Player("Test Player " + i, game));
            }
            return game;
        }

        public static QuestMatch GameWithDeal(int playerCount) {
            QuestMatch game = GameNoDeal(playerCount);
            game.Setup();
            return game;
        }
    }
}