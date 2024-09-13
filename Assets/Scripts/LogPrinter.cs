using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class LogPrinter : MonoBehaviour
{
	public LogMessage.LogType catchLog;
	Queue<LogMessage> _threadedLogs = new Queue<LogMessage>();
	static StringBuilder _stb = new StringBuilder();

#if UNITY_EDITOR
	static string path = $"{Application.dataPath}/Log_{DateTime.Now.ToString("yyMMdd_hhmmss")}.txt";
#else
	static string path = $"{Application.persistentDataPath}/Log_{DateTime.Now.ToString("yyMMdd_hhmmss")}.txt";
#endif

	private void Start()
	{
		//catchLog = LogMessage.LogType.Assert | LogMessage.LogType.Error | LogMessage.LogType.Exception | LogMessage.LogType.Log;
		Application.logMessageReceivedThreaded += CaptureLogThread;

		CreateText();
	}

	void CreateText()
	{

#if UNITY_EDITOR
		if (!Directory.Exists(Application.dataPath)) {
			Directory.CreateDirectory(Application.dataPath);
		}

		if (!Directory.Exists(Application.dataPath + "/Log")) {
			Directory.CreateDirectory(Application.dataPath + "/Log");
		}
#endif
		using (StreamWriter sw = new StreamWriter(path)) {
			sw.WriteLine("Log Start");
		}
	}

	public static void SaveLog()
	{
#if !UNITY_EDITOR
		string baseMessage = "";
		using (StreamReader sw = new StreamReader(path)) {
			baseMessage = sw.ReadToEnd();
		}

		using (StreamWriter sw = new StreamWriter(path)) {
			sw.WriteLine(baseMessage);
			sw.WriteLine(_stb.ToString());
		}
#endif
	}

	void CaptureLogThread(string condition, string stacktrace, LogType type)
	{
		LogMessage log = new LogMessage() { condition = condition, stacktrace = stacktrace };
		log.SetLogType(type);
		lock (_threadedLogs) {
			if (catchLog.HasFlag(log.logType)) {
				_stb.AppendLine(log.ToString() + "\n");
				FastSave(log);
			}
		}
	}

	private void FastSave(LogMessage log)
	{
		switch (log.logType) {
			case LogMessage.LogType.Log:
				break;
			case LogMessage.LogType.Assert:
			case LogMessage.LogType.Error:
			case LogMessage.LogType.Exception:
			case LogMessage.LogType.Warning:
				SaveLog();
				break;
		}
	}

	public class LogMessage
	{
		[Flags]
		public enum LogType
		{
			Assert = 1 << 0,
			Error = 1 << 1,
			Exception = 1 << 2,
			Log = 1 << 3,
			Warning = 1 << 4
		}

		public int count = 1;
		public LogType logType;
		public string condition;
		public string stacktrace;
		public int sampleId;

		public LogMessage CreateCopy()
		{
			return (LogMessage)this.MemberwiseClone();
		}

		public void SetLogType(UnityEngine.LogType type)
		{
			switch (type) {
				case UnityEngine.LogType.Error:
					logType = LogType.Error;
					break;
				case UnityEngine.LogType.Assert:
					logType = LogType.Assert;
					break;
				case UnityEngine.LogType.Warning:
					logType = LogType.Warning;
					break;
				case UnityEngine.LogType.Log:
					logType = LogType.Log;
					break;
				case UnityEngine.LogType.Exception:
					logType = LogType.Exception;
					break;
			}
		}

		public float GetMemoryUsage()
		{
			return (float)(sizeof(int) +
					sizeof(LogType) +
					condition.Length * sizeof(char) +
					stacktrace.Length * sizeof(char) +
					sizeof(int));
		}

		public override string ToString()
		{
			return $"{logType}] {condition}\n{stacktrace}";
		}
	}

}
