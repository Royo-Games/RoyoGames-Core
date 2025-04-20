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
        // Foldout header
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, label, true);

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;

        // Get the Parameters instance
        var targetObject = property.serializedObject.targetObject;
        var field = fieldInfo;
        var parameters = field.GetValue(targetObject) as Parameters;
        if (parameters != null)
        {
            // Access the private _data dictionary
            var dataField = parameters.GetType()
                .GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic);
            var dict = dataField?.GetValue(parameters) as IDictionary;

            float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var entryHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (dict != null)
            {
                // If no entries, show a placeholder message
                if (dict.Count == 0)
                {
                    var rect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(rect, "No parameters added yet");
                    y += entryHeight;
                }
                else
                {
                    // Draw each key/value pair
                    foreach (DictionaryEntry kv in dict)
                    {
                        var rect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                        EditorGUI.LabelField(rect, kv.Key.ToString(), kv.Value?.ToString() ?? "null");
                        y += entryHeight;
                    }
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
                // Always reserve at least one line for placeholder if empty
                int lines = Math.Max(dict.Count, 1);
                height += lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            }
        }

        return height;
    }
}
