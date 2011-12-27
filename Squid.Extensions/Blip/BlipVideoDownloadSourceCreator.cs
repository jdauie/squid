using System;
using System.Text.RegularExpressions;

using Squid.Core;

namespace Squid.Extensions.Blip
{
	public class BlipVideoDownloadSourceCreator : VideoDownloadSourceCreator
	{
		public const string ShowParameterName = "Show";
		public const string ShowHrefParameterName = "ShowHref";
		public const string ShowArchiveHrefParameterName = "ShowArchiveHref";
		public const string ShowThumbnailParameterName = "ShowThumbnail";
		
		public BlipVideoDownloadSourceCreator(ISourceFactory factory)
			: base(factory)
		{
		}

		protected override MediaTypeCollection RegisteredMediaTypes
		{
			get
			{
				return new MediaTypeCollection(
					new MediaType("video/mp4", ".mp4"),
					new MediaType("video/x-flv", ".flv")
				);
			}
		}

		protected override string SupportedHost
		{
			get { return @"((?<user>[\w-]+).)?blip.tv"; }
		}

		protected override string SupportedPathAndQuery
		{
			get { return @"/file/(?<id>[\d]+)[^\d]*"; }
		}

		public override ISource Create(Uri uri, string pageData)
		{
			Match m = PathAndQueryRegex.Match(uri.PathAndQuery);
			int id = int.Parse(m.Groups["id"].Value);

			string showIcon = pageData.GetStringBetween("<div id=\"ShowIcon\">", " src=\"", "\"");
			string showTitle = pageData.GetStringBetween("<div id=\"ShowTitle\">", ">", "<");

			int index = 0;
			string showInfoLinks = pageData.GetStringBetween("<div id=\"ShowInfoLinks\">", "</div>");
			string showHref = showInfoLinks.GetStringBetween(ref index, "href=\"", "\"");
			string showArchiveHref = showInfoLinks.GetStringBetween(ref index, "href=\"", "\"");
			
			string title = pageData.GetStringBetween("player.setPostsTitle(\"", "\"");
			string description = pageData.GetStringBetween("<meta name=\"description\" content=\"", "\"");
			string thumbnail = pageData.GetStringBetween("<link rel=\"videothumbnail\" href=\"", "\"");

			ParameterList parameters = new ParameterList();
			parameters.AddValue(VideoDownloadSourceCreator.IdParameterName, id.ToString());
			parameters.AddValue(VideoDownloadSourceCreator.TitleParameterName, title);
			parameters.AddValue(VideoDownloadSourceCreator.DescriptionParameterName, description);
			parameters.AddValue(VideoDownloadSourceCreator.ThumbnailParameterName, thumbnail);
			parameters.AddValue(ShowParameterName, showTitle);
			parameters.AddValue(ShowHrefParameterName, showHref);
			parameters.AddValue(ShowArchiveHrefParameterName, showArchiveHref);
			parameters.AddValue(ShowThumbnailParameterName, showIcon);

			VideoDownloadSource downloadSource = new VideoDownloadSource(uri, this, parameters);
			
			string formatOptionStart = String.Format("<option value=\"/file/{0}?filename=", id);
			int currentPosition = 0;

			while (true)
			{
				string fileName = pageData.GetStringBetween(ref currentPosition, formatOptionStart, "\"");
				if (string.IsNullOrEmpty(fileName))
					break;
				currentPosition += fileName.Length;
				string formatName = pageData.GetStringBetween(ref currentPosition, ">", "</option>");
				currentPosition += formatName.Length;
				formatName = formatName.HtmlDecode();
				Uri fileUri = new Uri(String.Format("http://blip.tv/file/get/{0}", fileName));

				ParameterList specParameters = new ParameterList();
				specParameters.AddValue(BlipVideoDownloadSpecifier.FormatNameParameterName, formatName);

				DownloadSpecifier ds = new BlipVideoDownloadSpecifier(fileUri, downloadSource, specParameters);
				downloadSource.Add(ds);
			}

			return downloadSource;
		}
	}
}
