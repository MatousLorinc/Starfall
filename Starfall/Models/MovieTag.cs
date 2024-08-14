using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starfall.Models
{
	public class MovieTag
	{
		public long Id { get; set; }
		public string Name { get; set; }
	}
	public interface ITagsButtonListener
	{
		public void OnTagAdded(MovieTag tag);
		public void OnTagRemoved(MovieTag tag);
	}
}
