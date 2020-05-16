namespace ScrapTDWrapper.Entities
{
    internal class TLeaderboard
    {
        public TLbPlayer[] Players { get; }
        public TLbTeam[] Teams { get; }

        internal TLeaderboard(TLbPlayer[] players, TLbTeam[] teams)
        {
            Players = players;
            Teams = teams;
        }
    }
}
