using System;
using System.Collections.Generic;
using System.Linq;

namespace Squid.Core
{
	public class Segment
	{
		public long StartPosition { get; private set; }
		public long EndPosition { get; private set; }

		public long Length
		{
			get
			{
				return EndPosition - StartPosition;
			}
		}

		public Segment(long startPosition, long endPosition)
		{
			StartPosition = startPosition;
			EndPosition = endPosition;
		}

		public override string ToString()
		{
			return String.Format("({0}, {1}) => {2}", StartPosition, EndPosition, (EndPosition - StartPosition).ToSize());
		}
	}
}
