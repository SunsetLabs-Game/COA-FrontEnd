using UnityEngine;
using System.Collections.Generic;

public class TrafficPathController : MonoBehaviour
{
    public TrafficNode[] Nodes { get; private set; }

    [Header("Debug")]
    public Color sphereColor = Color.cyan;
    [Range(0.5f, 3.0f)]public float sphereRadius = 2.0f;

    [Header("Parameters")]
    [SerializeField] private bool isLoop = true;
    [SerializeField] private List<TrafficNode> waypoints = new();

    public int WaypointCount => waypoints?.Count ?? 0;
    public List<TrafficNode> WayPointNodes => waypoints;

    private void Awake()
    {
        Nodes = waypoints.ToArray();
    }

    public void CreatePath()
    {
        waypoints.Clear();
        Nodes = GetComponentsInChildren<TrafficNode>();
        foreach(var node in Nodes)
        {
            node.ConnectedNodes.ForEach(x => x.AddNewNode(node));
            if(waypoints.Contains(node) != true)
            {
                waypoints.Add(node);
            }
        }
    }

    public void RenameNodes()
    {
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
                waypoints[i].name = "Node " + (i + 1).ToString("000");
        }
    }

    public void CreateNode()
    {
        GameObject obj = new("Node 001");
        TrafficNode node = obj.AddComponent<TrafficNode>();

        obj.transform.SetParent(transform);
        obj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        waypoints.Add(node);
    }

    private void OnDrawGizmosSelected()
    {
        waypoints.RemoveAll(x => x == null);
    }

    public TrafficNode GetNode(int index) => waypoints[index];

    private TrafficNode GetClosestNode(Transform requester, List<TrafficNode> list)
    {
        TrafficNode closestNode = null;
        float maxDistance = float.MaxValue;
        Vector3 position = requester.position;

        for (int i = 0; i < list.Count; i++)
        {
            Transform node = list[i].transform;
            float distance = Vector3.SqrMagnitude(node.position - position);
            if (distance < maxDistance)
            {
                maxDistance = distance;
                closestNode = list[i];
            }
        }
        return closestNode;
    }

    public TrafficNode GetClosestNodeInFrontOfObject(Transform requester, float detectAngle)
    {
        Vector3 position = requester.position;
        List<TrafficNode> nodeInSight = new();

        for (int i = 0; i < waypoints.Count; ++i)
        {
            var candidate = waypoints[i];
            if (candidate == null)
            {
                continue;
            }
            Vector3 direction = (candidate.transform.position - position);
            float viewAngle = Vector3.Angle(direction, requester.forward);
            if (viewAngle < detectAngle / 2)
            {
                nodeInSight.Add(candidate);
            }
        }
        int allCount = nodeInSight.Count;
        return (allCount > 0) ? GetClosestNode(requester, nodeInSight) : GetClosestNode(requester, waypoints);
    }

    public TrafficNode GetNode(Transform requester, TrafficNode excludeNode, out TrafficNode startNode)
    {
        List<TrafficNode> validList = new();
        startNode = (excludeNode != null) ? excludeNode : GetClosestNodeInFrontOfObject(requester, 180f);
        foreach (var node in Nodes)
        {
            if (node == startNode || node == excludeNode)
            {
                continue;
            }

            if (startNode.ConnectedNodes.Contains(node))
            {
                continue;
            }
            validList.Add(node);
        }
        if (validList.Count == 0) return null;
        int random = Random.Range(0, validList.Count);
        return validList[random];
    }
}
