using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Spawn Point")]
    public Transform spawnPoint;

    private Vector3 _fallbackSpawnPos;
    private Rigidbody2D _rb;
    private PlayerEffects _effects;
    private PlayerController2D _controller;

    private bool _isDying;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _effects = GetComponent<PlayerEffects>();
        _controller = GetComponent<PlayerController2D>();
    }

    private void Start()
    {
        if (spawnPoint != null)
            transform.position = spawnPoint.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hazard")) return;

        Die();
    }

    public void Die()
    {
        if (_isDying) return;
        if (GameStateManager.Instance.CurrentState != GameState.Playing) return;

        _isDying = true;

        GameStateManager.Instance.SetState(GameState.Dying);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        if (_controller != null)
            _controller.enabled = false;

        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;

        if (_effects != null)
            _effects.PlayDeath();

        yield return new WaitForSeconds(0.3f);

        GameStateManager.Instance.SetState(GameState.Transitioning);

        LevelTransition.Instance.RestartLevel();
    }
}