using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;

    private Animator anim;
    private Rigidbody rb;
    private PlayerInput input;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();   // 추가
    }

    void FixedUpdate()
    {
        float h = input.Horizontal;            // Input 직접 호출 제거
        float v = input.Vertical;

        Move(h, v);
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        Vector3 movement = new Vector3(h, 0f, v).normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(transform.position + movement);
    }

    void Animating(float h, float v)
    {
        anim.SetBool("IsWalking", h != 0f || v != 0f);
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