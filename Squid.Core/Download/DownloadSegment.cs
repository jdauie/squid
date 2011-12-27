using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Squid.Core
{
	public enum DownloadSegmentState
	{
		RUNNING = 0,
		PAUSED
	}

	public class DownloadSegment : Segment, INotifyPropertyChanged
	{
		public DownloadSpecifier DownloadSpecifier { get; private set; }
		public long CurrentPosition { get; private set; }
		public int Index { get; private set; }
		public DateTime StartTime { get; private set; }

		private long m_rate;
		private DateTime m_lastRateTime;
		private DownloadSegmentState m_state;

		public DownloadSegment(DownloadSpecifier downloadSpecifier, Segment segment, int index)
			: base(segment.StartPosition, segment.EndPosition)
		{
			DownloadSpecifier = downloadSpecifier;
			CurrentPosition = StartPosition;
			Index = index;
			StartTime = DateTime.Now;

			m_lastRateTime = StartTime;
			m_rate = 0;
		}

		public Stream CreateInputStream()
		{
			return DownloadSpecifier.Source.Creator.CreateStream(DownloadSpecifier.Uri, StartPosition, EndPosition);
		}

		public void IncreaseCurrentPosition(int bytesRead)
		{
			// average this byte chunk with the last
			DateTime newRateTime = DateTime.Now;
			TimeSpan thisChunkTime = newRateTime - m_lastRateTime;
			m_lastRateTime = newRateTime;
			
			long currentBytesPerMS = (long)((double)bytesRead / thisChunkTime.TotalMilliseconds);
			double previousWeight = 0.99;
			double currentWeight = 1 - previousWeight;

			lock (this)
			{
				CurrentPosition += bytesRead;
				m_rate = (long)(m_rate * previousWeight + currentBytesPerMS * 1000 * currentWeight);
			}

			RaisePropertyChanged("Progress");
			RaisePropertyChanged("Downloaded");
			RaisePropertyChanged("TimeTaken");
			RaisePropertyChanged("Rate");
		}

		public DownloadSegmentState State
		{
			get
			{
				DownloadSegmentState state;
				lock (this)
				{
					state = m_state;
				}
				return state;
			}
			set
			{
				bool changed = false;
				lock (this)
				{
					if (value != m_state)
						changed = true;
					m_state = value;
				}
				if (changed)
					RaisePropertyChanged("State");
			}
		}

		public bool Completed
		{
			get
			{
				bool completed = false;
				lock (this)
				{
					completed = CurrentPosition >= EndPosition;
				}
				return completed;
			}
		}

		public long Rate
		{
			get
			{
				long rate = 0;
				lock (this)
				{
					rate = m_rate;
				}
				return rate;
			}
		}

		public long Downloaded
		{
			get
			{
				long completed = 0;
				lock (this)
				{
					completed = CurrentPosition - StartPosition;
				}
				return completed;
			}
		}

		public int Progress
		{
			get
			{
				int progress = 0;
				long total = EndPosition - StartPosition;
				if (total > 0)
				{
					lock (this)
					{
						progress = (int)(100 * ((double)CurrentPosition - StartPosition) / total);
					}
				}
				return progress;
			}
		}

		public TimeSpan TimeTaken
		{
			get
			{
				TimeSpan timeSpan = new TimeSpan();
				long total = EndPosition - StartPosition;
				if (total > 0)
				{
					timeSpan = (DateTime.Now - StartTime);
				}
				return timeSpan;
			}
		}

		public override string ToString()
		{
			return String.Format("{0}:{1}", Index, base.ToString());
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
