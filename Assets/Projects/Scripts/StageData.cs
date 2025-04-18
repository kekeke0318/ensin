using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData", order = 0)]
public class StageData : ScriptableObject
{
    public int stageID;

    [Header("重力値")]
    public float gravity;

    [Header("必要な Star 数")]
    public int targetStarCount;

    [Header("ステージ毎のActorのプレファブ")]
    public Actor actorPrefab;

    [Header("ステージ毎の会話セット")]
    public DialogueSet dialogueSet;

    [Header("ステージ毎のギミック")]
    public GimmickData[] gimmicks;

    [Header("ステージ毎のギミック")]
    public GameObject previewActorPrefab;
    
    [Header("最終ステージ")]
    public bool isLast;
}

[System.Serializable]
public class DialogueSet
{
    public string dialogueClear;
    public string dialogueFail;

    public string endingDialogue;
    // 必要に応じて、他の会話設定を追加可能
}

[System.Serializable]
public class GimmickData
{
    public GameObject gimmickPrefab;
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
}