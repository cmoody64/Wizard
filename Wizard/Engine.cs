using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public class Engine
    {
        public Engine()
        {
            _players = new List<Player>();
            _players.Add(new HumanPlayer(this, "Barack"));
            _players.Add(new HumanPlayer(this, "Ronald"));
            _players.Add(new HumanPlayer(this, "George"));
            _deck = new Deck();
            Frontend = new ConsoleFrontend();
            _playerScores = new Dictionary<Player, int>();                      
        }

        // blocking method that executes the entirity of the game flow
        public void Run()
        {
            while (true)
                PlaySingleGame();
        }

        private void PlaySingleGame()
        {
            Frontend.DisplayStartGame();
            ResetPlayerScores();
            var curRoundBids = new Dictionary<Player, int>();
            // setup deck and deal cards
            _deck.Shuffle();
            int rounds = _deck.Cards.Count / _players.Count;
            for (int round = 1; round <= rounds; round++)
            {
                DealDeck(round);

                // gameplay
                PlaySingleRound(round);
            }
        }

        private void PlaySingleRound(int roundNum)
        {
            Frontend.DisplayStartRound(roundNum);
            var curRoundBids = new Dictionary<Player, int>();
            var curRoundResults = new Dictionary<Player, int>();

            // bid on current round
            _players.ForEach(player => curRoundBids[player] = player.MakeBid());

            // execute tricks and record results
            for (int trickNum = 1; trickNum <= roundNum; trickNum++)
            {
                Player winner = PlaySingleTrick(trickNum);
                curRoundResults[winner]++;
            }

            // resolve round scores

        }

        // executes a single trick and returns the player that won the trick
        private Player PlaySingleTrick(int trickNum)
        {
            Frontend.DisplayStartTrick(trickNum);
            return null;
        }

        private void DealDeck(int roundNum)
        {
            for(int i = 0; i < roundNum; i++)
                foreach (var player in _players)
                    player.TakeCard(_deck.PopTop());
        }

        private void ResetPlayerScores()
        {
            _players.ForEach(player => _playerScores[player] = 0);
        }

        private List<Player> _players;
        private Deck _deck;
        private Dictionary<Player, int> _playerScores;
        public IWizardFrontend Frontend { get; }        
    }
}
