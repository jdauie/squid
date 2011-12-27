using System;
using System.Linq;
using System.Collections.Generic;

namespace Squid.Core
{
	public class VideoChannelSource : ChannelSource
	{
		public VideoChannelSource(Uri uri, ChannelSourceCreator creator, ParameterList parameters)
			: base(uri, creator, parameters)
		{
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(VideoChannelSourceCreator.UserParameterName, String.Empty),
				new Parameter(VideoChannelSourceCreator.UserHrefParameterName, String.Empty),
				new Parameter(VideoChannelSourceCreator.TitleParameterName, String.Empty),
				new Parameter(VideoChannelSourceCreator.DescriptionParameterName, String.Empty),
				new Parameter(VideoChannelSourceCreator.ThumbnailParameterName, String.Empty),
			});
		}

		public string User
		{
			get { return (string)GetParameterValue(VideoChannelSourceCreator.UserParameterName); }
		}

		public string UserHref
		{
			get { return (string)GetParameterValue(VideoChannelSourceCreator.UserHrefParameterName); }
		}

		public string Title
		{
			get { return (string)GetParameterValue(VideoChannelSourceCreator.TitleParameterName); }
		}

		public string Description
		{
			get { return (string)GetParameterValue(VideoChannelSourceCreator.DescriptionParameterName); }
		}

		public string Thumbnail
		{
			get { return (string)GetParameterValue(VideoChannelSourceCreator.ThumbnailParameterName); }
		}

		public override string Name
		{
			get { return Title; }
		}

		public override string ToString()
		{
			return Title;
		}
	}
}
