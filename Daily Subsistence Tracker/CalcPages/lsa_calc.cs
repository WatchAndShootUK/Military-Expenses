
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Daily_Subsistence_Tracker.CalcPages
{
    public class lsa_calc : ContentPage
    {
        Picker myPicker { get; set; }
        Entry myEntry { get; set; }
        Label resultLabel { get; set; }

        public lsa_calc()
        {
            resultLabel = MakeLabel("", 25);
            Title = "LSA";
            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Padding = new Thickness(20, 20, 20, 20),
                    BackgroundColor = App.colours[2],
                    Spacing = 20,
                    Children =
            {
                MakeLabel("Use the tool below to calculate how much Longer Separation Allowance (LSA) you'll earn on a deployment.", 20),
                MakeLabel("Select LSA level:",18),
                LSAPicker(),
                MakeLabel("Deployment length (days):",18),
                dayEntry(),
                calcButton(),
                resultLabel
            }
                }
            };
        }

        private Frame calcButton()
        {
            Label thisLabel = new Label
            {
                BackgroundColor = Color.LimeGreen,
                Text = "Calculate",
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.Black,
                Padding = 10
            };

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                calculate_LSA();
            };

            thisLabel.GestureRecognizers.Add(tap);

            return new Frame
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                CornerRadius = 10,
                Content = thisLabel,
                Padding = 0,
                Margin = 0
            };
        }

        private Entry dayEntry()
        {
            myEntry = new Entry
            {
                Keyboard = Keyboard.Numeric,
                TextColor = Color.WhiteSmoke,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 15
            };

            return myEntry;
        }
        private Picker LSAPicker()
        {
            List<string> myList = new List<string>()
            {
                "",
                "Level 1 (0 - 280 days)",
                "Level 2 (281 - 460 days)",
                "Level 3 (461 – 640 days)",
                "Level 4 (641 – 820 days)",
                "Level 5 (821 – 1000 days)",
                "Level 6 (1001 – 1180 days)",
                "Level 7 (1181 – 1360 days)",
                "Level 8 (1361 – 1540 days)",
                "Level 9 (1541 – 1720 days)",
                "Level 10 (1721 – 1900 days)",
                "Level 11 (1901 – 2080 days)",
                "Level 12 (2081 – 2260 days)",
                "Level 13 (2261 – 2440 days)",
                "Level 14 (2441 – 2800 days)",
                "Level 15 (2801 – 3160 days)",
                "Level 16 (3161+ days)"
            };

            myPicker = new Picker
            {
                ItemsSource = myList,
                TextColor = Color.WhiteSmoke,
                SelectedIndex = 0,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 15
            };

            return myPicker;
        }
        private Label MakeLabel(string text, int size)
        {
            return new Label
            {
                Text = text,
                FontSize = size,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.WhiteSmoke,
                Padding = 10
            };
        }

        private void calculate_LSA()
        {
            List<double> LSAlevels = new List<double>()
            {
                0,
                7.60,
                11.88,
                16.17,
                17.75,
                19.10,
                20.47,
                21.82,
                23.87,
                25.25,
                26.61,
                27.97,
                29.35,
                30.69,
                32.06,
                33.41,
                34.75
            };

            if (myPicker.SelectedIndex != 0 && myEntry.Text != null)
            {
                double result = LSAlevels[myPicker.SelectedIndex] * Convert.ToDouble(myEntry.Text);
                resultLabel.Text = "Deploying for " + myEntry.Text.ToString() + " days at LSA level " + myPicker.SelectedIndex.ToString() + " will earn (pre tax):\n\n£" + result.ToString("##.00");
            }
        }


    }
}