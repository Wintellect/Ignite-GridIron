using System.Threading.Tasks;
using Xamarin.Forms;
using System;

namespace GridIron
{
    public partial class BattlePage : ContentPage
    {
        bool _hasAppeared;
        bool _voting;

        void CascadeGestureRecognizersToChildren (Layout<View> parent)
        {
            foreach (var child in parent.Children) {
                foreach (var gesture in parent.GestureRecognizers) {
                    child.GestureRecognizers.Add (gesture);
                }
                if (child is Layout<View>) {
                    // recursion
                    CascadeGestureRecognizersToChildren ((Layout<View>)child);
                }
            }
        }

        public BattlePage ()
        {
            InitializeComponent ();

            BattleName.Text = " ";
            Counter.Text = " ";

            InfoBar.Opacity = 0.0;
            Score.Opacity = 0.0;
            InfoPanel.Opacity = 0.0;
            Banner1.Opacity = 0.0;
            Banner2.Opacity = 0.0;
            Score.ColumnDefinitions [0].Width = new GridLength (1, GridUnitType.Star);
            Score.ColumnDefinitions [2].Width = new GridLength (1, GridUnitType.Star);

            if (Device.OS == TargetPlatform.Android) {
                // a quirk in the Android renderer causes child elements to not bubble gestures up to the parent
                CascadeGestureRecognizersToChildren (Banner1);
                CascadeGestureRecognizersToChildren (Banner2);
            }

            Vote1Gesture.Command = new Command<string> (async (artistId) => await VoteFor (artistId, Vote1));
            Vote2Gesture.Command = new Command<string> (async (artistId) => await VoteFor (artistId, Vote2));
        }

        protected override void OnAppearing ()
        {
            base.OnAppearing ();
            App.Current.Game.TimeLeftUpdated += GameTimeLeftUpdated;
            App.Current.Game.BattleUpdated += GameBattleUpdated;
        }

        protected override void OnDisappearing ()
        {
            base.OnDisappearing ();
            App.Current.Game.TimeLeftUpdated -= GameTimeLeftUpdated;
            App.Current.Game.BattleUpdated -= GameBattleUpdated;
        }

        async void GameTimeLeftUpdated (object sender, TimeLeftEventArgs e)
        {
            Counter.Text = e.TimeLeft;

            if (e.TimeLeftValue.TotalSeconds < 5) {
                await Pulse (Counter);
            }
        }

        async void GameBattleUpdated (object sender, BattleEventArgs e)
        {
            BattleName.Text = e.Battle.BattleName;

            Vote1Gesture.CommandParameter = e.Battle.ArtistOne.ArtistId;
            Vote2Gesture.CommandParameter = e.Battle.ArtistTwo.ArtistId;
            Artist1.Text = e.Battle.ArtistOne.ArtistName;
            Artist2.Text = e.Battle.ArtistTwo.ArtistName;

            await AnimateScore (e.Battle.ArtistOne.Score, e.Battle.ArtistTwo.Score);

            if (!_hasAppeared) {
                _hasAppeared = true;
                await Animations ();
            }
        }

        Task AnimateScore (double artist1Score, double artist2Score)
        {
            var tcs = new TaskCompletionSource<object> ();

            artist1Score = Math.Max (1.0, artist1Score);
            artist2Score = Math.Max (1.0, artist2Score);

            if (!_hasAppeared) {
                Score.ColumnDefinitions [0].Width = new GridLength (artist1Score, GridUnitType.Star);
                Score.ColumnDefinitions [2].Width = new GridLength (artist2Score, GridUnitType.Star);
                tcs.SetResult (null);
                return tcs.Task;
            }

            const double EPSILON = 0.1;
            if ((artist1Score - Score.ColumnDefinitions [0].Width.Value > EPSILON)
                || (artist2Score - Score.ColumnDefinitions [2].Width.Value > EPSILON)) {

                var val1Start = Score.ColumnDefinitions [0].Width.Value;
                var val2Start = Score.ColumnDefinitions [2].Width.Value;

                var anim = new Animation ((progress) => {
                    var val1 = ((artist1Score - val1Start) * progress) + val1Start;
                    var val2 = ((artist2Score - val2Start) * progress) + val2Start;
                    Score.ColumnDefinitions [0].Width = new GridLength (val1, GridUnitType.Star);
                    Score.ColumnDefinitions [2].Width = new GridLength (val2, GridUnitType.Star);
                }, 0, 1, Easing.BounceOut, () => {
                    Score.ColumnDefinitions [0].Width = new GridLength (artist1Score, GridUnitType.Star);
                    Score.ColumnDefinitions [2].Width = new GridLength (artist2Score, GridUnitType.Star);
                    tcs.SetResult (null);
                });

                Score.Animate ("score", anim, 16, 100);
            } else {
                tcs.SetResult (null);
            }
            return tcs.Task;
        }

        async Task Animations ()
        {
            await Task.WhenAll (
                InfoBar.FadeTo (1.0),
                Score.FadeTo (1.0),
                InfoPanel.FadeTo (1.0)
            );

            await SlideBannerIn (Banner1, false);
            await SlideBannerIn (Banner2, true);
        }

        async Task SlideBannerIn (Grid banner, bool fromRight)
        {
            banner.TranslationX = fromRight ? banner.Width : -banner.Width;

            var b1 = banner.FadeTo (1.0, 300);
            var b2 = banner.TranslateTo (0, 0, 400, Easing.BounceOut);

            await Task.WhenAll (b1, b2);
        }

        async Task VoteFor (string artistId, Label voteLabel)
        {
            if (_voting)
                return;
            _voting = true;
            try {
                await Task.WhenAll (
                    Pulse (voteLabel),
                    App.Current.Game.VoteFor (artistId)
                );
            } finally {
                _voting = false;
            }
        }

        async Task Pulse (Label voteLabel)
        {
            await voteLabel.ScaleTo (1.25, 50);
            await voteLabel.ScaleTo (1, 75);
        }
    }
}
