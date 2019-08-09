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

	#region KillCountTrust & KillInJumpTrust & OneKillTrust
	EnemyType enemyType;
	int killCount;
    #endregion
    #region ClearTimeTrust & OnGroundTrust
    float needTime;
    #endregion
    #region UseBulletTrust
    int needBullet;
    #endregion
    #region JumpCountTrust
    int needJump;
    #endregion
    #region DashCountTrust
    int needDash;
    #endregion
    #region EnemyInViewTrust
    float limitTime;
    int limitEnemycount;
    #endregion
    #region HitCountTrust
    int needHit;
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

		if (script != null && !script.GetClass().IsSubclassOf(typeof(Trust)))
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
        else if (script.GetClass().Name == "ClearTimeTrust")
        {
            needTime = EditorGUILayout.FloatField("제한 시간(s)", needTime);
        }
        else if (script.GetClass().Name == "UseBulletTrust")
        {
            needBullet = EditorGUILayout.IntField("총알 수", needBullet);
        }
        else if (script.GetClass().Name == "JumpCountTrust")
        {
            needJump = EditorGUILayout.IntField("점프 수", needJump);
        }
        else if (script.GetClass().Name == "DashCountTrust")
        {
            needDash = EditorGUILayout.IntField("대쉬 수", needDash);
        }
        else if (script.GetClass().Name == "KillInJumpTrust")
        {
            killCount = EditorGUILayout.IntField("필요한 킬수", killCount);
        }
        else if (script.GetClass().Name == "OneKillTrust")
        {
            killCount = EditorGUILayout.IntField("필요한 킬수", killCount);
        }
        else if (script.GetClass().Name == "OnGroundTrust")
        {
            needTime = EditorGUILayout.FloatField("제한 시간(s)", needTime);
        }
        else if (script.GetClass().Name == "EnemyInViewTrust")
        {
            limitTime = EditorGUILayout.FloatField("제한 시간(s)", limitTime);
            limitEnemycount = EditorGUILayout.IntField("몬스터 수", limitEnemycount);
        }
        else if (script.GetClass().Name == "HitCountTrust")
        {
            needHit = EditorGUILayout.IntField("맞는 횟수", needHit);
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
            else if (script.GetClass().Name == "ClearTimeTrust")
            {
                ClearTimeTrust trust = CreateInstance(script.GetClass()) as ClearTimeTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.needClearTime = needTime;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "UseBulletTrust")
            {
                UseBulletTrust trust = CreateInstance(script.GetClass()) as UseBulletTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.needUseBullet = needBullet;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "JumpCountTrust")
            {
                JumpCountTrust trust = CreateInstance(script.GetClass()) as JumpCountTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.needJumpCount = needJump;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "DashCountTrust")
            {
                DashCountTrust trust = CreateInstance(script.GetClass()) as DashCountTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.needDashCount = needDash;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "KillInJumpTrust")
            {
                KillInJumpTrust trust = CreateInstance(script.GetClass()) as KillInJumpTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.needKillCount = killCount;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "KillInJumpTrust")
            {
                OneKillTrust trust = CreateInstance(script.GetClass()) as OneKillTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.needKillCount = killCount;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "OnGroundTrust")
            {
                OnGroundTrust trust = CreateInstance(script.GetClass()) as OnGroundTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.limitTime = needTime;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "EnemyInViewTrust")
            {
                EnemyInViewTrust trust = CreateInstance(script.GetClass()) as EnemyInViewTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.limitTime = limitTime;
                trust.limitEnemycount = limitEnemycount;

                AssetDatabase.CreateAsset(trust, "Assets/Resources/Trusts/" + assetName + ".asset");
            }
            else if (script.GetClass().Name == "HitCountTrust")
            {
                HitCountTrust trust = CreateInstance(script.GetClass()) as HitCountTrust;

                trust.trustName = trustName;
                trust.description = trustDescription;
                trust.trustType = trustType;
                trust.tier = trustTier;

                trust.needHitCount = needHit;

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
