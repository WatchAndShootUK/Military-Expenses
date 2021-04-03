
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daily_Subsistence_Tracker
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MasterTabbedPage : TabbedPage
    {
        public MasterTabbedPage()
        {
            ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = App.colours[0];
            ((NavigationPage)Application.Current.MainPage).BarTextColor = Color.WhiteSmoke;
            this.BarBackgroundColor = App.colours[0];
            Title = "Military Allowances Calculator";
            Children.Add(new CalcPages.mylsa_calc());
            Children.Add(new CalcPages.mma_calc());
        }
    }
}