#if UNITY_EDITOR
using System.IO;
using Bluff.Managers;
using Template.Input;
using Template.UI.Controllers;
using Template.UI.Views;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bluff.Editor.Commands
{
    /// <summary>
    /// Unity CLI: unity command setup_card_scene
    /// Creates CardRound.unity with empty hierarchy placeholders.
    /// </summary>
    public static class SetupCardSceneCommand
    {
        private const string ScenesFolder = "Assets/Content/Scenes";
        private const string CardRoundScenePath = ScenesFolder + "/CardRound.unity";
        private const string PauseMenuUxmlPath = "Assets/Content/UI/UXML/PauseMenu.uxml";
        private const string PanelSettingsPath = "Assets/Content/UI/MenuPanelSettings.asset";

        [CliCommand(
            "setup_card_scene",
            "Create CardRound scene with blackboard host, FSM host, and card table placeholders.",
            Tags = new[] { "scenes", "card" })]
        public static object SetupCardScene(
            [CliArg("overwrite", "If true, recreate the scene even when it already exists.")]
            bool overwrite = false)
        {
            EnsureFolder(ScenesFolder);

            if (File.Exists(CardRoundScenePath) && !overwrite)
            {
                return new
                {
                    scene = CardRoundScenePath,
                    created = false,
                    message = "CardRound already exists. Pass --overwrite true to recreate."
                };
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var roundRoot = new GameObject("CardRound");
            roundRoot.AddComponent<CardRoundManager>();

            new GameObject("BlackboardHost").transform.SetParent(roundRoot.transform);
            new GameObject("FsmHost").transform.SetParent(roundRoot.transform);
            new GameObject("CardTable").transform.SetParent(roundRoot.transform);
            new GameObject("EnemySeats").transform.SetParent(roundRoot.transform);
            new GameObject("PileAnchor").transform.SetParent(roundRoot.transform);

            // Pause menu reuse so Cancel still works while prototyping the table.
            var pauseUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PauseMenuUxmlPath);
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (pauseUxml != null && panelSettings != null)
            {
                var uiObject = new GameObject("PauseMenuUI");
                var document = uiObject.AddComponent<UIDocument>();
                document.panelSettings = panelSettings;
                document.visualTreeAsset = pauseUxml;
                uiObject.AddComponent<PauseMenuView>();
                uiObject.AddComponent<PauseMenuController>();
                uiObject.AddComponent<InputHandler>();
            }

            EditorSceneManager.SaveScene(scene, CardRoundScenePath);
            AppendBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new
            {
                scene = CardRoundScenePath,
                created = true,
                overwrite,
                message = "CardRound scene saved with empty hierarchy. Open it after recompile."
            };
        }

        private static void AppendBuildSettings()
        {
            var existing = EditorBuildSettings.scenes;
            for (var i = 0; i < existing.Length; i++)
            {
                if (existing[i].path == CardRoundScenePath)
                {
                    return;
                }
            }

            var next = new EditorBuildSettingsScene[existing.Length + 1];
            for (var i = 0; i < existing.Length; i++)
            {
                next[i] = existing[i];
            }

            next[existing.Length] = new EditorBuildSettingsScene(CardRoundScenePath, true);
            EditorBuildSettings.scenes = next;
        }

        private static void EnsureFolder(string assetFolderPath)
        {
            if (AssetDatabase.IsValidFolder(assetFolderPath))
            {
                return;
            }

            var parts = assetFolderPath.Split('/');
            var current = parts[0];
            for (var i = 1; i < parts.Length; i++)
            {
                var next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }
    }
}
#endif
