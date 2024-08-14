using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Starfall.Controllers;
using Starfall.Models;
using Starfall.UserControls;
using System.Globalization;
using static Starfall.Controllers.MovieController;

namespace Starfall.Pages
{
	/// <summary>
	/// Interaction logic for Database.xaml
	/// </summary>
	public partial class DatabasePage : Page, ITagsButtonListener, IRefreshable
	{
		public static CultureInfo CultureInfo { get; set; } = new CultureInfo("cs-CZ", false);
		public List<MovieTag> SelectedTags { get; set; } = new();
		Window EditMovieWindow { get; set; }
		string SearchExpression { get; set; } = "";
		long TagsSum { get; set; }
		public DatabasePage()
		{
			InitializeComponent();
			MoviesGrid.ItemsSource = MovieController.Movies;
			Random random = new Random();
			if(MovieController.Movies.Any())
				MoviesGrid.SelectedIndex = random.Next(0, MovieController.Movies.Count - 1);

			foreach (var item in MovieTagController.MovieTags)
			{
				TagButton tb = new(item, SelectedTags, this);
				TagsWrapPanel.Children.Add(tb);
			}
		}
		private void ShowMovieData(MovieModel movie)
		{
			FirstImage.Source = MovieController.GetMovieThumbnail(movie.Id, MovieThumbnailType.First);
			SecondImage.Source = MovieController.GetMovieThumbnail(movie.Id, MovieThumbnailType.Second);
			ThirdImage.Source = MovieController.GetMovieThumbnail(movie.Id, MovieThumbnailType.Third);
			MovieNameTextBlock.Text = movie.Name;
			MovieRatingTextBlock.Text = $"Rating : {movie.Rating}";
			MovieLengthTextBlock.Text = $"Length : {movie.Length}";
			MovieYearTextBlock.Text = $"Year : {movie.Year}";
			MovieQualityTextBlock.Text = $"Quality : {movie.Quality}";
		}
		private void MoviesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				MovieModel selectedMovie = e.AddedItems[0] as MovieModel;
				ShowMovieData(selectedMovie);
				// show movie tags
				MovieTagsWrapPanel.Children.Clear();
				foreach (var tag in MovieTagController.MovieTags)
				{
					if((selectedMovie.Tags & tag.Id) != 0)
					{
						TagButton tb = new(tag, null, this);
						tb.SelectTag();
						MovieTagsWrapPanel.Children.Add(tb);
					}
				}
			}
		}

		private void PlayMovieButton_Click(object sender, RoutedEventArgs e)
		{
			MovieModel movie = (MovieModel)MoviesGrid.SelectedItem;
			MovieController.OpenVideo(movie);
		}

		private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
		{
			MovieController.SaveMoviesData();
		}

		private void EditMovieButton_Click(object sender, RoutedEventArgs e)
		{
			// UnlinkImages();
			// Create a new window
			EditMovieWindow = new Window();
			// Set the background color to #FF1A1A1A
			System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF1A1A1A");
			System.Windows.Media.SolidColorBrush brush = new System.Windows.Media.SolidColorBrush(color);
			EditMovieWindow.Background = brush;
			EditMovieWindow.Title = "Edit Movie";

			// Instantiate your NewPage or UserControl (Replace NewPage() with your actual new page instantiation logic)
			AddMoviesPage newPage = new AddMoviesPage(this,MoviesGrid.SelectedItem as MovieModel);

			// Set the Content of the new window to your NewPage
			EditMovieWindow.Content = newPage;

			// Show the new window
			EditMovieWindow.Show();
		}
		public void OnTagAdded(MovieTag tag)
		{
			TagsSum |= tag.Id;
			ApplyFiltering();
		}
		public void OnTagRemoved(MovieTag tag)
		{
			TagsSum ^= tag.Id;
			ApplyFiltering();
		}
		public bool CheckTagFlag(long tags, long desiredFlag)
		{
			if(TagsSum > 0)
				//return (tags & desiredFlag) != 0;
				return (tags & desiredFlag) == desiredFlag;
			else return true;
		}
		public void ApplyFiltering()
		{
			MoviesGrid.ItemsSource = MovieController.Movies.FindAll(x => ContainsSubstring(x.Name, SearchExpression) && CheckTagFlag(x.Tags, TagsSum));
		}
		private static bool ContainsSubstring(string source, string keyword)
		{
			return CultureInfo.CompareInfo.IndexOf(source, keyword, CompareOptions.IgnoreCase) >= 0;
		}

		private void SeachBar_TextChanged(object sender, TextChangedEventArgs e)
		{
			SearchExpression = SeachBar.Text;
			ApplyFiltering();
		}

		public void OnRefresh()
		{
			MoviesGrid.ItemsSource = null; // Clear the current ItemsSource
			ApplyFiltering();
			//MoviesGrid.ItemsSource = MovieController.Movies;
			EditMovieWindow.Close();
		}
		public void UnlinkImages()
		{
			FirstImage.Source = null;
			SecondImage.Source = null;
			ThirdImage.Source = null;
		}
		public void RefreshImages(MovieModel movie)
		{
			if(MoviesGrid.Items.Count > 0)
			{
				FirstImage.Source = MovieController.GetMovieThumbnail(movie.Id, MovieThumbnailType.First);
				SecondImage.Source = MovieController.GetMovieThumbnail(movie.Id, MovieThumbnailType.Second);
				ThirdImage.Source = MovieController.GetMovieThumbnail(movie.Id, MovieThumbnailType.Third);
			}
		}
	}
}
