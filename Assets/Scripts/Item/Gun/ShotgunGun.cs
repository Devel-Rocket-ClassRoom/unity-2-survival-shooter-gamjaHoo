using UnityEngine;

public class ShotgunGun : Gun
{
    [Header("Shotgun Settings")]
    public int pelletCount = 8;
    public float spreadAngle = 15f;

    // 펠릿마다 별도 LineRenderer
    private LineRenderer[] pelletLines;

    protected override void Awake()
    {
        base.Awake();
        CreatePelletLines();
    }

    void CreatePelletLines()
    {
        pelletLines = new LineRenderer[pelletCount];

        for (int i = 0; i < pelletCount; i++)
        {
            GameObject obj = new GameObject($"PelletLine_{i}");
            obj.transform.SetParent(transform);

            LineRenderer lr = obj.AddComponent<LineRenderer>();

            // 베이스 gunLine 설정 복사
            lr.positionCount = 2;
            lr.startWidth = gunLine.startWidth;
            lr.endWidth = gunLine.endWidth;
            lr.material = gunLine.material;
            lr.enabled = false;

            pelletLines[i] = lr;
        }

        // 기존 gunLine은 샷건에서 안 씀
        gunLine.enabled = false;
    }

    protected override bool CanShoot()
    {
        return input.FireDown
            && !IsReloading
            && timer >= data.timeBetweenBullets
            && currentAmmoInMag > 0;
    }

    protected override void Shoot()
    {
        timer = 0f;
        ConsumeAmmo();
        PlayShootEffects();

        bool firstHit = true;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f) * gunBarrelEnd.forward;

            Ray ray = new Ray(gunBarrelEnd.position, direction);
            Vector3 endPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, data.range, shootableMask))
            {
                hit.collider.GetComponentInParent<LivingEntity>()
                   ?.OnDamage(data.damagePerShot, hit.point, -ray.direction);

                // 첫 번째 히트에만 이펙트
                if (firstHit)
                {
                    PlayHitEffect(hit.point);
                    firstHit = false;
                }

                endPoint = hit.point;
            }
            else
            {
                endPoint = ray.origin + ray.direction * data.range;
            }

            // 펠릿마다 LineRenderer 표시
            pelletLines[i].enabled = true;
            pelletLines[i].SetPosition(0, gunBarrelEnd.position);
            pelletLines[i].SetPosition(1, endPoint);
        }
    }

    protected override void Update()
    {
        base.Update();

        // 펠릿 라인도 일정 시간 후 끄기
        if (timer >= data.timeBetweenBullets * effectsDisplayRatio)
        {
            foreach (var line in pelletLines)
                line.enabled = false;
        }
    }
}