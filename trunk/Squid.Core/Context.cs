using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;

namespace Squid.Core
{
	public class Context
	{
		private string m_baseDirectory;
		private List<ISourceFactory> m_factories;

		private static Context m_instance;

		public static Context Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new Context();
				return m_instance;
			}
		}

		public Context()
		{
			ServicePointManager.DefaultConnectionLimit = int.MaxValue;

			m_baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			Console.WriteLine("Base: {0}", m_baseDirectory);
			
			RegisterExtensions();
			RegisterFactories();
		}

		public ISourceFactory FindFactoryByCreatorType(Type type)
		{
			ISourceFactory factory = m_factories.Where(f => f.CreatorType.IsAssignableFrom(type)).FirstOrDefault();
			return factory;
		}

		public ISource Create(Uri uri)
		{
			ISource source = null;

			var creator = m_factories
				.Select(f => f.GetCreator(uri))
				.Where(c => c != null)
				.FirstOrDefault();

			if (creator != null)
				source = creator.Factory.Create(uri);

			return source;
		}

		private void RegisterFactories()
		{
			Console.WriteLine("Registering Factories...");

			m_factories = new List<ISourceFactory>();

			Type baseType = typeof(ISourceFactory);
			AppDomain app = AppDomain.CurrentDomain;
			var assemblies = app.GetAssemblies();
			var factoryTypes = assemblies
				.SelectMany(a => a.GetTypes())
				.Where(t => baseType.IsAssignableFrom(t));

			foreach (Type type in factoryTypes)
			{
				char result = '-';
				if (!type.IsAbstract)
				{
					try
					{
						ISourceFactory factory = Activator.CreateInstance(type, this) as ISourceFactory;
						factory.Init();
						m_factories.Add(factory);
						result = '+';
					}
					catch (Exception)
					{
						result = 'x';
					}
				}
				Console.WriteLine(" {0} {1}", result, type.Name);
			}
		}

		/// <summary>
		/// Currently, this looks at all directories below the current domain base, recursively.
		/// </summary>
		private void RegisterExtensions()
		{
			Console.WriteLine("Registering Extensions...");

			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			var assemblyLookup = assemblies
				.Where(a => !String.IsNullOrEmpty(a.Location))
				.Distinct(a => a.Location, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(a => a.Location, a => a, StringComparer.OrdinalIgnoreCase);

			String path = m_baseDirectory;
			if (Directory.Exists(path))
			{
				var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
				var newAssemblyPaths = files
					.Distinct(StringComparer.OrdinalIgnoreCase)
					.Where(f => !assemblyLookup.ContainsKey(f));

				foreach (String assemblyPath in newAssemblyPaths)
				{
					char result = 'x';
					try
					{
						Assembly assembly = Assembly.LoadFrom(assemblyPath);
						assemblyLookup.Add(assemblyPath, assembly);
						result = '+';
					}
					catch (Exception) { }
					Console.WriteLine(" {0} {1}", result, assemblyPath);
				}
			}
		}
	}
}
