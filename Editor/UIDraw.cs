using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class UIDraw
{
    public static bool DrawOpenerHeader(Rect rect, string name, bool active)
    {
        rect.height = EditorGUIUtility.singleLineHeight + 6;

        GUIStyle style = new GUIStyle(GUI.skin.box)
        {
           alignment = TextAnchor.MiddleLeft,
           fontStyle = FontStyle.Bold,
        };

        style.fontSize = 11;

        if(EditorGUIUtility.isProSkin)
        style.normal.textColor = active ? new Color(0.9f, 0.9f, 0.9f) : new Color(0.7f, 0.7f, 0.7f);
        else
        style.normal.textColor = active ? new Color(0.15f, 0.15f, 0.15f) : new Color(0.3f, 0.3f, 0.3f);

        if (GUI.Button(rect, string.Format("{0}  {1}", active ? "▼" : "►", name), style))
        {
            active = !active;
        }

        return active;
    }

    public static bool DrawOpenerHeader(string name, bool active)
    {
        return DrawOpenerHeader(EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight + 6)), name, active);
    }
}
