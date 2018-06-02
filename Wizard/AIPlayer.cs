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

        public override int MakeBid(GameContext gameContext)
        {
            var emptyTrick = new TrickContext(1 /*trick num*/);
            var hiddenCards = Deck.GetDeckComplement(_hand);
            var trumpSuite = gameContext.CurRound.TrumpSuite;
            var playerCount = gameContext.PlayerCount;

            // first rank cards by win percentages on an empty trick (card from _hand is leading card)
            Dictionary<Card, double> handWinAverages = SimulateTrick(emptyTrick, hiddenCards, trumpSuite, playerCount);

            // then decide on a round strategy which determines winpctg thresholds for 
            var roundStrategy = AIPlayStrategy.AGGRESIVE;

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

            //var playableCards = CardUtils.GetPlayableCards(_hand, curTrick.LeadingSuite);

            List<Card> allKnownCards = _hand.Concat(curRoundTricks.Aggregate(new List<Card>(), (acc, trick) =>
            {
                acc.AddRange(trick.CardsPlayed);
                return acc;
            })).ToList();
            allKnownCards.Add(curRound.TrumpCard);

            List<Card> remainingCards = Deck.GetDeckComplement(allKnownCards);

            // simulate trick and save the win percentages (strenth) of each car in _hand
            Dictionary<Card, Double> winPercentagesByCard = SimulateTrick(curTrick, remainingCards, curRound.TrumpSuite, gameContext.PlayerCount);
            List<KeyValuePair<Card, double>> cardsSortedByWinPctg = winPercentagesByCard.ToList();
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

        // simulates playing each card in _hand against a list of already played cards
        // returns a dictionary of cards in _hand to win percentages
        // @param hiddenCards refers to the cards in the deck that could potentialy be played by other players
        private Dictionary<Card, double> SimulateTrick(TrickContext trick, List<Card> hiddenCards, CardSuite trumpSuite, int playerCount)
        {
            var playableCards = CardUtils.GetPlayableCards(_hand, trick.LeadingSuite);
            Dictionary<Card, int> winsByCard = playableCards.Aggregate(new Dictionary<Card, int>(), (acc, next) => { acc[next] = 0; return acc; });

            for (int i = 0; i < SIMULATION_COUNT; i++)
            {
                foreach (var card in playableCards)
                {
                    var curSimRemainingCards = new List<Card>(hiddenCards);
                    var simPlayedCards = new List<Card>(trick.CardsPlayed);
                    simPlayedCards.Add(card);

                    // each remaining player plays a random card from a randomly generated hand                  
                    for (int j = simPlayedCards.Count(); j < playerCount; j++)
                    {
                        // rand hand selected from the remaining cards specific to this simulation
                        var randHand = takeRandomCardsFromList(curSimRemainingCards, _hand.Count());
                        var playableCardsFromRandHand = CardUtils.GetPlayableCards(randHand, trick.LeadingSuite);
                        simPlayedCards.Add(playableCardsFromRandHand[_rand.Next() % playableCardsFromRandHand.Count()]);
                    }
                    var winningCard = CardUtils.CalcWinningCard(simPlayedCards, trumpSuite, trick.LeadingSuite);

                    if (card.Equals(winningCard))
                    {
                        winsByCard[card]++;
                    }
                }
            }

            Dictionary<Card, double> winPercentagesByCard = new Dictionary<Card, double>();
            foreach(var winCardPair in winsByCard)
            {
                double winPctg = winCardPair.Value * 1.0 / SIMULATION_COUNT;
                winPercentagesByCard[winCardPair.Key] = winPctg;
            }
            return winPercentagesByCard;
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

        enum AIPlayStrategy
        {
            CONSERVATIVE,
            MODERATE,
            AGGRESIVE
        }
    }
}
