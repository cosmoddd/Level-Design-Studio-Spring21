using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Linq;
using AssetUsageFinder.Styles;
using UnityEditor.SceneManagement;

namespace AssetUsageFinder {
    class DependencyWindow : EditorWindow {
        Vector2 _scrollPos;
        [SerializeField] DependencyAbstractFinder _data;
        [SerializeField] protected FindModeEnum _findMode;
        bool _expandFiles = true;
        bool _expandScenes = true;
        static GUIContent _sceneIcon;
        Rect _popupButtonRect;
        PrevClick _click;
        List<Action> _postponedActions;

        [Serializable]
        public class Style {
            public ContentStylePair LookupBtn = new ContentStylePair();
            public GUIStyle TabBreadcrumb0 = new GUIStyle();
            public GUIStyle TabBreadcrumb1 = new GUIStyle();
            public GUIStyle RowMainAssetBtn = new GUIStyle();
            public Vector2 Size = new Vector2(250f, 800f);
            public GUIStyle RowLabel = new GUIStyle();

            public static Style FindSelf() {
                var res = AufUtils.FirstOfType<WindowStyleAsset>();
                return EditorGUIUtility.isProSkin ? res.Pro : res.Personal;
            }
        }

        public static Style StyleInstance => Globals<Style>.GetOrCreate(Style.FindSelf);

        void OnEnable() {
            _postponedActions = new List<Action>();
        }

        void BreadCrumbs() {
            var parents = _data.Parents();
            parents.Reverse();
            var w = 0f;
            {
                using (new EditorGUILayout.VerticalScope()) {
                    EditorGUILayout.BeginHorizontal();
                    for (var i = 0; i < parents.Count; i++) {
                        var parent = parents[i];
                        var style = i == 0 ? StyleInstance.TabBreadcrumb0 : StyleInstance.TabBreadcrumb1;

                        var styleWidth = style.CalcSize(parent.TabContent).x;
                        if (w > EditorGUIUtility.currentViewWidth - styleWidth) {
                            w = 0f;
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                        }

                        w += styleWidth;

                        if (i == parents.Count - 1) {
                            var res = GUILayout.Toggle(true, parent.TabContent, style);
                            if (!res)
                                EditorGUIUtility.PingObject(parent.Target.Target);
                        }
                        else if (GUILayout.Button(parent.TabContent, style)) {
                            EditorGUIUtility.PingObject(parent.Target.Target);
                            _postponedActions.Add(() => { Init(parent); });
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        void Update() {
            if (!_postponedActions.Any()) return;
            foreach (var a in _postponedActions)
                a();
            _postponedActions.Clear();
        }

        void OnGUI() {
            if (_postponedActions == null || _data == null || (Event.current != null && Event.current.keyCode == KeyCode.Escape)) {
                _postponedActions = new List<Action>();
                _postponedActions.Add(() => Close());
                return;
            }

            EditorGUILayout.BeginVertical();
            {
                BreadCrumbs();
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
                {
                    EditorGUILayout.Space();
                    ShowDependencies(_data.Dependencies);
                }
                EditorGUILayout.EndScrollView();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            Footer();
        }

        void Footer() {
            if (Event.current.type != EventType.Repaint) return;
            _popupButtonRect = GUILayoutUtility.GetLastRect();
            _popupButtonRect.position += position.position;
        }

        public void Init(DependencyAbstractFinder d) {
            _data = d;
            // _labelMaxWidth = CalculateContentMaxWidth(EditorStyles.label, _data.Dependencies.SelectMany(dd => dd.Properties.Select(p => p.Content)));

            // var t = new[] {_data.Target.Root};
            // if (_data.Target.Nested.TryGet(out var nested))
 
            titleContent = new GUIContent($"{FindMode.GetWindowTitleByFindMode(_findMode)}");
            titleContent.tooltip = _data.Title;
        }

        void ShowDependencies(ResultRow[] dependencies) {
            var nDeps = dependencies.Count();
            _expandFiles = EditorGUILayout.Foldout(_expandFiles, $"{FindMode.GetContentByFindMode(_findMode)}: [{nDeps}]");

            if (_findMode == FindModeEnum.File) {
                if (_data.Target.Scene.IsValid() && !_data.Target.Scene.isLoaded)
                    return;
            }

            if (_expandFiles) {
                if (nDeps > 0) {
                    foreach (var dependency in dependencies)
                        if (dependency != null && dependency.SerializedObject != null && dependency.SerializedObject.targetObject != null)
                            DrawRow(dependency);
                        else
                            this.Close();
                }
                else {
                    EditorGUILayout.LabelField("No file dependencies found.");
                }
            }

            EditorGUILayout.Space();

            var fileDep = _data as FileDependencyFinder;

            if (fileDep == null)
                return;

            if (fileDep.ScenePaths == null) {
                fileDep.ScenePaths = DependencyFinderEngine.GetScenesThatContain(_data.Target.Target)
                    .Select(p => new FileDependencyFinder.Pair {Path = p, NicifiedPath = p.Replace("Assets/", string.Empty)}).ToArray();
            }

            var nScenes = fileDep.ScenePaths.Count();
            _expandScenes = EditorGUILayout.Foldout(_expandScenes, $"In Scenes: [{nScenes}]");

            if (!_expandScenes) return;
            if (nScenes > 0) {
                foreach (var p in fileDep.ScenePaths) {
                    using (new EditorGUILayout.HorizontalScope()) {
                        SceneIcon.text = p.NicifiedPath;

                        if (GUILayout.Button(SceneIcon, EditorStyles.label, GUILayout.Height(16f), GUILayout.MaxWidth(20f + p.NicifiedPath.Length * 7f)))
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(p.Path));

                        if (!GUILayout.Button("Open scene & search"))
                            continue;

                        var sceneToOpen = SceneManager.GetSceneByPath(p.Path);
                        if (sceneToOpen.isLoaded) {
                            GuiManager.OpenSceneWindow(_data.Target.Target, p.Path);
                        }
                        else {
                            var currentScene = EditorSceneManager.GetActiveScene();

                            if (currentScene.isDirty && EditorUtility.DisplayDialog(
                                    $"Unsaved changes",
                                    $"You are going to open and search in scene [{p.Path}]\n" +
                                    $"but you have unsaved changes at the scene [{currentScene.name}]",
                                    $"Stay at current scene and cancel search", $"Discard changes and search"))
                                return;

                            EditorSceneManager.OpenScene(p.Path);
                            GuiManager.OpenSceneWindow(_data.Target.Target, p.Path);
                        }
                    }
                }
            }
            else {
                EditorGUILayout.LabelField("No scene dependencies found.");
            }
        }

        struct PrevClick {
            Object _target;
            float _timeClicked;

            public PrevClick(Object target) {
                _target = target;
                _timeClicked = Time.realtimeSinceStartup;
            }

            const float DoubleClickTime = 0.5f;

            public bool IsDoubleClick(Object o) {
                return _target == o && Time.realtimeSinceStartup - _timeClicked < DoubleClickTime;
            }
        }

        void DrawRow(ResultRow dependency) {
            var id = dependency.Main.GetInstanceID();

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                using (new EditorGUILayout.HorizontalScope()) {
                    if (GUILayout.Button(dependency.LabelContent, StyleInstance.RowMainAssetBtn)) {
                        if (_click.IsDoubleClick(dependency.Main))
                            Selection.activeObject = dependency.Main;
                        else {
                            EditorGUIUtility.PingObject(dependency.Main);
                        }

                        _click = new PrevClick(dependency.Main);
                    }

                    if (GUILayout.Button(StyleInstance.LookupBtn.Content, StyleInstance.LookupBtn.Style)) {
                        _postponedActions.Add(() =>
                            Init(_data.Nest(dependency.Main)));
                    }
                }

                dependency.SerializedObject.Update();
                EditorGUI.BeginChangeCheck();


                if (dependency.Target) {
                    foreach (var prop in dependency.Properties) {
                        using (new EditorGUILayout.HorizontalScope()) {
                            var locked = prop.Property.objectReferenceValue is MonoScript;
                            var f = GUI.enabled;

                            if (locked) GUI.enabled = false;

                            EditorGUILayout.LabelField(prop.Content, StyleInstance.RowLabel, GUILayout.MaxWidth(this.position.width *.8f));
                            EditorGUILayout.PropertyField(prop.Property, GUIContent.none, true, GUILayout.MinWidth(this.position.width *.2f));

                            if (locked) GUI.enabled = f;
                        }
                    }
                }

                if (EditorGUI.EndChangeCheck())
                    dependency.SerializedObject.ApplyModifiedProperties();
            }
        }

        static GUIContent SceneIcon {
            get { return _sceneIcon ?? (_sceneIcon = new GUIContent(AssetPreview.GetMiniTypeThumbnail(typeof(SceneAsset)))); }
        }
    }
}