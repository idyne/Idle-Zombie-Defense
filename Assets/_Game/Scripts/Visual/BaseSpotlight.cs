using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpotlight : MonoBehaviour
{
    [SerializeField] private GameObject halo;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Light light;
    public void TurnOn()
    {
        Debug.Log("zorn", this);
        if (meshRenderer)
        {
            Material[] materials = meshRenderer.materials;
            materials[2].color = Color.white;
            meshRenderer.materials = materials;
        }
        if (halo) halo.SetActive(true);
        light.enabled = true;
    }

    public void TurnOff()
    {
        if (meshRenderer)
        {
            Material[] materials = meshRenderer.materials;
            materials[2].color = new Color(0.1f, 0.1f, 0.1f);
            meshRenderer.materials = materials;
        }
        if (halo) halo.SetActive(false);
        light.enabled = false;
    }
}
