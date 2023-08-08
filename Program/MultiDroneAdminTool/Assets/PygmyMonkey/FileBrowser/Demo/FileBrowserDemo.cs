using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using PygmyMonkey.FileBrowser;

public class FileBrowserDemo : MonoBehaviour
{
    [SerializeField] private Text m_ContentText = null;
    [SerializeField] private RawImage m_RawImage = null;

    void Awake()
    {
        m_RawImage.gameObject.SetActive(false);
        m_ContentText.gameObject.SetActive(false);
        m_ContentText.text = "";
    }

    /*
     * Simple actions
     */
    public void OnOpenFileButtonClicked()
    {
        FileBrowser.OpenFilePanel("Open file Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), null, null, (bool canceled, string filePath) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Open File]\nCanceled";
                return;
            }

            m_ContentText.text = "[Open File]\n<b>Selected file</b>: " + filePath;
        });
    }

    public void OnOpenMultipleFilesButtonClicked()
    {
        FileBrowser.OpenMultipleFilesPanel("Open multiple files Title", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), null, "Open multiple", (bool canceled, string[] filePathArray) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Open Multiple Files]\nCanceled";
                return;
            }

            m_ContentText.text = "[Open Multiple Files]\n";
            for (int i = 0; i < filePathArray.Length; i++)
            {
                m_ContentText.text += "<b>Selected file #" + i + "</b>: " + filePathArray[i] + "\n";
            }
        });
    }

    public void OnOpenFolderButtonClicked()
    {
        FileBrowser.OpenFolderPanel("Open folder Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), null, (bool canceled, string folderPath) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Open Folder]\nCanceled";
                return;
            }

            m_ContentText.text = "[Open Folder]\n<b>Selected folder</b>: " + folderPath;
        });
    }

    public void OnOpenMultipleFoldersButtonClicked()
    {
        FileBrowser.OpenMultipleFoldersPanel("Open multiple folders Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Open folders", (bool canceled, string[] folderPathArray) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Open Multiple Folders]\nCanceled";
                return;
            }

            m_ContentText.text = "[Open Multiple Folders]\n";
            for (int i = 0; i < folderPathArray.Length; i++)
            {
                m_ContentText.text += "<b>Selected folder #" + i + "</b>: " + folderPathArray[i] + "\n";
            }
        });
    }

    public void OnSaveFileButtonClicked()
    {
        FileBrowser.SaveFilePanel("Save file Title", "Type here your message...", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Default Name", new string[] { "jpg", "png" }, null, (bool canceled, string filePath) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Save File]\nCanceled";
                return;
            }

            m_ContentText.text = "[Save File]\nYou can now save the file to the path: " + filePath;
        });
    }

    /*
     * Complexe actions
     */
    public void OnLoadImageButtonClicked()
    {
        FileBrowser.OpenFilePanel("Select an image to load", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), new string[] { "jpg", "png" }, null, (bool canceled, string filePath) => {
            if (canceled)
            {
                m_ContentText.text = "[Load Image]\nCanceled";
                m_ContentText.gameObject.SetActive(true);
                m_RawImage.gameObject.SetActive(false);
                return;
            }

            m_RawImage.gameObject.SetActive(true);
            m_ContentText.gameObject.SetActive(false);
            m_ContentText.text = "";

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(filePath));
            m_RawImage.texture = texture;

            // Decide if we should resize the raw image width or height
            bool keepImageWidth = true;
            if (keepImageWidth)
            {
                m_RawImage.rectTransform.sizeDelta = new Vector2(m_RawImage.rectTransform.sizeDelta.x, texture.height * m_RawImage.rectTransform.sizeDelta.x / (float)texture.width);
            }
            else
            {
                m_RawImage.rectTransform.sizeDelta = new Vector2(texture.width * m_RawImage.rectTransform.sizeDelta.y / (float)texture.height, m_RawImage.rectTransform.sizeDelta.y);
            }
        });
    }

    public void OnLoadFileButtonClicked()
    {
        FileBrowser.OpenFilePanel("Open file Title", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new string[] { "txt", "html" }, null, (bool canceled, string filePath) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Open File]\nCanceled";
                return;
            }

            m_ContentText.text = File.ReadAllText(filePath);
        });
    }

    public void OnSaveTextButtonClicked()
    {
        FileBrowser.SaveFilePanel("Save text file", "Type here your message...", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DemoText", new string[] { "txt" }, null, (bool canceled, string filePath) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Save Text]\nCanceled";
                return;
            }

            string demoText = "Hello, this is a demo text!";

            File.WriteAllText(filePath, demoText);
            m_ContentText.text = "[Save Text]\nText (" + demoText + ") saved to path: " + filePath;
        });
    }

    public void OnSaveScreenshotButtonClicked()
    {
        FileBrowser.SaveFilePanel("Save screenshot", "Type here your message...", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DemoScreenshot.jpg", new string[] { "jpg" }, null, (bool canceled, string filePath) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[Save Screenshot]\nCanceled";
                return;
            }

            #if UNITY_2017_1_OR_NEWER
            ScreenCapture.CaptureScreenshot(filePath, 1);
            #else
            Application.CaptureScreenshot(filePath);
            #endif
            m_ContentText.text = "[Save File]\nScreenshot saved to path: " + filePath;

            Application.OpenURL("file://" + filePath);
        });
    }

    public void OnListFilesInDirectoriesButtonClicked()
    {
        FileBrowser.OpenMultipleFoldersPanel("Open multiple folders", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Open folders", (bool canceled, string[] folderPathArray) => {
            m_RawImage.gameObject.SetActive(false);
            m_ContentText.gameObject.SetActive(true);

            if (canceled)
            {
                m_ContentText.text = "[List Files From Multiple Folders]\nCanceled";
                return;
            }

            m_ContentText.text = "[List Files From Multiple Folders]\n";
            foreach (string folderPath in folderPathArray)
            {
                m_ContentText.text += "<b>Selected folder: " + folderPath + "</b>\n";

                string[] filePathArray = Directory.GetFiles(folderPath);
                foreach (string filePath in filePathArray)
                {
                    m_ContentText.text += "- <b>File</b>: " + filePath + "\n";
                }

                m_ContentText.text += "\n";
            }
        });
    }
}
