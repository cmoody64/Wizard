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
            _players.Add(new HumanPlayer(this._frontend, "Barack"));
            _players.Add(new HumanPlayer(this._frontend, "Ronald"));
            _players.Add(new HumanPlayer(this._frontend, "George"));
            _deck = new Deck();
            _frontend = new ConsoleFrontend();
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
            _frontend.DisplayStartGame();
            List<Player> players = _frontend.PromptPlayerCreation();

            _gameContext = new GameContext();
            _gameContext.AddPlayers(players);

            // ResetPlayerScores();
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
            _frontend.DisplayStartRound(roundNum);
            var curRoundBids = new Dictionary<Player, int>();
            var curRoundResults = new Dictionary<Player, int>();
            CardSuite trumpSuite = _deck.Cards.Count > 0 ? _deck.PopTop().Suite : CardSuite.SPECIAL;
            
            // bid on current round
            _players.ForEach(player => curRoundBids[player] = player.MakeBid());

            // execute tricks and record results
            for (int trickNum = 1; trickNum <= roundNum; trickNum++)
            {
                Player winner = PlaySingleTrick(trickNum);
                curRoundResults[winner]++;
            }

            // resolve round scores
            _players.ForEach(player =>
            {
                int diff = Math.Abs(curRoundBids[player] - curRoundResults[player]);
                if (diff == 0)
                    _playerScores[player] += (BASELINE_SCORE + curRoundBids[player] * HIT_SCORE);
                else
                    _playerScores[player] += (diff * MISS_SCORE);
            });
        }

        // executes a single trick and returns the player that won the trick
        private Player PlaySingleTrick(int trickNum, CardSuite trumpSuite)
        {
            _frontend.DisplayStartTrick(trickNum);
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
        private IWizardFrontend _frontend { get; }
        private GameContext _gameContext;

        private readonly int BASELINE_SCORE = 20;
        private readonly int HIT_SCORE = 10;
        private readonly int MISS_SCORE = -10;
    }
}
