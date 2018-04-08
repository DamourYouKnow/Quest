using System;
using Quest.Core.Players;
using Quest.Core.Cards;

namespace Quest.Core.Scenarios {
    public class ScenarioCreator {
        public static QuestMatch EmptyGame(GameController controller=null) {
            QuestMatch game = new QuestMatch(logger: new Logger("DefaultGame"), 
                                             controller:controller);
            game.Setup();
            return game;
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

        public static QuestMatch Scenario1(GameController controller=null) {
			QuestMatch game = new QuestMatch(logger:new Logger("Scenario1"), 
                                             controller:controller);

            game.StoryDeck.Push(new ProsperityEvent(game));
            game.StoryDeck.Push(new ChivalrousDeedEvent(game));
            game.StoryDeck.Push(new BoarHunt(game));

            game.Setup(shuffleDecks:true);

            return game;
        }

        public static QuestMatch Scenario2(GameController controller=null) {
			QuestMatch game = new QuestMatch(logger:new Logger("Scenario2"), 
                                             controller:controller);
            game.Setup(shuffleDecks:true);
            return game;
        }
    }
}