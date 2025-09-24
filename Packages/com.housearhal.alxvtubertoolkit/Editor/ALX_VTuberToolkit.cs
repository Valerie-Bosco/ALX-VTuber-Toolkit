#if UNITY_EDITOR

using System.Collections.Generic;
using System.Text.RegularExpressions;
using ALX_ARKIT;
using Swan;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using VRM;

namespace ALX_VTuber_Toolkit
{
    public class STYLE
    {
        public static Length BORDER_RADIUS = new Length(3, LengthUnit.Pixel);
    }

    public class ALX_VTuberToolkit : EditorWindow
    {
        private VisualElement DocumentRoot;

        [MenuItem("ALX 3D/ALX VTuber Toolkit")]
        private static void ShowWindow()
        {
            GetWindow<ALX_VTuberToolkit>("ALX VTuber Toolkit");
        }

        public GameObject VRM_TargetPrefab;
        public BlendShapeAvatar VRM_TargetBlendShapeAvatar;

        static GameObject MATERIALS_TargetPrefab;
        private List<ALX_MaterialData> MATERIALS_TargetPrefab_material_entries =
            new List<ALX_MaterialData>();

        enum PAGE
        {
            VRM = 0,
            MATERIALS = 1,
        }

        Dictionary<PAGE, VisualElement> pages = new Dictionary<PAGE, VisualElement>();

        private void SwitchUIPage(PAGE page)
        {
            foreach (var pair in pages)
            {
                if (pair.Key != page)
                {
                    pair.Value.style.display = DisplayStyle.None;
                }
                else
                {
                    pair.Value.style.display = DisplayStyle.Flex;
                }
            }
        }

        public struct ALX_MaterialData
        {
            public string name;
            public string shader_name;
            public GUID guid;
        }

        class ALX_MaterialData_VisualElement : VisualElement
        {
            public VisualElement root_element;
            public Foldout title_foldout;
            public Button ping_button;
            public GUID guid;

            public ALX_MaterialData_VisualElement()
            {
                root_element = new VisualElement()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                        backgroundColor = new Color(0.345098f, 0.345098f, 0.345098f, 1),

                        borderTopLeftRadius = STYLE.BORDER_RADIUS,
                        borderBottomLeftRadius = STYLE.BORDER_RADIUS,
                        borderTopRightRadius = STYLE.BORDER_RADIUS,
                        borderBottomRightRadius = STYLE.BORDER_RADIUS,

                        marginTop = new Length(3, LengthUnit.Pixel),
                        marginBottom = new Length(3, LengthUnit.Pixel),
                    },
                };
                this.Add(root_element);

                title_foldout = new Foldout() { value = false, style = { flexGrow = 1 } };
                Button ping_button = new Button(() =>
                {
                    if (guid != null)
                    {
                        EditorGUIUtility.PingObject(
                            AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                                AssetDatabase.GUIDToAssetPath(guid)
                            )
                        );
                    }
                })
                {
                    text = "ping",
                    style =
                    {
                        flexGrow = 1,
                        alignSelf = Align.Center,
                        justifyContent = Justify.Center,
                        right = 0,

                        maxWidth = 64,
                    },
                };

                root_element.Add(title_foldout);
                root_element.Add(ping_button);
            }
        }

        #region makeItem/bindItem
        VisualElement Material_ListView_makeItem()
        {
            return new ALX_MaterialData_VisualElement();
        }

        private void Material_ListView_bindItem(VisualElement element, int index)
        {
            try
            {
                ALX_MaterialData_VisualElement material_element =
                    (ALX_MaterialData_VisualElement)element;

                material_element.guid = MATERIALS_TargetPrefab_material_entries[index].guid;

                material_element.title_foldout.text =
                    $"{MATERIALS_TargetPrefab_material_entries[index].name} | [{MATERIALS_TargetPrefab_material_entries[index].shader_name}]";
            }
            catch { }
        }

        #endregion

        private void OnEnable()
        {
            DocumentRoot = rootVisualElement;
        }

        private void CreateGUI()
        {
            StyleSheet style_sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.housearhal.alxvtubertoolkit/Editor/StyleSheet.uss"
            );

            DocumentRoot.styleSheets.Add(style_sheet);

            ScrollView main_scrollview = new ScrollView(ScrollViewMode.Vertical)
            {
                horizontalScrollerVisibility = ScrollerVisibility.Hidden,
                style = { flexGrow = 1 },
            };
            DocumentRoot.Add(main_scrollview);

            // WINDOWS
            VisualElement vrm_window_visual_root = new VisualElement();
            vrm_window_visual_root.style.top = 0;
            vrm_window_visual_root.style.bottom = 0;
            VisualElement materials_window_visual_root = new VisualElement();
            pages.Add(PAGE.VRM, vrm_window_visual_root);
            pages.Add(PAGE.MATERIALS, materials_window_visual_root);

            #region HEADER

            // TOOLBAR
            Toolbar page_selector_toolbar = new Toolbar();
            main_scrollview.Add(page_selector_toolbar);

            // TOOLBAR BUTTON VRM/MATERIALS
            ToolbarButton vrm_view_button = new ToolbarButton()
            {
                text = "VRM",
                tabIndex = 0,
                style = { flexGrow = 1, color = Color.white },
            };
            ;
            ToolbarButton materials_view_button = new ToolbarButton()
            {
                text = "Materials",
                tabIndex = 1,
                style = { flexGrow = 1, color = Color.white },
            };
            ;

            EventCallback<ClickEvent> vrm_toolbar_onclick = (click_event) =>
            {
                SwitchUIPage(PAGE.VRM);
                vrm_view_button.style.backgroundColor = new Color(0.2196f, 0.2196f, 0.2196f, 1);
                materials_view_button.style.backgroundColor = new Color(
                    0.1568628f,
                    0.1568628f,
                    0.1568628f,
                    1
                );
            };

            EventCallback<ClickEvent> materials_toolbar_onclick = (click_event) =>
            {
                SwitchUIPage(PAGE.MATERIALS);
                vrm_view_button.style.backgroundColor = new Color(
                    0.1568628f,
                    0.1568628f,
                    0.1568628f,
                    1
                );
                materials_view_button.style.backgroundColor = new Color(
                    0.2196f,
                    0.2196f,
                    0.2196f,
                    1
                );
            };

            SwitchUIPage(PAGE.VRM);
            vrm_view_button.style.backgroundColor = new Color(0.2196f, 0.2196f, 0.2196f, 1);
            materials_view_button.style.backgroundColor = new Color(
                0.1568628f,
                0.1568628f,
                0.1568628f,
                1
            );

            vrm_view_button.RegisterCallback<ClickEvent>(vrm_toolbar_onclick);
            materials_view_button.RegisterCallback<ClickEvent>(materials_toolbar_onclick);

            page_selector_toolbar.Add(vrm_view_button);
            page_selector_toolbar.Add(materials_view_button);

            # endregion

            main_scrollview.Add(vrm_window_visual_root);
            main_scrollview.Add(materials_window_visual_root);

            #region BODY

            // VRM - ARKit
            ObjectField vrm_target_prefab_field = new ObjectField(label: "Prefab:")
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true,
            };
            vrm_target_prefab_field.RegisterValueChangedCallback(
                (change_event) =>
                {
                    VRM_TargetPrefab = (GameObject)change_event.newValue;
                }
            );

            ObjectField vrm_target_blendshape_avatar_field = new ObjectField(
                label: "BlendShape Avatar"
            )
            {
                objectType = typeof(BlendShapeAvatar),
                allowSceneObjects = false,
            };
            vrm_target_blendshape_avatar_field.RegisterValueChangedCallback(
                (change_event) =>
                {
                    VRM_TargetBlendShapeAvatar = (BlendShapeAvatar)change_event.newValue;
                }
            );

            Button auto_setup_button = new Button(() =>
            {
                ALX_ARKIT_Tools.AutoSetup(VRM_TargetPrefab, VRM_TargetBlendShapeAvatar);
            })
            {
                text = "Auto-Setup",
            };
            Button clear_clips_button = new Button(() =>
            {
                ALX_ARKIT_Tools.ClearClips(VRM_TargetBlendShapeAvatar);
            })
            {
                text = "Clear Clips",
            };

            vrm_window_visual_root.Add(vrm_target_prefab_field);
            vrm_window_visual_root.Add(vrm_target_blendshape_avatar_field);
            vrm_window_visual_root.Add(auto_setup_button);
            vrm_window_visual_root.Add(clear_clips_button);

            // MATERIALS - GENERAL
            ObjectField materials_target_prefab_field = new ObjectField(label: "Prefab:")
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true,
            };
            Foldout material_list_foldout = new Foldout()
            {
                text = "Materials:",
                style = { flexGrow = 1 },
            };
            ListView material_list_view = new ListView()
            {
                makeItem = Material_ListView_makeItem,

                bindItem = Material_ListView_bindItem,
                itemsSource = MATERIALS_TargetPrefab_material_entries,
                selectionType = SelectionType.None,
                style = { flexGrow = 1, maxHeight = new Length(270, LengthUnit.Pixel) },
                fixedItemHeight = 270 / 10,
            };

            materials_target_prefab_field.RegisterValueChangedCallback(
                (change_event) =>
                {
                    MATERIALS_TargetPrefab = (GameObject)change_event.newValue;
                    MATERIALS_TargetPrefab_material_entries.Clear();
                    MATERIALS_TargetPrefab_material_entries.AddRange(
                        GET_Prefab_Materials(MATERIALS_TargetPrefab)
                    );
                    material_list_view.Rebuild();
                }
            );
            material_list_foldout.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 1)
                {
                    material_list_foldout.value = !material_list_foldout.value;
                }
            });

            materials_window_visual_root.Add(materials_target_prefab_field);
            materials_window_visual_root.Add(material_list_foldout);
            material_list_foldout.Add(material_list_view);

            #endregion


            #region FOOTER

            VisualElement links_footer = new VisualElement()
            {
                style = { flexDirection = FlexDirection.Row },
            };

            DocumentRoot.Add(links_footer);

            Button discord_button = new Button(() =>
            {
                Application.OpenURL("https://discord.com/invite/44fSbYrzZd");
            })
            {
                text = "Discord",
                style =
                {
                    flexGrow = 1,
                    maxHeight = 20,
                    alignItems = Align.Center,
                },
            };

            Button github_button = new Button(() =>
            {
                Application.OpenURL("https://github.com/Valerie-Bosco/ALX-VTuber-Toolkit");
            })
            {
                text = "Github",
                style =
                {
                    flexGrow = 1,
                    maxHeight = 20,
                    alignItems = Align.Center,
                },
            };

            links_footer.Add(discord_button);
            links_footer.Add(github_button);

            #endregion
        }

        private void OnGUI() { }

        static List<ALX_MaterialData> GET_Prefab_Materials(GameObject target_prefab)
        {
            List<ALX_MaterialData> materials = new List<ALX_MaterialData>();

            if (target_prefab != null)
            {
                foreach (var material in _materials_from_prefab(target_prefab))
                {
                    GUID material_guid = AssetDatabase.GUIDFromAssetPath(
                        AssetDatabase.GetAssetPath(material)
                    );
                    string match = Regex
                        .Match(
                            material.shader.name,
                            @"(Poiyomi)\s+[A-Z].*(?=\/)|(lil[A-Z].*)(?=\/)*"
                        )
                        .Value;

                    materials.Add(
                        new ALX_MaterialData()
                        {
                            name = material.name,
                            shader_name = match == "" ? material.shader.name : match,
                            guid = material_guid,
                        }
                    );
                }
            }
            return materials;
        }

        static List<UnityEngine.Material> _materials_from_prefab(GameObject target_prefab)
        {
            if (target_prefab != null)
            {
                List<UnityEngine.Material> materials = new List<UnityEngine.Material>();
                foreach (
                    SkinnedMeshRenderer mesh_renderer in target_prefab.GetComponentsInChildren<SkinnedMeshRenderer>()
                )
                {
                    foreach (UnityEngine.Material material in mesh_renderer.sharedMaterials)
                    {
                        if (!materials.Contains(material))
                        {
                            materials.Add(material);
                        }
                    }
                }
                foreach (
                    MeshRenderer mesh_renderer in target_prefab.GetComponentsInChildren<MeshRenderer>()
                )
                {
                    foreach (UnityEngine.Material material in mesh_renderer.sharedMaterials)
                    {
                        if (!materials.Contains(material))
                        {
                            materials.Add(material);
                        }
                    }
                }

                return materials;
            }
            else
            {
                return null;
            }
        }
    }
}
#endif
