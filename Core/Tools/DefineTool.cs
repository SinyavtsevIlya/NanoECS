#if UNITY_EDITOR
using System.Linq;
using UnityEditor;

public static class DefineTool
{
    public static void AddDefine(string define)
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var strDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var defines = strDefines.Split(';').ToList();
        if (defines.Count(x => x.ToLower() == define.ToLower()) == 0)
        {
            defines.Add(define);
            var str = "";
            if (defines.Count > 1)
                str = defines.Aggregate((a, b) => b + ";" + a) + ";";
            else if (defines.Count == 1)
                str = defines[0] + ";";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, str);
        }
    }

    public static void RemoveDefine(string define)
    {
        var group = EditorUserBuildSettings.selectedBuildTargetGroup;
        var strDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
        var defines = strDefines.Split(';').ToList();
        if (defines.Count(x => x.ToLower() == define.ToLower()) > 0)
        {
            defines.Remove(define);
            var str = "";
            if (defines.Count > 1)
                str = defines.Aggregate((a, b) => b + ";" + a) + ";";
            else if (defines.Count == 1)
                str = defines[0] + ";";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, str);
        }
    }
}
#endif