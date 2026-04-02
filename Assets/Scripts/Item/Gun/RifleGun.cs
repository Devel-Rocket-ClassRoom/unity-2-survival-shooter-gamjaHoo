using UnityEngine;

public class RifleGun : Gun
{
    [Header("Rifle Settings")]
    public float baseSpread = 0.02f;  // 기본 탄착 범위
    public float maxSpread = 0.12f;  // 최대 탄착 범위
    public float spreadPerShot = 0.02f;  // 연사 시 확산량
    public float spreadRecovery = 0.05f; // 초당 회복량

    private float currentSpread = 0f;

    protected override void Update()
    {
        // 방아쇠 안 당기면 반동 회복
        if (!input.Fire)
            currentSpread = Mathf.Max(0f,
                currentSpread - spreadRecovery * Time.deltaTime);

        base.Update();
    }

    protected override void Shoot()
    {
        timer = 0f;
        ConsumeAmmo();
        PlayShootEffects();

        // 반동 적용된 방향 계산
        Vector3 spread = new Vector3(
            Random.Range(-currentSpread, currentSpread),
            Random.Range(-currentSpread, currentSpread),
            0f);
        Vector3 direction = (gunBarrelEnd.forward + spread).normalized;

        Ray ray = new Ray(gunBarrelEnd.position, direction);

        if (Physics.Raycast(ray, out RaycastHit hit, data.range, shootableMask))
        {
            hit.collider.GetComponentInParent<LivingEntity>()
               ?.OnDamage(data.damagePerShot, hit.point, -ray.direction);

            PlayHitEffect(hit.point);
            ShowGunLine(hit.point);
        }
        else
        {
            ShowGunLine(ray.origin + ray.direction * data.range);
        }

        // 연사할수록 반동 증가
        currentSpread = Mathf.Min(currentSpread + spreadPerShot, maxSpread);
    }
}