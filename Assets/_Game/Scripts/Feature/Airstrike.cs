using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FateGames;

public class Airstrike : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private List<Transform> bombPoints { get => TowerController.Instance.GetCurrentTower().BombPoints; }
    [SerializeField] private Image cooldownLayerImage;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite bombSprite, napalmSprite;
    private float cooldown { get => Settings.AirstrikeCooldown; }
    private float remainingCooldown = 0;

    private void Awake()
    {
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
        switch (PlayerProgression.PlayerData.AirstrikeLevel)
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
        List<Transform> points = bombPoints;
        if (index < points.Count)
        {
            DropBomb(points[index]);
            yield return new WaitForSeconds(delay);
            yield return Bomb(index + 1, delay);
        }
    }

    private void DropBomb(Transform point)
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
        ObjectPooler.SpawnFromPool(bombTag, point.position, point.rotation);
    }
}
