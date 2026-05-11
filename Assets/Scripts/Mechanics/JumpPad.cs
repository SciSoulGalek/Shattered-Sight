using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public Vector2 launchDirection = Vector2.up;
    public float bounceForce = 18f;
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null)
            return;

        PlayerController2D player = other.GetComponent<PlayerController2D>();
        if (player != null)
            player.CancelHeldJump();

        Vector2 dir = launchDirection.normalized;

        float existingSpeed = Vector2.Dot(rb.linearVelocity, dir);

        if (existingSpeed > 0f)
            rb.linearVelocity -= dir * existingSpeed;

        rb.AddForce(dir * bounceForce, ForceMode2D.Impulse);
    }
}