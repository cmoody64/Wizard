using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public static class CardComparator
    {
        public static Card CalcWinningCard(List<Card> cardsPlayed, CardSuite trumpSuite, CardSuite leadingSuite)
        {
            Card winningCard = null;
            foreach (var curCard in cardsPlayed)
            {
                if (curCard.Value == CardValue.WIZARD)
                {
                    winningCard = curCard;
                    break;
                }

                if (winningCard == null)
                {
                    winningCard = curCard;
                }
                else if (curCard.Suite == trumpSuite)
                {
                    if (winningCard.Suite == trumpSuite)
                    {
                        if (curCard.Value > winningCard.Value)
                            winningCard = curCard;
                    }
                    else
                    {
                        // if winning suite is not trump suite, current card is now winner
                        winningCard = curCard;
                    }
                }
                else if (curCard.Suite == leadingSuite)
                {
                    if (winningCard.Suite == leadingSuite && curCard.Value > winningCard.Value)
                    {
                        winningCard = curCard;
                    }
                }
            }
            return winningCard;
        }
    }
}
