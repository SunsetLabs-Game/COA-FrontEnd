using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrafficPathController))]
public class TrafficPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TrafficPathController pathController = (TrafficPathController)target;

        EditorHelper.CreateButton("Create Node", pathController.CreateNode);
        EditorHelper.CreateButton("Create Path", pathController.CreatePath);
        EditorHelper.CreateButton("Rename Nodes", pathController.RenameNodes);
    }
}

[CustomEditor(typeof(TrafficNode))]
public class TrafficNodeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TrafficNode node = (TrafficNode)target;

        EditorHelper.CreateButton("Create Connector Node", node.CreateNewConnectorNode);
    }
}

[CustomEditor(typeof(TrafficManager))]
public class TrafficManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TrafficManager manager = (TrafficManager)target;

        EditorHelper.CreateButton("Clear Vehicles", manager.ClearSpawn);
        EditorHelper.CreateButton("Create Vehicles", manager.SpawnVehicle);
    }
}
