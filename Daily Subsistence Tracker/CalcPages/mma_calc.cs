using System;
using Xamarin.Forms;

namespace Daily_Subsistence_Tracker.CalcPages
{
    public class mma_calc : ContentPage
    {
        Label resultLabel { get; set; }
        Entry myEntry { get; set; }

        public mma_calc()
        {
            resultLabel = MakeLabel("", 25);
            Title = "MMA";
            IconImageSource = new FontImageSource { FontFamily = "fa.otf#fa", Glyph = FontAwesomeIcons.FontAwesomeIcons.Car };

            Grid thisGrid = new Grid { RowSpacing = 0 };
            thisGrid.Children.Add(new StackLayout
            {
                Padding = new Thickness(20, 20, 20, 20),
                BackgroundColor = App.colours[2],
                Spacing = 20,
                Children =
            {
                MakeLabel("Use the tool below to calculate what the Motor Mileage Allowance (MMA)   is for a given journey.", 20),
                MakeLabel("Enter mileage:",18),
                mileageEntry(),
                calcButton(),
                resultLabel
            }
            }, 0, 1, 0, 9);
            thisGrid.Children.Add(App.FooterGrid(), 0, 1, 9, 10);

            Content = thisGrid;

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
                if (myEntry.Text == null || Convert.ToInt32(myEntry.Text) < 10)
                {
                    DisplayAlert("Error", "Journey must be a minimum of 10 miles", "Ok");
                }
                else
                {
                    resultLabel.Text = "A journey of " + (Convert.ToInt32(myEntry.Text).ToString() + " miles is authorised an MMA payment of\n\n£" + ((Convert.ToInt32(myEntry.Text) - 6) * 0.25).ToString("##.00"));
                }
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

        private Entry mileageEntry()
        {
            myEntry = new Entry
            {
                Keyboard = Keyboard.Numeric,
                TextColor = Color.WhiteSmoke,
                HorizontalTextAlignment = TextAlignment.Center,
                BackgroundColor = Color.Black,
                Placeholder = "        ",
                Margin = 10,
                FontSize = 15
            };

            return myEntry;
        }
    }
}