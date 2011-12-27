using System;

namespace Squid.Core
{
	public interface IChannelSource : ISource
	{
		ISourceCreator Creator { get; }
		ObservableDownloadSourceCollection DownloadSources { get; }

		string Name { get; }

		void Add(IDownloadSource downloadSource);
		string ToString();
	}
}
