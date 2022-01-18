using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class AutomatedBuildWindow : EditorWindow
{
	private string _assetSavePath;
	private string _assetName;
	private static AutomatedBuildSettings _automatedBuildSettingsData;
	private const string SETTINGS_DATA_PATH = "Assets/Resources/";
	private const string SETTINGS_FILE_NAME = "AutomatedBuildSettings";
	private const string SETTINGS_RESOURCE_EXTENSION = ".asset";
	private List<string> _logOutput = new List<string>();
	private Vector2 _logScrollPos = Vector2.zero;
	private ReorderableList _logConsoleList;
	private string _buildLogFileName = string.Empty;
	private int selectedTab = 0;

	public const string DEBUG_CONFIG_ASSET_PATH = "Assets/BuildConfigurations/Debug.asset";
	public const string RELEASE_CONFIG_ASSET_PATH = "Assets/BuildConfigurations/Release.asset";

	public enum GradleTask
	{
		Assemble,
		Build,
		Check,
		Clean
	}

	[MenuItem("Kiloo/Auto Build/Android")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		var window = (AutomatedBuildWindow)GetWindow(typeof(AutomatedBuildWindow));
		window.Show();
	}


	// ReSharper disable once InconsistentNaming
	void OnGUI()
	{
		selectedTab = Tabs(new[] { "Android", "Ios" }, selectedTab);

		if (selectedTab == 0)
		{
			DrawAndroidSettings();

		}
		else if (selectedTab == 1)
		{
			DrawIosSettings();
		}
	}

	private void DrawIosSettings()
	{
		EditorGUILayout.Space();
		FindSettingsResourceFile(ref _automatedBuildSettingsData);
		{
			EditorGUILayout.LabelField("Build settings", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("Box");

			#region projectExportPath

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.iosProjectExportPath = EditorGUILayout.TextField("Project export path:", _automatedBuildSettingsData.iosProjectExportPath);
			if (GUILayout.Button("Browse", GUILayout.Width(100)))
			{
				var path = EditorUtility.OpenFolderPanel("Select project export path", Application.dataPath, "");
				_automatedBuildSettingsData.iosProjectExportPath = ConvertFromAbsoluteToRelativePath(path);
				GUI.FocusControl(string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			#endregion

			#region gradle build file path

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.iosGradleRootFolder = EditorGUILayout.TextField("Gradle root folder:", _automatedBuildSettingsData.iosGradleRootFolder);
			if (GUILayout.Button("Browse", GUILayout.Width(100)))
			{
				var path = EditorUtility.OpenFolderPanel("Select gradle root folder", Application.dataPath, "");
				_automatedBuildSettingsData.iosGradleRootFolder = ConvertFromAbsoluteToRelativePath(path);
				GUI.FocusControl(string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			#endregion

			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();

			EditorGUILayout.Space();

			if (GUILayout.Button("Build", GUILayout.Width(100), GUILayout.Height(30)))
			{
				SaveSettings();
				IosBuildAutomated();
			}

			EditorGUILayout.Space();
		}
	}

	private void DrawAndroidSettings()
	{
		EditorGUILayout.Space();
		FindSettingsResourceFile(ref _automatedBuildSettingsData);
		{
			EditorGUILayout.LabelField("Android signing credentials", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("Box");

			#region keystorename

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.keyStorePath = EditorGUILayout.TextField("Keystore path:",
				_automatedBuildSettingsData.keyStorePath);
			if (GUILayout.Button("Browse keystore", GUILayout.Width(150)))
			{
				var path = EditorUtility.OpenFilePanel("Select keystore path", Application.dataPath, "");
				_automatedBuildSettingsData.keyStorePath = ConvertFromAbsoluteToRelativePath(path);
				GUI.FocusControl(string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			#endregion

			#region keystorepassword

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.keyStorePassword = EditorGUILayout.PasswordField("Keystore password:",
				_automatedBuildSettingsData.keyStorePassword);
			EditorGUILayout.EndHorizontal();

			#endregion

			#region keyaliasname

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.keyAliasName = EditorGUILayout.TextField("Keyalias name:",
				_automatedBuildSettingsData.keyAliasName);
			EditorGUILayout.EndHorizontal();

			#endregion

			#region keyaliaspassword

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.keyAliasPassword = EditorGUILayout.PasswordField("Keyalias password:",
				_automatedBuildSettingsData.keyAliasPassword);
			EditorGUILayout.EndHorizontal();

			#endregion

			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();

			EditorGUILayout.LabelField("Build settings", EditorStyles.boldLabel);
			EditorGUILayout.BeginVertical("Box");

			#region projectExportPath

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.projectExportPath = EditorGUILayout.TextField("Project export path:",
				_automatedBuildSettingsData.projectExportPath);
			if (GUILayout.Button("Browse", GUILayout.Width(100)))
			{
				var path = EditorUtility.OpenFolderPanel("Select project export path", Application.dataPath, "");
				_automatedBuildSettingsData.projectExportPath = ConvertFromAbsoluteToRelativePath(path);
				GUI.FocusControl(string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			#endregion

			#region gradle build file path

			EditorGUILayout.BeginHorizontal();
			_automatedBuildSettingsData.gradleRootFolder = EditorGUILayout.TextField("Gradle root folder:",
				_automatedBuildSettingsData.gradleRootFolder);
			if (GUILayout.Button("Browse", GUILayout.Width(100)))
			{
				var path = EditorUtility.OpenFolderPanel("Select gradle root folder", Application.dataPath, "");
				_automatedBuildSettingsData.gradleRootFolder = ConvertFromAbsoluteToRelativePath(path);
				GUI.FocusControl(string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			#endregion

			_automatedBuildSettingsData.useApkExpansionFiles = EditorGUILayout.Toggle("Split application binary:",
				_automatedBuildSettingsData.useApkExpansionFiles);

			EditorGUILayout.EndVertical();

			EditorGUILayout.Separator();

			if (GUILayout.Button("Start Gradle task"))
			{
				SaveSettings();
				StartGradleTask();
			}

			EditorGUILayout.LabelField("Log file:", _buildLogFileName, "Box");

			EditorGUILayout.Space();
			_logScrollPos = GUILayout.BeginScrollView(_logScrollPos);
			//using (var scroll = new GUIScrollView(_logScrollPos))
			{
				_logConsoleList.DoLayoutList();
			}

			GUILayout.EndScrollView();
			//using (new GUIHorizontal())
			{
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("Build", GUILayout.Width(100), GUILayout.Height(30)))
				{
					EditorApplication.delayCall += () =>
					{
						SaveSettings();
						AndroidBuildAutomated();
					};
				}

				if (GUILayout.Button("Open Android Project Folder", GUILayout.Width(200), GUILayout.Height(30)))
				{
					var parsedProjectExportPath = ConvertFromRelativeToAbsolute(_automatedBuildSettingsData.projectExportPath);
					var projectFolder = parsedProjectExportPath + Path.DirectorySeparatorChar + PlayerSettings.productName;
					if (Directory.Exists(projectFolder))
					{
						Process.Start(@"" + projectFolder);
					}
				}

				if (GUILayout.Button("Open Build Folder", GUILayout.Width(200), GUILayout.Height(30)))
				{
					var parsedProjectExportPath = ConvertFromRelativeToAbsolute(_automatedBuildSettingsData.projectExportPath);
					var projectBuildFolder = parsedProjectExportPath + Path.DirectorySeparatorChar + PlayerSettings.productName + Path.DirectorySeparatorChar + "build" + Path.DirectorySeparatorChar + "outputs" + Path.DirectorySeparatorChar + "apk";
					if (Directory.Exists(projectBuildFolder))
					{
						Process.Start(@"" + projectBuildFolder);
					}
				}

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();
			}
		}
	}

	void CreateSettingsFile()
	{
		_assetSavePath = SETTINGS_DATA_PATH + SETTINGS_FILE_NAME + SETTINGS_RESOURCE_EXTENSION;
		_assetName = SETTINGS_FILE_NAME;
		MethodInfo method = typeof(AutomatedBuildWindow).GetMethod("CreateAsset");
		MethodInfo generic = method.MakeGenericMethod(typeof(AutomatedBuildSettings));
		generic.Invoke(this, new object[] { _assetName });

	}

	/// <summary>
	//	This makes it easy to create, name and place unique new ScriptableObject asset files.
	/// </summary>

	public void CreateAsset<T>(string assetName) where T : ScriptableObject
	{
		var asset = CreateInstance<T>();

		var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(_assetSavePath);

		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();
	}


	void OnDisable()
	{
		SaveSettings();
	}


	void OnDestroy()
	{
		SaveSettings();
	}

	void SaveSettings()
	{
		EditorUtility.SetDirty(_automatedBuildSettingsData);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}


	void OnEnable()
	{
		//CreateSettingsFile();
		//try to search for a settings file, otherwise just create a new one
		LoadAutonatedBuildSettings();
		_logConsoleList = new ReorderableList(_logOutput, typeof(string), false, false, false, false);

		AutomatedBuildSettings buildSettings = null;
		if (FindSettingsResourceFile(ref buildSettings))
		{
			CreaetLogs(ConvertFromRelativeToAbsolute(buildSettings.projectExportPath + Path.DirectorySeparatorChar + PlayerSettings.productName));
		}
	}

	private void CreaetLogs(string workingDirectory)
	{
		if (Directory.Exists(workingDirectory + Path.DirectorySeparatorChar + "build" + Path.DirectorySeparatorChar + "logs"))
		{
			var directory = new DirectoryInfo(workingDirectory + Path.DirectorySeparatorChar + "build" + Path.DirectorySeparatorChar + "logs");
			var logFile = (from f in directory.GetFiles() orderby f.LastAccessTime descending select f).First();


			if (logFile != null)
			{
				_logOutput = File.ReadAllLines(logFile.FullName).ToList();
				_buildLogFileName = logFile.FullName;
				_logConsoleList = new ReorderableList(_logOutput, typeof(string), false, false, false, false);
				_logConsoleList.drawElementCallback = (rect, index, active, focused) =>
				{
					var style = new GUIStyle(EditorStyles.textField)
					{
						fontStyle = FontStyle.Normal
					};
					var element = (string)_logConsoleList.list[index];

					if (element.Contains("BUILD SUCCESSFUL"))
					{
						EditorGUI.DrawRect(rect, Color.green);
						style = new GUIStyle(EditorStyles.textField)
						{
							fontStyle = FontStyle.Bold
						};
					}
					else if (element.Contains("BUILD FAILED"))
					{
						EditorGUI.DrawRect(rect, Color.red);
						style = new GUIStyle(EditorStyles.textField)
						{
							fontStyle = FontStyle.Bold
						};
					}

					EditorGUI.LabelField(rect, element, style);
				};

				if (_logOutput.Exists(s => s.Contains("BUILD SUCCESSFUL")))
				{
					var line = _logOutput.Find(s => s.Contains("BUILD SUCCESSFUL"));
					_logConsoleList.index = _logOutput.IndexOf(line);
					_logScrollPos = new Vector2(0, _logConsoleList.index * _logConsoleList.elementHeight);

				}
				else if (_logOutput.Exists(s => s.Contains("BUILD FAILED")))
				{
					var line = _logOutput.Find(s => s.Contains("BUILD FAILED"));
					_logConsoleList.index = _logOutput.IndexOf(line);
				}
			}
		}
	}

	void LoadAutonatedBuildSettings()
	{
		if (!FindSettingsResourceFile(ref _automatedBuildSettingsData))
		{
			CreateSettingsFile();
			FindSettingsResourceFile(ref _automatedBuildSettingsData);
		}
	}

	static bool FindSettingsResourceFile(ref AutomatedBuildSettings buildSettings)
	{
		//we already have a cached version of the build settings in the tool
		if (buildSettings != null) return true;

		//trying to find build settings
		var settingsFilePathGuids = AssetDatabase.FindAssets(SETTINGS_FILE_NAME).ToList();
		var settingsFilePathGuid = string.Empty;
		foreach (var filePathGuid in from filePathGuid in settingsFilePathGuids let filePath = AssetDatabase.GUIDToAssetPath(filePathGuid) where filePath.Contains(".asset") select filePathGuid)
		{
			settingsFilePathGuid = filePathGuid;
		}

		if (string.IsNullOrEmpty(settingsFilePathGuid)) return false;
		buildSettings = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(settingsFilePathGuid), typeof(AutomatedBuildSettings)) as AutomatedBuildSettings;
		return true;
	}

	#region methods to handle relative paths

	private static string ConvertFromAbsoluteToRelativePath(string filePath)
	{
		var absolutePath = filePath;
		var relativeReference = Application.dataPath;

		var relativePath = string.Empty;

		absolutePath = absolutePath.Replace("\\" + "", Path.DirectorySeparatorChar + "");
		relativeReference = relativeReference.Replace("\\" + "", Path.DirectorySeparatorChar + "");

		absolutePath = absolutePath.Replace("//" + "", Path.DirectorySeparatorChar + "");
		relativeReference = relativeReference.Replace("//" + "", Path.DirectorySeparatorChar + "");

		absolutePath = absolutePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		relativeReference = relativeReference.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

		var absolutePathFolderList = absolutePath.Split(Path.DirectorySeparatorChar);
		var relativeReferenceFolderList = relativeReference.Split(Path.DirectorySeparatorChar);

		if (absolutePath.Contains(relativeReference))
		{
			//this is a subfolder of the Assets folder
			relativePath = absolutePath.Replace(relativeReference, "");

			//Debug.Log(relativePath);

			return relativePath;
		}

		//this is not a subfolder of the Assets folder
		var startRelativeDepth = 0;
		for (var i = 0; i < absolutePathFolderList.Count(); i++)
		{
			if (String.Compare(absolutePathFolderList[i], relativeReferenceFolderList[i], StringComparison.Ordinal) != 0)
			{
				startRelativeDepth = i;
				break;
			}
		}

		var numberOfLevels = relativeReferenceFolderList.Count() - startRelativeDepth;

		for (var i = 0; i < numberOfLevels; i++)
		{
			relativePath += ".." + Path.DirectorySeparatorChar;
		}

		for (var i = startRelativeDepth; i < absolutePathFolderList.Count(); i++)
		{
			if (i < absolutePathFolderList.Count() - 1)
			{
				relativePath += absolutePathFolderList[i] + Path.DirectorySeparatorChar;
			}
			else
			{
				relativePath += absolutePathFolderList[i];
			}
		}

		//Debug.Log(relativePath);

		return relativePath;
	}

	private static string ConvertFromRelativeToAbsolute(string filePath)
	{
		var absolutePath = string.Empty;

		var relativePath = filePath;
		var relativeReference = Application.dataPath;

		relativePath = relativePath.Replace("\\", Path.DirectorySeparatorChar + "");
		relativeReference = relativeReference.Replace("\\", Path.DirectorySeparatorChar + "");

		relativePath = relativePath.Replace("//", Path.DirectorySeparatorChar + "");
		relativeReference = relativeReference.Replace("//", Path.DirectorySeparatorChar + "");

		relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		relativeReference = relativeReference.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

		var relativePathFolderList = relativePath.Split(Path.DirectorySeparatorChar).ToList();
		var relativeReferenceFolderList = relativeReference.Split(Path.DirectorySeparatorChar).ToList();

		var levelsUp = relativePathFolderList.Count(s => String.Compare(s, "..", StringComparison.Ordinal) == 0);

		if (levelsUp == 0)
		{
			//it's a subfolder
			absolutePath = Application.dataPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar) + relativePath;

			//Debug.Log(absolutePath);

			return absolutePath;
		}

		//it's not in the Assets folder
		for (var i = 0; i < relativeReferenceFolderList.Count() - levelsUp; i++)
		{
			absolutePath += relativeReferenceFolderList[i] + Path.DirectorySeparatorChar;
		}

		relativePathFolderList.RemoveAll(s => String.Compare(s, "..", StringComparison.Ordinal) == 0);
		for (var i = 0; i < relativePathFolderList.Count(); i++)
		{
			if (i == 0)
			{
				absolutePath += relativePathFolderList[i];
			}
			else
			{
				absolutePath += Path.DirectorySeparatorChar + relativePathFolderList[i];
			}
		}

		//Debug.Log(absolutePath);

		return absolutePath;
	}

	#endregion
	/// <summary>
	/// This is called by the editor tool to export the eclipse project and the build the apk with all the associated libraries. This is ment to be used only in the local automated build.
	/// </summary>
	public void AndroidBuildAutomated()
	{
		BuildAndroid();
		//StartGradleBuild(_automatedBuildSettingsData);
	}

	/// <summary>
	/// This is called by the editor tool to export the eclipse project and the build the apk with all the associated libraries. This is ment to be used only in the local automated build.
	/// </summary>
	public void IosBuildAutomated()
	{
		BuildIos();
		StartGradleTaskXcode(_automatedBuildSettingsData.iosProjectExportPath);
		//start the gradle task
	}

	/// <summary>
	/// Performs an android build and copies the gradle files. Called by the CI server.
	/// </summary>
	public static void BuildAndroid()
	{
		//perform operations on the paths

		//ParseExtraCommandLineArgs();

		if (FindSettingsResourceFile(ref _automatedBuildSettingsData))
		{
			var editorBuildScenes = EditorBuildSettings.scenes;
			var scenes = (from editorBuildSettingsScene in editorBuildScenes
						  where editorBuildSettingsScene.enabled && !string.IsNullOrEmpty(editorBuildSettingsScene.path)
						  select editorBuildSettingsScene.path);

			//fix here path for the server

			var parsedKeyStorePath = ConvertFromRelativeToAbsolute(_automatedBuildSettingsData.keyStorePath);
			var parsedProjectExportPath = ConvertFromRelativeToAbsolute(_automatedBuildSettingsData.projectExportPath);
//			var parsedGradleRootFolder = ConvertFromRelativeToAbsolute(_automatedBuildSettingsData.gradleRootFolder);

			if (Directory.Exists(parsedProjectExportPath))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(parsedProjectExportPath);
				foreach (var dir in directoryInfo.GetDirectories())
				{
					RecursiveForceDelete(dir);
				}
			}
			EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
			//android credentials
			PlayerSettings.Android.keystoreName = parsedKeyStorePath;
			PlayerSettings.Android.keystorePass = _automatedBuildSettingsData.keyStorePassword;
			PlayerSettings.Android.keyaliasName = _automatedBuildSettingsData.keyAliasName;
			PlayerSettings.Android.keyaliasPass = _automatedBuildSettingsData.keyAliasPassword;
			PlayerSettings.Android.useAPKExpansionFiles = _automatedBuildSettingsData.useApkExpansionFiles;

			if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
			{
				//switch to the correct build target
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
			}


			
			//_buildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;
			BuildPipeline.BuildPlayer(scenes.ToArray(), parsedProjectExportPath+".apk", BuildTarget.Android, _buildOptions);

		//	var sourcePath = parsedGradleRootFolder;
		//	var destinationPath = parsedProjectExportPath + Path.DirectorySeparatorChar + PlayerSettings.productName;

//			//Now Create all of the directories
//			foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
//			{
//				if (!dirPath.Contains(".svn"))
//				{
//					Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
//				}
//			}
//
//			//Copy all the files & Replaces any files with the same name
//			foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Where(s => !s.Contains(".meta")))
//			{
//				if (!newPath.Contains(".svn"))
//				{
//					File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
//					File.SetAttributes(destinationPath, FileAttributes.Normal);
//				}
//			}
		}
	}

	static void ParseExtraCommandLineArgs()
	{
		var commandLineArgs = Environment.GetCommandLineArgs();
		foreach (var commandLineArg in commandLineArgs)
		{
			if (commandLineArg.ToLower().Contains("enabledevbuild"))
			{
				Debug.Log("Extra commnad line argument:" + commandLineArg);
				SetDevBuild(commandLineArg);
			}
		}
	}

	private static BuildOptions _buildOptions = BuildOptions.None;

	static void SetDevBuild(string commandLineArg)
	{
		var devBuild = false;
		devBuild = commandLineArg.ToLower().Contains("true");

		if (devBuild)
		{
			Debug.Log("Creating a development build");
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
			{
				if (_buildOptions == BuildOptions.None)
				{
					_buildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;
				}
				else
				{
					_buildOptions |= BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler;
				}

				PlayerSettings.strippingLevel = StrippingLevel.StripAssemblies;
				PlayerSettings.stripEngineCode = true;
				PlayerSettings.SetPropertyInt("ScriptingBackend", (int)ScriptingImplementation.IL2CPP, BuildTargetGroup.iOS);
			}else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			{
				_buildOptions = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.ConnectWithProfiler | BuildOptions.AcceptExternalModificationsToPlayer;
			}
		}
	}

	/// <summary>
	/// Performs an ios build and copies the gradle files. Called by the CI server.
	/// </summary>
	public static void BuildIos()
	{

		ParseExtraCommandLineArgs();

		if (FindSettingsResourceFile(ref _automatedBuildSettingsData))
		{
			var editorBuildScenes = EditorBuildSettings.scenes;
			var scenes = (from editorBuildSettingsScene in editorBuildScenes
						  where editorBuildSettingsScene.enabled && !string.IsNullOrEmpty(editorBuildSettingsScene.path)
						  select editorBuildSettingsScene.path);

			var parsedProjectExportPath = ConvertFromRelativeToAbsolute(_automatedBuildSettingsData.iosProjectExportPath);
			var parsedGradleRootFolder = ConvertFromRelativeToAbsolute(_automatedBuildSettingsData.iosGradleRootFolder);

			//clean up the project export path
			DirectoryInfo directoryInfo = new DirectoryInfo(parsedProjectExportPath);
			foreach (var dir in directoryInfo.GetDirectories())
			{
				RecursiveForceDelete(dir);
			}

			if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
			{
				//switch to the correct build target
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.iOS);
			}

			Debug.Log("Build options IOS: " + _buildOptions);

			EditorUserBuildSettings.development = true;
			EditorUserBuildSettings.connectProfiler = true;
			EditorUserBuildSettings.allowDebugging = true;


			//ios export project path
			BuildPipeline.BuildPlayer(scenes.ToArray(), parsedProjectExportPath, BuildTarget.iOS, BuildOptions.None);

			var sourcePath = parsedGradleRootFolder;
			var destinationPath = parsedProjectExportPath;

			//Now Create all of the directories
			foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
			{
				if (!dirPath.Contains(".svn"))
				{
					Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
				}
			}

			//Copy all the files & Replaces any files with the same name
			foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).Where(s => !s.Contains(".meta")))
			{
				if (!newPath.Contains(".svn"))
				{
					File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
					File.SetAttributes(destinationPath, FileAttributes.Normal);
				}
			}
		}
	}

	private void StartGradleBuild(AutomatedBuildSettings buildSettings)
	{
		StartGradleCommandLine(ConvertFromRelativeToAbsolute(buildSettings.projectExportPath) + Path.DirectorySeparatorChar + PlayerSettings.productName);
	}

	/// <summary>
	/// Recurse through the directory's contents and delete all files and directories,
	/// first setting the read only attribute on the files to false first.
	/// </summary>
	/// <param name="dir"></param>
	private static void RecursiveForceDelete(DirectoryInfo dir)
	{
		foreach (FileInfo file in dir.GetFiles())
		{
			try
			{
				file.IsReadOnly = false;
				file.Delete();
			}
			catch (Exception e)
			{
				Debug.LogError("Failed to delete file: " + file.FullName + ". Please make sure it's not open before building.\n" + e.Message);
			}
		}
		foreach (DirectoryInfo directory in dir.GetDirectories())
		{
			RecursiveForceDelete(directory);
		}
		try
		{
			dir.Delete();
		}
		catch (Exception e)
		{
			Debug.LogWarning("Failed to delete directory: " + dir.FullName + ". exception:\n" + e.Message);
		}
	}

	/// <summary>
	/// Initiates a gradle build from the command line
	/// </summary>
	/// <param name="workingDirectory"></param>
	void StartGradleCommandLine(string workingDirectory)
	{
		//select the correct way of startig gradle as a process based on the OS
		var osVersion = Environment.OSVersion.ToString();
		if (osVersion.Contains("Windows"))
		{
			StartGradleTaskWindows(workingDirectory);

		}
		else if (osVersion.Contains("Unix"))
		{
			StartGradleTaskUnix(workingDirectory);
		}
	}

	private void StartGradleTaskWindows(string workingDirectory = "")
	{
		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			WindowStyle = ProcessWindowStyle.Normal,
			FileName = @workingDirectory + Path.DirectorySeparatorChar + "gradlew.bat",
			WorkingDirectory = @workingDirectory,
			Arguments = @"assemble",
			UseShellExecute = false,
			CreateNoWindow = false
		};

		try
		{
			// Start the process.
			process.StartInfo = startInfo;
			process.Start();

			// Display the process statistics until 
			// the user closes the program. 
			do
			{
				if (!process.HasExited)
				{
					// Refresh the current process property values.
					process.Refresh();
					//Debug.Log("Process refresh");
				}
			} while (!process.WaitForExit(1000));

		}
		finally
		{
			process.Close();
			process.Dispose();
			CreaetLogs(workingDirectory);
		}
	}

	/// <summary>
	/// Starts the 
	/// </summary>
	private void StartGradleTaskXcode(string workingDirectory = "")
	{
		var osVersion = Environment.OSVersion.ToString();
		if (osVersion.Contains("Unix"))
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("'" + workingDirectory);
			stringBuilder.Append(Path.DirectorySeparatorChar);
			stringBuilder.Append("./gradlew' -b");
			stringBuilder.Append(" ");
			stringBuilder.Append("'" + workingDirectory + Path.DirectorySeparatorChar + "build.gradle'");
			stringBuilder.Append(" ");
			stringBuilder.Append("xcodebuild package");

			var command = stringBuilder.ToString();

			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Normal,
				FileName = "/bin/bash",
				Arguments = "-c \" " + command + " \"",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			process.StartInfo = startInfo;
			process.Start();

			EditorUtility.DisplayProgressBar("Building the xcode project", "Starting gradle...", 0);

			while (!process.StandardOutput.EndOfStream)
			{
				var output = process.StandardOutput.ReadLine();
				EditorUtility.DisplayProgressBar("Building the xcode project", output, 0);

				if (output != null && output.Contains("BUILD SUCCESSFUL"))
				{
					EditorUtility.DisplayDialog("XCode Build", "Build completed successfully !", "Ok");
				}
				else if (output != null && output.Contains("BUILD FAILED"))
				{
					EditorUtility.DisplayDialog("XCode Build", "Build failed !", "Ok");
				}
			}

			EditorUtility.ClearProgressBar();
			CreaetLogs(workingDirectory);
		}
	}

	/// <summary>
	/// Start the android gradle build
	/// </summary>
	/// <param name="workingDirectory"></param>
	private void StartGradleTaskUnix(string workingDirectory = "")
	{
		//Debug.Log("Start UNIX Gradle");
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("'" + workingDirectory);
		stringBuilder.Append(Path.DirectorySeparatorChar);
		stringBuilder.Append("./gradlew' -b");
		stringBuilder.Append(" ");
		stringBuilder.Append("'" + workingDirectory + Path.DirectorySeparatorChar + "build.gradle'");
		stringBuilder.Append(" ");
		stringBuilder.Append("assemble");

		var command = stringBuilder.ToString();

		Process process = new Process();
		ProcessStartInfo startInfo = new ProcessStartInfo
		{
			WindowStyle = ProcessWindowStyle.Normal,
			FileName = "/bin/bash",
			Arguments = "-c \" " + command + " \"",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};

		process.StartInfo = startInfo;
		process.Start();

		EditorUtility.DisplayProgressBar("Building the android project", "Starting gradle...", 0);

		while (!process.StandardOutput.EndOfStream)
		{
			var output = process.StandardOutput.ReadLine();
			EditorUtility.DisplayProgressBar("Building the android project", output, 0);

			if (output != null && output.Contains("BUILD SUCCESSFUL"))
			{
				EditorUtility.DisplayDialog("Android Build", "Build completed successfully !", "Ok");
			}
			else if (output != null && output.Contains("BUILD FAILED"))
			{
				EditorUtility.DisplayDialog("Android Build", "Build failed !", "Ok");
			}
		}

		EditorUtility.ClearProgressBar();
		CreaetLogs(workingDirectory);
	}

	/// <summary>
	/// Starts a gradle task on an already exported project
	/// </summary>
	public void StartGradleTask()
	{
		AutomatedBuildSettings buildSettings = _automatedBuildSettingsData;

		if (FindSettingsResourceFile(ref buildSettings))
		{
			var destinationPath = ConvertFromRelativeToAbsolute(buildSettings.projectExportPath + Path.AltDirectorySeparatorChar + PlayerSettings.productName);
			if (Directory.Exists(destinationPath))
			{
				StartGradleCommandLine(destinationPath);
			}
			else
			{
				Debug.LogError("Please build the project first !");
			}

		}
	}

	/// <summary>
	/// Creates tabs from buttons, with their bottom edge removed by the magic of Haxx
	/// </summary>
	/// <remarks> 
	/// The line will be misplaced if other elements is drawn before this
	/// </remarks> 
	/// <returns>Selected tab</returns>
	public static int Tabs(string[] options, int selected)
	{
		const float darkGray = 0.4f;
		const float lightGray = 0.9f;
		const float startSpace = 10;

		GUILayout.Space(startSpace);
		Color storeColor = GUI.backgroundColor;
		Color highlightCol = new Color(darkGray, darkGray, darkGray);
		Color bgCol = new Color(lightGray, lightGray, lightGray);

		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button) { padding = { bottom = 8 } };

		GUILayout.BeginHorizontal();
		{   //Create a row of buttons
			for (int i = 0; i < options.Length; ++i)
			{
				GUI.backgroundColor = i == selected ? highlightCol : bgCol;
				if (GUILayout.Button(options[i], buttonStyle))
				{
					selected = i; //Tab click
				}
			}
		}
		GUILayout.EndHorizontal();
		//Restore color
		GUI.backgroundColor = storeColor;
		//Draw a line over the bottom part of the buttons (ugly haxx)
		var texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, highlightCol);
		texture.Apply();
        GUI.DrawTexture(new Rect(0, buttonStyle.lineHeight + buttonStyle.border.top + buttonStyle.margin.top + startSpace, UnityEngine.Screen.width, 4), texture);

		return selected;
	}
}
