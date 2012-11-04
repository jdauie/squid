using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;

using Squid.Core;

namespace Squid.App
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class ChannelWindow : Window
	{
		public ChannelWindow()
		{
			InitializeComponent();

			InputPath.Text = "http://www.youtube.com/user/psystarcraft";
		}

		private void OnRetrieveClick(object sender, RoutedEventArgs e)
		{
			string url = InputPath.Text;
			buttonRetrieve.Tag = buttonRetrieve.Content;
			buttonRetrieve.Content = "Working...";
			buttonRetrieve.IsEnabled = false;

			DownloadSources.ItemsSource = null;

			var worker = new BackgroundWorker();
			worker.DoWork += BackgroundDownloadSourceStart;
			worker.RunWorkerCompleted += BackgroundDownloadSourceCompleted;
			worker.RunWorkerAsync(url);
		}

		private void BackgroundDownloadSourceStart(object sender, DoWorkEventArgs e)
		{
			var url = new Uri((string)e.Argument);
			var context = Context.Instance;
			var source = context.FindFactoryByCreatorType(typeof(ChannelSourceCreator)).Create(url);
			e.Result = source;
		}

		private void BackgroundDownloadSourceCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Dispatcher.BeginInvoke((Action)delegate()
			{
				var channelSource = (ChannelSource)e.Result;
				var channelCollection = new ObservableCollection<ChannelSource>(new ChannelSource[] { channelSource });

				ChannelInfo.ItemsSource = channelCollection;
				DownloadSources.ItemsSource = channelSource.DownloadSources;

				buttonRetrieve.Content = (string)buttonRetrieve.Tag;
				buttonRetrieve.IsEnabled = true;
			});
		}

		private void OnDownloadSourcesMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var lightweightDownloadSource = DownloadSources.SelectedItem as DownloadSource;
			if (lightweightDownloadSource != null)
			{
				var window = new MainWindow();
				window.LoadUri(lightweightDownloadSource.Uri.AbsoluteUri);
				window.ShowDialog();
			}
		}

		private void OnChannelInfoMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var source = ChannelInfo.SelectedItem as ChannelSource;
			if (source != null)
			{
				var pagedSourceCreator = source.Creator as IPagedChannelSourceCreator;
				if (pagedSourceCreator != null)
				{
					var downloadSources = pagedSourceCreator.GetPage(source, 1);
				}
			}
		}
	}

}
