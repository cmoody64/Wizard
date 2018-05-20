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
            _frontend = new ConsoleFrontend();
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
            _frontend.DisplayStartGame();
            _players = _frontend.PromptPlayerCreation();

            _gameContext = new GameContext(_players);

            int roundCount = _deck.Cards.Count / _players.Count;
            for (int round = 1; round <= roundCount; round++)
                PlaySingleRound(round);
        }

        private void PlaySingleRound(int roundNum)
        {
            _frontend.DisplayStartRound(roundNum);

            _deck.Shuffle();
            CardSuite trumpSuite = _deck.Cards.Count > 0
                ? _deck.PopTop().Suite
                : CardSuite.SPECIAL;

            _gameContext.Rounds.Add(new RoundContext(roundNum, trumpSuite));
            var curRound = _gameContext.CurRound;
            curRound.Dealer = roundNum == 1
                ? _players[0]
                : _players[(_players.IndexOf(_gameContext.PrevRound.Dealer) + 1) % _players.Count];

            // bid on current round
            _players.ForEach(player => curRound.Bids[player] = player.MakeBid());

            // execute tricks and record results
            for (int trickNum = 1; trickNum <= roundNum; trickNum++)
            {
                PlaySingleTrick(trickNum);
                Player winner = curRound.CurTrick.Winner;
                curRound.Results[winner]++;
            }

            // resolve round scores
            _players.ForEach(player =>
            {
                int diff = Math.Abs(curRound.Bids[player] - curRound.Results[player]);
                if (diff == 0)
                    _gameContext.PlayerScores[player] += (BASELINE_SCORE + curRound.Bids[player] * HIT_SCORE);
                else
                    _gameContext.PlayerScores[player] += (diff * MISS_SCORE);
            });
        }

        // executes a single trick and stores state in a new TrickContext instance, as well
        private void PlaySingleTrick(int trickNum)
        {
            _frontend.DisplayStartTrick(trickNum);
            _gameContext.CurRound.Tricks.Add(new TrickContext(trickNum));

            var curRound = _gameContext.CurRound;
            var curTrick = curRound.CurTrick;

            Player leader = trickNum == 1
                ? leader = curRound.Dealer
                : leader = curRound.PrevTrick.Winner;

            List<Player> trickPlayerOrder = null; // TODO elegant way to wrap around _player list in play oder starting w/ dealer

            trickPlayerOrder.ForEach(player =>
            {
                curTrick.CardsPlayed.Add(player.MakeTurn(_gameContext));
            });

            // set winner
        }

        private void DealDeck(int roundNum)
        {
            for(int i = 0; i < roundNum; i++)
                foreach (var player in _players)
                    player.TakeCard(_deck.PopTop());
        }


        private List<Player> _players;
        private Deck _deck;
        //private Dictionary<Player, int> _playerScores;
        private IWizardFrontend _frontend { get; }
        private GameContext _gameContext;

        private readonly int BASELINE_SCORE = 20;
        private readonly int HIT_SCORE = 10;
        private readonly int MISS_SCORE = -10;
    }
}
