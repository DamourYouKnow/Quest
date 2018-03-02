using System;
using Quest.Core.Players;
using Quest.Core.Cards;

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

        public static QuestMatch Scenario1() {
            QuestMatch game = new QuestMatch(new Logger("Scenario1"));

            Player player1 = new Player("Player 1", game);
            player1.Behaviour = new HumanPlayer();
            player1.Hand.Add(new Saxons(game));
            player1.Hand.Add(new Boar(game));
            player1.Hand.Add(new Sword(game));
            player1.Hand.Add(new Dagger(game));

            Player player2 = new Player("Player 2", game);
            player2.Hand.Add(new Dagger(game)); // So that player 2 can discard a weapon.

            Player player3 = new Player("Player 3", game);
            player3.Hand.Add(new Horse(game));
            player3.Hand.Add(new Excalibur(game));
            player3.Hand.Add(new Amour(game));

            Player player4 = new Player("Player 4", game);
            player4.Hand.Add(new BattleAx(game));
            player4.Hand.Add(new Lance(game));

            game.StoryDeck.Push(new ProsperityEvent(game));
            game.StoryDeck.Push(new BoarHunt(game));

            game.AddPlayer(player1);
            game.AddPlayer(player2);
            game.AddPlayer(player3);
            game.AddPlayer(player4);

            game.Setup(shuffleDecks:false);

            return game;
        }

        public static QuestMatch Scenario2() {
            QuestMatch game = new QuestMatch(new Logger("Scenario2"));

            Player player1 = new Player("Player 1", game);
            player1.Behaviour = new HumanPlayer();
            player1.Hand.Add(new Saxons(game));
            player1.Hand.Add(new Boar(game));
            player1.Hand.Add(new Sword(game));
            player1.Hand.Add(new Dagger(game));

            Player player2 = new Player("Player 2", game);
            player2.Hand.Add(new Dagger(game)); // So that player 2 can discard a weapon.
            player2.Hand.Add(new GreenKnight(game)); // Add powerfull foes so that player 4 can be eliminated in first stage.
            player2.Hand.Add(new Dragon(game));

            Player player3 = new Player("Player 3", game);
            player3.Hand.Add(new Horse(game));
            player3.Hand.Add(new Excalibur(game));
            player3.Hand.Add(new Amour(game));

            Player player4 = new Player("Player 4", game);
            player4.Hand.Add(new BattleAx(game));
            player4.Hand.Add(new Lance(game));

            game.StoryDeck.Push(new ProsperityEvent(game));
            game.StoryDeck.Push(new BoarHunt(game));

			player1.Behaviour = new HumanPlayer ();
			player2.Behaviour = new HumanPlayer ();
			player3.Behaviour = new HumanPlayer ();
			player4.Behaviour = new HumanPlayer ();

            game.AddPlayer(player1);
            game.AddPlayer(player2);
            game.AddPlayer(player3);
            game.AddPlayer(player4);

            game.Setup(shuffleDecks: false);

            return game;
        }
    }
}