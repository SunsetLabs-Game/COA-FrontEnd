using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

#region Structs

[System.Serializable]
public struct Boundary
{
    public float minValue;
    public float maxValue;

    public Boundary(float min, float max)
    {
        minValue = min;
        maxValue = max;
    }
}

[System.Serializable]
public struct TileVariant
{
    public FloorTile Prefab;   // the original
    public int Rotations;  // 0–(UniqueRotationCount-1)
    public TilesBoundary Boundary; // precomputed rotated boundary

    public TileVariant(FloorTile prefab, int rotations)
    {
        Prefab = prefab;
        Rotations = rotations;
        Boundary = prefab.Boundary.Rotated(rotations);
    }
}

[System.Serializable]
public struct TilesBoundary
{
    [SerializeField] private bool forceFour;
    [field: Tooltip("0 Means Floor, 1 Means Road, 2 Means Fence")]
    [field: SerializeField][field: Range(0, 2)] public int Left { get; private set; }
    [field: Tooltip("0 Means Floor, 1 Means Road, 2 Means Fence")]
    [field: SerializeField][field: Range(0, 2)] public int Right { get; private set; }
    [field: Tooltip("0 Means Floor, 1 Means Road, 2 Means Fence")]
    [field: SerializeField][field: Range(0, 2)] public int South { get; private set; }
    [field: Tooltip("0 Means Floor, 1 Means Road, 2 Means Fence")]
    [field: SerializeField][field: Range(0, 2)] public int North { get; private set; }

    public TilesBoundary(int l, int r, int s, int n, bool force)
    {
        forceFour = force;
        Left = l; Right = r; South = s; North = n;
    }

    private readonly TilesBoundary Rotated90()
    {
        return new(l: South, r: North, s: Right, n: Left, force: forceFour);
    }

    public readonly TilesBoundary Rotated(int steps)
    {
        steps = ((steps % 4) + 4) % 4;
        var b = this;
        for (int i = 0; i < steps; i++)
            b = b.Rotated90();
        return b;
    }

    // how many unique rotations exist?
    // 1: all sides equal
    // 2: opposite sides equal (L==R && N==S), but L!=N
    // 4: else
    public readonly int UniqueRotationCount()
    {
        if(forceFour)
        {
            return 4;
        }

        bool allEqual = (Left == Right) && (Right == South) && (South == North);
        if (allEqual)
        {
            return 1;
        }

        bool oppEqual = (Left == Right) && (North == South);
        if (oppEqual)
        {
            return 2;
        }
        return 4;
    }
}

[System.Serializable]
public struct BoundInt
{
    [Range(1, 360)] public int minValue;
    [Range(1, 360)] public int maxValue;

    public BoundInt(int min, int max)
    {
        minValue = min;
        maxValue = max;
    }
}

[System.Serializable]
public struct BoundFloat
{
    [Range(-10, 10)] public float minValue;
    [Range(-10, 10)] public float maxValue;
}

#endregion

#region Static Classes

public static class TrailFX
{
    public static void HandleTrailFX(float simulationSpeed, Vector3 start, Vector3 end, ObjectPool<TrailRenderer> trailPool, MonoBehaviour mb)
    {
        mb.StartCoroutine(TrailFXRoutine(simulationSpeed, start, end, trailPool));
    }

    private static System.Collections.IEnumerator TrailFXRoutine(float simulationSpeed, Vector3 start, Vector3 end, ObjectPool<TrailRenderer> trailPool)
    {
        TrailRenderer trail = trailPool.Get();
        Transform trailTransform = trail.transform;

        trailTransform.position = start;
        yield return null;

        trail.emitting = true;
        float distance = Vector3.Distance(start, end);
        float remainingDistance = distance;
        while (remainingDistance > 0f)
        {
            trailTransform.position = Vector3.Lerp(start, end, Mathf.Clamp01(1 - (remainingDistance / distance)));
            remainingDistance -= simulationSpeed * Time.deltaTime;
            yield return null;
        }

        trailTransform.position = end;
        yield return new WaitForSeconds(trail.time);
        yield return null;

        trail.emitting = false;
        trailTransform.position = end;
        trailPool.Release(trail);
    }
}

public static class ObjectPooler
{
    public static ObjectPool<TrailRenderer> TrailPool(TrailRenderer objectToPool)
    {
        ObjectPool<TrailRenderer> objectPool = new(
            () => { return GameObject.Instantiate(objectToPool); },
            spawnObject => { spawnObject.gameObject.SetActive(true); },
            spawnObject => { spawnObject.gameObject.SetActive(false); },
            spawnObject => { GameObject.Destroy(spawnObject.gameObject); },
            false, 400, 500
        );
        return objectPool;
    }

    public static ObjectPool<GameObject> GameObjectPool(GameObject objectToPool, int min, int max)
    {
        ObjectPool<GameObject> objectPool = new
        (
            () => { return GameObject.Instantiate(objectToPool); },
            spawnObject => { spawnObject.SetActive(true); },
            spawnObject => { spawnObject.SetActive(false); },
            spawnObject => { GameObject.Destroy(spawnObject); },
            false, min, max
        );
        return objectPool;
    }

    public static ObjectPool<BulletFX> BulletFXPool(GameObject decalPrefab, ParticleSystem fxPrefab, MonoBehaviour context)
    {
        ObjectPool<BulletFX> pool = null;

        pool = new ObjectPool<BulletFX>
        (
            () =>
            {
                var fx = new BulletFX(decalPrefab, fxPrefab, context);
                fx.SetPool(pool);
                return fx;
            },
            fx => fx.GetObject(),
            fx => fx.Release(),
            null,
            false, 75, 200
        );
        return pool;
    }
}

public static class GameObjectTool
{
    public static T GetComponentByName<T>(string name) where T : Component
    {
        GameObject obj = GameObject.Find(name);
        return (obj != null) ? obj.GetComponent<T>() : null;
    }

    public static T GetRandomExcluding<T>(T exclude, T[] objectArray) where T : Object
    {
        var indexList = new List<int>();
        for (int i = 0; i < objectArray.Length; i++)
        {
            if (objectArray[i] == null || objectArray[i] == exclude) continue;
            indexList.Add(i);
        }
        if (indexList.Count == 0)
        {
            return null;
        }
        int rnd = Random.Range(0, indexList.Count);
        return objectArray[indexList[rnd]];
    }

    public static T GetRandomExcluding<T>(T exclude, List<T> objectList) where T : Object
    {
        var indexList = new List<int>();
        for (int i = 0; i < objectList.Count; i++)
        {
            if (objectList[i] == null || objectList[i] == exclude) continue;
            indexList.Add(i);
        }
        if (indexList.Count == 0)
        {
            return null;
        }
        int rnd = Random.Range(0, indexList.Count);
        return objectList[indexList[rnd]];
    }

    public static bool TryGetComponentInParent<T>(Transform parent, out T result) where T : Component
    {
        result = parent.GetComponentInParent<T>();
        return (result != null);
    }

    public static bool TryGetComponentInChildren<T>(Transform parent, out T result) where T : Component
    {
        result = parent.GetComponentInChildren<T>();
        return (result != null);
    }

    public static bool TryFindFirstObject<T>(out T result) where T : Component
    {
        result = GameObject.FindFirstObjectByType<T>();
        return (result != null);
    }

    public static bool TryFindChildRecursively(Transform parent, string name, out Transform result)
    {
        if (parent == null)
        {
            result = null;
            return false;
        }

        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                result = child;
                return true;
            }
            if (FindChildRecursively(child, name) is Transform recursiveChild && recursiveChild != null)
            {
                result = recursiveChild;
                return true;
            }
        }
        result = null;
        return false;
    }

    public static Transform FindChildRecursively(Transform parent, string name)
    {
        if (parent == null) return null;
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var recursiveChild = FindChildRecursively(child, name);
            if (recursiveChild != null) return recursiveChild;
        }
        return null;
    }
}

public static class EditorHelper
{
    public static void CreateButton(string text, System.Action action)
    {
        EditorGUILayout.Space();
        if (GUILayout.Button(text))
        {
            action?.Invoke();
        }
    }
}

public static class MathPhysics_Helper
{
    public static float Square(float x)
    {
        return x * x;
    }
}

public static class LevelLoader
{
    private static AsyncOperation sceneLoadingOperation;
    public static void HandleLoadLevel(string levelName, System.Action action, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(LoadLevelCoroutine(levelName, action));
    }

    public static IEnumerator LoadSceneAsync(string sceneName)
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();

        sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
        sceneLoadingOperation.allowSceneActivation = false;

        while (sceneLoadingOperation.isDone != true)
        {
            if (sceneLoadingOperation.progress >= 0.9f)
            {
                sceneLoadingOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public static IEnumerator LoadLevelCoroutine(string levelName, System.Action action)
    {
        sceneLoadingOperation = SceneManager.LoadSceneAsync(levelName);
        sceneLoadingOperation.completed += operation => { action?.Invoke(); };

        while (sceneLoadingOperation.isDone != true)
        {
            System.GC.Collect();
            yield return null;
        }
    }
}

public static class Tiles_Helper
{
    private static readonly Dictionary<CornerBoundary, (TileDirection main, TileDirection other)> CornerMap = new()
    {
        [CornerBoundary.NorthLeft] = (TileDirection.North, TileDirection.Left),
        [CornerBoundary.NorthRight] = (TileDirection.North, TileDirection.Right),
        [CornerBoundary.SouthLeft] = (TileDirection.South, TileDirection.Left),
        [CornerBoundary.SouthRight] = (TileDirection.South, TileDirection.Right),
    };

    public static TileDirection GetOtherBoundaryTypeViaCornerType(CornerBoundary cType, TileDirection bType)
    {
        if (CornerMap.TryGetValue(cType, out var pair))
        {
            return bType == pair.main ? pair.other : pair.main;
        }
        return TileDirection.Ignore;
    }

    public static CornerBoundary GetCornerType(int x, int y, int max) =>
        (x, y) switch
        {
            (0, 0) => CornerBoundary.SouthLeft,
            (0, var yy) when yy == max => CornerBoundary.NorthLeft,
            (var xx, 0) when xx == max => CornerBoundary.SouthRight,
            (var xx, var yy) when xx == max && yy == max => CornerBoundary.NorthRight,
            _ => CornerBoundary.None
        };

    public static TileDirection GetDirectionToNeighbor(Cell from, Cell to)
    {
        Vector2Int toDim = to.CellDimensions();
        Vector2Int fromDim = from.CellDimensions();
        
        int dx = fromDim.x - toDim.x;
        int dy = fromDim.y - toDim.y;

        if (dx == 1)  return TileDirection.Left;
        if (dx == -1) return TileDirection.Right;
        if (dy == 1)  return TileDirection.South;
        if (dy == -1) return TileDirection.North;

        return TileDirection.Ignore;
    }

    public static TileDirection GetOppositeDirection(TileDirection dir)
    {
        return dir switch
        {
            TileDirection.Left => TileDirection.Right,
            TileDirection.Right => TileDirection.Left,
            TileDirection.North => TileDirection.South,
            TileDirection.South => TileDirection.North,
            _ => TileDirection.Ignore
        };
    }

    public static TileDirection GetBoundaryType(int x, int y, int max)
    {
        if (x == 0) return TileDirection.Left;
        if (x == max) return TileDirection.Right;
        if (y == 0) return TileDirection.South;
        if (y == max) return TileDirection.North;
        return TileDirection.Ignore;
    }

    public static int BoundedCell(TilesBoundary tileBound, TileDirection tileDir)
    {
        return tileDir switch
        {
            TileDirection.Left => tileBound.Left,
            TileDirection.Right => tileBound.Right,
            TileDirection.North => tileBound.North,
            TileDirection.South => tileBound.South,
            _ => -1
        };
    }
}

#endregion
