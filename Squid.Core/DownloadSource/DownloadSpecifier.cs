using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;

namespace Squid.Core
{
	public class DownloadSpecifier : SourceBase, IDownloadSpecifier
	{
		private const string SourceParameterName = "Source";
		private const string RemoteFileInfoParameterName = "RemoteFileInfo";

		public DownloadSpecifier(Uri uri, IDownloadSource source)
			: this(uri, source, null)
		{
		}

		public DownloadSpecifier(Uri uri, IDownloadSource source, ParameterList parameters)
			: base(uri, parameters)
		{
			SetParameterValue(SourceParameterName, source);
			
			// this doesn't fit the method of parameterization that I am using
			var info = Source.Creator.GetFileInfo(Uri);
			SetParameterValue(RemoteFileInfoParameterName, info);
		}

		protected override IEnumerable<Parameter> GetDefaultParameters()
		{
			return base.GetDefaultParameters().Concat(new Parameter[] {
				new Parameter(SourceParameterName, null),
				new Parameter(RemoteFileInfoParameterName, null)
			});
		}

		public IDownloadSource Source
		{
			get { return (IDownloadSource)GetParameterValue(SourceParameterName); }
		}

		public RemoteFileInfo RemoteFileInfo
		{
			get { return (RemoteFileInfo)GetParameterValue(RemoteFileInfoParameterName); }
		}

		public string SuggestedFileName
		{
			get { return GetSuggestedFileName(); }
		}

		public static string MakeValidFileName(string name)
		{
			string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			string invalidReStr = string.Format(@"[{0}]+", invalidChars);
			return Regex.Replace(name, invalidReStr, "_");
		}

		public virtual string Key
		{
			get { return Uri.AbsoluteUri; }
		}

		public virtual string Name
		{
			get { return Source.Name; }
		}

		private string GetSuggestedFileName()
		{
			string name = MakeValidFileName(Name);
			string ext = GetExtension();
			if (!string.IsNullOrEmpty(ext))
				name += ext;
			return name;
		}

		public virtual int CompareTo(IDownloadSpecifier specifier)
		{
			return String.CompareOrdinal(Key, specifier.Key);
		}

		protected virtual string GetExtension()
		{
			string ext = Source.Creator.GetRegisteredMediaExtension(RemoteFileInfo.ContentType);
			if (!string.IsNullOrEmpty(ext))
				return ext;
			return GetDefaultExtension();
		}

		private string GetDefaultExtension()
		{
			string mimeType = RemoteFileInfo.ContentType;
			string result = string.Empty;

			if (!string.IsNullOrEmpty(mimeType))
			{
				RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
				if (key != null)
				{
					object value = key.GetValue("Extension", null);
					if (value != null)
						result = value.ToString();
				}
			}

			return result;
		}
	}
}
