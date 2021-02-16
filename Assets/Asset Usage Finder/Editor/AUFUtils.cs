using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetUsageFinder {
    public static class AufUtils {
        public static Object[] LoadAllAssetsAtPath(string assetPath) {
            // prevents error "Do not use readobjectthreaded on scene objects!"
            return typeof(SceneAsset) == AssetDatabase.GetMainAssetTypeAtPath(assetPath)
                ? new[] {AssetDatabase.LoadMainAssetAtPath(assetPath)}
                : AssetDatabase.LoadAllAssetsAtPath(assetPath);
        }

        public static T FirstOfType<T>() where T : Object {
            var typeName = typeof(T).Name;

            var guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeName));
            if (!guids.Any()) {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }

            Asr.IsTrue(guids.Length > 0, string.Format("No '{0}' assets found", typeName));
            return guids.Select(g => AssetDatabase.GUIDToAssetPath(g)).Select(t => (T) AssetDatabase.LoadAssetAtPath(t, typeof(T))).First();
        }
    }
}