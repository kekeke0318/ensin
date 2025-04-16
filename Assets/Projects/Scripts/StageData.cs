using UnityEngine;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData", order = 0)]
public class StageData : ScriptableObject
{
    public int stageID;
    public float gravity;
    public DialogueSet dialogueSet;
}

[System.Serializable]
public class DialogueSet
{
    // 例えば、クリア時、失敗時、エンディングの会話内容をそれぞれ設定
    public string dialogueClear;
    public string dialogueFail;
    public string endingDialogue;
    
    // 必要に応じて追加の会話パターンも設定可能
    // public string dialogueRetry;
    // public string dialogueIntroduction;
}
