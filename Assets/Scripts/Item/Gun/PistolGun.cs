using UnityEngine;

public class PistolGun : Gun
{
    protected override void Shoot()
    {
        timer = 0f;
        ConsumeAmmo();
        PlayShootEffects();

        Ray ray = new Ray(gunBarrelEnd.position, gunBarrelEnd.forward);

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
    }
}