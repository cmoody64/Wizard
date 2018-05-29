using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public class GameContext
    {
        // state that persists across the scope of a single game
        public GameContext(List<Player> players)
        {
            PlayerCount = players.Count;
            Rounds = new List<RoundContext>();
            PlayerScores = new Dictionary<Player, int>();

            // initialize player scores based off of the current player list passed in by the engine
            players.ForEach(player => PlayerScores[player] = 0);
        }
        public int PlayerCount { get; }
        public List<RoundContext> Rounds { get; }
        public Dictionary<Player, int> PlayerScores { get; }
        public RoundContext CurRound { get { return Rounds.Last(); } }
        public RoundContext PrevRound
        {
            get { return Rounds.Count > 1 ? Rounds[Rounds.Count - 2] : null; }
        }
    }

    // state that persists across a single round
    public class RoundContext
    {
        public RoundContext(int roundNum, CardSuite trumpSuite)
        {
            RoundNum = roundNum;
            TrumpSuite = trumpSuite;
            Tricks = new List<TrickContext>();
            Bids = new Dictionary<Player, int>();
            Results = new Dictionary<Player, int>();
        }
        public int RoundNum { get; }
        public List<TrickContext> Tricks { get; }
        public Dictionary<Player, int> Bids { get; }
        public Dictionary<Player, int> Results { get; }
        public CardSuite TrumpSuite { get; }
        public Player Dealer { get; set; }
        public TrickContext CurTrick { get { return Tricks.Last(); } }
        public TrickContext PrevTrick
        {
            get { return Tricks.Count > 1 ? Tricks[Tricks.Count - 2] : null; }
        }
    }

    // state that persists across a single trick
    public class TrickContext
    {
        public TrickContext(int trickNum)
        {
            TrickNum = trickNum;
            CardsPlayed = new List<Card>();
        }
        public int TrickNum { get; }
        public List<Card> CardsPlayed { get; }
        public CardSuite LeadingSuite
        {
            get
            {
                if (CardsPlayed.Count > 0)
                    return CardsPlayed[0].Suite;
                else
                    throw new IndexOutOfRangeException("LeadingSuite cannot be accessed because a leading card has not yet been played");
            }
        }
        public Player Winner;
    }
}
