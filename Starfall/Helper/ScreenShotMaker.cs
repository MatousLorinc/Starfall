using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starfall.Helper
{
	/// <summary>
	/// This class requires installed ffmpeg on your system
	/// winget install ffmpeg
	/// https://www.gyan.dev/ffmpeg/builds/
	/// ffmpeg -i testvideo.mp4 -ss 00:00:15 -vframes 1 output_framee.png
	/// </summary>
	static class ScreenShotMaker
  {
		public static void TakeScreenShot(string inputVideo, string outputScreenshot, string timeStamp = "00:00:00")
		{
			//string inputVideo = "JOS1_1080p.mp4";
			//string outputFrame = "output_frame.png";
			string ffmpegPath = @"C:\Users\Poseidon\AppData\Local\Microsoft\WinGet\Packages\Gyan.FFmpeg_Microsoft.Winget.Source_8wekyb3d8bbwe\ffmpeg-7.0-full_build\bin\ffmpeg";

			// -y confirms override question
			string arguments = $"-y -ss \"{timeStamp}\" -i \"{inputVideo}\" -vframes 1 \"{outputScreenshot}\"";

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = ffmpegPath,
				Arguments = arguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using (Process process = new Process())
			{
				process.StartInfo = startInfo;

				//StringBuilder outputBuilder = new StringBuilder();
				//StringBuilder errorBuilder = new StringBuilder();

				//process.OutputDataReceived += (sender, e) =>
				//{
				//	if (!string.IsNullOrEmpty(e.Data))
				//	{
				//		outputBuilder.AppendLine(e.Data);
				//	}
				//};

				//process.ErrorDataReceived += (sender, e) =>
				//{
				//	if (!string.IsNullOrEmpty(e.Data))
				//	{
				//		errorBuilder.AppendLine(e.Data);
				//	}
				//};

				try { process.Start(); }
				catch { }
				
				//process.BeginOutputReadLine();
				//process.BeginErrorReadLine();

				//process.WaitForExit();

				//string output = outputBuilder.ToString();
				//string error = errorBuilder.ToString();

				//if (process.ExitCode == 0)
				//{
				//	Console.WriteLine("Frame extracted successfully.");
				//}
				//else
				//{
				//	Console.WriteLine($"Error extracting frame. Exit code: {process.ExitCode}");
				//	Console.WriteLine($"Output: {output}");
				//	Console.WriteLine($"Error: {error}");
				//}
			}
		}
		public enum TimestampOptions
		{
			PlusMinuteFromStart,
			Middle,
			MinusMinuteFromEnd
		}

		public static string GetModifiedTimestamp(string timestamp, TimestampOptions timestampOption)
		{
			try
			{
				TimeSpan originalTime = TimeSpan.Parse(timestamp);
				if (originalTime.Hours == 0 && originalTime.Minutes < 1)
					return timestamp;
				TimeSpan minuteTimestamp = new TimeSpan(0, 1, 0);
				switch (timestampOption)
				{
					case TimestampOptions.PlusMinuteFromStart:
						return minuteTimestamp.ToString();
					case TimestampOptions.Middle:
						return originalTime.Divide(2).ToString();
					case TimestampOptions.MinusMinuteFromEnd:
					{
						return (originalTime - minuteTimestamp).ToString();
					}
					default:
						return timestamp;
				}
			}
			catch { return timestamp; }
		}
	}
}
