using System;
using Xamarin.Forms;

namespace GridIron
{
    public partial class PasscodePage : ContentPage
    {
        public PasscodePage ()
        {
            InitializeComponent ();
        }

        void EnterClicked (object sender, EventArgs e)
        {
            EnterButton.IsEnabled = false;
            App.Current.Game.Passcode = Passcode.Text;

            App.Current.Game.BattleStatusUpdated += GameBattleStatusUpdated;
            App.Current.Game.Error += GameError;

            App.Current.Game.EnablePolling ();
        }

        void Reset ()
        {
            App.Current.Game.BattleStatusUpdated -= GameBattleStatusUpdated;
            App.Current.Game.Error -= GameError;
            EnterButton.IsEnabled = true;
        }

        async void GameError (object sender, ErrorEventArgs e)
        {
            App.Current.Game.DisablePolling ();
            Reset ();
            await DisplayAlert ("Error", e.Error.Message, "OK");
        }

        void GameBattleStatusUpdated (object sender, BattleStatusEventArgs e)
        {
            Reset ();
        }
    }
}
