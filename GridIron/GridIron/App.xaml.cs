using System.Diagnostics;
using Xamarin.Forms;
using Plugin.DeviceInfo;

namespace GridIron
{
    public partial class App : Application
    {
        public GameData Game { get; private set; }

        new static public App Current {
            get {
                return (App)Application.Current;
            }
        }

        public App ()
        {
            InitializeComponent ();

            DefaultEntryStyle.Setters.Add (new Setter { Property = InputView.KeyboardProperty, Value = BasicKeyboard.Keyboard });

            Game = new GameData ();
            Game.UserId = CrossDeviceInfo.Current.Id;

            Game.Error += Error;
            Game.BattleStatusUpdated += GameBattleStatusUpdated;

            MainPage = new PasscodePage ();
        }

        void Error (object sender, ErrorEventArgs e)
        {
            var err = e.Error;
            while (err != null) {
                Debug.WriteLine ("Error {0}: {1}", err.GetType ().Name, err.Message);
                Debug.WriteLine (err.StackTrace);
                err = err.InnerException;
            }
        }

        void GameBattleStatusUpdated (object sender, BattleStatusEventArgs e)
        {
            switch (e.BattleStatus) {
            case "closed":
                Current.MainPage = new IntermissionPage ();
                break;
            case "open":
            case "active":
                Current.MainPage = new BattlePage ();
                break;
            case "final":
                Current.MainPage = new WinnerPage (e.WinningArtist);
                break;
            }
        }

        protected override void OnStart ()
        {
            // Handle when your app starts
        }

        protected override void OnSleep ()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume ()
        {
            // Handle when your app resumes
        }
    }
}
