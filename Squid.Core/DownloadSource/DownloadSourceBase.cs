using System;
using System.Linq;
using System.Collections.Generic;

namespace Squid.Core
{
	/// <summary>
	/// Lightweight DownloadSource implementation.
	/// </summary>
	public abstract class DownloadSourceBase : SourceBase, ILightweightDownloadSource
	{
		public DownloadSourceBase(Uri uri, DownloadSourceCreator creator, ParameterList parameters)
			: base(uri, parameters)
		{
			SetParameterValue(DownloadSourceCreator.CreatorParameterName, creator);
		}

		public DownloadSourceBase(Uri uri, DownloadSourceCreator creator)
			: this(uri, creator, null)
		{
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(DownloadSourceCreator.CreatorParameterName, null)
			});
		}

		public DownloadSourceCreator Creator
		{
			get { return (DownloadSourceCreator)GetParameterValue(DownloadSourceCreator.CreatorParameterName); }
		}

		public virtual string Name
		{
			get { return Uri.Segments.LastOrDefault(); }
		}

		public override string ToString()
		{
			return String.Format("{0}", Uri);
		}
	}
}
