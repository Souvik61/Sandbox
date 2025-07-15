using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor( typeof(Panel) )]
public class PanelEditor : Editor {

	private Panel panel;


	[MenuItem ("CONTEXT/Panel/Open PanelIDs")]
	static void PreferredFit (MenuCommand command)
	{
		AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath((AssetDatabase.FindAssets("PanelIDs")[0])), typeof(Object)));
	}


	public void OnEnable()
	{
		panel = (Panel)target;
	}

	public override void OnInspectorGUI()
	{
		if(EditorApplication.isPlaying)
		{
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Transition ON"))
				panel.Transition(Panel.onTransitionName);
			if (GUILayout.Button("Transition OFF"))
				panel.Transition(Panel.offTransitionName);
			EditorGUILayout.EndHorizontal();
		}
		base.OnInspectorGUI();
	}
}
