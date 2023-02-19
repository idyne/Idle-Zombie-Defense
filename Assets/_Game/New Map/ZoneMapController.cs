using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ZoneMapController : MonoBehaviour
{
    [SerializeField] private Animator[] pathAnimationControllers = null;
    [SerializeField] private Zone[] zones = null;
    [SerializeField] private ZombieIcon[] zombieIcons = null;
    [SerializeField] private Transform map = null;
    [SerializeField] private GameObject canvas = null;
    [SerializeField] private GameObject nextButton = null;

    /*private int a = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Open();
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            PlayPath(a);
            a++;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            BeginingShow();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            FindNextPathOnWorld(a);
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            SetupZonesAndPaths(3);
        }
    }*/

    public void Open()
    {
        map.localScale = Vector3.one * 0.25f;
        map.localPosition = Vector3.zero;
        canvas.SetActive(true);
        nextButton.SetActive(false);
    }

    public void BeginingShow()
    {
        FocusZone(2);
        DOVirtual.DelayedCall(3f, () => FocusZone(0));
        DOVirtual.DelayedCall(5f, () => nextButton.SetActive(true));
    }

    public void FindNextPathOnWorld(int lastAchivedZoneIndex)
    {
        if (lastAchivedZoneIndex < zones.Length - 1)
        {
            Vector3 target = (zones[lastAchivedZoneIndex].transform.localPosition + zones[lastAchivedZoneIndex + 1].transform.localPosition) / 2;
            map.DOKill();
            map.DOLocalMove(-target, 1f);
            map.DOScale(Vector3.one, 1f);
        }
    }

    public void PlayPath(int fromIndex)
    {
        zones[fromIndex].Hop();
        zones[fromIndex].Wave();
        zombieIcons[fromIndex].Fade();
        DOVirtual.DelayedCall(1f, () => zones[fromIndex].CleanSign());
        DOVirtual.DelayedCall(3f, () => nextButton.SetActive(true));

        if (fromIndex != zones.Length - 1)
        {
            DrawPath(fromIndex);
            DOVirtual.DelayedCall(2.5f, () => zones[fromIndex + 1].Hop()); // 3 yerine 2.5, neden bilmiyorum
        }
    }

    public void SetupZonesAndPaths(int lastAchivedZoneIndex)
    {
        for (int i = 0; i < lastAchivedZoneIndex; i++)
        {
            InstantDrawPath(i);
            zones[i].InstantCleanSign();
            zombieIcons[i].InstantFade();
        }
    }

    public void Close()
    {
        canvas.SetActive(false);
    }





    private void FocusZone(int index)
    {
        if (index < zones.Length)
        {
            map.DOKill();
            map.DOLocalMove(-zones[index].transform.localPosition, 2f);
            map.DOScale(Vector3.one, 2f);
        }
    }

    private void DrawPath(int index)
    {
        pathAnimationControllers[index].SetTrigger("Go");
    }

    private void InstantDrawPath(int index)
    {
        pathAnimationControllers[index].SetTrigger("Instant");
    }

}
