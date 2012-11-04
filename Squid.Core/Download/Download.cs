using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Squid.Core
{
	public class Download : INotifyPropertyChanged
	{
		const int BUFFER_SIZE = 8 * 1024;

		public DownloadSpecifier DownloadSpecifier { get; private set; }
		public string LocalPath { get; private set; }
		public string LocalDirectory { get; private set; }
		public string LocalFileName { get; private set; }
		
		private Task[] m_tasks;

		public ObservableDownloadSegmentCollection DownloadSegments { get; private set; }

		public Download(DownloadSpecifier downloadSpecifier, string localPath)
		{
			DownloadSpecifier = downloadSpecifier;
			LocalPath = localPath;
			LocalDirectory = Path.GetDirectoryName(LocalPath);
			LocalFileName = Path.GetFileName(LocalPath);
			DownloadSegments = new ObservableDownloadSegmentCollection();
		}

		public int Progress
		{
			get
			{
				if (DownloadSegments.Count > 0)
					return (int)DownloadSegments.Average(s => s.Progress);
				return 0;
			}
		}

		public long Rate
		{
			get
			{
				if (DownloadSegments.Count > 0)
					return DownloadSegments.Sum(s => s.Rate);
				return 0;
			}
		}

		public long Downloaded
		{
			get
			{
				if (DownloadSegments.Count > 0)
					return DownloadSegments.Sum(s => s.Downloaded);
				return 0;
			}
		}

		private void AllocateLocalFile()
		{
			string localDirectory = Path.GetDirectoryName(LocalPath);

			if (!Directory.Exists(localDirectory))
			{
				Directory.CreateDirectory(localDirectory);
			}

			if (File.Exists(LocalPath))
			{
				File.Delete(LocalPath);
			}

			using (var fs = new FileStream(LocalPath, FileMode.Create, FileAccess.Write))
			{
				fs.SetLength(Math.Max(DownloadSpecifier.RemoteFileInfo.FileSize, 0));
			}
		}

		public void Start()
		{
			int segmentCount = 4;

			foreach(DownloadSegment segment in GetDownloadSegments(segmentCount, DownloadSpecifier))
				DownloadSegments.Add(segment);

			AllocateLocalFile();

			using (var fs = new FileStream(LocalPath, FileMode.Open, FileAccess.Write))
			{
				m_tasks = DownloadSegments.Select(s => new Task(() => BackgroundDownloadSegmentStart(s, fs))).ToArray();
				Parallel.ForEach(m_tasks, t => t.Start());
				
				Task.WaitAll(m_tasks.ToArray());
			}
		}

		public void Stop()
		{
			foreach (var segment in DownloadSegments)
			{
				segment.State = DownloadSegmentState.PAUSED;
			}
		}

		private IEnumerable<DownloadSegment> GetDownloadSegments(int segments, DownloadSpecifier downloadSpecifier)
		{
			IEnumerable<Segment> calculatedSegments = GetSegments(segments, downloadSpecifier.RemoteFileInfo);
			return calculatedSegments.Select((s, idx) => new DownloadSegment(downloadSpecifier, s, idx + 1));
		}

		private IEnumerable<Segment> GetSegments(int segmentCount, RemoteFileInfo remoteFileInfo)
		{
			long fileSize = remoteFileInfo.FileSize;
			long segmentSize = fileSize / segmentCount;
			var segments = new List<Segment>();
			long segmentStart = 0;
			while (segmentStart < fileSize)
			{
				long segmentEnd = Math.Min(fileSize, segmentStart + segmentSize);
				if (segments.Count == segmentCount - 1)
					segmentEnd = fileSize;
				segments.Add(new Segment(segmentStart, segmentEnd));
				segmentStart = segmentEnd;
			}
			return segments;
		}

		private void BackgroundDownloadSegmentStart(DownloadSegment segment, Stream outputStream)
		{
			using (Stream inputStream = segment.CreateInputStream())
			{
				byte[] buffer = new byte[BUFFER_SIZE];

				while (!segment.Completed && segment.State == DownloadSegmentState.RUNNING)
				{
					long bytesRead = inputStream.Read(buffer, 0, BUFFER_SIZE);

					if (segment.CurrentPosition + bytesRead > segment.EndPosition)
					{
						bytesRead = (segment.EndPosition - segment.CurrentPosition);
					}

					lock (outputStream)
					{
						outputStream.Position = segment.CurrentPosition;
						outputStream.Write(buffer, 0, (int)bytesRead);
					}

					segment.IncreaseCurrentPosition((int)bytesRead);

					RaisePropertyChanged("Progress");
					RaisePropertyChanged("Downloaded");
					RaisePropertyChanged("Rate");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
