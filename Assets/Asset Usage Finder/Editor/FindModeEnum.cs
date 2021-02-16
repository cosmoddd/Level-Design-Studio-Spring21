using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetUsageFinder {
    public enum FindModeEnum {
        Unknown = 0,
        File = 1,
        Scene = 2,
        Stage = 3
    }

    public static class FindMode {
        public static string GetWindowTitleByFindMode(FindModeEnum findMode) {
            switch (findMode) {
                case FindModeEnum.File:
                    return "Usages in Project";
                case FindModeEnum.Scene:
                    return "Usages in Scene";
                case FindModeEnum.Stage:
                    return "Usages in Stage";
                default:
                    return "Unknown Title!";
            }
        }

        public static string GetContentByFindMode(FindModeEnum findMode) {
            switch (findMode) {
                case FindModeEnum.File:
                    return "In Project Files";
                case FindModeEnum.Scene:
                    return "In Current Scene";
                case FindModeEnum.Stage:
                    return "In Current Stage";
                default:
                    return "Unknown Content!";
            }
        }
    }
}