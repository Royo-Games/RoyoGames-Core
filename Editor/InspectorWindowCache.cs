using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

static class InspectorWindowCache
{
    private static readonly Assembly editorAsm = typeof(EditorWindow).Assembly;
    private static readonly Type inspectorType = editorAsm.GetType("UnityEditor.InspectorWindow");
    private static readonly FieldInfo windowsField =
        typeof(EditorWindow).GetField("s_WindowList", BindingFlags.NonPublic | BindingFlags.Static);

    public static IEnumerable<EditorWindow> GetInspectorWindows()
    {
        if (inspectorType == null || windowsField == null)
            yield break;

        var allWindows = windowsField.GetValue(null) as IEnumerable<EditorWindow>;
        if (allWindows == null)
            yield break;

        foreach (var w in allWindows)
        {
            if (w != null && w.GetType() == inspectorType)
                yield return w;
        }
    }
}
