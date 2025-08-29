using UnityEngine;
using System.Collections.Generic;

public class FloorTile : MonoBehaviour
{
    private List<GameObject> props = new();
    private List<GameObject> buildings = new();

    [Header("Road Generation Parameters")]
    public bool hasCollapsed;
    public Vector2 tileDimensions;

    [field: Header("Status")]
    [field: SerializeField] public TileType TypeOfTile { get; private set; }
    [field: SerializeField] public TilesBoundary Boundary { get; private set; }

    [Header("Parameters")]
    [SerializeField] private GameObject[] possibleProps;
    [SerializeField] private GameObject[] possibleBuildings;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] props_SpawnPoint;
    [SerializeField] private Transform[] building_SpawnPoint;

    private void InitializeTile()
    {
        GenerateObjectRandomly(possibleProps, props_SpawnPoint, props);
        GenerateObjectRandomly(possibleBuildings, building_SpawnPoint, buildings);
    }

    public void SetParameters(TilesBoundary boundary)
    {
        Boundary = boundary;
    }

    private void SpawnObject(GameObject obj, Transform point, List<GameObject> list)
    {
        GameObject newObj = Instantiate(obj, point);

        list.Add(newObj);
        newObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    private void GenerateObjectRandomly(GameObject[] possibleObjects, Transform[] points, List<GameObject> list)
    {
        foreach (var point in points)
        {
            int random = Random.Range(0, 10);
            bool shouldSpawn = (random > 4);
            if (shouldSpawn != true)
            {
                continue;
            }
            random = Random.Range(0, possibleObjects.Length);
            GameObject objectToSpawn = possibleObjects[random];
            SpawnObject(objectToSpawn, point, list);
        }
    }
}
