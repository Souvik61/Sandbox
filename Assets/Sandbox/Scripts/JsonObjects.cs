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
        public List<ObjectFixedJointJson> gameObjectsFixedJoint;
        public List<ObjectSpringJointJson> gameObjectsSpringJoint;
    }

    [Serializable]
    public class ObjectJson
    {
        public string type;
        public string name;
        public Vector2 position;
        public float rotation;
        public Color color;

        /// <summary>
        /// List of properties I want to serialize
        /// </summary>
        public List<PropertyJson> propertyJsons;

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

    [Serializable]
    public class ObjectFixedJointJson : ObjectJson
    {
        public string objectAName;
        public string objectBName;
    }

    [Serializable]
    public class ObjectSpringJointJson : ObjectJson
    {
        public string objectAName;
        public string objectBName;
    }

    /// <summary>
    /// Properties json representation
    /// </summary>
    [Serializable]
    public class PropertyJson
    {
        public string id;
        public PropertyType type;
        public string value;
    }

}