using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Daily_Subsistence_Tracker
{
    public class StartPage : ContentPage
    {
        public StartPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = App.colours[2];
            Content = drawLayout();
        }
        private Grid drawLayout()
        {
            Grid masterGrid = new Grid { RowSpacing = 0, ColumnSpacing = 0, Padding = 0, Margin = 0 };

            StackLayout thisLayout = new StackLayout { Spacing = 0, MinimumHeightRequest = App.ScreenHeight };
            if (App.SavedLines.Count != 0)
            {
                foreach (KeyValuePair<string, Dictionary<DateTime, List<MyItem>>> line in App.SavedLines)
                {
                    thisLayout.Children.Add(DrawDeploymentButton(line.Key, App.GetRandomColour()));
                }
            }
            else
            {

                thisLayout.Children.Add(new Label
                {
                    Text = "Welcome to the Military Expenses tracker, by Watch & Shoot Developments.\n\nYou currently have no deployments to show.\n\nTo add a new deployment tap the '+' in the header.",
                    FontSize = 20,
                    TextColor = Color.WhiteSmoke,
                    FontAttributes = FontAttributes.Bold,
                    Padding = 10,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Start
                });
            }

            masterGrid.Children.Add(HeaderGrid(), 0, 1, 0, 1);
            masterGrid.Children.Add(new ScrollView { Content = thisLayout }, 0, 1, 1, 9);
            masterGrid.Children.Add(App.FooterGrid(), 0, 1, 9, 10);

            return masterGrid;
        }
        private Grid HeaderGrid()
        {
            Grid grid = new Grid { HeightRequest = App.ScreenHeight * 0.1, BackgroundColor = App.colours[0] };
            grid.Children.Add(new Label
            {
                Text = "Military Expenses",
                FontAttributes = FontAttributes.Bold,
                FontSize = 25,
                TextColor = Color.WhiteSmoke,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 10
            }, 0, 4, 0, 1);
            grid.Children.Add(CalculatorButton(), 4, 5, 0, 1);
            grid.Children.Add(AddSmallNewDeploymentButton(), 5, 6, 0, 1);
            return grid;
        }
        private Label AddSmallNewDeploymentButton()
        {
            Label thisLabel = new Label { Text = FontAwesomeIcons.FontAwesomeIcons.Plus, FontFamily = "fa.otf#fa", FontSize = 30, TextColor = Color.WhiteSmoke, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                string result = await DisplayPromptAsync("Add Deployment", "Enter deployment name.", "Ok", "Cancel", "", 17);
                if (result != null && result != "")
                {
                    if (App.SavedLines.ContainsKey(result))
                    {
                        DisplayAlert(null, result + " has already been added!", "OK");
                    }
                    else
                    {
                        App.SavedLines.Add(result, new Dictionary<DateTime, List<MyItem>>());
                        App.UpdateCache();
                        Content = drawLayout();
                    }
                }
            };
            thisLabel.GestureRecognizers.Add(tap);

            return thisLabel;
        }
        private Frame DrawDeploymentButton(string text, Color colour)
        {
            #region Title
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                Navigation.PushAsync(new DeploymentPage(text));
            };
            Label thisLabel = new Label
            {
                Text = text,
                TextColor = Color.WhiteSmoke,
                BackgroundColor = Color.Transparent,
                FontSize = 30,
                FontAttributes = FontAttributes.Bold,
                Padding = 10,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center
            };
            thisLabel.GestureRecognizers.Add(tap);
            #endregion

            #region Delete Button
            Label deleteLabel = new Label
            {
                Text = FontAwesomeIcons.FontAwesomeIcons.TrashAlt,
                FontFamily = "fa.otf#fa",
                TextColor = Color.WhiteSmoke,
                BackgroundColor = Color.Transparent,
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                Padding = 10,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            TapGestureRecognizer tap1 = new TapGestureRecognizer();
            tap1.Tapped += async (s, e) =>
            {
                bool answer = await DisplayAlert("Warning", "Are you sure you want to delete " + text + "?\n This cannot be reversed.", "Yes", "No");
                if (answer == true)
                {
                    Xamarin.Forms.Application.Current.Properties.Clear();
                    App.SavedLines.Remove(text);
                    App.UpdateCache();
                    Content = drawLayout();
                }



            };
            deleteLabel.GestureRecognizers.Add(tap1);
            #endregion

            #region Edit Button
            Label editLabel = new Label
            {
                Text = FontAwesomeIcons.FontAwesomeIcons.Edit,
                FontFamily = "fa.otf#fa",
                TextColor = Color.WhiteSmoke,
                BackgroundColor = Color.Transparent,
                FontSize = 25,
                FontAttributes = FontAttributes.Bold,
                Padding = 10,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            TapGestureRecognizer tap2 = new TapGestureRecognizer();
            tap2.Tapped += async (s, e) =>
            {
                string answer = await DisplayPromptAsync(null, "Update deployment name", "OK", "Cancel", "", 20, Keyboard.Plain, text);
                if (answer != null)
                {
                    if (answer != text && answer != "")
                    {
                        App.SavedLines.Add(answer, App.SavedLines[text]);
                        App.SavedLines.Remove(text);
                        App.UpdateCache();
                        Content = drawLayout();
                    }
                }
            };
            editLabel.GestureRecognizers.Add(tap2);
            #endregion

            #region Grid View

            Grid grid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                Margin = 0,
                Padding = 0,
                BackgroundColor = colour
            };

            grid.Children.Add(thisLabel, 0, 6, 0, 1);
            grid.Children.Add(deleteLabel, 7, 8, 0, 1);
            grid.Children.Add(editLabel, 6, 7, 0, 1);
            #endregion

            return new Frame { CornerRadius = 10, Margin = 5, Padding = 0, Content = grid };
        }

        private Label CalculatorButton()
        {
            Label thisLabel = new Label { Text = FontAwesomeIcons.FontAwesomeIcons.Calculator, FontFamily = "fa.otf#fa", FontSize = 30, TextColor = Color.WhiteSmoke, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {

                Navigation.PushAsync(new MasterTabbedPage());
            };
            thisLabel.GestureRecognizers.Add(tap);

            return thisLabel;
        }
    }
}