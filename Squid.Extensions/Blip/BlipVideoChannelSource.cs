using System;
using System.Collections.Generic;

using Squid.Core;

namespace Squid.Extensions.Blip
{
	public class BlipVideoChannelSource : VideoChannelSource
	{
		public BlipVideoChannelSource(Uri uri, ChannelSourceCreator creator, ParameterList parameters)
			: base(uri, creator, parameters)
		{
		}

		public int CurrentPage
		{
			get { return (int)GetParameterValue(BlipVideoChannelSourceCreator.CurrentPageParameterName); }
		}

		public int TotalPages
		{
			get { return (int)GetParameterValue(BlipVideoChannelSourceCreator.TotalPagesParameterName); }
		}

		public string PagingUrlFormat
		{
			get { return (string)GetParameterValue(BlipVideoChannelSourceCreator.PagingUrlFormatParameterName); }
		}
	}
}
