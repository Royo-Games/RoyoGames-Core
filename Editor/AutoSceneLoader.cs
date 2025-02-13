#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
#if UNITY_2019_1_OR_NEWER
#else
using UnityEngine.Experimental.UIElements;
#endif
using UnityEngine.UIElements;

namespace _Game.Scripts.Editor
{
    [InitializeOnLoad]
    public static class AutoSceneLoader
    {
        private static List<string> _sceneNameList;
        private static int _selectedSceneBuildIndex;

        [System.Serializable]
        public class SerializableHashSet
        {
            public List<ulong> items = new List<ulong>();

            public SerializableHashSet(HashSet<ulong> hashSet)
            {
                foreach (var item in hashSet)
                {
                    items.Add(item);
                }
            }

            public HashSet<ulong> ToHashSet()
            {
                HashSet<ulong> hashSet = new HashSet<ulong>();
                foreach (var item in items)
                {
                    hashSet.Add(item);
                }
                return hashSet;
            }
        }

        static AutoSceneLoader()
        {
            EditorApplication.playModeStateChanged += LogPlayModeState;
            Action selectTargetScene = OnDrawTargetScene;
            ToolbarExtender.RightToolbarGUI.Insert(0, selectTargetScene);

            //Action clearPlayerPrefs = OnDrawPlayerPrefsClear;
            //ToolbarExtender.RightToolbarGUI.Insert(1, clearPlayerPrefs);

            _sceneNameList = new List<string> { "Default" };
            var sceneCount = EditorBuildSettings.scenes.Length;
            for (var i = 0; i < sceneCount; i++)
            {
                var scene = EditorBuildSettings.scenes[i];
                _sceneNameList.Add(scene.path.Split('/').Last());
            }

            _selectedSceneBuildIndex = EditorPrefs.GetInt("target_scene_index", 0);
        }

        public static void OnDrawPlayerPrefsClear()
        {
#if UNITY_2019_3_OR_NEWER
            var clearPlayerPrefsBtn = EditorGUIUtility.IconContent("SaveFromPlay");
            clearPlayerPrefsBtn.tooltip = "Clear player prefs";

            if (GUILayout.Button(clearPlayerPrefsBtn))
            {
                PlayerPrefs.DeleteAll();
            }
#endif
        }
        public static void OnDrawTargetScene()
        {
#if UNITY_2019_3_OR_NEWER
            var tempIndex = _selectedSceneBuildIndex;
            _selectedSceneBuildIndex = EditorGUILayout.Popup(_selectedSceneBuildIndex, _sceneNameList.ToArray(),
                GUILayout.Width(100));
            EditorGUILayout.Space(20);

            if (GUI.changed && tempIndex != _selectedSceneBuildIndex)
            {
                EditorPrefs.SetInt("target_scene_index", _selectedSceneBuildIndex);
            }
#endif
        }
        private static void LogPlayModeState(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode
                && _selectedSceneBuildIndex != 0)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                var ids = SaveActiveHierarchy();
                EditorPrefs.SetString("active_scene_hierarchy", JsonUtility.ToJson(new SerializableHashSet(ids)));
                var path = EditorSceneManager.GetActiveScene().path;
                EditorPrefs.SetString("last_active_scene_path", path);
                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[_selectedSceneBuildIndex - 1].path);
            }
            else if (state == PlayModeStateChange.EnteredEditMode
                     && EditorPrefs.HasKey("last_active_scene_path"))
            {
                EditorSceneManager.OpenScene(EditorPrefs.GetString("last_active_scene_path"));
                EditorPrefs.DeleteKey("last_active_scene_path");

                var ids = JsonUtility.FromJson<SerializableHashSet>(
                    EditorPrefs.GetString("active_scene_hierarchy")).ToHashSet();
                ExpandHierarchy(ids);
            }
        }
        public static ulong GetGameObjectUniqueID(GameObject o)
        {
            return GlobalObjectId.GetGlobalObjectIdSlow(o).targetObjectId;
        }
        public static List<GameObject> GetExpandedGameObjects()
        {
            var sceneHierarchy = GetSceneHierarchy();
            if (sceneHierarchy == null) return new List<GameObject>();

            var methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");

            object result = methodInfo.Invoke(sceneHierarchy, new object[0]);

            return (List<GameObject>)result;
        }
        static void SetExpanded(GameObject go, bool expand)
        {
            var sceneHierarchy = GetSceneHierarchy();

            var methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
        }
        public static object GetSceneHierarchy()
        {
            object sceneHierarchy = null;
            try
            {
                EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
                sceneHierarchy = typeof(EditorWindow).Assembly
                    .GetType("UnityEditor.SceneHierarchyWindow")
                    .GetProperty("sceneHierarchy")
                    .GetValue(EditorWindow.focusedWindow);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n" + e.StackTrace);
                return null;
            }

            return sceneHierarchy;
        }
        public static void ExpandHierarchy(HashSet<ulong> idList)
        {
            var objects = GameObject.FindObjectsOfType<GameObject>();
            foreach (var o in objects)
            {
                var objectId = GlobalObjectId.GetGlobalObjectIdSlow(o).targetObjectId;
                if (idList.Contains(objectId))
                {
                    SetExpanded(o, true);
                }
            }
        }
        public static HashSet<ulong> SaveActiveHierarchy()
        {
            var expandedObjectIDList = new HashSet<ulong>();
            foreach (var gameObject in GetExpandedGameObjects())
            {
                expandedObjectIDList.Add(GetGameObjectUniqueID(gameObject));
            }

            return expandedObjectIDList;
        }
    }
    [InitializeOnLoad]
    public static class ToolbarExtender
    {
        private static int m_toolCount;
        private static GUIStyle m_commandStyle = null;

        public static readonly List<Action> LeftToolbarGUI = new List<Action>();
        public static readonly List<Action> RightToolbarGUI = new List<Action>();

        static ToolbarExtender()
        {
            EditorApplication.update += Init;
            EditorApplication.playModeStateChanged += OnChangePlayMode;
        }
        private static void OnChangePlayMode(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
                InitElements();
        }
        private static void Init()
        {
            EditorApplication.update -= Init;
            InitElements();
        }
        public static void InitElements()
        {
            Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

#if UNITY_2019_1_OR_NEWER
            string fieldName = "k_ToolCount";
#else
			string fieldName = "s_ShownToolIcons";
#endif

            FieldInfo toolIcons = toolbarType.GetField(fieldName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

#if UNITY_2019_3_OR_NEWER
            m_toolCount = toolIcons != null ? ((int)toolIcons.GetValue(null)) : 8;
#elif UNITY_2019_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((int) toolIcons.GetValue(null)) : 7;
#elif UNITY_2018_1_OR_NEWER
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 6;
#else
			m_toolCount = toolIcons != null ? ((Array) toolIcons.GetValue(null)).Length : 5;
#endif

            ToolbarCallback.OnToolbarGUI -= OnGUI;
            ToolbarCallback.OnToolbarGUI += OnGUI;
            ToolbarCallback.OnToolbarGUILeft -= GUILeft;
            ToolbarCallback.OnToolbarGUILeft += GUILeft;
            ToolbarCallback.OnToolbarGUIRight -= GUIRight;
            ToolbarCallback.OnToolbarGUIRight += GUIRight;
        }

#if UNITY_2019_3_OR_NEWER
        public const float space = 8;
#else
		public const float space = 10;
#endif
        public const float largeSpace = 20;
        public const float buttonWidth = 32;
        public const float dropdownWidth = 80;
#if UNITY_2019_1_OR_NEWER
        public const float playPauseStopWidth = 140;
#else
		public const float playPauseStopWidth = 100;
#endif

        public static void OnGUI()
        {
            // Create two containers, left and right
            // Screen is whole toolbar

            if (m_commandStyle == null)
            {
                m_commandStyle = new GUIStyle("CommandLeft");
            }

            var screenWidth = EditorGUIUtility.currentViewWidth;

            // Following calculations match code reflected from Toolbar.OldOnGUI()
            float playButtonsPosition = Mathf.RoundToInt((screenWidth - playPauseStopWidth) / 2);

            Rect leftRect = new Rect(0, 0, screenWidth, Screen.height);
            leftRect.xMin += space; // Spacing left
            leftRect.xMin += buttonWidth * m_toolCount; // Tool buttons
#if UNITY_2019_3_OR_NEWER
            leftRect.xMin += space; // Spacing between tools and pivot
#else
			leftRect.xMin += largeSpace; // Spacing between tools and pivot
#endif
            leftRect.xMin += 64 * 2; // Pivot buttons
            leftRect.xMax = playButtonsPosition;

            Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
            rightRect.xMin = playButtonsPosition;
            rightRect.xMin += m_commandStyle.fixedWidth * 3; // Play buttons
            rightRect.xMax = screenWidth;
            rightRect.xMax -= space; // Spacing right
            rightRect.xMax -= dropdownWidth; // Layout
            rightRect.xMax -= space; // Spacing between layout and layers
            rightRect.xMax -= dropdownWidth; // Layers
#if UNITY_2019_3_OR_NEWER
            rightRect.xMax -= space; // Spacing between layers and account
#else
			rightRect.xMax -= largeSpace; // Spacing between layers and account
#endif
            rightRect.xMax -= dropdownWidth; // Account
            rightRect.xMax -= space; // Spacing between account and cloud
            rightRect.xMax -= buttonWidth; // Cloud
            rightRect.xMax -= space; // Spacing between cloud and collab
            rightRect.xMax -= 78; // Colab

            // Add spacing around existing controls
            leftRect.xMin += space;
            leftRect.xMax -= space;
            rightRect.xMin += space;
            rightRect.xMax -= space;

            // Add top and bottom margins
#if UNITY_2019_3_OR_NEWER
            leftRect.y = 4;
            leftRect.height = 22;
            rightRect.y = 4;
            rightRect.height = 22;
#else
			leftRect.y = 5;
			leftRect.height = 24;
			rightRect.y = 5;
			rightRect.height = 24;
#endif

            if (leftRect.width > 0)
            {
                GUILayout.BeginArea(leftRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in LeftToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }

            if (rightRect.width > 0)
            {
                GUILayout.BeginArea(rightRect);
                GUILayout.BeginHorizontal();
                foreach (var handler in RightToolbarGUI)
                {
                    handler();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        private static void GUILeft()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in LeftToolbarGUI)
            {
                handler();
            }

            GUILayout.EndHorizontal();
        }

        private static void GUIRight()
        {
            GUILayout.BeginHorizontal();
            foreach (var handler in RightToolbarGUI)
            {
                handler();
            }

            GUILayout.EndHorizontal();
        }
    }
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold
            };
        }
    }
    public static class ToolbarCallback
    {
        public static Type m_toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        public static Type m_guiViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
        public static Type m_iWindowBackendType =
            typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");
        public static PropertyInfo m_windowBackend = m_guiViewType.GetProperty("windowBackend",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static PropertyInfo m_viewVisualTree = m_iWindowBackendType.GetProperty("visualTree",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
		public static PropertyInfo m_viewVisualTree = m_guiViewType.GetProperty("visualTree",
			BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        public static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static ScriptableObject m_currentToolbar;

        /// <summary>
        /// Callback for toolbar OnGUI method.
        /// </summary>
        public static Action OnToolbarGUI;
        public static Action OnToolbarGUILeft;
        public static Action OnToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            // Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
            if (m_currentToolbar == null)
            {
                // Find toolbar
                var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
                m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

                if (m_currentToolbar != null)
                {
#if UNITY_2021_1_OR_NEWER
                    var root = m_currentToolbar.GetType()
                        .GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
                    var rawRoot = root.GetValue(m_currentToolbar);
                    var mRoot = rawRoot as VisualElement;
                    RegisterCallback("ToolbarZoneLeftAlign", OnToolbarGUILeft);
                    RegisterCallback("ToolbarZoneRightAlign", OnToolbarGUIRight);

                    void RegisterCallback(string root, Action cb)
                    {
                        var toolbarZone = mRoot.Q(root);

                        var parent = new VisualElement()
                        {
                            style =
                            {
                                flexGrow = 1,
                                flexDirection = FlexDirection.Row,
                            }
                        };
                        var container = new IMGUIContainer();
                        container.onGUIHandler += () => { cb?.Invoke(); };
                        parent.Add(container);
                        toolbarZone.Add(parent);
                    }
#else
#if UNITY_2020_1_OR_NEWER
					var windowBackend = m_windowBackend.GetValue(m_currentToolbar);

					// Get it's visual tree
					var visualTree = (VisualElement) m_viewVisualTree.GetValue(windowBackend, null);
#else
					// Get it's visual tree
					var visualTree = (VisualElement) m_viewVisualTree.GetValue(m_currentToolbar, null);
#endif
					// Get first child which 'happens' to be toolbar IMGUIContainer
					var container = (IMGUIContainer) visualTree[0];

					// (Re)attach handler
					var handler = (Action) m_imguiContainerOnGui.GetValue(container);
					handler -= OnGUI;
					handler += OnGUI;
					m_imguiContainerOnGui.SetValue(container, handler);
#endif
                }
            }
        }

        static void OnGUI()
        {
            var handler = OnToolbarGUI;
            if (handler != null) handler();
        }
    }
}
#endif