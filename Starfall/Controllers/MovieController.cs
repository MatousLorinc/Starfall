using Newtonsoft.Json;
using Starfall.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Starfall.Controllers
{
	public enum MovieThumbnailType
	{
		First = 0,
		Second = 1,
		Third = 2
	}
	public static class MovieController
	{
		public interface IRefreshable
		{
			public void OnRefresh();
			public void UnlinkImages();
			public void RefreshImages(MovieModel movie);
		}
		public static List<MovieModel> Movies { get; set; } = new();

		public const string DatabaseFile = "Database.json";
		public const string ThumnailsDirectory = "Thumbnails";
		private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".bmp" };

		public static bool LoadMoviesData()
		{
			try
			{
				if (!File.Exists(Database.GetFullTablePath(DatabaseFile)))
				{
					File.Create(Database.GetFullTablePath(DatabaseFile));
				}
				Movies = JsonConvert.DeserializeObject<List<MovieModel>>(File.ReadAllText(Database.GetFullTablePath(DatabaseFile)));
				Movies ??= new();
				return true;

			}
			catch
			{
				Log.Write(Log.LogMsgType.Chyba, "Error : Failed to deserialize database");
				return false;
			}
		}
		public static void SaveMoviesData()
		{
			Database.SaveTable(Movies, DatabaseFile);
		}

		public static int AddMovie(MovieModel movie) {
			int largestId = Movies.Count > 0 ? Movies.Max(movie => movie.Id) : -1;
			movie.Id = ++largestId;
			Movies.Add(movie);
			SaveMoviesData();
			return largestId;
		}

		public static bool ReplaceMovie(MovieModel movie)
		{
			// Find the index of the movie with the given ID
			int index = Movies.FindIndex(x => x.Id == movie.Id);
			// Check if the movie exists in the list
			if (index == -1)
			{
				// Movie not found
				return false;
			}
			Movies[index] = movie;
			SaveMoviesData();
			return true;
		}

		public static void OpenVideo(MovieModel movie)
		{
			Process.Start("explorer.exe", movie.FilePath);
		}

		public static BitmapImage? GetMovieThumbnail(int movieId, MovieThumbnailType movieThumbnailType)
		{
			string thumbnailPath = Path.Combine(System.AppContext.BaseDirectory, ThumnailsDirectory, movieId.ToString(), ((int)movieThumbnailType).ToString());
			foreach(string extension in ImageExtensions)
			{
				string finalPath = thumbnailPath + extension;
				if (Path.Exists(finalPath))
				{
					byte[] allBytes = File.ReadAllBytes(finalPath);

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
						catch (Exception ex)
						{
							// Handle any exceptions that occur during BitmapImage creation
							Console.WriteLine($"Error creating BitmapImage: {ex.Message}");
							return null; // Return null or handle the error accordingly
						}
					}
					return bitmapImage;
					//using FileStream fileStream = File.Open(finalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					//return new BitmapImage(new Uri(finalPath));
				}
			}
			return null;
		}

		public static string? GetMovieThumbnailPath(int movieId, MovieThumbnailType movieThumbnailType)
		{
			string thumbnailPath = Path.Combine(System.AppContext.BaseDirectory, ThumnailsDirectory, movieId.ToString(), ((int)movieThumbnailType).ToString());
			foreach (string extension in ImageExtensions)
			{
				string finalPath = thumbnailPath + extension;
				if (Path.Exists(finalPath))
				{
					return finalPath;
				}
			}
			return null;
		}

		public static VideoQuality GetVideoQuality(string videoHeight)
		{
			if(int.TryParse(videoHeight, out var videoHeightNum))
			{
				if (videoHeightNum < 720)
					return VideoQuality.low;
				if (videoHeightNum < 1080)
					return VideoQuality.hd;
				if (videoHeightNum < 1440)
					return VideoQuality.fullHd;
				if (videoHeightNum < 2160)
					return VideoQuality.k2;
				else if (videoHeightNum < 4320)
					return VideoQuality.k4;
				else if (videoHeightNum >= 4320)
					return VideoQuality.k8;
			}
			return VideoQuality.low;
		}
	}


}
