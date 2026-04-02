using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform weaponHolder;

    [System.Serializable]
    public class WeaponSlot
    {
        public WeaponData data;
        public Gun gun;   // Inspector에서 직접 연결
    }

    public WeaponSlot[] slots;  // 슬롯 배열

    private Gun currentGun;
    private int currentIndex = -1;

    public event System.Action<Gun> OnWeaponChanged;

    void Awake()
    {
        foreach (var slot in slots)
            slot.gun.gameObject.SetActive(false);
    }

    public void EquipWeapon(WeaponData data, int index)
    {
        if (currentIndex == index) return;

        currentGun?.gameObject.SetActive(false);

        var slot = System.Array.Find(slots, s => s.data == data);
        if (slot == null)
        {
            return;
        }

        slot.gun.gameObject.SetActive(true);

        // 처음 장착할 때만 Setup
        if (slot.gun.data == null)
            slot.gun.Setup(data);

        currentGun = slot.gun;
        currentIndex = index;

        OnWeaponChanged?.Invoke(currentGun);
    }

    public void AddAmmoToWeapon(WeaponData data)
    {
        var slot = System.Array.Find(slots, s => s.data == data);
        slot?.gun.AddAmmo(data.ammoPerPickup);
    }

    public Gun GetCurrentGun() => currentGun;
}