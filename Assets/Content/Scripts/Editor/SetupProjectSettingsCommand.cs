#if UNITY_EDITOR
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEngine;

namespace Template.Editor.Commands
{
    /// <summary>
    /// Unity CLI command: unity command setup_project_settings
    /// Applies template defaults without fragile eval scripts.
    /// </summary>
    public static class SetupProjectSettingsCommand
    {
        [CliCommand(
            "setup_project_settings",
            "Apply template player settings: Input System only, static batching on, dynamic batching off.",
            Tags = new[] { "settings/player" })]
        public static object SetupProjectSettings()
        {
            var playerSettings = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
            var serialized = new SerializedObject(playerSettings);

            var inputHandler = serialized.FindProperty("activeInputHandler");
            var previousInput = inputHandler != null ? inputHandler.intValue : -1;
            if (inputHandler != null)
            {
                inputHandler.intValue = 1;
            }

            var batching = serialized.FindProperty("m_BuildTargetBatching");
            var batchTargetsUpdated = 0;
            if (batching != null)
            {
                for (var i = 0; i < batching.arraySize; i++)
                {
                    var element = batching.GetArrayElementAtIndex(i);
                    var staticBatching = element.FindPropertyRelative("m_StaticBatching");
                    var dynamicBatching = element.FindPropertyRelative("m_DynamicBatching");

                    if (staticBatching != null && staticBatching.propertyType == SerializedPropertyType.Boolean)
                    {
                        staticBatching.boolValue = true;
                    }

                    if (dynamicBatching != null && dynamicBatching.propertyType == SerializedPropertyType.Boolean)
                    {
                        dynamicBatching.boolValue = false;
                    }

                    batchTargetsUpdated++;
                }
            }

            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssets();

            return new
            {
                activeInputHandler = 1,
                previousInputHandler = previousInput,
                batchTargetsUpdated,
                productName = PlayerSettings.productName,
                message = "Template player settings applied."
            };
        }
    }
}
#endif
