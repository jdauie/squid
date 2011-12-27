using System;
using System.Linq;
using System.Collections.Generic;

using Squid.Core;
using System.Text.RegularExpressions;

namespace Squid.Extensions.Blip
{
	class BlipVideoChannelSourceCreator : VideoChannelSourceCreator, IPagedChannelSourceCreator
	{
		public const string CurrentPageParameterName = "CurrentPage";
		public const string TotalPagesParameterName = "TotalPages";
		public const string PagingUrlFormatParameterName = "PagingUrlFormat";

		public BlipVideoChannelSourceCreator(ChannelSourceFactory factory)
			: base(factory)
		{
		}

		protected override string SupportedHost
		{
			// http://support.blip.tv, but who cares
			get { return @"((?<user>[\w-]+).)?blip.tv"; }
		}

		protected override string SupportedPathAndQuery
		{
			// http://day9tv.blip.tv/posts?view=archive&nsfw=dc
			// http://day9tv.blip.tv/?sort=custom;view=archive;date=;user=day9tv;s=posts;nsfw=dc;page=2
			get { return @"/(posts\?view=archive.*|\?.*view=archive.*s=posts.*)"; }
		}

		public override ISource Create(Uri uri, string pageData)
		{
			string userHref = uri.Host;
			string username = userHref.Split('.')[0];
			
			string title = pageData.GetStringBetween("<title>", "</title>").Trim();
			string currentPageStr = pageData.GetStringBetween("<span id=\"pagination_current_page\"", ">", "<");
			string totalPagesStr = pageData.GetStringBetween("<span id=\"pagination_total_pages\"", ">", "<");

			string pagingUrl = pageData.GetStringBetween("<div class=\"view_pages_page\"", " href=\"", "\"");
			string pagingUrlFormat = Regex.Replace(pagingUrl, @"page=\d+", "page={0}");

			ParameterList parameters = new ParameterList();
			parameters.AddValue(VideoChannelSourceCreator.UserParameterName, username);
			parameters.AddValue(VideoChannelSourceCreator.UserHrefParameterName, userHref);
			parameters.AddValue(VideoChannelSourceCreator.TitleParameterName, title);
			parameters.AddValue(CurrentPageParameterName, int.Parse(currentPageStr));
			parameters.AddValue(TotalPagesParameterName, int.Parse(totalPagesStr));
			parameters.AddValue(PagingUrlFormatParameterName, pagingUrlFormat);

			BlipVideoChannelSource channelSource = new BlipVideoChannelSource(uri, this, parameters);

			var downloadSources = ParseDownloadSources(pageData, channelSource);
			foreach (var downloadSource in downloadSources)
				channelSource.Add(downloadSource);

			return channelSource;
		}

		private IEnumerable<IDownloadSource> ParseDownloadSources(string pageData, VideoChannelSource channelSource)
		{
			List<IDownloadSource> downloadSources = new List<IDownloadSource>();
			DownloadSourceCreator downloadSourceCreator = null;

			string videoStart = "<div class=\"ArchiveEpisodeWrapper\">";
			int currentPosition = 0;

			while (true)
			{
				string testStart = pageData.GetStringBetween(ref currentPosition, videoStart, null);
				if (String.IsNullOrEmpty(testStart))
					break;

				string videoUri = pageData.GetStringBetween(ref currentPosition, "<a href=\"", "\"");
				string videoThumb = pageData.GetStringBetween(ref currentPosition, " src=\"", "\"");
				string videoTitle = pageData.GetStringBetween(ref currentPosition, " alt=\"", "\"");
				
				string videoId = videoUri.GetStringBetween("/file/", "/");

				Uri absoluteUri = new Uri(channelSource.Uri, videoUri);

				if (downloadSourceCreator == null)
					downloadSourceCreator = (DownloadSourceCreator)Factory.Context.FindFactoryByCreatorType(typeof(DownloadSourceCreator)).GetCreator(absoluteUri);

				ParameterList videoParameters = new ParameterList();
				videoParameters.AddValue(VideoDownloadSourceCreator.IdParameterName, videoId);
				videoParameters.AddValue(VideoDownloadSourceCreator.TitleParameterName, videoTitle);
				videoParameters.AddValue(VideoDownloadSourceCreator.ThumbnailParameterName, videoThumb);

				VideoDownloadSource ds = new VideoDownloadSource(absoluteUri, downloadSourceCreator, videoParameters);
				downloadSources.Add(ds);
			}
			return downloadSources;
		}

		public ICollection<IDownloadSource> GetPage(IChannelSource source, int page)
		{
			BlipVideoChannelSource channelSource = (BlipVideoChannelSource)source;
			
			// page is relative to the current page
			// instead of throwing an exception for the current page, 
			// it could just return the current download sources
			int actualPage = channelSource.CurrentPage + page;
			if (actualPage <= 0 || actualPage > channelSource.TotalPages)
				throw new ArgumentOutOfRangeException("page");

			string pagingUrlFormat = channelSource.PagingUrlFormat;
			string pagingUrl = String.Format(pagingUrlFormat, actualPage);
			Uri pagingUri = new Uri(channelSource.Uri, pagingUrl);
			string pageData = GetPageData(pagingUri);

			IDownloadSource[] downloadSources = ParseDownloadSources(pageData, channelSource).ToArray();

			return downloadSources;
		}
	}
}
