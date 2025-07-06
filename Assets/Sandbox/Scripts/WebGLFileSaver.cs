using System.Runtime.InteropServices;
using UnityEngine;

namespace SandboxGame
{
    public class WebGLFileSaver : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(string filename, byte[] data, int length);
#endif

        public void SaveToFile(string fileName, string content)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);

#if UNITY_WEBGL && !UNITY_EDITOR
        DownloadFile(fileName, bytes, bytes.Length);
#else

#endif
        }
    }
}