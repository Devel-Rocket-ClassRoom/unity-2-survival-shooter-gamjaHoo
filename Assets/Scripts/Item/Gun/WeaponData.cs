using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Survival Shooter/Weapon Data")]
public class WeaponData : ItemData
{
    [Header("Stats")]
    public float damagePerShot = 20f;
    public float timeBetweenBullets = 0.15f;
    public float range = 100f;

    [Header("Ammo")]
    public int magazineSize = 30;   // 탄창 크기
    public int maxAmmo = 120;  // 최대 보유 탄약
    public float reloadTime = 1.5f; // 재장전 시간
    public int ammoPerPickup = 30;   // 아이템 줍기 시 추가 탄약

    [Header("Prefab")]
    public GameObject weaponPrefab; // 장착될 무기 프리팹
}