﻿using UnityEngine;
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
		public void EqualTest(){
			KingArthur king = new KingArthur (null);
			SearchForTheHolyGrail grail1 = new SearchForTheHolyGrail (null);
			SearchForTheHolyGrail grail2 = new SearchForTheHolyGrail (null);

			Assert.IsTrue (king == king);
			Assert.IsTrue (grail1 == grail2);
			Assert.IsFalse (king == grail1);
		}

		[Test]
		public void NotEqualTest(){
			KingArthur king = new KingArthur (null);
			SearchForTheHolyGrail grail1 = new SearchForTheHolyGrail (null);
			SearchForTheHolyGrail grail2 = new SearchForTheHolyGrail (null);

			Assert.IsFalse (king != king);
			Assert.IsFalse (grail1 != grail2);
			Assert.IsTrue (king != grail1);
		}
    }

    public class GameManagerTests {
        [Test]
        public void SetupGame() {
            QuestMatch game = new QuestMatch();

            // TODO: Write code for generating preset scenarios.
            List<Player> players = new List<Player>();
            players.Add(new Player(null, "Test Player 1"));
            players.Add(new Player(null, "Test Player 2"));
            players.Add(new Player(null, "Test Player 3"));

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
