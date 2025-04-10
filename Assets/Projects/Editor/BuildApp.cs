using System.Linq;
using UnityEditor;

public static class BuildApp 
{
    [MenuItem("Build/BuildApp")]
    public static void Build()
    {
        //windows64のプラットフォームでアプリをビルドする
        BuildPipeline.BuildPlayer(
            new string[] { "Assets/Scenes/SampleScene.unity" },
            "Builds/App/SampleApp.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );
    }
    
    [MenuItem("Build/WebGL")]
    public static void BuildWebGL()
    {
		//ビルドパイプライン設定
		var buildPlayerOptions = new BuildPlayerOptions
		{
			scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
			locationPathName = "WebGL Builds",
			target = BuildTarget.WebGL,
			options = BuildOptions.Development
		};
		BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
