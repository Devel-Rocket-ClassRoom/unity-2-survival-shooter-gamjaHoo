using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int maxWeaponSlots = 4;

    public WeaponData startingWeapon;

    private WeaponData[] weaponSlots;
    private WeaponController weaponController;
    private PlayerInput input;

    public event System.Action<WeaponData[], int> OnInventoryChanged;

    void Awake()
    {
        weaponSlots = new WeaponData[maxWeaponSlots];
        weaponController = GetComponent<WeaponController>();
        input = GetComponent<PlayerInput>();
    }

    void Start()
    {
        if (startingWeapon != null)
            PickupWeapon(startingWeapon);
    }

    void Update()
    {
        if (input.WeaponSlot >= 0 && input.WeaponSlot < maxWeaponSlots)
            TryEquip((int)input.WeaponSlot);
    }

    // 무기 아이템 줍기 시 호출
    public void PickupWeapon(WeaponData data)
    {
        // 이미 보유 중이면 탄약만 추가
        for (int i = 0; i < maxWeaponSlots; i++)
        {
            if (weaponSlots[i] == data)
            {
                weaponController.AddAmmoToWeapon(data);
                return;
            }
        }

        // 빈 슬롯에 추가
        for (int i = 0; i < maxWeaponSlots; i++)
        {
            if (weaponSlots[i] == null)
            {
                weaponSlots[i] = data;
                OnInventoryChanged?.Invoke(weaponSlots, i);
                TryEquip(i);  // 줍자마자 장착
                return;
            }
        }

        Debug.Log("무기 슬롯이 가득 찼습니다.");
    }

    void TryEquip(int index)
    {
        if (weaponSlots[index] == null) return;
        weaponController.EquipWeapon(weaponSlots[index], index);
        OnInventoryChanged?.Invoke(weaponSlots, index);
    }

    public WeaponData GetWeapon(int index) => weaponSlots[index];
}