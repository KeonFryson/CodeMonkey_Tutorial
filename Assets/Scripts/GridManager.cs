using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the world grid and tracks which cells are occupied.
/// Attach to an empty GameObject in your scene (e.g. "BuildingSystem").
/// </summary>
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    [Tooltip("Size of each grid cell in world units.")]
    public float cellSize = 1f;

    // Stores occupied cell positions (using Vector3Int for x/y/z grid coords)
    private HashSet<Vector3Int> occupiedCells = new HashSet<Vector3Int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Snaps a world position to the nearest grid cell center.
    /// </summary>
    public Vector3 SnapToGrid(Vector3 worldPosition)
    {
        float x = Mathf.Round(worldPosition.x / cellSize) * cellSize;
        float y = Mathf.Round(worldPosition.y / cellSize) * cellSize;
        float z = Mathf.Round(worldPosition.z / cellSize) * cellSize;
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Converts a world position to grid cell coordinates.
    /// </summary>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return new Vector3Int(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.y / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }

    /// <summary>
    /// Returns true if ALL cells required by the given positions are free.
    /// </summary>
    public bool AreCellsFree(List<Vector3Int> cells)
    {
        foreach (var cell in cells)
        {
            if (occupiedCells.Contains(cell))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Marks a list of cells as occupied.
    /// </summary>
    public void OccupyCells(List<Vector3Int> cells)
    {
        foreach (var cell in cells)
            occupiedCells.Add(cell);
    }

    /// <summary>
    /// Frees a list of cells.
    /// </summary>
    public void FreeCells(List<Vector3Int> cells)
    {
        foreach (var cell in cells)
            occupiedCells.Remove(cell);
    }

    /// <summary>
    /// Draws the grid in the Scene view for debugging.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        int range = 20;
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize, 0.01f, cellSize));
            }
        }
    }
}
