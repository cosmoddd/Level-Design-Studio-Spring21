using System;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Assertions;

namespace AssetUsageFinder {
    [Serializable]
    sealed class FileDependencyFinder : DependencyAbstractFinder {
        public class Pair {
            public string NicifiedPath;
            public string Path;
        }

        public Pair[] ScenePaths;

        public FileDependencyFinder(Object target) {
            Asr.IsTrue(target, "Asset you're trying to search is corrupted");
            Target = new SearchTarget(target, FindModeEnum.File);
            FindDependencies();
            var path = AssetDatabase.GetAssetPath(Target.Target);
            Title = path;
            TabContent = new GUIContent {
                text = target.name,
                image = AssetDatabase.GetCachedIcon(path)
            };
        }


        public override void FindDependencies() {
            var files = DependencyFinderEngine.GetFilesThatReference(Target);
            Dependencies = Group(files.Where(f => !(f.Target is SceneAsset)))
                .OrderBy(t => t.LabelContent.text, StringComparer.Ordinal)
                .ToArray();
        }

        public override DependencyAbstractFinder Nest(Object o) {
            return new FileDependencyFinder(o) {Parent = this};
        }
    }
}