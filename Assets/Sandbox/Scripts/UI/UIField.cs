using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    public abstract class UIField : MonoBehaviour
    {
        public string Id;
        public string Name;
        public string Type;

        public abstract System.Object Value { get; set; }

    }
}
