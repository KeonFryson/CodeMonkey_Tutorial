using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Optional HUD overlay for the building system.
/// Requires Unity UI (Canvas) with:
///   - a TextMeshProUGUI for status text
///   - a TextMeshProUGUI for selected prefab name
///   - Buttons wired to the methods below
///
/// Attach this to your Canvas or a UI Manager GameObject.
/// </summary>
public class BuildingUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI selectedPrefabText;
    public GameObject buildPanel;  // Panel shown only in build mode

    void Update()
    {
        if (BuildingSystem.Instance == null) return;

        bool active = BuildingSystem.Instance.IsBuildModeActive();

        if (buildPanel != null)
            buildPanel.SetActive(active);

        if (statusText != null)
            statusText.text = active ? "BUILD MODE  [Tab] to exit" : "[Tab] to enter build mode";

        if (selectedPrefabText != null && active)
            selectedPrefabText.text = $"Selected: {BuildingSystem.Instance.GetSelectedName()}\n" +
                                      "Scroll – cycle  |  Q/E – rotate\n" +
                                      "LMB – place  |  RMB – delete";
    }

    // ── Button Callbacks ───────────────────────────────────────────────────

    public void OnToggleBuildMode()
    {
        if (BuildingSystem.Instance == null) return;
        BuildingSystem.Instance.SetBuildMode(!BuildingSystem.Instance.IsBuildModeActive());
    }

    public void OnSelectPrefab(int index)
    {
        BuildingSystem.Instance?.SelectPrefab(index);
    }
}
