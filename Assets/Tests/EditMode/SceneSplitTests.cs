using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace TurnTheOrientation.Tests
{
    public sealed class SceneSplitTests
    {
        private static readonly string[] ScenePaths =
        {
            "Assets/Scenes/SampleScene.unity",
            "Assets/Scenes/Level_01.unity",
            "Assets/Scenes/Level_02.unity",
            "Assets/Scenes/Level_03.unity"
        };

        private static readonly string[] PrefabPaths =
        {
            "Assets/Prefabs/Common/Player.prefab",
            "Assets/Prefabs/Common/Wall.prefab",
            "Assets/Prefabs/Common/Floor.prefab",
            "Assets/Prefabs/Common/CoreCrate.prefab",
            "Assets/Prefabs/Common/PressurePlate.prefab",
            "Assets/Prefabs/Common/SignalGate.prefab",
            "Assets/Prefabs/Common/ExitSignal.prefab",
            "Assets/Prefabs/Common/SignalDot.prefab"
        };

        [Test]
        public void BootstrapAndEveryLevelSceneAreEnabledInBuildSettings()
        {
            for (int i = 0; i < ScenePaths.Length; i++)
            {
                Assert.IsNotNull(
                    AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePaths[i]),
                    $"Missing scene asset {ScenePaths[i]}");
                Assert.IsTrue(
                    System.Array.Exists(
                        EditorBuildSettings.scenes,
                        scene => scene.enabled && scene.path == ScenePaths[i]),
                    $"Scene is not enabled in Build Settings: {ScenePaths[i]}");
            }
        }

        [Test]
        public void CommonGameplayActorsAreStoredAsPrefabs()
        {
            for (int i = 0; i < PrefabPaths.Length; i++)
            {
                Assert.IsNotNull(
                    AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPaths[i]),
                    $"Missing common prefab {PrefabPaths[i]}");
            }
        }
    }
}
