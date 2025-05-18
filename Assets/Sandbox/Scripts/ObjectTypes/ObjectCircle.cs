using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{

    public class ObjectCircle : ObjectBase
    {
        public float radius;

        public SpriteRenderer _spriteRenderer;

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

            outList.Add(new PropertyItem { id = "_type", name = "Type", datatype = "string", value = type });
            outList.Add(new PropertyItem { id = "_posX", name = "Position X", datatype = "float", value = transform.position.x });
            outList.Add(new PropertyItem { id = "_posY", name = "Position Y", datatype = "float", value = transform.position.y });
            outList.Add(new PropertyItem { id = "_rot", name = "Rotation", datatype = "float", value = GetZRotation() });
            outList.Add(new PropertyItem { id = "_col", name = "Color", datatype = "color", value = GetColor() });
            outList.Add(new PropertyItem { id = "_radius", name = "Radius", datatype = "float", value = radius });

            return outList;
        }

        public override void UpdateProperties()
        {
            _properties["_type"] = new PropertyItem { id = "_type", name = "Type", datatype = "string", value = GetType() };
            _properties["_posX"] = new PropertyItem { id = "_posX", name = "Position X", datatype = "float", value = transform.position.x };
            _properties["_posY"] = new PropertyItem { id = "_posY", name = "Position Y", datatype = "float", value = transform.position.y };
            _properties["_rot"] = new PropertyItem { id = "_rot", name = "Rotation", datatype = "float", value = GetZRotation() };
            _properties["_col"] = new PropertyItem { id = "_col", name = "Color", datatype = "color", value = GetColor() };
            _properties["_radius"] = new PropertyItem { id = "_radius", name = "Radius", datatype = "float", value = radius };
        }

    }

}