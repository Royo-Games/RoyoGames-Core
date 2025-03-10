using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyAttiribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var guiEnabled = GUI.enabled;
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, true);
        GUI.enabled = guiEnabled;
    }
}
