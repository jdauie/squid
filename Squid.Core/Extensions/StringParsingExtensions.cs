using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squid.Core
{
	public static class StringParsingExtensions
	{
		public static string GetStringBetween(this string content, string startIdentifier, string endIdentifier)
		{
			int startIndex = 0;
			return GetStringBetween(content, ref startIndex, startIdentifier, null, endIdentifier);
		}

		public static string GetStringBetween(this string content, ref int startIndex, string startIdentifier, string endIdentifier)
		{
			return GetStringBetween(content, ref startIndex, startIdentifier, null, endIdentifier);
		}

		public static string GetStringBetween(this string content, string startIdentifier, string startRecordingAfter, string endIdentifier)
		{
			int startIndex = 0;
			return GetStringBetween(content, ref startIndex, startIdentifier, startRecordingAfter, endIdentifier);
		}

		public static string GetStringBetween(this string content, ref int startIndex, string startIdentifier, string startRecordingAfter, string endIdentifier)
		{
			string between = String.Empty;
			int newStartIndex = startIndex;
			
			if (!string.IsNullOrEmpty(startIdentifier))
				newStartIndex = content.IndexOf(startIdentifier, newStartIndex);

			if (newStartIndex > -1)
			{
				if(startIdentifier != null)
					newStartIndex += startIdentifier.Length;

				if (!string.IsNullOrEmpty(startRecordingAfter))
				{
					int newStartRecordingIndex = content.IndexOf(startRecordingAfter, newStartIndex);
					if (newStartRecordingIndex > -1)
						newStartIndex = newStartRecordingIndex + startRecordingAfter.Length;
				}

				int endIndex = content.Length;
				if (!string.IsNullOrEmpty(endIdentifier))
					endIndex = content.IndexOf(endIdentifier, newStartIndex);
				
				if (endIndex > -1)
				{
					startIndex = newStartIndex;
					between = content.Substring(newStartIndex, endIndex - newStartIndex);
				}
			}
			
			return between;
		}
	}
}
