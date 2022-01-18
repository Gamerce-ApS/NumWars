using UnityEngine;

public class AutomatedBuildSettings : ScriptableObject
{
    #region Android
    #region Android signing credentials
    public string keyStorePath = string.Empty;
    public string keyStorePassword = string.Empty;
    public string keyAliasName = string.Empty;
    public string keyAliasPassword = string.Empty;
    #endregion

    public bool useApkExpansionFiles = false;

    public string projectExportPath = string.Empty;
    public string gradleBuildFilePath = string.Empty;
    public string gradleRootFolder = string.Empty;

    public string unityApplicationDataPath = string.Empty;
    public AutomatedBuildWindow.GradleTask gradleTask = AutomatedBuildWindow.GradleTask.Assemble;
    #endregion

    #region Ios

    public string iosProjectExportPath = string.Empty;
    public string iosGradleRootFolder = string.Empty;

    #endregion
}
