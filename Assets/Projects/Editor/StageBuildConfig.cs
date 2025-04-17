using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "MotherGravity/Stage Build Config", fileName = "StageBuildConfig")]
public class StageBuildConfig : ScriptableObject
{
    public StageData stageDataAsset;
    public string sceneOutputFolder = "Assets/Scenes";

    public bool randomizeGimmicks = false;
    public Vector2 gimmickSpawnMin = new Vector2(-3f, -3f);
    public Vector2 gimmickSpawnMax = new Vector2(3f, 3f);

    public bool randomizeTargets = false;
    public GameObject targetPrefab;
    public int targetCount = 3;
    public Vector2 targetSpawnMin = new Vector2(-5f, -5f);
    public Vector2 targetSpawnMax = new Vector2(5f, 5f);

    public bool placeCustomPrefabs = false;
    public List<GameObject> customPrefabs;
    public Vector2 customPrefabSpawnMin = new Vector2(-5f, -5f);
    public Vector2 customPrefabSpawnMax = new Vector2(5f, 5f);
}
