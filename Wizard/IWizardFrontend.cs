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
        Card PromptPlayerCardSelection(Player player);
        int PromptPlayerBid(Player player);
        List<Player> PromptPlayerCreation();
    }
}
