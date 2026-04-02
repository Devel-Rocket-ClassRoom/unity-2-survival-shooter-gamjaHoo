using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSlotUI : MonoBehaviour
{
    public Image slotBackground;
    public Image weaponIcon;
    public TMP_Text keyText;
    public TMP_Text weaponNameText;
    public GameObject emptyIndicator;

    private static readonly Color colorActive = new Color(1f, 0.85f, 0.2f, 0.15f);
    private static readonly Color colorInactive = new Color(1f, 1f, 1f, 0.05f);
    private static readonly Color borderActive = new Color(1f, 0.85f, 0.2f, 0.8f);
    private static readonly Color borderInactive = new Color(1f, 1f, 1f, 0.15f);

    public void Init(int keyNumber)
    {
        keyText.text = keyNumber.ToString();
        SetEmpty();
    }

    public void SetWeapon(WeaponData data, bool isActive)
    {
        if (data == null) { SetEmpty(); return; }

        emptyIndicator?.SetActive(false);
        weaponIcon.gameObject.SetActive(true);

        if (data.icon != null)
            weaponIcon.sprite = data.icon;

        weaponNameText.text = data.itemName;

        // 활성 슬롯 강조
        slotBackground.color = isActive ? colorActive : colorInactive;

        var outline = GetComponent<Outline>();
        if (outline != null)
            outline.effectColor = isActive ? borderActive : borderInactive;
    }

    public void SetEmpty()
    {
        weaponIcon.gameObject.SetActive(false);
        weaponNameText.text = "";
        emptyIndicator?.SetActive(true);
        slotBackground.color = new Color(1f, 1f, 1f, 0.03f);
    }
}