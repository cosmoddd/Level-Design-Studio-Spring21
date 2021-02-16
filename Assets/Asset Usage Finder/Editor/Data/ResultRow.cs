using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetUsageFinder {
    [Serializable]
    class ResultRow {
        [Serializable]
        public class PropertyData {
            public SerializedProperty Property;
            public GUIContent Content;
        }

        public Object Target;
        public SerializedObject SerializedObject;
        public List<PropertyData> Properties = new List<PropertyData>();
        public Object Root;
        public GUIContent LabelContent = new GUIContent();
        public GUIContent PropertyFieldContent = new GUIContent();
        public Object Main;
    }
}