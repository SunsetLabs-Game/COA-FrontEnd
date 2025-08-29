using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    public static NPCController Instance;

    private WeaponManager weapon;
    private TextAsset selectedMessage;
    private enum SpawnMethod { Random, RoundRobin }

    private CharacterManager mercenary;
    public List<CharacterManager> npcList = new();
    public CharacterManager Mercenary => mercenary;

    private int spawnedNPCCount;
    private Collider[] npcColliders;
    private const int cnst_tileSize = 10;
    private readonly HashSet<Vector3> spawnedTiles = new();

    private ObjectPool<CharacterManager> mercenaryPool;
    private readonly Dictionary<CharacterManager, ObjectPool<CharacterManager>> npcPools = new();

    public CharacterManager Player { get; private set; }
    public PlayerCompanion Companion { get; private set; }

    [Header("Parameters")]
    [SerializeField] private bool showRadius;
    [Range(15, 40)]
    [SerializeField] private int maxNPCCount = 20;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private LayerMask characterLayer;

    [Header("Objects To Spawn")]
    [SerializeField] private CharacterManager[] npcsArray;
    [SerializeField] private CharacterManager mercenaryPrefab;
    [SerializeField] private Vector3Int navMeshSize = new(40, 10, 40);

    [Header("Spawn Tools")]
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private Transform spawnParent;
    [SerializeField] private float spawnDensityPerTile = 0.5f;
    [SerializeField] private SpawnMethod spawnMethod = SpawnMethod.RoundRobin;

    [Header("Mercenary Parameters")]
    public bool spawnMercenary;
    [SerializeField] private TextAsset[] messages;
    [SerializeField] private WeaponManager[] possibleMercenaryWeapons;

    // internal helpers
    private int roundRobinCounter = 0;
    private const int maxSampleAttemptsPerSpawn = 5;
    public Transform PlayerTransform => Player.transform;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        npcColliders = new Collider[maxNPCCount];

        CreateNewPool();
        mercenaryPool = CharacterPool(3, 5, true, mercenaryPrefab);

        if (GameObjectTool.TryFindFirstObject<NavMeshBaker>(out var navMeshBuilder))
        {
            navMeshBuilder.OnNavMeshBuild += HandleSpawnUpdater;
        }
    }

    public void SetPlayerAndCompanion(CharacterManager p, PlayerCompanion c)
    {
        Player = p;
        Companion = c;
    }

    /// <summary>
    /// Called by NavMeshBaker when a partial navmesh build completes for a region (bounds).
    /// Updates the list of NPCs inside the bounds and releases distant NPCs; then spawns new NPCs around the player tile.
    /// </summary>
    public void HandleSpawnUpdater(Bounds bounds)
    {
        int countFound = Physics.OverlapBoxNonAlloc(bounds.center, bounds.extents, npcColliders, Quaternion.identity, characterLayer);

        var nearCharacters = new List<CharacterManager>(countFound);
        for (int i = 0; i < countFound; i++)
        {
            var col = npcColliders[i];
            if (col == null)
            {
                continue;
            }

            if(npcColliders[i].TryGetComponent<CharacterManager>(out var character) && character != Player)
            {
                nearCharacters.Add(character);
            }
        }
        HashSet<CharacterManager> present = new(nearCharacters);
        var currentNPCsCopy = new List<CharacterManager>(npcList);

        foreach (var npc in currentNPCsCopy)
        {
            if(npc == null)
            {
                npcList.Remove(npc);
                continue;
            }

            if(present.Contains(npc) != true || npc.isDead)
            {
                npcList.Remove(npc);
                npc.mySpawnPool?.Release(npc);
                spawnedNPCCount = Mathf.Max(0, spawnedNPCCount - 1);
            }
        }

        if(mercenary != null && mercenary.isDead)
        {
            mercenary = null;
        }
        HandleSpawn(true);

        spawnMercenary = (Random.Range(0, 100) > 80);
        if (spawnMercenary && mercenary == null) 
        {
            HandleSpawn(false);
        }
        Companion.mentalState = (mercenary != null) ? CombatMentalState.High_Alert : CombatMentalState.Friendly;
    }

    private void HandleSpawn(bool isNPC)
    {
        Transform playerTransform = Player.transform;
        Vector3 currentTilePosition = new
        (
            Mathf.FloorToInt(playerTransform.position.x) / cnst_tileSize,
            Mathf.FloorToInt(playerTransform.position.y) / cnst_tileSize,
            Mathf.FloorToInt(playerTransform.position.z) / cnst_tileSize
        );

        if (!spawnedTiles.Contains(currentTilePosition))
        {
            spawnedTiles.Add(currentTilePosition);
        }
        HandleSpawn(currentTilePosition, isNPC);
    }

    private void HandleSpawn(Vector3 currentTilePosition, bool isNPC)
    {
        if (!isNPC && mercenary != null)
        {
            return;
        }
        if (isNPC && spawnedNPCCount >= maxNPCCount)
        {
            return;
        }
        SpawnMesh(isNPC, currentTilePosition);
    }

    private void SpawnMesh(bool isNPC, Vector3 currentTilePosition)
    {
        int navMeshCalc = (navMeshSize.x / cnst_tileSize) / 2;
        for (int x = -navMeshCalc; x < navMeshCalc; x++)
        {
            for (int z = -navMeshCalc; z < navMeshCalc; z++)
            {
                Vector3 tilePosition = new(currentTilePosition.x + x, currentTilePosition.y, currentTilePosition.z + z);
                if(isNPC)
                {
                    NPC_SpawnMesh(tilePosition);
                    continue;
                }
                Mercenary_SpawnMesh(tilePosition);
            }
        }
    }

    private void NPC_SpawnMesh(Vector3 tilePosition)
    {
        if (spawnedNPCCount >= maxNPCCount || spawnedTiles.Contains(tilePosition))
        {
            return;
        }

        int enemiesSpawnedPerTile = 0;
        spawnedTiles.Add(tilePosition);

        int rndCount = Random.Range(7, maxNPCCount);
        while ((spawnedNPCCount < rndCount))
        {
            int spawnIndex;
            int length = npcsArray.Length;
            if (spawnMethod == SpawnMethod.RoundRobin)
            {
                spawnIndex = roundRobinCounter % length;
                roundRobinCounter++;
            }
            else
            {
                spawnIndex = Random.Range(0, length);
            }

            bool spawnSucceeded = false;
            CharacterManager prefab = npcsArray[spawnIndex];
            for (int attempt = 0; attempt < maxSampleAttemptsPerSpawn && (spawnedNPCCount < maxNPCCount); attempt++)
            {
                if (SpawnNPCOnNavmesh(tilePosition, prefab))
                {
                    enemiesSpawnedPerTile++;
                    spawnedNPCCount++;
                    spawnSucceeded = true;
                    break;
                }
            }

            if (!spawnSucceeded)
            {
                break;
            }
        }
    }

    private void Mercenary_SpawnMesh(Vector3 tilePos)
    {
        if (mercenary != null)
        {
            return;
        }

        int walkableIndex = NavMesh.GetAreaFromName("Walkable");
        int walkableAreaMask = (walkableIndex >= 0) ? (1 << walkableIndex) : NavMesh.AllAreas;

        Vector3 randomPos = new(Random.Range(-27.5f, 27.5f), 0.0f, Random.Range(-27.5f, 27.5f));
        Vector3 samplePos = tilePos * cnst_tileSize + randomPos;

        if (NavMesh.SamplePosition(samplePos, out NavMeshHit hit, spawnRadius, walkableAreaMask))
        {
            spawnPosition = hit.position;
            CharacterManager spawn = mercenaryPool.Get();
            if(spawn != null)
            {
                mercenary = spawn;
                spawn.mySpawnPool = mercenaryPool;
            }
        }

    }

    /// <summary>
    /// Try to sample a walkable position inside the tile and spawn an NPC from the pool.
    /// Returns true if spawn succeeded (pooled object is positioned, activated and added).
    /// </summary>
    private bool SpawnNPCOnNavmesh(Vector3 tilePos, CharacterManager prefab)
    {
        if (prefab == null)
        {
            return false;
        }

        int walkableIndex = NavMesh.GetAreaFromName("Walkable");
        int walkableAreaMask = (walkableIndex >= 0) ? (1 << walkableIndex) : NavMesh.AllAreas;

        Vector3 randomPos = new(Random.Range(-27.5f, 27.5f), 0.0f, Random.Range(-27.5f, 27.5f));
        Vector3 samplePos = tilePos * cnst_tileSize + randomPos;

        if (NavMesh.SamplePosition(samplePos, out NavMeshHit hit, spawnRadius, walkableAreaMask))
        {
            spawnPosition = hit.position;
            CharacterManager spawn = npcPools[prefab].Get();
            if (spawn == null) return false;

            spawn.mySpawnPool = npcPools[prefab];

            if (!npcList.Contains(spawn))
            {
                npcList.Add(spawn);
            }
            return true;
        }
        return false;
    }


    #region Object Pool

    private CharacterManager CreateObject(CharacterManager objToSpawn)
    {
        CharacterManager spawn = Instantiate(objToSpawn, spawnParent);
        spawn.gameObject.SetActive(false);
        spawn.canUpdate = false;
        return spawn;
    }

    private void GetObject(CharacterManager spawnedObject, bool isMercenary)
    {
        StartCoroutine(PrepareGetObject(spawnedObject, isMercenary));
    }

    private IEnumerator PrepareGetObject(CharacterManager spawnedObject, bool isMercenary)
    {
        spawnedObject.canUpdate = false;
        Transform spawnTransform = spawnedObject.transform;
        spawnTransform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

        yield return null;
        if (GameObjectTool.TryGetComponentInChildren(spawnTransform, out RandomSkinSelector rss))
        {
            rss.RandomSkin();
        }

        spawnedObject.StatsManager.ResetStats();
        PrepareMercenary(isMercenary, spawnedObject);
        spawnedObject.gameObject.SetActive(true);

        var agent = spawnedObject.Agent;
        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(spawnPosition);
            agent.ResetPath();
        }

        yield return null;
        spawnedObject.canUpdate = true;
    }


    private void PrepareMercenary(bool isMercenary, CharacterManager mercenary)
    {
        if(isMercenary != true)
        {
            return;
        }
        mercenary.SetTarget(Player);
        mercenary.hasAssignment = true;
        mercenary.mentalState = CombatMentalState.High_Alert;

        mercenary.Assignment.AddListener(() => NewAssignment(mercenary));
        mercenary.CombatManager.CreateEnemyWeapons(weapon, possibleMercenaryWeapons);
    }

    private void NewAssignment(CharacterManager mercenary)
    {
        selectedMessage = GameObjectTool.GetRandomExcluding(selectedMessage, messages);
        DialogueManager.Instance.HandleDialogue(selectedMessage, mercenary);
    }

    private ObjectPool<CharacterManager> CharacterPool(int min, int max, bool isMercenary, CharacterManager objToSpawn)
    {
        ObjectPool<CharacterManager> objectPool = new(
            () => CreateObject(objToSpawn),              // create
            spawn => { GetObject(spawn, isMercenary); }, // onGet
            spawn => { spawn.gameObject.SetActive(false); spawn.canUpdate = false; }, // onRelease
            spawn => { if (spawn != null) GameObject.Destroy(spawn.gameObject); },   // onDestroy
            false, min, max
        );
        return objectPool;
    }

    private void CreateNewPool()
    {
        foreach (var character in npcsArray)
        {
            if (character == null) continue;
            if (npcPools.ContainsKey(character))
            {
                continue;
            }
            npcPools[character] = CharacterPool(30, 35, false, character);
        }
    }
    #endregion
}
