using System;
using System.Linq;

namespace Squid.Core
{
	public class DownloadSource : DownloadSourceBase, IDownloadSource
	{
		public ObservableDownloadSpecifierCollection Specifiers { get; private set; }

		public DownloadSource(Uri uri, DownloadSourceCreator creator, ParameterList parameters)
			: base(uri, creator, parameters)
		{
			Specifiers = new ObservableDownloadSpecifierCollection();
		}

		public DownloadSource(Uri uri, DownloadSourceCreator creator)
			: this(uri, creator, null)
		{
		}

		public void Add(IDownloadSpecifier specifier)
		{
			Specifiers.Add(specifier);
		}

		public IDownloadSpecifier GetDownloadSpecifier(string key)
		{
			return Specifiers.FirstOrDefault(s => s.Key == key);
		}
	}
}
