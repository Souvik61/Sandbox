using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RandomMaterialSelector : MonoBehaviour
{
    [SerializeField] private Material[] Materials;
    [SerializeField] private MeshRenderer Renderer;

    private void OnValidate()
    {
        if (Renderer == null)
            Renderer = GetComponentInChildren<MeshRenderer>();
    }

    public void Init(int Seed)
    {
        // Seed random with the GUID
        UnityEngine.Random.InitState(Seed);

        int randomIndex = UnityEngine.Random.Range(0, Materials.Length);
        if (ArrayExtensions.IsInRange(_Array:Materials, randomIndex))
        {
            Material RandomMaterial = Materials[randomIndex];
            if (RandomMaterial != null)
                Renderer.material = RandomMaterial;
        }
    }
}
