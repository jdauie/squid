using System;
using System.Collections.Generic;

namespace Squid.Core
{
	public interface IPagedChannelSourceCreator
	{
		/// <summary>
		/// Get another page of download sources.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="page">Page number relative to the current channel page.</param>
		/// <returns></returns>
		ICollection<IDownloadSource> GetPage(IChannelSource source, int page);
	}
}
