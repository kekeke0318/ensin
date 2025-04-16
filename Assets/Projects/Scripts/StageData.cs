using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData", order = 0)]
public class StageData : ScriptableObject
{
    public int stageID;
    public float gravity;
    public float timeLimit;         // 追加設定例：制限時間
    public int targetStarCount;     // 追加設定例：必要な Star 数

    public DialogueSet dialogueSet; // 会話内容（クリア、失敗、エンディングなど）
    
    // ギミック情報：配置する各ギミックの prefab と位置／回転
    public GimmickData[] gimmicks;
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