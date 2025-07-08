using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SandboxGame
{
    public class WebGLFileSaver : MonoBehaviour
    {
        [System.Serializable]
        private class FileData
        {
            public string filename;
            public string content;
        }

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void DownloadFile(string filename, byte[] data, int length);

    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string onSuccessMethod, string onCancelMethod, string accept);

#endif

        /// <summary>
        /// Subscribe to this action to get file upload events
        /// </summary>
        public Action<string,string> OnFileUploaded;

        /// <summary>
        /// Subscribe to this action to get file upload cancelled events
        /// </summary>
        public Action<string> OnFileUploadCancelled;

        public void SaveToFile(string fileName, string content)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadFile(fileName, bytes, bytes.Length);
#endif
        }

        public void TriggerFileLoad()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(gameObject.name, nameof(OnFileLoaded), nameof(OnFileCancelled), ".json,.txt");
#else
            Debug.LogWarning("File loading works only in WebGL build.");
#endif
        }

        public void OnFileLoaded(string content)
        {
            FileData data = JsonUtility.FromJson<FileData>(content);
            Debug.Log("File successfully loaded:\n" + content);

            OnFileUploaded?.Invoke(data.filename, data.content);
        }

        public void OnFileCancelled(string unused)
        {
            Debug.Log("File load was cancelled by the user.");

            OnFileUploadCancelled?.Invoke(unused);
        }


    }
}