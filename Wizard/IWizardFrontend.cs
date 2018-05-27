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
        void DisplayTurnTaken(Card cardPlayed, Player player);
        void DisplayDealDone(Player dealer, Card trumpCard);
        void DisplayTrickWinner(Player winner, Card winningCard);
        void DisplayRoundScores(GameContext gameContext);
        void DisplayBidOutcome(int roundNum, int totalBids);
        Card PromptPlayerCardSelection(Player player);
        int PromptPlayerBid(Player player);
        List<Player> PromptPlayerCreation();
    }
}
