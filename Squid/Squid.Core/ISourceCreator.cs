using System;

namespace Squid.Core
{
	public interface ISourceCreator
	{
		ISourceFactory Factory { get; }
		Type SourceType { get; }

		void Init();
		bool Supports(Uri uri);
		ISource Create(Uri uri);
	}
}
