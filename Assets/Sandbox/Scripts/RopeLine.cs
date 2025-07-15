using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class RopeLine : MonoBehaviour
    {
        Rope2DCreator rope;
        LineRenderer line;

        private void Awake()
        {
            rope = GetComponent<Rope2DCreator>();
            line = GetComponent<LineRenderer>();

            line.enabled = true;
            line.positionCount = rope.segments.Length;
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < rope.segments.Length; i++)
            {
                line.SetPosition(i, rope.segments[i].position);
            }
        }
    }
}
