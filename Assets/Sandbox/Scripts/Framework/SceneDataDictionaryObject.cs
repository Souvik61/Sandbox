using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Redapple/Create New SceneData Dictionary Object")]
public class SceneDataDictionaryObject : ScriptableObject
{
    public SceneDataDictionary sceneDatas;
    public string GetSceneString(SceneName _SceneName)
    {
        try { return sceneDatas[_SceneName].sceneName; }
        catch (KeyNotFoundException)
        {
            throw new KeyNotFoundException($"Scene '{_SceneName}' not found in scene data dictionary");
        }
    }

    public SceneName? GetSceneNameFromString(string _SceneName)
    {
        SceneName sceneNameToReturn = SceneName.None;

        // Create a comparision fake data
        SceneData sceneData = new SceneData()
        {
            sceneName = _SceneName,
        };

        foreach (KeyValuePair<SceneName, SceneData> item in sceneDatas)
        {
            if (item.Value.sceneName == _SceneName)
            {
                sceneNameToReturn = item.Key;
                break;
            }
        }

        if (sceneNameToReturn == SceneName.None)
        {
            Debug.LogError($"No Scene Exists: {_SceneName}.");
            return null;
        }

        return sceneNameToReturn;
    }
}
