using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public class HumanPlayer : IPlayer
    {
        public HumanPlayer()
        {
            _hand = new List<Card>();
        }

        public Card PlayCard()
        {
            // TODO add meaningful functionality here
            var cardToPlay = _hand[_hand.Count-1];
            _hand.RemoveAt(_hand.Count - 1);
            return cardToPlay;
        }

        public void TakeCard(Card card)
        {
            _hand.Add(card);
        }

        private List<Card> _hand;
    }
}
