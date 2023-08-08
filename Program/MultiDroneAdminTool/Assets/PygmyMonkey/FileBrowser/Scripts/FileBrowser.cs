#define NATIVE_FILE_BROWSER

using System;
using UnityEngine;

namespace PygmyMonkey.FileBrowser
{
    /// <summary>
    /// File browser Wrapper, that will automatically call the Mac or Windows methods to select/save files and folders.
    /// </summary>
    public class FileBrowser
    {
        /// <summary>
        /// Opens the select file panel (Select a single file).
        /// Will only allow to select files with extension defined in extensionArray.
        /// </summary>
        /// <param name="title">Title (Only available on Windows).</param>
        /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
        /// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
        /// <param name="buttonName">The name of the button. You can set this to null to use the defaut value (Only available on Mac).</param>
        /// <param name="onDone">Callback called when a file has been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the file selected).</param>
        public static void OpenFilePanel(string title, string startingDirectory, string[] extensionArray, string buttonName, Action<bool, string> onDone)
        {
            #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            FileBrowserMac.OpenFilePanel(startingDirectory, extensionArray, buttonName, onDone);
            #elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            FileBrowserWindows.OpenFilePanel(title, startingDirectory, extensionArray, onDone);
            #else
            throw new UnityException("[FileBrowser] Platform " + Application.platform.ToString() + " not supported (only Mac and Windows are supported)");
            #endif
	    }

	    /// <summary>
	    /// Opens the select multiple files panel (Select multiple files).
	    /// Will only show files with extension defined in extensionArray.
	    /// </summary>
	    /// <param name="title">Title (Only available on Windows).</param>
	    /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	    /// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	    /// <param name="buttonName">The name of the button. You can set this to null to use the defaut value (Only available on Mac).</param>
        /// <param name="onDone">Callback called when files have been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string[]) is the file selected array).</param>
	    public static void OpenMultipleFilesPanel(string title, string startingDirectory, string[] extensionArray, string buttonName, Action<bool, string[]> onDone)
	    {
            #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            FileBrowserMac.OpenMultipleFilesPanel(startingDirectory, extensionArray, buttonName, onDone);
            #elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            FileBrowserWindows.OpenMultipleFilesPanel(title, startingDirectory, extensionArray, onDone);
            #else
            throw new UnityException("[FileBrowser] Platform " + Application.platform.ToString() + " not supported (only Mac and Windows are supported)");
            #endif
	    }

	    /// <summary>
	    /// Opens the select folder panel (Select a single folder).
	    /// </summary>
	    /// <param name="title">Title (Only available on Windows).</param>
	    /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	    /// <param name="buttonName">The name of the button. You can set this to null to use the defaut value (Only available on Mac).</param>
        /// <param name="onDone">Callback called when a folder has been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
	    public static void OpenFolderPanel(string title, string startingDirectory, string buttonName, Action<bool, string> onDone)
	    {
            #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            FileBrowserMac.OpenFolderPanel(startingDirectory, buttonName, onDone);
            #elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            FileBrowserWindows.OpenFolderPanel(title, startingDirectory, onDone);
            #else
            throw new UnityException("[FileBrowser] Platform " + Application.platform.ToString() + " not supported (only Mac and Windows are supported)");
            #endif
	    }

	    /// <summary>
	    /// Opens the select multiple folders panel (Select multiple folders).
	    /// </summary>
	    /// <param name="title">Title (Only available on Windows).</param>
	    /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	    /// <param name="buttonName">The name of the button. You can set this to null to use the defaut value (Only available on Mac).</param>
        /// <param name="onDone">Callback called when folders have been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string[]) is the folder selected array).</param>
	    public static void OpenMultipleFoldersPanel(string title, string startingDirectory, string buttonName, Action<bool, string[]> onDone)
	    {
            #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            FileBrowserMac.OpenMultipleFoldersPanel(startingDirectory, buttonName, onDone);
            #elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            FileBrowserWindows.OpenMultipleFoldersPanel(title, startingDirectory, onDone);
            #else
            throw new UnityException("[FileBrowser] Platform " + Application.platform.ToString() + " not supported (only Mac and Windows are supported)");
            #endif
	    }

	    /// <summary>
	    /// Opens the save file panel (Save a file).
	    /// Will set the file types dropdown with the extensions defined in extensionArray, if not null.
	    /// </summary>
	    /// <param name="title">Title (Only available on Windows).</param>
	    /// <param name="message">A hint message on top of the panel, to display a hint to users (Only available on Mac).</param>
	    /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
	    /// <param name="defaultName">Default Name of the file to be saved. (If null, no name is pre-filled in the inputField).</param>
	    /// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
	    /// <param name="buttonName">The name of the button. You can set this to null to use the defaut value (Only available on Mac).</param>
        /// <param name="onDone">Callback called when a folder has been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
	    public static void SaveFilePanel(string title, string message, string startingDirectory, string defaultName, string[] extensionArray, string buttonName, Action<bool, string> onDone)
	    {
            #if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            FileBrowserMac.SaveFilePanel(message, startingDirectory, defaultName, extensionArray, buttonName, onDone);
            #elif UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            FileBrowserWindows.SaveFilePanel(title, startingDirectory, defaultName, extensionArray, onDone);
            #else
            throw new UnityException("[FileBrowser] Platform " + Application.platform.ToString() + " not supported (only Mac and Windows are supported)");
            #endif
	    }
    }
}
