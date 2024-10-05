using System;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{

    [Serializable]
    public struct SaveJson
    {
        public List<ObjectRectJson> gameObjectsRect;
        public List<ObjectCircJson> gameObjectsCircle;
        public List<ObjectTriJson> gameObjectsTriangle;
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

}