using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
static class InspectorUpdater
{
    static InspectorUpdater()
    {
        EditorApplication.update += UpdateInspectors;
    }

    private static void UpdateInspectors()
    {
        if (!Application.isPlaying) return;

        var inspectors = Resources
            .FindObjectsOfTypeAll<EditorWindow>()
            .Where(w => w.GetType().Name == "InspectorWindow");

        foreach (var insp in inspectors)
            insp.Repaint();
    }
}
