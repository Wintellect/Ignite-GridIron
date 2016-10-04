using System;

namespace GridIron.Api
{
    public class Battle
    {
        public string BattleId { get; set; }
        public string BattleName { get; set; }
        public string BattleStatus { get; set; }
        public TimeSpan TimeLeft { get; set; }
        public TimeSpan Duration { get; set; }
        public Artist ArtistOne { get; set; }
        public Artist ArtistTwo { get; set; }
    }
}
