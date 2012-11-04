using System;
using System.Collections.Generic;
using System.Linq;

namespace Squid.Core
{
	public class SourceCreatorTreeNode
	{
		private readonly Type m_type;
		private readonly ISourceCreator m_instance;
		private readonly List<SourceCreatorTreeNode> m_childCreatorNodes;

		public ISourceCreator Creator
		{
			get { return m_instance; }
		}

		public SourceCreatorTreeNode(ISourceFactory factory, Type baseType, IList<Type> creatorTypes)
		{
			m_type = baseType;

			if (!m_type.IsAbstract)
			{
				m_instance = Activator.CreateInstance(m_type, factory) as ISourceCreator;
				m_instance.Init();
			}

			var childCreatorTypes = creatorTypes.Where(t => t.BaseType == m_type).ToList();

			if (childCreatorTypes.Count > 0)
			{
				// remove children from search
				creatorTypes = creatorTypes.Except(childCreatorTypes).ToList();

				m_childCreatorNodes = childCreatorTypes.Select(t => new SourceCreatorTreeNode(factory, t, creatorTypes)).ToList();
			}
		}

		public static SourceCreatorTreeNode CreateImplementationHeirarchy(ISourceFactory factory)
		{
			Type baseType = factory.CreatorType;

			AppDomain app = AppDomain.CurrentDomain;
			var assemblies = app.GetAssemblies();
			var creatorTypes = assemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => !t.IsInterface && baseType.IsAssignableFrom(t))
				.ToList();

			var rootCreatorNode = new SourceCreatorTreeNode(factory, baseType, creatorTypes);

			return rootCreatorNode;
		}

		public ISourceCreator GetCreator(Uri uri)
		{
			if (m_childCreatorNodes != null)
			{
				foreach (var childCreatorNode in m_childCreatorNodes)
				{
					if (childCreatorNode.Creator == null || childCreatorNode.Creator.Supports(uri))
						return childCreatorNode.GetCreator(uri);
				}
			}
			return Creator;
		}

		public ISourceCreator GetCreator(Type type)
		{
			if (m_type == type)
				return Creator;

			if (m_childCreatorNodes != null)
			{
				foreach (var childCreatorNode in m_childCreatorNodes)
				{
					var creator = childCreatorNode.GetCreator(type);
					if (creator != null)
						return creator;
				}
			}

			return null;
		}

		public override string ToString()
		{
			return m_type.Name;
		}
	}
}
