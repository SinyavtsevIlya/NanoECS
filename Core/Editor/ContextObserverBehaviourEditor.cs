#if UNITY_EDITOR && NANOECS_DEBUG
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ContextObserverBehaviour))]
public class ContextObserverBehaviourEditor : Editor
{
    ContextObserverBehaviour handler
    {
        get
        {
            return (target as ContextObserverBehaviour);
        }
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Space(20);

        GUILayout.BeginVertical(NanoEditorHelper.backStyle());

        GUILayout.Space(5);

        GUILayout.Label(handler.ContextName + " Context", new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });

        GUILayout.Space(20);

        if (GUILayout.Button("Create Entity"))
        {
            handler.CreateEntity();
        }

        GUILayout.Space(5);

        GUILayout.EndVertical();

        GUILayout.Space(20);

    }
} 
#endif