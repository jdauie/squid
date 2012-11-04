using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;

namespace Squid.Core
{
	public class HttpDownloadSourceCreator : DownloadSourceCreator
	{
		public HttpDownloadSourceCreator(ISourceFactory factory)
			: base(factory)
		{
		}

		protected override SchemeCollection SupportedSchemes
		{
			get { return new SchemeCollection("http"); }
		}

		protected override MediaTypeCollection RegisteredMediaTypes
		{
			get
			{
				// in the future, return common media types from a utility class?
				return null;
			}
		}

		public override ISource Create(Uri uri)
		{
			var downloadSource = new DownloadSource(uri, this);
			// why was this here? debugging?
			//var downloadSpecifier = new DownloadSpecifier(uri, downloadSource);
			return downloadSource;
		}

		protected virtual WebRequest GetRequest(Uri uri)
		{
#warning DUPLICATE CODE
			WebRequest request = WebRequest.Create(uri);
			request.Timeout = 30000;
			return request;
		}

		public override Stream CreateStream(Uri uri, long startPosition, long endPosition)
		{
			var request = (HttpWebRequest)GetRequest(uri);
			if (startPosition > 0)
			{
				if (endPosition > 0)
					request.AddRange(startPosition, endPosition);
				else
					request.AddRange(startPosition);
			}
			var response = (HttpWebResponse)request.GetResponse();
			Stream stream = response.GetResponseStream();
			return stream;
		}

		public override RemoteFileInfo GetFileInfo(Uri uri)
		{
			RemoteFileInfo fileInfo = null;
			WebRequest request = GetRequest(uri);
			using (var response = (HttpWebResponse)request.GetResponse())
			{
				bool acceptRanges = String.Equals(response.Headers["Accept-Ranges"], "bytes", StringComparison.InvariantCultureIgnoreCase);
				fileInfo = new RemoteFileInfo(response.ContentType, response.LastModified, response.ContentLength, acceptRanges);
			}
			return fileInfo;
		}
	}
}
