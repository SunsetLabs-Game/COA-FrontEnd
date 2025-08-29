using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.AI.Navigation;
using System.Collections.Generic;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

public class NavMeshBaker : MonoBehaviour
{
    private Vector3 playerLastPosition;
    private WaitForSeconds waitForSeconds;

    public delegate void NavMeshBuildEvent(Bounds bound);
    public NavMeshBuildEvent OnNavMeshBuild;

    private NavMeshData navMeshData;
    private List<NavMeshBuildSource> navMeshSources = new();
    private List<NavMeshBuildSource> cachedVolumeSources = new();

    [Header("NavMesh Tools")]
    [SerializeField] private float distanceThreshold = 10.0f;
    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] private Vector3 navMeshAreaBakeSize = new(20, 20, 20);

    private void Awake()
    {
        waitForSeconds = new(0.25f);
        navMeshData = new NavMeshData();

        navMeshData = new();
        NavMesh.AddNavMeshData(navMeshData);
        navMeshSurface.navMeshData = navMeshData;
    }

    private void Start()
    {
        CacheVolumeSources();
        NPCController_Start();
    }

    public void NPCController_Start()
    {
        BuildNavMeshSurface(false);
        StartCoroutine(CheckPlayerMovement());
    }

    private IEnumerator CheckPlayerMovement()
    {
        Transform player = NPCController.Instance.PlayerTransform;
        while (true)
        {
            if (Vector3.Distance(playerLastPosition, player.position) > distanceThreshold)
            {
                BuildNavMeshSurface(true);
                playerLastPosition = player.position;
            }
            yield return waitForSeconds;
        }
    }

    private void BuildNavMeshSurface(bool asyncBuild)
    {
        List<NavMeshBuildMarkup> mark = new();
        Transform player = NPCController.Instance.PlayerTransform;
        Bounds navMeshBounds = new(player.position, navMeshAreaBakeSize);

        navMeshSources.Clear();
        LayerMask surfaceMask = navMeshSurface.layerMask;
        NavMeshData navMeshData = navMeshSurface.navMeshData;
        bool surfaceChildren = navMeshSurface.collectObjects.Equals(CollectObjects.Children);

        if (surfaceChildren)
        {
            NavMeshBuilder.CollectSources(transform, surfaceMask, navMeshSurface.useGeometry, navMeshSurface.defaultArea, mark, navMeshSources);
        }
        else
        {
            NavMeshBuilder.CollectSources(navMeshBounds, surfaceMask, navMeshSurface.useGeometry, navMeshSurface.defaultArea, mark, navMeshSources);
        }

        foreach (var volume in cachedVolumeSources)
        {
            if (VolumeSourceIntersectsBound(volume, navMeshBounds))
            {
                navMeshSources.Add(CloneNavMeshBuildSource(volume));
            }
        }
        navMeshSources.RemoveAll(RemoveNavMeshAgentPredicate);

        Bounds buildBound = new(player.position, navMeshAreaBakeSize);
        if (asyncBuild)
        {
            AsyncOperation navMeshUpdateOperation = NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData, navMeshSurface.GetBuildSettings(), navMeshSources, buildBound);
            navMeshUpdateOperation.completed += HandleNavMeshUpdateOperation;
            return;
        }
        NavMeshBuilder.UpdateNavMeshData(navMeshData, navMeshSurface.GetBuildSettings(), navMeshSources, buildBound);
        OnNavMeshBuild?.Invoke(buildBound);
    }

    private void CacheVolumeSources()
    {
        cachedVolumeSources.Clear();
        var volumes = (navMeshSurface.collectObjects.Equals(CollectObjects.Children)) ?
            navMeshSurface.GetComponentsInChildren<NavMeshModifierVolume>() : NavMeshModifierVolume.activeModifiers.ToArray();

        foreach (var volume in volumes)
        {
            if (!volume.isActiveAndEnabled)
            {
                continue;
            }

            NavMeshBuildSource s = new()
            {
                shape = NavMeshBuildSourceShape.Box,
                transform = Matrix4x4.TRS(volume.transform.position, volume.transform.rotation, Vector3.one),
                size = Vector3.Scale(volume.size, volume.transform.lossyScale),
                area = volume.area,
                component = volume
            };
            cachedVolumeSources.Add(s);
        }
    }

    private bool RemoveNavMeshAgentPredicate(NavMeshBuildSource Source)
    {
        return Source.component != null && Source.component.gameObject.GetComponent<NavMeshAgent>() != null;
    }

    private void HandleNavMeshUpdateOperation(AsyncOperation asyncOperation)
    {
        Bounds bounds = new(playerLastPosition, navMeshAreaBakeSize);
        OnNavMeshBuild?.Invoke(bounds);
    }

    public void RefreshVolumeCache()
    {
        CacheVolumeSources();
    }

    private static NavMeshBuildSource CloneNavMeshBuildSource(NavMeshBuildSource original)
    {
        NavMeshBuildSource clone = new()
        {
            shape = original.shape,
            transform = original.transform,
            size = original.size,
            area = original.area,
            component = original.component,
        };
        return clone;
    }

    private static bool VolumeSourceIntersectsBound(NavMeshBuildSource volume, Bounds bakeBound)
    {
        Vector3 half = volume.size * 0.5f;

        float x = half.x;
        float y = half.y;
        float z = half.z;

        Vector3[] localCorners = new Vector3[]
        {
            new(x, y, z),
            new(-x, y, z),
            new(x, -y, z),
            new(x, y, -z),
            new(-x, -y, z),
            new(-x, y, -z),
            new(x, -y, -z),
            new(-x, -y, -z)
        };
        Matrix4x4 m = volume.transform;
        float length = localCorners.Length;

        if (length == 0)
        {
            return false;
        }
        Vector3 firstWorld = m.MultiplyPoint3x4(localCorners[0]);
        Bounds boxAABB = new(firstWorld, Vector3.zero);

        for(int i = 1; i < length; i++)
        {
            Vector3 world = m.MultiplyPoint3x4(localCorners[i]);
            boxAABB.Encapsulate(world);
        }
        return boxAABB.Intersects(bakeBound);
    }
}
