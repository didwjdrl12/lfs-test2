using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class FileBrowserConfigurator
{
    private static bool mHasAtLeastOneLibraryBeenModified = false;

    static FileBrowserConfigurator()
    {
        DeleteOldFiles();
        ConfigureLibraries();
    }

    // Delete old FileBrowser files and directory if they exists
    private static void DeleteOldFiles()
    {
        string[] directoryPathArray = new string[]
        {
            Application.streamingAssetsPath + "/PygmyMonkey/FileBrowser/",
            Application.dataPath + "/PygmyMonkey/FileBrowser/Plugins/MacOS/FileBrowserBundle.bundle",
        };

        for (int i = 0; i < directoryPathArray.Length; i++)
        {
            if (Directory.Exists(directoryPathArray[i]))
            {
                Directory.Delete(directoryPathArray[i], true);
            }
        }

        string[] oldFilePathArray = new string[]
        {
            Application.streamingAssetsPath + "/PygmyMonkey/FileBrowser.meta",
            Application.streamingAssetsPath + "/PygmyMonkey/FileBrowser/FileBrowser.exe.meta",
            Application.dataPath + "/PygmyMonkey/FileBrowser/Plugins/MacOS/FileBrowserBundle.bundle.meta",
            Application.dataPath + "/PygmyMonkey/FileBrowser/Scripts/FileBrowserDispatcher.cs",
            Application.dataPath + "/PygmyMonkey/FileBrowser/Scripts/FileBrowserDispatcher.cs.meta",
        };

        for (int i = 0; i < oldFilePathArray.Length; i++)
        {
            if (File.Exists(oldFilePathArray[i]))
            {
                File.Delete(oldFilePathArray[i]);
            }
        }
    }

    private static void SetCompatibleWithEditor(PluginImporter pluginImporter, bool enable)
    {
        if (pluginImporter.GetCompatibleWithEditor() != enable)
        {
            pluginImporter.SetCompatibleWithEditor(enable);
            mHasAtLeastOneLibraryBeenModified = true;
        }
    }

    private static void SetCompatibleWithAnyPlatform(PluginImporter pluginImporter, bool enable)
    {
        if (pluginImporter.GetCompatibleWithAnyPlatform() != enable)
        {
            pluginImporter.SetCompatibleWithAnyPlatform(enable);
            mHasAtLeastOneLibraryBeenModified = true;
        }
    }

    private static void SetCompatibleWithPlatform(PluginImporter pluginImporter, BuildTarget buildTarget, bool enable)
    {
        if (pluginImporter.GetCompatibleWithPlatform(buildTarget) != enable)
        {
            pluginImporter.SetCompatibleWithPlatform(buildTarget, enable);
            mHasAtLeastOneLibraryBeenModified = true;
        }
    }

    private static void SetExcludeEditorFromAnyPlatform(PluginImporter pluginImporter, bool enable)
    {
        if (pluginImporter.GetExcludeEditorFromAnyPlatform() != enable)
        {
            pluginImporter.SetExcludeEditorFromAnyPlatform(enable);
            mHasAtLeastOneLibraryBeenModified = true;
        }
    }

    private static void SetExcludeFromAnyPlatform(PluginImporter pluginImporter, BuildTarget buildTarget, bool enable)
    {
        if (pluginImporter.GetExcludeFromAnyPlatform(buildTarget) != enable)
        {
            pluginImporter.SetExcludeFromAnyPlatform(buildTarget, enable);
            mHasAtLeastOneLibraryBeenModified = true;
        }
    }

    private static void SetEditorData(PluginImporter pluginImporter, string cpu, string os)
    {
        if (!pluginImporter.GetEditorData("CPU").Equals(cpu))
        {
            pluginImporter.SetEditorData("CPU", cpu);
            mHasAtLeastOneLibraryBeenModified = true;
        }

        if (!pluginImporter.GetEditorData("OS").Equals(os))
        {
            pluginImporter.SetEditorData("OS", os);
            mHasAtLeastOneLibraryBeenModified = true;
        }
    }

    private static void ConfigureLibraries()
    {
        PluginImporter windows32 = AssetImporter.GetAtPath("Assets/PygmyMonkey/FileBrowser/Plugins/Windows/x86/NativeFileBrowser.dll") as PluginImporter;
        SetCompatibleWithEditor(windows32, true); 
        SetCompatibleWithAnyPlatform(windows32, false);
        SetCompatibleWithPlatform(windows32, BuildTarget.StandaloneWindows, true);
        SetCompatibleWithPlatform(windows32, BuildTarget.StandaloneWindows64, false);
        SetExcludeEditorFromAnyPlatform(windows32, false);
        SetExcludeFromAnyPlatform(windows32, BuildTarget.StandaloneWindows, false);
        SetExcludeFromAnyPlatform(windows32, BuildTarget.StandaloneWindows64, true);
        SetEditorData(windows32, "x86", "Windows");

        PluginImporter windows64 = AssetImporter.GetAtPath("Assets/PygmyMonkey/FileBrowser/Plugins/Windows/x86_64/NativeFileBrowser.dll") as PluginImporter;
        SetCompatibleWithEditor(windows64, true);
        SetCompatibleWithAnyPlatform(windows64, false);
        SetCompatibleWithPlatform(windows64, BuildTarget.StandaloneWindows, false);
        SetCompatibleWithPlatform(windows64, BuildTarget.StandaloneWindows64, true);
        SetExcludeEditorFromAnyPlatform(windows64, false);
        SetExcludeFromAnyPlatform(windows64, BuildTarget.StandaloneWindows, true);
        SetExcludeFromAnyPlatform(windows64, BuildTarget.StandaloneWindows64, false);
        SetEditorData(windows64, "x86_64", "Windows");

        PluginImporter osx32 = AssetImporter.GetAtPath("Assets/PygmyMonkey/FileBrowser/Plugins/MacOS/x86/NativeFileBrowser.bundle") as PluginImporter;
        SetCompatibleWithEditor(osx32, true);
        SetCompatibleWithAnyPlatform(osx32, false);
        #if UNITY_2017_3_OR_NEWER
        SetCompatibleWithPlatform(osx32, BuildTarget.StandaloneOSX, false);
        #else
        SetCompatibleWithPlatform(osx32, BuildTarget.StandaloneOSXUniversal, false);
        SetCompatibleWithPlatform(osx32, BuildTarget.StandaloneOSXIntel, true);
        SetCompatibleWithPlatform(osx32, BuildTarget.StandaloneOSXIntel64, false);
        #endif
        SetExcludeEditorFromAnyPlatform(osx32, false);
        #if UNITY_2017_3_OR_NEWER
        SetExcludeFromAnyPlatform(osx32, BuildTarget.StandaloneOSX, true);
        #else
        SetExcludeFromAnyPlatform(osx32, BuildTarget.StandaloneOSXUniversal, true);
        SetExcludeFromAnyPlatform(osx32, BuildTarget.StandaloneOSXIntel, false);
        SetExcludeFromAnyPlatform(osx32, BuildTarget.StandaloneOSXIntel64, true);
        #endif
        SetEditorData(osx32, "x86", "OSX");

        PluginImporter osx64 = AssetImporter.GetAtPath("Assets/PygmyMonkey/FileBrowser/Plugins/MacOS/x86_64/NativeFileBrowser.bundle") as PluginImporter;
        SetCompatibleWithEditor(osx64, true);
        SetCompatibleWithAnyPlatform(osx64, false);
        #if UNITY_2017_3_OR_NEWER
        SetCompatibleWithPlatform(osx64, BuildTarget.StandaloneOSX, true);
        #else
        SetCompatibleWithPlatform(osx64, BuildTarget.StandaloneOSXUniversal, false);
        SetCompatibleWithPlatform(osx64, BuildTarget.StandaloneOSXIntel, false);
        SetCompatibleWithPlatform(osx64, BuildTarget.StandaloneOSXIntel64, true);
        #endif
        SetExcludeEditorFromAnyPlatform(osx64, false);
        #if UNITY_2017_3_OR_NEWER
        SetExcludeFromAnyPlatform(osx64, BuildTarget.StandaloneOSX, false);
        #else
        SetExcludeFromAnyPlatform(osx64, BuildTarget.StandaloneOSXUniversal, true);
        SetExcludeFromAnyPlatform(osx64, BuildTarget.StandaloneOSXIntel, true);
        SetExcludeFromAnyPlatform(osx64, BuildTarget.StandaloneOSXIntel64, false);
        #endif
        SetEditorData(osx64, "x86_64", "OSX");

        if (mHasAtLeastOneLibraryBeenModified)
        {
            windows32.SaveAndReimport();
            windows64.SaveAndReimport();
            osx32.SaveAndReimport();
            osx64.SaveAndReimport();
        }
    }
}