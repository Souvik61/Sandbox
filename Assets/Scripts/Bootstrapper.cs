using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
#if !FINAL
    [SerializeField] private bool isLogEnabled = true;
#endif

    private void Start()
    {
#if FINAL
		Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = isLogEnabled;
#endif
        Debug.Log($"<color=green>Starting Game</color>");
        
        StartCoroutine(AsyncLoadGameManagerScene());
    }
    IEnumerator AsyncLoadGameManagerScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameManager", LoadSceneMode.Additive);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Debug.Log($"<color=green>Done Loading 'GameMananger' </color>");
        SceneManager.UnloadSceneAsync("Bootstrapper");
    }
}
