using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Map
{
    public class ZoneMapController : MonoBehaviour
    {
        [SerializeField] private Animator[] pathAnimationControllers = null;
        [SerializeField] private Zone[] zones = null;
        [SerializeField] private Transform[] zombieNotesMasks = null;
        [SerializeField] private Transform map = null;
        [SerializeField] private GameObject canvas = null;
        [SerializeField] private GameObject nextButton = null;
        public bool Closed { get; private set; } = true;

        private int a = 0;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                CleanMap();
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
        }

        public void CleanMap()
        {
            map.localScale = Vector3.one * 0.25f;
            map.localPosition = Vector3.zero;
            canvas.SetActive(true);
            nextButton.SetActive(false);
            Closed = false;
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
                Vector3 target = (zones[lastAchivedZoneIndex].Root.parent.localPosition + zones[lastAchivedZoneIndex + 1].Root.parent.localPosition) / 2;
                map.DOKill();
                map.DOLocalMove(-target, 1f);
                map.DOScale(Vector3.one, 1f);
            }
        }

        public void SetupZonesAndPaths(int lastAchivedZoneIndex)
        {
            for (int i = 0; i < lastAchivedZoneIndex; i++)
            {
                InstantDrawPath(i);
                InstantClearZoneEffect(i);
            }
        }

        public void PlayPath(int fromIndex)
        {
            HopZone(fromIndex);
            ClearZoneEffect(fromIndex);
            DOVirtual.DelayedCall(3f, () => nextButton.SetActive(true));

            if (fromIndex != zones.Length - 1)
            {
                DrawPath(fromIndex);
                DOVirtual.DelayedCall(2.5f, () => HopZone(fromIndex + 1)); // 3 yerine 2.5, neden bilmiyorum
            }
        }

        public void Close()
        {
            canvas.SetActive(false);
            Closed = true;
        }





        private void FocusZone(int index)
        {
            if (index < zones.Length)
            {
                map.DOKill();
                map.DOLocalMove(-zones[index].Root.parent.localPosition, 2f);
                map.DOScale(Vector3.one, 2f);
            }
        }

        private void InstantClearZoneEffect(int index)
        {
            zones[index].CleanSign.localScale = Vector3.one;
            Color tempColor = zones[index].CleanSign.GetComponent<Image>().color;
            tempColor.a = 1;
            zones[index].CleanSign.GetComponent<Image>().color = tempColor;
            zones[index].CleanSign.GetChild(0).GetComponent<Image>().color = tempColor;
            zombieNotesMasks[index].localScale = Vector3.one;
        }

        private void ClearZoneEffect(int index)
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                zones[index].CleanSign.DOScale(Vector3.one, 0.5f);
                zones[index].CleanSign.GetComponent<Image>().DOFade(1, 0.5f);
                zones[index].CleanSign.GetChild(0).GetComponent<Image>().DOFade(1, 0.5f);
            });


            zones[index].CleaningWave.DOScale(Vector3.one * 5, 1f);
            zones[index].CleaningWave.GetComponent<Image>().DOFade(0, 1f);

            zombieNotesMasks[index].DOScale(Vector3.one, 1f);
        }

        private void HopZone(int index)
        {
            zones[index].Root.DOKill(true);
            zones[index].Root.DOScale(Vector3.one * 1.5f, 0.2f).SetLoops(2, LoopType.Yoyo);

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

}
