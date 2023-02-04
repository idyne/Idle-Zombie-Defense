using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static LevelManager;
public class TimePeriodAnimation : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TMP_FontAsset originalFont, creepyFont;
    [SerializeField] private Material originalfontMaterial, creepyFontMaterial;
    [SerializeField] private VertexGradient redGradient;
    float animationDuration = 1.9f;

    

    public IEnumerator SetTimePeriod(TimePeriod period)
    {
        text.rectTransform.anchoredPosition = new Vector3(0, -50, 0);
        text.enableVertexGradient = false;
        text.font = originalFont;
        text.fontMaterial = originalfontMaterial;
        switch (period)
        {
            case TimePeriod.Morning:
                text.text = "Morning";
                break;
            case TimePeriod.Noon:
                text.text = "Noon";
                break;
            case TimePeriod.Evening:
                text.text = "Evening";
                break;
            case TimePeriod.Night:
                text.font = creepyFont;
                text.fontMaterial = creepyFontMaterial;
                text.text = "Night";
                text.enableVertexGradient = true;
                break;
            default:
                break;
        }
        float speed = (period == TimePeriod.Night ? 0.5f : 1);
        anim.SetFloat("Speed", speed);
        float delay = animationDuration * (1f / speed);
        anim.SetTrigger("Period");
        yield return new WaitForSeconds(delay);
    }
}
