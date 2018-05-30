using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Wizard
{
    public class AIPlayer : Player
    {
        public AIPlayer(IWizardFrontend frontend, string name): base(frontend, name)
        {
            _rand = new Random();
        }

        public override int MakeBid(RoundContext roundContext)
        {
            CardValue trumpCutoff;
            CardValue nonTrumpCutoff;
            if (roundContext.RoundNum < 2)
            {
                trumpCutoff = CardValue.TWO;
                nonTrumpCutoff = CardValue.THREE;
            }
            else if (roundContext.RoundNum < 5)
            {
                trumpCutoff = CardValue.THREE;
                nonTrumpCutoff = CardValue.SIX;
            }
            else
            {
                trumpCutoff = CardValue.FIVE;
                nonTrumpCutoff = CardValue.TEN;
            }

            int bid = 0;
            foreach(var card in _hand)
            {
                if (card.Value == CardValue.WIZARD)
                    bid++;
                else if (card.Suite == roundContext.TrumpSuite && card.Value >= trumpCutoff)
                    bid++;
                else if (card.Suite != roundContext.TrumpSuite && card.Value >= nonTrumpCutoff)
                    bid++;                    
            }
            _curBid = bid;
            _frontend.DisplayPlayerBid(bid, this);
            return bid;
        }

        public override Card MakeTurn(GameContext gameContext)
        {
            var curRound = gameContext.CurRound;
            var curTrick = curRound.CurTrick;
            var curRoundTricks = curRound.Tricks;

            var playableCards = CardUtils.GetPlayableCards(_hand, curTrick.LeadingSuite);
            
            List<Card> allKnownCards = _hand.Concat(curRoundTricks.Aggregate(new List<Card>(), (acc, trick) =>
            {
                acc.AddRange(trick.CardsPlayed);
                return acc;
            })).ToList();

            List<Card> remainingCards = new List<Card>(new Deck().Cards);
            foreach (var card in allKnownCards)
                remainingCards.Remove(card);

            Dictionary<Card, int> winsByCard = playableCards.Aggregate(new Dictionary<Card, int>(), (acc, next) =>{ acc[next] = 0; return acc; });

            for (int i = 0; i < SIMULATION_COUNT; i++)
            {
                foreach (var card in playableCards)
                {
                    var curSimRemainingCards = new List<Card>(remainingCards);
                    var cardsPlayed = new List<Card>(curTrick.CardsPlayed);
                    cardsPlayed.Add(card);

                    // each remaining player plays a random card from a randomly generated hand                  
                    for(int j = cardsPlayed.Count(); j < gameContext.PlayerCount; j++)
                    {
                        // rand hand selected from the remaining cards specific to this simulation
                        var randHand = takeRandomCardsFromList(curSimRemainingCards, _hand.Count());
                        var playableCardsFromRandHand = CardUtils.GetPlayableCards(randHand, curTrick.LeadingSuite);
                        cardsPlayed.Add(playableCardsFromRandHand[_rand.Next() % playableCardsFromRandHand.Count()]);
                    }
                    var winningCard = CardUtils.CalcWinningCard(cardsPlayed, curRound.TrumpSuite, curTrick.LeadingSuite);

                    if (card.Equals(winningCard))
                    {
                        winsByCard[card]++;
                    }
                }
            }

            List<KeyValuePair<Card, double>> cardsSortedByWinPctg = winsByCard.Aggregate(new List<KeyValuePair<Card, double>>(), (acc, nextPair) =>
            {
                double winPercentage = nextPair.Value * 1.0 / SIMULATION_COUNT;
                acc.Add(new KeyValuePair<Card, double>(nextPair.Key, winPercentage));
                return acc;
            });
            // sort it so that weakest cards are at lower indices and stronger card at higher indices                       
            cardsSortedByWinPctg.Sort((a, b) => a.Value < b.Value ? -1 : 1);

            var curBid = curRound.Bids[this];
            var curWins = curRound.Results[this];
            var roundsLeft = curRound.RoundNum - curRound.Tricks.Count + 1;
            /*
                requieredStrengthOfPlay is a value determining strength of card to play based on game context
                given that requiredStrengthOfPlay = k:

                k>1  => impossible to win, play strongest card
                k=1 => play strongest cards from here on out to win
                0<k<1 => sort cards by strength and select closest index to k val
	                i.e. k = .6 and 5 cards => choose card 3 /5 ranked by strength
                k=0 => play weakest cards from here on out to win
                k<0 => won too many, impossible to win, play weakest card

            */
            double requiredStrengthOfPlay = (curBid - curWins) * 1.0 / roundsLeft;

            Card cardToPlay = null;
            if (requiredStrengthOfPlay >= 1)
                cardToPlay = cardsSortedByWinPctg.Last().Key;
            else if (requiredStrengthOfPlay <= 0)
                cardToPlay = cardsSortedByWinPctg.First().Key;
            else
            {
                // requiredStrengthOfPlay between 0 and 1
                int indexToPlay = (int)(requiredStrengthOfPlay * cardsSortedByWinPctg.Count());
                cardToPlay = cardsSortedByWinPctg[indexToPlay].Key;
            }

            _frontend.DisplayTurnInProgress(this);
            _hand.Remove(cardToPlay);
            return cardToPlay;
        }

        private List<Card> takeRandomCardsFromList(List<Card> cardList, int numberToTake)
        {
            List<Card> removedCards = new List<Card>();
            for(int i = 0; i < numberToTake; i++)
            {
                var randIndex = _rand.Next() % cardList.Count();
                removedCards.Add(cardList[randIndex]);
                cardList.RemoveAt(randIndex);
            }
            return removedCards;
        }

        // updated each round, this stores the current bid to hit
        private int _curBid;
        private readonly int SIMULATION_COUNT = 1000;
        private Random _rand;
    }
}
