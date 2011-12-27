using System.Collections.Generic;

namespace Squid.Core
{
	public class SchemeCollection : SortedSet<string>
	{
		public SchemeCollection(params string[] schemes)
		{
			foreach (string scheme in schemes)
				Add(scheme);
		}
	}
}
