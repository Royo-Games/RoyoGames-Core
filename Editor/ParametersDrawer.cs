using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Parameters))]
public class ParametersDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, label, true);

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;

        var targetObject = property.serializedObject.targetObject;
        var field = fieldInfo; 
        var parameters = field.GetValue(targetObject) as Parameters;
        if (parameters != null)
        {
            var dataField = parameters.GetType()
                .GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic);
            var dict = dataField?.GetValue(parameters) as IDictionary;

            if (dict != null)
            {
                float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                foreach (DictionaryEntry kv in dict)
                {
                    var rect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(rect, kv.Key.ToString(), kv.Value?.ToString() ?? "null");
                    y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        EditorGUI.indentLevel--;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;

        if (!property.isExpanded)
            return height;

        var targetObject = property.serializedObject.targetObject;
        var parameters = fieldInfo.GetValue(targetObject) as Parameters;
        if (parameters != null)
        {
            var dataField = parameters.GetType()
                .GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic);
            var dict = dataField?.GetValue(parameters) as IDictionary;
            if (dict != null)
            {
                height += dict.Count * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
        }

        return height;
    }
}
