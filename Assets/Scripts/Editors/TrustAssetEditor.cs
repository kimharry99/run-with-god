using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class TrustAssetEditor : EditorWindow
{
	MonoScript script;
	string assetName;
	string trustName;
	string trustDescription;

	[MenuItem("Editors/신탁 어셋 만들기")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		TrustAssetEditor window = (TrustAssetEditor)GetWindow(typeof(TrustAssetEditor));
		window.name = "어셋 만들기";
		window.Show();
	}

	void OnGUI()
	{
		GUILayout.Label("신탁 스크립트", EditorStyles.helpBox);
		script = EditorGUILayout.ObjectField(script, typeof(MonoScript), false) as MonoScript;

		GUILayout.Label("어셋 이름", EditorStyles.helpBox);
		assetName = EditorGUILayout.TextField("어셋 이름", assetName);

		GUILayout.Label("신탁 이름", EditorStyles.helpBox);
		trustName = EditorGUILayout.TextField("신탁 이름", trustName);

		GUILayout.Label("신탁 내용", EditorStyles.helpBox);
		trustDescription = EditorGUILayout.TextArea(trustDescription);

		if (GUILayout.Button("생성"))
		{
			Trust trust = CreateInstance(script.GetClass()) as Trust;

			trust.trustName = trustName;
			trust.description = trustDescription;

			AssetDatabase.CreateAsset(trust, "Assets/Trusts/" + assetName + ".asset");
			AssetDatabase.Refresh();
		}
	}
}
#endif
