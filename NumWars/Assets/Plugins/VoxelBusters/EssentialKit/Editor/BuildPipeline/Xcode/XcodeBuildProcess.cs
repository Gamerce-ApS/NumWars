﻿#if UNITY_IOS || UNITY_TVOS
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.Editor;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.CoreLibrary.Editor.NativePlugins;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build;
using VoxelBusters.CoreLibrary.Editor.NativePlugins.Build.Xcode;
using VoxelBusters.EssentialKit;
using UnityEditor.Build.Reporting;
namespace VoxelBusters.EssentialKit.Editor.Build.Xcode
{
    [InitializeOnLoad]
    public static class XcodeBuildProcess
    {
#region Static fields

        private     static  EssentialKitSettings    s_settings;

        private     static  BuildReport             s_buildReport;

#endregion

#region Constructors

        static XcodeBuildProcess()
        {
            // unregister from events
            BuildProcessReporter.OnPreprocessBuild     -= OnPreprocessBuild;
            BuildProcessReporter.OnPostprocessBuild    -= OnPostprocessBuild;

            // register for events
            BuildProcessReporter.OnPreprocessBuild     += OnPreprocessBuild;
            BuildProcessReporter.OnPostprocessBuild    += OnPostprocessBuild;
        }

#endregion

#region Static methods

        public static void OnPreprocessBuild(BuildReport report)
        {
            // check whether plugin is configured
            if (!EssentialKitSettingsEditorUtility.SettingsExists)
            {
                EssentialKitSettingsEditorUtility.ShowSettingsNotFoundErrorDialog();
                return;
            }
            // check whether target is compatible for these tasks
            if (!IsBuildTargetSupported(report.summary.platform))
            {
                return;
            }

            DebugLogger.Log("[XcodeBuildProcess] Initiating pre-build task execution.");
            
            // update cached information
            s_settings      = EssentialKitSettingsEditorUtility.DefaultSettings;
            s_buildReport   = report;

            // execute tasks
            UpdateExporterSettings();
            EssentialKitBuildUtility.CreateStrippingFile(report.summary.platform);

            DebugLogger.Log("[XcodeBuildProcess] Successfully completed pre-build task execution.");
        }

        public static void OnPostprocessBuild(BuildReport report)
        {
            // check whether plugin is configured
            if (!EssentialKitSettingsEditorUtility.SettingsExists)
            {
                EssentialKitSettingsEditorUtility.ShowSettingsNotFoundErrorDialog();
                return;
            }
            if (!IsBuildTargetSupported(report.summary.platform))
            {
                return;
            }

            DebugLogger.Log("[XcodeBuildProcess] Initiating post-build task execution.");

            // update cached information
            s_settings      = EssentialKitSettingsEditorUtility.DefaultSettings;
            s_buildReport   = report;

            // execute tasks
            UpdateInfoPlist();
            UpdateUnityPreprocessor();

            DebugLogger.Log("[XcodeBuildProcess] Successfully completed post-build task execution.");


        }

        #endregion

        #region Private methods

        private static bool IsBuildTargetSupported(BuildTarget buildTarget)
        {
            return (BuildTarget.iOS == buildTarget || BuildTarget.tvOS == buildTarget);
        }

        private static void UpdateExporterSettings()
        {
            DebugLogger.Log("[XcodeBuildProcess] Updating native plugins exporter settings.");

            bool    enableBaseExporter  = false;
            var     baseExporter        = default(NativeFeatureExporterSettings);
            var     currentPlatform     = PlatformMappingServices.GetActivePlatform();
            foreach (var exporter in NativeFeatureExporterSettings.FindAllExporters(includeInactive: true))
            {
                switch (exporter.name)
                {
                    case Defaults.kBaseExporterName:
                        exporter.IsEnabled  = false;
                        baseExporter        = exporter;
                        break;

                    case NativeFeatureType.kAddressBook:
                    case NativeFeatureType.kNativeUI:
                    case NativeFeatureType.KSharingServices:
                    case NativeFeatureType.kCloudServices:
                    case NativeFeatureType.kGameServices:
                    case NativeFeatureType.kBillingServices:
                    case NativeFeatureType.kNetworkServices:
                    case NativeFeatureType.kWebView:
                    case NativeFeatureType.kMediaServices:
                        exporter.IsEnabled  = s_settings.IsFeatureUsed(exporter.name); 
                        enableBaseExporter |= exporter.IsEnabled;     
                        break;
                        
                    case NativeFeatureType.kNotificationServices:
                        var     notificationServicesSettings    = s_settings.NotificationServicesSettings;
                        exporter.IsEnabled  = notificationServicesSettings.IsEnabled;
                        exporter.IosProperties.ClearCapabilities();
                        exporter.IosProperties.ClearMacros();
                        exporter.IosProperties.AddMacro("NATIVE_PLUGINS_USES_NOTIFICATION");
                        exporter.IosProperties.RemoveFramework(new PBXFramework("CoreLocation.framework"));
                        if ((PushNotificationServiceType.Custom == notificationServicesSettings.PushNotificationServiceType) && exporter.IsEnabled)
                        {
                            exporter.IosProperties.AddMacro("NATIVE_PLUGINS_USES_PUSH_NOTIFICATION");
                            exporter.IosProperties.AddCapability(PBXCapability.PushNotifications());
                        }
                        if (notificationServicesSettings.UsesLocationBasedNotification)
                        {
                            exporter.IosProperties.AddMacro("NATIVE_PLUGINS_USES_CORE_LOCATION");
                            exporter.IosProperties.AddFramework(new PBXFramework("CoreLocation.framework"));
                        }
                        enableBaseExporter |= exporter.IsEnabled;
                        break;

                    case NativeFeatureType.kDeepLinkServices:
                        var     deepLinkSettings    = s_settings.DeepLinkServicesSettings;
                        var     associatedDomains   = deepLinkSettings.GetUniversalLinksForPlatform(currentPlatform);
                        exporter.IsEnabled          = deepLinkSettings.IsEnabled;
                        exporter.IosProperties.ClearCapabilities();
                        if (deepLinkSettings.IsEnabled && associatedDomains.Length > 0)
                        {
                            var     domains         = Array.ConvertAll(associatedDomains, (item) =>
                            {
                                string  serviceType = string.IsNullOrEmpty(item.ServiceType) ? "applinks" : item.ServiceType;
                                return string.Format("{0}:{1}", serviceType, item.Host);
                            });
                            exporter.IosProperties.AddCapability(PBXCapability.AssociatedDomains(domains));
                        }
                        enableBaseExporter |= exporter.IsEnabled;
                        break;
                        
                    default:
                        break;
                }
                EditorUtility.SetDirty(exporter);
            }

            // update base exporter status
            if (baseExporter)
            {
                baseExporter.IsEnabled  = enableBaseExporter;
            }
        }

        private static void UpdateInfoPlist()
        {
            DebugLogger.Log("[XcodeBuildProcess] Updating plist configuration.");

            // open plist
            string  plistPath   = s_buildReport.summary.outputPath + "/Info.plist";
            var     plist       = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            var     rootDict    = plist.root;

            // add usage permissions
            var     permissions = GetUsagePermissions();
            foreach (string key in permissions.Keys)
            {
                rootDict.SetString(key, permissions[key]);
            }

            // add LSApplicationQueriesSchemes
            string[]    appQuerySchemes = GetApplicationQueriesSchemes();
            if (appQuerySchemes.Length > 0)
            {
                PlistElementArray   array;
                if (false == rootDict.TryGetElement(InfoPlistKey.kNSQuerySchemes, out array))
                {
                    array = rootDict.CreateArray(InfoPlistKey.kNSQuerySchemes);
                }

                // add required schemes
                for (int iter = 0; iter < appQuerySchemes.Length; iter++)
                {
                    if (false == array.Contains(appQuerySchemes[iter]))
                    {
                        array.AddString(appQuerySchemes[iter]);
                    }
                }
            }


            // Add NSAppTrans


            PlistElementDict dic = rootDict.CreateDict("NSAppTransportSecurity");
            dic.SetBoolean("NSAllowsArbitraryLoads",true);
            PlistElementDict dic2 = dic.CreateDict("NSExceptionDomains");

            PlistElementDict dic3 = dic2.CreateDict("facebook.com");
            dic3.SetBoolean("NSIncludesSubdomains", true);
            dic3.SetBoolean("NSThirdPartyExceptionRequiresForwardSecrecy", false);

            PlistElementDict dic4 = dic2.CreateDict("fbcdn.net");
            dic4.SetBoolean("NSIncludesSubdomains", true);
            dic4.SetBoolean("NSThirdPartyExceptionRequiresForwardSecrecy", false);


            PlistElementDict dic5 = dic2.CreateDict("akamaihd.net");
            dic5.SetBoolean("NSIncludesSubdomains", true);
            dic5.SetBoolean("NSThirdPartyExceptionRequiresForwardSecrecy", false);

            rootDict.values.Remove("UIRequiredDeviceCapabilities");



            PlistElementArray arr = rootDict.CreateArray("SKAdNetworkItems");

            PlistElementDict arrDic = arr.AddDict();
            arrDic.SetString("SKAdNetworkIdentifier", "v9wttpbfk9.skadnetwork");

            PlistElementDict arrDic2 = arr.AddDict();
            arrDic2.SetString("SKAdNetworkIdentifier", "n38lu8286q.skadnetwork");




           // PlistElementDict dic6 = rootDict.CreateDict("FacebookClientToken");
            rootDict.SetString("FacebookClientToken", "d1f119442ac37c00554bdf3cd6868b82");

            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);


            

            // add deeplinks
            var deepLinkCustomSchemeUrls    = GetDeepLinkCustomSchemeUrls();
            if (deepLinkCustomSchemeUrls.Length > 0)
            {
                PlistElementArray   urlTypes;
                if (false == rootDict.TryGetElement(InfoPlistKey.kCFBundleURLTypes, out urlTypes))
                {
                    urlTypes    = rootDict.CreateArray(InfoPlistKey.kCFBundleURLTypes);
                }

                // add elements
                foreach (var current in deepLinkCustomSchemeUrls)
                {
                    var     newElement      = urlTypes.AddDict();
                    newElement.SetString(InfoPlistKey.kCFBundleURLName, current.Identifier);

                    var     schemeArray     = newElement.CreateArray(InfoPlistKey.kCFBundleURLSchemes);
                    schemeArray.AddString(current.Scheme);
                }
            }

            // save changes to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }

        private static Dictionary<string, string> GetUsagePermissions()
        {
            var     requiredPermissionsDict     = new Dictionary<string, string>(4);
            var     permissionSettings          = s_settings.ApplicationSettings.UsagePermissionSettings;

            // add address book permission
            var     abSettings                  = s_settings.AddressBookSettings;
            if (abSettings.IsEnabled || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
            {
                requiredPermissionsDict[InfoPlistKey.kNSContactsUsage] = permissionSettings.AddressBookUsagePermission.GetDescription(NativePlatform.iOS);
            }

            // add media related permissions
            var     mediaSettings               = s_settings.MediaServicesSettings;
            if (mediaSettings.IsEnabled || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
            {
                if (mediaSettings.UsesCamera || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
                {
                    requiredPermissionsDict[InfoPlistKey.kNSCameraUsage]       = permissionSettings.CameraUsagePermission.GetDescription(NativePlatform.iOS);
                }
                if (mediaSettings.UsesGallery || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
                {
                    requiredPermissionsDict[InfoPlistKey.kNSPhotoLibraryUsage] = permissionSettings.GalleryUsagePermission.GetDescription(NativePlatform.iOS);
                }
                if (mediaSettings.SavesFilesToGallery || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
                {
                    requiredPermissionsDict[InfoPlistKey.kNSPhotoLibraryAdd]   = permissionSettings.GalleryWritePermission.GetDescription(NativePlatform.iOS);
                }
            }

            // add notification related permissions
            var     notificationSettings        = s_settings.NotificationServicesSettings;
            if (notificationSettings.IsEnabled || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
            {
                if (notificationSettings.UsesLocationBasedNotification || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
                {
                    requiredPermissionsDict[InfoPlistKey.kNSLocationWhenInUse] = permissionSettings.LocationWhenInUsePermission.GetDescription(NativePlatform.iOS);
                }
            }

            // add sharing related permissions
            var sharingSettings = s_settings.SharingServicesSettings;
            if (sharingSettings.IsEnabled || !NativeFeatureUnitySettingsBase.CanToggleFeatureUsageState())
            {
                // added for supporting sharing/saving to gallery when share sheet is shown
                requiredPermissionsDict[InfoPlistKey.kNSPhotoLibraryAdd]   = permissionSettings.GalleryWritePermission.GetDescription(NativePlatform.iOS);
            }

            return requiredPermissionsDict;
        }

        private static string[] GetApplicationQueriesSchemes()
        {
            var     sharingSettings = s_settings.SharingServicesSettings;
            var     schemeList      = new List<string>();
            if (sharingSettings.IsEnabled)
            {
                schemeList.Add("fb");
                schemeList.Add("twitter");
                schemeList.Add("whatsapp");

                schemeList.Add("fbapi");
                schemeList.Add("fbapi20130214");
                schemeList.Add("fbapi20130410");
                schemeList.Add("fbapi20130702");
                schemeList.Add("fbapi20131010");
                schemeList.Add("fbapi20131219");
                schemeList.Add("fbapi20140410");
                schemeList.Add("fbapi20140116");
                schemeList.Add("fbapi20150313");
                schemeList.Add("fbapi20150629");
                schemeList.Add("fbauth");
                schemeList.Add("fbauth2");
                schemeList.Add("fb - messenger - api20140430");
                schemeList.Add("fb-messenger-api20140430");
            }

            return schemeList.ToArray();
        }


        private static DeepLinkDefinition[] GetDeepLinkCustomSchemeUrls()
        {
            var     deepLinkSettings    = s_settings.DeepLinkServicesSettings;
            if (deepLinkSettings.IsEnabled)
            {
                var     currentPlatform = PlatformMappingServices.GetActivePlatform();
                return deepLinkSettings.GetCustomSchemeUrlsForPlatform(currentPlatform);
            }
            
            return new DeepLinkDefinition[0];
        }

#endregion

#region Misc methods

        public static void UpdateUnityPreprocessor()
        {
            var     notificationSettings    = s_settings.NotificationServicesSettings;
            if (notificationSettings.IsEnabled && notificationSettings.PushNotificationServiceType == PushNotificationServiceType.Custom)
            {
                string  preprocessorPath    = s_buildReport.summary.outputPath + "/Classes/Preprocessor.h";
                string  text                = File.ReadAllText(preprocessorPath);
                text                        = text.Replace("UNITY_USES_REMOTE_NOTIFICATIONS 0", "UNITY_USES_REMOTE_NOTIFICATIONS 1");
                File.WriteAllText(preprocessorPath, text);
            }
        }

#endregion

#region Nested types

        private static class Defaults
        {
            internal const string   kBaseExporterName   = "Base";
        }

#endregion
    }
}
#endif