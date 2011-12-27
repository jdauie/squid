using System;

namespace Squid.Core
{
	public interface ISourceFactory
	{
		Context Context { get; }
		Type CreatorType { get; }
		
		void Init();
		ISource Create(Uri uri);
		ISourceCreator GetCreator(Uri uri);
	}
}
