using System.Collections.Generic;

namespace Squid.Core
{
	public class MediaTypeCollection : SortedDictionary<string, string>
	{
		public MediaTypeCollection(params MediaType[] mediaTypes)
		{
			foreach (MediaType mt in mediaTypes)
				Add(mt.Type, mt.Extension);
		}
	}
}
