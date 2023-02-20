using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FateGames;

public class Placeable : MonoBehaviour
{
    [SerializeField] private Grid initialGrid;
    [SerializeField] private string soundTag;
    public virtual bool CanSelect { get; protected set; } = true;
    public UnityEvent OnSelect { get; private set; } = new();
    public UnityEvent OnDrop { get; private set; } = new();

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
        if (grid == null) { gameObject.SetActive(false); return; };
        transform.position = grid.transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void Attach(Grid grid, bool sound = true, bool init = false)
    {
        if (!grid) return;
        Detach();
        this.grid = grid;
        grid.SetPlaceable(this);
        PlaceOnGrid();
        if (sound && !init) SoundFX.PlaySound(soundTag, transform.position);
    }

    public void Detach()
    {
        if (!grid) return;
        grid.SetPlaceable(null);
        grid = null;
    }
}
