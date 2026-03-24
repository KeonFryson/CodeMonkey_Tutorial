using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to every prefab that can be placed in the world.
/// Defines how many grid cells this structure occupies.
/// </summary>
public class BuildablePrefab : MonoBehaviour
{
    [Header("Prefab Info")]
    public string buildingName = "Structure";

    [Tooltip("Local-space cell offsets this structure occupies. (0,0,0) = its pivot cell.")]
    public List<Vector3Int> occupiedCellOffsets = new List<Vector3Int> { Vector3Int.zero };

    /// <summary>
    /// Returns the world-space grid cells this prefab occupies given a root cell.
    /// </summary>
    public List<Vector3Int> GetWorldCells(Vector3Int rootCell)
    {
        var cells = new List<Vector3Int>();
        foreach (var offset in occupiedCellOffsets)
            cells.Add(rootCell + offset);
        return cells;
    }

    /// <summary>
    /// Stores which cells this placed instance owns (set after placement).
    /// </summary>
    [HideInInspector]
    public List<Vector3Int> placedCells = new List<Vector3Int>();
}
