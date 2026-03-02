using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class LevelSetBuilder
{
    [MenuItem("Tools/Build Level Set Data")]
    public static void BuildLevelSetData()
    {
        string assetPath = "Assets/Resources/LevelSetData.asset";
        LevelSetData levelSetData = AssetDatabase.LoadAssetAtPath<LevelSetData>(assetPath);

        if (levelSetData == null)
        {
            levelSetData = ScriptableObject.CreateInstance<LevelSetData>();
            AssetDatabase.CreateAsset(levelSetData, assetPath);
        }

        levelSetData.levelSets.Clear();

        Regex regex = new(@"level(\d+)_(\d+)", RegexOptions.IgnoreCase);
        Dictionary<int, List<string>> levelSets = new();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                var sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                var match = regex.Match(sceneName);

                if (match.Success)
                {
                    int setNumber = int.Parse(match.Groups[1].Value);
                    if (!levelSets.ContainsKey(setNumber))
                        levelSets[setNumber] = new List<string>();

                    levelSets[setNumber].Add(sceneName);
                }
            }
        }

        foreach (var kvp in levelSets.OrderBy(kvp => kvp.Key))
        {
            kvp.Value.Sort((a, b) =>
            {
                int aY = int.Parse(Regex.Match(a, @"_(\d+)$").Groups[1].Value);
                int bY = int.Parse(Regex.Match(b, @"_(\d+)$").Groups[1].Value);
                return aY.CompareTo(bY);
            });

            levelSetData.levelSets.Add(new LevelSetData.LevelSet
            {
                setNumber = kvp.Key,
                levelSceneNames = kvp.Value
            });
        }

        EditorUtility.SetDirty(levelSetData);
        AssetDatabase.SaveAssets();

        Debug.Log("LevelSetData asset built successfully.");
    }
}
