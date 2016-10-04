using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

//[assembly: ExportRenderer (typeof (Button), typeof (GridIron.Droid.ZeroPaddingRenderer))]

namespace GridIron.Droid
{
    public class ZeroPaddingRenderer : ButtonRenderer
    {
        protected override void OnElementChanged (ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged (e);

            Control?.SetPadding (0, Control.PaddingTop, 0, Control.PaddingBottom);
        }
    }
}
