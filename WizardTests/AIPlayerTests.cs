using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard.Tests
{
    [TestClass()]
    public class AIPlayerTests
    {
        [TestMethod()]
        public void MakeTurnTest()
        {
            var testFrontend = new ConsoleFrontend();

            var roundNum = 4;
            var deck = new Deck();
            deck.Shuffle();

            var aiPlayer = new AIPlayer(testFrontend, "wizardAI");
            var humanPlayer1 = new HumanPlayer(testFrontend, "Connor");
            var humanPlayer2 = new HumanPlayer(testFrontend, "Diana");
            for(int i = 0; i < roundNum; i++)
            {
                aiPlayer.TakeCard(deck.PopTop());
                humanPlayer1.TakeCard(deck.PopTop());
                humanPlayer2.TakeCard(deck.PopTop());
            }            

            var testPlayers = new List<Player>
            {
                aiPlayer,
                humanPlayer1,
                humanPlayer2
            };
            var testGameContext = new GameContext(testPlayers);

            var curRound = new RoundContext(roundNum, CardSuite.DIAMONDS);
            var trick1 = new TrickContext(1);
            trick1.CardsPlayed.Add(new Card(CardValue.JACK, CardSuite.CLUBS));

            curRound.Tricks.Add(trick1);
            testGameContext.Rounds.Add(curRound);

            aiPlayer.MakeTurn(testGameContext);
        }
    }
}