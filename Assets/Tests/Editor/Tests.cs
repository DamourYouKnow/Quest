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

namespace NUnitTesting {
    [TestFixture]
    public class CardTests {
        [Test]
        public void TestDraw() {
            AdventureDeck deck = new AdventureDeck(null);
            Player player = new Player(null, "Test Player");

            player.Draw(deck, 10);
            Assert.AreEqual(player.Hand.Count, 10);
            Assert.AreEqual(deck.Count, deck.DeckSize - 10);
        }

        [Test]
        public void TransferCards() {
            // Transfer cards from player hand to battle area.
            KingArthur testCard = new KingArthur(null);
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

			Assert.IsTrue (match.CurrentStory == null);

			BoarHunt boarhunt = new BoarHunt(match);
			boarhunt.run ();

			Assert.IsTrue (match.CurrentStory == boarhunt);
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
            eventCard.RunEvent();

            // TODO: Player pulls event, run handler on draw.
            foreach (Player player in game.Players) {
                Assert.AreEqual(2, player.Hand.Count);
                foreach (Card card in player.Hand.Cards) {
                    Assert.IsInstanceOf(typeof(AdventureCard), card);
                }
            } 
        }
    }
}
