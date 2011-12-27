using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squid.Core
{
	public class DownloadSourceFactory : SourceFactoryBase
	{
		//private SortedDictionary<string, string> m_registeredMediaTypes;

		public DownloadSourceFactory(Context context) : base(context)
		{
			CreatorType = typeof(DownloadSourceCreator);
		}
	}
}
