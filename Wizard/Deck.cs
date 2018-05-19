using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    class Deck
    {
        // populates cards list with fixed Wizard deck
        public Deck()
        {
            var cards = new List<Card>();
            var standardSuites = new List<CardSuite> { CardSuite.CLUBS, CardSuite.SPADES, CardSuite.HEARTS, CardSuite.DIAMONDS };

            // add in TWO to ACE in each suite besides special
            foreach(var cardVal in Enumerable.Range((int)CardValue.TWO, (int)CardValue.ACE))
            {
                foreach(var cardSuite in standardSuites)
                {
                    cards.Add(new Card((CardValue)cardVal, cardSuite));
                }
            }

            // add in special cards
            for(int i = 0; i < NUM_SPECIAL_CARDS; i++)
            {
                cards.Add(new Card(CardValue.JESTER, CardSuite.SPECIAL));
                cards.Add(new Card(CardValue.WIZARD, CardSuite.SPECIAL));
            }
        }

        public IReadOnlyList<Card> Cards { get { return _cards; } }
        private List<Card> _cards;

        public Card PopTop()
        {
            Card top = _cards[_cards.Count];
            _cards.Remove(top);
            return top;
        }

        public void Shuffle()
        {
            var rand = new Random();
            for(int i = 0; i < _cards.Count; i++)
            {
                SwapCards(i, rand.Next(i, _cards.Count));
            }
        }

        private void SwapCards(int i, int j)
        {
            Card temp = _cards[i];
            _cards[i] = _cards[j];
            _cards[j] = temp;
        }

        private readonly int NUM_SPECIAL_CARDS = 4;
    }
}
