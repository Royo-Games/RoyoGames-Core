using UnityEditor;

[InitializeOnLoad]
static class InspectorUpdater
{
    private static double _lastRepaintTime;
    private const double RepaintInterval =1;

    static InspectorUpdater()
    {
        EditorApplication.update += UpdateInspectors;
    }

    private static void UpdateInspectors()
    {
        if (!EditorApplication.isPlaying) return;

        if (EditorApplication.timeSinceStartup - _lastRepaintTime < RepaintInterval)
            return;

        _lastRepaintTime = EditorApplication.timeSinceStartup;

        foreach (var insp in InspectorWindowCache.GetInspectorWindows())
            insp.Repaint();
    }
}
