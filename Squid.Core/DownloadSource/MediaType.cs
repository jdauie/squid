using System;

namespace Squid.Core
{
	public class MediaType
	{
		public string Type { get; private set; }
		public string Extension { get; private set; }

		public MediaType(string type, string extension)
		{
			Type = type;
			Extension = extension;
		}

		public override string ToString()
		{
			return String.Format("{0}:{1}", Type, Extension);
		}
	}
}
