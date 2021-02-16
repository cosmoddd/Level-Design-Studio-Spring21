using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AssetUsageFinder {
    static class DependencyFinderEngine {
        #region Private

        const string AssetsRootPath = "Assets/";
        static double _cachedTime;

        static bool NeedsUpdate {
            get {
                var t = _cachedTime;
                _cachedTime = EditorApplication.timeSinceStartup;
                var result = _cachedTime - t > 0.03;
                return result;
            }
        }


        static ResultRow GenerateResultRowByObject(SearchTarget target, Object c, bool isScene = true) {
            if (target.Check(c)) return null;
            if (target.Root == c) return null;
            var so = new SerializedObject(c);
            var sp = so.GetIterator();
            ResultRow row = null;
            while (sp.Next(enterChildren: true)) {
                string transformPath = string.Empty;
                if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (!target.Check(sp.objectReferenceValue)) continue;
                if (row == null) {
                    row = new ResultRow {
                        Root = c,
                        Target = c,
                        SerializedObject = so
                    };

                    if (isScene) {
                        var component = c as Component;
                        if (component) {
                            row.Main = component.gameObject;
                        }
                        else {
                            var o = c as GameObject;
                            if (o != null) {
                                row.Main = o;
                            }
                        }

                        var go = row.Main as GameObject;
                        // Assert.NotNull(go);
                        row.LabelContent.text = AnimationUtility.CalculateTransformPath(go.transform, null);
                        row.LabelContent.image = AssetPreview.GetMiniThumbnail(go);
                    }
                    else {
                        var path = AssetDatabase.GetAssetPath(c);
                        row.Main = AssetDatabase.LoadMainAssetAtPath(path);

                        var mainType = PrefabUtility.GetPrefabAssetType(row.Main);

                        if (mainType != PrefabAssetType.NotAPrefab) {
                            var comp = row.Target as Component;
                            if (comp) {
                                try {
                                    transformPath = string.Format("{0}/", AnimationUtility.CalculateTransformPath(comp.transform, null)).Replace("/", "/\n");
                                }
                                catch {
                                    // ignored
                                }
                            }
                        }

                        row.LabelContent.text = path.Replace(AssetsRootPath, string.Empty);
                        row.LabelContent.image = AssetDatabase.GetCachedIcon(path);
                    }
                }

                Texture2D miniTypeThumbnail = row.Main == c ? null : AssetPreview.GetMiniThumbnail(c);

                row.Properties.Add(new ResultRow.PropertyData {
                    Property = sp.Copy(),
                    Content = new GUIContent {
                        image = miniTypeThumbnail,
                        text = Nicify(sp, sp.serializedObject.targetObject, row.Main, target),
                        tooltip = string.Format("{2}{0}.{1}", sp.serializedObject.targetObject.GetType().Name, sp.propertyPath, transformPath)
                    }
                });
            }

            if (row == null)
                so.Dispose();
            return row;
        }


        public static IEnumerable<(int state, string crumbs)> Traverse(AnimatorController controller) {
            for (var index = 0; index < controller.layers.Length; index++) {
                var controllerLayer = controller.layers[index];
                foreach (var i in Inner(controllerLayer.stateMachine, $"{controllerLayer.name}({index})"))
                    yield return i;
            }
        }

        static IEnumerable<(int state, string crumbs)> Inner(AnimatorStateMachine f, string crumbs) {
            foreach (var state in f.states)
                yield return (state.state.GetInstanceID(), crumbs);

            foreach (var child in f.stateMachines) {
                foreach (var tuple in Inner(child.stateMachine, $"{crumbs}/{child.stateMachine.name}"))
                    yield return tuple;
            }
        }

        // todo use GetInstanceID instead of unityobj refs
        static Dictionary<int, Dictionary<int, string>> _animCache = new Dictionary<int, Dictionary<int, string>>();

        static string GetBread(AnimatorController c, AnimatorState state) {
            if (!_animCache.TryGetValue(c.GetInstanceID(), out var res)) {
                res = new Dictionary<int, string>();
                _animCache.Add(c.GetInstanceID(), res);
                foreach (var (stateId, crumbs) in Traverse(c))
                    res.Add(stateId, crumbs);
            }

            if (res.TryGetValue(state.GetInstanceID(), out var crumb))
                return $"{crumb}/";
            return string.Empty;
        }

        static string Nicify(SerializedProperty sp, Object o, Object main, SearchTarget target) {
            //            return sp.propertyPath;

            string nice = string.Empty;
            switch (o) {
                case AnimatorController _: {
                    return Nice(sp.propertyPath);
                }

                case BlendTree blendTree: {
                    return $"{blendTree.name}({o.GetType().Name})";
                }

                case AnimatorState animatorState: {
                    if (main is AnimatorController animatorController) {
                        var bread = GetBread(animatorController, animatorState);
                        return $"{bread}{animatorState.name}";
                    }

                    break;
                }
                case StateMachineBehaviour smb: {
                    var ctx = AnimatorController.FindStateMachineBehaviourContext(smb);
                    if (ctx.Length == 0)
                        break;

                    var first = ctx[0];
                    var bread = string.Empty;
                    switch (first.animatorObject) {
                        case AnimatorStateMachine _:
                            // nothing
                            break;
                        case AnimatorState ast: {
                            bread = GetBread(first.animatorController, ast);
                            break;
                        }
                    }

                    return $"{bread}{first.animatorObject.name}";
                }
                case Material _:
                    nice = sp.displayName;
                    break;
                default: {
                    nice = Nice(sp.propertyPath);
                    break;
                }
            }

            nice = string.Format("{0}.{1}", o.GetType().Name, nice);
            return nice;
        }

        static string Nice(string path) {
            var nice = path.Replace(".Array.data", string.Empty);
            if (nice.IndexOf(".m_PersistentCalls.m_Calls", StringComparison.Ordinal) > 0) {
                nice = nice.Replace(".m_PersistentCalls.m_Calls", string.Empty)
                    .Replace(".m_Target", string.Empty);
            }


            nice = nice.Replace("m_", string.Empty);
            nice = nice.Split('.').Select(t => ObjectNames.NicifyVariableName(t).Replace(" ", string.Empty)).Aggregate((a, b) => a + "." + b);
            return nice;
        }

        public static IEnumerable<T> AsEnumerable<T>(T o) {
            yield return o;
        }

        #endregion

        #region Project

        static HashSet<string> GetDependencies(Object activeObject) {
            var targetPath = AssetDatabase.GetAssetPath(activeObject);
            var targetGuid = AssetDatabase.AssetPathToGUID(targetPath);
            var objectGuids = AssetDatabase.FindAssets("t:Object");
            var results = new HashSet<string>(StringComparer.Ordinal);
            var total = objectGuids.LongLength;
            var cache = Globals<CacheManager>.Get();
            try {
                for (int i = 0; i < total; i++) {
                    var path = AssetDatabase.GUIDToAssetPath(objectGuids[i]);
                    var res = cache.Get(path, objectGuids[i]);

                    if (path.Contains(".unity"))
                        continue;

                    if (res.Contains(targetGuid))
                        results.Add(path);

                    if (!NeedsUpdate) continue;

                    if (EditorUtility.DisplayCancelableProgressBar("Generating cache", "Searching for file usages", (float) i / total))
                        break;
                }
            }
            finally {
                cache.Serialize();
                EditorUtility.ClearProgressBar();
            }

            results.Remove(targetPath);
            return results;
        }

        public static IEnumerable<ResultRow> GetFilesThatReference(SearchTarget target) {
            return GetDependencies(target.Target)
                .SelectMany(p => AssetDatabase.LoadAllAssetsAtPath(p))
                .Where(t => t && !(t is DefaultAsset) && !(t is Transform))
                .Select(asset => GenerateResultRowByObject(target, asset, false))
                .Where(row => row != null);
        }

        public static HashSet<string> GetScenesThatContain(Object activeObject) {
            var targetPath = AssetDatabase.GetAssetPath(activeObject);
            var targetGuid = AssetDatabase.AssetPathToGUID(targetPath);

            var results = new HashSet<string>(StringComparer.Ordinal);
            var sceneGuids = AssetDatabase.FindAssets("t:Scene");

            var total = sceneGuids.LongLength;

            var cache = Globals<CacheManager>.Get();
            try {
                for (int i = 0; i < total; i++) {
                    var path = AssetDatabase.GUIDToAssetPath(sceneGuids[i]);
                    var res = cache.Get(path, sceneGuids[i]);

                    if (res.Contains(targetGuid))
                        results.Add(path);

                    if (!NeedsUpdate) continue;

                    if (EditorUtility.DisplayCancelableProgressBar("Searching for file usages in scenes..", path, (float) i / total))
                        break;
                }
            }
            finally {
                cache.Serialize();
                EditorUtility.ClearProgressBar();
            }

            EditorUtility.ClearProgressBar();
            results.Remove(targetPath);
            return results;
        }

        #endregion Project

        public static IEnumerable<ResultRow> GetDependenciesInScene(SearchTarget target) {
            var referencedBy = new List<ResultRow>();

            for (int ii = 0; ii < SceneManager.sceneCount; ii++) {
                var currentScene = SceneManager.GetSceneAt(ii);

                if (!currentScene.IsValid() || !currentScene.isLoaded)
                    continue;

                var allObjects = currentScene
                    .GetRootGameObjects()
                    .SelectMany(g => {
                        if (g != target.Target)
                            return g.GetComponentsInChildren<Component>(true).Where(FilterComponents).Union(AsEnumerable(g as Object));
                        return g.GetComponentsInChildren<Component>(true).Where(FilterComponents);
                    }).ToArray();
                var total = allObjects.Length;

                var step = total / 5;

                try {
                    if (target.Nested.TryGet(out var nested))
                        foreach (var n in nested) {
                            var searchTarget = new SearchTarget(n, FindModeEnum.Scene);
                            for (var i = 0; i < total; i++) {
                                var comp = allObjects[i];
                                var res = GenerateResultRowByObject(target, comp);

                                if (res != null && target.Target != res.Main)
                                    referencedBy.Add(res);


                                var resultNested = GenerateResultRowByObject(searchTarget, comp);
                                if (resultNested != null && searchTarget.Target != resultNested.Main)
                                    referencedBy.Add(resultNested);

                                if (step == 0) continue;
                                if (i % step != 0) continue;
                                if (EditorUtility.DisplayCancelableProgressBar("Searching for file usages in current scene..", target.Target.name, (float) i / total))
                                    break;
                            }
                        }
                }
                finally {
                    EditorUtility.ClearProgressBar();
                }
            }

            return referencedBy.Distinct().ToList();
        }

        static bool FilterComponents(Component c) {
            switch (c) {
                case Transform _:
                    return false;
                default:
                    return true;
            }
        }

        public static IEnumerable<ResultRow> GetDependenciesInStage(SearchTarget target, PrefabStage stage) {
            var referencedBy = new List<ResultRow>();

            var allObjects = stage.scene
                .GetRootGameObjects()
                .SelectMany(g => {
                    if (g != target.Target)
                        return g.GetComponentsInChildren<Component>(true).Where(FilterComponents).OfType<Object>().Union(AsEnumerable(g as Object));
                    return g.GetComponentsInChildren<Component>(true).Where(FilterComponents).OfType<Object>();
                }).ToArray();
            var total = allObjects.Length;
            for (int i = 0; i < total; i++) {
                var comp = allObjects[i];
                var res = GenerateResultRowByObject(target, comp);
                if (res != null) {
                    referencedBy.Add(res);
                }

                if (EditorUtility.DisplayCancelableProgressBar("Searching for file usages in current scene..", target.Target.name, (float) i / total))
                    break;
            }

            EditorUtility.ClearProgressBar();


            return referencedBy;
        }
    }
}