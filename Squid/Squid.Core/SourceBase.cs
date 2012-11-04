using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squid.Core
{
	public abstract class SourceBase : ISource
	{
		private readonly ParameterList m_parameters;

		protected SourceBase(Uri uri, ParameterList parameters)
			: this(uri)
		{
			if (parameters != null)
				m_parameters.SetParameters(parameters);
		}

		private SourceBase(Uri uri)
		{
			m_parameters = new ParameterList();

			var defaultParams = GetDefaultParameters();
			if (defaultParams != null)
			{
				foreach (Parameter parameter in defaultParams)
				{
					m_parameters.Add(parameter.Name, parameter);
				}
			}

			SetParameterValue(SourceCreatorBase.UriParameterName, uri);
		}

		protected virtual IEnumerable<Parameter> GetDefaultParameters()
		{
			return new Parameter[]
			{
				new Parameter(SourceCreatorBase.UriParameterName, null)
			};
		}

		protected object GetParameterValue(string name)
		{
			return m_parameters.GetValue(name);
		}

		protected void SetParameterValue(string name, object value)
		{
			m_parameters.SetValue(name, value);
		}

		public Uri Uri
		{
			get { return (Uri)GetParameterValue(SourceCreatorBase.UriParameterName); }
		}

		public IEnumerable<Parameter> GetParameters()
		{
			return m_parameters.Values;
		}
	}
}
