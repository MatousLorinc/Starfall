using Starfall.Controllers;
using Starfall.Models;
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

namespace Starfall.Pages
{
	/// <summary>
	/// Interaction logic for TagsPage.xaml
	/// </summary>
	public partial class TagsPage : Page
	{
		public TagsPage()
		{
			InitializeComponent();
			TagsGrid.ItemsSource = MovieTagController.MovieTags;
		}

		private void TagsGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
		{
			MovieTag editedItem = e.Row.Item as MovieTag;
			// when using search bar we must add item to orginal collection
			if (!MovieTagController.MovieTags.Contains(editedItem))
			{
				MovieTagController.MovieTags.Add(editedItem);
			}
		}

		private void TagsGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
		{
			MovieTag newItem = (MovieTag)e.NewItem;
			newItem.Id = MovieTagController.GetNextId();
			//MovieTagController.AddTag(newItem);
		}

		private void TagsGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
		{
			//MovieTag newItem = (MovieTag)e.NewItem;
			//MovieTagController.AddTag(newItem);
		}

		private void SaveTagsButton_Click(object sender, RoutedEventArgs e)
		{
			MovieTagController.SaveMoviesTags();
		}
	}
}
