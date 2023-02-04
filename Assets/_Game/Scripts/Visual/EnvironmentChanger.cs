using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;

public class EnvironmentChanger : MonoBehaviour
{
    [SerializeField] private GameObject supermarket, hospital, airport, shoppingMall, church, militaryBase;

    public void SetEnvironment()
    {
        supermarket.SetActive(false);
        hospital.SetActive(false);
        airport.SetActive(false);
        shoppingMall.SetActive(false);
        church.SetActive(false);
        militaryBase.SetActive(false);

        if (WorldLevel == 1)
        {
            switch (ZoneLevel)
            {
                case 1:
                    supermarket.SetActive(true);
                    break;
                case 2:
                    hospital.SetActive(true);
                    break;
                case 3:
                    airport.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        else if (WorldLevel == 2)
        {
            switch (ZoneLevel)
            {
                case 1:
                    shoppingMall.SetActive(true);
                    break;
                case 2:
                    church.SetActive(true);
                    break;
                case 3:
                    militaryBase.SetActive(true);
                    break;
                default:
                    break;
            }
        }
    }
}
