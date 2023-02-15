using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;

public class Airstrike : MonoBehaviour
{
    [SerializeField] private Transform bombPointContainer;
    [SerializeField] private Animator animator;
    private List<Transform> bombPoints = new();
    [SerializeField] private Image cooldownLayerImage;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite bombSprite, napalmSprite;
    private float cooldown = 30;
    private float remainingCooldown = 0;

    private void Awake()
    {
        for (int i = 0; i < bombPointContainer.childCount; i++)
            bombPoints.Add(bombPointContainer.GetChild(i));
        bombPoints.Sort((a, b) => Mathf.CeilToInt(b.position.x - a.position.x));
        WaveController.Instance.OnWaveStart.AddListener(() =>
        {
            if (PlayerProgression.PlayerData.AirstrikeLevel >= 1)
            {
                ResetCooldown();
                ShowButton();
            }
        });
        WaveController.Instance.OnWaveEnd.AddListener(() => { HideButton(); });
        PauseButton.OnPause.AddListener(() => { if (WaveController.State == WaveController.WaveState.RUNNING) HideButton(); });
        PauseButton.OnResume.AddListener(() => { if (WaveController.State == WaveController.WaveState.RUNNING && PlayerProgression.PlayerData.AirstrikeLevel >= 1) ShowButton(); });
    }

    private void Start()
    {
        HideButton();
    }

    private void Update()
    {
        cooldownLayerImage.fillAmount = Mathf.Clamp(remainingCooldown / cooldown, 0, 1);
        remainingCooldown -= Time.deltaTime;
    }

    private void HideButton() => button.gameObject.SetActive(false);
    private void ShowButton()
    {
        switch (PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel)
        {
            case 1:
                buttonImage.sprite = bombSprite;
                break;
            case 2:
                buttonImage.sprite = napalmSprite;
                break;
        }
        button.gameObject.SetActive(true);
    }
    private void ResetCooldown() => remainingCooldown = 0;

    public void Call()
    {
        if (remainingCooldown > 0) return;
        animator.SetTrigger("Call");
        remainingCooldown = cooldown;
        StartCoroutine(Bomb());
    }

    private IEnumerator Bomb(int index = 0, float delay = 0.05f)
    {
        if (index < bombPoints.Count)
        {
            DropBomb(bombPoints[index].position);
            yield return new WaitForSeconds(delay);
            yield return Bomb(index + 1, delay);
        }
    }

    private void DropBomb(Vector3 to)
    {
        string bombTag;
        switch (PlayerProgression.PlayerData.AirstrikeLevel)
        {
            case 1:
                bombTag = "Airstrike Bomb";
                break;
            case 2:
                bombTag = "Airstrike Napalm";
                break;
            default:
                bombTag = "Airstrike Napalm";
                break;
        }
        ObjectPooler.SpawnFromPool(bombTag, to, Quaternion.identity);
    }
}
