using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public enum CardValue
    {
        JESTER = 1,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIZE,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING,
        ACE,
        WIZARD
    }

    public enum CardSuite
    {
        SPADES,
        CLUBS,
        HEARTS,
        DIAMONDS,
        SPECIAL
    }

    public class Card
    {
        public Card(CardValue value, CardSuite suite)
        {
            Value = value;
            Suite = suite;
        }

        public CardValue Value { get; }
        public CardSuite Suite { get; }
    }
}
