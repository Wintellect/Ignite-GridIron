using Xamarin.Forms;

namespace GridIron
{
    public partial class WinnerPage : ContentPage
    {
        public WinnerPage (string competitionWinner)
        {
            InitializeComponent ();

            WinnerLabel.Text = competitionWinner;
        }
    }
}
