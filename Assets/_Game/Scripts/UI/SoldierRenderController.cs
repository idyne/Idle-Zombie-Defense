using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierRenderController : MonoBehaviour
{
    public static SoldierRenderController Instance = null;

    [SerializeField] private float rotateSpeed = 10;
    [SerializeField] private GameObject[] soldiers = null;
    [SerializeField] private Transform soldierParent = null;
    [SerializeField] private Camera camera;
    [SerializeField] private RawImage image;

    private int lastRenderedSoldierIndex = 1;

    void Awake()
    {
        Instance = this;
        camera.targetTexture = new RenderTexture(512, 512, 24);
        camera.targetTexture.Create();
        RenderTexture.active = camera.targetTexture;
        image.texture = camera.targetTexture;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }


    public void Render(int soldierIndex)
    {
        EnableCamera();
        soldiers[lastRenderedSoldierIndex].SetActive(false);
        soldiers[soldierIndex].SetActive(true);
        lastRenderedSoldierIndex = soldierIndex;
    }

    public void EnableCamera()
    {
        camera.enabled = true;
    }

    public void DisableCamera()
    {
        camera.enabled = false;
    }
}
