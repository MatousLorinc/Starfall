using Starfall.Controllers;
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

namespace Starfall
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			MovieTagController.LoadMoviesTags(); // load tags
			MovieController.LoadMoviesData(); // load movies
			MainFrame.Content = new DatabasePage();
			this.Title = "Starfall";
		}

		private void DatabaseButton_Click(object sender, RoutedEventArgs e)
		{
			MainFrame.Content = new DatabasePage();
		}

		private void AddVideoButton_Click(object sender, RoutedEventArgs e)
		{
			MainFrame.Content = new AddMoviesPage();
		}

		private void TagsButton_Click(object sender, RoutedEventArgs e)
		{
			MainFrame.Content = new TagsPage();
		}

		private void SettingButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
