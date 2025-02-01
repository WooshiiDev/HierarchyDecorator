using System.IO;
using UnityEditor;

namespace HierarchyDecorator
{
    // Register the Settings for the HiearchyDecorator
    public static class HierarchyDecoratorProvider
    {
        private static string Path => Constants.Paths.SETTINGS_PATH;

        public static SettingsProvider[] tabs =
        {
            new SettingsPreferences (Path, SettingsScope.User),
        };

        [SettingsProviderGroup]
        public static SettingsProvider[] Load()
        {
            return tabs;
        }
    }

}