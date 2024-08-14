using Starfall.Models;
using Starfall.Pages;
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

namespace Starfall.UserControls
{
	/// <summary>
	/// Interaction logic for TagButton.xaml
	/// </summary>
	public partial class TagButton : UserControl
	{
		ITagsButtonListener ParentListener { get; set; }
		MovieTag MovieTag { get; set; }
		List<MovieTag> ParentTags { get; set; } = new();
		bool TagIsSelected { get; set; } = false;
		public TagButton(MovieTag movieTag, List<MovieTag> parentTags, ITagsButtonListener parent)
		{
			InitializeComponent();
			MovieTag = movieTag;
			ParentTags = parentTags;
			ParentListener = parent;
			TagButtonTextBlock.Text = MovieTag.Name;
		}
		private void TagButton_Click(object sender, RoutedEventArgs e)
		{
			if (ParentTags is not null)
			{
				if (ParentTags.Contains(MovieTag))
					UnselectTag();
				else
					SelectTag();
			}
		}
		public void SelectTag()
		{
			if(ParentTags is not null)
			{
				ParentTags.Add(MovieTag);
				ParentListener.OnTagAdded(MovieTag);
			}
			TagIsSelected = true;
			TagButtonElement.Style = (Style)FindResource("SelectedTagButtonStyle");
		}
		public void UnselectTag()
		{
			if (ParentTags is not null)
			{
				ParentTags.Remove(MovieTag);
				ParentListener.OnTagRemoved(MovieTag);
			}
			TagIsSelected = false;
			TagButtonElement.Style = (Style)FindResource("MenuButton");
		}
	}
}
