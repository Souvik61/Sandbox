using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Redapple/Create New SceneData")]
public class SceneData : ScriptableObject
{
    [SerializeField] public string sceneName;
}

//Data Structure
[Serializable]
public class SceneDataDictionary : SerializableDictionary<SceneName, SceneData> { }

