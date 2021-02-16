using System;

namespace AssetUsageFinder {
    static class Globals<T> where T : class {
        static T _instance;

        public static void TryInit(Func<T> ctor) {
            if (_instance != null) return;
            _instance = ctor.Invoke();
        }

        public static T Get() => _instance;

        public static T GetOrCreate(Func<T> ctor) {
            TryInit(ctor);
            return _instance;
        }

        public static void Set(T value) => _instance = value;
    }
}