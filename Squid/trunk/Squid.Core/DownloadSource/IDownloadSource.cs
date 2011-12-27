using System;

namespace Squid.Core
{
	public interface IDownloadSource : ILightweightDownloadSource
	{
		ObservableDownloadSpecifierCollection Specifiers { get; }
		IDownloadSpecifier GetDownloadSpecifier(string key);

		void Add(IDownloadSpecifier downloadSpecifier);
	}
}
