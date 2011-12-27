using System;
using System.Collections.Generic;
using System.Linq;

using Squid.Core;

namespace Squid.Extensions.Blip
{
	class BlipVideoDownloadSpecifier : DownloadSpecifier
	{
		public const string FormatNameParameterName = "FormatName";
		
		public BlipVideoDownloadSpecifier(Uri uri, IDownloadSource downloadSource, ParameterList parameters)
			: base(uri, downloadSource, parameters)
		{
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(FormatNameParameterName, string.Empty)
			});
		}

		public string FormatName
		{
			get { return (string)GetParameterValue(FormatNameParameterName); }
		}

		public override string Key
		{
			get
			{
				return FormatName;
			}
		}

		public override string ToString()
		{
			return String.Format("{0}", FormatName);
		}
	}
}
