using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public interface IPlayer
    {
        void TakeCard(Card card);
        Card PlayCard();
        IReadOnlyList<Card> Hand { get; }
        string Name { get; }
    }
}
