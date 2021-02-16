#if UNITY_2018_3_OR_NEWER
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace AssetUsageFinder {
	public static class PrefabUtilities {
		public class PrefabProperties {
			public bool IsPartOfStage;
			public bool IsStageRoot;
			public bool IsPartOfInstance;
			public bool IsInstanceRoot;
			public GameObject NearestInstanceRoot;
			public bool IsAssetRoot;
			public bool IsPartOfPrefabAsset;
			public string Path;
			public GameObject PrefabAssetRoot;

			public bool IsSceneObject => (!IsPartOfPrefabAsset && !IsPartOfStage);
			public bool IsPartOfAnyPrefab => Path != null;
			public bool IsRootOfAnyPrefab => IsAssetRoot || IsInstanceRoot || IsStageRoot;
		}

		public static PrefabProperties GetPrefabProperties(GameObject gameObject) {
			var p = new PrefabProperties();

			p.IsPartOfPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(gameObject);

			if (!p.IsPartOfPrefabAsset)
				p.IsPartOfPrefabAsset = !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(gameObject));

			GameObject nerestInstanceRoot = p.NearestInstanceRoot = PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);
			p.IsPartOfInstance = (nerestInstanceRoot != null);
			p.IsInstanceRoot = (gameObject == nerestInstanceRoot);

			if (p.IsPartOfPrefabAsset) {
				p.PrefabAssetRoot = gameObject.transform.root.gameObject;
				p.IsAssetRoot = (gameObject == p.PrefabAssetRoot);
			}

			var editorPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (editorPrefabStage != null) {
				if (p.IsPartOfPrefabAsset == false)
					p.IsPartOfStage = true;

				if (p.IsPartOfStage && gameObject.transform.parent == null)
					p.IsStageRoot = true;
			}

			if (p.IsRootOfAnyPrefab) {
				if (p.IsStageRoot) {
					p.Path = editorPrefabStage.prefabAssetPath;
				}
				else if (p.IsInstanceRoot) {
					p.Path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
				}
				else if (p.IsAssetRoot) {
					p.Path = AssetDatabase.GetAssetPath(gameObject);
				}
			}
			else {
				if (p.IsPartOfStage) {
					p.Path = editorPrefabStage.prefabAssetPath;
				}
				else if (p.IsPartOfInstance) {
					p.Path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(p.NearestInstanceRoot);
				}
				else if (p.IsPartOfPrefabAsset) {
					p.Path = AssetDatabase.GetAssetPath(gameObject.transform.root.gameObject);
				}
			}

			return p;
		}
	}
}
#endif