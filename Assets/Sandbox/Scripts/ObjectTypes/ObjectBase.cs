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
            public string datatype;
            public System.Object value;
        }

        public ObjectType type;
        private Color _color;

        protected Dictionary<string, PropertyItem> _properties;

        public virtual void Init()
        {
          
        }

        virtual public void SetColor(Color color)
        {
            _color = color;
        }

        virtual public Color GetColor()
        {
            return _color;
        }

        public float GetZRotation()
        {
            float ang = Mathf.Atan2(transform.right.y, transform.right.x);
            return ang * Mathf.Rad2Deg;
        }

        public virtual List<PropertyItem> GetAllProperties()
        {
            return null;
        }

        public virtual PropertyItem GetProperty(string id)
        {
            UpdateProperties();
            return _properties[id];
        }

        public virtual void UpdateProperties()
        {
            
        }

    }

}