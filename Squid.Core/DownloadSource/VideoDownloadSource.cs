using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Squid.Core
{
	public class VideoDownloadSource : DownloadSource
	{
		public VideoDownloadSource(Uri uri, DownloadSourceCreator creator, ParameterList parameters)
			: base(uri, creator, parameters)
		{
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(VideoDownloadSourceCreator.IdParameterName, String.Empty),
				new Parameter(VideoDownloadSourceCreator.TitleParameterName, String.Empty),
				new Parameter(VideoDownloadSourceCreator.DescriptionParameterName, String.Empty),
				new Parameter(VideoDownloadSourceCreator.ThumbnailParameterName, String.Empty),
			});
		}

		public string Id
		{
			get { return (string)GetParameterValue(VideoDownloadSourceCreator.IdParameterName); }
		}

		public string Title
		{
			get { return (string)GetParameterValue(VideoDownloadSourceCreator.TitleParameterName); }
		}

		public string Description
		{
			get { return (string)GetParameterValue(VideoDownloadSourceCreator.DescriptionParameterName); }
		}

		public string Thumbnail
		{
			get { return (string)GetParameterValue(VideoDownloadSourceCreator.ThumbnailParameterName); }
		}

		public override string Name
		{
			get
			{
				return Title;
			}
		}

		public override string ToString()
		{
			return Title;
		}
	}
}
