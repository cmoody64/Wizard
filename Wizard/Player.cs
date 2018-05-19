using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public abstract class Player
    {
        public Player(Engine engine, string name)
        {
            Name = name;
            _hand = new List<Card>();
            _engine = engine;
        }
        
        public abstract Card MakeTurn();
        public abstract int MakeBid();

        public void TakeCard(Card card)
        {
            _hand.Add(card);
        }

        public override int GetHashCode()
        {
            return 17 * Name.GetHashCode();
        }

        protected Engine _engine;
        protected List<Card> _hand;
        public IReadOnlyList<Card> Hand { get { return _hand; } }
        public string Name { get; }
    }
}
