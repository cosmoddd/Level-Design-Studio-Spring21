using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetUsageFinder {
    [Serializable]
    abstract class DependencyAbstractFinder {
        public SearchTarget Target;
        public ResultRow[] Dependencies;
        public GUIContent TabContent;
        public DependencyAbstractFinder Parent;
        public string Title = "";

        public List<DependencyAbstractFinder> Parents() {
            var res = new List<DependencyAbstractFinder>();
            for (var cur = this; cur != null; cur = cur.Parent)
                res.Add(cur);
            return res;
        }

        public IEnumerable<ResultRow> Group(IEnumerable<ResultRow> inScenePro) {
            ResultRow cur = null;
            var res = new List<ResultRow>();
            var list = inScenePro.OrderBy(t => t.Main.GetInstanceID());
            var modificationsString = "Object.Modification.Modifications";
            var correpsondingString = "CorrespondingSourceObject";
            var gameObjectString = "GameObject";
            foreach (var source in list.ToList()) {
                if (cur == null || cur.Main != source.Main) {
                    cur = source;
                    var buf = source.Properties.Where(p =>
                        !p.Content.text.Contains(gameObjectString) &&
                        !p.Content.text.Contains(modificationsString) &&
                        !p.Content.text.Contains(correpsondingString)).ToList();
                    cur.Properties = buf;
                    res.Add(cur);
                }
                else {
                    foreach (var item in source.Properties) {
                        if (
                            !item.Content.text.Contains(gameObjectString) &&
                            !item.Content.text.Contains(modificationsString) &&
                            !item.Content.text.Contains(correpsondingString) &&
                            !cur.Properties.Any(p =>
                                p.Content.text == item.Content.text &&
                                p.Content.tooltip == p.Content.tooltip)) {
                            cur.Properties.Add(item);
                        }
                    }
                }
            }

            return res;
        }

        public abstract void FindDependencies();
        public abstract DependencyAbstractFinder Nest(Object o);
    }
}