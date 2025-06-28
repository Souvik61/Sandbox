using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public class Utilities
    {
        public static void Swap<T>(ref T a,ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }
}
