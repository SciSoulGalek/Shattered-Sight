using System;
using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static event Action OnKeyCollected;

    public string playerTag = "Player";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        OnKeyCollected?.Invoke();

        Destroy(gameObject);
    }
}