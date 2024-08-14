using System;
using System.Collections.Generic;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;
using Shell32;
using Starfall.Models;
using Starfall.Controllers;
using Starfall.UserControls;
using Path = System.IO.Path;
using static Starfall.Controllers.MovieController;
using System.Threading;
using Starfall.Helper;

namespace Starfall.Pages
{
  /// <summary>
  /// Interaction logic for AddMoviesPage.xaml
  /// </summary>
  public partial class AddMoviesPage : Page, ITagsButtonListener
	{
		public List<MovieTag> SelectedTags { get; set; } = new();
		long TagsSum { get; set; }
		MovieModel? EditingMovie { get; set; }
		IRefreshable? EditingParent { get; set; }
		public AddMoviesPage(IRefreshable? editingParent = null,MovieModel? editingMovie = null)
		{
			InitializeComponent();
			if(editingMovie == null)
			{
				foreach (var item in MovieTagController.MovieTags)
				{
					TagButton tb = new(item, SelectedTags, this);
					TagsWrapPanel.Children.Add(tb);
				}
			}
			else
			{
				EditingParent = editingParent;
				EditingMovie = editingMovie;
				MovieNameTextBox.Text = EditingMovie?.Name;
				MovieFilePathTextBox.Text = EditingMovie?.FilePath;
				MovieRatingTextBox.Text = EditingMovie?.Rating.ToString();
				MovieLengthTextBox.Text = EditingMovie?.Length?.ToString();
				MovieQualityComboBox.SelectedIndex = EditingMovie is not null ? (int)EditingMovie.Quality : 0;
				MovieYearTextBox.Text = EditingMovie?.Year.ToString();

				foreach (var item in MovieTagController.MovieTags)
				{
					TagButton tb = new(item, SelectedTags, this);
					if((EditingMovie.Tags & item.Id) != 0)
					{
						tb.SelectTag();
					}
					TagsWrapPanel.Children.Add(tb);
				}
				try
				{
					FirstImage.Source = MovieController.GetMovieThumbnail(EditingMovie.Id, MovieThumbnailType.First);
					FirstImageLabel.Text = MovieController.GetMovieThumbnailPath(EditingMovie.Id, MovieThumbnailType.First);
					SecondImage.Source = MovieController.GetMovieThumbnail(EditingMovie.Id, MovieThumbnailType.Second);
					SecondImageLabel.Text = MovieController.GetMovieThumbnailPath(EditingMovie.Id, MovieThumbnailType.Second);
					ThirdImage.Source = MovieController.GetMovieThumbnail(EditingMovie.Id, MovieThumbnailType.Third);
					ThirdImageLabel.Text = MovieController.GetMovieThumbnailPath(EditingMovie.Id, MovieThumbnailType.Third);
				}
				catch
				{

				}
				AddMovieButtonTextBlock.Text = "SAVE CHANGES";
			}
			NotificationTextBlock.Text = "";
		}
		#region Images
		private void ImagesDragDrop_Drop(object sender, DragEventArgs e)
		{
			ProcessDroppedImages(sender, e, 0);
		}

		private void ProcessDroppedImages(object sender, DragEventArgs e, int startIndex)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				for (int i = 0; i < 3; i++)
				{
					if (i + 1 + startIndex > files.Length)
					{
						return;
					}
					SetThumbnailImage(files[i], i + startIndex);
				}
			}
		}

		private void FirstImage_Drop(object sender, DragEventArgs e)
		{
			SetSingleImage(sender, e, 0);
		}
		private void SecondImage_Drop(object sender, DragEventArgs e)
		{
			SetSingleImage(sender, e, 1);
		}

		private void ThirdImage_Drop(object sender, DragEventArgs e)
		{
			SetSingleImage(sender, e, 2);
		}

		private void SetSingleImage(object sender, DragEventArgs e, int imageIndex)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				SetThumbnailImage(files[0], imageIndex);
			}
		}

		private void SetThumbnailImage(string path, int i)
		{
			// Get basic file information
			FileInfo fileInfo = new(path);
			string fileExtension = fileInfo.Extension;
			List<string> allowedFileExtensions = new() { ".png", ".jpg", ".jpeg", ".bmp" };
			if(!allowedFileExtensions.Contains(fileExtension))
				return;

			BitmapImage? image;
			try
			{
				byte[] allBytes = File.ReadAllBytes(path);
				// Create a BitmapImage from the bytes
				BitmapImage bitmapImage = new BitmapImage();
				using (MemoryStream memoryStream = new MemoryStream(allBytes))
				{
					try
					{
						memoryStream.Position = 0;
						bitmapImage.BeginInit();
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.StreamSource = memoryStream;
						bitmapImage.EndInit();
					}
					catch
					{

					}
				}
				image = bitmapImage;
			}
			catch
			{
				image = null;
			}
			switch (i)
			{
				case 0:
					FirstImage.Source = image;
					FirstImageLabel.Text = path;
					break;
				case 1:
					SecondImage.Source = image;
					SecondImageLabel.Text = path;
					break;
				case 2:
					ThirdImage.Source = image;
					ThirdImageLabel.Text = path;
					break;
				default:
					break;
			}
		}
		#endregion
		#region Video

		string VideoHeight { get; set; }
		VideoQuality VideoQuality { get; set; }
		private void MovieDragDrop_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				foreach (string file in files)
				{
					FileInfo fileInfo = new FileInfo(file);

					// Get basic file information
					string fileName = System.IO.Path.GetFileNameWithoutExtension(fileInfo.FullName);
					string fileExtension = fileInfo.Extension;

					long fileSize = fileInfo.Length; // File size in bytes

					// Create Shell32 instance
					Shell shell = new Shell();
					Folder folder = shell.NameSpace(System.IO.Path.GetDirectoryName(file));
					FolderItem folderItem = folder.ParseName(System.IO.Path.GetFileName(file));

					// Get video duration
					string duration = folder.GetDetailsOf(folderItem, 27); // 27 is the index for duration
					VideoHeight = folder.GetDetailsOf(folderItem, 314); // 27 is the index for duration
					DateTime creationTime = fileInfo.LastWriteTime;
					string relativePath = (System.IO.Path.GetRelativePath(System.AppContext.BaseDirectory, fileInfo.FullName));

					//StringBuilder stringBuilder = new StringBuilder();
					//for (int i = 0; i < short.MaxValue; i++)
					//{
					//	string property = folder.GetDetailsOf(null, i);
					//	if (!string.IsNullOrEmpty(property))
					//	{
					//		stringBuilder.AppendLine($"Index: {i} - Property: {property}");
					//		Console.WriteLine($"Index: {i} - Property: {property}");
					//	}
					//}
					//duration = stringBuilder.ToString();

					MovieNameTextBox.Text = fileName;
					MovieLengthTextBox.Text = duration;
					MovieYearTextBox.Text = creationTime.Year.ToString();
					MovieFilePathTextBox.Text = relativePath;
					VideoQuality = MovieController.GetVideoQuality(VideoHeight);
					MovieQualityComboBox.SelectedIndex = (int)VideoQuality;
				}
			}
		}
		#endregion

		private void AddMovieButton_Click(object sender, RoutedEventArgs e)
		{
			MovieModel movie = new();
			movie.Name = MovieNameTextBox.Text;
			movie.FilePath = MovieFilePathTextBox.Text;
			try {
				movie.Rating = int.Parse(MovieRatingTextBox.Text);
			}
			catch{
				movie.Rating = 0;
			}
			movie.Length = MovieLengthTextBox.Text;
			try
			{
				movie.Year = int.Parse(MovieYearTextBox.Text);
			}
			catch
			{
				movie.Year = 0;
			}
			movie.Quality = (VideoQuality)MovieQualityComboBox.SelectedIndex;
			movie.Tags = TagsSum;


			string movieDirectory = "";
			if (EditingMovie is null)
			{
				int movieId = MovieController.AddMovie(movie);
				movieDirectory = Path.Combine(System.AppContext.BaseDirectory, MovieController.ThumnailsDirectory, movieId.ToString());
				Directory.CreateDirectory(movieDirectory);
			}
			else
			{
				FirstImage.Source = null;
				SecondImage.Source = null;
				ThirdImage.Source = null;
				movieDirectory = Path.Combine(System.AppContext.BaseDirectory, MovieController.ThumnailsDirectory, EditingMovie.Id.ToString());
				movie.Id = EditingMovie.Id;
				MovieController.ReplaceMovie(movie);
			}
			string firstImagePath = Path.Combine(movieDirectory, $"0{Path.GetExtension(FirstImageLabel.Text)}");
			if (!string.IsNullOrEmpty(FirstImageLabel.Text) && firstImagePath != FirstImageLabel.Text)
			{
				byte[] allBytes = File.ReadAllBytes(FirstImageLabel.Text);
				File.WriteAllBytes(firstImagePath, allBytes);
				//File.Copy(FirstImageLabel.Text, firstImagePath, true);
			}
			else
			{
				string image = Path.Combine(movieDirectory, $"0.png");
				string timeStamp = ScreenShotMaker.GetModifiedTimestamp(movie.Length,
					ScreenShotMaker.TimestampOptions.PlusMinuteFromStart);
				ScreenShotMaker.TakeScreenShot(movie.FilePath, image, timeStamp);
			}
			string secondImagePath = Path.Combine(movieDirectory, $"1{Path.GetExtension(SecondImageLabel.Text)}");
			if (!string.IsNullOrEmpty(SecondImageLabel.Text) && secondImagePath != SecondImageLabel.Text)
			{
				byte[] allBytes = File.ReadAllBytes(SecondImageLabel.Text);
				File.WriteAllBytes(secondImagePath, allBytes);
				//File.Copy(SecondImageLabel.Text, secondImagePath, true);
			}
			else
			{
				string image = Path.Combine(movieDirectory, $"1.png");
				string timeStamp = ScreenShotMaker.GetModifiedTimestamp(movie.Length,
					ScreenShotMaker.TimestampOptions.Middle);
				ScreenShotMaker.TakeScreenShot(movie.FilePath, image, timeStamp);
			}
			string thirdImagePath = Path.Combine(movieDirectory, $"2{Path.GetExtension(ThirdImageLabel.Text)}");
			if (!string.IsNullOrEmpty(ThirdImageLabel.Text) && thirdImagePath != ThirdImageLabel.Text)
			{
				//using FileStream fileStream = File.Open(ThirdImageLabel.Text, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

				byte[] allBytes = File.ReadAllBytes(ThirdImageLabel.Text);
				File.WriteAllBytes(thirdImagePath, allBytes);
				//File.Create(thirdImagePath);
				//File.Copy(ThirdImageLabel.Text, thirdImagePath, true);
			}
			else
			{
				string image = Path.Combine(movieDirectory, $"2.png");
				string timeStamp = ScreenShotMaker.GetModifiedTimestamp(movie.Length,
					ScreenShotMaker.TimestampOptions.MinusMinuteFromEnd);
				ScreenShotMaker.TakeScreenShot(movie.FilePath, image, timeStamp);
			}
			if (EditingMovie is not null)
			{
				EditingParent.RefreshImages(movie);
				EditingParent.OnRefresh();
			}
			else
			{
				NotificationTextBlock.Text = $"MOVIE ADDED : {movie.Name}";
			}
		}

		public void OnTagAdded(MovieTag tag)
		{
			TagsSum |= tag.Id;
			TagsSumTextBlock.Text = TagsSum.ToString();
		}
		public void OnTagRemoved(MovieTag tag)
		{
			TagsSum ^= tag.Id;
			TagsSumTextBlock.Text = TagsSum.ToString();
		}
	}
}
