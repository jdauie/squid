using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Squid.Core
{
	public class ObservableDownloadSpecifierCollection : ObservableCollection<IDownloadSpecifier>
	{
		private bool m_sorting = false;

		public void Sort()
		{
			var sorted = new SortedSet<IDownloadSpecifier>(this);
			Clear();
			foreach (IDownloadSpecifier spec in sorted)
				Add(spec);
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			if (!m_sorting)
			{
				m_sorting = true;
				Sort();
				m_sorting = false;
			}
		}
	}
}
