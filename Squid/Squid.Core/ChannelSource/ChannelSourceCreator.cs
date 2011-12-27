using System;
using System.IO;

namespace Squid.Core
{
	public abstract class ChannelSourceCreator : SourceCreatorBase
	{
		public const string CreatorParameterName = "Creator";

		public ChannelSourceCreator(ISourceFactory factory)
			: base(factory)
		{
			SourceType = typeof(IChannelSource);
		}

		protected abstract SchemeCollection SupportedSchemes
		{
			get;
		}

		public override bool Supports(Uri uri)
		{
			return (SupportedSchemes != null && SupportedSchemes.Contains(uri.Scheme));
		}

		protected abstract Stream CreateStream(Uri uri);
	}
}
