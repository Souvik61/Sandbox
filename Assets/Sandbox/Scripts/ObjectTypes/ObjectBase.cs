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
        public ObjectType type;
        private Color _color;

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
    }

}