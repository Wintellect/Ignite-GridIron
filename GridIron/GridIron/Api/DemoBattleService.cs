using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GridIron.Api
{
    public class DemoBattleService : IBattleService
    {
        Queue<Battle> _battles;
        Battle _current;
        DateTime _started;
        int _nextArtist = 101;
        int _nextBattle = 1; 

        public DemoBattleService ()
        {
            _battles = new Queue<Battle> ();
            _battles.Enqueue (MakeBattle ("Intermission", "Ric Flair", "Dusty Rhodes", "closed", TimeSpan.FromSeconds (10)));
            _battles.Enqueue (MakeBattle ("Qualifying Round 1", "Ric Flair", "Dusty Rhodes", "open", TimeSpan.FromSeconds(15)));

            _battles.Enqueue (MakeBattle ("Intermission", "\"Stone Cold\" Steve Austin", "The Rock", "closed", TimeSpan.FromSeconds (10)));
            _battles.Enqueue (MakeBattle ("Qualifying Round 2", "\"Stone Cold\" Steve Austin", "The Rock", "open", TimeSpan.FromSeconds (15)));

            _battles.Enqueue (MakeBattle ("Intermission", "John Cena", "The Undertaker", "closed", TimeSpan.FromSeconds (10)));
            _battles.Enqueue (MakeBattle ("Qualifying Round 3", "John Cena", "The Undertaker", "open", TimeSpan.FromSeconds (15)));

            _battles.Enqueue (MakeBattle ("Intermission", "\"Rowdy\" Roddy Piper", "\"Macho Man\" Randy Savage", "closed", TimeSpan.FromSeconds (10)));
            _battles.Enqueue (MakeBattle ("Qualifying Round 4", "\"Rowdy\" Roddy Piper", "\"Macho Man\" Randy Savage", "open", TimeSpan.FromSeconds (15)));

            _battles.Enqueue (MakeBattle ("Intermission", "Dusty Rhodes", "The Rock", "closed", TimeSpan.FromSeconds (10)));
            _battles.Enqueue (MakeBattle ("Semifinal Round 1", "Dusty Rhodes", "The Rock", "open", TimeSpan.FromSeconds (15)));

            _battles.Enqueue (MakeBattle ("Intermission", "\"Macho Man\" Randy Savage", "The Undertaker", "closed", TimeSpan.FromSeconds (10)));
            _battles.Enqueue (MakeBattle ("Semifinal Round 2", "\"Macho Man\" Randy Savage", "The Undertaker", "open", TimeSpan.FromSeconds (15)));

            _battles.Enqueue (MakeBattle ("Intermission", "Dusty Rhodes", "\"Macho Man\" Randy Savage", "closed", TimeSpan.FromSeconds (10)));
            _battles.Enqueue (MakeBattle ("Championship", "Dusty Rhodes", "\"Macho Man\" Randy Savage", "open", TimeSpan.FromSeconds (15)));

            _battles.Enqueue (MakeBattle ("Final", "\"Macho Man\" Randy Savage", "", "final", TimeSpan.FromSeconds (60)));

            Update ();
        }

        void Update ()
        {
            if (_current != null) {
                var duration = DateTime.Now - _started;
                _current.TimeLeft = _current.Duration - duration;
                if (_current.TimeLeft < TimeSpan.Zero) {
                    _current.TimeLeft = TimeSpan.Zero;
                    _current = null;
                }
            }

            if (_current == null) {
                _current = _battles.Dequeue ();
                _started = DateTime.Now;
                _current.ArtistOne.Score = 0;
                _current.ArtistTwo.Score = 0;
            }
        }

        Battle MakeBattle (string battleName, string artistOne, string artistTwo, string status, TimeSpan duration)
        {
            var result = new Battle {
                BattleId = _nextBattle.ToString(),
                BattleName = battleName,
                BattleStatus = status,
                Duration = duration,
                TimeLeft = TimeSpan.Zero,
                ArtistOne = MakeArtist(_nextBattle.ToString(), artistOne),
                ArtistTwo = MakeArtist(_nextBattle.ToString(), artistTwo)
            };
            _nextBattle++;
            return result;
        }

        Artist MakeArtist (string battleId, string artistName)
        {
            var result = new Artist {
                ArtistId = _nextArtist.ToString(),
                BattleId = battleId,
                ArtistName = artistName,
                Score = 0
            };
            _nextArtist++;
            return result;
        }

        Task<Battle> IBattleService.GetStatus (StatusRequest request)
        {
            Update ();
            return Task.FromResult (_current);
        }

        Task IBattleService.Vote (VoteRequest request)
        {
            Update ();
            if (request.ArtistId == _current.ArtistOne.ArtistId) {
                _current.ArtistOne.Score += 1;
            }
            if (request.ArtistId == _current.ArtistTwo.ArtistId) {
                _current.ArtistTwo.Score += 1;
            }
            return Task.Delay(0);
        }
    }
}
