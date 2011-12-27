using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Web;
using System.Xml.XPath;

namespace Squid.Core
{
	public abstract class HostDownloadSourceCreator : HttpDownloadSourceCreator
	{
		protected Regex HostRegex { get; private set; }
		protected Regex PathAndQueryRegex { get; private set; }

		public HostDownloadSourceCreator(ISourceFactory factory)
			: base(factory)
		{
		}

		public override void Init()
		{
			base.Init();
			HostRegex = new Regex(String.Format("^({0})$", SupportedHost), RegexOptions.IgnoreCase);
			PathAndQueryRegex = new Regex(String.Format("^({0})$", SupportedPathAndQuery), RegexOptions.IgnoreCase);
		}

		protected abstract string SupportedHost
		{
			get;
		}

		protected abstract string SupportedPathAndQuery
		{
			get;
		}

		public override bool Supports(Uri uri)
		{
			if (base.Supports(uri))
			{
				return HostRegex.IsMatch(uri.Host) && PathAndQueryRegex.IsMatch(uri.PathAndQuery);
			}
			return false;
		}
	}
}
