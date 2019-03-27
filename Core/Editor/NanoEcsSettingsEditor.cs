using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(NanoEcsSettings))]
public class NanoEcsSettingsEditor : Editor
{
    const int SpaceSize = 5;

    NanoEcsSettings settings
    {
        get
        {
            return (NanoEcsSettings) target;
        }
    }

    ReorderableList list;
    void OnEnable()
    {
        list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("Contexts"),
                true, true, true, true);

      

        list.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => 
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Name"), label: new GUIContent("Name"));
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + SpaceSize, rect.width, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("MinEntitiesPoolSize"), label: new GUIContent("Min Entities Pool Size"));
            };

        list.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Contexts");
        };
        list.elementHeight = EditorGUIUtility.singleLineHeight * 2 + SpaceSize * 2;
    }



    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawLogo();
        GUILayout.BeginVertical(NanoEditorHelper.backStyle());

        GUILayout.Space(5);
        GUILayout.Label("(triggers recompilation)", NanoEditorHelper.SettingsTitleStyle);
        GUILayout.Space(5);

        settings.VisualDebugEnabled = GUILayout.Toggle(settings.VisualDebugEnabled, "Visual Debug Enabled");
        settings.ReactivityEnabled = GUILayout.Toggle(settings.ReactivityEnabled, "Reactivity Enabled");

        GUILayout.EndVertical();
        GUILayout.Space(25);
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("VisualDebugEnabled"));

        list.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        if (target != null)
        {
            EditorUtility.SetDirty(target);
        }
    }

    static void DrawLogo()
    {
        var baseRect = EditorGUILayout.GetControlRect();
        var titleRect = new Rect(baseRect.x, baseRect.y, baseRect.width, baseRect.height * 6);
        var texture = Resources.Load<Texture>("NanoEcs_Title_Flat");
        var scaleMode = baseRect.width < texture.width ? ScaleMode.ScaleAndCrop : ScaleMode.ScaleToFit;
        EditorGUI.DrawTextureTransparent(titleRect, texture, ScaleMode.ScaleToFit);

        for (int i = 0; i < 15; i++)
        {
            EditorGUILayout.Space();
        }
    }

}
