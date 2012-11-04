using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Squid.Core;

namespace Squid.App
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ObservableCollection<Download> m_downloads;
		private string downloadFolder = @"C:\tmp\downloader\";

		public MainWindow()
		{
			InitializeComponent();
			m_downloads = new ObservableCollection<Download>();

			var binding = new Binding();
			binding.Source = m_downloads;
			binding.Mode = BindingMode.OneWay;
			Downloads.SetBinding(ItemsControl.ItemsSourceProperty, binding);

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

			var worker = new BackgroundWorker();
			worker.DoWork += OnBackgroundDownloadSourceStart;
			worker.RunWorkerCompleted += OnBackgroundDownloadSourceCompleted;
			worker.RunWorkerAsync(uri);
		}

		private void OnRetrieveClick(object sender, RoutedEventArgs e)
		{
			LoadUri(InputPath.Text);
		}

		private void OnBackgroundDownloadSourceStart(object sender, DoWorkEventArgs e)
		{
			var url = new Uri((string)e.Argument);
			var context = Context.Instance;
			var source = context.FindFactoryByCreatorType(typeof(DownloadSourceCreator)).Create(url);
			e.Result = source;
		}

		private void OnBackgroundDownloadSourceCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dispatcher.BeginInvoke((Action)delegate()
			{
				var downloadSource = (DownloadSource)e.Result;
				DownloadSpecifiers.ItemsSource = downloadSource.Specifiers;
				
				buttonRetrieve.Content = (string)buttonRetrieve.Tag;
				buttonRetrieve.IsEnabled = true;
			});
		}

		private void OnBackgroundDownloadStart(object sender, DoWorkEventArgs e)
		{
			var download = (Download)e.Argument;
			download.Start();
			e.Result = download;
		}

		private void OnBackgroundDownloadCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dispatcher.BeginInvoke((Action)delegate()
			{
				var download = (Download)e.Result;
				lock (m_downloads)
				{
					//m_downloads.Remove(download);
				}
			});
		}

		private void OnDownloadSpecifiersMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var downloadSpecifier = ((ListBox)sender).SelectedItem as DownloadSpecifier;
			if (downloadSpecifier != null)
			{
				string fileName = OutputFilePath.Text;
				string filePath = System.IO.Path.Combine(downloadFolder, fileName);
				var download = new Download(downloadSpecifier, filePath);
				
				lock (m_downloads)
				{
					m_downloads.Add(download);
				}

				var worker = new BackgroundWorker();
				worker.DoWork += OnBackgroundDownloadStart;
				worker.RunWorkerCompleted += OnBackgroundDownloadCompleted;
				worker.RunWorkerAsync(download);
			}
		}

		private void OnDownloadsMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var download = ((DataGrid)sender).SelectedItem as Download;
			if (download != null)
			{
				download.Stop();
			}
		}
	}

}
