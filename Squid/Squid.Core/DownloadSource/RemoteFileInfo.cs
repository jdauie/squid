using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squid.Core
{
	public class RemoteFileInfo
	{
		public string ContentType { get; private set; }
		public DateTime LastModified { get; private set; }
		public long FileSize { get; private set; }
		public bool AcceptRanges { get; private set; }

		public RemoteFileInfo(string contentType, DateTime lastModified, long fileSize, bool acceptRanges)
		{
			ContentType = contentType;
			LastModified = lastModified;
			FileSize = fileSize;
			AcceptRanges = acceptRanges;
		}

		public override string ToString()
		{
			return String.Format("{0}, {1}", ContentType, FileSize.ToSize());
		}
	}

}
