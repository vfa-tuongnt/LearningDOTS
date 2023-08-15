﻿#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Text;
using GpuEcsAnimationBaker.Engine.Data;
using UnityEditor;
using UnityEngine;

namespace GPUECSAnimationBaker.Engine.Baker
{
    [CustomEditor(typeof(GpuEcsAnimationBakerBehaviour))]
    public class GpuEcsAnimationBakerEditor : UnityEditor.Editor
    {
        private SerializedProperty bakerDataProperty;
        private SerializedProperty gpuEcsAnimatorProperty;
        private bool showPrefabError = false;
        
        void OnEnable()
        {
            bakerDataProperty = serializedObject.FindProperty("bakerData");
            gpuEcsAnimatorProperty = serializedObject.FindProperty("gpuEcsAnimator");
            showPrefabError = false;
        }

        public override void OnInspectorGUI()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = new Color(1f, 0.5f, 0f, 1f);
            labelStyle.wordWrap = true; 
            labelStyle.alignment = TextAnchor.MiddleLeft;
            labelStyle.fontSize = 22;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fixedHeight = 36;
            GUILayout.Label("GPU ECS Animation Baker", labelStyle);
            serializedObject.Update();
            EditorGUILayout.PropertyField(bakerDataProperty);

            bool validated = GpuEcsAnimationBakerServices.ValidateAnimationBakerData(
                (GpuEcsAnimationBakerData)bakerDataProperty.boxedValue, out string errors);
            if(!validated) EditorGUILayout.HelpBox(errors, MessageType.Error);
            if(showPrefabError) EditorGUILayout.HelpBox("Generation can only happen on unloaded, selected prefabs", MessageType.Error);
            GUI.enabled = validated;
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = new Color(1f, 0.5f, 0f, 1f);
            buttonStyle.alignment = TextAnchor.MiddleCenter;
            buttonStyle.fontSize = 22;
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fixedHeight = 36;
            if (GUILayout.Button("Generate GPU ECS Animator", buttonStyle))
            {
                GameObject gameObject = ((GpuEcsAnimationBakerBehaviour)target).gameObject;
                string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
                if (string.IsNullOrEmpty(path) || !PrefabUtility.IsPartOfAnyPrefab(gameObject))
                    showPrefabError = true;
                else
                {
                    string folder = Path.GetDirectoryName(path);
                    string subFolder = $"BakedAssets_{gameObject.name}";
                    string generatedAssetsFolder = Path.Combine(folder, subFolder);
                    if (!AssetDatabase.IsValidFolder(generatedAssetsFolder))
                        generatedAssetsFolder = AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(
                            folder, subFolder));

                    GameObject gpuEcsAnimator = (GameObject)gpuEcsAnimatorProperty.boxedValue;
                    string targetAssetPath = (gpuEcsAnimator == null)
                        ? Path.Combine(generatedAssetsFolder, $"{gameObject.name}_GpuEcsAnimator.prefab")
                        : AssetDatabase.GetAssetPath(gpuEcsAnimator);
                    string targetEnumAssetPath =
                        Path.Combine(generatedAssetsFolder, $"{gameObject.name}_AnimationIDs.cs");

                    GpuEcsAnimationBakerData bakerData = (GpuEcsAnimationBakerData)bakerDataProperty.boxedValue;
                    GameObject newGpuEcsAnimator = GpuEcsAnimationBakerServices.GenerateAnimationObject(path, bakerData,
                        generatedAssetsFolder, targetAssetPath, targetEnumAssetPath);
                    gpuEcsAnimatorProperty.boxedValue = newGpuEcsAnimator;
                    showPrefabError = false;
                }
            }
            GUI.enabled = true;

            EditorGUILayout.PropertyField(gpuEcsAnimatorProperty);
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
