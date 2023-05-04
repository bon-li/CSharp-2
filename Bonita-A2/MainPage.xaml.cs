using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bonita_A2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<string> locations = new List<string>();
        List<string> genres = new List<string>();
        ObservableCollection<String> movies = new ObservableCollection<String>();
        ObservableCollection<Movie> moviesObject = new ObservableCollection<Movie>();
        ObservableCollection<Movie> moviesFiltered = new ObservableCollection<Movie>();
        string selectedMovieName = "";
        public MainPage()
        {
            this.InitializeComponent();

            locations.Add("Canada");
            locations.Add("International");

            genres.Add("Action");
            genres.Add("Biography");
            genres.Add("Comedy");
            genres.Add("Drama");
            genres.Add("Educational");
            genres.Add("History");
            genres.Add("Horror");
            genres.Add("Musical");
            genres.Add("Mystery");
            genres.Add("Romance");
            genres.Add("Sci-Fi");
            genres.Add("Thriller");
            genres.Add("Western");

            ratingcb.Items.Add("1");
            ratingcb.Items.Add("2");
            ratingcb.Items.Add("3");
            ratingcb.Items.Add("4");
            ratingcb.Items.Add("5");

            locationSelection.ItemsSource = locations; 
            genreSelection.ItemsSource = genres;
            ObservableCollection<Movie> moviesFiltered = new ObservableCollection<Movie>(moviesObject);
        }

        //Saves movie to list
        private async void saveMovie(object sender, RoutedEventArgs e)
        {
            int rating;
            int duration;
            double price;

            //checks for empty fields
            if (locationSelection.SelectedItem == null || durationtb.Text == "" ||
                 pricetb.Text == "" || genreSelection.SelectedItem == null ||
                 ratingcb.SelectedItem == null || movieNametb.Text == "")
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = "Unable to add movie",
                    CloseButtonText = "OK"

                };

                await contentDialog.ShowAsync();
            }
            else
            {
                Console.WriteLine(ratingcb.SelectedItem.ToString() + " " + durationtb.Text);

                rating = Convert.ToInt32(ratingcb.SelectedItem.ToString());
                duration = Convert.ToInt32(durationtb.Text);
                price = Convert.ToDouble(pricetb.Text);

                Movie movie = new Movie(movieNametb.Text, releaseDatePicker.Date.Value.DateTime, locationSelection.SelectedItem.ToString(),
                        genreSelection.SelectedItem.ToString(), rating,
                        duration, price);

                movies.Add(movie.ToString());
                moviesObject.Add(movie);
                moviesFiltered.Add(movie);
                moviesList.ItemsSource = moviesFiltered;

                movieNametb.Text = "";
                releaseDatePicker.Date = null;
                locationSelection.SelectedIndex = -1;
                genreSelection.SelectedIndex = -1;
                ratingcb.SelectedIndex = -1;
                durationtb.Text = "";
                pricetb.Text = "";
            }

        }

        //Searches for a movie by name
        private void search(object sender, RoutedEventArgs e)
        {

            List<Movie> TempFiltered;
            //filter temp list
            TempFiltered = moviesObject.Where(movie => movie.movieName.Contains(searchMovie.Text.ToString(),
                StringComparison.InvariantCultureIgnoreCase)).ToList();
            //remove any movie in movies filtered that is absent in temp
            for (int i = moviesFiltered.Count - 1; i >= 0; i--)
            {
                var item = moviesFiltered[i];
                if (!TempFiltered.Contains(item))
                {
                    moviesFiltered.Remove(item);
                }
            }

            //add back any movie that are in temp but not in movies filtered
            foreach (var item in TempFiltered)
            {
                if (!moviesFiltered.Contains(item))
                {
                    moviesFiltered.Add(item);
                }
            }
        }

        //Deletes movie from list
        private async void deleteMovie(object sender, RoutedEventArgs e)
        {
            //Checks for unselected item
            if (moviesList.SelectedIndex == -1)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = "No item selected",
                    CloseButtonText = "OK"
                };

                await contentDialog.ShowAsync();
            }
            else
            {
                //Deletes selected movie
                var item = moviesFiltered[moviesList.SelectedIndex];
                moviesFiltered.Remove(item);
                moviesObject.Remove(item);

            }

        }

        //Listview generation
        private void moviesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (moviesList.SelectedItem == null)
            {
                return;
            }

            String selectedItem = moviesList.SelectedItem.ToString();
            String[] movies = selectedItem.Split(", ");

            selectedMovieName = movies[0];

            //Prints selected movies into textboxes
            movieNametb.Text = movies[0];
            releaseDatePicker.Date = DateTime.Parse(movies[1]);
            locationSelection.SelectedIndex = locationSelection.Items.IndexOf(movies[2]);
            genreSelection.SelectedIndex = genreSelection.Items.IndexOf(movies[3]);
            ratingcb.SelectedIndex = ratingcb.Items.IndexOf(movies[4]);
            durationtb.Text = movies[5];
            pricetb.Text = movies[6];

        }

        //Saves listview into a file
        public async void saveData(object sender, RoutedEventArgs e)
        {
            if (moviesObject.Count == 0)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = "List is empty",
                    CloseButtonText = "OK"
                };

                await contentDialog.ShowAsync();
                return;
            }

            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();

            var s = await file.OpenStreamForWriteAsync();
            IFormatter f = new BinaryFormatter();
            f.Serialize(s, moviesObject);

            ContentDialog contentDialog2 = new ContentDialog()
            {
                Title = "Success",
                Content = "Movies have been saved to file",
                CloseButtonText = "OK"
            };
        }

        //Retrieves movie list from file
        private async void getData(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");
            var file = await picker.PickSingleFileAsync();
            var s = await file.OpenStreamForReadAsync();
            IFormatter f = new BinaryFormatter();
            moviesObject = (ObservableCollection<Movie>)f.Deserialize(s);
            moviesFiltered = moviesObject;
            moviesList.ItemsSource = moviesFiltered;

            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "Success",
                Content = "File has been opened",
                CloseButtonText = "OK"
            };

            await contentDialog.ShowAsync();
        }

        //Duration textbx only takes numbers
        private void durationtb_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }


        //Updates selected movie
        private async void updateItem(object sender, RoutedEventArgs e)
        {
            int rating;
            int duration;
            double price;

            //checks for selected movie
            if (moviesList.SelectedIndex == -1)
            {
                ContentDialog contentDialog = new ContentDialog()
                {
                    Title = "Error",
                    Content = "No movie selected",
                    CloseButtonText = "OK"

                };

                await contentDialog.ShowAsync();
                return;
            }
            else
            {
                //checks for empty fields
                if (locationSelection.SelectedItem == null || durationtb.Text == "" ||
                 pricetb.Text == "" || genreSelection.SelectedItem == null ||
                 ratingcb.SelectedItem == null || movieNametb.Text == "")
                {
                    ContentDialog contentDialog = new ContentDialog()
                    {
                        Title = "Error",
                        Content = "Unable to add movie",
                        CloseButtonText = "OK"

                    };

                    await contentDialog.ShowAsync();
                }
                try
                {
                    rating = Convert.ToInt32(ratingcb.SelectedItem.ToString());
                    duration = Convert.ToInt32(durationtb.Text);
                    price = Convert.ToDouble(pricetb.Text);
                }
                catch (FormatException)
                {
                    ContentDialog contentDialog = new ContentDialog()
                    {
                        Title = "Error",
                        Content = "Unable to add movie",
                        CloseButtonText = "OK"

                    };

                    await contentDialog.ShowAsync();
                    return;
                }

                //Creates new movie object
                Movie movie = new Movie(movieNametb.Text, releaseDatePicker.Date.Value.DateTime, locationSelection.SelectedItem.ToString(),
                        genreSelection.SelectedItem.ToString(), rating,
                        duration, price);
                var item = moviesFiltered[moviesList.SelectedIndex];


                //Searches for selected movie name and saves new movie object in same place
                for (int i = moviesFiltered.Count - 1; i >= 0; i--)
                {
                    if (moviesFiltered[i].movieName.Equals(selectedMovieName))
                    {
                        moviesFiltered[i] = movie;
                    }
                }
                for (int i = moviesObject.Count - 1; i >= 0; i--)
                {
                    if (moviesObject[i].movieName.Equals(selectedMovieName))
                    {
                        moviesObject[i] = movie;
                    }
                }

            }
        }


    }
}
