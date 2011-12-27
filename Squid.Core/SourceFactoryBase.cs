using System;

namespace Squid.Core
{
	public abstract class SourceFactoryBase : ISourceFactory
	{
		public Context Context { get; private set; }

		public Type CreatorType { get; protected set; }
		
		private SourceCreatorTreeNode m_rootCreatorTree;

		public SourceFactoryBase(Context context)
		{
			Context = context;
		}

		public void Init()
		{
			m_rootCreatorTree = SourceCreatorTreeNode.CreateImplementationHeirarchy(this);
		}

		public ISource Create(Uri uri)
		{
			ISourceCreator creator = GetCreator(uri) as ISourceCreator;
			if (creator != null)
				return creator.Create(uri);
			return null;
		}

		public ISourceCreator GetCreator(Uri uri)
		{
			ISourceCreator creator = m_rootCreatorTree.GetCreator(uri);
			return creator;
		}
	}
}
