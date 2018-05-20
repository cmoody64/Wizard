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
            Console.WriteLine($"Starting Round {roundNum}\n");
        }

        public void DisplayStartTrick(int trickNum)
        {
            Console.WriteLine($"Starting Trick {trickNum}\n");
        }

        public int PromptPlayerBid(Player player)
        {
            Console.Write("enter bid: ");
            int bid = -1;
            while (bid < 0)
            {
                var strInput = Console.ReadLine();
                var numInput = 0;
                if (Int32.TryParse(strInput, out numInput) && numInput >= 0 && numInput < player.Hand.Count)
                {
                    bid = numInput;
                }
                else
                {
                    Console.WriteLine($"please enter a valid number greater than 0");
                }
            }
            return bid;
        }

        public Card PromptPlayerCardSelection(Player player)
        {
            Console.WriteLine($"Current Hand: ");
            foreach(var card in player.Hand)
            {
                Console.WriteLine($"\t(0) {card.ToString()}");
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
