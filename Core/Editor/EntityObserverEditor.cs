#if UNITY_EDITOR && NANOECS_DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System;
using UnityEditor.IMGUI.Controls;

[CanEditMultipleObjects]
[CustomEditor(typeof(EntityObserver))]
public class EntityObserverEditor : Editor
{
    const int MaxFieldToStringLength = 128;

    SearchField filterComponentsField;
    string filterComponentString;
    SearchField addComponentsField;

    Entity entity
    {
        get
        {
            return (target as EntityObserver).Entity;
        }
    }

    EntityObserver observer
    {
        get
        {
            return target as EntityObserver;
        }
    }


    private void OnEnable()
    {
        filterComponentsField = new SearchField();
        filterComponentsField.SetFocus();
        addComponentsField = new SearchField();
        observer.DisplayDropDown = false;
    }

    private void OnDisable()
    {

    }

    public override void OnInspectorGUI()
    {
        UpdateEditor();
    }

    void UpdateEditor()
    {

        EditorGUILayout.Space();

        if (GUILayout.Button(new GUIContent("Destroy"), GUILayout.Height(20)))
        {
            observer.OnEntityDestroy(entity);
        }

        EditorGUILayout.Space();




#if UNITY_EDITOR && NANOECS_DEBUG


        var componentObservers = new List<ComponentObserver>(entity.ComponentObservers);

        var rawTypes = observer.ComponentsLookup.Keys;

        var usedTypes = rawTypes
            .Where(type => HasTypes(type, componentObservers))
            .ToArray();

        var unusedTypes = rawTypes
            .Where(type => !HasTypes(type, componentObservers))
            .ToArray();


        filterComponentString = filterComponentsField.OnGUI(EditorGUILayout.GetControlRect(), filterComponentString);

        var filter = filterComponentString == null ? null : filterComponentString.Replace(' ', ',').Split(new char[] { ',', ' ', '.', ';' }).Where(x => x != null).Where(x => x != "");

        var filteredTypes = (filterComponentString == null || filterComponentString == "") ? usedTypes : usedTypes
            .Where(type => 
            {
                foreach (var f in filter)
                {
                    var lowerType = type.ToLower();
                    var lowerF = f.ToLower();
                    if (lowerType.Contains(lowerF))
                    {
                        return true;
                    }
                }
                return false;
            }).ToArray();

        foreach (var componentObserver in componentObservers)
        {
            var component = componentObserver.Component;

            //DrawUILine(back, 4);
            GUILayout.Space(5);

            var type = component.GetType();

            var name = type.Name;
            if (!filteredTypes.Contains(name)) continue;
            name = name.Replace("Component", "");
            
            GUILayout.BeginHorizontal(NanoEditorHelper.backStyle(component.GetHashCode()));


            var fields = type.GetFields(
             BindingFlags.NonPublic |
             BindingFlags.Instance).ToList();

            if (fields.Count > 0)
            {
                GUILayout.Space(15);
                componentObserver.IsFoldout = EditorGUILayout.Foldout(componentObserver.IsFoldout, name, true);
            }
            else
            {
                EditorGUILayout.LabelField(name);
            }
            if (GUILayout.Button("✕", GUILayout.Width(19), GUILayout.Height(19)))
            {
                entity.RemoveComponentOfIndex(observer.ComponentsLookup[component.GetType().ToString()]);
            }

            GUILayout.EndHorizontal();

            if (!componentObserver.IsFoldout) continue;

            foreach (var field in fields)
            {
                var fieldValue = field.GetValue(component);
                var fieldType = field.FieldType;

                var strVal = fieldValue != null ? string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}", fieldValue) : "null";
                if (strVal.Length > MaxFieldToStringLength)
                {
                    strVal = strVal.Substring(0, MaxFieldToStringLength);
                }

                GUILayout.BeginHorizontal(NanoEditorHelper.backStyle(component.GetHashCode()));

                //EditorGUILayout.LabelField(field.Name, GUILayout.MaxWidth(EditorGUIUtility.labelWidth - 16));

                if (fieldType == typeof(bool))
                {
                    var newValue = EditorGUILayout.Toggle(field.Name, (bool)fieldValue);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType == typeof(int))
                {
                    var newValue = EditorGUILayout.IntField(field.Name, (int)fieldValue);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType == typeof(float))
                {
                    var newValue = EditorGUILayout.FloatField(field.Name, (float)fieldValue);
                    SetValue(component, fields, field, newValue);

                }
                else if (fieldType == typeof(string))
                {

                    var newValue = EditorGUILayout.DelayedTextField(field.Name, (string)fieldValue);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType == typeof(Vector3))
                {
                    var newValue = EditorGUILayout.Vector3Field(field.Name, (Vector3)fieldValue);
                    SetValue(component, fields, field, newValue);
                }

                else if (fieldType == typeof(Vector2))
                {
                    var newValue = EditorGUILayout.Vector2Field(field.Name, (Vector2)fieldValue);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType == typeof(Vector2Int))
                {
                    var newValue = EditorGUILayout.Vector2IntField(field.Name, (Vector2Int)fieldValue);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType == typeof(Color))
                {
                    var newValue = EditorGUILayout.ColorField(field.Name, (Color)fieldValue);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType.IsEnum)
                {
                    var newValue = (Enum)EditorGUILayout.EnumPopup(field.Name, (Enum)fieldValue);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType == typeof(UnityEngine.Object) || fieldType.IsSubclassOf(typeof(UnityEngine.Object)) || (fieldType.IsInterface && fieldValue is MonoBehaviour))
                {
                    var newValue = EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)fieldValue, fieldType, true);
                    SetValue(component, fields, field, newValue);
                }
                else if (fieldType.IsSubclassOf(typeof(Entity)) && fieldValue != null)
                {
                    var p = observer.transform.parent;
                    var go = p.Find("Entity_" + (fieldValue as Entity).ID);
                    var newValue = EditorGUILayout.ObjectField(field.Name, go, fieldType, true);
                    //SetValue(component, fields, field, newValue);
                }
                else if (fieldType.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    int itemsCount = 0;
                    float minHeight = EditorGUIUtility.singleLineHeight;

                    foreach (var item in (IEnumerable)fieldValue) { itemsCount++; }

                    GUILayout.BeginScrollView(EditorGUILayout.GetControlRect().position, GUILayout.Height(itemsCount * minHeight + minHeight));
                    GUILayout.BeginVertical();
                    foreach (var item in (IEnumerable)fieldValue)
                    {
                        EditorGUILayout.SelectableLabel(item.ToString(), (GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight)));
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();

                }
                else
                {

                    EditorGUILayout.SelectableLabel(strVal, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                }

                GUILayout.EndHorizontal();

            }

        }

        GUILayout.Space(25);


        if (!observer.DisplayDropDown)
        {
            if (GUILayout.Button(new GUIContent("Add Component"), GUILayout.Height(20)))
            {
                addComponentsField.SetFocus();
                observer.DisplayDropDown = true;
            }
        }

        if (observer.DisplayDropDown)
        {


            var r = EditorGUILayout.GetControlRect(); r.height = 20;
            observer.CurrentComponentName = EditorExtend.TextFieldAutoComplete(addComponentsField, r, observer.CurrentComponentName, unusedTypes, (string value) => value.Replace("Component", ""), maxShownCount: 10, levenshteinDistance: 0.5f);
        }

        if (observer.ComponentsLookup.ContainsKey(observer.CurrentComponentName))
        {
            entity.Add<ComponentEcs>(observer.ComponentsLookup[observer.CurrentComponentName]);
            observer.CurrentComponentName = string.Empty;
        }

        Event e = Event.current;
        if (e.type == EventType.Ignore ||
            (e.type == EventType.MouseDown))
        {
            observer.DisplayDropDown = false;
        }
        GUILayout.Space(50);

        if (target != null)
        {
            EditorUtility.SetDirty(target);
        }
#endif
    }

    private static bool HasTypes(string type, List<ComponentObserver> componentObservers)
    {
        return componentObservers
            .Select(o => o.Component.GetType().ToString()).ToList().Contains(type);
    }

    private static GUIStyle ComponentLabelStyle(bool verticalOffset = true)
    {
        var y = verticalOffset ? 5 : 0;
        var style = new GUIStyle()
        {
            fontStyle = FontStyle.BoldAndItalic,
            fontSize = 12,
            contentOffset = new Vector2(5, y)
        };

        var v = 0.25f;
        style.normal.textColor = new Color(v, v, v);

        return style;
    }



    static void SetValue(ComponentEcs component, List<FieldInfo> fields, FieldInfo field, object newValue)
    {
        if (newValue != field.GetValue(component))
        {
            field.SetValue(component, newValue);
            component._InternalOnValueChange((ushort)fields.IndexOf(field));
        }
    }

    public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += padding / 2;
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }


}



#endif