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
			boarhunt.run ();

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
            eventCard.run();

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
            eventCard.run();

            Assert.AreEqual(1, game.Players[0].Rank.TotalShields());
            Assert.AreEqual(3, game.Players[1].Rank.TotalShields());
            Assert.AreEqual(3, game.Players[2].Rank.TotalShields());
        }

        [Test]
        public void QueensFavour() {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            game.Players[0].Rank.AddShields(10);

            QueensFavourEvent eventCard = new QueensFavourEvent(game);
            eventCard.run();

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


}
