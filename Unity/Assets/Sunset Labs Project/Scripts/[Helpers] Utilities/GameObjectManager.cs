using UnityEngine;
using UnityEngine.Pool;

public class GameObjectManager : MonoBehaviour
{
    public static ObjectPool<TrailRenderer> bulletTrailPool { get; private set; }
    public static ObjectPool<GameObject> woodBulletHolesPool { get; private set; }
    public static ObjectPool<GameObject> metalBulletHolesPool { get; private set; }
    public static ObjectPool<GameObject> cementBulletHolesPool { get; private set; }


    [Header("Impact Objects")]
    [SerializeField] private GameObject woodBulletHoles;
    [SerializeField] private GameObject metalBulletHoles;
    [SerializeField] private GameObject cementBulletHoles;
    [SerializeField] protected TrailRenderer bulletTrailPrefab;

    [Header("Objects To Spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private UIBar healthBarPrefab;
    [SerializeField] private GameObject bloodPrefab;

    private void Awake()
    {
        bulletTrailPool = TrailFXPool(spawnPoint, bulletTrailPrefab);

        //Impact Objects
        woodBulletHolesPool = GameObjectPool(spawnPoint, woodBulletHoles);
        metalBulletHolesPool = GameObjectPool(spawnPoint, metalBulletHoles);
        cementBulletHolesPool = GameObjectPool(spawnPoint, cementBulletHoles);
    }

    public static ObjectPool<GameObject> GameObjectPool(Transform spawnPoint, GameObject objectToPool)
    {
        ObjectPool<GameObject> objectPool = new
        (
            () => { return GameObject.Instantiate(objectToPool, spawnPoint); },
            spawnObject => { spawnObject.SetActive(true); },
            spawnObject => { spawnObject.SetActive(false); },
            spawnObject => { GameObject.Destroy(spawnObject); },
            false, 50, 100
        );
        return objectPool;
    }

    public static ObjectPool<TrailRenderer> TrailFXPool(Transform spawnPoint, TrailRenderer objectToPool)
    {
        ObjectPool<TrailRenderer> objectPool = new
        (
            () => { return GameObject.Instantiate(objectToPool, spawnPoint); },
            spawnObject => { spawnObject.gameObject.SetActive(true); },
            spawnObject => { spawnObject.gameObject.SetActive(false); },
            spawnObject => { GameObject.Destroy(spawnObject.gameObject); },
            false, 400, 500
        );
        return objectPool;
    }
}
