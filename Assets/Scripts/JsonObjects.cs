using System;
using UnityEngine;

[Serializable]
public struct SaveJson
{
    public ObjectJson[] gameObjects;


}

[Serializable]
public class ObjectJson
{
    public string type;
    public string name;
    public Vector2 position;
    public float rotation;

}

[Serializable]
public class ObjectRectJson : ObjectJson
{
    public Vector2 size;

}

[Serializable]
public class ObjectCircJson : ObjectJson
{
    public float radius;

}

[Serializable]
public class ObjectTriJson : ObjectJson
{
    public Vector2 size;
}