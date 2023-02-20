using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private LayerMask gridLayerMask, placeableLayerMask, groundLayerMask;
    [SerializeField] private Transform gridContainer;
    private List<Grid> grids = null;
    private Grid hoveredGrid = null;
    public List<Grid> Grids
    {
        get
        {
            if (grids == null)
            {
                grids = new();
                for (int i = 0; i < gridContainer.childCount; i++)
                {
                    grids.Add(gridContainer.GetChild(i).GetComponent<Grid>());
                }
            }
            return grids;
        }
    }
    private Camera mainCamera { get => TowerController.Instance.GetCurrentTower().CameraController.Camera; }
    private Placeable selectedPlaceable = null;

    private void Update()
    {
        if (PauseButton.Paused) return;
        if (WaveController.State == WaveController.WaveState.RUNNING) return;
        if (Input.GetMouseButtonDown(0)) Select();
        if (selectedPlaceable && Input.GetMouseButton(0)) Hover();
        if (selectedPlaceable && Input.GetMouseButtonUp(0)) Place();
    }

    private void Select()
    {
        // Check if the mouse was clicked over a UI element
        if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null) return;


        // A ray from camera to mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // Perform the raycast for 'Placeable' layermask
        if (Physics.Raycast(ray, out RaycastHit hit, 100, placeableLayerMask))
        {
            // Transform of the hit object
            Transform hitTransform = hit.transform;
            // Check if the hit object has tag 'Placeable'
            if (hitTransform.CompareTag("Placeable"))
            {
                // Get the 'Placeable' component of the hit object
                Placeable placeable = hit.transform.GetComponent<Placeable>();
                Select(placeable);
            }
        }
    }

    public void Select(Placeable placeable)
    {
        if (placeable.CanSelect)
        {
            // Set selectedPlaceable to hit placeable
            selectedPlaceable = placeable;
            selectedPlaceable.OnSelect.Invoke();
            // Show grids for placement
            ShowGrids();
            // Set main camera to placement mode
            //CameraController.InPlacementMode = true;
            //UIButtonManager.Instance.HidePlacementButtons();
            //UIButtonManager.Instance.HideStartAndUpgradeButtons();
            PlacementModeController.Instance.HidePlacementButtons();
            PlacementModeController.Instance.HideExitPlacementModeButton();
        }
    }

    // Called on every frame
    private void Hover()
    {
        // A ray from camera to mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        // Perform the raycast for 'Ground' layermask
        if (Physics.Raycast(ray, out RaycastHit hit, 100, groundLayerMask))
        {
            // Move selected placeable to mouse position
            selectedPlaceable.SetPosition(hit.point);
        }
        // Perform the raycast for 'Grid' layermask
        if (Physics.Raycast(ray, out hit, 100, gridLayerMask))
        {
            // Transform of the hit object
            Transform hitTransform = hit.transform;
            // Check if the hit object has tag 'Grid'
            if (hitTransform.CompareTag("Grid"))
            {
                // Get the 'Grid' component of the hit object
                Grid grid = hit.transform.GetComponent<Grid>();
                // If hit grid is not previous hovered grid
                if (grid != hoveredGrid)
                {
                    // If there is a previous hovered grid
                    if (hoveredGrid != null)
                        hoveredGrid.Show(selectedPlaceable);
                    // Set hovered grid to the hit grid
                    hoveredGrid = grid;
                    // Hover the hovered grid
                    if (hoveredGrid.IsAvaliable)
                        hoveredGrid.Hover();
                }
            }
        }
        // If raycast do not hit a grid
        else
        {
            if (hoveredGrid)
            {
                hoveredGrid.Show(selectedPlaceable);
                hoveredGrid = null;
            }
        }
    }

    private void Place()
    {
        if (hoveredGrid && hoveredGrid.IsAvaliable)
            selectedPlaceable.Attach(hoveredGrid);
        else selectedPlaceable.PlaceOnGrid();
        selectedPlaceable.OnDrop.Invoke();
        selectedPlaceable = null;
        hoveredGrid = null;
        HideGrids();
        // Set main camera to not placement mode
        //CameraController.InPlacementMode = false;
        //UIButtonManager.Instance.ShowPlacementButtons();
        //UIButtonManager.Instance.ShowStartAndUpgradeButtons();
        PlacementModeController.Instance.ShowPlacementButtons();
        PlacementModeController.Instance.ShowExitPlacementModeButton();
    }

    private void ShowGrids()
    {
        foreach (Grid grid in Grids)
            grid.Show(selectedPlaceable);
    }

    private void HideGrids()
    {
        foreach (Grid grid in Grids)
            grid.Hide();
    }

    public Grid GetGrid(int id)
    {
        for (int i = 0; i < Grids.Count; i++)
        {
            Grid grid = Grids[i];
            if (grid.Id == id)
                return grid;
        }
        return null;
    }
}
