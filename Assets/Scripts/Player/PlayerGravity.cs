using System.Collections;
using UnityEngine;

public class PlayerGravity2D : MonoBehaviour
{
    [Header("Default Gravity")]
    [SerializeField] private Vector2 defaultGravityDirection = Vector2.down;
    [SerializeField] private float defaultGravityStrength = 22f;

    public Vector2 GravityDirection { get; private set; }
    public float GravityStrength { get; private set; }

    private Rigidbody2D rb;
    private PlayerController2D player;

    private Coroutine gravityRoutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerController2D>();

        // Disable Unity built-in gravity
        rb.gravityScale = 0f;

        GravityDirection = defaultGravityDirection.normalized;
        GravityStrength = defaultGravityStrength;
    }

    private void FixedUpdate()
    {
        // No gravity during dash
        if (player != null && player.IsDashing)
            return;

        rb.linearVelocity +=
            GravityDirection *
            GravityStrength *
            Time.fixedDeltaTime;
    }

    public void SetGravity(Vector2 direction, float strength)
    {
        GravityDirection = direction.normalized;
        GravityStrength = strength;
    }

    public void ResetGravity()
    {
        GravityDirection = defaultGravityDirection.normalized;
        GravityStrength = defaultGravityStrength;
    }

    public void OverrideGravityTemporarily(float temporaryStrength, float duration)
    {
        if (gravityRoutine != null)
            StopCoroutine(gravityRoutine);

        gravityRoutine = StartCoroutine(
            GravityOverrideRoutine(
                temporaryStrength,
                duration
            )
        );
    }

    private IEnumerator GravityOverrideRoutine(
        float temporaryStrength,
        float duration
    )
    {
        float originalStrength = GravityStrength;

        GravityStrength = temporaryStrength;

        yield return new WaitForSeconds(duration);

        GravityStrength = originalStrength;
    }
}