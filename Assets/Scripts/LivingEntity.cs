using UnityEngine;

public abstract class LivingEntity : MonoBehaviour
{
    public float startingHealth = 100f;
    public float currentHealth { get; protected set; }
    public bool IsDead { get; protected set; }

    public event System.Action OnDead;

    protected virtual void OnEnable()
    {
        currentHealth = startingHealth;
        IsDead = false;
    }

    public virtual void OnDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);
        Debug.Log($"{gameObject.name} took {damage} damage, current health: {currentHealth}");
        if (currentHealth <= 0f && !IsDead)
            Die();
    }

    public virtual void Die()
    {
        IsDead = true;
        OnDead?.Invoke();
    }
}