using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Squid.Core;

namespace Squid.Extensions.YouTube
{
	class YoutubeVideoChannelSourceCreator : VideoChannelSourceCreator, IPagedChannelSourceCreator
	{
		private const string pagingUrlFormat = "/profile_ajax?action_ajax=1&user={0}&new=1&box_method=load_playlist_page&box_name=user_playlist_navigator";
		private const string pagingPostDataFormat = "messages=[{{\"type\":\"box_method\",\"request\":{{\"name\":\"user_playlist_navigator\",\"x_position\":1,\"y_position\":-2,\"palette\":\"default\",\"method\":\"load_playlist_page\",\"params\":{{\"playlist_name\":\"uploads\",\"encrypted_playlist_id\":\"uploads\",\"query\":\"\",\"encrypted_shmoovie_id\":\"uploads\",\"page_num\":{0},\"view\":\"grid\",\"playlist_sort\":\"default\"}}}}}}]&{1}";

		public const string AjaxSessionInfoParameterName = "AjaxSessionInfo";

		public YoutubeVideoChannelSourceCreator(ChannelSourceFactory factory)
			: base(factory)
		{
		}

		protected override string SupportedHost
		{
			get { return @"(www.)?youtube.com"; }
		}

		protected override string SupportedPathAndQuery
		{
			get { return @"/user/(?<id>[A-Za-z0-9]+)/?"; }
		}

		public override ISource Create(Uri uri, string pageData)
		{
			string userHref = pageData.GetStringBetween("<link rel=\"canonical\"", " href=\"", "\"");
			string username = userHref.GetStringBetween("/user/", null);
			
			string title = pageData.GetStringBetween("<meta name=\"title\" content=\"", "\"");
			string description = pageData.GetStringBetween("<meta name=\"description\" content=\"", "\"");
			string thumbnail = pageData.GetStringBetween("<meta property=\"og:image\" content=\"", "\"");

			string ajaxSessionInfo = pageData.GetStringBetween("window.ajax_session_info", "'", "'");

			ParameterList parameters = new ParameterList();
			parameters.AddValue(VideoChannelSourceCreator.UserParameterName, username);
			parameters.AddValue(VideoChannelSourceCreator.UserHrefParameterName, userHref);
			parameters.AddValue(VideoChannelSourceCreator.TitleParameterName, title);
			parameters.AddValue(VideoChannelSourceCreator.DescriptionParameterName, description);
			parameters.AddValue(VideoChannelSourceCreator.ThumbnailParameterName, thumbnail);
			parameters.AddValue(AjaxSessionInfoParameterName, ajaxSessionInfo);

			YoutubeVideoChannelSource channelSource = new YoutubeVideoChannelSource(uri, this, parameters);
			
			var downloadSources = ParseDownloadSources(pageData, channelSource);
			foreach (var downloadSource in downloadSources)
				channelSource.Add(downloadSource);

			return channelSource;
		}

		private IEnumerable<IDownloadSource> ParseDownloadSources(string pageData, YoutubeVideoChannelSource channelSource)
		{
			List<IDownloadSource> sources = new List<IDownloadSource>();
			DownloadSourceCreator downloadSourceCreator = null;

			int currentPosition = 0;

			while (true)
			{
				string videoId = pageData.GetStringBetween(ref currentPosition, "\"encryptedVideoId\">", "<");
				if (string.IsNullOrEmpty(videoId))
					break;
				videoId = videoId.Trim();

				string videoUri = pageData.GetStringBetween(ref currentPosition, "<div class=\"playnav-video-thumb\"", " href=\"", "\"");
				string videoThumb = pageData.GetStringBetween(ref currentPosition, " src=\"", "\"");
				string videoTitle = pageData.GetStringBetween(ref currentPosition, "title=\"", "\"").HtmlDecode();
				string videoTime = pageData.GetStringBetween(ref currentPosition, "<span class=\"video-time\">", "<");
				string videoViews = pageData.GetStringBetween(ref currentPosition, "<div class=\"metadata\">", ">", " views");

				videoThumb = new Uri(channelSource.Uri, videoThumb).AbsoluteUri;
				int parsedViews = int.Parse(videoViews, System.Globalization.NumberStyles.AllowThousands);
				Uri absoluteUri = new Uri(channelSource.Uri, videoUri);

				if (downloadSourceCreator == null)
					downloadSourceCreator = (DownloadSourceCreator)Factory.Context.FindFactoryByCreatorType(typeof(DownloadSourceCreator)).GetCreator(absoluteUri);

				ParameterList videoParameters = new ParameterList();
				videoParameters.AddValue(VideoDownloadSourceCreator.IdParameterName, videoId);
				videoParameters.AddValue(VideoDownloadSourceCreator.TitleParameterName, videoTitle);
				videoParameters.AddValue(VideoDownloadSourceCreator.DescriptionParameterName, channelSource.Description);
				videoParameters.AddValue(VideoDownloadSourceCreator.ThumbnailParameterName, videoThumb);
				videoParameters.AddValue(YoutubeVideoDownloadSourceCreator.UserParameterName, channelSource.User);
				videoParameters.AddValue(YoutubeVideoDownloadSourceCreator.UserHrefParameterName, channelSource.UserHref);
				videoParameters.AddValue(YoutubeVideoDownloadSourceCreator.TimeParameterName, videoTime);
				videoParameters.AddValue(YoutubeVideoDownloadSourceCreator.ViewCountParameterName, parsedViews);

				YoutubeVideoDownloadSource ds = new YoutubeVideoDownloadSource(absoluteUri, downloadSourceCreator, videoParameters);
				sources.Add(ds);
			}
			return sources;
		}

		public ICollection<IDownloadSource> GetPage(IChannelSource source, int page)
		{
			YoutubeVideoChannelSource channelSource = (YoutubeVideoChannelSource)source;
			// youtube channels are always on page zero,
			// so relative pages must be greater than zero
			if (page <= 0)
				throw new ArgumentOutOfRangeException("page");

			string ajaxSessionInfo = channelSource.AjaxSessionInfo;
			string postData = String.Format(pagingPostDataFormat, page, ajaxSessionInfo);

			string pagingUrl = String.Format(pagingUrlFormat, channelSource.User);
			Uri pagingUri = new Uri(channelSource.Uri, pagingUrl);
			string pageData = GetPageData(pagingUri, postData);

			string escapedMarkup = pageData.GetStringBetween("\"data\"", "\"", "<\\/div>\"");
			string markup = Regex.Unescape(escapedMarkup);

			IDownloadSource[] downloadSources = ParseDownloadSources(markup, channelSource).ToArray();

			return downloadSources;
		}
	}
}
