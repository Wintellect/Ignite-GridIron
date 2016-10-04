using System;
using System.Threading.Tasks;
using GridIron.Api;
using Xamarin.Forms;
using System.Diagnostics;

namespace GridIron
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Error { get; internal set; }
    }

    public class BattleEventArgs : EventArgs
    {
        public Battle Battle { get; internal set; }
    }

    public class BattleStatusEventArgs : EventArgs
    {
        public string BattleStatus { get; internal set; }
        public string WinningArtist { get; internal set; }
    }

    public class TimeLeftEventArgs : EventArgs
    {
        public string TimeLeft { get; internal set; }
        public TimeSpan TimeLeftValue { get; internal set;}
    }

    public class GameData
    {
        IBattleService _svc;
        bool _running;
        Task _task;
        Task _vote;
        string _status;
        string _passcode;

        public string UserId { get; set; }
        public string TimeLeft { get; private set; }

        public string Passcode { 
            get {
                return _passcode;
            }
            set {
                if (_passcode == value)
                    return;
                _passcode = value;
                ResetService (string.Equals (_passcode, "demo", StringComparison.OrdinalIgnoreCase));
            }
        }

        void ResetService (bool useDemoService)
        {
            if (useDemoService)
                _svc = new DemoBattleService ();
            else
                _svc = new BattleService (@"https://fatmambasite.azurewebsites.net/api");
        }

        public GameData ()
        {
            _passcode = string.Empty;
            ResetService (false);
        }

        public void EnablePolling ()
        {
            if (_running == true)
                return;
            _running = true;
            if (_task == null)
                _task = Task.Run (PollingLoop);
            return;
        }

        public void DisablePolling ()
        {
            if (_running == false)
                return;
            if (_task == null)
                return;
            _running = false;
        }

        public async Task VoteFor (string artistId)
        {
            if (_vote != null)
                return;
            try {
                Debug.WriteLine ("Voting for {0}", artistId);
                _vote = _svc.Vote (VoteRequest.From (Passcode, UserId, artistId));
                await _vote;
            } catch (Exception ex) {
                OnError (ex);
            }
            _vote = null;
        }

        public event EventHandler<ErrorEventArgs> Error;
        public event EventHandler<BattleEventArgs> BattleUpdated;
        public event EventHandler<BattleStatusEventArgs> BattleStatusUpdated;
        public event EventHandler<TimeLeftEventArgs> TimeLeftUpdated;

        void OnError (Exception ex)
        {
            if (ex is TaskCanceledException)
                return;
            var handler = Error;
            if (handler == null)
                return;
            Device.BeginInvokeOnMainThread (() => handler (this, new ErrorEventArgs { Error = ex }));
        }

        void OnBattle (Battle battle)
        {
            var handler = BattleUpdated;
            if (handler == null)
                return;
            Device.BeginInvokeOnMainThread (() => handler (this, new BattleEventArgs { Battle = battle }));
        }

        void OnBattleStatus (string status, Artist winningArtist)
        {
            var handler = BattleStatusUpdated;
            if (handler == null)
                return;
            var winner = (winningArtist == null) ? string.Empty : winningArtist.ArtistName;
            Device.BeginInvokeOnMainThread (() => handler (this, new BattleStatusEventArgs { BattleStatus = status, WinningArtist = winner }));
        }

        void OnTimeLeftChanged (string timeLeft, TimeSpan timeLeftValue)
        {
            if (TimeLeft == timeLeft)
                return;
            TimeLeft = timeLeft;
            var handler = TimeLeftUpdated;
            if (handler == null)
                return;
            Device.BeginInvokeOnMainThread (() => handler (this, new TimeLeftEventArgs { TimeLeft = timeLeft, TimeLeftValue = timeLeftValue }));
        }

        async Task PollingLoop ()
        {
            while (true) {
                if (!_running) {
                    _task = null;
                    return;
                }
                try {
                    Debug.WriteLine ("Polling");
                    var curr = await _svc.GetStatus (StatusRequest.From (Passcode));
                    if (curr.BattleStatus != _status) {
                        _status = curr.BattleStatus;
                        OnBattleStatus (_status, curr.ArtistOne);
                    }
                    var timeLeft = curr.TimeLeft.ToString (@"mm\:ss");
                    OnTimeLeftChanged (timeLeft, curr.TimeLeft);
                    OnBattle (curr);
                    if (string.Equals (_status, "final", StringComparison.OrdinalIgnoreCase)) {
                        DisablePolling ();
                        _task = null;
                        return;
                    }
                } catch (Exception ex) {
                    OnError (ex);
                }
                if (!_running) {
                    _task = null;
                    return;
                }
                await Task.Delay (TimeSpan.FromSeconds (0.5));
            }
        }
    }
}
