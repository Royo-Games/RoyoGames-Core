using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CustomNameAttribute))]
public class CustomNameDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string elementName = "";

        var subBlockData = property.boxedValue as ICustomName;

        if (subBlockData != null)
        {
            elementName = subBlockData.CustomName;
        }

        var tempIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel--;
        EditorGUI.PropertyField(position, property, new GUIContent(elementName), true);
        EditorGUI.indentLevel = tempIndentLevel;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
