using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System;
using System.Collections.Generic;
using NUnit.Framework;

using Quest.Core;
using Quest.Core.Cards;
using Quest.Core.Players;
using Quest.Core.Scenarios;
using Utils;

namespace NUnitTesting {
    [TestFixture]
    public class CardTests {
        [Test]
        public void TestDraw() {
            QuestMatch game = ScenarioCreator.EmptyGame();
            AdventureDeck deck = new AdventureDeck(game);
            Player player = new Player("Test Player", game);
            player.Draw(deck, 10);
            Assert.AreEqual(player.Hand.Count, 10);
            Assert.AreEqual(deck.Count, deck.DeckSize - 10);
        }

        [Test]
        public void TransferCards() {
            // Transfer cards from player hand to battle area.
            KingArthur testCard = new KingArthur(ScenarioCreator.EmptyGame());
            Hand playerHand = new Hand();
            BattleArea battleArea = new BattleArea();
            playerHand.Add(testCard);
            playerHand.Transfer(battleArea, testCard);

            Assert.AreEqual(playerHand.Count, 0);
            Assert.AreEqual(battleArea.Count, 1);
        }

		[Test]
		public void QuestCardRunTest(){
			QuestMatch match = new QuestMatch ();

			//match has no story to start
			Assert.IsTrue (match.CurrentStory == null);

			BoarHunt boarhunt = new BoarHunt(match);
			boarhunt.Run ();

			//match currentstory is initiated after quest.run
			Assert.IsTrue (match.CurrentStory == boarhunt);

			//questFoes are initialized
			Assert.IsTrue (boarhunt.QuestFoes.Contains (typeof(Boar)));
		}

		[Test]
		public void AdventureDeckCheck(){
			QuestMatch game = ScenarioCreator.GameNoDeal(3);
			AdventureDeck deck = new AdventureDeck(game);

			Assert.IsTrue (deck.Peek (deck) == "Test Of The Questing Beast");

			for (int i = 0; i < 77; i++) {
				deck.Draw ();
				if (i == 2) {
					Assert.IsTrue (deck.Peek (deck) == "Test Of Morgan Le Fey");
				}
				if (i == 10) {
					Assert.IsTrue (deck.Peek (deck) == "Thieves");
				}
				if (i == 20) {
					Assert.IsTrue (deck.Peek (deck) == "Saxons");
				}
				if (i == 30) {
					Assert.IsTrue (deck.Peek (deck) == "Robber Knight");
				}
			}
			Assert.IsTrue (deck.Peek (deck) == "Dagger"); 
		}

		[Test]
		public void StoryDeckCheck(){
			QuestMatch game = ScenarioCreator.GameNoDeal(3);
			StoryDeck deck = new StoryDeck(game);

			Assert.IsTrue (deck.Peek (deck) == "King's Call To Arms");

			for (int i = 0; i < 27; i++) {
				deck.Draw ();
				if (i == 1) {
					Assert.IsTrue (deck.Peek (deck) == "Chivalrous Deed");
				}
				if (i == 10) {
					Assert.IsTrue (deck.Peek (deck) == "Tournament At York");
				}
				if (i == 20) {
					Assert.IsTrue (deck.Peek (deck) == "Vanquish King Arthur's Enemies");
				}
			}
			Assert.IsTrue (deck.Peek (deck) == "Search For The Holy Grail"); 
		}

		[Test]
		public void QuestAreaTest(){
			QuestMatch match = new QuestMatch (new Quest.Core.Logger ("QuestAreaTest"));
			QuestArea qarea = new QuestArea (new List<Card>());
			Card card1 = new Boar (match);
			qarea.Add (card1);

			Assert.IsTrue (qarea.MainCard == card1);

			Card card2 = new TestOfValor (match);
			qarea.Add (card2);

			Assert.IsTrue (qarea.MainCard == card1);
			Assert.IsFalse (qarea.Cards.Contains (card2));

			Assert.AreEqual (5, qarea.BattlePoints());

			Card card3 = new Sword (match);

			qarea.Add (card3);

			Assert.IsTrue (qarea.MainCard == card1);
			Assert.IsTrue (qarea.Cards.Contains (card3));

			Assert.AreEqual (15, qarea.BattlePoints());

			match.CurrentStory = new BoarHunt (match);

			Assert.AreEqual (25, qarea.BattlePoints ());
		}

		[Test]
		public void QuestTest(){
			QuestMatch match = new QuestMatch (new Quest.Core.Logger ("QuestTest"));
			Player p1 = new Player ("p1", match);
			Player p2 = new Player ("p2", match);
			Player p3 = new Player ("p3", match);
			Player p4 = new Player ("p4", match);

			QuestCard quest = new BoarHunt (match);
			quest.Run ();

			quest.Sponsor = p1;

			Assert.IsTrue ((match.CurrentStory as QuestCard).Sponsor == p1);

            quest.AddParticipant(p2);
            quest.AddParticipant(p3);
            quest.AddParticipant(p4);

			Assert.IsTrue(quest.Participants.Contains (p2));

            quest.AddFoeStage(new Boar(match), new List<WeaponCard>() { new Sword(match), new Horse(match)});

			QuestArea qa1 = new QuestArea(new List<Card>());
			qa1.Add (new Boar(match));
			qa1.Add (new Sword(match));
			qa1.Add (new Horse(match));
			lqa.Add (qa1);
			Assert.AreEqual (35, qa1.BattlePoints());

			QuestArea qa2 = new QuestArea (new List<Card>());
			qa2.Add (new Thieves(match));
			lqa.Add (qa2);

			Assert.AreEqual (5, qa2.BattlePoints());

			quest.Stages = lqa;

			p2.BattleArea.Add (new Sword(match));
			p2.BattleArea.Add (new Excalibur(match));
			p2.BattleArea.Add (new Dagger(match));
			p2.BattleArea.Add (new Horse(match));
			p2.BattleArea.Add (new Lance (match));
			p2.BattleArea.Add (new BattleAx (match));

			Assert.AreEqual (90, p2.BattleArea.BattlePoints ());

			p3.BattleArea.Add (new Dagger (match));
			p3.BattleArea.Add (new Excalibur (match));

			Assert.AreEqual (35, p3.BattleArea.BattlePoints ());

			p4.BattleArea.Add (new Sword (match));

			Assert.AreEqual (10, p4.BattleArea.BattlePoints ());

			quest.Run ();

			quest.ResolveStage ();

			Assert.AreEqual (2, quest.CurrentStage);

			Assert.IsTrue (quest.Participants.Contains (p2));
			Assert.IsTrue (quest.Participants.Contains (p3));
			Assert.IsFalse (quest.Participants.Contains (p4));

			Assert.IsTrue (p2.BattleArea.Cards.Count == 0);
			Assert.IsTrue (p3.BattleArea.Cards.Count == 0);
			Assert.IsTrue (p4.BattleArea.Cards.Count == 0);

			p2.BattleArea.Add (new Dagger (match));

			Assert.AreEqual (5, p2.BattleArea.BattlePoints());

			quest.ResolveStage ();

			Assert.IsTrue (quest.Participants.Contains (p2));
			Assert.IsFalse (quest.Participants.Contains (p3));

			Assert.AreEqual (0, p1.Rank.Shields);
			Assert.AreEqual (2, p2.Rank.Shields);
			Assert.AreEqual (0, p3.Rank.Shields);
			Assert.AreEqual (0, p4.Rank.Shields);

			Assert.AreEqual (6, p1.Hand.Count);
			Assert.AreEqual (0, p2.Hand.Count);
			Assert.AreEqual (0, p3.Hand.Count);
			Assert.AreEqual (0, p4.Hand.Count);
		}
    }

    public class GameManagerTests {
        [Test]
        public void SetupGame() {
            QuestMatch game = ScenarioCreator.GameWithDeal(3);

            // Test if each player has 12 adventure cards.
            foreach (Player player in game.Players) {
                Assert.AreEqual(player.Hand.Count, Constants.MaxHandSize);
                foreach (Card card in player.Hand.Cards) {
                    Assert.IsInstanceOf(typeof(AdventureCard), card);
                }
            }
        }
    }

    public class EventTests {
        [Test]
        public void Prosperity() {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            ProsperityEvent eventCard = new ProsperityEvent(game);
            eventCard.Run();

            // TODO: Player pulls event, run handler on draw.
            foreach (Player player in game.Players) {
                Assert.AreEqual(2, player.Hand.Count);
                foreach (Card card in player.Hand.Cards) {
                    Assert.IsInstanceOf(typeof(AdventureCard), card);
                }
            } 
        }

        [Test]
        public void ChivalrousDeed() {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            game.Players[0].Rank.AddShields(1);

            ChivalrousDeedEvent eventCard = new ChivalrousDeedEvent(game);
            eventCard.Run();

            Assert.AreEqual(1, game.Players[0].Rank.TotalShields());
            Assert.AreEqual(3, game.Players[1].Rank.TotalShields());
            Assert.AreEqual(3, game.Players[2].Rank.TotalShields());
        }

        [Test]
        public void QueensFavour() {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            game.Players[0].Rank.AddShields(10);

            QueensFavourEvent eventCard = new QueensFavourEvent(game);
            eventCard.Run();

            Assert.AreEqual(0, game.Players[0].Hand.Count);
            Assert.AreEqual(2, game.Players[1].Hand.Count);
            Assert.AreEqual(2, game.Players[2].Hand.Count);

            foreach (Card card in game.Players[1].Hand.Cards) {
                Assert.IsInstanceOf(typeof(AdventureCard), card);
            }

            foreach (Card card in game.Players[2].Hand.Cards) {
                Assert.IsInstanceOf(typeof(AdventureCard), card);
            }
        }

        [Test]
        public void CourtCalled() {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);

            List<Card> testCards = new List<Card>() {
                new KingArthur(game),
                new SirLancelot(game),
                new Sword(game)
            };
            game.Players[0].BattleArea.Add(testCards);

            CourtCalledEvent eventCard = new CourtCalledEvent(game);
            eventCard.Run();

            foreach (Card card in game.Players[0].BattleArea.Cards) {
                Assert.IsNotInstanceOf(typeof(AllyCard), card);
            }
        }

        [Test]
        public void Pox() {
            QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.Players[0].Rank.AddShields(1);
            game.Players[1].Rank.AddShields(1);

            PoxEvent eventCard = new PoxEvent(game);
            game.Players[0].Hand.Add(eventCard);
            eventCard.Run();

            Assert.AreEqual(1, game.Players[0].Rank.Shields);
            Assert.AreEqual(0, game.Players[1].Rank.Shields);
        }

        [Test]
        public void Plague() {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            PlagueEvent eventCard = new PlagueEvent(game);
            game.Players[0].Rank.AddShields(2);
            game.Players[0].Hand.Add(eventCard);
            eventCard.Run();
            Assert.AreEqual(0, game.Players[0].Rank.Shields);
        }
    }

    public class PlayerTests {
        [Test]
        public void Ranking() {
            Player player = new Player("Test Player");
            Assert.AreEqual(Rank.Squire, player.Rank.Value);
            Assert.AreEqual(0, player.Rank.Shields);

            player.Rank.AddShields(5);
            Assert.AreEqual(Rank.Knight, player.Rank.Value);
            Assert.AreEqual(0, player.Rank.Shields);

            player.Rank.AddShields(8);
            Assert.AreEqual(Rank.ChampionKnight, player.Rank.Value);
            Assert.AreEqual(1, player.Rank.Shields);

            player.Rank.RemoveShields(2);
            Assert.AreEqual(Rank.ChampionKnight, player.Rank.Value);
            Assert.AreEqual(0, player.Rank.Shields);

            player.Rank.AddShields(12);
            Assert.AreEqual(Rank.KnightOfTheRoundTable, player.Rank.Value);
            Assert.AreEqual(2, player.Rank.Shields);
        }
    }

    public class TestTests {
       
    }

    public class Strategy2Tests {
        [Test]
        public void TestTournamentParticipation() {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy2();
            TournamentAtCamelot tournament = new TournamentAtCamelot(game);

            // Test tournament participation.
            Assert.IsTrue(aiPlayer.Behaviour.ParticipateInTournament(tournament));

            // Test best possible battle points.
            // 5 BP from rank.
            aiPlayer.Hand.Add(new SirGalahad(game)); // 15 BP.
            aiPlayer.Hand.Add(new Sword(game)); // 10 BP.
            aiPlayer.Hand.Add(new Sword(game)); // 10 BP, should not be played.
            aiPlayer.Hand.Add(new Amour(game)); // 10 BP.

            // Should play SirGalahad, sword, and amour.
            List<BattleCard> played = aiPlayer.Behaviour.PlayCardsInTournament(tournament, aiPlayer);
            Assert.AreEqual(3, played.Count);
            aiPlayer.Play(played);
            Assert.AreEqual(40, aiPlayer.BattlePointsInPlay());

            // Test playing as few cards as possible to get 50 battle points.
            aiPlayer.BattleArea.Transfer(aiPlayer.Hand, aiPlayer.BattleArea.Cards);
            aiPlayer.Hand.Add(new Excalibur(game));

            played = aiPlayer.Behaviour.PlayCardsInTournament(tournament, aiPlayer);
            Assert.AreEqual(2, played.Count);
            aiPlayer.Play(played);
            Assert.AreEqual(50, aiPlayer.BattlePointsInPlay());
        }

        [Test]
        public void TestQuestSponsoring() {
            QuestMatch game = ScenarioCreator.GameNoDeal(2);
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy2();
            Player winningPlayer = game.Players[1];

            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 Stages with bonus to Black Knight.

            // Test case where another player can win.
            winningPlayer.Rank.AddShields(21);
            Assert.IsFalse(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));
            winningPlayer.Rank.RemoveShields(10);

            // Test cards.
            Boar boar = new Boar(game); // 5 BP
            Thieves thieves = new Thieves(game); // 5 BP
            BlackKnight blackKnight = new BlackKnight(game); // Should be worth 35 BP, not 25.
            GreenKnight greenKnight = new GreenKnight(game);
            Mordred mordred = new Mordred(game); // 30 BP.
            Lance lance = new Lance(game); // +20 BP.

            // Ensure having a test card is taken into consideration for the next tests.
            aiPlayer.Hand.Add(new TestOfValor(game));

            // First case, not enough battle points in second stage, expect false.
            aiPlayer.Hand.Add(boar);
            aiPlayer.Hand.Add(thieves);
            Assert.IsFalse(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));

            // Add weapon, expect true.
            aiPlayer.Hand.Add(lance);
            Assert.IsTrue(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));
            aiPlayer.Hand.Remove(lance);
            aiPlayer.Hand.Remove(boar);
            aiPlayer.Hand.Remove(thieves);

            // Green knight and black knight test, black night quest bonuse should be considered, expect true.
            aiPlayer.Hand.Add(blackKnight);
            aiPlayer.Hand.Add(greenKnight);
            Assert.IsTrue(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));
        }

        [Test]
        public void TestQuestParticipation() {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy2();

            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 stages.

            // Make player knight, 10 BP.
            aiPlayer.Rank.AddShields(5);

            // Test cards.
            KingArthur arthur = new KingArthur(game); // 10 BP.
            SirLancelot lancelot = new SirLancelot(game); // 15 BP.
            SirGalahad galahad = new SirGalahad(game); // 15 BP.
            Boar boar = new Boar(game); // 5 BP, should be discarded.
            Thieves thieves = new Thieves(game);
            BlackKnight blackKnight = new BlackKnight(game); // 25 BP, should not be discarded.
            Dagger dagger = new Dagger(game); // +5 BP.
            Lance lance = new Lance(game);

            // Cannot increase for all 3 stages, expect false.
            aiPlayer.Hand.Add(boar);
            aiPlayer.Hand.Add(thieves);
            aiPlayer.Hand.Add(blackKnight);
            aiPlayer.Hand.Add(arthur);
            aiPlayer.Hand.Add(lancelot);
            aiPlayer.Hand.Add(galahad);
            Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));

            // Add weapon, expect false, still increments by +5 for a stage.
            aiPlayer.Hand.Add(dagger);
            Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));

            // Add another weapon, expect true.
            aiPlayer.Hand.Add(lance);
            Assert.IsTrue(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));

            // Remove discardable foe less than 25 BP, expect false.
            aiPlayer.Hand.Remove(boar);
            Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));
        }

        [Test]
        public void TestPlayCardsInQuest() {
            // TODO: Create new preset scenario for fully initialized quest.
            QuestMatch game = ScenarioCreator.GameNoDeal(2);
            Player aiPlayer = game.Players[0];
            Player sponsorPlayer = game.Players[1];
            aiPlayer.Behaviour = new Strategy2();

            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 stages.
            quest.Sponsor = sponsorPlayer;
            quest.AddParticipant(aiPlayer);

            // Make player knight, 10 BP.
            aiPlayer.Rank.AddShields(5);

            // Test cards.
            Amour amour = new Amour(game);
            SirTristan tristan = new SirTristan(game);
            Sword sword = new Sword(game);

            // Test first stage. Amour should be played first.

            // Does ally get played second?

            // Does weapon get played last?

            // Test last stage, strongest valid combination
        }

        [Test]
        public void TestDiscardAfterWinningTest() {

        }
    }
}
