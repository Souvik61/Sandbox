using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{
    public class ObjectRect : ObjectPrimitive
    {
        public Vector2 size;

        public SpriteRenderer _spriteRenderer;

        public bool IsStatic;

        private bool isOutlineEnabled;

        public override void Init()
        {
            base.Init();

            UpdateProperties();
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);

            _spriteRenderer.color = color;

        }

        public override void EnableOutline(bool enable)
        {
            isOutlineEnabled = enable;

            if (isOutlineEnabled)
            {
                var spRend = transform.GetComponentInChildren<SpriteRenderer>();
                spRend.material = new Material(GameManager.Instance.ConfigData.outlineMaterial);
            }
            else
            {
                var spRend = transform.GetComponentInChildren<SpriteRenderer>();
                spRend.material = new Material(GameManager.Instance.ConfigData.spritedefMaterial);

            }

        }

        /// <summary>
        /// Get list of all properties of this object
        /// </summary>
        /// <returns></returns>
        public override List<PropertyItem> GetAllProperties()
        {
            return _properties.Values.ToList();
        }

        public override void UpdateProperties()
        {
            _properties["_type"] = new PropertyItem { id = "_type", name = "Type", proptype = PropertyType.STRING, getter = () => type.ToString() };
            _properties["_posX"] = new PropertyItem { id = "_posX", name = "Position X", proptype = PropertyType.FLOAT, getter = () => transform.position.x };
            _properties["_posY"] = new PropertyItem { id = "_posY", name = "Position Y", proptype = PropertyType.FLOAT, getter = () => transform.position.y };
            _properties["_rot"] = new PropertyItem { id = "_rot", name = "Rotation", proptype = PropertyType.FLOAT, getter = () => GetZRotation() };
            _properties["_col"] = new PropertyItem { id = "_col", name = "Color", proptype = PropertyType.COLOR, getter = () => GetColor() };
            _properties["_width"] = new PropertyItem { id = "_width", name = "Width", proptype = PropertyType.FLOAT, getter = () => size.x };
            _properties["_height"] = new PropertyItem { id = "_height", name = "Height", proptype = PropertyType.FLOAT, getter = () => size.y };
            _properties["_static"] = new PropertyItem { id = "_static", name = "Static", proptype = PropertyType.BOOL, getter = () => IsStatic, setter = (val) => { IsStatic = (bool)val; } };
        }
    }

}