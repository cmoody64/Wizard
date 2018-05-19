using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    class GameLoop
    {
        public GameLoop()
        {
            _players = new List<IPlayer>();
            _players.Add(new HumanPlayer());
            _deck = new Deck();            
        }

        // blocking method that executes the entirity of the game flow
        public void Run()
        {
            while (true)
                PlaySingleGame();
        }

        private void PlaySingleGame()
        {
            // setup deck and deal cards
            _deck.Shuffle();
            int rounds = _deck.Cards.Count / _players.Count;
            for (int round = 1; round <= rounds; round++)
            {
                DealDeck(round);

                // gameplay
                var x = 2;
            }
        }

        private void DealDeck(int roundNum)
        {
            for(int i = 0; i < roundNum; i++)
                foreach (var player in _players)
                    player.TakeCard(_deck.PopTop());
        }

        private List<IPlayer> _players;
        private Deck _deck;
    }
}
