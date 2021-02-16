using System;
using System.Linq;
using UnityEngine;

namespace AssetUsageFinder {
    [Serializable]
    public class AssetDependencies {
        public string[] DependencyGuids;
        public string HashString;

        [NonSerialized] Hash128 _hashCache;

        public Hash128 DependencyHash {
            get { return _hashCache.Equals(default(Hash128)) ? Hash128.Parse(HashString) : _hashCache; }
            set {
                _hashCache = value;
                HashString = value.ToString();
            }
        }

        public bool Contains(string guid) {
            return DependencyGuids.Any(d => StringComparer.InvariantCultureIgnoreCase.Equals(guid, d));
        }
    }
}