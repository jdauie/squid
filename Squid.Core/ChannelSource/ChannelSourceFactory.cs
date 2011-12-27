using System;

namespace Squid.Core
{
	public class ChannelSourceFactory : SourceFactoryBase
	{
		public ChannelSourceFactory(Context context) : base(context)
	    {
			CreatorType = typeof(ChannelSourceCreator);
	    }
	}
}
