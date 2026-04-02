using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyHealth : LivingEntity
{
    [Header("Effects")]
    public ParticleSystem bloodEffect;
    public AudioClip hitClip;
    public AudioClip deathClip;

    [Header("Settings")]
    public float sinkSpeed = 1.5f;
    public int scoreValue;

    private Animator anim;
    private AudioSource audioSource;
    private NavMeshAgent navAgent;
    private Collider col;
    private bool isSinking;

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        navAgent = GetComponent<NavMeshAgent>();
        col = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        if (isSinking)
            transform.Translate(-Vector3.up * sinkSpeed * Time.deltaTime);
    }

    public override void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return;

        audioSource.PlayOneShot(hitClip);

        if (bloodEffect != null)
        {
            bloodEffect.transform.position = hitPoint;
            bloodEffect.transform.forward = hitNormal;
            bloodEffect.Play();
        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }

    public override void Die()
    {
        if (IsDead) return;
        base.Die();

        anim.SetTrigger("Die");
        audioSource.PlayOneShot(deathClip);

        navAgent.enabled = false;
        col.enabled = false;

        ScoreManager.instance?.AddScore(scoreValue);

        Destroy(gameObject, 2f);
    }

    public void StartSinking()
    {
        isSinking = true;
    }

    public void Setup(EnemyData data)
    {
        startingHealth = data.maxHP;
        scoreValue = data.scoreValue;

        var rend = GetComponentInChildren<SkinnedMeshRenderer>();
        if (rend != null)
            rend.material.color = data.skinColor;
    }
}