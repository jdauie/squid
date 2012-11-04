using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Squid.Core
{
	public abstract class VideoDownloadSourceCreator : HostDownloadSourceCreator
	{
		public const string IdParameterName = "Id";
		public const string TitleParameterName = "Title";
		public const string DescriptionParameterName = "Description";
		public const string ThumbnailParameterName = "Thumbnail";

		protected VideoDownloadSourceCreator(ISourceFactory factory)
			: base(factory)
		{
		}

		public abstract ISource Create(Uri uri, string pageData);

		public override ISource Create(Uri uri)
		{
			String pageData = GetPageData(uri);
			return Create(uri, pageData);
		}

		protected string GetPageData(Uri uri)
		{
			String pageData;
			using (var stream = CreateStream(uri, 0, 0))
			{
				using (var sr = new StreamReader(stream))
				{
					pageData = sr.ReadToEnd();
				}
			}
			return pageData;
		}
	}
}
