using UnityEngine;
using UnityEditor;

public static class EditorFunctionHolder
{
    public static void HandleButtonEvent(string label, System.Action func)
    {
        EditorGUILayout.Space();
        if (GUILayout.Button(label))
        {
            func.Invoke();
        }
    }
}

[CustomEditor(typeof(EnviromentGenerator))]
public class EnviromentGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EnviromentGenerator enviromentGenerator = (EnviromentGenerator)target;

        EditorFunctionHolder.HandleButtonEvent("Reset", enviromentGenerator.ResetAndRegenerate);
        EditorFunctionHolder.HandleButtonEvent("Generate Cells", enviromentGenerator.GenerateGridCells);

        EditorFunctionHolder.HandleButtonEvent("Collapse Cell", enviromentGenerator.HandleCellCollapse);
        EditorFunctionHolder.HandleButtonEvent("Modify Cells", enviromentGenerator.ModifySelectedCell);
        EditorFunctionHolder.HandleButtonEvent("Indicate Bounding Cells", enviromentGenerator.IndicateMaterials);
    }
}

[CustomEditor(typeof(AerialWayPoint))]
public class AerialWayPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AerialWayPoint aerialController = (AerialWayPoint)target;

        base.OnInspectorGUI();
        if (GUILayout.Button("Get All Nodes")) { aerialController.GetWayPointNodes(); }
        if (GUILayout.Button("Rename Nodes")) { aerialController.RenameNodes(); }
    }
}

