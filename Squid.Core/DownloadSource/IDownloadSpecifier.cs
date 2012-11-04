using System;

namespace Squid.Core
{
	public interface IDownloadSpecifier : IComparable<IDownloadSpecifier>
	{
		IDownloadSource Source { get; }
		RemoteFileInfo RemoteFileInfo { get; }

		string Key { get; }
		string Name { get; }
		string SuggestedFileName { get; }
	}
}
