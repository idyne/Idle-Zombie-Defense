using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Placeable : MonoBehaviour
{
    [SerializeField] private Grid initialGrid;

    private void Start()
    {
        Attach(initialGrid);
    }

    private Transform _transform = null;
    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }
    public Grid grid { get; private set; } = null;

    public void SetGrid(Grid grid)
    {
        this.grid = grid;
    }

    public void PlaceOnGrid()
    {
        if (grid == null) return;
        transform.position = grid.transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Attach(Grid grid)
    {
        if (!grid) return;
        Detach();
        this.grid = grid;
        grid.SetPlaceable(this);
        PlaceOnGrid();
    }

    public void Detach()
    {
        if (!grid) return;
        grid.SetPlaceable(null);
        grid = null;
    }
}
