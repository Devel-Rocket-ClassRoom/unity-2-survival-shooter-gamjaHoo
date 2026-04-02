using UnityEngine;

public class PlayerTurning : MonoBehaviour
{
    private PlayerInput input;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    void Update()
    {
        Vector3 lookTarget = new Vector3(
            input.MouseWorldPos.x,
            transform.position.y,
            input.MouseWorldPos.z
        );

        transform.LookAt(lookTarget);
    }
    void OnEnable()
    {
        GetComponent<PlayerHealth>().OnDead += OnDead;
    }

    void OnDisable()
    {
        GetComponent<PlayerHealth>().OnDead -= OnDead;
    }

    void OnDead()
    {
        enabled = false;
    }
}