#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SettingsPreview
{
    [UnityEditor.MenuItem(UnityMenuStructure.SettingsItem)]
    public static void DebugSettingns()
    {
        selectFile();
    }

    static void selectFile()
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath(NanoEcsSettings.AssetRelativePath + ".asset", typeof(Object));
    }
}
#endif
