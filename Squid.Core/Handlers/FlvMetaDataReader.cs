using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Squid.Core
{
	/// <summary>
	/// Reads the meta information embedded in an FLV file
	/// http://johndyer.name/post/Flash-FLV-meta-reader-in-NET-%28C%29.aspx#id_95a74b1f-352c-4a01-8e1d-8c243e2fe07c
	/// </summary>
	public class FlvMetaDataReader
	{
		static string onMetaData = "";
		static string bytesToFile = "";

		/// <summary>
		/// Reads the meta information (if present) in an FLV
		/// </summary>
		/// <param name="path">The path to the FLV file</returns>
		public static FlvMetaInfo GetFlvMetaInfo(string path)
		{
			if (!File.Exists(path))
				throw new ArgumentException(String.Format("File '{0}' doesn't exist", path));

			bool hasMetaData = false;
			double duration = 0;
			double width = 0;
			double height = 0;
			double videoDataRate = 0;
			double audioDataRate = 0;
			Double frameRate = 0;

			FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				byte[] bytes = new byte[1000];
				fileStream.Seek(27, SeekOrigin.Begin);
				int result = fileStream.Read(bytes, 0, 1000);
				bytesToFile = ByteArrayToString(bytes);
				onMetaData = bytesToFile.Substring(0, 10);

				// if "onMetaData" exists then proceed to read the attributes
				if (onMetaData == "onMetaData")
				{
					hasMetaData = true;
					// 16 bytes past "onMetaData" is the data for "duration"
					duration = GetNextDouble(bytes, bytesToFile.IndexOf("duration") + 9, 8);
					// 8 bytes past "duration" is the data for "width"
					width = GetNextDouble(bytes, bytesToFile.IndexOf("width") + 6, 8);
					// 9 bytes past "width" is the data for "height"
					height = GetNextDouble(bytes, bytesToFile.IndexOf("height") + 7, 8);
					// 16 bytes past "height" is the data for "videoDataRate"
					videoDataRate = GetNextDouble(bytes, bytesToFile.IndexOf("videodatarate") + 14, 8);
					// 16 bytes past "videoDataRate" is the data for "audioDataRate"
					audioDataRate = GetNextDouble(bytes, bytesToFile.IndexOf("audiodatarate") + 14, 8);
					// 12 bytes past "audioDataRate" is the data for "frameRate"
					frameRate = GetNextDouble(bytes, bytesToFile.IndexOf("framerate") + 10, 8);
				}
			}
			catch (Exception e)
			{
				// no error handling
			}
			finally
			{
				fileStream.Close();
			}
			return new FlvMetaInfo(hasMetaData, duration, width, height, videoDataRate, audioDataRate, frameRate);
		}

		private static Double GetNextDouble(Byte[] b, int offset, int length)
		{
			MemoryStream ms = new MemoryStream(b);
			// move the desired number of places in the array
			ms.Seek(offset, SeekOrigin.Current);
			// create byte array
			byte[] bytes = new byte[length];
			// read bytes
			int result = ms.Read(bytes, 0, length);
			// convert to double (all flass values are written in reverse order)
			return ByteArrayToDouble(bytes, true);
		}

		private static string ByteArrayToString(byte[] bytes)
		{
			string byteString = string.Empty;
			foreach (byte b in bytes)
			{
				byteString += Convert.ToChar(b).ToString();
			}
			return byteString;
		}

		private static Double ByteArrayToDouble(byte[] bytes, bool readInReverse)
		{
			if (bytes.Length != 8)
				throw new Exception("bytes must be exactly 8 in Length");
			if (readInReverse)
				Array.Reverse(bytes);
			return BitConverter.ToDouble(bytes, 0);
		}
	}
	/// <summary>
	/// Read only container holding meta data embedded in FLV files
	/// </summary>
	public class FlvMetaInfo
	{
		public Double Duration { get; private set; }
		public Double Width { get; private set; }
		public Double Height { get; private set; }
		public Double FrameRate { get; private set; }
		public Double VideoDataRate { get; private set; }
		public Double AudioDataRate { get; private set; }
		public bool HasMetaData { get; private set; }
		
		public FlvMetaInfo(bool hasMetaData, Double duration, Double width, Double height, Double videoDataRate, Double audioDataRate, Double frameRate)
		{
			HasMetaData = hasMetaData;
			Duration = duration;
			Width = width;
			Height = height;
			VideoDataRate = videoDataRate;
			AudioDataRate = audioDataRate;
			FrameRate = frameRate;
		}
	}
}
