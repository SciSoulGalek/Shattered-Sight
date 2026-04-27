using UnityEngine;

public class JumpPad2D : MonoBehaviour
{
    public Vector2 launchDirection = Vector2.up;
    public float launchSpeed = 13f;
    public string playerTag = "Player";

    [Header("Anti-stuck")]
    public float pushOutDistance = 0.08f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb == null)
            return;

        Vector2 direction = launchDirection.normalized;

        // Move player slightly away from the wall/pad
        rb.position += direction * pushOutDistance;

        // Replace velocity completely
        rb.linearVelocity = direction * launchSpeed;
    }
}