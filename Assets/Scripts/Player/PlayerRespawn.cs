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
        _rb        = GetComponent<Rigidbody2D>();
        _effects   = GetComponent<PlayerEffects>();
        _controller = GetComponent<PlayerController2D>();
    }

    private void Start()
    {
        _fallbackSpawnPos = transform.position;
    }

    private Vector3 GetSpawnPosition()
    {
        return spawnPoint ? spawnPoint.position : _fallbackSpawnPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Hazard")) return;
        if (_isDying) return;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        _isDying = true;

        if (_controller != null)
            _controller.enabled = false;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _rb.simulated = false; 
        }

        float deathWait = 0.3f;
        if (_effects != null)
        {
            _effects.PlayDeath();
            if (_effects.deathAnimDuration > 0f)
                deathWait = _effects.deathAnimDuration;
        }

        if (deathWait > 0f)
            yield return new WaitForSeconds(deathWait);

        transform.position = GetSpawnPosition();

        float spawnWait = 0.1f;
        if (_effects != null)
        {
            _effects.PlaySpawn();
            if (_effects.spawnAnimDuration > 0f)
                spawnWait = _effects.spawnAnimDuration;
        }

        if (spawnWait > 0f)
            yield return new WaitForSeconds(spawnWait);

        if (_rb != null)
            _rb.simulated = true;

        if (_controller != null)
            _controller.enabled = true;

        _isDying = false;
    }
}
