using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class ObjectTriangle : ObjectBase
    {
        public Vector2 size;

        public SpriteRenderer _spriteRenderer;
        
        public bool IsStatic;

        public override void SetColor(Color color)
        {
            base.SetColor(color);

            _spriteRenderer.color = color;
        }

        /// <summary>
        /// Get list of all properties of this object
        /// </summary>
        /// <returns></returns>
        public override List<PropertyItem> GetAllProperties()
        {
            List<PropertyItem> outList = new();

            outList.Add(new PropertyItem { id = "_type", name = "Type", proptype = PropertyType.STRING, getter = () => type.ToString() });
            outList.Add(new PropertyItem { id = "_posX", name = "Position X", proptype = PropertyType.FLOAT, getter = () => transform.position.x });
            outList.Add(new PropertyItem { id = "_posY", name = "Position Y", proptype = PropertyType.FLOAT, getter = () => transform.position.y });
            outList.Add(new PropertyItem { id = "_rot", name = "Rotation", proptype = PropertyType.FLOAT, getter = () => GetZRotation() });
            outList.Add(new PropertyItem { id = "_col", name = "Color", proptype = PropertyType.COLOR, getter = () => GetColor() });
            outList.Add(new PropertyItem { id = "_width", name = "Width", proptype = PropertyType.FLOAT, getter = () => size.x });
            outList.Add(new PropertyItem { id = "_height", name = "Height", proptype = PropertyType.FLOAT, getter = () => size.y });
            outList.Add(new PropertyItem { id = "_static", name = "Static", proptype = PropertyType.BOOL, getter = () => IsStatic, setter = (val) => { IsStatic = (bool)val; } });

            return outList;
        }

        public override void UpdateProperties()
        {
            //_properties["_type"] = new PropertyItem { id = "_type", name = "Type", proptype = PropertyType.STRING, value = GetType() };
            //_properties["_posX"] = new PropertyItem { id = "_posX", name = "Position X", proptype = PropertyType.FLOAT, value = transform.position.x };
            //_properties["_posY"] = new PropertyItem { id = "_posY", name = "Position Y", proptype = PropertyType.FLOAT, value = transform.position.y };
            //_properties["_rot"] = new PropertyItem { id = "_rot", name = "Rotation", proptype = PropertyType.FLOAT, value = GetZRotation() };
            //_properties["_col"] = new PropertyItem { id = "_col", name = "Color", proptype = PropertyType.COLOR, value = GetColor() };
            //_properties["_width"] = new PropertyItem { id = "_width", name = "Width", proptype = PropertyType.FLOAT, value = size.x };
            //_properties["_height"] = new PropertyItem { id = "_height", name = "Height", proptype = PropertyType.FLOAT, value = size.y };
        }
    }
}