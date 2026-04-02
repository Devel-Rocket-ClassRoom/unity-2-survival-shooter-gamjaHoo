using UnityEngine;
using System.Collections;

public abstract class Gun : MonoBehaviour
{
    [HideInInspector] public WeaponData data;

    public int currentAmmoInMag { get; protected set; }
    public int currentAmmoStock { get; protected set; }
    public bool IsReloading { get; protected set; }

    public event System.Action<int, int> OnAmmoChanged;
    public event System.Action OnReloadStart;
    public event System.Action OnReloadEnd;

    private bool isSetup = false;

    [Header("References")]
    public Transform gunBarrelEnd;
    public ParticleSystem gunParticles;
    public ParticleSystem hitParticles;
    public LineRenderer gunLine;
    //public Light gunLight;

    protected float timer;
    protected int shootableMask;
    protected AudioSource gunAudio;
    protected PlayerInput input;

    protected const float effectsDisplayRatio = 0.2f;

    protected virtual void Awake()
    {
        gunAudio = GetComponent<AudioSource>();
        input = GetComponentInParent<PlayerInput>();
        shootableMask = LayerMask.GetMask("Shootable", "Environment");
    }

    public virtual void Setup(WeaponData weaponData)
    {
        if (isSetup) return;
        isSetup = true;

        data = weaponData;
        currentAmmoInMag = data.magazineSize;
        currentAmmoStock = data.maxAmmo - data.magazineSize;
        NotifyAmmo();
    }

    public void AddAmmo(int amount)
    {
        int totalAmmo = currentAmmoInMag + currentAmmoStock;
        int canAdd = Mathf.Max(0, data.maxAmmo - totalAmmo);
        int actualAdd = Mathf.Min(amount, canAdd);

        currentAmmoStock += actualAdd;
        NotifyAmmo();
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;

        if (CanShoot())
            Shoot();

        if (currentAmmoInMag <= 0 && !IsReloading && currentAmmoStock > 0)
            StartCoroutine(Reload());

        if (input.Reload
            && !IsReloading
            && currentAmmoInMag < data.magazineSize
            && currentAmmoStock > 0)
            StartCoroutine(Reload());

        if (timer >= data.timeBetweenBullets * effectsDisplayRatio)
            DisableEffects();
    }

    protected virtual bool CanShoot()
    {
        return input.Fire
            && !IsReloading
            && timer >= data.timeBetweenBullets
            && currentAmmoInMag > 0;
    }

    // 서브클래스에서 발사 로직 구현
    protected abstract void Shoot();

    protected void PlayShootEffects()
    {
        gunAudio.Play();
        //gunLight.enabled = true;
        gunParticles.Stop();
        gunParticles.Play();
    }

    protected void PlayHitEffect(Vector3 point)
    {
        hitParticles.transform.position = point;
        hitParticles.Stop();
        hitParticles.Play();
    }

    protected void ShowGunLine(Vector3 end)
    {
        gunLine.enabled = true;
        gunLine.SetPosition(0, gunBarrelEnd.position);
        gunLine.SetPosition(1, end);
    }

    protected void DisableEffects()
    {
        gunLine.enabled = false;
        //gunLight.enabled = false;
    }

    protected void ConsumeAmmo()
    {
        currentAmmoInMag--;
        NotifyAmmo();
    }

    protected void NotifyAmmo()
    {
        OnAmmoChanged?.Invoke(currentAmmoInMag, currentAmmoStock);
    }

    protected IEnumerator Reload()
    {
        IsReloading = true;
        OnReloadStart?.Invoke();

        yield return new WaitForSeconds(data.reloadTime);

        int needed = data.magazineSize - currentAmmoInMag;
        int actual = Mathf.Min(needed, currentAmmoStock);
        currentAmmoInMag += actual;
        currentAmmoStock -= actual;

        IsReloading = false;
        OnReloadEnd?.Invoke();
        NotifyAmmo();
    }

    // PlayerHealth 사망 연동
    private PlayerHealth playerHealth;

    protected virtual void OnEnable()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.OnDead += OnPlayerDead;
    }

    protected virtual void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDead -= OnPlayerDead;
    }

    void OnPlayerDead() => enabled = false;
}