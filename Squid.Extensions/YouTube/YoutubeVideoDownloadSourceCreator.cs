using System;
using System.Collections.Generic;

using Squid.Core;

namespace Squid.Extensions.YouTube
{
	class YoutubeVideoDownloadSourceCreator : VideoDownloadSourceCreator
	{
		public const string UserParameterName = "User";
		public const string UserHrefParameterName = "UserHref";
		public const string DateParameterName = "Date";
		public const string TimeParameterName = "Time";
		public const string ViewCountParameterName = "ViewCount";
		
		public YoutubeVideoDownloadSourceCreator(ISourceFactory factory)
			: base(factory)
		{
		}

		protected override MediaTypeCollection RegisteredMediaTypes
		{
			get
			{
				return new MediaTypeCollection(
					new MediaType("video/mp4", ".mp4"),
					new MediaType("video/x-flv", ".flv"),
					new MediaType("video/webm", ".webm"),
					new MediaType("video/3gpp", ".3gp")
				);
			}
		}

		protected override string SupportedHost
		{
			get { return @"(www.)?youtube.com"; }
		}

		protected override string SupportedPathAndQuery
		{
			get { return @"/watch\?v=(?<id>[\w-]+)"; }
		}

		public override ISource Create(Uri uri, string pageData)
		{
			string title = pageData.GetStringBetween("<meta name=\"title\" content=\"", "\"");
			string description = pageData.GetStringBetween("<meta name=\"description\" content=\"", "\"");
			string thumbnail = pageData.GetStringBetween("<meta property=\"og:image\" content=\"", "\"");

			string viewsRegion = pageData.GetStringBetween("<span class=\"watch-view-count\"", ">", "</span>");
			string views = viewsRegion.GetStringBetween("<strong>", "</strong>");
			int viewCount = int.Parse(views, System.Globalization.NumberStyles.AllowThousands);

			string userRegion = pageData.GetStringBetween("<a class=\"watch-description-username\"", "</a>");
			string userHref = userRegion.GetStringBetween("href=\"", "\"");
			string user = userRegion.GetStringBetween("<strong>", "</strong>");

			string dateRegion = pageData.GetStringBetween("<span id=\"eow-date-short\"", ">", "</span>").Trim();
			DateTime date = DateTime.Parse(dateRegion);
			
			string flashVarsValue = pageData.GetStringBetween("<param name=\\\"flashvars\\\" value=\\\"", "\\\"");
			string[] flashVarsSplit = flashVarsValue.Split('&');
			SortedList<string, string> flashVars = new SortedList<string, string>(flashVarsSplit.Length);
			foreach (string chunk in flashVarsSplit)
			{
				string[] chunkSplit = chunk.Split('=');
				flashVars.Add(chunkSplit[0], Uri.UnescapeDataString(chunkSplit[1]));
			}

			string fmt_map = flashVars["fmt_map"];
			string[] fmtSplit = fmt_map.Split(',');
			Dictionary<int, int[]> qualityLevels = new Dictionary<int, int[]>();
			foreach (string chunk in fmtSplit)
			{
				string[] fmt = chunk.Split('/');
				string[] res = fmt[1].Split('x');
				qualityLevels.Add(int.Parse(fmt[0]), new int[] { int.Parse(res[0]), int.Parse(res[1]) });
			}

			string id = flashVars["video_id"];

			ParameterList parameters = new ParameterList();
			parameters.AddValue(VideoDownloadSourceCreator.IdParameterName, id);
			parameters.AddValue(VideoDownloadSourceCreator.TitleParameterName, title);
			parameters.AddValue(VideoDownloadSourceCreator.DescriptionParameterName, description);
			parameters.AddValue(VideoDownloadSourceCreator.ThumbnailParameterName, thumbnail);
			parameters.AddValue(UserParameterName, user);
			parameters.AddValue(UserHrefParameterName, userHref);
			parameters.AddValue(DateParameterName, date);
			parameters.AddValue(ViewCountParameterName, viewCount);

			VideoDownloadSource downloadSource = new YoutubeVideoDownloadSource(uri, this, parameters);

			string fmt_url_map = flashVars["fmt_url_map"];
			string[] urlSplit = fmt_url_map.Split(',');
			foreach (string chunk in urlSplit)
			{
				string[] fmt_url = chunk.Split('|');
				Uri fmt_uri = new Uri(fmt_url[1]);
				int qualityLevel = int.Parse(fmt_url[0]);
				int[] res = qualityLevels[qualityLevel];
				string resolution = String.Format("{0}x{1}", res[0], res[1]);

				ParameterList specParameters = new ParameterList();
				specParameters.AddValue(YoutubeVideoDownloadSpecifier.FmtParameterName, qualityLevel);
				specParameters.AddValue(YoutubeVideoDownloadSpecifier.ResolutionParameterName, resolution);
				specParameters.AddValue(YoutubeVideoDownloadSpecifier.MaxPixelHeightParameterName, res[1]);

				DownloadSpecifier ds = new YoutubeVideoDownloadSpecifier(fmt_uri, downloadSource, specParameters);
				downloadSource.Add(ds);
			}

			return downloadSource;
		}
	}
}
