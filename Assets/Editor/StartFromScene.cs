using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine;

public static class StartFromScene
{
    private const string ScenePath = "Assets/Sandbox/Scenes/Bootstrapper.unity";

    [MenuItem("Sandbox/Play _F5")] // _F5 makes it accessible via F5
    public static void PlayFromLevel1()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("Already in Play Mode. Stop the game to restart from the selected scene.");
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(ScenePath);
            EditorApplication.EnterPlaymode();
        }
    }
}