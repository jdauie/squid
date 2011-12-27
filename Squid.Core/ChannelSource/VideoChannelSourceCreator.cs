using System;
using System.IO;

namespace Squid.Core
{
	public abstract class VideoChannelSourceCreator : HostChannelSourceCreator
	{
		public const string UserParameterName = "User";
		public const string UserHrefParameterName = "UserHref";
		public const string TitleParameterName = "Title";
		public const string DescriptionParameterName = "Description";
		public const string ThumbnailParameterName = "Thumbnail";

		public VideoChannelSourceCreator(ISourceFactory factory)
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
			using (Stream stream = CreateStream(uri))
			{
				using (StreamReader sr = new StreamReader(stream))
				{
					pageData = sr.ReadToEnd();
				}
			}
			return pageData;
		}

		protected string GetPageData(Uri uri, string postData)
		{
			String pageData;
			using (Stream stream = CreateStream(uri, postData))
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
