#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NanoEcs
{
    public static class Directives
    {
        public const string Debug = "NANOECS_DEBUG";
        public const string Reactivity = "NANOECS_REACTIVE";
    }

    public static class UnityMenuStructure
    {
        public const string RootFolder = "NanoECS/";
        public const string SettingsItem = RootFolder + "Settings";
    }

    [CreateAssetMenu(fileName = SettingsFileName, order = 51)]
    public class NanoEcsSettings : ScriptableObject
    {
        public const string SettingsFileName = "NanoECS Settings";
        public const string AssetRelativePath = "Assets/Settings/" + SettingsFileName;

        public bool VisualDebugEnabled
        {
            get
            {
                return visualDebugEnabled;
            }
            set
            {
                visualDebugEnabled = value;

                if (visualDebugEnabled)
                {
                    DefineTool.AddDefine(Directives.Debug);
                }
                else
                {
                    DefineTool.RemoveDefine(Directives.Debug);
                }
            }
        }

        public bool ReactivityEnabled
        {
            get
            {
                return reactivityEnabled;
            }

            set
            {
                reactivityEnabled = value;

                if (reactivityEnabled)
                {
                    DefineTool.AddDefine(Directives.Reactivity);
                }
                else
                {
                    DefineTool.RemoveDefine(Directives.Reactivity);
                }
            }
        }

        public bool reactivityEnabled;
        public bool visualDebugEnabled;

        public List<ContextSettings> Contexts;

        public string GeneratedFolderPath;
        public string SourceFolderPath;

        public bool TriggerGenerationOnSourceChange;
    }

    [System.Serializable]
    public class ContextSettings
    {
        public string Name;
        public int MinEntitiesPoolSize;
    }

}

#endif