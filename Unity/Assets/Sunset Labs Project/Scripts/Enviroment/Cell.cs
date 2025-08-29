using UnityEngine;
using System.Collections.Generic;

public class Cell : MonoBehaviour
{
    public Cell Left { get; private set; }
    public Cell Right { get; private set; }
    public Cell North { get; private set; }
    public Cell South { get; private set; }

    [TextArea] public string DebugMessage;

    [field: SerializeField] public int TotalNeighborEntropies { get; private set; }
    private CornerBoundary cornerBoundaryType = CornerBoundary.None;
    [field: SerializeField] public List<TileVariant> TemporaryTilesList { get; private set; } = new();

    [Header("Status")]
    public bool hasCollapsed;
    public TileVariant selectedChoice;
    [field: SerializeField] public MeshRenderer CellRenderer { get; private set; }
    [field: SerializeField] public TileDirection BoundaryType { get; private set; } = TileDirection.Ignore;

    [Header("Generation Parameters")]
    public List<Cell> neighboringCells = new();
    [SerializeField] private FloorTile grassTile;
    [SerializeField] private FloorTile selectedTile;
    [SerializeField] private Vector2Int cellDimensions;
    [field: SerializeField] public FloorTile[] TileObjects { get; private set; }

    public int TemporaryTilesCount() => TemporaryTilesList.Count;

    public Vector2Int CellDimensions() => cellDimensions;

    public void ModifyCell()
    {
        DestroyImmediate(selectedTile.gameObject);
        selectedTile = Instantiate(grassTile, transform);
        selectedTile.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        hasCollapsed = true;
        CellRenderer.enabled = false;
    }

    public void SetFirstCellTemporaryList()
    {
        TemporaryTilesList.RemoveAll(x => SelectTilesBasedOnBoundaryType(x) != true);
    }

    public void InitializeCell(int x, int y, bool hasCollapsed, int dimensions)
    {
        DebugMessage = "Start: ";
        neighboringCells.Clear();

        cellDimensions.x = x;
        cellDimensions.y = y;
        this.hasCollapsed = hasCollapsed;

        int max = dimensions - 1;
        BoundaryType = Tiles_Helper.GetBoundaryType(x, y, max);
        cornerBoundaryType = Tiles_Helper.GetCornerType(x, y, max);
        foreach(var tile in TileObjects)
        {
            int maxSteps = tile.Boundary.UniqueRotationCount();
            for(int steps = 0; steps < maxSteps; steps++)
            {
                var variant = new TileVariant(tile, steps);
                TemporaryTilesList.Add(variant);
            }
        }
    }

    public void GetNeighboringCells(List<Cell> cellsList)
    {
        North = GetCell(cellDimensions.x, cellDimensions.y + 1, cellsList);
        Left = GetCell(cellDimensions.x + 1, cellDimensions.y, cellsList);
        Right = GetCell(cellDimensions.x - 1, cellDimensions.y, cellsList);
        South = GetCell(cellDimensions.x, cellDimensions.y - 1, cellsList);
    }

    public void InstantiateTile(TileVariant tile, List<TileVariant> validList)
    {
        selectedChoice = tile;
        selectedTile = Instantiate(tile.Prefab, transform);

        selectedTile.SetParameters(tile.Boundary);
        Quaternion rot = Quaternion.Euler(0, 90f * tile.Rotations, 0);
        selectedTile.transform.SetLocalPositionAndRotation(Vector3.zero, rot);

        hasCollapsed = true;
        CellRenderer.enabled = false;
        RemoveNeighboringInvalidTiles(validList);
    }

    public void RecreateCell(List<TileVariant> tiles)
    {
        TemporaryTilesList.Clear();
        TemporaryTilesList.AddRange(tiles);
        UpdateNeighborEntropy();
    }

    private void RemoveNeighboringInvalidTiles(List<TileVariant> validList)
    {
        foreach(var neighbor in neighboringCells)
        {
            if(neighbor.hasCollapsed)
            {
                continue;
            }
            PruneNeighbor(neighbor, validList);
        }
    }

    private void PruneNeighbor(Cell neighbor, List<TileVariant> validList)
    {
        validList.Clear();
        foreach (var variant in neighbor.TemporaryTilesList)
        {
            string message = $"{neighbor} is checking if variant: {variant.Prefab} matches {selectedTile}";
            DebugMessage += message;
            if (IsValidForNeighbor(neighbor, variant) != true)
            {
                continue;
            }
            validList.Add(variant);
        }
        neighbor.RecreateCell(validList);
    }

    private void UpdateNeighborEntropy()
    {
        TotalNeighborEntropies = 0;
        foreach (var neighbor in neighboringCells)
        {
            TotalNeighborEntropies += neighbor.TemporaryTilesList.Count;
        }
    }

    private Cell GetCell(int x, int y, List<Cell> cellsList)
    {
        Cell neighbor = cellsList.Find(cell => cell.cellDimensions.x == x && cell.cellDimensions.y == y);
        if (neighbor != null && !neighboringCells.Contains(neighbor))
        {
            neighboringCells.Add(neighbor);
        }
        return neighbor;
    }

    private bool SelectTilesBasedOnBoundaryType(TileVariant variant)
    {
        TileType tileType = variant.Prefab.TypeOfTile;
        if (BoundaryType == TileDirection.Ignore)
        {
            return tileType == TileType.Normal;
        }

        // If it's a boundary cell, validate side matches
        if (Tiles_Helper.BoundedCell(variant.Boundary, BoundaryType) != 2)
        {
            return false;
        }

        // If it's a corner, restrict to CornerPiece tiles with matching second side
        TileDirection corner = Tiles_Helper.GetOtherBoundaryTypeViaCornerType(cornerBoundaryType, BoundaryType);
        if (corner != TileDirection.Ignore)
        {
            return tileType == TileType.CornerPiece
                && Tiles_Helper.BoundedCell(variant.Boundary, corner) == 2;
        }
        // Otherwise it's just a boundary (not a corner) → only Fence tiles allowed
        return tileType == TileType.Fence;
    }

    private bool MatchesNeighbor(Cell neighbor, TileVariant variant)
    {
        TileDirection toNeighbor = Tiles_Helper.GetDirectionToNeighbor(this, neighbor);
        TileDirection fromNeighbor = Tiles_Helper.GetOppositeDirection(toNeighbor);

        int neighborSide = Tiles_Helper.BoundedCell(variant.Boundary, fromNeighbor);
        int selfSide = Tiles_Helper.BoundedCell(selectedTile.Boundary, toNeighbor);
        return neighborSide == selfSide;
    }

    private bool IsValidForNeighbor(Cell cell, TileVariant variant)
    {
        TileType tileType = variant.Prefab.TypeOfTile;
        if (cell.BoundaryType == TileDirection.Ignore)
        {
            return (tileType == TileType.Normal) && MatchesNeighbor(cell, variant);
        }

        if(tileType == TileType.Normal)
        {
            return false;
        }

        //Is Bordering Cell
        //Check If Tile Wall Is Not Facing Direction
        if (Tiles_Helper.BoundedCell(variant.Boundary, cell.BoundaryType) != 2)
        {
            return false;
        }
        TileDirection corner = Tiles_Helper.GetOtherBoundaryTypeViaCornerType(cell.cornerBoundaryType, cell.BoundaryType);
        bool validTilePiece = (corner == TileDirection.Ignore) ? (tileType == TileType.Fence) :
            (tileType == TileType.CornerPiece && Tiles_Helper.BoundedCell(variant.Boundary, corner) == 2);
        return validTilePiece;
    }
}
