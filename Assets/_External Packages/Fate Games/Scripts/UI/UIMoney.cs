using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace FateGames
{
    public class UIMoney : MonoBehaviour
    {
        private Transform _transform = null;
        private bool animating = false;
        public Transform Transform
        {
            get
            {
                if (_transform == null)
                    _transform = transform;
                return _transform;
            }
        }
        [SerializeField] private TextMeshProUGUI moneyText;

        private void Awake()
        {
            PlayerProgression.OnMoneyChanged.AddListener(SetMoney);
        }

        private void SetMoney(int money)
        {
            moneyText.text = FormatMoney(PlayerProgression.MONEY);
            if (animating || money <= 0) return;
            animating = true;
            Transform.DOScale(1.1f, 0.05f).SetLoops(2, LoopType.Yoyo).OnComplete(() => { animating = false; });
        }



        public static string FormatMoney(int money)
        {
            string text;
            if (money >= 1000000 && money - (money / 1000000) * 1000000 >= 10000) text = (money / 1000000f).ToString("0.00").Replace(',', '.') + "M";
            else if (money >= 1000000) text = (money / 1000000f).ToString("0").Replace(',', '.') + "M";
            else if (money >= 10000 && money - (money / 10000) * 10000 >= 100) text = (money / 1000f).ToString("0.0").Replace(',', '.') + "K";
            else if (money >= 10000) text = (money / 1000f).ToString("0").Replace(',', '.') + "K";
            else if (money >= 1000 && money - (money / 1000) * 1000 >= 10) text = (money / 1000f).ToString("0.00").Replace(',', '.') + "K";
            else if (money >= 1000) text = (money / 1000f).ToString("0").Replace(',', '.') + "K";
            else text = money.ToString();
            return text;
        }


    }

}
