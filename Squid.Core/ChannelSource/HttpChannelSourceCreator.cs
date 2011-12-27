using System;
using System.IO;
using System.Net;
using System.Text;

namespace Squid.Core
{
	public abstract class HttpChannelSourceCreator : ChannelSourceCreator
	{
		public HttpChannelSourceCreator(ISourceFactory factory)
			: base(factory)
		{
		}

		protected override SchemeCollection SupportedSchemes
		{
			get { return new SchemeCollection("http"); }
		}

		protected virtual WebRequest GetRequest(Uri uri)
		{
#warning DUPLICATE CODE
			WebRequest request = WebRequest.Create(uri);
			request.Timeout = 30000;
			return request;
		}

		protected virtual WebRequest GetRequest(Uri uri, string postData)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] bytes = encoding.GetBytes(postData);

			WebRequest request = WebRequest.Create(uri);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = bytes.Length;
			request.Timeout = 30000;

			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(bytes, 0, bytes.Length);
			}

			return request;
		}

		private Stream CreateStream(HttpWebRequest request)
		{
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			Stream stream = response.GetResponseStream();
			return stream;
		}

		protected override Stream CreateStream(Uri uri)
		{
			HttpWebRequest request = (HttpWebRequest)GetRequest(uri);
			return CreateStream(request);
		}

		protected Stream CreateStream(Uri uri, string postData)
		{
			HttpWebRequest request = (HttpWebRequest)GetRequest(uri, postData);
			return CreateStream(request);
		}
	}
}
