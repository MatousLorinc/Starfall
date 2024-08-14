using Newtonsoft.Json;
using Starfall.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Starfall.Controllers
{
	public static class MovieTagController
	{
		private const string DatabaseFile = "MovieTags.json";

		public static List<MovieTag> MovieTags { get; set; } = new();
		public static bool LoadMoviesTags()
		{
			try
			{
				if (!File.Exists(Database.GetFullTablePath(DatabaseFile)))
				{
					File.Create(Database.GetFullTablePath(DatabaseFile));
				}
				MovieTags = JsonConvert.DeserializeObject<List<MovieTag>>(File.ReadAllText(Database.GetFullTablePath(DatabaseFile)));
				MovieTags ??= new();
				return true;
			}
			catch
			{
				Log.Write(Log.LogMsgType.Chyba, "Error : Failed to deserialize database");
				MovieTags = new();
				return false;
			}
		}
		public static void SaveMoviesTags()
		{
			Database.SaveTable(MovieTagController.MovieTags, DatabaseFile);
		}
		public static long GetNextId()
		{
			// iterate power of 2
			for (long  i = 1; i < (long.MaxValue / 2) - 1; i *= 2)
			{
				if(!MovieTags.Any(x=>x.Id == i))
				{
					return i;
				}
			}
			return 1;
		}
	}
}
