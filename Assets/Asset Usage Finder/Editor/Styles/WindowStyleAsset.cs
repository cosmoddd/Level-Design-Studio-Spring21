using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AssetUsageFinder.Styles {
    class WindowStyleAsset : ScriptableObject {
#pragma warning disable 0649
        public DependencyWindow.Style Pro;
        public DependencyWindow.Style Personal;
#pragma warning restore
        
#if !false
        [CustomEditor(typeof (WindowStyleAsset))]
        class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                EditorGUI.BeginChangeCheck();
                base.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                    InternalEditorUtility.RepaintAllViews();
            }
        }
#endif
    }
}