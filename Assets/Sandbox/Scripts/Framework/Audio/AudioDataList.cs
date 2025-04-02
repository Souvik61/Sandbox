using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Redapple/Create AudioDataList Object")]
public class AudioDataList : ScriptableObject
{
    public List<AudioData> list;
}

[System.Serializable]
public class AudioData
{
    public string id;
    public AudioClip clip;
}