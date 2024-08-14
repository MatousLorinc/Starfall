using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Starfall
{
	public static class Log
	{
		public static TextBlock textBlock { get; set; }
		private static readonly StringBuilder ContentOut = new();
		private static readonly List<string> LogMsgs = new();

		public static void Write(string content)
		{
			LogMsgs.Add(content);
		}
		public static void Write(LogMsgType msgType, string content)
		{
			LogMsgs.Add(String.Format("{0} : {1}", msgType, content));
		}
		public static void Print()
		{
			ContentOut.Clear();
			for (int i = LogMsgs.Count; i > 0; i--)
			{
				ContentOut.AppendLine(LogMsgs[i - 1]);
			}
			textBlock.Text = ContentOut.ToString();
		}
		public static void WritePrint(string content)
		{
			Write(content);
			Print();
		}
		public static void WritePrint(LogMsgType msgType, string content)
		{
			Write(msgType, content);
			Print();
		}

		public static void Clear()
		{
			ContentOut.Clear();
			LogMsgs.Clear();
			Print();
		}

		public enum LogMsgType
		{
			Oznameni,
			Varovani,
			Chyba,
			CritickaChyba
		}
	}
}
