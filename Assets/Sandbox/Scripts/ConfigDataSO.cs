using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SandboxGame
{
    [CreateAssetMenu(fileName = "ConfigData", menuName = "SandboxGame/ConfigurationData")]
    public class ConfigDataSO : ScriptableObject
    {
        public Color[] colorList;

        public Material spritedefMaterial;
        public Material outlineMaterial;

        public float JointMinLength;
        public float RopeJointMinLength;
    }
}
