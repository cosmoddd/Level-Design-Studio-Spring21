using System;
using UnityEngine;

namespace AssetUsageFinder.Styles {
    [Serializable]
    class ContentStylePair {
        public GUIContent Content = new GUIContent();
        public GUIStyle Style = new GUIStyle();
    }
}