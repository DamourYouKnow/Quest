using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System;
using System.Collections;
using NUnit.Framework;

using Quest.Core;

namespace NUnitTesting {
    [TestFixture]
    public class CardTests {
        [Test]
        public void TestTest() {
            // If this doesn't pass things are bad.
            Assert.Pass();
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

    public class PlayerTests {
        [Test]
        public void SetupGame() {
            GameManager game = new GameManager();
            game.AddPlayer(new Player("Test Player 1"));
            game.AddPlayer(new Player("Test Player 2"));
            game.AddPlayer(new Player("Test Player 3"));

            // TODO: Test dealing required cards across players.
            // TODO: Validate game state.
            throw new NotImplementedException();
        }
    }
}
