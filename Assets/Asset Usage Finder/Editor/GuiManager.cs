using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AssetUsageFinder {
    [InitializeOnLoad]
    public class GuiManager : UnityEditor.AssetModificationProcessor {
        static string _version = "v4.0";

        static GuiManager() {
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }

        static void OnSceneSaved(Scene scene) { }

        public static string[] OnWillSaveAssets(string[] paths) {
            return paths;
        }


        static void InitCache() {
            Globals<CacheManager>.GetOrCreate(() => {
                var res = new CacheManager();
                res.Init();
                return res;
            });
        }

        #region Menu

        [MenuItem("Assets/− Find Usages in Project", false, 30)]
        static void FileMenu(MenuCommand command) {
            InitCache();
            
            var continueFinding = DoYouWantToSaveScene();
            if (!continueFinding) return;

            var pickupMessage = $"Please pick up a file from the project!";

            var selected = Selection.activeObject;
            var type = selected.GetType();

            if (selected == null ||
                type == typeof(DefaultAsset) ||
                type == typeof(SceneAsset)) {
                EditorUtility.DisplayDialog($"{_version}", $"{pickupMessage}", "Ok");
                return;
            }

            if (type == typeof(GameObject)) {
                var prefabProperties = PrefabUtilities.GetPrefabProperties(Selection.activeObject as GameObject);
                if (prefabProperties.IsPartOfStage || prefabProperties.IsSceneObject) {
                    EditorUtility.DisplayDialog($"{_version}", $"{pickupMessage}", "Ok");
                    return;
                }
            }

            EditorApplication.ExecuteMenuItem("File/Save Project");
            OpenFileWindow(selected);
            return;
        }


        [MenuItem("GameObject/− Find Usages in Scene && Stage", false, -1)]
        public static void SceneOrStageMenu(MenuCommand data) {
            InitCache();
            
            var message = $"Please pick up an object from the scene && stage!";

            var selected = Selection.activeObject;

            if (selected == null || !(selected is GameObject)) {
                EditorUtility.DisplayDialog($"{_version}", $"{message}", "Ok");
                return;
            }

            var continueFinding = DoYouWantToSaveScene();
            if (!continueFinding) return;

            var prefabProperties = PrefabUtilities.GetPrefabProperties(Selection.activeObject as GameObject);

            if (prefabProperties.IsPartOfStage) {
                OpenStageWindow(selected, prefabProperties.Path);
            }
            else if (prefabProperties.IsSceneObject) {
                OpenSceneWindow(selected, SceneManager.GetActiveScene().path);
            }
            else {
                EditorUtility.DisplayDialog($"{_version}", $"{message}", "Ok");
                return;
            }
        }

        [MenuItem("CONTEXT/Component/− Find Usages of Component", false, 159)]
        public static void FindReferencesToComponent(MenuCommand data) {
            InitCache();

            Object selected = data.context;
            if (!selected) return;

            var continueFinding = DoYouWantToSaveScene();
            if (!continueFinding) return;

            var scenePath = SceneManager.GetActiveScene().path;

            OpenSceneWindow(selected, scenePath);
        }

        static bool DoYouWantToSaveScene() {
            var scene = SceneManager.GetActiveScene();
            if (scene.isDirty || string.IsNullOrEmpty(scene.path)) {
                var response = EditorUtility.DisplayDialogComplex(
                    title: "Asset Usage Finder v4.0",
                    message: "Current scene is not saved yet!",
                    ok: "Save scene and find usages",
                    cancel: "Cancel usage finding",
                    alt: "Find without saving");
                switch (response) {
                    case 0: // ok
                        EditorApplication.ExecuteMenuItem("File/Save");
                        return true;
                    case 1: // cancel
                        return false;
                    case 2: // find without saving
                        return true;
                    default:
                        return true;
                }
            }

            return true;
        }

        #endregion Menu

        #region InitWindow

        static void OpenFileWindow(Object selected) {
            var finder = new FileDependencyFinder(selected);

            var window = ScriptableObject.CreateInstance<FileDependencyWindow>();
            window.Init(finder);
            var p = window.position;
            p.size = DependencyWindow.StyleInstance.Size;
            window.position = p;
            window.Show();
        }

        public static void OpenSceneWindow(Object target, string scenePath) {
            var finder = new InSceneDependencyFinder(target, scenePath);
            var window = ScriptableObject.CreateInstance<SceneDependencyWindow>();
            window.Init(finder);
            window.Show();
        }

        static void OpenStageWindow(Object target, string stagePath) {
            var finder = new InStageDependencyFinder(target, stagePath);
            var window = ScriptableObject.CreateInstance<StageDependencyWindow>();
            window.Init(finder);
            window.Show();
        }

        #endregion InitWindow
    }
}