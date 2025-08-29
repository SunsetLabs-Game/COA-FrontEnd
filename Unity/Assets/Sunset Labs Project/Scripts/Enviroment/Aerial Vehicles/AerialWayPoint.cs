using UnityEngine;
using System.Collections.Generic;

public class AerialWayPoint : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float gizmoRadius;
    [field: SerializeField] public List<Transform> WayPointNodes { get; private set; } = new();

    public void RenameNodes()
    {
        WayPointNodes.RemoveAll(x => x == null);
        for (int i = 0; i < WayPointNodes.Count; i++)
        {
            WayPointNodes[i].name = "Node " + (i + 1).ToString("000");
        }
    }

    public void GetWayPointNodes()
    {
        WayPointNodes.Clear();
        for(int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if(child == transform || WayPointNodes.Contains(child))
            {
                continue;
            }
            WayPointNodes.Add(child);
        }
    }

    public int GetClosestNode(Vector3 position)
    {
        int index = 0;
        float maxDistance = float.MaxValue;

        for (int i = 0; i < WayPointNodes.Count; i++)
        {
            Transform node = WayPointNodes[i].transform;
            float distance = Vector3.SqrMagnitude(node.position - position);
            if (distance < maxDistance)
            {
                index = i;
                maxDistance = distance;
            }
        }
        return index;
    }

    private void OnDrawGizmosSelected()
    {
        for (int i = 0; i < WayPointNodes.Count; i++)
        {
            if(WayPointNodes[i] == null)
            {
                continue;
            }
            Vector3 position = WayPointNodes[i].position;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, gizmoRadius);

            if(i != WayPointNodes.Count - 1)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(position, WayPointNodes[i + 1].position);
            }
        }
    }
}
