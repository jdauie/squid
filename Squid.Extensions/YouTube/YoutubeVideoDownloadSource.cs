using System;
using System.Collections.Generic;
using System.Linq;

using Squid.Core;

namespace Squid.Extensions.YouTube
{
	class YoutubeVideoDownloadSource : VideoDownloadSource
	{
		public YoutubeVideoDownloadSource(Uri uri, DownloadSourceCreator creator, ParameterList parameters)
			: base(uri, creator, parameters)
		{
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(YoutubeVideoDownloadSourceCreator.UserParameterName, String.Empty),
				new Parameter(YoutubeVideoDownloadSourceCreator.UserHrefParameterName, String.Empty),
				new Parameter(YoutubeVideoDownloadSourceCreator.DateParameterName, String.Empty),
				new Parameter(YoutubeVideoDownloadSourceCreator.TimeParameterName, String.Empty),
				new Parameter(YoutubeVideoDownloadSourceCreator.ViewCountParameterName, String.Empty),
			});
		}

		public string User
		{
			get { return (string)GetParameterValue(YoutubeVideoDownloadSourceCreator.UserParameterName); }
		}

		public string UserHref
		{
			get { return (string)GetParameterValue(YoutubeVideoDownloadSourceCreator.UserHrefParameterName); }
		}

		public DateTime Date
		{
			get { return (DateTime)GetParameterValue(YoutubeVideoDownloadSourceCreator.DateParameterName); }
		}

		public string Time
		{
			get { return (string)GetParameterValue(YoutubeVideoDownloadSourceCreator.TimeParameterName); }
		}

		public int ViewCount
		{
			get { return (int)GetParameterValue(YoutubeVideoDownloadSourceCreator.ViewCountParameterName); }
		}

		public override string ToString()
		{
			return String.Format("{0}: {1}", User, base.ToString());
		}
	}
}
