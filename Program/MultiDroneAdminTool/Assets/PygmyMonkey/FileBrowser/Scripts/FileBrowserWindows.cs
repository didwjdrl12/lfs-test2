using System;
using UnityEngine;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
namespace PygmyMonkey.FileBrowser
{
    /// <summary>
    /// Has static methods to select/save files and folders on Windows.
    /// </summary>
    public class FileBrowserWindows
    {
        /// <summary>
        /// Opens the select file panel (Select a single file).
        /// Will only show files with extension defined in extensionArray.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
        /// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
        /// <param name="onDone">Callback called when a file has been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the file selected).</param>
        public static void OpenFilePanel(string title, string startingDirectory, string[] extensionArray, Action<bool, string> onDone)
        {
            if (onDone == null)
            {
                Debug.LogError("[FileBrowserWindows] There is no callback define for OpenFilePanel, you won't get any result from FileBrowser...");
                return;
            }

            Internal.FileBrowserWindowsNative.OpenFile(GetTitle(title), GetStartingDirectory(startingDirectory), GetExtensionString(extensionArray), false, (string[] filePathArray) =>
            {
                bool isCanceled = filePathArray[0].Equals("cancel");
                onDone(isCanceled, isCanceled ? null : filePathArray[0]);
            });
        }

        /// <summary>
        /// Opens the select multiple files panel (Select multiple files).
        /// Will only show files with extension defined in extensionArray.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
        /// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
        /// <param name="onDone">Callback called when files have been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string[]) is the file selected array).</param>
        public static void OpenMultipleFilesPanel(string title, string startingDirectory, string[] extensionArray, Action<bool, string[]> onDone)
        {
            if (onDone == null)
            {
                Debug.LogError("[FileBrowserWindows] There is no callback define for OpenMultipleFilesPanel, you won't get any result from FileBrowser...");
                return;
            }

            Internal.FileBrowserWindowsNative.OpenFile(GetTitle(title), GetStartingDirectory(startingDirectory), GetExtensionString(extensionArray), true, (string[] filePathArray) =>
            {
                bool isCanceled = filePathArray[0].Equals("cancel");
                onDone(isCanceled, isCanceled ? null : filePathArray);
            });
        }

        /// <summary>
        /// Opens the select folder panel (Select a single folder).
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
        /// <param name="onDone">Callback called when a folder has been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
        public static void OpenFolderPanel(string title, string startingDirectory, Action<bool, string> onDone)
        {
            if (onDone == null)
            {
                Debug.LogError("[FileBrowserWindows] There is no callback define for OpenFolderPanel, you won't get any result from FileBrowser...");
                return;
            }

            Internal.FileBrowserWindowsNative.OpenFolder(GetTitle(title), GetStartingDirectory(startingDirectory), (string filePath) =>
            {
                bool isCanceled = filePath.Equals("cancel");
                onDone(isCanceled, isCanceled ? null : filePath);
            });
        }

        /// <summary>
        /// Opens the select multiple folders panel (Select multiple folders).
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
        /// <param name="onDone">Callback called when folders have been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string[]) is the folder selected array).</param>
        public static void OpenMultipleFoldersPanel(string title, string startingDirectory, Action<bool, string[]> onDone)
        {
            if (onDone == null)
            {
                Debug.LogError("[FileBrowserWindows] There is no callback define for OpenMultipleFoldersPanel, you won't get any result from FileBrowser...");
                return;
            }

            Debug.LogWarning("Opening multiple folders is not available on Windows (only on Mac), single folder selection will be used...");

            OpenFolderPanel(GetTitle(title), GetStartingDirectory(startingDirectory), (bool canceled, string filePath) =>
            {
                onDone(canceled, new String[] { filePath });
            });
        }

        /// <summary>
        /// Opens the save file panel (Save a file).
        /// Will set the file types dropdown with the extensions defined in extensionArray, if not null.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="startingDirectory">Starting directory (if null, will use the last opened folder).</param>
        /// <param name="defaultName">Default Name of the file to be saved. (If null, no name is pre-filled in the inputField).</param>
        /// <param name="extensionArray">Extension array (specify only the extension, no symbols (,.*) - example "jpg", "png"). If null, it will allow any file.</param>
        /// <param name="onDone">Callback called when a folder has been selected (It has two parameters. First (bool) to check if the panel has been canceled. Second (string) is the folder selected).</param>
        public static void SaveFilePanel(string title, string startingDirectory, string defaultName, string[] extensionArray, Action<bool, string> onDone)
        {
            if (onDone == null)
            {
                Debug.LogError("[FileBrowserWindows] There is no callback define for SaveFilePanel, you won't get any result from FileBrowser...");
                return;
            }

            Internal.FileBrowserWindowsNative.SaveFile(GetTitle(title), GetStartingDirectory(startingDirectory), GetExtensionString(extensionArray), GetDefaultName(defaultName), (string filePath) =>
            {
                bool isCanceled = filePath.Equals("cancel");
                onDone(isCanceled, isCanceled ? null : filePath);
            });
        }

        private static string GetExtensionString(string[] extensionArray)
        {
            string extensionString = "";

            if (extensionArray != null && extensionArray.Length != 0)
            {
                extensionString = "Files (";

                for (int i = 0; i < extensionArray.Length; i++)
                {
                    if (extensionArray[i].Contains(",") || extensionArray[i].Contains(".") || extensionArray[i].Contains("*"))
                    {
                        Debug.LogError("[FileBrowserWindows] Extensions should not contain , . or *");
                        return null;
                    }

                    extensionString += "*." + extensionArray[i] + ", ";
                }

                if (extensionString.EndsWith(", "))
                {
                    extensionString = extensionString.Substring(0, extensionString.Length - 2);
                }

                extensionString += ")|";

                for (int i = 0; i < extensionArray.Length; i++)
                {
                    extensionString += "*." + extensionArray[i] + ";";
                }
            }

            return extensionString;
        }

        private static string GetTitle(string title)
        {
            if (title == null) title = string.Empty;
            return title;
        }

        private static string GetStartingDirectory(string startingDirectory)
        {
            if (startingDirectory == null) startingDirectory = string.Empty;
            startingDirectory = startingDirectory.Replace(@"\\", @"\").Replace("//", @"\").Replace(@"/", @"\");

            if (!startingDirectory.EndsWith(@"\"))
            {
                startingDirectory += @"\";
            }

            return startingDirectory;
        }

        private static string GetDefaultName(string defaultName)
        {
            if (defaultName == null) defaultName = string.Empty;
            return defaultName;
        }
    }
}
#endif