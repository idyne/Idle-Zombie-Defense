using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using FateGames;
using UnityEngine.UI;

public class ThrowableWeaponsGuy : MonoBehaviour
{
    [SerializeField] private LayerMask zombieLayerMask;
    [SerializeField] private Animator animator;
    private bool usingSkill = false;
    private bool throwing = false;
    private Camera _mainCamera = null;
    private Camera mainCamera { get { if (_mainCamera == null) _mainCamera = Camera.main; return _mainCamera; } }
    private float shootingPeriod = 0.5f;
    private float lastShootTime = -50;
    private Zombie target = null;
    private Weapon weapon;
    [SerializeField] private float range = 20;
    [SerializeField] private Transform rightHand;
    private Transform _transform = null;
    private ThrowableWeapon throwableWeapon = null;
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;
    private float cooldown = 30;
    private float remainingCooldown = 0;
    [SerializeField] private Image cooldownLayerImage;
    [SerializeField] private GameObject ragdoll;
    [SerializeField] private GameObject skillActiveEffect;
    [SerializeField] private Sprite grenadeSprite, molotovSprite;
    private Vector3 grenadePoint;
    [SerializeField] private TargetIndicator targetIndicator;

    private void ResetCooldown() => remainingCooldown = 0;

    public Transform transform
    {
        get
        {
            if (_transform == null)
                _transform = base.transform;
            return _transform;
        }
    }
    private void Awake()
    {
        weapon = GetComponentInChildren<Weapon>();
        WaveController.Instance.OnWaveStart.AddListener(() =>
        {
            if (PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel >= 1)
            {
                DeactivateSkill();
                ResetCooldown();
                ShowButton();
            }
        });
        WaveController.Instance.OnWaveEnd.AddListener(() => { HideButton(); });
    }

    private void Start()
    {
        HideButton();
    }

    private void Update()
    {
        if (target)
        {
            Vector3 direction = target.Transform.position - transform.position;
            direction.y = 0;
            Vector3 newDir = Vector3.MoveTowards(transform.forward, direction, Time.deltaTime * 8);
            transform.rotation = Quaternion.LookRotation(newDir);
        }

        if (Input.GetMouseButtonDown(0) && !(EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject != null))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            // Perform the raycast for 'Trap' layermask
            if (Physics.Raycast(ray, out RaycastHit hit, 100, zombieLayerMask))
            {
                Zombie zombie = hit.transform.GetComponent<Zombie>();
                SetTarget(zombie);
            }
        }
        cooldownLayerImage.fillAmount = Mathf.Clamp(remainingCooldown / cooldown, 0, 1);
        remainingCooldown -= Time.deltaTime;
    }

    private void HideButton() => button.gameObject.SetActive(false);
    private void ShowButton()
    {
        switch (PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel)
        {
            case 1:
                buttonImage.sprite = grenadeSprite;
                break;
            case 2:
                buttonImage.sprite = molotovSprite;
                break;
        }

        button.gameObject.SetActive(true);
    }

    public void SetTarget(Zombie zombie)
    {
        if (WaveController.Instance.CurrentWave == null || WaveController.State != WaveController.WaveState.RUNNING) return;
        if (zombie == null || Vector3.Distance(zombie.Transform.position, transform.position) > range) return;
        if (zombie == target) return;
        if (target)
            RemoveTarget();
        target = zombie;
        zombie.OnDeath.AddListener(OnTargetDeath);
        if (lastShootTime + shootingPeriod > Time.time)
            DOVirtual.DelayedCall(lastShootTime + shootingPeriod - Time.time, StartShooting);
        else
            StartShooting();
        targetIndicator.SetTarget(target);
    }

    public void ActivateRagdoll()
    {
        if (ragdoll)
        {
            ragdoll.transform.SetParent(null);
            ragdoll.SetActive(true);
            Rigidbody[] rigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in rigidbodies)
                rb.AddExplosionForce(5, Vector3.zero, 10, 1, ForceMode.Impulse);
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        RemoveTarget();
        DOTween.Kill(transform);
        CancelInvoke();
    }

    private void OnTargetDeath()
    {
        grenadePoint = target.Transform.position;
        RemoveTarget();
    }

    private void StartShooting()
    {
        if (!target) return;
        CancelInvoke(nameof(Shoot));
        InvokeRepeating(nameof(Shoot), 0, shootingPeriod);

    }
    private void StopShooting()
    {
        CancelInvoke(nameof(Shoot));
        animator.SetTrigger("Idle");
    }

    private void Shoot()
    {
        if (throwing) return;
        lastShootTime = Time.time;
        if (usingSkill)
        {
            ThrowWeapon();
        }
        else
        {
            animator.SetTrigger("Shoot");
            weapon.Shoot(target);
        }
    }

    private void ThrowWeapon()
    {
        DeactivateSkill();
        animator.SetTrigger("Throw");
        throwing = true;
        remainingCooldown = cooldown;
    }
    private void RemoveTarget()
    {
        target?.OnDeath.RemoveListener(OnTargetDeath);
        target = null;
        targetIndicator.Hide();
        if (!throwing)
        {
            StopShooting();
        }
    }

    public void HandleSkillButton()
    {
        if (remainingCooldown > 0) return;
        if (usingSkill) DeactivateSkill();
        else ActivateSkill();

    }

    public void ActivateSkill()
    {
        usingSkill = true;
        skillActiveEffect.SetActive(true);
    }
    public void DeactivateSkill()
    {
        usingSkill = false;
        skillActiveEffect.SetActive(false);
    }

    public void GetGrenadeToHand()
    {

        string weaponTag;
        switch (PlayerProgression.PlayerData.ThrowableWeaponsGuyLevel)
        {
            case 1:
                weaponTag = "Grenade";
                break;
            case 2:
                weaponTag = "Molotov";
                break;
            default:
                weaponTag = "Molotov";
                break;
        }
        throwableWeapon = ObjectPooler.SpawnFromPool(weaponTag, rightHand.position, rightHand.rotation).GetComponent<ThrowableWeapon>();
        throwableWeapon.transform.SetParent(rightHand);
    }

    public void DropGrenadeFromHand()
    {
        throwableWeapon.transform.SetParent(null);
        throwableWeapon.Throw(!target ? grenadePoint : target.Transform.position);
        throwing = false;
        if (!target)
        {
            StopShooting();
        }
    }
}
