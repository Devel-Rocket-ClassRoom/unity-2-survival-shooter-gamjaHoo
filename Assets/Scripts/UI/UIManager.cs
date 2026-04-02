using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WeaponController;

public class UIManager : MonoBehaviour
{
    [Header("HP")]
    public Slider healthSlider;
    public Image healthFill;
    public TMP_Text healthText;

    [Header("Score")]
    public TMP_Text scoreText;

    [Header("Ammo")]
    public TMP_Text ammoMagText;
    public TMP_Text ammoStockText;
    public GameObject reloadingPanel;
    public Slider reloadSlider;

    [Header("Weapon Slots")]
    public WeaponSlotUI[] weaponSlotUIs;  // 슬롯 4개

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    private PlayerHealth playerHealth;
    private PlayerInventory inventory;
    private WeaponController weaponController;
    private Gun currentGun;

    // HP 색상 변화
    private static readonly Color colorFull = new Color(0.22f, 0.85f, 0.45f, 1f);
    private static readonly Color colorMid = new Color(0.95f, 0.75f, 0.10f, 1f);
    private static readonly Color colorLow = new Color(0.88f, 0.20f, 0.20f, 1f);

    void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
        inventory = FindObjectOfType<PlayerInventory>();
        weaponController = FindObjectOfType<WeaponController>();

        healthSlider.maxValue = playerHealth.startingHealth;
        healthSlider.value = playerHealth.startingHealth;
        UpdateHealthColor(playerHealth.startingHealth, playerHealth.startingHealth);

        scoreText.text = "0";
        gameOverPanel.SetActive(false);
        reloadingPanel?.SetActive(false);

        InitWeaponSlots();

        // 이벤트 구독
        playerHealth.OnHealthChanged += UpdateHealth;
        playerHealth.OnDead += ShowGameOver;
        ScoreManager.instance.OnScoreChanged += UpdateScore;
        inventory.OnInventoryChanged += UpdateWeaponSlots;
        weaponController.OnWeaponChanged += OnWeaponChanged;

        Gun currentGunOnStart = weaponController.GetCurrentGun();
        if (currentGunOnStart != null)
            OnWeaponChanged(currentGunOnStart);
    }

    // ── HP ──────────────────────────────────────

    void UpdateHealth(float current, float max)
    {
        healthSlider.value = current;
        if (healthText != null)
            healthText.text = $"{(int)current}";
        UpdateHealthColor(current, max);
    }

    void UpdateHealthColor(float current, float max)
    {
        if (healthFill == null) return;
        float ratio = current / max;
        healthFill.color = ratio > 0.5f ? colorFull
                         : ratio > 0.25f ? colorMid
                         : colorLow;
    }

    // ── Score ────────────────────────────────────

    void UpdateScore(int score)
    {
        scoreText.text = score.ToString("N0");
    }

    // ── Ammo ─────────────────────────────────────

    void OnWeaponChanged(Gun gun)
    {
        // 이전 무기 이벤트 해제
        if (currentGun != null)
        {
            currentGun.OnAmmoChanged -= UpdateAmmo;
            currentGun.OnReloadStart -= OnReloadStart;
            currentGun.OnReloadEnd -= OnReloadEnd;
        }

        currentGun = gun;

        if (currentGun != null)
        {
            currentGun.OnAmmoChanged += UpdateAmmo;
            currentGun.OnReloadStart += OnReloadStart;
            currentGun.OnReloadEnd += OnReloadEnd;

            UpdateAmmo(currentGun.currentAmmoInMag, currentGun.currentAmmoStock);
        }
    }

    void UpdateAmmo(int inMag, int stock)
    {
        ammoMagText.text = inMag.ToString("D2");
        ammoStockText.text = stock.ToString();

        // 탄약 부족 시 빨간색
        ammoMagText.color = inMag <= 5
            ? colorLow
            : Color.white;
    }

    void OnReloadStart()
    {
        reloadingPanel?.SetActive(true);
        StartCoroutine(AnimateReloadBar());
    }

    void OnReloadEnd()
    {
        reloadingPanel?.SetActive(false);
        if (reloadSlider != null) reloadSlider.value = 0f;
    }

    System.Collections.IEnumerator AnimateReloadBar()
    {
        if (reloadSlider == null || currentGun == null) yield break;
        float duration = currentGun.data.reloadTime;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            reloadSlider.value = elapsed / duration;
            yield return null;
        }
    }

    // ── Weapon Slots ──────────────────────────────

    void InitWeaponSlots()
    {
        for (int i = 0; i < weaponSlotUIs.Length; i++)
            weaponSlotUIs[i].Init(i + 1);  // 키 번호 1~4
    }

    void UpdateWeaponSlots(WeaponData[] slots, int activeIndex)
    {
        for (int i = 0; i < weaponSlotUIs.Length; i++)
        {
            if (i < slots.Length)
                weaponSlotUIs[i].SetWeapon(slots[i], i == activeIndex);
            else
                weaponSlotUIs[i].SetEmpty();
        }
    }

    // ── Game Over ─────────────────────────────────

    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        if (finalScoreText != null)
            finalScoreText.text = $"{ScoreManager.instance.score:N0}";
    }

    void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealth;
            playerHealth.OnDead -= ShowGameOver;
        }
        if (ScoreManager.instance != null)
            ScoreManager.instance.OnScoreChanged -= UpdateScore;
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateWeaponSlots;
        if (weaponController != null)
            weaponController.OnWeaponChanged -= OnWeaponChanged;
        if (currentGun != null)
        {
            currentGun.OnAmmoChanged -= UpdateAmmo;
            currentGun.OnReloadStart -= OnReloadStart;
            currentGun.OnReloadEnd -= OnReloadEnd;
        }
    }
}