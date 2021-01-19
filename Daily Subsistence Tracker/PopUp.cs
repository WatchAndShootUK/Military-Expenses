
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace Daily_Subsistence_Tracker
{
    public partial class PopUp : PopupPage
    {
        public PopUp(StackLayout myLayout)
        {
            this.Content = new StackLayout
            {
                Margin = 30,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                   myLayout
                }
            };
        }
    }
}