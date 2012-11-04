using System;
using System.Text.RegularExpressions;

namespace Squid.Core
{
	public abstract class HostChannelSourceCreator : HttpChannelSourceCreator
	{
		protected Regex HostRegex { get; private set; }
		protected Regex PathAndQueryRegex { get; private set; }

		protected HostChannelSourceCreator(ISourceFactory factory)
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
