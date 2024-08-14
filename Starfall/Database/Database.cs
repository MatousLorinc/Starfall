using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;

namespace Starfall
{
	class Database
	{
		private const string DatabaseDirectory = "Database";
		/// <summary>
		/// Gets fullpath to user setting file
		/// </summary>
		public static string GetFullTablePath(string filename)
    {
			if(!Directory.Exists(DatabaseDirectory))
				Directory.CreateDirectory(DatabaseDirectory);

      return Path.Combine(System.AppContext.BaseDirectory, DatabaseDirectory, filename);
		}

		/// <summary>
		/// using Newtonsoft creates and saves json string to file
		/// </summary>
		/// <returns>Serialized Config class into json</returns>
		public static string SaveTable(object table, string filename)
    {
      string json = JsonConvert.SerializeObject(table, Formatting.Indented);
      File.WriteAllText(GetFullTablePath(filename), json);
      return json;
    }
	}
}
