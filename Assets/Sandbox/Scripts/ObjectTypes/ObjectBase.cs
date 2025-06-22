using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    /// <summary>
    /// Base class for all scene objects (physical object, joints)
    /// </summary>
    public class ObjectBase : MonoBehaviour
    {
        /// <summary>
        /// This struct describes a property
        /// </summary>
        public struct PropertyItem
        {
            public string id;
            public string name;
            public PropertyType proptype;
            public Func<object> getter;
            public Action<object> setter;
        }

        public ObjectType type;
        private Color _color;

        protected Dictionary<string, PropertyItem> _properties;

        public virtual void Init()
        {
            _properties = new();
        }

        virtual public void SetColor(Color color)
        {
            _color = color;
        }

        virtual public Color GetColor()
        {
            return _color;
        }

        public virtual List<PropertyItem> GetAllProperties()
        {
            return null;
        }

        public virtual PropertyItem GetProperty(string id)
        {
            return _properties[id];
        }

        public virtual void UpdateProperties()
        {
            
        }

        public virtual void SetLayer(int layer)
        { 
        
        }

    }

    /// <summary>
    /// Base class for all object that are solid(Rect,Circ,Tri)
    /// </summary>
    public class ObjectPrimitive : ObjectBase
    {
        public float GetZRotation()
        {
            float ang = Mathf.Atan2(transform.right.y, transform.right.x);
            return ang * Mathf.Rad2Deg;
        }

        public virtual void EnableOutline(bool enable)
        { 
        
        }

    }

    /// <summary>
    /// Base class for all object that are joints
    /// </summary>
    public class ObjectJoint : ObjectBase
    {
        public virtual void EnableOutline(bool enable)
        {

        }
    }

}