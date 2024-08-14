using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starfall.Models
{
	public class MovieModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string FilePath { get; set; }
		public int Rating { get; set; }
		public string Length { get; set; }
		public int Year { get; set; }
		public VideoQuality Quality { get; set; }
		public long Tags { get; set; }
	}
	public enum VideoQuality
	{
		low,
		hd,
		fullHd,
		k2,
		k4,
		k8
	}
}
