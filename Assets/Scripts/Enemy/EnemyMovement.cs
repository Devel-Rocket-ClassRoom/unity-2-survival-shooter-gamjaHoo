using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public enum Status { Idle, Trace, Attack, Die }

    [Header("Settings")]
    public float traceDistance = 8f;
    public float attackDistance = 1.5f;
    public LayerMask targetLayer;

    public Status CurrentStatus { get; private set; }

    private NavMeshAgent navAgent;
    private Animator anim;
    private EnemyHealth enemyHealth;
    private EnemyAttack enemyAttack;
    private Transform target;

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    void OnEnable()
    {
        SetStatus(Status.Idle);
    }

    void Update()
    {
        if (enemyHealth.IsDead)
        {
            SetStatus(Status.Die);
            return;
        }

        switch (CurrentStatus)
        {
            case Status.Idle: UpdateIdle(); break;
            case Status.Trace: UpdateTrace(); break;
            case Status.Attack: UpdateAttack(); break;
        }
    }

    void SetStatus(Status next)
    {
        CurrentStatus = next;

        switch (next)
        {
            case Status.Idle:
                anim.SetBool("HasTarget", false);
                if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
                    navAgent.isStopped = true;
                break;
            case Status.Trace:
                anim.SetBool("HasTarget", true);
                if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
                    navAgent.isStopped = false;
                break;
            case Status.Attack:
                anim.SetBool("HasTarget", false);
                if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
                    navAgent.isStopped = true;
                break;
            case Status.Die:
                if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
                    navAgent.isStopped = true;
                navAgent.enabled = false;
                break;
        }
    }

    void UpdateIdle()
    {
        // 기존 타겟이 트레이스 범위 안이면 바로 추적
        if (target != null &&
            Vector3.Distance(transform.position, target.position) <= traceDistance)
        {
            SetStatus(Status.Trace);
            return;
        }

        target = FindTarget(traceDistance);
    }

    void UpdateTrace()
    {
        if (target == null ||
            Vector3.Distance(transform.position, target.position) > traceDistance)
        {
            target = null;
            SetStatus(Status.Idle);
            return;
        }

        if (enemyAttack.IsTargetInHitBox(target))
        {
            SetStatus(Status.Attack);
            return;
        }

        // 추가
        if (navAgent.isActiveAndEnabled && navAgent.isOnNavMesh)
            navAgent.SetDestination(target.position);
    }

    void UpdateAttack()
    {
        if (target == null)
        {
            SetStatus(Status.Idle);
            return;
        }

        // HitBox 범위 벗어나면 다시 추적
        if (!enemyAttack.IsTargetInHitBox(target))
        {
            SetStatus(Status.Trace);
            return;
        }

        // 공격 중에도 타겟 방향 바라봄
        Vector3 lookAt = new Vector3(target.position.x,
                                      transform.position.y,
                                      target.position.z);
        transform.LookAt(lookAt);
    }

    Transform FindTarget(float radius)
    {
        Collider[] cols = Physics.OverlapSphere(
            transform.position, radius, targetLayer);

        if (cols.Length == 0) return null;

        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var col in cols)
        {
            float dist = Vector3.Distance(
                transform.position, col.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = col.transform;
            }
        }
        return nearest;
    }

    public void Setup(EnemyData data)
    {
        traceDistance = data.traceDistance;
        attackDistance = data.attackDistance;
        navAgent.speed = data.speed;
    }
}