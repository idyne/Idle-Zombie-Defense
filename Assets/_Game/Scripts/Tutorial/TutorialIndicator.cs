using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIndicator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void Show()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("Play");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
