using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squid.Core
{
	public class Parameter
	{
		public string Name { get; private set; }
		public object Value { get; set; }

		public Parameter(string name, object value)
		{
			Name = name;
			Value = value;
		}

		public override string ToString()
		{
			return (Value != null ? Value.ToString() : "null");
		}
	}
}
