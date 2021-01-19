using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Daily_Subsistence_Tracker
{
    public class DeploymentPage : ContentPage
    {
        private static string DeploymentName { get; set; }
        public DeploymentPage(string input_string)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = App.colours[2];
            DeploymentName = input_string;
            Content = drawLayout();
        }

        private Grid drawLayout()
        {
            Grid masterGrid = new Grid { RowSpacing = 0, ColumnSpacing = 0, Padding = 0, Margin = 0 };

            StackLayout thisLayout = new StackLayout { Spacing = 0, MinimumHeightRequest = App.ScreenHeight };

            List<DateTime> datesorter = new List<DateTime>();

            foreach (KeyValuePair<DateTime, List<MyItem>> keys in App.SavedLines[DeploymentName])
            {
                datesorter.Add(keys.Key); // Add DS lines to new list in order to sort by date.
            }

            datesorter.Sort(); // Sort DateTime items.

            if (App.SavedLines[DeploymentName].Count != 0)
            {
                foreach (DateTime line in datesorter)
                {
                    #region Title

                    TapGestureRecognizer tap = new TapGestureRecognizer();
                    tap.Tapped += async (s, e) =>
                    {
                        Navigation.PushAsync(new DS_Day(DeploymentName, line));

                    };

                    Label thisLabel = new Label
                    {
                        Text = line.ToLongDateString().ToString(),
                        TextColor = Color.WhiteSmoke,
                        BackgroundColor = Color.Transparent,
                        FontSize = 30,
                        FontAttributes = FontAttributes.Bold,
                        Padding = 10,
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center
                    };

                    #endregion

                    #region Cash Total
                    decimal cashtotal = 0;
                    int lines = 0;
                    foreach (MyItem item in App.SavedLines[DeploymentName][line])
                    {
                        lines += 1;
                        cashtotal += item.Amount.Value;
                    }
                    Label linesLabel = MakeLabel("Lines: " + lines, 20);
                    linesLabel.HorizontalTextAlignment = TextAlignment.Start;
                    Label cashTotalLabel = MakeLabel("£" + cashtotal.ToString(), 30);
                    cashTotalLabel.HorizontalTextAlignment = TextAlignment.End;
                    if (cashtotal > 25)
                    {
                        cashTotalLabel.TextColor = Color.Red;
                    }
                    else
                    {
                        cashTotalLabel.TextColor = Color.LimeGreen;
                    }

                    #endregion

                    thisLabel.GestureRecognizers.Add(tap);
                    linesLabel.GestureRecognizers.Add(tap);
                    cashTotalLabel.GestureRecognizers.Add(tap);

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
                        bool answer = await DisplayAlert("Warning", "Are you sure you want to delete " + line.ToString() + "?\n This cannot be reversed.", "Yes", "No");
                        if (answer == true)
                        {
                            App.SavedLines[DeploymentName].Remove(line);
                            Content = drawLayout();
                        }

                    };
                    deleteLabel.GestureRecognizers.Add(tap1);
                    #endregion

                    #region Grid
                    Grid grid = new Grid
                    {
                        RowSpacing = 0,
                        ColumnSpacing = 0,
                        Margin = 0,
                        Padding = 0,
                        BackgroundColor = App.GetRandomColour()
                    };

                    grid.Children.Add(thisLabel, 0, 7, 0, 1);
                    grid.Children.Add(deleteLabel, 7, 8, 0, 1);
                    grid.Children.Add(linesLabel, 0, 4, 1, 2);
                    grid.Children.Add(cashTotalLabel, 0, 8, 1, 2);
                    #endregion

                    thisLayout.Children.Add(new Frame { CornerRadius = 10, Margin = 5, Padding = 0, Content = grid });
                }
            }
            else
            {
                thisLayout.Children.Add(new Label
                {
                    Text = "No DS lines added yet.\nTo add a new DS line tap the '+' in the header.",
                    FontSize = 25,
                    TextColor = Color.WhiteSmoke,
                    FontAttributes = FontAttributes.Bold,
                    Padding = 10,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Start
                });
            }

            masterGrid.Children.Add(HeaderGrid(), 0, 1, 0, 1);
            masterGrid.Children.Add(new ScrollView { Content = thisLayout }, 0, 1, 1, 9);
            masterGrid.Children.Add(FooterGrid(), 0, 1, 9, 10);

            return masterGrid;
        }
        private Grid HeaderGrid()
        {
            Label emailLabel = MakeLabel("e",20);
            TapGestureRecognizer tap1 = new TapGestureRecognizer();
            tap1.Tapped += async (s, e) =>
            {
                await Email.ComposeAsync("Your expenses report for " + DeploymentName + ", from Watch&Shoot UK.", App.SavedLines[DeploymentName].ToString(), "markwileman@ymail.com");
            };
            emailLabel.GestureRecognizers.Add(tap1);

            Grid grid = new Grid { HeightRequest = App.ScreenHeight * 0.1, BackgroundColor = App.colours[0] };
            grid.Children.Add(new Label
            {
                Text = DeploymentName,
                FontAttributes = FontAttributes.Bold,
                FontSize = 25,
                TextColor = Color.WhiteSmoke,
                VerticalTextAlignment = TextAlignment.Center,
                Padding = 10
            }, 0, 9, 0, 1);
            grid.Children.Add(AddLine(), 8, 10, 0, 1);
            grid.Children.Add(emailLabel, 6, 8, 0, 1);
            return grid;
        }
        private Grid FooterGrid()
        {
            return new Grid { BackgroundColor = App.colours[0] };
        }
        private Label AddLine()
        {
            #region Add Line Button
            Label thisLabel = new Label { Text = FontAwesomeIcons.FontAwesomeIcons.Plus, FontFamily = "fa.otf#fa", FontSize = 30, TextColor = Color.WhiteSmoke, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) =>
            {
                PopupNavigation.PushAsync(new PopUp(AddNewLineForm()));
            };
            thisLabel.GestureRecognizers.Add(tap);
            return thisLabel;
            #endregion
        }
        private StackLayout AddNewLineForm()
        {
            FileResult photo = null;
            string PhotoPath = null;

            #region Define elements
            DatePicker dateEntry = new DatePicker { 
                Date = DateTime.Now  
            };
            TimePicker timeEntry = new TimePicker { 
                Time = DateTime.Now.TimeOfDay, 
                HorizontalOptions = LayoutOptions.End 
            };
            Picker currencyPicker = new Picker {
                ItemsSource = new List<string>() { "£", "€", "$" },
                SelectedIndex = 0,
                HorizontalTextAlignment = TextAlignment.Start,
                FontAttributes = FontAttributes.Bold,
            };
            Entry amountEntry = new Entry { 
                Keyboard = Keyboard.Numeric 
            };
            Entry descEntry = new Entry { 
                Placeholder = "e.g. Lunch at services"
            };

            Label recieptButton = new Label { 
                Text = FontAwesomeIcons.FontAwesomeIcons.Camera,
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.WhiteSmoke,
                FontFamily = "fa.otf#fa"
            };
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                photo = await MediaPicker.CapturePhotoAsync();
                if (photo != null)
                {
                    var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                    using (var stream = await photo.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newFile))
                        await stream.CopyToAsync(newStream);
                    PhotoPath = newFile;
                }
            };
            recieptButton.GestureRecognizers.Add(tap);
            #endregion

            #region Enter Button 
            Label enterButton = new Label { 
                Text = "Add",
                FontSize = 30,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.WhiteSmoke,
                FontAttributes = FontAttributes.Bold};

            TapGestureRecognizer tap1 = new TapGestureRecognizer();
            tap1.Tapped += (s, e) =>
            {
                if (amountEntry.Text == null)
                {
                    DisplayAlert("Error", "You must enter a reciept amount in " + currencyPicker.SelectedItem + ".", "OK");
                }
                else
                {
                    decimal dcml = Convert.ToDecimal(amountEntry.Text);
                    PopupNavigation.PopAsync();
                    MyItem newLineList = new MyItem()
                    {
                        Time = timeEntry.Time.Hours.ToString() + ":" + timeEntry.Time.Minutes.ToString(),
                        Amount = new KeyValuePair<string, decimal>(currencyPicker.SelectedItem.ToString(), dcml),
                        Reciept = PhotoPath,
                        Desc = descEntry.Text
                    };

                    if (App.SavedLines[DeploymentName].ContainsKey(dateEntry.Date))
                    {
                        App.SavedLines[DeploymentName][dateEntry.Date].Add(newLineList);
                    }
                    else
                    {
                        App.SavedLines[DeploymentName].Add(dateEntry.Date, new List<MyItem>() { newLineList });
                    }
                    Content = drawLayout();
                }
            };

            enterButton.GestureRecognizers.Add(tap1);
            #endregion

            #region Draw Grid Layout

            Grid grid = new Grid();

            grid.Children.Add(MakeLabel("DTG", 20), 0, 1, 0, 1);
            grid.Children.Add(dateEntry, 1, 3, 0, 1);
            grid.Children.Add(timeEntry, 2, 3, 0, 1);


            grid.Children.Add(MakeLabel("Reciept", 20), 0, 1, 1, 2);
            Grid recieptGrid = new Grid();
            recieptGrid.Children.Add(currencyPicker, 0, 1, 0, 1);
            recieptGrid.Children.Add(amountEntry, 1, 5, 0, 1);
            recieptGrid.Children.Add(recieptButton, 5, 6, 0, 1);
            grid.Children.Add(recieptGrid, 1, 3, 1, 2);

            grid.Children.Add(MakeLabel("Remarks",20), 0, 1, 2, 3);
            grid.Children.Add(descEntry, 1, 3, 2, 3);

            grid.Children.Add(new Frame
            {
                Content = enterButton,
                CornerRadius = 10,
                Padding = 0,
                Margin = 0,
                BackgroundColor = Color.LimeGreen
            },0,3,3,4);
            #endregion

            return new StackLayout { Padding = 10, Children = { 
                    MakeLabel("Add new expenses line", 25),
                    new BoxView {Color = Color.WhiteSmoke, HeightRequest = 1},
                    grid 
                }, BackgroundColor = App.GetRandomColour()};

        }
        private Label MakeLabel (string text, int size)
        {
            return new Label
            {
                Text = text,
                FontSize = size,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.WhiteSmoke,
                Padding = 10
            };
        }
    }
}