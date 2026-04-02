using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // 다른 스크립트에서 읽어가는 프로퍼티
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool Fire { get; private set; }
    public bool FireDown { get; private set; }
    public bool Reload { get; private set; }
    public float WeaponSlot { get; private set; }  // 숫자키 1~4

    // 마우스 월드 좌표 (PlayerTurning에서 사용)
    public Vector3 MouseWorldPos { get; private set; }

    private Plane floorPlane = new Plane(Vector3.up, Vector3.zero);

    void Update()
    {
        if (PauseManager.instance != null && PauseManager.instance.IsPaused)
        {
            Horizontal = 0f;
            Vertical = 0f;
            Fire = false;
            FireDown = false;
            Reload = false;
            WeaponSlot = -1f;
            return;
        }

        Horizontal = Input.GetAxisRaw("Horizontal");
        Vertical = Input.GetAxisRaw("Vertical");
        Fire = Input.GetButton("Fire1");
        FireDown = Input.GetButtonDown("Fire1");
        Reload = Input.GetKeyDown(KeyCode.R);

        WeaponSlot = -1f;
        if (Input.GetKeyDown(KeyCode.Alpha1)) WeaponSlot = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) WeaponSlot = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) WeaponSlot = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) WeaponSlot = 3;

        UpdateMouseWorldPos();
    }

    void UpdateMouseWorldPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (floorPlane.Raycast(ray, out float dist))
            MouseWorldPos = ray.GetPoint(dist);
    }
}