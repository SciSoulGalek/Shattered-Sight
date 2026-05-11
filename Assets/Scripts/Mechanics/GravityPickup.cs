using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GravityPickup2D : MonoBehaviour
{
    [Header("Gravity")]
    public Vector2 gravityDirection = Vector2.down;
    public float gravityStrength = 35f;

    [Header("Respawn")]
    public float respawnTime = 3f;

    [Header("Player")]
    public string playerTag = "Player";

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        PlayerGravity2D gravity = other.GetComponent<PlayerGravity2D>();

        if (gravity != null)
            gravity.SetGravity(gravityDirection, gravityStrength);

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        spriteRenderer.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        spriteRenderer.enabled = true;
        col.enabled = true;
    }
}