using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Squid.Core
{
	public abstract class DownloadSourceCreator : SourceCreatorBase
	{
		public const string CreatorParameterName = "Creator";

		protected DownloadSourceCreator(ISourceFactory factory)
			: base(factory)
		{
			SourceType = typeof(IDownloadSource);
		}

		protected abstract SchemeCollection SupportedSchemes
		{
			get;
		}

		protected abstract MediaTypeCollection RegisteredMediaTypes
		{
			get;
		}

		public string GetRegisteredMediaExtension(string mediaType)
		{
			MediaTypeCollection collection = RegisteredMediaTypes;
			if (collection != null && collection.ContainsKey(mediaType))
				return collection[mediaType];
			return null;
		}

		public override bool Supports(Uri uri)
		{
			return (SupportedSchemes != null && SupportedSchemes.Contains(uri.Scheme));
		}

		public abstract Stream CreateStream(Uri uri, long initialPosition, long endPosition);

		public abstract RemoteFileInfo GetFileInfo(Uri uri);
	}
}
