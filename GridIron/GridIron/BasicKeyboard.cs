using Xamarin.Forms;

namespace GridIron
{
    public class BasicKeyboard
    {
        public static Keyboard Keyboard { get; private set; }

        static BasicKeyboard ()
        {
            Keyboard = Keyboard.Create (KeyboardFlags.None);
        }
    }
}
