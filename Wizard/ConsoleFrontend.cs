using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    class ConsoleFrontend : IWizardFrontend
    {
        public void DisplayStartGame()
        {
            Console.WriteLine("Welcome to Console Wizard\n\n");
        }

        public void DisplayStartRound(int roundNum)
        {
            Console.WriteLine("***********************");
            Console.WriteLine($"\n\nStarting Round {roundNum}\n");
        }

        public void DisplayStartTrick(int trickNum)
        {
            Console.WriteLine($"\n\nStarting Trick {trickNum}");
        }

        public void DisplayTurnTaken(Card cardPlayed, Player player)
        {
            Console.WriteLine($"{player.Name} played {cardPlayed.ToString()}");
        }

        public void DisplayTrickWinner(Player winner, Card winningCard)
        {
            Console.WriteLine($"\n{winner.Name} won with {winningCard.ToString()}");
        }

        public void DisplayDealDone(Player dealer, Card trumpCard)
        {
            if (trumpCard != null)
            {
                Console.WriteLine($"{dealer.Name} dealt - {trumpCard.ToString()} is flipped");
            }
            else
            {
                Console.WriteLine("{dealer.Name} dealt - no trump suite set");
            }
        }

        public int PromptPlayerBid(Player player)
        {
            Console.WriteLine($"\n{player.Name}'s hand:");
            foreach(var card in player.Hand)
            {
                Console.WriteLine($"\t{card.ToString()}");
            }
            Console.Write($"{player.Name}, enter bid: ");
            int bid = -1;
            while (bid < 0)
            {
                var strInput = Console.ReadLine();
                var numInput = 0;
                if (Int32.TryParse(strInput, out numInput) && numInput >= 0 && numInput <= player.Hand.Count)
                {
                    bid = numInput;
                }
                else
                {
                    Console.WriteLine($"please enter a valid number greater than 0 and less than {player.Hand.Count+1}");
                }
            }
            return bid;
        }

        public Card PromptPlayerCardSelection(Player player)
        {
            Console.WriteLine($"\n{player.Name}'s turn: current hand: ");
            for(int i = 0; i < player.Hand.Count; i++)
            {
                Console.WriteLine($"\t({i}) {player.Hand[i].ToString()}");
            }
            Console.Write("enter number of card selection: ");
            int selection = -1;
            while(!(selection >= 0 && selection < player.Hand.Count))
            {
                var strInput = Console.ReadLine();
                var numInput = 0;
                if(Int32.TryParse(strInput, out numInput) && numInput >= 0 && numInput < player.Hand.Count)
                {
                    selection = numInput;
                }
                else
                {
                    Console.WriteLine($"please enter a valid number 1 - {player.Hand.Count}");
                }
            }

            return player.Hand[selection];
        }

        public List<Player> PromptPlayerCreation()
        {
            Console.WriteLine("type player names - enter blank name when done");
            List<Player> players = new List<Player>();
            while(true)
            {
                string curName = Console.ReadLine();
                if (curName.Length > 0)
                    players.Add(new HumanPlayer(this, curName));
                else
                    break;
            }

            return players;
        }
    }
}
