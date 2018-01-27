using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System;
using System.Collections.Generic;
using NUnit.Framework;

using Quest.Core;
using Quest.Core.Cards;
using Quest.Core.Players;

namespace NUnitTesting {
    [TestFixture]
    public class CardTests {
        [Test]
        public void TestDeal() {
            AdventureDeck deck = new AdventureDeck();
            Player player = new Player("Test Player");

            deck.Deal(player, 10);
            Assert.AreEqual(player.Hand.Count, 10);
            Assert.AreEqual(deck.Count, AdventureDeck.deckSize - 10);
        }

        [Test]
        public void TransferCards() {
            // Transfer cards from player hand to battle area.
            AllyCard testCard = new AllyCard();
            Hand playerHand = new Hand();
            BattleArea battleArea = new BattleArea();
            playerHand.Add(testCard);
            playerHand.Transfer(battleArea, testCard);

            Assert.AreEqual(playerHand.Count, 0);
            Assert.AreEqual(battleArea.Count, 1);
        }
    }

    public class GameManagerTests {
        [Test]
        public void SetupGame() {
            GameManager game = new GameManager();

            // TODO: Write code for generating preset scenarios.
            List<Player> players = new List<Player>();
            players.Add(new Player("Test Player 1"));
            players.Add(new Player("Test Player 2"));
            players.Add(new Player("Test Player 3"));

            foreach (Player player in players) {
                game.AddPlayer(player);
            }

            game.Setup();

            // Test if each player has 12 adventure cards.
            foreach (Player player in players) {
                Assert.AreEqual(player.Hand.Count, Constants.MaxHandSize);
                foreach (Card card in player.Hand.Cards) {
                    Assert.IsInstanceOf(typeof(AdventureCard), card);
                }
            }
        }
    }
}
