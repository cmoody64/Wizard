using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public interface IWizardFrontend
    {
        void DisplayStartGame();
        void DisplayStartRound(int roundNum);
        void DisplayStartTrick(int trickNum);
        Card PromptPlayerCardSelection(IPlayer player);
        int PromptPlayerBid(IPlayer player);
    }
}
