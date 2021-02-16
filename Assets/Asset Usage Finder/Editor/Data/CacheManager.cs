using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetUsageFinder {
    public class CacheManager {
        Dictionary<string, AssetDependencies> _dict;
        HashSet<string> _used;

        // [MenuItem("Tools/LogPath")]
        static void Log() {
            Debug.Log(Application.temporaryCachePath);
        }

        public CacheManager() {
            _dict = new Dictionary<string, AssetDependencies>(StringComparer.InvariantCultureIgnoreCase);
            _used = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Init() {
            var serializable = Deserialize();
            _dict = serializable.OnDeserialize();
        }

        static SerializableData Deserialize() {
            SerializableData data;
            string json;

            if (!File.Exists(Path)) {
                // not exists - write new
                data = SerializableData.Empty();
                json = JsonUtility.ToJson(data);
                File.WriteAllText(Path, json);
            }
            else {
                // exists
                json = File.ReadAllText(Path);

                if (string.IsNullOrEmpty(json)) {
                    // but corrupted - overwrite with new
                    data = SerializableData.Empty();
                    json = JsonUtility.ToJson(data);
                    File.WriteAllText(Path, json);
                }

                data = JsonUtility.FromJson<SerializableData>(json);
                if (!SerializableData.Valid(data)) {
                    // but corrupted - overwrite with new
                    data = SerializableData.Empty();
                    json = JsonUtility.ToJson(data);
                    File.WriteAllText(Path, json);
                }
                // todo assert valid
            }

            return data;
        }

        static string Path => $"{Application.temporaryCachePath}/AssetUsageFinder4.json";

        public void Serialize() {
            var data = SerializableData.OnSerialize(_dict);
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(Path, json);
        }

        internal AssetDependencies Get(string path, string guid) {
            _used.Add(guid);
            if (_dict.TryGetValue(guid, out var res)) {
                var assetDependencyHash = AssetDatabase.GetAssetDependencyHash(path);

                if (assetDependencyHash.isValid && res.DependencyHash == assetDependencyHash) {
                    return res;
                }

                res = HashByPath();
                _dict[guid] = res;
                return res;
            }

            res = HashByPath();
            _dict.Add(guid, res);
            return res;

            AssetDependencies HashByPath() {
                var dependencyPaths = AssetDatabase.GetDependencies(path, recursive: false);
                var guids = dependencyPaths.Select(AssetDatabase.AssetPathToGUID).ToArray();
                return new AssetDependencies {
                    DependencyGuids = guids,
                    DependencyHash = AssetDatabase.GetAssetDependencyHash(path),
                };
            }
        }

        [Serializable]
        public class SerializableData {
            public const int CurrentVersion = 7; // for cache invalidation

            public int Version;
            public string[] Keys;
            public AssetDependencies[] Values;

            public static SerializableData Empty() => new SerializableData() {
                Keys = new string[0],
                Values = new AssetDependencies[0],
                Version = CurrentVersion,
            };

            public static SerializableData OnSerialize(Dictionary<string, AssetDependencies> dict) {
                return new SerializableData() {
                    Keys = dict.Keys.ToArray(),
                    Values = dict.Values.ToArray(),
                    Version = CurrentVersion,
                };
            }

            public Dictionary<string, AssetDependencies> OnDeserialize() {
                var res = new Dictionary<string, AssetDependencies>(StringComparer.InvariantCultureIgnoreCase);
                var keysLength = Keys.Length;
                for (var i = 0; i < keysLength; i++) res.Add(Keys[i], Values[i]);
                return res;
            }

            public static bool Valid(SerializableData that) =>
                that != null && CurrentVersion == that.Version && that.Keys != null && that.Values != null && that.Keys.Length == that.Values.Length;
        }
    }
}