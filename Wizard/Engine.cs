﻿using System;
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
        }

        // blocking method that executes the entirity of the game flow
        public void Run()
        {
            while (true)
                PlaySingleGame();
        }

        private void PlaySingleGame()
        {
            _curDeck = new Deck();
            _frontend.DisplayStartGame();
            _players = _frontend.PromptPlayerCreation();

            _gameContext = new GameContext(_players);

            int roundCount = _curDeck.Cards.Count / _players.Count;
            for (int round = 1; round <= roundCount; round++)
                PlaySingleRound(round);
        }

        private void PlaySingleRound(int roundNum)
        {
            _frontend.DisplayStartRound(roundNum);

            // shuffle, deal, and initialize round context
            _curDeck.Shuffle();
            _frontend.DisplayDealInProgess(3/*message duration seconds*/);
            DealDeck(roundNum);
            Card trumpCard = _curDeck.Cards.Count > 0 ? _curDeck.PopTop() : null;
            CardSuite trumpSuite = trumpCard != null ? trumpCard.Suite : CardSuite.SPECIAL;

            _gameContext.Rounds.Add(new RoundContext(roundNum, trumpSuite));
            var curRound = _gameContext.CurRound;
            curRound.Dealer = roundNum == 1
                ? _players[0]
                : _players[(_players.IndexOf(_gameContext.PrevRound.Dealer) + 1) % _players.Count];
            _players.ForEach(player => curRound.Results[player] = 0);

            _frontend.DisplayDealDone(curRound.Dealer, trumpCard);

            // bid on current round
            _players.ForEach(player => curRound.Bids[player] = player.MakeBid(curRound));
            int totalBids = curRound.Bids.Aggregate(0, (accumulator, bidPair) => accumulator + bidPair.Value);
            _frontend.DisplayBidOutcome(roundNum, totalBids);

            // execute tricks and record results
            for (int trickNum = 1; trickNum <= roundNum; trickNum++)
            {
                PlaySingleTrick(trickNum);
                Player winner = curRound.CurTrick.Winner;
                if (curRound.Results.ContainsKey(winner))
                    curRound.Results[winner]++;
                else
                    curRound.Results[winner] = 1;
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

            _frontend.DisplayRoundScores(_gameContext);
        }

        // executes a single trick and stores state in a new TrickContext instance, as well
        private void PlaySingleTrick(int trickNum)
        {
            _frontend.DisplayStartTrick(trickNum);
            _gameContext.CurRound.Tricks.Add(new TrickContext(trickNum));

            var curRound = _gameContext.CurRound;
            var curTrick = curRound.CurTrick;

            Player leader = trickNum == 1
                ? leader = _players[(_players.IndexOf(curRound.Dealer)+1) % _players.Count]
                : leader = curRound.PrevTrick.Winner;
            int leaderIndex = _players.IndexOf(leader);

            // create a player list that starts at the trick leader and wraps around
            List<Player> trickPlayerOrder = _players
                .GetRange(leaderIndex, _players.Count - leaderIndex)
                .Concat(_players.GetRange(0, leaderIndex)).ToList();

            trickPlayerOrder.ForEach(player =>
            {
                var cardPlayed = player.MakeTurn(_gameContext);
                curTrick.CardsPlayed.Add(cardPlayed);
                _frontend.DisplayTurnTaken(cardPlayed, player);
            });

            // find winner and save it to trick context
            var winningCard = CardUtils.CalcWinningCard(curTrick.CardsPlayed, curRound.TrumpSuite, curTrick.LeadingSuite);
            var winningPlayer = trickPlayerOrder[curTrick.CardsPlayed.IndexOf(winningCard)];
            curTrick.Winner = winningPlayer;
            _frontend.DisplayTrickWinner(winningPlayer, winningCard);            
        }

        private void DealDeck(int roundNum)
        {
            for(int i = 0; i < roundNum; i++)
                foreach (var player in _players)
                    player.TakeCard(_curDeck.PopTop());
        }


        private List<Player> _players;
        private Deck _curDeck;
        //private Dictionary<Player, int> _playerScores;
        private IWizardFrontend _frontend { get; }
        private GameContext _gameContext;

        private readonly int BASELINE_SCORE = 20;
        private readonly int HIT_SCORE = 10;
        private readonly int MISS_SCORE = -10;
    }
}
