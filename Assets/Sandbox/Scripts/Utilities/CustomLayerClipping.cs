using UnityEngine;

/// <summary>
/// Allows setting custom culling distance for every layer.
/// </summary>
public class CustomLayerClipping : MonoBehaviour
{
	[SerializeField] float[] layerDistances = new float[32];
	
	// Use this for initialization
	void Awake()
	{
		GetComponent<Camera>().layerCullDistances = layerDistances;
	}
	
#if UNITY_EDITOR
	void OnValidate()
	{
		if (!Application.isPlaying)
		{
			//Debug.Log("Set the camera culling distances");
			GetComponent<Camera>().layerCullDistances = layerDistances;
		}
	}
#endif
}