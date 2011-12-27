using System;
using System.IO;

namespace Squid.Core
{
	public abstract class SourceCreatorBase : ISourceCreator
	{
		public const string UriParameterName = "Uri";

		public ISourceFactory Factory { get; private set; }
		public Type SourceType { get; protected set; }

		public SourceCreatorBase(ISourceFactory factory)
		{
			Factory = factory;
		}

		public virtual void Init()
		{
		}

		public abstract bool Supports(Uri uri);

		public abstract ISource Create(Uri uri);
	}
}
