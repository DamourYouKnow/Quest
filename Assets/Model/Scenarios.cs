using System;
using Quest.Core.Players;

namespace Quest.Core.Scenarios {
    public class ScenarioCreator {
        public static QuestMatch EmptyGame() {
            return new QuestMatch(new Logger("Scenario"));
        }

        public static QuestMatch GameNoDeal(int playerCount) {
            QuestMatch game = EmptyGame();
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

        public static QuestMatch AIGameNoDeal(int aiCount) {
            QuestMatch game = EmptyGame();
            for (int i = 1; i <= aiCount; i++) {
                Player aiPlayer = new Player("AI Player " + i, game);
                aiPlayer.Behaviour = new Strategy2();
            }
            return game;
        }
    }
}