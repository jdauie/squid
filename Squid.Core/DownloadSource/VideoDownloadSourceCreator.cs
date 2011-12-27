using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Squid.Core
{
	public abstract class VideoDownloadSourceCreator : HostDownloadSourceCreator
	{
		public const string IdParameterName = "Id";
		public const string TitleParameterName = "Title";
		public const string DescriptionParameterName = "Description";
		public const string ThumbnailParameterName = "Thumbnail";
		
		public VideoDownloadSourceCreator(ISourceFactory factory)
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
			using (Stream stream = CreateStream(uri, 0, 0))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					pageData = sr.ReadToEnd();
				}
			}
			return pageData;
		}
	}
}
