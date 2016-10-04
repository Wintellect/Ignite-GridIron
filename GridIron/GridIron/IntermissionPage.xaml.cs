using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace GridIron
{
    public partial class IntermissionPage : ContentPage
    {
        bool _hasAppeared;

        public IntermissionPage ()
        {
            InitializeComponent ();

            Counter.Text = "";
            Artist1.Text = "";
            Artist2.Text = "";

            Label1.Opacity = 0;
            Artist1.Opacity = 0;
            Label2.Opacity = 0;
            Artist2.Opacity = 0;
            Counter.Opacity = 0;
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

        void GameTimeLeftUpdated (object sender, TimeLeftEventArgs e)
        {
            Counter.Text = e.TimeLeft;
        }

        async void GameBattleUpdated (object sender, BattleEventArgs e)
        {
            Artist1.Text = e.Battle.ArtistOne.ArtistName;
            Artist2.Text = e.Battle.ArtistTwo.ArtistName + " !";

            if (!_hasAppeared) {
                _hasAppeared = true;
                await Animations ();
            }
        }

        async Task Animations ()
        {
            await Label1.FadeTo (1.0, 250);
            await Task.Delay (TimeSpan.FromSeconds (1));
            var t1 = Artist1.FadeTo (1.0, 125);
            await Artist1.ScaleTo (1.5, 100);
            await Artist1.ScaleTo (1.0, 150);
            await t1;
            await Label2.FadeTo (1.0, 60);
            await Task.Delay (TimeSpan.FromSeconds (1));
            var t2 = Artist2.FadeTo (1.0, 125);
            await Artist2.ScaleTo (1.5, 100);
            await Artist2.ScaleTo (1.0, 150);
            await t2;
            await Task.Delay (TimeSpan.FromSeconds (1));
            await Counter.FadeTo (1.0, 250);
        }
    }
}
