using UnityEngine;

namespace SandboxGame
{
    public class ObjectTriangle : ObjectBase
    {
        public Vector2 size;

        public SpriteRenderer _spriteRenderer;

        public override void SetColor(Color color)
        {
            base.SetColor(color);

            _spriteRenderer.color = color;
        }
    }
}