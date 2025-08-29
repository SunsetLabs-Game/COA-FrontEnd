using UnityEngine;
using UnityEngine.Pool;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;

#region Enums
public enum DriverExperience { Novice, Mid, Expert }
public enum CornerBoundary 
{ 
    NorthLeft,
    NorthRight,
    SouthLeft,
    SouthRight,
    None
}

public enum TileDirection
{
    Left,
    Right,
    North,
    South,
    Ignore,
}

public enum TileType
{
    Fence,
    Normal,
    CornerPiece
}

public enum ItemType
{
    Currency,
    Collectible
}

public enum CombatMentalState
{
    Friendly,
    High_Alert
}

public enum DuelState
{
    Win,
    Draw,
    Lost,
    None,
    OnGoing
}

public enum ItemBoxTier //More For Design
{
    Wood, //2 Items[1-3] and Coin[5-15]
    Metal, //3 Items[1-5] and Coin[5-25]
    Gold, //3 Items[2-5], Coin[10-25] and Token[5-15]
    Diamond //5 Items[3-7] and Ruby[1-10]
}

public enum WeaponType
{
    Gun,
    Melee
}

#endregion

#region Classes

[System.Serializable]
public class MintRequest
{
    public string userId;
    public int[] tokenIds;
    public int[] amounts;
    public string recipient;
}

[System.Serializable]
public class UseItemRequest
{
    public int nftId;
    public int quantity;

    public UseItemRequest(int nftId, int quantity)
    {
        this.nftId = nftId;
        this.quantity = quantity;
    }
}

[System.Serializable]
public class ItemClass
{
    public int itemCount;
    public PickableObject pickedObj;
    public InventorySlotUI SlotUI { get; private set; }

    public ItemClass(int count, PickableObject item)
    {
        pickedObj = item;
        itemCount = count;
    }

    public void SetSlotUI(InventorySlotUI slotUI)
    {
        SlotUI = slotUI;
    }

    public void UpdateItemCount(bool shouldAdd)
    {
        float alpha;
        if (shouldAdd)
        {
            alpha = 1.0f;
            itemCount++;
        }
        else
        {
            alpha = 0.04f;
            itemCount--;
        }

        if(itemCount <= 0) itemCount = 0;
        SlotUI.UpdateSlotUI(alpha);
    }
}

[System.Serializable]
public class RewardBox
{
    public List<ItemClass> itemsList = new();
    public bool FinishedCleaning { get; private set; }

    public void AddItemToBox(ItemClass reward)
    {
        FinishedCleaning = false;
        itemsList.Add(reward);
    }

    public void CleanBox()
    {
        itemsList.RemoveAll(x => x.pickedObj == null);
        FinishedCleaning = true;
    }

    public void EmptyBox()
    {
        itemsList.Clear();
        FinishedCleaning = true;
    }

    public void HandleRewarding()
    {
        for (int i = 0; i < itemsList.Count; i++)
        {
            ItemClass itemClass = itemsList[i];
            CharacterInventoryManager.Instance.HandleItemAddition(itemClass);
        }
        EmptyBox();
    }
}

[System.Serializable]
public class WeaponRecoil
{
    private int index;
    private float time;
    private Transform cameraObject;

    [Header("Parameters")]
    [SerializeField] private float duration;
    [SerializeField] private Vector2[] recoilPattern;

    [Header("Components")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    public void Initialize(Transform cameraObj, CinemachineImpulseSource impulseSource)
    {
        cameraObject = cameraObj;
        this.impulseSource = impulseSource;
    }

    public void ResetIndex()
    {
        index = 0;
    }

    public void HandleRecoil(float delta)
    {
        if(time > 0.0f)
        {
            //freeLook.m_YAxis.Value -= ((verticalRecoil / 1000) * delta) / duration;
            //freeLook.m_XAxis.Value -= ((horizontalRecoil / 1000) * delta) / duration;
            time -= delta;
        }
    }

    public void GenerateRecoilPattern()
    {
        time = duration;
        impulseSource.GenerateImpulse(cameraObject.forward);

        //float verticalRecoil = recoilPattern[index].y;
        //float horizontalRecoil = recoilPattern[index].x;

        index = NextIndex();
    }

    private int NextIndex()
    {
        return (index + 1) % recoilPattern.Length;
    }
}

public class Bullet
{
    public float time;
    public Vector3 initialPosition;
    public Vector3 initialVelocity;

    public Bullet(Vector3 pos, Vector3 vel)
    {
        time = 0.0f;
        initialPosition = pos;
        initialVelocity = vel;
    }
}

public class BulletFX
{
    private ObjectPool<BulletFX> pool;
    private ObjectPool<GameObject> bulletDecalPool;

    private MonoBehaviour context;
    private ParticleSystem bulletImpactFX;

    private WaitForSeconds impactDelay = new(2f);
    private WaitForSeconds decalDelay = new(7.5f);

    public BulletFX(GameObject decalPrefab, ParticleSystem impactFXPrefab, MonoBehaviour context)
    {
        bulletImpactFX = GameObject.Instantiate(impactFXPrefab);
        bulletDecalPool = ObjectPooler.GameObjectPool(decalPrefab, 50, 100);
        this.context = context;
    }

    public void SetPool(ObjectPool<BulletFX> pool) => this.pool = pool;

    public void GetObject()
    {
        bulletImpactFX.gameObject.SetActive(true);
    }

    public void HandleBulletImpact(Vector3 pos, Quaternion rot)
    {
        bulletImpactFX.transform.SetPositionAndRotation(pos, rot);
        bulletImpactFX.Emit(1);

        var decal = bulletDecalPool.Get();
        decal.transform.SetPositionAndRotation(pos, rot);
        decal.transform.Rotate(Vector3.forward, Random.Range(0, 360));
        decal.SetActive(true);

        context.StartCoroutine(ReleaseDecalAfterDelay(decal));
        pool.Release(this);
    }

    public void Release()
    {
        context.StartCoroutine(DisableImpactFX());
    }

    private IEnumerator DisableImpactFX()
    {
        yield return impactDelay;
        bulletImpactFX.gameObject.SetActive(false);
    }

    private IEnumerator ReleaseDecalAfterDelay(GameObject decal)
    {
        yield return decalDelay;
        bulletDecalPool.Release(decal);
    }
}

public class EvenOrOddAttribute : PropertyAttribute
{
    public bool isEven;
    public BoundInt boundary;

    public EvenOrOddAttribute(int min, int max, bool isEven = true)
    {
        this.isEven = isEven;
        boundary = new(min, max);
    }
}

public class ReadOnlyInspectorAttribute : PropertyAttribute
{
}

[System.Serializable]
public class WayPointPath
{
    private Vector3 _finalDestination;

    public TrafficNode FinalDestinationNode { get; private set; }
    public bool HasPath => PathNodes != null && PathNodes.Count > 0;

    [Header("Non-expert randomization tuning")]
    [Tooltip("Higher value -> more randomness for novices (bigger range)")]
    [SerializeField] private int noviceMaxRange = 3;
    [Tooltip("Higher value -> more randomness for mid-senior; lower -> more deterministic")]
    [SerializeField] private int midSnrMaxRange = 4;
    [field: SerializeField] public List<TrafficNode> PathNodes { get; private set; } = new();

    public int PathNodeCount => PathNodes.Count;

    public void RefreshPath(DriverExperience exp, Transform requester, TrafficNode finalNode, TrafficPathController controller)
    {
        if (controller == null)
        {
            RefreshPath(exp, (TrafficNode)null, finalNode);
            return;
        }
        TrafficNode startNode = controller.GetClosestNodeInFrontOfObject(requester, 180f);
        RefreshPath(exp, startNode, finalNode);
    }

    public void RefreshPath(DriverExperience exp, Transform requester, TrafficNode startNode, TrafficNode finalNode)
    {
        var controller = Object.FindObjectOfType<TrafficPathController>();
        if (controller != null && requester != null)
        {
            startNode = controller.GetClosestNodeInFrontOfObject(requester, 180f);
        }
        RefreshPath(exp, startNode, finalNode);
    }

    public void RefreshPath(DriverExperience exp, TrafficNode startNode, TrafficNode finalNode)
    {
        PathNodes.Clear();
        if (startNode == null || finalNode == null)
        {
            //CachePositionsAndDistances();
            FinalDestinationNode = finalNode;
            _finalDestination = finalNode != null ? finalNode.transform.position : Vector3.zero;
            return;
        }
        PathNodes.Add(startNode);
        var visited = new HashSet<TrafficNode>(capacity: 32)
        {
            startNode
        };

        TrafficNode current = startNode;

        int safety = 0;
        const int maxSteps = 10; // existing constraint

        while (current != null && current != finalNode && ++safety <= maxSteps)
        {
            TrafficNode next;
            if (exp == DriverExperience.Expert)
            {
                next = current.GetNextNodeDistanceBased(finalNode.transform.position, visited);
            }
            else
            {
                int maxRange = (exp == DriverExperience.Novice) ? noviceMaxRange : midSnrMaxRange;
                bool preferDistance = Random.Range(0, Mathf.Max(1, maxRange)) >= 2;
                next = current.GetNextNode(preferDistance, finalNode.transform.position, visited);
            }

            if (next == null)
            {
                break;
            }

            if (!PathNodes.Contains(next))
            {
                visited.Add(next);
                PathNodes.Add(next);
            }
            current = next;
            if (current == finalNode) break;
        }
        FinalDestinationNode = finalNode;
        _finalDestination = finalNode != null ? finalNode.transform.position : Vector3.zero;
    }

    public float DistanceToFinalDestination(Vector3 currentPosition)
    {
        if (FinalDestinationNode == null)
        {
            return float.PositiveInfinity;
        }
        return Vector3.Distance(currentPosition, _finalDestination);
    }

    public float DistanceBetweenTwoNodes(int indexA, int indexB)
    {
        int count = PathNodes?.Count ?? 0;
        if (count == 0)
        {
            return -1f;
        }

        if (indexA < 0 || indexB < 0 || indexA >= count || indexB >= count)
        {
            return -1f;
        }

        var a = PathNodes[indexA];
        var b = PathNodes[indexB];
        if (a == null || b == null)
        {
            return -1f;
        }
        return Vector3.Distance(a.NodePosition, b.NodePosition);
    }

    public float DistanceBetweenLastTwoNodes()
    {
        int c = PathNodes?.Count ?? 0;
        if (c < 2)
        {
            return -1f;
        }
        return DistanceBetweenTwoNodes(c - 1, c - 2);
    }

    public void Clear()
    {
        PathNodes?.Clear();
        FinalDestinationNode = null;
        _finalDestination = Vector3.zero;
    }

    public void SetNonExpertRanges(int noviceRange, int midSnrRange)
    {
        noviceMaxRange = Mathf.Max(1, noviceRange);
        midSnrMaxRange = Mathf.Max(1, midSnrRange);
    }
}
#endregion