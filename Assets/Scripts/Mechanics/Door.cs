using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Animation")]
    public Vector3 openOffset = new Vector3(0f, 2f, 0f);
    public float openDuration = 0.5f;

    private Vector3 _closedPos;
    private Vector3 _openPos;

    [SerializeField] private string requiredKeyID;
    private bool _isOpen;
    private Coroutine _animRoutine;
    private Collider2D _collider;

    private void Awake()
    {
        _closedPos = transform.position;
        _openPos = _closedPos + openOffset;
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        GameEvents.OnKeyCollected += TryOpen;
    }

    private void OnDisable()
    {
        GameEvents.OnKeyCollected -= TryOpen;
    }

    private void TryOpen(string keyID)
    {
        if (_isOpen) return;

        if (keyID == requiredKeyID)
        {
            Open();
        }
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;

        if (_animRoutine != null)
            StopCoroutine(_animRoutine);

        _animRoutine = StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        float t = 0f;

        while (t < openDuration)
        {
            t += Time.deltaTime;
            float k = t / openDuration;

            transform.position = Vector3.Lerp(_closedPos, _openPos, k);
            yield return null;
        }

        transform.position = _openPos;

        if (_collider != null)
            _collider.enabled = false;
    }
}