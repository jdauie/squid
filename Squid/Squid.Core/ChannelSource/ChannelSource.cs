using System;
using System.Linq;
using System.Collections.Generic;

namespace Squid.Core
{
	public abstract class ChannelSource : SourceBase, IChannelSource
	{
		public ObservableDownloadSourceCollection DownloadSources { get; private set; }

		protected ChannelSource(Uri uri, ChannelSourceCreator creator, ParameterList parameters)
			: base(uri, parameters)
		{
			SetParameterValue(ChannelSourceCreator.CreatorParameterName, creator);

			DownloadSources = new ObservableDownloadSourceCollection();
		}

		protected ChannelSource(Uri uri, ChannelSourceCreator creator)
			: this(uri, creator, null)
		{
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(ChannelSourceCreator.CreatorParameterName, null)
			});
		}

		public ISourceCreator Creator
		{
			get { return (ISourceCreator)GetParameterValue(ChannelSourceCreator.CreatorParameterName); }
		}

		public virtual string Name
		{
			get { return Uri.Segments.LastOrDefault(); }
		}

		public void Add(IDownloadSource downloadSource)
		{
			DownloadSources.Add(downloadSource);
		}

		public override string ToString()
		{
			return String.Format("{0}", Uri);
		}
	}
}
