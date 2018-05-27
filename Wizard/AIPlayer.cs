using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    class AIPlayer : Player
    {
        public AIPlayer(IWizardFrontend frontend, string name): base(frontend, name)
        {            
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
            var curTrick = gameContext.CurRound.CurTrick;
            var cardsPlayed = curTrick.CardsPlayed;
            if(cardsPlayed.Contains(new Card(CardValue.WIZARD, CardSuite.SPADES)))
            {
                // use duck card
            }
            else
            {
                var playableCards = _hand.Where(card => card.Suite == curTrick.LeadingSuite || card.Suite == CardSuite.SPECIAL);
                playableCards = playableCards.Count() > 0 ? playableCards : _hand;                
            }
        }

        // updated each round, this stores the current bid to hit
        private int _curBid;

        // each trick, after the bid is made, the hand is sorted by strength
        // towards the beginning are duck cards (i.e. jester), and towards the end are winning cards (i.e. wizard)
        private void SortHandByStrength()
        {
        }
    }
}
