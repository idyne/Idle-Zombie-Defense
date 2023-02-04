using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelManager;
public class MapController : MonoBehaviour
{
    [SerializeField] private Animator[] routes;
    [SerializeField] private float[] xPositions;
    [SerializeField] private GameObject[] inits;

    private RectTransform Transform;

    /*private int last = 0;*/
    private void Awake()
    {
        Transform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        InitLastPath(ZoneLevel - 2);
    }

    /*void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FillPath(last);
            last++;
        }
    }*/
    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
    public void InitLastPath(int lastPathIndex)
    {
        if (lastPathIndex < 0) return;
        for (int i = 0; i < lastPathIndex; i++)
        {
            inits[i].SetActive(true);
        }
    }

    public void FillPath(int pathIndex)
    {
        if (pathIndex < 0) return;
        routes[pathIndex].SetTrigger("Go");
    }

    public void GoPosition(int pathIndex)
    {
        Transform.localPosition = new Vector3(xPositions[pathIndex], 0);
    }
}
