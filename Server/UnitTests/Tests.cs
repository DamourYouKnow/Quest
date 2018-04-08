using System;
using System.Collections.Generic;
using NUnit.Framework;

using Quest.Core;
using Quest.Core.Cards;
using Quest.Core.Players;
using Quest.Core.Scenarios;
using Utils;

namespace UnitTests
{
    [TestFixture]
    public class GameTests
    {
        [Test]
        public void ThreePlayerAIGameTest()
        {
            QuestMatch game = new QuestMatch(logger: new Quest.Core.Logger("TestGame"));
            for (int i = 1; i <= 3; i++)
            {
                Player newPlayer = new Player("AI Player " + i, game);
                newPlayer.Behaviour = new Strategy2();
                game.AddPlayer(newPlayer);
            }

            game.Setup();
            game.RunGame();
        }
    }

    [TestFixture]
    public class CardTests
    {
        [Test]
        public void TestDraw()
        {
            QuestMatch game = ScenarioCreator.EmptyGame();
            AdventureDeck deck = new AdventureDeck(game);
            Player player = new Player("Test Player", game);
            player.Draw(deck, 10);
            Assert.AreEqual(player.Hand.Count, 10);
            Assert.AreEqual(deck.Count, deck.DeckSize - 10);
        }

        [Test]
        public void TransferCards()
        {
            // Transfer cards from player hand to battle area.
            KingArthur testCard = new KingArthur(ScenarioCreator.EmptyGame());
            Hand playerHand = new Hand(null);
            PlayerArea battleArea = new PlayerArea();
            playerHand.Add(testCard);
            playerHand.Transfer(battleArea, testCard);

            Assert.AreEqual(playerHand.Count, 0);
            Assert.AreEqual(battleArea.Count, 1);
        }

        [Test]
        public void QuestCardRunTest()
        {
            QuestMatch match = new QuestMatch();

            //match has no story to start
            Assert.IsTrue(match.CurrentStory == null);

            BoarHunt boarhunt = new BoarHunt(match);

            // These tests are no longer relevant!
            //boarhunt.Run();
            //match currentstory is initiated after quest.run
            //Assert.IsTrue(match.CurrentStory == boarhunt);

            //questFoes are initialized
            Assert.IsTrue(boarhunt.QuestFoes.Contains(typeof(Boar)));
        }

        [Test]
        public void AdventureDeckCheck()
        {
            Quest.Core.Logger log = new Quest.Core.Logger("AdventureDeckCheck");
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            AdventureDeck deck = new AdventureDeck(game);

            Assert.IsTrue(deck.Peek(deck) == "Test Of The Questing Beast");

            for (int i = 0; i < 77; i++)
            {
                deck.Draw();
                if (i == 2)
                {
                    Assert.IsTrue(deck.Peek(deck) == "Test Of Morgan Le Fey");
                }
                if (i == 10)
                {
                    Assert.IsTrue(deck.Peek(deck) == "Thieves");
                }
                if (i == 20)
                {
                    Assert.IsTrue(deck.Peek(deck) == "Saxons");
                }
                if (i == 30)
                {
                    Assert.IsTrue(deck.Peek(deck) == "Robber Knight");
                }
            }
        }

        [Test]
        public void StoryDeckCheck()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            StoryDeck deck = new StoryDeck(game);

            Assert.IsTrue(deck.Peek(deck) == "King's Call To Arms");

            for (int i = 0; i < 27; i++)
            {
                deck.Draw();
                if (i == 1)
                {
                    Assert.IsTrue(deck.Peek(deck) == "Chivalrous Deed");
                }
                if (i == 10)
                {
                    Assert.IsTrue(deck.Peek(deck) == "Tournament At York");
                }
                if (i == 20)
                {
                    Assert.IsTrue(deck.Peek(deck) == "Vanquish King Arthur's Enemies");
                }
            }
            Assert.IsTrue(deck.Peek(deck) == "Search For The Holy Grail");
        }

        [Test]
        public void QuestTest()
        {
            QuestMatch match = new QuestMatch(logger: new Quest.Core.Logger("QuestTest"));
            Player p1 = new Player("p1", match);
            Player p2 = new Player("p2", match);
            Player p3 = new Player("p3", match);
            Player p4 = new Player("p4", match);
            match.AddPlayer(p1);
            match.AddPlayer(p2);
            match.AddPlayer(p3);
            match.AddPlayer(p4);

            QuestCard quest = new BoarHunt(match);
            match.CurrentStory = quest;

            quest.SponsorshipResponse(p1, true);

            Assert.IsTrue((match.CurrentStory as QuestCard).Sponsor == p1);

            quest.ParticipationResponse(p2, true);
            quest.ParticipationResponse(p3, true);
            quest.ParticipationResponse(p4, true);

            Assert.IsFalse(quest.Participants.Contains(p1));
            Assert.IsTrue(quest.Participants.Contains(p2));
            Assert.IsTrue(quest.Participants.Contains(p3));
            Assert.IsTrue(quest.Participants.Contains(p4));

            Boar boar = new Boar(match);
            Sword sword = new Sword(match);
            Horse horse = new Horse(match);
            p1.Hand.Add(new List<Card>() { boar, sword, horse });

            quest.AddFoeStage(boar, new List<WeaponCard>() { sword, horse });
            Assert.AreEqual(35, quest.GetStage(1).BattlePoints());

            Thieves thieves = new Thieves(match);
            p1.Hand.Add(thieves);
            quest.AddFoeStage(thieves);
            Assert.AreEqual(5, quest.GetStage(2).BattlePoints());

            p2.BattleArea.Add(new Sword(match));
            p2.BattleArea.Add(new Excalibur(match));
            p2.BattleArea.Add(new Dagger(match));
            p2.BattleArea.Add(new Horse(match));
            p2.BattleArea.Add(new Lance(match));
            p2.BattleArea.Add(new BattleAx(match));

            Assert.AreEqual(90, p2.BattleArea.BattlePoints());

            p3.BattleArea.Add(new Dagger(match));
            p3.BattleArea.Add(new Excalibur(match));

            Assert.AreEqual(35, p3.BattleArea.BattlePoints());

            p4.BattleArea.Add(new Sword(match));

            Assert.AreEqual(10, p4.BattleArea.BattlePoints());

            quest.Resolve(); 

            Assert.AreEqual(2, quest.CurrentStage);

            Assert.IsTrue(quest.Participants.Contains(p2));
            Assert.IsTrue(quest.Participants.Contains(p3));
            Assert.IsFalse(quest.Participants.Contains(p4));

            Assert.IsTrue(p2.BattleArea.Cards.Count == 0);
            Assert.IsTrue(p3.BattleArea.Cards.Count == 0);
            Assert.IsTrue(p4.BattleArea.Cards.Count == 0);

            p2.BattleArea.Add(new Dagger(match));

            Assert.AreEqual(5, p2.BattleArea.BattlePoints());

            quest.Resolve();

            Assert.IsTrue(quest.Participants.Contains(p2));
            Assert.IsFalse(quest.Participants.Contains(p3));

            Assert.AreEqual(0, p1.Rank.Shields);
            Assert.AreEqual(2, p2.Rank.Shields);
            Assert.AreEqual(0, p3.Rank.Shields);
            Assert.AreEqual(0, p4.Rank.Shields);

            Assert.AreEqual(6, p1.Hand.Count);
            Assert.AreEqual(1, p2.Hand.Count);
            Assert.AreEqual(1, p3.Hand.Count);
            Assert.AreEqual(0, p4.Hand.Count);
        }
    }

    [TestFixture]
    public class GameManagerTests
    {
        [Test]
        public void SetupGame()
        {
            QuestMatch game = ScenarioCreator.GameWithDeal(3);

            // Test if each player has 12 adventure cards.
            foreach (Player player in game.Players)
            {
                Assert.AreEqual(player.Hand.Count, Constants.MaxHandSize);
                foreach (Card card in player.Hand.Cards)
                {
                    Assert.IsInstanceOf(typeof(AdventureCard), card);
                }
            }
        }
    }

    [TestFixture]
    public class EventTests
    {
        [Test]
        public void Prosperity()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            game.AttachLogger(new Quest.Core.Logger("TestProsperityEvent"));

            ProsperityEvent eventCard = new ProsperityEvent(game);
            eventCard.Run();

            // TODO: Player pulls event, run handler on draw.
            foreach (Player player in game.Players)
            {
                Assert.AreEqual(2, player.Hand.Count);
                foreach (Card card in player.Hand.Cards)
                {
                    Assert.IsInstanceOf(typeof(AdventureCard), card);
                }
            }
        }

        [Test]
        public void ChivalrousDeed()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            game.AttachLogger(new Quest.Core.Logger("TestChivalrousDeedEvent"));

            game.Players[0].Rank.AddShields(1);

            ChivalrousDeedEvent eventCard = new ChivalrousDeedEvent(game);
            eventCard.Run();

            Assert.AreEqual(1, game.Players[0].Rank.TotalShields());
            Assert.AreEqual(3, game.Players[1].Rank.TotalShields());
            Assert.AreEqual(3, game.Players[2].Rank.TotalShields());
        }

        [Test]
        public void QueensFavour()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(3);
            game.AttachLogger(new Quest.Core.Logger("TestQueensFavourEvent"));

            game.Players[0].Rank.AddShields(10);

            QueensFavourEvent eventCard = new QueensFavourEvent(game);
            eventCard.Run();

            Assert.AreEqual(0, game.Players[0].Hand.Count);
            Assert.AreEqual(2, game.Players[1].Hand.Count);
            Assert.AreEqual(2, game.Players[2].Hand.Count);

            foreach (Card card in game.Players[1].Hand.Cards)
            {
                Assert.IsInstanceOf(typeof(AdventureCard), card);
            }

            foreach (Card card in game.Players[2].Hand.Cards)
            {
                Assert.IsInstanceOf(typeof(AdventureCard), card);
            }
        }

        [Test]
        public void CourtCalled()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestCourtCalledEvent"));

            List<Card> testCards = new List<Card>() {
                new KingArthur(game),
                new SirLancelot(game),
                new Sword(game)
            };
            game.Players[0].BattleArea.Add(testCards);

            CourtCalledEvent eventCard = new CourtCalledEvent(game);
            eventCard.Run();

            foreach (Card card in game.Players[0].BattleArea.Cards)
            {
                Assert.IsNotInstanceOf(typeof(AllyCard), card);
            }
        }

        [Test]
        public void Pox()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.AttachLogger(new Quest.Core.Logger("TestPoxEvent"));
            game.Players[0].Rank.AddShields(1);
            game.Players[1].Rank.AddShields(1);

            PoxEvent eventCard = new PoxEvent(game);
            game.Players[0].Hand.Add(eventCard);
            eventCard.Run();

            Assert.AreEqual(1, game.Players[0].Rank.Shields);
            Assert.AreEqual(0, game.Players[1].Rank.Shields);
        }

        [Test]
        public void Plague()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestPlagueEvent"));
            PlagueEvent eventCard = new PlagueEvent(game);
            game.Players[0].Rank.AddShields(2);
            game.Players[0].Hand.Add(eventCard);
            eventCard.Run();
            Assert.AreEqual(0, game.Players[0].Rank.Shields);
        }
    }

    [TestFixture]
    public class PlayerTests
    {
        [Test]
        public void Ranking()
        {
            QuestMatch game = new QuestMatch();
            game.AttachLogger(new Quest.Core.Logger("TestRanking"));

            Player player = new Player("Test Player", game);
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

    [TestFixture]
    public class TestTests
    {

    }

    [TestFixture]
    public class TournamentTests {
        [Test]
        public void TestTournament() {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class Strategy1Tests{
		[Test]
		public void TestTournamentParticipation(){
			QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.AttachLogger(new Quest.Core.Logger("TestTournamentParticipation"));
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy1();
			Player otherPlayer = game.Players[1];
            TournamentAtCamelot tournament = new TournamentAtCamelot(game);
			
			//no player can rank up by winning tournament - don't participate
			otherPlayer.Rank.AddShields(1);
			aiPlayer.Rank.AddShields(1);
            Assert.IsFalse(aiPlayer.Behaviour.ParticipateInTournament(tournament));
			
			//other player has 11 shields, can rank up - participate
            otherPlayer.Rank.AddShields(10);
			Assert.IsTrue(aiPlayer.Behaviour.ParticipateInTournament(tournament));

            //make ai player a knight - 10 base BP
            aiPlayer.Rank.AddShields(10);

            //cards
            Amour amour = new Amour(game);
            Dagger dagger = new Dagger(game);
            Dagger dagger2 = new Dagger(game);
            SirGalahad sirGalahad = new SirGalahad(game);
            Sword sword = new Sword(game);
            Sword sword2 = new Sword(game);
            //other player can win - play strongest possible hand
            aiPlayer.Hand.Add(amour);//10
			aiPlayer.Hand.Add(dagger);//5
			aiPlayer.Hand.Add(dagger2);//5
			aiPlayer.Hand.Add(sirGalahad);//15
			aiPlayer.Hand.Add(sword);//10
			aiPlayer.Hand.Add(sword2);//10
			//expected: amour, dagger, SirGalahad, sword
			List<BattleCard> played1 = aiPlayer.Behaviour.PlayCardsInTournament(tournament, aiPlayer);
			Assert.AreEqual(4, played1.Count);
            aiPlayer.Play(played1);
            //40 BP from cards, 10 base BP from rank
            Assert.AreEqual(50, aiPlayer.BattlePointsInPlay());
        }

        [Test]
        public void TestTournamentParticipation2()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.AttachLogger(new Quest.Core.Logger("TestTournamentParticipation2"));
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy1();
            Player otherPlayer = game.Players[1];
            TournamentAtCamelot tournament = new TournamentAtCamelot(game);
            
            //ai player has 11 shields, can rank up - participate
            aiPlayer.Rank.AddShields(11);
            Assert.IsTrue(aiPlayer.Behaviour.ParticipateInTournament(tournament));

            //make other player a squire - 5 base BP, can't rank up
            otherPlayer.Rank.AddShields(1);

            //cards
            Amour amour = new Amour(game);
            Dagger dagger = new Dagger(game);
            Dagger dagger2 = new Dagger(game);
            SirGalahad sirGalahad = new SirGalahad(game);
            Sword sword = new Sword(game);
            Sword sword2 = new Sword(game);
            //other player can win - play strongest possible hand
            aiPlayer.Hand.Add(amour);//10
            aiPlayer.Hand.Add(dagger);//5
            aiPlayer.Hand.Add(dagger2);//5
            aiPlayer.Hand.Add(sirGalahad);//15
            aiPlayer.Hand.Add(sword);//10
            aiPlayer.Hand.Add(sword2);//10

            //expected: dagger, sword
            List<BattleCard> played2 = aiPlayer.Behaviour.PlayCardsInTournament(tournament, aiPlayer);
            Assert.IsTrue((played2.Contains(dagger2))||(played2.Contains(dagger)));
            Assert.IsTrue((played2.Contains(sword2)) || (played2.Contains(sword)));
            Assert.AreEqual(2, played2.Count);
            aiPlayer.Play(played2);
            //5 from a dagger, 10 from a sword, 10 from base BP
            Assert.AreEqual(25, aiPlayer.BattlePointsInPlay());
        }


        [Test]
        public void TestQuestSponsoring(){
			QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.AttachLogger(new Quest.Core.Logger("TestQuestSponsoring"));
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy1();
            Player otherPlayer = game.Players[1];

            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 Stages with bonus to Black Knight.
            game.CurrentStory = quest;
			
			// Test case where another player can win.
			//(conditions for whether or not to sponsor quest is same as strategy 2)
            otherPlayer.Rank.AddShields(21);
            Assert.IsFalse(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));
            otherPlayer.Rank.RemoveShields(10);
			
			//quest cards
			Dragon dragon = new Dragon(game);//50
			BlackKnight blackKnight = new BlackKnight(game);//35 (25+10)
			Mordred mordred = new Mordred(game);//30
			Thieves thieves = new Thieves(game);//5
			Boar boar = new Boar(game);//5 
			Lance lance = new Lance(game);//20
			Sword sword = new Sword(game);//10
			Dagger dagger = new Dagger(game);//5
			
			aiPlayer.Hand.Add(new TestOfValor(game));
            aiPlayer.Hand.Add(boar);
            aiPlayer.Hand.Add(thieves);
			//hand: boar, thieves, testOfValor - not enough bp
            Assert.IsFalse(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));

			aiPlayer.Hand.Add(blackKnight);
            aiPlayer.Hand.Add(lance);
			//hand: boar, thieves, test, blackknight, lance - enough bp
			Assert.IsTrue(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));
			
            /*
			aiPlayer.Hand.Remove(boar);
			aiPlayer.Hand.Remove(thieves);
            aiPlayer.Hand.Remove(lance);
			aiPlayer.Hand.Add(dragon);
			//hand: black knight, test, lance, dragon - enough bp
			//(last stage dragon, 2nd stage test, first black knight (no lance))
			Assert.IsTrue(aiPlayer.Behaviour.SponsorQuest(quest, aiPlayer.Hand));
            */
        }

        [Test]
        public void TestSetupQuest()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestSetupQuest"));
            Player sponsorAI = game.Players[0];
            sponsorAI.Behaviour = new Strategy1();

            // Setup quest
            RescueTheFairMaiden quest = new RescueTheFairMaiden(game);//3 stages
            game.CurrentStory = quest;
            quest.Sponsor = sponsorAI;

            //quest cards
            Dragon dragon = new Dragon(game);//50
            BlackKnight blackKnight = new BlackKnight(game);//35 (25+10)
            Thieves thieves = new Thieves(game);//5
            Boar boar = new Boar(game);//5 
            TestOfValor testOfValor = new TestOfValor(game);

            sponsorAI.Hand.Add(new List<Card>() { testOfValor, boar, thieves, dragon });

            List<AdventureCard>[] stages = sponsorAI.Behaviour.SetupQuest(quest, sponsorAI.Hand);
            Assert.AreEqual(3, stages.Length);

            //test last stage - should contain strongest foe (dragon)
            Assert.AreEqual(1, stages[2].Count);
            Assert.IsTrue(stages[2].Contains(dragon));

            //test second last stage - should contain test of valor
            Assert.AreEqual(1, stages[1].Count);
            Assert.IsTrue(stages[1].Contains(testOfValor));

            //test first stage
            Assert.AreEqual(1, stages[0].Count);
        }

        [Test]
        public void TestSetupQuest2()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestSetupQuest2"));
            Player sponsorAI = game.Players[0];
            sponsorAI.Behaviour = new Strategy1();

            // Setup quest
            SearchForTheQuestingBeast quest = new SearchForTheQuestingBeast(game); // 4 stages.
            game.CurrentStory = quest;
            quest.Sponsor = sponsorAI;

            //cards, no test
            Giant giant = new Giant(game);//40
            Lance lance = new Lance(game);//20
            Mordred mordred = new Mordred(game);//30
            Sword sword = new Sword(game);//10
            Sword sword2 = new Sword(game);//10
            Dagger dagger = new Dagger(game);//5
            Dagger dagger2 = new Dagger(game);//5
            Thieves thieves = new Thieves(game);//5
            Boar boar = new Boar(game);//5 

            sponsorAI.Hand.Add(new List<Card>() { giant, lance, thieves, boar, mordred,
                                                    dagger, dagger2, sword, sword2});

            List<AdventureCard>[] stages = sponsorAI.Behaviour.SetupQuest(quest, sponsorAI.Hand);
            Assert.AreEqual(4, stages.Length);

            //test last stage - should contain giant and lance
            Assert.AreEqual(2, stages[3].Count);
            Assert.IsTrue(stages[3].Contains(giant));
            Assert.IsTrue(stages[3].Contains(lance));

            //test 3rd stage - should be mordred and one of the daggers
            Assert.AreEqual(2, stages[2].Count);
            Assert.IsTrue(stages[2].Contains(mordred));
            Assert.IsTrue((stages[2].Contains(dagger))||(stages[2].Contains(dagger2)));

            //test 2nd stage - thieves or boar, and one of the swords
            Assert.AreEqual(2, stages[1].Count);
            Assert.IsTrue((stages[1].Contains(thieves)) || (stages[1].Contains(boar)));
            Assert.IsTrue((stages[1].Contains(sword)) || (stages[1].Contains(sword2)));
            //test 1st stage - thieves or boar (whichever wasn't played previously)
            Assert.AreEqual(1, stages[0].Count);
            Assert.IsTrue((stages[1].Contains(thieves)) || (stages[1].Contains(boar)));
        }

        [Test]
        public void TestQuestParticipation(){
			QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestQuestParticipation"));
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy1();

            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 stages.
            game.CurrentStory = quest;
			
			//cards
			Lance lance = new Lance(game);//20
			Dagger dagger = new Dagger(game);//5
			Sword sword = new Sword(game);//10
			SirGalahad sirGalahad = new SirGalahad(game);//15
			SirLancelot sirLancelot = new SirLancelot(game);//15
			KingPellinore kingPellinore = new KingPellinore(game);//10
			Mordred mordred = new Mordred(game);//30
			Thieves thieves = new Thieves(game);//5
			Boar boar = new Boar(game);//5 
			
			aiPlayer.Hand.Add(lance);
			aiPlayer.Hand.Add(dagger);
			aiPlayer.Hand.Add(sword);
			aiPlayer.Hand.Add(sirGalahad);
			aiPlayer.Hand.Add(boar);
			aiPlayer.Hand.Add(thieves);
			//hand: lance, dagger, sword, galahad, boar, thieves - not enough weapon/allies
			Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));
			
			aiPlayer.Hand.Add(sirLancelot);
			aiPlayer.Hand.Add(kingPellinore);
			//hand: lance, dagger, sword, galahad, lancelot, pelinore, boar, thieves
			Assert.IsTrue(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));
			
			aiPlayer.Hand.Remove(thieves);
			//not enough foes (to discard)
			Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));
			//2 foes, but mordred has too much bp
			aiPlayer.Hand.Add(mordred);
			Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));
		}
		
		[Test]
        public void TestPlayCardsInQuest(){
			//not testing bids for now
			QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.AttachLogger(new Quest.Core.Logger("TestPlayCardsInTest"));
            Player aiPlayer = game.Players[0];
            Player sponsorPlayer = game.Players[1];
            aiPlayer.Behaviour = new Strategy1();
			
			// Setup quest
            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 stages.
            game.CurrentStory = quest;
            quest.Sponsor = sponsorPlayer;
            //quest.AddParticipant(aiPlayer); // FIXME
			
			Thieves questThieves = new Thieves(game);
            Saxons questSaxons = new Saxons(game);
            RobberKnight questRobberKnight = new RobberKnight(game);
            sponsorPlayer.Hand.Add(new List<Card>() { questThieves, questSaxons, questRobberKnight });

            quest.AddFoeStage(questThieves);//5
            quest.AddFoeStage(questSaxons);//10
            quest.AddFoeStage(questRobberKnight);//15
			// Make player knight, 10 BP.
            aiPlayer.Rank.AddShields(5);

			//cards, no foes
			Lance lance = new Lance(game);//20
			Lance lance2 = new Lance(game);//20
			BattleAx battleAx = new BattleAx(game);//15
			SirGalahad sirGalahad = new SirGalahad(game);//15
			Amour amour = new Amour(game);//10
			Sword sword = new Sword(game);//10
			KingPellinore kingPellinore = new KingPellinore(game);//10
			aiPlayer.Hand.Add(lance);
			aiPlayer.Hand.Add(lance2);
			aiPlayer.Hand.Add(battleAx);
			aiPlayer.Hand.Add(sirGalahad);//play stage 2
			aiPlayer.Hand.Add(amour);//play stage 1
			aiPlayer.Hand.Add(sword);
			aiPlayer.Hand.Add(kingPellinore);
			
			//first stage: amour
			List<BattleCard> played = aiPlayer.Behaviour.PlayCardsInQuest(quest, aiPlayer.Hand);
            Assert.AreEqual(1, played.Count);
            Assert.IsTrue(played.Contains(amour));
            aiPlayer.Play(played);
            //quest.ResolveStage(); FIXME
			//2nd stage: galahad
			played = aiPlayer.Behaviour.PlayCardsInQuest(quest, aiPlayer.Hand);
            Assert.AreEqual(1, played.Count);
            Assert.IsTrue(played.Contains(sirGalahad));
            aiPlayer.Play(played);
            //quest.ResolveStage(); FIXME
			//3rd stage: a lance, battleAx, sword, kingPellinore
			played = aiPlayer.Behaviour.PlayCardsInQuest(quest, aiPlayer.Hand);
			Assert.AreEqual(4, played.Count);
			Assert.IsTrue((played.Contains(lance) || played.Contains(lance2)));
			Assert.IsTrue(played.Contains(battleAx));
			Assert.IsTrue(played.Contains(sword));
			Assert.IsTrue(played.Contains(kingPellinore));
		}
	}

    [TestFixture]
    public class Strategy2Tests
    {
        [Test]
        public void TestTournamentParticipation()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestTournamentParticipation"));
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
        public void TestQuestSponsoring()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.AttachLogger(new Quest.Core.Logger("TestQuestSponsoring"));
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy2();
            Player winningPlayer = game.Players[1];

            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 Stages with bonus to Black Knight.
            game.CurrentStory = quest;

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
        public void TestQuestParticipation()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestQuestParticipation"));
            Player aiPlayer = game.Players[0];
            aiPlayer.Behaviour = new Strategy2();

            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 stages.
            game.CurrentStory = quest;

            // Make player knight, 10 BP.
            aiPlayer.Rank.AddShields(5);

            // Test cards.
            KingArthur arthur = new KingArthur(game); // 10 BP.
            SirLancelot lancelot = new SirLancelot(game); // 15 BP.
            SirGalahad galahad = new SirGalahad(game); // 15 BP.
            Boar boar = new Boar(game); // 5 BP, should be discarded.
            Thieves thieves = new Thieves(game); // 5 BP, should be discarded.
            BlackKnight blackKnight = new BlackKnight(game); // 25 BP, should not be discarded.
            Excalibur excalibur = new Excalibur(game); // +30 BP.
            Lance lance = new Lance(game); // + 20 BP.

            // Cannot increase for all 3 stages, expect false.
            aiPlayer.Hand.Add(boar);
            aiPlayer.Hand.Add(thieves);
            aiPlayer.Hand.Add(blackKnight);
            aiPlayer.Hand.Add(arthur);
            aiPlayer.Hand.Add(lancelot);
            aiPlayer.Hand.Add(galahad);
            Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));

            // Add weapons, expect true.
            aiPlayer.Hand.Add(excalibur);
            aiPlayer.Hand.Add(lance);
            Assert.IsTrue(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));

            // Remove discardable foe less than 25 BP, expect false.
            aiPlayer.Hand.Remove(boar);
            Assert.IsFalse(aiPlayer.Behaviour.ParticipateInQuest(quest, aiPlayer.Hand));
        }

        [Test]
        public void TestPlayCardsInQuest()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(2);
            game.AttachLogger(new Quest.Core.Logger("TestPlayCardsInTest"));
            Player aiPlayer = game.Players[0];
            Player sponsorPlayer = game.Players[1];
            aiPlayer.Behaviour = new Strategy2();

            // Setup quest
            RescueTheFairMaiden quest = new RescueTheFairMaiden(game); // 3 stages.
            game.CurrentStory = quest;
            quest.Sponsor = sponsorPlayer;
            //quest.AddParticipant(aiPlayer); FIXME

            Thieves questThieves = new Thieves(game);
            Saxons questSaxons = new Saxons(game);
            RobberKnight questRobberKnight = new RobberKnight(game);
            sponsorPlayer.Hand.Add(new List<Card>() { questThieves, questSaxons, questRobberKnight });

            quest.AddFoeStage(questThieves);
            quest.AddFoeStage(questSaxons);
            quest.AddFoeStage(questRobberKnight);

            // Make player knight, 10 BP.
            aiPlayer.Rank.AddShields(5);

            // Test cards.
            Amour amour1 = new Amour(game); // 10 BP.
            Amour amour2 = new Amour(game); // Hopefully only one is played.
            SirGawain gawain = new SirGawain(game); // 10 BP.
            SirTristan tristan = new SirTristan(game); // 10 BP.
            SirGalahad galahad = new SirGalahad(game); // 15 BP.
            BattleAx axe = new BattleAx(game); // +5 BP
            aiPlayer.Hand.Add(amour1);
            aiPlayer.Hand.Add(amour2);
            aiPlayer.Hand.Add(gawain);
            aiPlayer.Hand.Add(tristan);
            aiPlayer.Hand.Add(galahad);
            aiPlayer.Hand.Add(axe);

            // Test first stage. Amour should be played first.
            List<BattleCard> played = aiPlayer.Behaviour.PlayCardsInQuest(quest, aiPlayer.Hand);
            Assert.AreEqual(1, played.Count);
            Assert.IsTrue((played.Contains(amour1) || played.Contains(amour2)));
            aiPlayer.Play(played);
            //quest.ResolveStage(); // FIXME

            // Does allies get played second?
            played = aiPlayer.Behaviour.PlayCardsInQuest(quest, aiPlayer.Hand);
            Assert.AreEqual(2, played.Count);
            Assert.IsTrue(played.Contains(gawain));
            Assert.IsTrue(played.Contains(tristan));
            aiPlayer.Play(played);
            //quest.ResolveStage(); FIXME

            // Does weapon (and galahad) get played last?
            played = aiPlayer.Behaviour.PlayCardsInQuest(quest, aiPlayer.Hand);
            Assert.AreEqual(2, played.Count);
            Assert.IsTrue(played.Contains(galahad));
            Assert.IsTrue(played.Contains(axe));
        }

        [Test]
        public void TestDiscardAfterWinningTest()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void TestSetupTest()
        {
            QuestMatch game = ScenarioCreator.GameNoDeal(1);
            game.AttachLogger(new Quest.Core.Logger("TestSetupQuest"));
            Player sponsorAI = game.Players[0];
            sponsorAI.Behaviour = new Strategy2();

            // Setup quest
            SearchForTheQuestingBeast quest = new SearchForTheQuestingBeast(game); // 4 stages.
            game.CurrentStory = quest;
            quest.Sponsor = sponsorAI;

            // Test cards.
            TestOfTemptation testOfTemptation = new TestOfTemptation(game); // Play second last (stage 3).
            Boar boar = new Boar(game); // 5 BP.
            Saxons saxons = new Saxons(game); // 10 BP.
            Mordred mordred = new Mordred(game); // 40 BP, played last stage.
            Sword sword = new Sword(game); // 10 BP, played last stage.

            sponsorAI.Hand.Add(new List<Card>() { testOfTemptation, boar, saxons, mordred, sword });

            List<AdventureCard>[] stages = sponsorAI.Behaviour.SetupQuest(quest, sponsorAI.Hand);
            Assert.AreEqual(4, stages.Length);

            // Validate stage 1.
            Assert.AreEqual(1, stages[0].Count);
            Assert.IsTrue(stages[0].Contains(boar));

            // Validate stage 2.
            Assert.AreEqual(1, stages[1].Count);
            Assert.IsTrue(stages[1].Contains(saxons));

            // Validate stage 3.
            Assert.AreEqual(1, stages[2].Count);
            Assert.IsTrue(stages[2].Contains(testOfTemptation));

            // Validate stage 4.
            Assert.AreEqual(2, stages[3].Count);
            Assert.IsTrue(stages[3].Contains(mordred));
            Assert.IsTrue(stages[3].Contains(sword));
        }
    }
}
