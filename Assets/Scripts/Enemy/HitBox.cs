using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public List<Collider> Colliders { get; private set; } = new List<Collider>();

    void OnTriggerEnter(Collider other)
    {
        Colliders.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        Colliders.Remove(other);
    }

    public bool Contains(Transform target)
    {
        return Colliders.Exists(c => c != null && c.transform == target);
    }
}