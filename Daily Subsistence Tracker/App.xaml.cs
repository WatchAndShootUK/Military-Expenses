using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Daily_Subsistence_Tracker
{

    public class MyItem
    {
        public int Line { get; set; }
        public string Time { get; set; }
        public KeyValuePair<string, decimal> Amount { get; set; }
        public string Desc { get; set; }
        public string Reciept { get; set; }
    }

    public partial class App : Application
    {
        public static Dictionary<string, Dictionary<DateTime, List<MyItem>>> SavedLines { get; set; }
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public static List<Color> colours { get; set; }

        public App()
        {
            InitializeComponent();
            colours = new List<Color>() {
                Color.FromHex("#2a2a2a"),
                Color.FromHex("#FFFFFF"),
                Color.FromHex("#5b594a"),
                Color.FromHex("#7b664a"),
                Color.FromHex("#958a68"),
                Color.FromHex("#77775f"),
                Color.FromHex("#958b60"),
                Color.FromHex("#9d9b80")
                };

            OnStart();
            MainPage = new NavigationPage(new StartPage());
        }


        public static Color GetRandomColour()
        {
            Random r = new Random();
            return colours[r.Next(3, 7)];
        }
        protected override void OnStart()
        {
            if (App.Current.Properties.ContainsKey("DataCache"))
            {
                SavedLines = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<DateTime, List<MyItem>>>>(Application.Current.Properties["DataCache"].ToString());
            }
            else
            {
                Xamarin.Forms.Application.Current.Properties.Clear();
                SavedLines = new Dictionary<string, Dictionary<DateTime, List<MyItem>>>();
            }
        }
        protected override void OnSleep()
        {
        }
        protected override void OnResume()
        {
        }
        public static Grid FooterGrid()
        {
            Grid logoGrid = new Grid
            {
                Children =
                {
                    new Label {TextColor = GetRandomColour(), Text = FontAwesomeIcons.FontAwesomeIcons.CircleNotch, FontFamily = "fa.otf#fa", FontSize = 50, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.None },
                    new Label {TextColor = GetRandomColour(), Text = FontAwesomeIcons.FontAwesomeIcons.Skull, FontFamily = "fa.otf#fa", FontSize = 20, VerticalTextAlignment = TextAlignment.Center, HorizontalTextAlignment = TextAlignment.Center },
                }
            };

            Grid thisGrid = new Grid { BackgroundColor = App.colours[0] };
            thisGrid.Children.Add(logoGrid, 0, 1, 0, 1);
            thisGrid.Children.Add(new Label
            {
                Text = "Click here for more applications by Watch & Shoot Developments. Avaliable on Android and iOS.",
                TextColor = GetRandomColour(),
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 15,
                Padding = 10,
                FontAttributes = FontAttributes.Bold
            }, 1, 5, 0, 1);

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                Device.OpenUri(new System.Uri("https://play.google.com/store/apps/developer?id=Watch%26Shoot"));
            };

            thisGrid.GestureRecognizers.Add(tap);
            return thisGrid;
        }
        public static void UpdateCache()
        {
            Application.Current.Properties.Clear();
            Application.Current.Properties["DataCache"] = JsonConvert.SerializeObject(App.SavedLines);
        }
    }
}
