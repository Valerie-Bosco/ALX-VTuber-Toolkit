#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Swan;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;

namespace ALX_VTuber_Toolkit
{
    namespace Material
    {
        namespace DataTypes { }

        namespace Interface
        {
            class ALX_UIPreset_Material
            {
                static GameObject TargetPrefab;

                // MATERIAL LIST
                static bool b_open_material_list_foldout;
                static Vector2 material_scrollview_value;

                ListView material_ListView;

                VisualElement Material_ListView_makeItem()
                {
                    VisualElement element_root = new VisualElement();
                    Foldout header_foldout = new Foldout();

                    header_foldout.RegisterCallback<MouseDownEvent>(evt =>
                    {
                        if (evt.button == 1)
                        {
                            header_foldout.value = !header_foldout.value;
                        }
                    });

                    element_root.Add(header_foldout);

                    return element_root;
                }

                private void Material_ListView_bindItem(VisualElement element, int index) { }

                public void GUI()
                {
                    TargetPrefab = (GameObject)
                        EditorGUILayout.ObjectField(
                            label: "Prefab:",
                            obj: TargetPrefab,
                            objType: typeof(GameObject),
                            allowSceneObjects: true
                        );

                    GUIContent button_content = new GUIContent();
                    if (GUILayout.Button(button_content)) { }

                    b_open_material_list_foldout = EditorGUILayout.Foldout(
                        foldout: b_open_material_list_foldout,
                        content: "Materials:",
                        toggleOnLabelClick: true
                    );

                    if (b_open_material_list_foldout)
                    {
                        material_scrollview_value = EditorGUILayout.BeginScrollView(
                            scrollPosition: material_scrollview_value,
                            alwaysShowHorizontal: false,
                            alwaysShowVertical: false
                        );

                        EditorGUILayout.EndScrollView();
                    }
                }
            }
        }

        struct Utilities { }
    }
}

#endif
