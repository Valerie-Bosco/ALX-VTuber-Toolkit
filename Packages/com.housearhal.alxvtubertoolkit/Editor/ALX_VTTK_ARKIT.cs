#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRM;

namespace ALX_ARKIT
{
    class ALX_ARKIT_Presets
    {
        public static readonly List<string> arkit_blend_shape_names = new List<string>
        {
            "eyeBlinkLeft",
            "eyeLookDownLeft",
            "eyeLookInLeft",
            "eyeLookOutLeft",
            "eyeLookUpLeft",
            "eyeSquintLeft",
            "eyeWideLeft",
            "eyeBlinkRight",
            "eyeLookDownRight",
            "eyeLookInRight",
            "eyeLookOutRight",
            "eyeLookUpRight",
            "eyeSquintRight",
            "eyeWideRight",
            "jawForward",
            "jawLeft",
            "jawRight",
            "jawOpen",
            "mouthClose",
            "mouthFunnel",
            "mouthPucker",
            "mouthRight",
            "mouthLeft",
            "mouthSmileLeft",
            "mouthSmileRight",
            "mouthFrownRight",
            "mouthFrownLeft",
            "mouthDimpleLeft",
            "mouthDimpleRight",
            "mouthStretchLeft",
            "mouthStretchRight",
            "mouthRollLower",
            "mouthRollUpper",
            "mouthShrugLower",
            "mouthShrugUpper",
            "mouthPressLeft",
            "mouthPressRight",
            "mouthLowerDownLeft",
            "mouthLowerDownRight",
            "mouthUpperUpLeft",
            "mouthUpperUpRight",
            "browDownLeft",
            "browDownRight",
            "browInnerUp",
            "browOuterUpLeft",
            "browOuterUpRight",
            "cheekPuff",
            "cheekSquintLeft",
            "cheekSquintRight",
            "noseSneerLeft",
            "noseSneerRight",
            "tongueOut",
        };

        public static readonly List<string> arkit_blend_shape_names_lowercase_lookup =
            new List<string>
            {
                "eyeblinkleft",
                "eyelookdownleft",
                "eyelookinleft",
                "eyelookoutleft",
                "eyelookupleft",
                "eyesquintleft",
                "eyewideleft",
                "eyeblinkright",
                "eyelookdownright",
                "eyelookinright",
                "eyelookoutright",
                "eyelookupright",
                "eyesquintright",
                "eyewideright",
                "jawforward",
                "jawleft",
                "jawright",
                "jawopen",
                "mouthclose",
                "mouthfunnel",
                "mouthpucker",
                "mouthright",
                "mouthleft",
                "mouthsmileleft",
                "mouthsmileright",
                "mouthfrownright",
                "mouthfrownleft",
                "mouthdimpleleft",
                "mouthdimpleright",
                "mouthstretchleft",
                "mouthstretchright",
                "mouthrolllower",
                "mouthrollupper",
                "mouthshruglower",
                "mouthshrugupper",
                "mouthpressleft",
                "mouthpressright",
                "mouthlowerdownleft",
                "mouthlowerdownright",
                "mouthupperupleft",
                "mouthupperupright",
                "browdownleft",
                "browdownright",
                "browinnerup",
                "browouterupleft",
                "browouterupright",
                "cheekpuff",
                "cheeksquintleft",
                "cheeksquintright",
                "nosesneerleft",
                "nosesneerright",
                "tongueout",
            };
    }

    class ALX_ARKIT_Utils
    {
        public static bool isClipIdenticalTo(BlendShapeClip clip, BlendShapeClip compare_to)
        {
            if (
                (
                    (clip.name == compare_to.name)
                    || (clip.BlendShapeName == compare_to.BlendShapeName)
                ) && (clip.Preset == compare_to.Preset)
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class ALX_ARKIT_Tools
    {
        public static void ClearClips(BlendShapeAvatar target_BlendShapeAvatar)
        {
            if (target_BlendShapeAvatar != null)
                target_BlendShapeAvatar.Clips.Clear();
        }

        public static void AutoSetup(
            GameObject target_ScenePrefab,
            BlendShapeAvatar target_BlendShapeAvatar
        )
        {
            if ((target_ScenePrefab != null) && (target_BlendShapeAvatar != null))
            {
                string BlendShapeAvatarPath = AssetDatabase.GetAssetPath(target_BlendShapeAvatar);
                SerializedObject SerializedBlendShapeAvatar = new SerializedObject(
                    target_BlendShapeAvatar
                );

                List<int> removal_queue = new List<int>();
                for (int i = 0; i < target_BlendShapeAvatar.Clips.Count; i++)
                {
                    if (
                        SerializedBlendShapeAvatar
                            .FindProperty("Clips")
                            .GetArrayElementAtIndex(i)
                            .objectReferenceValue == null
                    )
                    {
                        Debug.LogWarning(
                            "null clip has been found on the blendshape avatar, remove it ([right-click] -> [delete array element] in list view on the blend shape avatar) and run auto-setup again to ensure ARKit is fully present"
                        );
                    }
                }
                for (int i = 0; i < removal_queue.Count; i++)
                {
                    SerializedBlendShapeAvatar
                        .FindProperty("Clips")
                        .DeleteArrayElementAtIndex(removal_queue[i] - i);
                }
                SerializedBlendShapeAvatar.ApplyModifiedProperties();

                foreach (string clip_name in ALX_ARKIT_Presets.arkit_blend_shape_names)
                {
                    BlendShapeClip clip;
                    string clip_path;

                    string[] clip_guids = AssetDatabase.FindAssets(
                        filter: clip_name,
                        searchInFolders: new string[]
                        {
                            BlendShapeAvatarPath.Replace(
                                $"{target_BlendShapeAvatar.name}.asset",
                                ""
                            ),
                        }
                    );

                    if (clip_guids.Length > 0)
                    {
                        clip_path = AssetDatabase.GUIDToAssetPath(clip_guids[0]);
                        clip = AssetDatabase.LoadAssetAtPath<BlendShapeClip>(clip_path);
                    }
                    else
                    {
                        clip_path =
                            $"{BlendShapeAvatarPath.Replace($"{target_BlendShapeAvatar.name}.asset", "")}{Path.DirectorySeparatorChar}{clip_name}.asset";
                        clip = ScriptableObject.CreateInstance<BlendShapeClip>();

                        AssetDatabase.CreateAsset(clip, clip_path);
                    }

                    if (clip != null)
                    {
                        clip.name = clip_name;

                        SerializedObject serializedClip = new SerializedObject(clip);
                        serializedClip.FindProperty("BlendShapeName").stringValue = clip_name;

                        SkinnedMeshRenderer[] meshes =
                            target_ScenePrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
                        List<BlendShapeBinding> bindings = new List<BlendShapeBinding>();

                        foreach (
                            SkinnedMeshRenderer mesh in meshes.Where(skinned_mesh =>
                                skinned_mesh.sharedMesh.blendShapeCount > 0
                            )
                        )
                        {
                            if (mesh.sharedMesh.GetBlendShapeIndex(clip_name) != -1)
                            {
                                SerializedProperty BlendShapeValues = serializedClip.FindProperty(
                                    "Values"
                                );

                                BlendShapeValues.ClearArray();
                                int values_index = BlendShapeValues.arraySize;

                                BlendShapeValues.InsertArrayElementAtIndex(values_index);
                                BlendShapeValues
                                    .GetArrayElementAtIndex(values_index)
                                    .FindPropertyRelative("RelativePath")
                                    .stringValue = mesh.name;
                                BlendShapeValues
                                    .GetArrayElementAtIndex(values_index)
                                    .FindPropertyRelative("Index")
                                    .intValue = mesh.sharedMesh.GetBlendShapeIndex(clip_name);
                                BlendShapeValues
                                    .GetArrayElementAtIndex(values_index)
                                    .FindPropertyRelative("Weight")
                                    .floatValue = 100.0f;
                            }
                            else
                            {
                                Debug.Log("no blendshape found with that name");
                            }
                        }
                        serializedClip.ApplyModifiedProperties();
                        SerializedBlendShapeAvatar.ApplyModifiedProperties();

                        if (
                            (target_BlendShapeAvatar.Clips != null)
                            && target_BlendShapeAvatar.Clips.Count > 0
                        )
                        {
                            foreach (BlendShapeClip blendShapeClip in target_BlendShapeAvatar.Clips)
                            {
                                if (blendShapeClip == null) { }
                            }
                        }

                        foreach (
                            string avatar_clip_name in ALX_ARKIT_Presets.arkit_blend_shape_names.Where(
                                preset_clip_name =>
                                    !target_BlendShapeAvatar
                                        .Clips.Select(x => x != null ? x.name : "MISSING")
                                        .Contains(preset_clip_name)
                            )
                        )
                        {
                            if (clip.name == avatar_clip_name)
                            {
                                int index = SerializedBlendShapeAvatar
                                    .FindProperty("Clips")
                                    .arraySize;
                                SerializedBlendShapeAvatar
                                    .FindProperty("Clips")
                                    .InsertArrayElementAtIndex(index);
                                SerializedBlendShapeAvatar
                                    .FindProperty("Clips")
                                    .GetArrayElementAtIndex(index)
                                    .objectReferenceValue = clip;
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Clip is null");
                    }
                }

                SerializedBlendShapeAvatar.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif
