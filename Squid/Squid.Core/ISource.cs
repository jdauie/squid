using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squid.Core
{

	public interface ISource
	{
		Uri Uri { get; }

		IEnumerable<Parameter> GetParameters();
	}
}
