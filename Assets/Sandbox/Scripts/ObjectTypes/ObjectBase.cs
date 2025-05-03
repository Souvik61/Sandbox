using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{

    public class ObjectBase : MonoBehaviour
    {
        public ObjectType type;
        private Color _color;

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