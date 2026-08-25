#if UNITY_EDITOR
using System.IO;
using Template.Core;
using Template.Input;
using Template.Managers;
using Template.UI.Controllers;
using Template.UI.Views;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Template.Editor.Commands
{
    /// <summary>
    /// Unity CLI command: unity command setup_menus
    /// Creates scenes with menu objects already in the hierarchy (no runtime bootstrap).
    /// </summary>
    public static class SetupMenusCommand
    {
        private const string ScenesFolder = "Assets/Content/Scenes";
        private const string MainMenuScenePath = ScenesFolder + "/MainMenu.unity";
        private const string GameplayScenePath = ScenesFolder + "/SampleGameplay.unity";
        private const string MainMenuUxmlPath = "Assets/Content/UI/UXML/MainMenu.uxml";
        private const string PauseMenuUxmlPath = "Assets/Content/UI/UXML/PauseMenu.uxml";
        private const string MenuStylesPath = "Assets/Content/UI/USS/MenuStyles.uss";
        private const string PanelSettingsPath = "Assets/Content/UI/MenuPanelSettings.asset";

        [CliCommand(
            "setup_menus",
            "Create MainMenu and SampleGameplay scenes with UI Document, Panel Settings, and managers wired in the hierarchy.",
            Tags = new[] { "scenes", "ui" })]
        public static object SetupMenus(
            [CliArg("overwrite", "If true, recreate scenes even when they already exist.")]
            bool overwrite = false)
        {
            EnsureFolder(ScenesFolder);
            EnsureFolder("Assets/Content/UI");

            var mainMenuUxml = LoadRequired<VisualTreeAsset>(MainMenuUxmlPath);
            var pauseMenuUxml = LoadRequired<VisualTreeAsset>(PauseMenuUxmlPath);
            LoadRequired<StyleSheet>(MenuStylesPath);
            var panelSettings = EnsurePanelSettings();

            var mainCreated = CreateMainMenuScene(mainMenuUxml, panelSettings, overwrite);
            var gameplayCreated = CreateGameplayScene(pauseMenuUxml, panelSettings, overwrite);

            UpdateBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return new
            {
                mainMenuScene = MainMenuScenePath,
                gameplayScene = GameplayScenePath,
                panelSettings = PanelSettingsPath,
                mainMenuCreated = mainCreated,
                gameplayCreated = gameplayCreated,
                overwrite,
                buildSettings = new[] { SceneNames.MainMenu, SceneNames.Gameplay },
                message = "Scenes saved with hierarchy objects wired. Open MainMenu and press Play."
            };
        }

        private static bool CreateMainMenuScene(VisualTreeAsset uxml, PanelSettings panelSettings, bool overwrite)
        {
            if (File.Exists(MainMenuScenePath) && !overwrite)
            {
                return false;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            CreateManagers();
            CreateUiDocument("MainMenuUI", uxml, panelSettings, typeof(MainMenuView), typeof(MainMenuController));
            EditorSceneManager.SaveScene(scene, MainMenuScenePath);
            return true;
        }

        private static bool CreateGameplayScene(VisualTreeAsset uxml, PanelSettings panelSettings, bool overwrite)
        {
            if (File.Exists(GameplayScenePath) && !overwrite)
            {
                return false;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            var uiObject = CreateUiDocument("PauseMenuUI", uxml, panelSettings, typeof(PauseMenuView), typeof(PauseMenuController));
            uiObject.AddComponent<InputHandler>();
            EditorSceneManager.SaveScene(scene, GameplayScenePath);
            return true;
        }

        private static void CreateManagers()
        {
            var audioObject = new GameObject("AudioManager");
            audioObject.AddComponent<AudioSource>();
            audioObject.AddComponent<AudioManager>();

            var gameObject = new GameObject("GameManager");
            gameObject.AddComponent<GameManager>();
        }

        private static GameObject CreateUiDocument(
            string objectName,
            VisualTreeAsset uxml,
            PanelSettings panelSettings,
            params System.Type[] extraComponents)
        {
            var uiObject = new GameObject(objectName);
            var document = uiObject.AddComponent<UIDocument>();
            document.panelSettings = panelSettings;
            document.visualTreeAsset = uxml;

            foreach (var componentType in extraComponents)
            {
                uiObject.AddComponent(componentType);
            }

            return uiObject;
        }

        private static PanelSettings EnsurePanelSettings()
        {
            var panelSettings = AssetDatabase.LoadAssetAtPath<PanelSettings>(PanelSettingsPath);
            if (panelSettings == null)
            {
                panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                AssetDatabase.CreateAsset(panelSettings, PanelSettingsPath);
            }

            panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            panelSettings.referenceResolution = new Vector2Int(1920, 1080);
            EditorUtility.SetDirty(panelSettings);
            return panelSettings;
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(MainMenuScenePath, true),
                new EditorBuildSettingsScene(GameplayScenePath, true)
            };
        }

        private static T LoadRequired<T>(string assetPath) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset == null)
            {
                throw new FileNotFoundException($"Required asset missing: {assetPath}");
            }

            return asset;
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
