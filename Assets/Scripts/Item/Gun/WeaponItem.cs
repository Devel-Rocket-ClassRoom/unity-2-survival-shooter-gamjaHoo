using UnityEngine;

public class WeaponItem : Item
{
    protected override void OnPickup(GameObject player)
    {
        player.GetComponent<PlayerInventory>()
              ?.PickupWeapon(data as WeaponData);
    }
}