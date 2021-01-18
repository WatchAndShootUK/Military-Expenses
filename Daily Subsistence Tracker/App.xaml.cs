using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Daily_Subsistence_Tracker
{

    public class MyItem
    {
        public string Time { get; set; }
        public KeyValuePair<string,decimal> Amount { get; set; }
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
            SavedLines = new Dictionary<string, Dictionary<DateTime, List<MyItem>>>();
            //SavedLines = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<DateTime, List<MyItem>>>>(json);
            MainPage = new NavigationPage (new StartPage());
        }
        public static Color GetRandomColour()
        {
            Random r = new Random();
            return colours[r.Next(3,7)];
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        

    }
}
