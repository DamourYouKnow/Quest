using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

using Cards;

namespace Tests {
    public class CardDeckTests {
        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator DealCards() {
            // Deal an even deck of cards between two hands and check sizes
            // FIXME: This is inteded to fail for testing purposes. The collection in these classes aren't created.

            Hand hand1 = new Hand();
            Hand hand2 = new Hand();

            Deck deck = new Deck();
            // TODO: Build deck, deal and assert equal.

            yield return null;
            Assert.AreEqual(hand1.Cards.Count, hand2.Cards.Count);
        }
    }
}
