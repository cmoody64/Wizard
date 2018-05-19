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
            _players = new List<IPlayer>();
            _players.Add(new HumanPlayer(this, "Barack"));
            _players.Add(new HumanPlayer(this, "Ronald"));
            _players.Add(new HumanPlayer(this, "George"));
            _deck = new Deck();
            Frontend = new ConsoleFrontend();
            _playerScores = new Dictionary<IPlayer, int>();                      
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
            var curRoundBids = new Dictionary<IPlayer, int>();
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
            Diction
            _players.ForEach(player)
            for (int trickNum = 1; trickNum <= roundNum; trickNum++)
            {
                PlaySingleTrick(trickNum);
            }
        }

        // executes a single trick and returns the player that won the trick
        private IPlayer PlaySingleTrick(int trickNum)
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

        private List<IPlayer> _players;
        private Deck _deck;
        private Dictionary<IPlayer, int> _playerScores;
        public IWizardFrontend Frontend { get; }        
    }
}
