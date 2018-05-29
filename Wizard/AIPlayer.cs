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
                nonTrumpCutoff = CardValue.FOUR;
            }
            else if (roundContext.RoundNum < 5)
            {
                trumpCutoff = CardValue.FOUR;
                nonTrumpCutoff = CardValue.EIGHT;
            }
            else
            {
                trumpCutoff = CardValue.SEVEN;
                nonTrumpCutoff = CardValue.JACK;
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

            Dictionary<Card, double> winPercentageByCard = new Dictionary<Card, double>();
            foreach(var cardWinPair in winsByCard)
            {
                winPercentageByCard[cardWinPair.Key] = cardWinPair.Value * 1.0 / SIMULATION_COUNT;
            }

            return null;

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
