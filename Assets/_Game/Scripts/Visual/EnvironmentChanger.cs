using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentChanger : MonoBehaviour
{
    [SerializeField] private GameObject swamp, forest, desert, city;

    public void SetZone(int zoneLevel)
    {
        swamp.SetActive(false);
        forest.SetActive(false);
        desert.SetActive(false);
        city.SetActive(false);
        switch (zoneLevel)
        {
            case 1:
                swamp.SetActive(true);
                break;
            case 2:
                forest.SetActive(true);
                break;
            case 3:
                desert.SetActive(true);
                break;
            case 4:
                city.SetActive(true);
                break;
            default:
                break;
        }
    }
}
