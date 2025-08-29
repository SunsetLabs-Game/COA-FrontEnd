using UnityEngine;
using System.Collections.Generic;

public class EnviromentGenerator : MonoBehaviour
{
    private Cell previousCell;
    private bool generationComplete;

    private Queue<Cell> collapseQueue = new();
    private List<TileVariant> validTilesList = new();

    [Header("Generation Parameters")]
    [SerializeField] private int dimensions;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector2Int cellSpacing;

    [Header("Indicator")]
    [SerializeField] private Material boundaryMat;

    [Header("Modification")]
    [SerializeField] private Cell cellToModify;
    [Range(0, 5)][SerializeField] private int modifyRange;

    [Header("Cell Objects")]
    [SerializeField] private Cell cellObject;
    [SerializeField] private List<Cell> cellList = new();

    private void OnValidate()
    {
        HandleSpacing();
    }

    private void HandleSpacing()
    {
        if (cellList == null || cellList.Count < dimensions * dimensions)
        {
            return;
        }

        for (int x = 0; x < dimensions; x++)
        {
            for (int y = 0; y < dimensions; y++)
            {
                Vector3 spacing = new(x * cellSpacing.x, 0.0f, y * cellSpacing.y);

                int index = x * dimensions + y;
                cellList[index].transform.localPosition = spacing;
            }
        }
    }

    public void IndicateMaterials()
    {
        foreach(var cell in cellList)
        {
            if(cell.BoundaryType != TileDirection.Ignore)
            {
                cell.CellRenderer.material = boundaryMat;
            }
        }
    }

    public void GenerateGridCells()
    {
        generationComplete = false;
        cellList.ForEach(x => DestroyImmediate(x.gameObject));
        cellList.Clear();

        for(int x = 0; x < dimensions; x++)
        {
            for(int y = 0; y < dimensions; y++)
            {
                Vector3 spacing = new(x * cellSpacing.x, 0.0f, y * cellSpacing.y);
                Cell newCell = Instantiate(cellObject, spawnPoint);

                cellList.Add(newCell);
                newCell.InitializeCell(x, y, false, dimensions);
                newCell.name = $"CellObject: {cellList.IndexOf(newCell)}";
                newCell.transform.SetLocalPositionAndRotation(spacing, Quaternion.identity);
            }
        }
        cellList.ForEach(x => x.GetNeighboringCells(cellList));
    }

    public void HandleCellCollapse()
    {
        if (cellList.Count == 0 || generationComplete)
        {
            return;
        }
        Cell startCell = cellList[0];
        startCell.SetFirstCellTemporaryList();

        collapseQueue.Clear();
        collapseQueue.Enqueue(startCell);
        PropagateCollapse();
    }

    public void ModifySelectedCell()
    {
        int x = cellToModify.CellDimensions().x;
        int y = cellToModify.CellDimensions().y;

        int range = modifyRange % (2 * dimensions);
        for(int i = x; i < x + range; i++)
        {
            for(int j = y; j < y + range; j++)
            {
                int index = i * dimensions + j;
                cellList[index].ModifyCell();
            }    
        }
    }

    public void ResetAndRegenerate()
    {
        StopAllCoroutines(); // If you're using async collapse later
        collapseQueue.Clear();

        // Destroy current cells and clear lists
        cellList.ForEach(cell => DestroyImmediate(cell.gameObject));
        cellList.Clear();

        // Generate fresh cells
        GenerateGridCells();

        // Start new collapse process
        HandleCellCollapse();
    }

    //Logic
    private void PropagateCollapse()
    {
        while (collapseQueue.Count > 0)
        {
            Cell current = collapseQueue.Dequeue();
            if (current.hasCollapsed)
            {
                continue;
            }
            CollapseSingleCell(current);
            EnqueueLowestEntropyNeighbors(current);
        }
        CheckForUncollapsedCells();
    }

    private void CollapseSingleCell(Cell cell)
    {
        if (cell.hasCollapsed)
        {
            return;
        }
        if (cell.TemporaryTilesCount() == 0)
        {
            print(previousCell + " " + cell);
        }
        int random = Random.Range(0, cell.TemporaryTilesCount());

        if(random > cell.TemporaryTilesCount()) { print(cell); }
        var selectedChoice = cell.TemporaryTilesList[random];

        cell.InstantiateTile(selectedChoice, validTilesList);
        previousCell = cell;
    }

    private void EnqueueLowestEntropyNeighbors(Cell cell)
    {
        List<Cell> uncollapsedNeighbors = new();
        foreach(var c in cell.neighboringCells)
        {
            if(c.hasCollapsed)
            {
                continue;
            }
            uncollapsedNeighbors.Add(c);
        }

        if (uncollapsedNeighbors.Count == 0)
        {
            return;
        }

        uncollapsedNeighbors.Sort((a, b) => a.TotalNeighborEntropies - b.TotalNeighborEntropies);
        int lowestEntropy = uncollapsedNeighbors[0].TotalNeighborEntropies;
        foreach (var neighbor in uncollapsedNeighbors)
        {
            if (neighbor.TotalNeighborEntropies == lowestEntropy && !collapseQueue.Contains(neighbor))
            {
                collapseQueue.Enqueue(neighbor);
            }
        }
    }

    private void CheckForUncollapsedCells()
    {
        if (generationComplete)
        {
            return;
        }
        var uncollapsedCells = cellList.FindAll(x => x.hasCollapsed != true && x.TemporaryTilesCount() > 0);
        int count = uncollapsedCells.Count;
        if(count == 0)
        {
            generationComplete = true;
            return;
        }
        int random = Random.Range(0, count);
        Cell cellToCollapse = uncollapsedCells[random];
        cellToCollapse.SetFirstCellTemporaryList();

        collapseQueue.Clear();
        collapseQueue.Enqueue(cellToCollapse);
        PropagateCollapse();
    }
}
