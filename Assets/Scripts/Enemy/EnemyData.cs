using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Survival Shooter/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float maxHP = 100f;
    public float damage = 10f;
    public float speed = 3.5f;
    public float traceDistance = 8f;
    public float attackDistance = 1.5f;
    public float attackDelay = 0.5f;
    public int scoreValue = 10;
    public Color skinColor = Color.white;
}