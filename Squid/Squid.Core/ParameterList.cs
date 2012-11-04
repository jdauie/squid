using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squid.Core
{
	public class ParameterList : SortedList<string, Parameter>
	{
		public ParameterList()
		{
		}

		public void SetParameters(ParameterList parameters)
		{
			if (parameters != null)
			{
				foreach (KeyValuePair<string, Parameter> kvp in parameters)
				{
					if (ContainsKey(kvp.Key))
					{
						SetValue(kvp.Key, kvp.Value.Value);
					}
					else
					{
						Add(kvp.Key, kvp.Value);
					}
				}
			}
		}

		public object GetValue(string name)
		{
			return this[name].Value;
		}

		public void AddValue(string name, object value)
		{
			var parameter = new Parameter(name, value);
			Add(name, parameter);
		}

		public void SetValue(string name, object value)
		{
			this[name].Value = value;
		}
	}
}
