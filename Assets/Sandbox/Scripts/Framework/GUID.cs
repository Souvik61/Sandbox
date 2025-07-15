using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using System;
using System.Collections.Generic;

[Serializable, DisallowMultipleComponent]
public class GUID : MonoBehaviour
{
	[SerializeField]
	private int guidHashCode = 0;

    private Guid Guid;
    public void Init()
    {
        Guid = Guid.NewGuid();
        guidHashCode = Guid.GetHashCode();
    }

    private void Reset()
    {
     //   Init();
    }

    public bool IsValid
	{
		get
		{
			return (0 != guidHashCode);
		}
	}

	public int HashCode
	{
		get
		{
			return guidHashCode;
		}
	}
}
