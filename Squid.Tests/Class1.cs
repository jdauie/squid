using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;

using Squid.Core;

namespace Squid.Tests
{
	[TestFixture]
	public class ExampleTestOfNUnit
	{
		private Context m_context;
		private string[] m_channelUrls;
		private string[] m_downloadUrls;

		[SetUp]
		public void Init()
		{
			m_context = Context.Instance;

			m_channelUrls = new string[]
			{
				"http://www.youtube.com/user/psystarcraft",
				"http://day9tv.blip.tv/posts?view=archive&nsfw=dc"
			};

			m_downloadUrls = new string[]
			{
				"http://www.youtube.com/watch?v=oklB9_k5mNk",
				"http://day9tv.blip.tv/file/4593518/"
			};
		}

		[Test]
		public void TestChannelSources()
		{
			foreach (string url in m_channelUrls)
			{
				Uri uri = new Uri(url);
				ISource source = GetSource(uri, typeof(ChannelSourceCreator));
			}
		}

		[Test]
		public void TestDownloadSources()
		{
			foreach (string url in m_downloadUrls)
			{
				Uri uri = new Uri(url);
				ISource source = GetSource(uri, typeof(DownloadSourceCreator));
			}
		}

		private ISource GetSource(Uri uri, Type creatorType)
		{
			ISourceFactory factory = m_context.FindFactoryByCreatorType(creatorType);
			Assert.That(factory, Is.Not.Null, "Factory");

			ISourceCreator creator = factory.GetCreator(uri);
			Assert.That(creator, Is.Not.Null, "Creator");

			ISource source = factory.Create(uri);
			Assert.That(source, Is.Not.Null, "Source");

			return source;
		}
	}

}
