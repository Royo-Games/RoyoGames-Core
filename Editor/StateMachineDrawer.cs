using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StateMachine<>))]
public class StateMachineDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, label, true);

        if (!property.isExpanded)
            return;

        EditorGUI.indentLevel++;
        float y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        float lh = EditorGUIUtility.singleLineHeight;

        object sm = fieldInfo.GetValue(property.serializedObject.targetObject);
        if (sm != null)
        {
            Type smType = sm.GetType();

            var currentStateProp = smType.GetProperty("CurrentState", BindingFlags.Public | BindingFlags.Instance);
            object currentState = currentStateProp?.GetValue(sm);
            string currentStateID = "<none>";
            if (currentState != null)
            {
                var idProp = currentState.GetType().GetProperty("StateID");
                currentStateID = idProp?.GetValue(currentState)?.ToString() ?? "<unknown>";
            }

            var statesField = smType.GetField("_states", BindingFlags.NonPublic | BindingFlags.Instance);
            var statesDict = statesField?.GetValue(sm) as IDictionary;
            int stateCount = statesDict?.Count ?? 0;

            var transField = smType.GetField("_transitions", BindingFlags.NonPublic | BindingFlags.Instance);
            var transDict = transField?.GetValue(sm) as IDictionary;
            int localCount = 0;
            if (transDict != null)
            {
                foreach (var listObj in transDict.Values)
                    if (listObj is ICollection col) localCount += col.Count;
            }

            var globField = smType.GetField("_globalTransitions", BindingFlags.NonPublic | BindingFlags.Instance);
            var globList = globField?.GetValue(sm) as ICollection;
            int globalCount = globList?.Count ?? 0;

            Rect lineRect = new Rect(position.x, y, position.width, lh);

            EditorGUI.LabelField(lineRect, "Current State", currentStateID);
            y += lh + EditorGUIUtility.standardVerticalSpacing;
            lineRect.y = y;
            EditorGUI.LabelField(lineRect, "Total States", stateCount.ToString());
            y += lh + EditorGUIUtility.standardVerticalSpacing;
            lineRect.y = y;
            EditorGUI.LabelField(lineRect, "Local Transitions", localCount.ToString());
            y += lh + EditorGUIUtility.standardVerticalSpacing;
            lineRect.y = y;
            EditorGUI.LabelField(lineRect, "Global Transitions", globalCount.ToString());
            y += lh + EditorGUIUtility.standardVerticalSpacing;

            lineRect.y = y;
            EditorGUI.LabelField(lineRect, "States", "");
            y += lh + EditorGUIUtility.standardVerticalSpacing;

            if (statesDict != null)
            {
                foreach (var key in statesDict.Keys)
                {
                    lineRect = new Rect(position.x + 16, y, position.width, lh);
                    EditorGUI.LabelField(lineRect, "", key?.ToString() ?? "<null>");
                    y += lh + EditorGUIUtility.standardVerticalSpacing;
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

        object sm = fieldInfo.GetValue(property.serializedObject.targetObject);
        int idCount = 0;
        if (sm != null)
        {
            var statesField = sm.GetType().GetField("_states", BindingFlags.NonPublic | BindingFlags.Instance);
            var statesDict = statesField?.GetValue(sm) as IDictionary;
            if (statesDict != null)
                idCount = statesDict.Count;

            int lines = 5 + idCount;
            height += lines * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }

        return height;
    }
}
