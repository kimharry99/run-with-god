using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class TrustAssetEditor : EditorWindow
{
	MonoScript script;
	string assetName;
	TrustType trustType;
	string trustName;
	string trustDescription;
	int trustTier;

	#region KillCountTrust
	EnemyType enemyType;
	int killCount;
	#endregion

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

		if (!script.GetClass().IsSubclassOf(typeof(Trust)))
			return;
		assetName = EditorGUILayout.TextField("어셋 이름", assetName);

		trustType = (TrustType)EditorGUILayout.EnumPopup("신탁 유형",trustType);

		trustName = EditorGUILayout.TextField("신탁 이름", trustName);

		GUILayout.Label("신탁 내용", EditorStyles.helpBox);
		trustDescription = EditorGUILayout.TextArea(trustDescription);

		GUILayout.Label("신탁 티어", EditorStyles.helpBox);
		trustTier = EditorGUILayout.IntSlider(trustTier, -5, 5);

		if (script.GetClass().Name == "KillCountTrust")
		{
			enemyType = (EnemyType)EditorGUILayout.EnumPopup("적 종류", enemyType);
			killCount = EditorGUILayout.IntField("필요한 킬수", killCount);
		}

		if (GUILayout.Button("생성"))
		{
			if (script.GetClass().Name == "KillCountTrust")
			{
				KillCountTrust trust = CreateInstance(script.GetClass()) as KillCountTrust;

				trust.trustName = trustName;
				trust.description = trustDescription;
				trust.trustType = trustType;
				trust.tier = trustTier;
				trust.enemyType = enemyType;
				trust.needKillCount = killCount;

				AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
			}
			else
			{

				Trust trust = CreateInstance(script.GetClass()) as Trust;

				trust.trustName = trustName;
				trust.description = trustDescription;
				trust.trustType = trustType;
				trust.tier = trustTier;

				AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
			}
			AssetDatabase.Refresh();
		}
	}
}
#endif
