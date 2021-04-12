using Rg.Plugins.Popup.Services;
using System;
using System.IO;

using Xamarin.Forms;

namespace Daily_Subsistence_Tracker
{
    public class DS_Day : ContentPage
    {
        string DeploymentName { get; set; }
        DateTime DayName { get; set; }
        public DS_Day(string thisDeployment, DateTime thisDay)
        {
            DeploymentName = thisDeployment;
            DayName = thisDay;

            BackgroundColor = App.colours[2];

            Content = DrawLayout(thisDeployment, thisDay);
        }

        public Grid DrawLayout(string thisDeployment, DateTime thisDay)
        {
            StackLayout thisLayout = new StackLayout { Padding = 1, Spacing = 5, MinimumHeightRequest = App.ScreenHeight };

            foreach (MyItem line in App.SavedLines[thisDeployment][thisDay])
            {
                #region Draw Results Grid
                Grid grid = new Grid { BackgroundColor = App.GetRandomColour() };

                grid.Children.Add(MakeLabel(line.Line.ToString()), 0, 1, 0, 1);
                grid.Children.Add(MakeLabel(line.Time.ToString()), 1, 3, 0, 1);
                grid.Children.Add(MakeLabel(line.Amount.Key.ToString() + line.Amount.Value.ToString("##.00")), 3, 5, 0, 1);

                if (line.Desc == null)
                {
                    grid.Children.Add(FALabel(FontAwesomeIcons.FontAwesomeIcons.CommentDots, 20, true), 5, 6, 0, 1);
                    grid.Children.Add(FALabel(FontAwesomeIcons.FontAwesomeIcons.Ban, 35, false), 5, 6, 0, 1);
                }
                else
                {
                    grid.Children.Add(RemarksButton(line.Desc), 5, 6, 0, 1);
                }

                if (line.Reciept == null)
                {
                    grid.Children.Add(FALabel(FontAwesomeIcons.FontAwesomeIcons.Image, 20, true), 6, 7, 0, 1);
                    grid.Children.Add(FALabel(FontAwesomeIcons.FontAwesomeIcons.Ban, 35, false), 6, 7, 0, 1);
                }
                else
                {
                    grid.Children.Add(ImageButton(line.Reciept.ToString()), 6, 7, 0, 1);
                }
                grid.Children.Add(DeleteButton(), 7, 8, 0, 1);



                #endregion

                #region Delete Button
                Label DeleteButton()
                {
                    Label thisLabel = new Label
                    {
                        Text = FontAwesomeIcons.FontAwesomeIcons.TrashAlt,
                        FontFamily = "fa.otf#fa",
                        TextColor = Color.WhiteSmoke,
                        BackgroundColor = Color.Transparent,
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                        Padding = 10,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    TapGestureRecognizer tap1 = new TapGestureRecognizer();
                    tap1.Tapped += async (s, e) =>
                    {
                        bool answer = await DisplayAlert("Warning", "Are you sure you want to delete this item?\nThis cannot be reversed.", "Yes", "No");
                        if (answer == true)
                        {
                            App.SavedLines[thisDeployment][thisDay].Remove(line);
                            if (line.Reciept != null) { File.Delete(line.Reciept); }
                            App.UpdateCache();
                            Content = DrawLayout(thisDeployment, thisDay);
                        }

                    };
                    thisLabel.GestureRecognizers.Add(tap1);


                    return thisLabel;
                }
                #endregion

                thisLayout.Children.Add(grid);
            
            }

            Grid masterGrid = new Grid { Padding = 0, RowSpacing = 0, ColumnSpacing = 0, Margin = 0 };

                (App.Current.MainPage as NavigationPage).BarBackgroundColor = App.colours[0];
                (App.Current.MainPage as NavigationPage).BarTextColor = Color.WhiteSmoke;
                Title = DeploymentName + ": " + DayName.ToString("M");
                masterGrid.Children.Add(new ScrollView { Content = thisLayout }, 0, 1, 0, 9);
                masterGrid.Children.Add(App.FooterGrid(), 0, 1, 9, 10);

            return masterGrid;
        }

        private Label MakeLabel(string text)
        {
            return new Label
            {
                Padding = 5,
                Text = text,
                FontSize = 20,
                TextColor = Color.WhiteSmoke,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center
            };
        }
        private Label RemarksButton(string remarks)
        {
            var thisLabel = MakeLabel(null);
            thisLabel.FontFamily = "fa.otf#fa";
            thisLabel.FontAttributes = FontAttributes.Bold;

            thisLabel.Text = FontAwesomeIcons.FontAwesomeIcons.CommentDots;
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                PopupNavigation.PushAsync(new PopUp(
                    new StackLayout
                    {
                        BackgroundColor = App.GetRandomColour(),
                        Children =
                        {
                                MakeLabel("Remarks"),
                                MakeLabel(remarks)
                        }
                    }));
            };
            thisLabel.GestureRecognizers.Add(tap);

            return thisLabel;
        }
        private Label ImageButton(string path)
        {
            var thisLabel = MakeLabel(null);
            thisLabel.FontFamily = "fa.otf#fa";
            thisLabel.FontAttributes = FontAttributes.Bold;

            thisLabel.Text = FontAwesomeIcons.FontAwesomeIcons.Image;
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                Image thisImage = new Image
                {
                    Source = path
                };

                PopupNavigation.PushAsync(new PopUp(
                    new StackLayout
                    {
                        BackgroundColor = App.GetRandomColour(),
                        Children =
                        {
                                MakeLabel("Reciept Image (" + path.ToString() + ")"),
                                thisImage
                        }
                    }));
            };
            thisLabel.GestureRecognizers.Add(tap);

            return thisLabel;
        }
        private Label FALabel(string text, int size, bool onBottom)
        {
            Label BanLabel = MakeLabel(text);
            BanLabel.FontSize = size;
            BanLabel.FontFamily = "fa.otf#fa";
            if (onBottom == true)
            {
                BanLabel.TextColor = Color.WhiteSmoke;
            }
            else
            {
                BanLabel.TextColor = Color.Black;
            }
            if (text == FontAwesomeIcons.FontAwesomeIcons.Ban)
            {
                BanLabel.InputTransparent = true;
            }
            return BanLabel;
        }
    }
}