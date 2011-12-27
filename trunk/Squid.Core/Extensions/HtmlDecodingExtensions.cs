using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Entity = System.Collections.Generic.KeyValuePair<string, string>;

namespace Squid.Core
{
	public static class HtmlDecodingExtensions
	{
		private static readonly KeyValuePair<string, string>[] HTML_ENTITIES = new KeyValuePair<string, string>[]
		{
			new Entity("&amp;", "&"),
			new Entity("&mdash;", "—"),
			new Entity("&quot;", "\"")
		};

		public static string HtmlDecode(this string content)
		{
			string temp = content;
			foreach (var kvp in HTML_ENTITIES)
			{
				temp = temp.Replace(kvp.Key, kvp.Value);
			}
			return temp;
		}
	}
}
