using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Parameters))]
public class ParametersDrawer : PropertyDrawer
{
    const float padding = 2f;
    private List<(string, object)> changedValues = new();

    private static readonly MethodInfo setMethodDef =
        typeof(Parameters).GetMethod("Set", BindingFlags.Instance | BindingFlags.Public);
    private static readonly FieldInfo dataFieldInfo =
        typeof(Parameters).GetField("_data", BindingFlags.Instance | BindingFlags.NonPublic);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        changedValues.Clear();

        property.isExpanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            property.isExpanded, label, true);
        if (!property.isExpanded) return;

        EditorGUI.indentLevel++;
        float y = position.y + EditorGUIUtility.singleLineHeight + padding;

        var target = property.serializedObject.targetObject;
        var parameters = fieldInfo.GetValue(target) as Parameters;
        if (parameters == null)
        {
            EditorGUI.LabelField(new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight),
                                 "Cannot cast to Parameters");
            EditorGUI.indentLevel--;
            return;
        }

        var dict = dataFieldInfo.GetValue(parameters) as IDictionary<string, object>;
        if (dict == null)
        {
            EditorGUI.indentLevel--;
            return;
        }

        if (dict.Count == 0)
        {
            Rect msgRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(msgRect, "Play to see parameters");
            EditorGUI.indentLevel--;
            return;
        }

        foreach (var kv in dict)
        {
            var key = kv.Key;
            var value = kv.Value;
            var valType = value?.GetType();
            float lineHeight = EditorGUIUtility.singleLineHeight;

            if (valType == typeof(float))
            {
                Rect floatRect = new Rect(position.x, y, position.width, lineHeight);
                EditorGUI.BeginChangeCheck();
                float newFloat = EditorGUI.FloatField(floatRect, key, (float)value);
                if (EditorGUI.EndChangeCheck())
                    changedValues.Add((key, newFloat));

                y += lineHeight + padding;
                continue;
            }

            Rect labelRect = new Rect(position.x, y, position.width * 0.3f, lineHeight);
            Rect fieldRect = new Rect(position.x + position.width * 0.3f + padding,
                                      y,
                                      position.width * 0.7f - padding,
                                      lineHeight);

            EditorGUI.LabelField(labelRect, key);
            object newValue = value;

            if (valType != null)
            {
                if (valType.IsEnum)
                {
                    var enumVal = (Enum)value;
                    if (Attribute.IsDefined(valType, typeof(FlagsAttribute)))
                        newValue = EditorGUI.EnumFlagsField(fieldRect, enumVal);
                    else
                        newValue = EditorGUI.EnumPopup(fieldRect, enumVal);
                }
                else
                {
                    switch (Type.GetTypeCode(valType))
                    {
                        case TypeCode.Boolean:
                            newValue = EditorGUI.Toggle(fieldRect, (bool)value);
                            break;
                        case TypeCode.Char:
                            {
                                string s = EditorGUI.TextField(fieldRect, value.ToString());
                                newValue = string.IsNullOrEmpty(s) ? '\0' : s[0];
                            }
                            break;
                        case TypeCode.SByte:
                            newValue = (sbyte)EditorGUI.IntField(fieldRect, (sbyte)value);
                            break;
                        case TypeCode.Byte:
                            newValue = (byte)EditorGUI.IntField(fieldRect, (byte)value);
                            break;
                        case TypeCode.Int16:
                            newValue = (short)EditorGUI.IntField(fieldRect, (short)value);
                            break;
                        case TypeCode.UInt16:
                            newValue = (ushort)EditorGUI.IntField(fieldRect, (ushort)value);
                            break;
                        case TypeCode.Int32:
                            newValue = EditorGUI.IntField(fieldRect, (int)value);
                            break;
                        case TypeCode.UInt32:
                            newValue = (uint)EditorGUI.LongField(fieldRect, Convert.ToInt64((uint)value));
                            break;
                        case TypeCode.Int64:
                            newValue = EditorGUI.LongField(fieldRect, (long)value);
                            break;
                        case TypeCode.UInt64:
                            newValue = (ulong)EditorGUI.LongField(fieldRect, Convert.ToInt64((ulong)value));
                            break;
                        case TypeCode.Double:
                            newValue = EditorGUI.DoubleField(fieldRect, (double)value);
                            break;
                        case TypeCode.Decimal:
                            {
                                double d = (double)(decimal)value;
                                d = EditorGUI.DoubleField(fieldRect, d);
                                newValue = (decimal)d;
                            }
                            break;
                        case TypeCode.String:
                            newValue = EditorGUI.TextField(fieldRect, (string)value);
                            break;
                        default:
                            EditorGUI.LabelField(fieldRect, value.ToString());
                            break;
                    }
                }
            }

            if (!Equals(newValue, value))
                changedValues.Add((key, newValue));

            y += lineHeight + padding;
        }

        ApplyChangedValues(parameters);
        EditorGUI.indentLevel--;
    }

    private void ApplyChangedValues(Parameters parameters)
    {
        foreach (var change in changedValues)
        {
            var genericSet = setMethodDef.MakeGenericMethod(change.Item2.GetType());
            genericSet.Invoke(parameters, new object[] { change.Item1, change.Item2 });
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight;
        if (!property.isExpanded)
            return height;

        height += padding;
        var target = property.serializedObject.targetObject;
        var parameters = fieldInfo.GetValue(target) as Parameters;
        if (parameters != null)
        {
            var dict = dataFieldInfo.GetValue(parameters) as IDictionary;
            if (dict != null)
            {
                if (dict.Count == 0)
                {
                    height += EditorGUIUtility.singleLineHeight;
                    return height;
                }

                foreach (DictionaryEntry kv in dict)
                {
                    height += EditorGUIUtility.singleLineHeight + padding;
                }
            }
        }

        return height;
    }
}
