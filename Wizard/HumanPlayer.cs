using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(Engine engine, string Name): base(engine, Name)
        {                
        }

        public override int MakeBid()
        {
            return _engine.Frontend.PromptPlayerBid(this);
        }

        public override Card MakeTurn()
        {
            var cardToPlay = _engine.Frontend.PromptPlayerCardSelection(this);
            _hand.Remove(cardToPlay);
            return cardToPlay;
        }
    }
}
