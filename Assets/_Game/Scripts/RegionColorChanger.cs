using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using FateGames;

public class RegionColorChanger : MonoBehaviour
{
    [SerializeField] private Transform zone1Container, zone2Container, zone3Container, zone4Container;
    private List<Image> zone1Regions = new(), zone2Regions = new(), zone3Regions = new(), zone4Regions = new();

    private void Awake()
    {
        DoIt(zone1Container, zone1Regions);
        DoIt(zone2Container, zone2Regions);
        DoIt(zone3Container, zone3Regions);
        DoIt(zone4Container, zone4Regions);
    }

    private void DoIt(Transform container, List<Image> imageList)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Image image = container.GetChild(i).GetComponent<Image>();
            imageList.Add(image);
            image.gameObject.SetActive(false);
        }
    }

    public IEnumerator AnimateRegion(int zoneLevel)
    {
        List<Image> images = new();
        switch (zoneLevel)
        {
            case 1:
                images = zone1Regions;
                break;
            case 2:
                images = zone2Regions;
                break;
            case 3:
                images = zone3Regions;
                break;
            case 4:
                images = zone4Regions;
                break;

        }
        yield return AnimateReg(images);

    }

    private IEnumerator AnimateReg(List<Image> images, int index = 0)
    {
        if (index < images.Count)
        {
            float delay = 2f / images.Count;
            Image image = images[index];
            image.gameObject.SetActive(true);
            Color color = Color.green;
            color.SetValue(Random.Range(0.4f, 0.7f));
            image.DOColor(color, delay);
            yield return new WaitForSeconds(delay);
            yield return AnimateReg(images, index + 1);
        }

    }
}
