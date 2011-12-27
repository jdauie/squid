using System;

using Squid.Core;

namespace Squid.Extensions.YouTube
{
	public class YoutubeVideoChannelSource : VideoChannelSource
	{
		public YoutubeVideoChannelSource(Uri uri, ChannelSourceCreator creator, ParameterList parameters)
			: base(uri, creator, parameters)
		{
		}

		public string AjaxSessionInfo
		{
			get { return (string)GetParameterValue(YoutubeVideoChannelSourceCreator.AjaxSessionInfoParameterName); }
		}
	}
}
