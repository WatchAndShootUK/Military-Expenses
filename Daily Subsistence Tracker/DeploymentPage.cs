using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Daily_Subsistence_Tracker
{
    public class DeploymentPage : ContentPage
    {
        private bool photo_attempted;
        private bool just_tapped = false;

        private static string DeploymentName { get; set; }

        public DeploymentPage(string input_string)
        {
            (App.Current.MainPage as NavigationPage).BarBackgroundColor = App.colours[0];
            (App.Current.MainPage as NavigationPage).BarTextColor = Color.WhiteSmoke;

            BackgroundColor = App.colours[2];
            DeploymentName = input_string;
        }

        protected override void OnAppearing()
        {
            Content = drawLayout();
        }

        private Grid drawLayout()
        {
            Grid masterGrid = new Grid { RowSpacing = 0, ColumnSpacing = 0, Padding = 0, Margin = 0 };

            if (Device.RuntimePlatform == Device.Android)
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }

            masterGrid.Children.Add(HeaderGrid(), 0, 1, 0, 1);
            masterGrid.Children.Add(new ScrollView { Content = DrawThisLayout() }, 0, 1, 1, 9);
            masterGrid.Children.Add(App.FooterGrid(), 0, 1, 9, 10);

            return masterGrid;
        }

        private StackLayout DrawThisLayout()
        {
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

                    Label linesLabel = MakeLabel("Lines: " + App.SavedLines[DeploymentName][line].Count(), 20);
                    linesLabel.HorizontalTextAlignment = TextAlignment.Start;
                    Label cashTotalLabel = MakeLabel("£" + App.SavedLines[DeploymentName][line].Sum(x => x.Amount.Value).ToString("##.00"), 30);
                    cashTotalLabel.HorizontalTextAlignment = TextAlignment.End;

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
                        bool answer = await DisplayAlert("Warning", "Are you sure you want to delete " + line.ToString("M") + "?\n This cannot be reversed.", "Yes", "No");
                        if (answer == true)
                        {
                            foreach (MyItem item in App.SavedLines[DeploymentName][line])
                            {
                                if (item.Reciept != null)
                                {
                                    File.Delete(item.Reciept); // Remove cached images.
                                }
                            }
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

            return thisLayout;
        }

        private Grid HeaderGrid()
        {
            Label emailLabel = MakeLabel(FontAwesomeIcons.FontAwesomeIcons.Envelope, 28);
            emailLabel.FontFamily = "fa.otf#fa";
            emailLabel.HorizontalTextAlignment = TextAlignment.Center;
            emailLabel.Padding = 0;
            TapGestureRecognizer tap1 = new TapGestureRecognizer();
            tap1.Tapped += async (s, e) =>
            {
                await EmailClass.SendEmail(DeploymentName + " expenses report.", create_email_format(), new List<string>() { "" }, get_images());
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
            }, 0, 6, 0, 1);
            grid.Children.Add(emailLabel, 6, 7, 0, 1);
            grid.Children.Add(AddLine(), 7, 8, 0, 1);
            return grid;
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
            DatePicker dateEntry = new DatePicker
            {
                Date = DateTime.Now
            };
            TimePicker timeEntry = new TimePicker
            {
                Time = DateTime.Now.TimeOfDay,
                HorizontalOptions = LayoutOptions.End
            };
            Picker currencyPicker = new Picker
            {
                ItemsSource = new List<string>() { "£", "€", "$" },
                SelectedIndex = 0,
                HorizontalTextAlignment = TextAlignment.Start,
                FontAttributes = FontAttributes.Bold,
            };
            Entry amountEntry = new Entry
            {
                Keyboard = Keyboard.Numeric
            };
            Entry descEntry = new Entry
            {
                Placeholder = "e.g. Lunch at services"
            };

            Label recieptButton = new Label
            {
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
                    photo_attempted = true;
                    var newFile = Path.Combine(FileSystem.CacheDirectory, dateEntry.Date.ToString("ddMMM_") + get_count() + ".jpg");
                    using (var stream = await photo.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newFile))
                        await stream.CopyToAsync(newStream);
                    PhotoPath = newFile;
                }
                else
                {
                    DisplayAlert("Error", "No photo taken, please try again.", "Ok");
                }

            };

            recieptButton.GestureRecognizers.Add(tap);
            #endregion

            #region Enter Button 
            Label enterButton = new Label
            {
                Text = "Add",
                FontSize = 30,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Color.WhiteSmoke,
                FontAttributes = FontAttributes.Bold
            };

            TapGestureRecognizer tap1 = new TapGestureRecognizer();
            tap1.Tapped += async (s, e) =>
            {
                if (just_tapped == true)
                {
                    await Task.Delay(1000);
                    just_tapped = false;
                }
                else
                {
                    just_tapped = true;
                    TapSuccess();
                }

                void TapSuccess()
                {
                    if (amountEntry.Text == null)
                    {
                        DisplayAlert("Error", "You must enter a reciept amount in " + currencyPicker.SelectedItem + ".", "OK");
                    }
                    else
                    {
                        PopupNavigation.PopAsync();

                        if (photo_attempted == true && PhotoPath == null) { DisplayAlert("Warning!", "Input failed!", "OK"); photo_attempted = false; } // Catches empty photo file if stream is laggy.
                        else
                        {
                            photo_attempted = false;

                            decimal dcml = Convert.ToDecimal(amountEntry.Text);
                            MyItem newLineList = new MyItem()
                            {
                                Time = timeEntry.Time.Hours.ToString() + ":" + timeEntry.Time.Minutes.ToString(),
                                Amount = new KeyValuePair<string, decimal>(currencyPicker.SelectedItem.ToString(), dcml),
                                Reciept = PhotoPath,
                                Desc = descEntry.Text,
                                Line = get_count()
                            };

                            if (App.SavedLines[DeploymentName].ContainsKey(dateEntry.Date))
                            {
                                App.SavedLines[DeploymentName][dateEntry.Date].Add(newLineList);
                                App.UpdateCache();
                            }
                            else
                            {
                                App.SavedLines[DeploymentName].Add(dateEntry.Date, new List<MyItem>() { newLineList });
                                App.UpdateCache();
                            }

                            Content = drawLayout();

                            Application.Current.Properties["DataCache"] = JsonConvert.SerializeObject(App.SavedLines);
                        }
                    }

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

            grid.Children.Add(MakeLabel("Remarks", 20), 0, 1, 2, 3);
            grid.Children.Add(descEntry, 1, 3, 2, 3);

            grid.Children.Add(new Frame
            {
                Content = enterButton,
                CornerRadius = 10,
                Padding = 0,
                Margin = 0,
                BackgroundColor = Color.LimeGreen
            }, 0, 3, 3, 4);
            #endregion

            #region Counter
            int get_count()
            {
                int i;

                if (App.SavedLines[DeploymentName].ContainsKey(dateEntry.Date))
                {
                    i = App.SavedLines[DeploymentName][dateEntry.Date].Count() + 1;
                }
                else
                {
                    i = 1;
                }

                return i;
            }
            #endregion

            return new StackLayout
            {
                Padding = 10,
                Children = {
                    MakeLabel("Add new expenses line", 25),
                    new BoxView {Color = Color.WhiteSmoke, HeightRequest = 1},
                    grid
                },
                BackgroundColor = App.GetRandomColour()
            };

        }
        private Label MakeLabel(string text, int size)
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
        private string create_email_format()
        {
            List<string> myList = new List<string>();

            myList.Add("Your expenses report for " + DeploymentName + ".\n");

            string currency = "";
            decimal deploymentTotal = 0;

            foreach (KeyValuePair<DateTime, List<MyItem>> line in App.SavedLines[DeploymentName])
            {
                myList.Add(line.Key.ToLongDateString());
                int i = 1;
                decimal total = 0;
                foreach (MyItem item in line.Value)
                {
                    string myDesc = "";
                    string myReciept = "";
                    if (item.Desc != null) { myDesc = item.Desc.ToString(); }

                    myList.Add(i.ToString() + " / " + item.Time.ToString() + " / " + myDesc + " / " + item.Amount.Key.ToString() + item.Amount.Value.ToString("##.00"));

                    i += 1;

                    total = total + item.Amount.Value;

                    currency = item.Amount.Key.ToString();
                }
                deploymentTotal = deploymentTotal + total;
                myList.Add("Daily total: " + currency + total.ToString("##.00") + "\n");
            }

            string myString = "";
            foreach (string str in myList)
            {
                myString = myString + str + "\n";
            }

            myString = myString + DeploymentName + " Total: " + currency + deploymentTotal.ToString("##.00") + "\n";
            myString = myString + "\nExpense report provided by Watch&ShootUK, check our other apps on Google Play and the App Store.";

            return myString;
        }

        private static List<EmailAttachment> get_images()
        {
            List<EmailAttachment> attachments = new List<EmailAttachment>();

            foreach (KeyValuePair<DateTime, List<MyItem>> line in App.SavedLines[DeploymentName])
            {
                int i = 1;
                foreach (MyItem item in line.Value)
                {
                    if (item.Reciept != null)
                    {
                        attachments.Add(new EmailAttachment(item.Reciept));
                    }

                    i += 1;
                }
            }
            return attachments;
        }

        public static Label BackButton()
        {
            Label thisLabel = new Label
            {
                FontSize = 20,
                FontFamily = "fa.otf#fa",
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = FontAwesomeIcons.FontAwesomeIcons.ArrowAltLeft,
                FontAttributes = FontAttributes.Bold
            };

            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
            };

            thisLabel.GestureRecognizers.Add(tap);

            return thisLabel;

        }
    }
}