using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetUsageFinder {
    [Serializable]
    sealed class InSceneDependencyFinder : DependencyAbstractFinder {
        [SerializeField] string _scenePath;

        public InSceneDependencyFinder(Object target, string scenePath) {
            Target = new SearchTarget(target, FindModeEnum.Scene, scenePath);
            _scenePath = scenePath;
            Title = scenePath;

            var name = target is Component ? target.GetType().Name : target.name;

            TabContent = new GUIContent {
                text = name,
                image = AssetPreview.GetMiniTypeThumbnail(Target.Target.GetType()) ?? AssetPreview.GetMiniThumbnail(Target.Target)
            };

            FindDependencies();
        }

        public override void FindDependencies() {
            var dependenciesInScene = DependencyFinderEngine.GetDependenciesInScene(Target);
            Dependencies = Group(dependenciesInScene).ToArray();
        }


        public override DependencyAbstractFinder Nest(Object o) {
            return new InSceneDependencyFinder(o, _scenePath) {Parent = this};
        }
    }
}