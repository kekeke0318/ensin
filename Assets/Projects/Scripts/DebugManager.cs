using System.Collections.Generic;
using UnityEngine;

public class DebugManager
{
    private readonly List<string> launchFailureLogs = new List<string>();

    // 発射失敗時のログを記録する
    public void LogLaunchFailure(Vector2 launchVector)
    {
        string log = $"Launch failed: Vector = {launchVector}";
        launchFailureLogs.Add(log);
        Debug.Log(log);
    }

    // 保持している失敗ログの一覧を取得
    public IReadOnlyList<string> GetLaunchFailureLogs() => launchFailureLogs.AsReadOnly();
}
