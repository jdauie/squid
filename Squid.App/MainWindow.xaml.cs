using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Markup;

using Squid.Core;

namespace Squid.App
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ObservableCollection<Download> m_downloads;
		private string downloadFolder = @"C:\tmp\downloader\";

		public MainWindow()
		{
			InitializeComponent();
			m_downloads = new ObservableCollection<Download>();

			Binding binding = new Binding();
			binding.Source = m_downloads;
			binding.Mode = BindingMode.OneWay;
			Downloads.SetBinding(DataGrid.ItemsSourceProperty, binding);

			//textBox1.Text = "http://day9tv.blip.tv/file/4503324/";
			//InputPath.Text = "http://www.youtube.com/watch?v=FuMSGkhmbFs";
		}

		public void LoadUri(string uri)
		{
			InputPath.Text = uri;

			buttonRetrieve.Tag = buttonRetrieve.Content;
			buttonRetrieve.Content = "Working...";
			buttonRetrieve.IsEnabled = false;

			DownloadSpecifiers.ItemsSource = null;

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler(BackgroundDownloadSourceStart);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundDownloadSourceCompleted);
			worker.RunWorkerAsync(uri);
		}

		private void buttonRetrieve_Click(object sender, RoutedEventArgs e)
		{
			LoadUri(InputPath.Text);
		}



		private void BackgroundDownloadSourceStart(object sender, DoWorkEventArgs e)
		{
			Uri url = new Uri((string)e.Argument);
			Context context = Context.Instance;
			ISource source = context.FindFactoryByCreatorType(typeof(DownloadSourceCreator)).Create(url);
			e.Result = source;
		}

		private void BackgroundDownloadSourceCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Dispatcher.BeginInvoke((Action)delegate()
			{
				DownloadSource downloadSource = (DownloadSource)e.Result;
				DownloadSpecifiers.ItemsSource = downloadSource.Specifiers;
				
				buttonRetrieve.Content = (string)buttonRetrieve.Tag;
				buttonRetrieve.IsEnabled = true;
			});
		}

		private void BackgroundDownloadStart(object sender, DoWorkEventArgs e)
		{
			Download download = (Download)e.Argument;
			download.Start();
			e.Result = download;
		}

		private void BackgroundDownloadCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Dispatcher.BeginInvoke((Action)delegate()
			{
				Download download = (Download)e.Result;
				lock (m_downloads)
				{
					//m_downloads.Remove(download);
				}
			});
		}

		private void DownloadSpecifiers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DownloadSpecifier downloadSpecifier = ((ListBox)sender).SelectedItem as DownloadSpecifier;
			if (downloadSpecifier != null)
			{
				string fileName = OutputFilePath.Text;
				string filePath = System.IO.Path.Combine(downloadFolder, fileName);
				Download download = new Download(downloadSpecifier, filePath);
				
				lock (m_downloads)
				{
					m_downloads.Add(download);
				}

				BackgroundWorker worker = new BackgroundWorker();
				worker.DoWork += new DoWorkEventHandler(BackgroundDownloadStart);
				worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundDownloadCompleted);
				worker.RunWorkerAsync(download);
			}
		}

		private void Downloads_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			Download download = ((DataGrid)sender).SelectedItem as Download;
			if (download != null)
			{
				download.Stop();
			}
		}
	}

}
