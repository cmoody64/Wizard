using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
    public class GameContext
    {
        public GameContext()
        {
            Rounds = new List<RoundContext>();
            PlayerScores = new Dictionary<Player, int>();
            _players = new List<Player>();
            Deck = new Deck();
        }
        public List<RoundContext> Rounds { get; }
        public Dictionary<Player, int> PlayerScores { get; }
        public IReadOnlyList<Player> Players { get { return _players; } }
        public Deck Deck { get; }
        public RoundContext CurRound { get { return Rounds.Last(); } }
        private List<Player> _players;

        public void AddPlayer(Player player)
        {
            _players.Add(player);
            PlayerScores[player] = 0;
        }

        public void AddPlayers(List<Player> players)
        {
            players.ForEach(player => AddPlayer(player));
        }
    }

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
        public TrickContext CurTrick { get { return Tricks.Last(); } }
    }

    public class TrickContext
    {
        public TrickContext(int trickNum, CardSuite leadingSuite)
        {
            TrickNum = trickNum;
            LeadingSuite = leadingSuite;
            CardsPlayed = new List<Card>();
        }
        public int TrickNum { get; }
        public List<Card> CardsPlayed { get; }
        public CardSuite LeadingSuite { get; }
    }
}
