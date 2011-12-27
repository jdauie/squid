using System;
using System.Linq;

using Squid.Core;
using System.Collections.Generic;

namespace Squid.Extensions.YouTube
{
	class YoutubeVideoDownloadSpecifier : DownloadSpecifier
	{
		public const string FmtParameterName = "Fmt";
		public const string ResolutionParameterName = "Resolution";
		public const string MaxPixelHeightParameterName = "MaxPixelHeight";

		public YoutubeVideoDownloadSpecifier(Uri uri, IDownloadSource source, ParameterList parameters)
			: base(uri, source, parameters)
		{
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(FmtParameterName, 0),
				new Parameter(ResolutionParameterName, string.Empty),
				new Parameter(MaxPixelHeightParameterName, 0),
			});
		}

		public int Fmt
		{
			get { return (int)GetParameterValue(FmtParameterName); }
		}

		public string Resolution
		{
			get { return (string)GetParameterValue(ResolutionParameterName); }
		}

		public int MaxPixelHeight
		{
			get { return (int)GetParameterValue(MaxPixelHeightParameterName); }
		}

		public override string Key
		{
			get { return Fmt.ToString(); }
		}

		public override string Name
		{
			get { return String.Format("{0} {1}p", base.Name, MaxPixelHeight); }
		}

		public override int CompareTo(IDownloadSpecifier specifier)
		{
			YoutubeVideoDownloadSpecifier localSpecifier = specifier as YoutubeVideoDownloadSpecifier;
			if (localSpecifier != null)
			{
				int heightCompare = MaxPixelHeight.CompareTo(localSpecifier.MaxPixelHeight);
				if (heightCompare != 0)
					return heightCompare;
				return Fmt.CompareTo(localSpecifier.Fmt);
			}
			return base.CompareTo(specifier);
		}

		public override string ToString()
		{
			return String.Format("{0}, {1}p, {2}", Fmt, MaxPixelHeight, RemoteFileInfo);
		}
	}
}
