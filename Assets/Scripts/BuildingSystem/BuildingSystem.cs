using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Main building system controller — uses Unity's New Input System.
/// Attach to the same GameObject as GridManager (or any persistent object).
///
/// SETUP:
///  1. Add your placeable prefabs (with BuildablePrefab component) to the
///     'buildingPrefabs' list in the Inspector.
///  2. Assign a 'groundLayer' LayerMask so raycasts hit your terrain/floor.
///  3. Wire up input actions OR use the default keyboard/mouse bindings below.
///     Default bindings:
///       Q / E          – Rotate
///       Scroll Wheel   – Cycle prefabs
///       Left Click     – Place
///       Right Click    – Delete
///       (Call HandleModeToggle() from a UI button or another InputAction)
/// </summary>
public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem Instance { get; private set; }

    [Header("Prefabs")]
    public List<GameObject> buildingPrefabs = new List<GameObject>();

    [Header("Placement Settings")]
    public LayerMask groundLayer;
    public float rotationStep = 90f;
    public Material validGhostMaterial;
    public Material invalidGhostMaterial;

    [Tooltip("Layer assigned to buildings after placement so the ground raycast ignores them.\nCreate a layer called 'PlacedBuilding' and assign it here.")]
    public LayerMask placedBuildingLayer;

    // ── State ──────────────────────────────────────────────────────────────
    private bool buildModeActive = false;
    private int selectedIndex = 0;
    private float currentRotation = 0f;

    private GameObject ghostObject;
    private BuildablePrefab ghostBuildable;
    private bool placementValid = false;

    private List<GameObject> placedObjects = new List<GameObject>();

    // ── Input ──────────────────────────────────────────────────────────────
    private Mouse mouse;
    private Keyboard keyboard;

    // ── Unity ──────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        mouse = Mouse.current;
        keyboard = Keyboard.current;
    }

    void Update()
    {
        if (!buildModeActive) return;

        HandlePrefabSelection();
        HandleRotation();
        UpdateGhost();
        HandlePlacement();
        HandleDeletion();
    }

    private void Start()
    {
        if (buildingPrefabs.Count == 0)
            Debug.LogWarning("No building prefabs assigned to BuildingSystem.");

        foreach (var prefab in buildingPrefabs)
        {
            if (prefab.GetComponent<BuildablePrefab>() == null)
                Debug.LogWarning($"Prefab '{prefab.name}' is missing a BuildablePrefab component.");
        }


        // Fix: find the components, then extract their GameObjects
        BuildablePrefab[] placeObjects = FindObjectsByType<BuildablePrefab>(FindObjectsSortMode.None);
        GameObject[] placeObjectGOs = System.Array.ConvertAll(placeObjects, bp => bp.gameObject);

        // Optional: Add existing placed objects to the system (if any)
        foreach (var obj in placeObjectGOs)
        {
            if (!placedObjects.Contains(obj))
                placedObjects.Add(obj);
        }
    }

    // ── Build Mode Toggle ──────────────────────────────────────────────────
    /// <summary>
    /// Call this from a UI Button's OnClick, or bind it to an InputAction.
    /// Example InputAction binding:
    ///   myToggleAction.performed += _ => HandleModeToggle();
    /// </summary>
    public void HandleModeToggle()
    {
        buildModeActive = !buildModeActive;

        if (buildModeActive)
            SpawnGhost();
        else
            DestroyGhost();

        Debug.Log("Build mode: " + buildModeActive);
    }

    // ── Ghost Management ───────────────────────────────────────────────────
    void SpawnGhost()
    {
        if (buildingPrefabs.Count == 0) return;
        DestroyGhost();

        ghostObject = Instantiate(buildingPrefabs[selectedIndex]);
        ghostBuildable = ghostObject.GetComponent<BuildablePrefab>();

        foreach (var col in ghostObject.GetComponentsInChildren<Collider>())
            col.enabled = false;

        ApplyGhostMaterial(invalidGhostMaterial);
    }

    void DestroyGhost()
    {
        if (ghostObject != null) Destroy(ghostObject);
        ghostObject = null;
        ghostBuildable = null;
    }

    void ApplyGhostMaterial(Material mat)
    {
        if (mat == null || ghostObject == null) return;
        foreach (var r in ghostObject.GetComponentsInChildren<Renderer>())
            r.material = mat;
    }

    // ── Per-Frame Ghost Update ─────────────────────────────────────────────
    void UpdateGhost()
    {
        if (ghostObject == null || mouse == null) return;

        Vector2 screenPos = mouse.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer))
        {
            Vector3 snapped = GridManager.Instance.SnapToGrid(hit.point);
            ghostObject.transform.position = snapped;
            ghostObject.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            Vector3Int rootCell = GridManager.Instance.WorldToCell(snapped);
            List<Vector3Int> cells = ghostBuildable != null
                ? ghostBuildable.GetWorldCells(rootCell)
                : new List<Vector3Int> { rootCell };

            placementValid = GridManager.Instance.AreCellsFree(cells);
            ApplyGhostMaterial(placementValid ? validGhostMaterial : invalidGhostMaterial);
        }
    }

    // ── Placement ──────────────────────────────────────────────────────────
    void HandlePlacement()
    {
        if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;
        if (!placementValid || ghostObject == null) return;

        Vector3 pos = ghostObject.transform.position;
        Vector3Int rootCell = GridManager.Instance.WorldToCell(pos);
        List<Vector3Int> cells = ghostBuildable != null
            ? ghostBuildable.GetWorldCells(rootCell)
            : new List<Vector3Int> { rootCell };

        if (!GridManager.Instance.AreCellsFree(cells)) return;

        GameObject placed = Instantiate(
            buildingPrefabs[selectedIndex],
            pos,
            Quaternion.Euler(0, currentRotation, 0)
        );

        // Move to PlacedBuilding layer so the ground raycast ignores it
        if (placedBuildingLayer.value > 0)
        {
            int layer = Mathf.RoundToInt(Mathf.Log(placedBuildingLayer.value, 2));
            SetLayerRecursive(placed, layer);
        }

        BuildablePrefab bp = placed.GetComponent<BuildablePrefab>();
        if (bp != null) bp.placedCells = cells;

        GridManager.Instance.OccupyCells(cells);
        placedObjects.Add(placed);

        Debug.Log($"Placed {buildingPrefabs[selectedIndex].name} at cell {rootCell}");
    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
            SetLayerRecursive(child.gameObject, layer);
    }

    // ── Deletion ───────────────────────────────────────────────────────────
    void HandleDeletion()
    {
        if (mouse == null || !mouse.rightButton.wasPressedThisFrame) return;

        Vector2 screenPos = mouse.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, placedBuildingLayer)) return;

        BuildablePrefab bp = hit.collider.GetComponentInParent<BuildablePrefab>();
        if (bp == null) return;

        GridManager.Instance.FreeCells(bp.placedCells);
        placedObjects.Remove(bp.gameObject);
        Destroy(bp.gameObject);

        Debug.Log("Deleted structure.");
    }

    // ── Prefab Selection (Scroll Wheel) ────────────────────────────────────
    void HandlePrefabSelection()
    {
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;
        if (scroll == 0f) return;

        selectedIndex = (selectedIndex + (scroll > 0 ? 1 : -1) + buildingPrefabs.Count)
                        % buildingPrefabs.Count;
        SpawnGhost();
    }

    // ── Rotation (Q / E) ───────────────────────────────────────────────────
    void HandleRotation()
    {
        if (keyboard == null) return;

        if (keyboard.qKey.wasPressedThisFrame) currentRotation -= rotationStep;
        if (keyboard.eKey.wasPressedThisFrame) currentRotation += rotationStep;
    }

    // ── Public API ─────────────────────────────────────────────────────────
    public void SelectPrefab(int index)
    {
        if (index < 0 || index >= buildingPrefabs.Count) return;
        selectedIndex = index;
        if (buildModeActive) SpawnGhost();
    }

    public void SetBuildMode(bool active)
    {
        buildModeActive = active;
        if (active) SpawnGhost();
        else DestroyGhost();
    }

    public bool IsBuildModeActive() => buildModeActive;
    public int GetSelectedIndex() => selectedIndex;
    public string GetSelectedName() => buildingPrefabs.Count > 0
        ? buildingPrefabs[selectedIndex].name : "None";
}