using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom editor for BuildablePrefab.
/// Snaps the object to the GridManager cell size whenever you move it
/// in the Scene view. Also draws the occupied cells as gizmos.
///
/// SETUP:
///   1. Place this file inside any folder named 'Editor' in your project
///      (e.g. Assets/Editor/BuildablePrefabEditor.cs).
///   2. That's it — any GameObject with a BuildablePrefab component will
///      auto-snap when dragged in the Scene view.
/// </summary>
[CustomEditor(typeof(BuildablePrefab))]
public class BuildablePrefabEditor : Editor
{
    // Grab cell size from GridManager if present, otherwise fall back to this
    private const float FallbackCellSize = 1f;

    private BuildablePrefab bp;

    void OnEnable()
    {
        bp = (BuildablePrefab)target;
    }

    // ── Scene GUI ──────────────────────────────────────────────────────────
    void OnSceneGUI()
    {
        // Only act when the user is actively moving the object
        if (Event.current.type == EventType.MouseDrag ||
            Event.current.type == EventType.MouseUp)
        {
            SnapToGrid();
        }

        DrawCellGizmos();
    }

    // ── Snapping ───────────────────────────────────────────────────────────
    private void SnapToGrid()
    {
        float cell = GetCellSize();
        Transform t = bp.transform;

        Vector3 pos = t.position;
        pos.x = Mathf.Round(pos.x / cell) * cell;
        pos.y = Mathf.Round(pos.y / cell) * cell;
        pos.z = Mathf.Round(pos.z / cell) * cell;

        if (t.position != pos)
        {
            Undo.RecordObject(t, "Snap to Grid");
            t.position = pos;
        }
    }

    // ── Cell Gizmos ────────────────────────────────────────────────────────
    private void DrawCellGizmos()
    {
        float cell = GetCellSize();
        Transform t = bp.transform;

        // Root cell from current snapped position
        Vector3Int rootCell = new Vector3Int(
            Mathf.RoundToInt(t.position.x / cell),
            Mathf.RoundToInt(t.position.y / cell),
            Mathf.RoundToInt(t.position.z / cell)
        );

        foreach (var offset in bp.occupiedCellOffsets)
        {
            Vector3Int worldCell = rootCell + offset;
            Vector3 center = new Vector3(
                worldCell.x * cell,
                worldCell.y * cell,
                worldCell.z * cell
            );

            // Green fill
            Handles.color = new Color(0f, 1f, 0.4f, 0.12f);
            Handles.DrawSolidRectangleWithOutline(
                GetCellCorners(center, cell),
                new Color(0f, 1f, 0.4f, 0.12f),
                new Color(0f, 1f, 0.4f, 0.8f)
            );

            // Label each cell offset
            Handles.color = Color.white;
            Handles.Label(center + Vector3.up * (cell * 0.5f),
                $"({offset.x},{offset.y},{offset.z})",
                EditorStyles.miniLabel);
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    /// <summary>Returns the four corners of a flat (XZ-plane) cell quad.</summary>
    private Vector3[] GetCellCorners(Vector3 center, float cell)
    {
        float h = cell * 0.5f;
        return new Vector3[]
        {
            center + new Vector3(-h, 0,  h),
            center + new Vector3( h, 0,  h),
            center + new Vector3( h, 0, -h),
            center + new Vector3(-h, 0, -h),
        };
    }

    /// <summary>
    /// Reads cell size from GridManager in the scene if available,
    /// otherwise uses the fallback constant.
    /// </summary>
    private float GetCellSize()
    {
        // FindFirstObjectByType is the non-deprecated Unity 2023+ API;
        // falls back to FindObjectOfType for older versions.
#if UNITY_2023_1_OR_NEWER
        var gm = FindFirstObjectByType<GridManager>();
#else
        var gm = FindObjectOfType<GridManager>();
#endif
        return gm != null ? gm.cellSize : FallbackCellSize;
    }

    // ── Inspector ──────────────────────────────────────────────────────────
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "This object snaps to the grid automatically while dragging in the Scene view.\n" +
            "Green overlays show occupied cells based on 'Occupied Cell Offsets'.",
            MessageType.Info);

        if (GUILayout.Button("Snap to Grid Now"))
        {
            SnapToGrid();
            SceneView.RepaintAll();
        }
    }
}