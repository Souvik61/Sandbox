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

    }

}