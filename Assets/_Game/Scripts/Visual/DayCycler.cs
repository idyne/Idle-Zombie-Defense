using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FateGames;
using static LevelManager;
public class DayCycler : MonoBehaviour
{
    private static DayCycler instance = null;
    public static DayCycler Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DayCycler>();
            return instance;
        }
    }
    [SerializeField] private Animator lightAnimator;
    [SerializeField] private Color morningFogColor, noonFogColor, eveningFogColor, nightFogColor;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Color[] directionalLightColors;
    private float fogStartDistance = 30;
    private float fogEndDistance = 45;
    private float fogOffset = 0;

    private void Update()
    {
        RenderSettings.fogStartDistance = fogStartDistance + fogOffset;
        RenderSettings.fogEndDistance = fogEndDistance + fogOffset;
    }

    public void SetTimePeriodWithoutAnimation(TimePeriod period)
    {
        lightAnimator.enabled = false;
        switch (period)
        {
            case TimePeriod.Morning:
                directionalLight.intensity = 0.9f;
                directionalLight.transform.rotation = Quaternion.Euler(380, -30, 0);
                directionalLight.color = directionalLightColors[0];
                ChangeFogColor(morningFogColor, false);
                ChangeFogDistance(30, 55, false);
                break;
            case TimePeriod.Noon:
                directionalLight.intensity = 1f;
                directionalLight.transform.rotation = Quaternion.Euler(80, -30, 0);
                directionalLight.color = directionalLightColors[1];
                ChangeFogColor(noonFogColor, false);
                ChangeFogDistance(35, 60, false);
                break;
            case TimePeriod.Evening:
                directionalLight.intensity = 0.8f;
                directionalLight.transform.rotation = Quaternion.Euler(157.4f, -30, 0);
                directionalLight.color = directionalLightColors[2];
                ChangeFogColor(eveningFogColor, false);
                ChangeFogDistance(30, 45, false);
                break;
            case TimePeriod.Night:
                directionalLight.transform.rotation = Quaternion.Euler(234.91f, -30, 0);
                directionalLight.color = directionalLightColors[3];
                directionalLight.intensity = 0.7f;
                ChangeFogColor(nightFogColor, false);
                ChangeFogDistance(25, 45, false);
                break;
            default:
                break;
        }
    }

    public IEnumerator SetTimePeriod(TimePeriod period)
    {
        lightAnimator.enabled = true;
        switch (period)
        {
            case TimePeriod.Morning:
                lightAnimator.SetTrigger("Morning");
                ChangeFogColor(morningFogColor);
                ChangeFogDistance(30, 55);
                break;
            case TimePeriod.Noon:
                lightAnimator.SetTrigger("Noon");
                ChangeFogColor(noonFogColor);
                ChangeFogDistance(35, 60);
                break;
            case TimePeriod.Evening:
                lightAnimator.SetTrigger("Evening");
                ChangeFogColor(eveningFogColor);
                ChangeFogDistance(30, 45);
                break;
            case TimePeriod.Night:
                lightAnimator.SetTrigger("Night");
                ChangeFogColor(nightFogColor);
                ChangeFogDistance(25, 45);
                break;
            default:
                break;
        }
        SoundFX.PlaySound("Tic Tac Sound");
        yield return new WaitForSeconds(2);
    }

    private void ChangeFogColor(Color color, bool animate = true)
    {
        if (animate)
        {
            float previousVal = 0;
            DOTween.To((val) =>
            {
                float deltaTime = val - previousVal;
                RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, color, deltaTime);
                previousVal = val;
            }, 0, 2, 2);
        }
        else
        {
            RenderSettings.fogColor = color;
        }
    }

    private void ChangeFogDistance(float start, float end, bool animate = true)
    {
        if (animate)
        {
            DOTween.To(() => fogStartDistance, x => fogStartDistance = x, start + fogOffset, 2);
            DOTween.To(() => fogEndDistance, x => fogEndDistance = x, end + fogOffset, 2);
        }
        else
        {
            fogStartDistance = start;
            fogEndDistance = end;
        }
    }

    public void ChangeFogOffset(float offset)
    {
        fogOffset = offset;
    }


}
