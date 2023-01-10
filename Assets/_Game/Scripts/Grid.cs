using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color availableColor, notAvailableColor, hoverColor, fromColor;
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
    public Placeable AttachedPlaceable { get; private set; }

    public bool IsAvaliable { get => !AttachedPlaceable; }

    public void SetPlaceable(Placeable placeable)
    {
        AttachedPlaceable = placeable;
    }

    public void Show(Placeable placeable = null)
    {
        if (!image.enabled) image.enabled = true;
        if (placeable != null && placeable == AttachedPlaceable)
            image.color = fromColor;
        else
            image.color = IsAvaliable ? availableColor : notAvailableColor;
    }

    public void Hover()
    {
        if (!image.enabled) image.enabled = true;
        image.color = hoverColor;
    }
    public void Hide()
    {
        image.enabled = false;
    }
}
