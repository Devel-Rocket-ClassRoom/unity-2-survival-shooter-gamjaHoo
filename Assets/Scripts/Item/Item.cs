using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemData data;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        OnPickup(other.gameObject);
        Destroy(gameObject);
    }

    protected abstract void OnPickup(GameObject player);
}