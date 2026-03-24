using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to every prefab that can be placed in the world.
/// Defines how many grid cells this structure occupies.
/// 
/// At runtime Start() automatically registers this building with GridManager
/// and moves it to the PlacedBuilding layer — so editor-placed buildings
/// block placement just like runtime-placed ones.
/// </summary>
public class BuildablePrefab : MonoBehaviour
{
    [Header("Prefab Info")]
    public string buildingName = "Structure";

    [Tooltip("Local-space cell offsets this structure occupies. (0,0,0) = its pivot cell.")]
    public List<Vector3Int> occupiedCellOffsets = new List<Vector3Int> { Vector3Int.zero };

    [Tooltip("Must match the 'Placed Building Layer' set on BuildingSystem.")]
    public string placedLayerName = "PlacedBuilding";

    /// <summary>
    /// Stores which cells this placed instance owns (set after placement).
    /// </summary>
    [HideInInspector]
    public List<Vector3Int> placedCells = new List<Vector3Int>();

    void Start()
    {
        // Only self-register if BuildingSystem hasn't already done it
        // (runtime-placed buildings have placedCells filled before Start runs)
        if (placedCells.Count == 0)
            RegisterWithGrid();

        // Always ensure the correct layer is set (covers editor-placed buildings)
        SetLayerRecursive(gameObject, LayerMask.NameToLayer(placedLayerName));
    }

    /// <summary>
    /// Snaps to the grid, computes occupied cells, and registers them
    /// with GridManager. Called automatically for editor-placed buildings.
    /// </summary>
    public void RegisterWithGrid()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogWarning($"[BuildablePrefab] No GridManager in scene — {buildingName} not registered.");
            return;
        }

        // Snap position to grid in case it was placed slightly off
        Vector3 snapped = GridManager.Instance.SnapToGrid(transform.position);
        transform.position = snapped;

        Vector3Int rootCell = GridManager.Instance.WorldToCell(snapped);
        placedCells = GetWorldCells(rootCell);

        GridManager.Instance.OccupyCells(placedCells);
        Debug.Log($"[BuildablePrefab] Auto-registered '{buildingName}' at cell {rootCell}");
    }

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

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        if (layer == -1) return; // layer name not found, skip
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }
}