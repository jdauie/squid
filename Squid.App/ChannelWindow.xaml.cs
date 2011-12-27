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
	public partial class ChannelWindow : Window
	{
		public ChannelWindow()
		{
			InitializeComponent();

			InputPath.Text = "http://www.youtube.com/user/psystarcraft";
		}

		private void buttonRetrieve_Click(object sender, RoutedEventArgs e)
		{
			string url = InputPath.Text;
			buttonRetrieve.Tag = buttonRetrieve.Content;
			buttonRetrieve.Content = "Working...";
			buttonRetrieve.IsEnabled = false;

			DownloadSources.ItemsSource = null;

			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += new DoWorkEventHandler(BackgroundDownloadSourceStart);
			worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundDownloadSourceCompleted);
			worker.RunWorkerAsync(url);
		}

		private void BackgroundDownloadSourceStart(object sender, DoWorkEventArgs e)
		{
			Uri url = new Uri((string)e.Argument);
			Context context = Context.Instance;
			ISource source = context.FindFactoryByCreatorType(typeof(ChannelSourceCreator)).Create(url);
			e.Result = source;
		}

		private void BackgroundDownloadSourceCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Dispatcher.BeginInvoke((Action)delegate()
			{
				ChannelSource channelSource = (ChannelSource)e.Result;
				var channelCollection = new ObservableCollection<ChannelSource>(new ChannelSource[] { channelSource });

				ChannelInfo.ItemsSource = channelCollection;
				DownloadSources.ItemsSource = channelSource.DownloadSources;

				buttonRetrieve.Content = (string)buttonRetrieve.Tag;
				buttonRetrieve.IsEnabled = true;
			});
		}

		private void DownloadSources_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DownloadSource lightweightDownloadSource = DownloadSources.SelectedItem as DownloadSource;
			if (lightweightDownloadSource != null)
			{
				MainWindow window = new MainWindow();
				window.LoadUri(lightweightDownloadSource.Uri.AbsoluteUri);
				window.ShowDialog();
			}
		}

		private void ChannelInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			ChannelSource source = ChannelInfo.SelectedItem as ChannelSource;
			if (source != null)
			{
				IPagedChannelSourceCreator pagedSourceCreator = source.Creator as IPagedChannelSourceCreator;
				if (pagedSourceCreator != null)
				{
					var downloadSources = pagedSourceCreator.GetPage(source, 1);
				}
			}
		}
	}

}
