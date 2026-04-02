using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public HitBox hitBox;

    private float damage;
    private float attackDelay = 1f;
    private float lastAttackTime;
    private EnemyHealth enemyHealth;
    private EnemyMovement enemyMovement;

    void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (enemyHealth.IsDead) return;
        if (enemyMovement.CurrentStatus != EnemyMovement.Status.Attack) return;

        if (Time.time - lastAttackTime >= attackDelay)
        {
            lastAttackTime = Time.time;
            AttackTargetsInHitBox();
        }
    }

    void AttackTargetsInHitBox()
    {
        foreach (var col in hitBox.Colliders)
        {
            if (col == null) continue;

            var living = col.GetComponent<LivingEntity>();
            if (living != null && !living.IsDead)
            {
                living.OnDamage(damage,
                    col.transform.position,
                    transform.position - col.transform.position);
            }
        }
    }

    public bool IsTargetInHitBox(Transform target)
    {
        return hitBox.Contains(target);
    }

    public void Setup(EnemyData data)
    {
        damage = data.damage;
        attackDelay = data.attackDelay;
    }
}