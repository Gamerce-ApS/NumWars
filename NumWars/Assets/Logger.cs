using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class Logger : MonoBehaviour
{
	static public System.Action onMemoryWarning;
	static public Logger ourInstance;
	static bool IsRetina()
	{
		return Screen.dpi > 180;
	}
	
	public enum ConsoleLogTrigger //should use flags instead?
	{
		Never = 0,
		ErrorsAndExceptions,
		WarningsErrorsAndExceptions,
		Always
	}
	public class Log
	{
		public string message;
		public string stackTrace;
		public LogType type;

		public Log()
		{
		}

		public Log(string message, string stackTrace, LogType type)
		{
			this.message = message;
			this.stackTrace = stackTrace;
			this.type = type;
		}
	}
	
	/// <summary>
	/// The hotkey to show and hide the console window.
	/// </summary>
	public KeyCode toggleKey = KeyCode.BackQuote;
	
	public List<Log> _logs = new List<Log>();
	private Vector2 _scrollPosition;
	public bool show;
	private bool _collapse = true;
	private bool _showStacktraces = true;
	
	// Visual elements:
	
	private static readonly Dictionary<LogType, Color> _logTypeColors = new Dictionary<LogType, Color>()
	{
		{LogType.Assert, Color.white},
		{LogType.Error, Color.red},
		{LogType.Exception, Color.red},
		{LogType.Log, Color.white},
		{LogType.Warning, Color.yellow},
	};
	
	private const int MARGIN = 20;
	private const int HALFMARGIN = 10;
	
	private Rect _windowRect = new Rect(MARGIN, MARGIN, Screen.width - (MARGIN * 2), Screen.height - (MARGIN * 2));
	private readonly Rect _titleBarRect = new Rect(0, 0, 10000, 20);
	private readonly GUIContent _hideLabel = new GUIContent("Hide", "Hide the console window");
	private readonly GUIContent _minimiseLabel = new GUIContent("Minimise", "Minimise the console window");
	private readonly GUIContent _clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	private readonly GUIContent _collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
	private readonly GUIContent _showStacktracesLabel = new GUIContent("Stacktrace", "Shows stacktraces.");
	
	public ConsoleLogTrigger consoleLogTrigger { get; set; }
	private bool hasAttached = false;

	public bool showStoredLogs= false;
	public void ShowLogs()
    {
		showStoredLogs = !showStoredLogs;

	}

	void Start()
	{
		ourInstance = this;
		hasAttached = true;
		Application.logMessageReceived += HandleLog;
	}

	private void OnEnable()
	{
		if(hasAttached)
			Application.logMessageReceived -= HandleLog;

		Application.logMessageReceived += HandleLog;
	}

	private void OnDisable()
	{
		hasAttached = false;
		Application.logMessageReceived -= HandleLog;
	}

	private void Update()
	{
		if(Input.GetKeyDown(toggleKey))
		{
			show = !show;
		}
	}

	//#if !UNITY_EDITOR
	private void OnGUI()
	{
		if(showStoredLogs)
        {
			ConsoleWindowStoredLogs();

		}



		if(!show)
		{
			return;
		}
		try
		{
			if (IsRetina())
			{
				const int scale = 2;
				GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));
				_windowRect = new Rect(HALFMARGIN, HALFMARGIN, Screen.width/scale - (MARGIN), Screen.height/scale - (MARGIN));
			}
			_windowRect = GUILayout.Window(123456, _windowRect, ConsoleWindow, "Console");
		}
		catch(Exception)
		{
			// ignored
		}
	}
	//#endif
	/// <summary>
	/// A window that displayss the recorded logs.
	/// </summary>
	/// <param name="windowID">Window ID.</param>
	public void ConsoleWindow(int windowID)
	{
		ConsoleView(true);
		
		// Allow the window to be dragged by its title bar.
		GUI.DragWindow(_titleBarRect);
	}
	public void ConsoleWindowStoredLogs()
    {
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
		{
			
				// Iterate through the recorded logs.
				
					try
					{
						GUILayout.TextArea(PlayerPrefs.GetString("Logs", ""));
					}
					catch (Exception)
					{
						// ignored
					}
				
			
		}
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal();
		try
		{
			if (GUILayout.Button(_clearLabel, GUILayout.Height(300)))
			{
				PlayerPrefs.DeleteKey("Logs");
			}

			if (GUILayout.Button(_hideLabel, GUILayout.Height(300)))
			{
				showStoredLogs = false;
			}
		}
		catch (Exception)
		{
			// ignored
		}
		GUILayout.EndHorizontal();
	}
	public void ConsoleView(bool showHideButton)
	{
		_scrollPosition = GUILayout.BeginScrollView (_scrollPosition);
		{
			lock(_logs)
			{
				// Iterate through the recorded logs.
				for(int i = 0; i < _logs.Count; i++)
				{
					try
					{
						var log = _logs[i];
						
						// Combine identical messages if collapse option is chosen.
						if(_collapse)
						{
							var messageSameAsPrevious = i > 0 && log.message == _logs[i - 1].message;
							
							if(messageSameAsPrevious)
							{
								continue;
							}
						}
						Color oldColor = GUI.color;
						GUI.color = _logTypeColors[log.type];
						{
							if (!string.IsNullOrEmpty(log.message))
							{
								GUILayout.Label(log.message);
							}
							if(_showStacktraces && !string.IsNullOrEmpty(log.stackTrace))
							{
								GUILayout.Label(log.stackTrace);
							}
						}
						GUI.color = oldColor;
					}
					catch(Exception)
					{
						// ignored
					}
				}
			}
		}
		GUILayout.EndScrollView ();
		GUILayout.BeginHorizontal();
		try
		{
			if(GUILayout.Button(_clearLabel))
			{
				lock(_logs)
				{
					_logs.Clear();
				}
			}
			if(showHideButton)
			{
				if(GUILayout.Button(_hideLabel))
				{
					show = false;
				}
			}
			if(GUILayout.Button(_minimiseLabel))
			{
				show = !show;
			}
			_collapse = GUILayout.Toggle(_collapse, _collapseLabel, GUILayout.ExpandWidth(false));
			_showStacktraces = GUILayout.Toggle(_showStacktraces, _showStacktracesLabel, GUILayout.ExpandWidth(false));
		}
		catch(Exception)
		{
			// ignored
		}
		GUILayout.EndHorizontal();
	}
	
	/// <summary>
	/// Records a log from the log callback.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="stackTrace">Trace of where the message came from.</param>
	/// <param name="type">Type of message (error, exception, warning, assert).</param>
	private void HandleLog(string message, string stackTrace, LogType type)
	{
		//switch (consoleLogTrigger)
		//{
		//case ConsoleLogTrigger.Always:
		//	break;
		//case ConsoleLogTrigger.ErrorsAndExceptions:
		//	if (type == LogType.Error || type == LogType.Exception)
		//	{
		//		break;
		//	}
		//	return;
		//case ConsoleLogTrigger.WarningsErrorsAndExceptions:
		//	if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
		//	{
		//		break;
		//	}
		//	return;
		//case ConsoleLogTrigger.Never:
		//	return;
		//default:
		//	return;
		//}
		//
		//if (!show)
		//{
		//	show = (type != LogType.Log); // Don't force a show on regular Logs. 
		//}
		bool invokeWarning = false;
		lock (_logs)
		{
			if(message.ToLower().Contains("memory") && (type != LogType.Log))
			{
				invokeWarning = true;
				_logs.Add(new Log("Cleared memory", "", LogType.Warning));
			}
			
			//if(type != LogType.Log && type != LogType.Warning)
				if(type == LogType.Exception || type == LogType.Error)
				show = true;

			_logs.Add(new Log
			          {
				message = message,
				stackTrace = stackTrace,
				type = type,
			});

	

			if (type == LogType.Error || type == LogType.Exception)
            {
				string all = PlayerPrefs.GetString("Logs", "");


					all += "type: " +type.ToString() + " message:" +message + " stackTrace:" + stackTrace;
				PlayerPrefs.SetString("Logs", all);
			}
		
		}
		if(invokeWarning && onMemoryWarning != null){
			onMemoryWarning.Invoke();
			_logs.Add(new Log("Callback Invoked", "", LogType.Warning));
		}
	}
}