using System;

namespace Squid.Core
{
	public interface ILightweightDownloadSource : ISource
	{
		DownloadSourceCreator Creator { get; }
		string Name { get; }
	}
}
