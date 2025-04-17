// Assets/Projects/Scripts/Logging/EnsinLog.cs
// 汎用デバッグログラッパー v1.2 — HideInCallstack 対応
// ------------------------------------------------------------
// * INFO, WARNING, ERROR などのレベルを定義し、必要に応じて
//   ビルド時・実行時の両方でフィルタリングできます。
// * "ENSIN_DEBUG_LOG" シンボルが無効なビルドでは完全にコードが
//   ストリップされるため、パフォーマンスを劣化させません。
// * Application.persistentDataPath 配下に自動でログファイルも生成。
// * Development / Editor ビルド時のみ簡易オンスクリーンコンソールを
//   追加（MonoBehaviour 部）。
// * 文字列以外の **任意オブジェクト** をそのまま渡せます。
// * Unity 2022.2 以降なら StackTrace から EnsinLog ラッパーを除外。
// ------------------------------------------------------------
#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define ENSIN_DEBUG_LOG
#endif

#if UNITY_2022_2_OR_NEWER
#define ENSIN_HIDE_CALLSTACK
#endif

using System;
using System.Diagnostics;          // Conditional 属性
using System.IO;                   // ファイル出力
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Ensin プロジェクト用の軽量ログユーティリティ。
/// Debug.Log() の代わりに使用することで、後片付けの手間と
/// ビルド後の不要ログ出力を防ぎます。
/// </summary>
public static class EnsinLog
{
    // -----------------------------
    // 設定
    // -----------------------------
    public enum Level
    {
        Verbose = 0,
        Info    = 1,
        Warning = 2,
        Error   = 3,
        Exception = 4,
    }

    public static bool Enabled { get; set; } = true;   // 実行時に動的 ON/OFF
    private static Level _minLevel = Level.Info;       // 最低表示レベル
    public  static string LogFilePath { get; }

    // -----------------------------
    // 初期化
    // -----------------------------
    static EnsinLog()
    {
#if ENSIN_DEBUG_LOG
        var time = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        LogFilePath = Path.Combine(Application.persistentDataPath, $"ensin_{time}.log");
#endif
    }

    public static void SetMinLevel(Level level) => _minLevel = level;

    // -----------------------------
    // パブリック API
    // -----------------------------
    // 文字列以外の任意オブジェクト (Vector3, Dictionary など) を直接渡せます。
    // Null は "null" 文字列として扱います。

#if ENSIN_HIDE_CALLSTACK
    [HideInCallstack]
#endif
    [Conditional("ENSIN_DEBUG_LOG")] public static void Verbose  (object msg, UnityEngine.Object ctx = null) => Print(Level.Verbose , msg, ctx);
#if ENSIN_HIDE_CALLSTACK
    [HideInCallstack]
#endif
    [Conditional("ENSIN_DEBUG_LOG")] public static void Info     (object msg, UnityEngine.Object ctx = null) => Print(Level.Info    , msg, ctx);
#if ENSIN_HIDE_CALLSTACK
    [HideInCallstack]
#endif
    [Conditional("ENSIN_DEBUG_LOG")] public static void Warning  (object msg, UnityEngine.Object ctx = null) => Print(Level.Warning , msg, ctx);
#if ENSIN_HIDE_CALLSTACK
    [HideInCallstack]
#endif
    [Conditional("ENSIN_DEBUG_LOG")] public static void Error    (object msg, UnityEngine.Object ctx = null) => Print(Level.Error   , msg, ctx);
#if ENSIN_HIDE_CALLSTACK
    [HideInCallstack]
#endif
    [Conditional("ENSIN_DEBUG_LOG")] public static void Exception(Exception ex , UnityEngine.Object ctx = null) => Print(Level.Exception, ex, ctx);

    // -----------------------------
    // 実体
    // -----------------------------
#if ENSIN_HIDE_CALLSTACK
    [HideInCallstack]
#endif
    private static void Print(Level level, object messageObj, UnityEngine.Object context)
    {
        if (!Enabled || level < _minLevel) return;

        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        string message   = messageObj?.ToString() ?? "null";
        string text      = $"[{timestamp}][{level}] {message}";

        switch (level)
        {
            case Level.Warning:   Debug.LogWarning(text, context); break;
            case Level.Error:     Debug.LogError  (text, context); break;
            case Level.Exception: Debug.LogError  ($"EXCEPTION => {text}", context); break;
            default:              Debug.Log       (text, context); break;
        }

        AppendToFile(text);
    }

    private static void AppendToFile(string line)
    {
#if ENSIN_DEBUG_LOG
        try
        {
            File.AppendAllText(LogFilePath, line + Environment.NewLine);
        }
        catch { /* 書き込み失敗は無視 */ }
#endif
    }

    // ------------------------------------------------------------
    // オンスクリーンコンソール (Development / Editor のみ)
    // ------------------------------------------------------------
#if ENSIN_DEBUG_LOG
    [AddComponentMenu("Ensin/OnScreenConsole")]
    public class OnScreenConsole : MonoBehaviour
    {
        private readonly System.Collections.Generic.Queue<string> _lines = new();
        private const int MaxLines = 25;

        private void OnEnable()  => Application.logMessageReceived += Handle;
        private void OnDisable() => Application.logMessageReceived -= Handle;

        private void Handle(string condition, string stackTrace, LogType type)
        {
            _lines.Enqueue(condition);
            while (_lines.Count > MaxLines) _lines.Dequeue();
        }

        private void OnGUI()
        {
            const float padding = 8f;
            GUILayout.BeginArea(new Rect(padding, padding, Screen.width * 0.6f, Screen.height * 0.4f), GUI.skin.box);
            foreach (var line in _lines)
            {
                GUILayout.Label(line);
            }
            GUILayout.EndArea();
        }
    }
#endif
}
