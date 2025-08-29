using UnityEngine;
using System.Collections.Generic;

public class TrafficNode : MonoBehaviour
{
    private TrafficPathController pathController;
    [SerializeField] private List<TrafficNode> connectedNodes = new();

    public Vector3 NodePosition => transform.position;
    public List<TrafficNode> ConnectedNodes => connectedNodes;

    public void CreateNewConnectorNode()
    {
        GameObject newObj = new("Node");
        TrafficNode newNode = newObj.AddComponent<TrafficNode>();

        AddNewNode(newNode);
        newNode.AddNewNode(this);
        newObj.transform.SetParent(transform.parent);
        newObj.transform.localPosition = transform.localPosition;
    }

    public void AddNewNode(TrafficNode node)
    {
        if (node == null || connectedNodes.Contains(node))
        {
            return;
        }
        connectedNodes.Add(node);
    }

    public TrafficNode GetNextNode(bool distanceBased, Vector3 target, HashSet<TrafficNode> visited = null)
    {
        return distanceBased ? GetNextNodeDistanceBased(target, visited) : GetNextNodeRandomly(visited);
    }

    private TrafficNode GetNextNodeRandomly(HashSet<TrafficNode> visited = null)
    {
        connectedNodes.RemoveAll(x => x == null);
        int count = connectedNodes.Count;
        if (count == 0)
        {
            return null;
        }

        int seen = 0;
        TrafficNode selected = null;
        for (int i = 0; i < count; i++)
        {
            var candidate = connectedNodes[i];
            if (HasVisited(candidate, visited))
            {
                continue;
            }

            seen++;
            if (Random.Range(0, seen) == 0)
            {
                selected = candidate;
            }
        }
        return selected;
    }

    public TrafficNode GetNextNodeDistanceBased(Vector3 target, HashSet<TrafficNode> visited = null)
    {
        TrafficNode best = null;
        float bestSq = float.MaxValue;
        connectedNodes.RemoveAll(x => x == null);

        for (int i = 0; i < connectedNodes.Count; i++)
        {
            var candidate = connectedNodes[i];
            if(HasVisited(candidate, visited))
            {
                continue;
            }

            float sq = (candidate.transform.position - target).sqrMagnitude;
            if (sq < bestSq)
            {
                bestSq = sq;
                best = candidate;
            }
        }
        return best;
    }

    private bool HasVisited(TrafficNode candidate, HashSet<TrafficNode> visited)
    {
        if (candidate == null)
        {
            return true;
        }
        if (visited != null && visited.Contains(candidate))
        {
            return true;
        }
        return false;
    }


    private void OnDrawGizmosSelected() => DisplayNodeGizmos();

    public void DisplayNodeGizmos()
    {
        if(pathController == null)
        {
            pathController = GetComponentInParent<TrafficPathController>();
        }

        Gizmos.color = pathController.sphereColor;
        Gizmos.DrawSphere(transform.position, pathController.sphereRadius);

        connectedNodes.RemoveAll(x => x == null);
        for (int i = 0; i < connectedNodes.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Vector3 pos = connectedNodes[i].transform.position;
            Gizmos.DrawLine(transform.position, pos);

            Gizmos.color = pathController.sphereColor;
            Gizmos.DrawSphere(pos, pathController.sphereRadius);
        }
    }
}