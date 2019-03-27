#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SettingsPreview
{
    [UnityEditor.MenuItem(UnityMenuStructure.SettingsItem)]
    public static void DebugSettingns()
    {
        selectFile("Settings");
    }

    static void selectFile(string fileName)
    {
        Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/Plugins/Nanory/NanoECS/Settings/" + fileName + ".asset", typeof(Object));
    }
}
#endif
