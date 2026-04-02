using UnityEngine;

public class PlayerHealth : LivingEntity
{
    public AudioClip deathClip;
    public AudioClip hurtClip;

    private Animator anim;
    private AudioSource audioSource;

    // UI 연동 이벤트
    public event System.Action<float, float> OnHealthChanged; // (current, max)

    public event System.Action OnDamaged;

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable(); // LivingEntity에서 currentHealth, IsDead 초기화
        OnHealthChanged?.Invoke(currentHealth, startingHealth);
    }

    // EnemyAttack에서 living.OnDamage() 로 호출됨
    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return;

        audioSource.PlayOneShot(hurtClip);

        base.OnDamage(damage, hitPoint, hitNormal); // 체력 감소 + Die 판정

        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke(currentHealth, startingHealth);
    }

    public override void Die()
    {
        if (IsDead) return;
        base.Die(); // IsDead = true, OnDead 이벤트 발생

        anim.SetTrigger("Die");
        audioSource.PlayOneShot(deathClip);
    }
}