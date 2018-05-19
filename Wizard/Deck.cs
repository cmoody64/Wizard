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

            Cards = cards;
        }

        public IReadOnlyList<Card> Cards { get; }

        private readonly int NUM_SPECIAL_CARDS = 4;
    }
}
