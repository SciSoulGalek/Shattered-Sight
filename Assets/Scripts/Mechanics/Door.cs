using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Animation")]
    public Vector3 openOffset = new Vector3(0f, 2f, 0f);
    public float openDuration = 0.5f;

    [Header("IDs")]
    [SerializeField] private string requiredKeyID;
    [SerializeField] private string requiredLaserID;

    [Header("Behavior")]
    [SerializeField] private bool closeWhenLaserOff = true;

    private Vector3 _closedPos;
    private Vector3 _openPos;

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
        GameEvents.OnKeyCollected += TryOpenWithKey;
        GameEvents.OnLaserActivated += TryOpenWithLaser;
        GameEvents.OnLaserDeactivated += TryCloseWithLaser;
    }

    private void OnDisable()
    {
        GameEvents.OnKeyCollected -= TryOpenWithKey;
        GameEvents.OnLaserActivated -= TryOpenWithLaser;
        GameEvents.OnLaserDeactivated -= TryCloseWithLaser;
    }

    private void TryOpenWithKey(string keyID)
    {
        if (string.IsNullOrEmpty(requiredKeyID))
            return;

        if (keyID == requiredKeyID)
            Open();
    }

    private void TryOpenWithLaser(string laserID)
    {
        if (string.IsNullOrEmpty(requiredLaserID))
            return;

        if (laserID == requiredLaserID)
            Open();
    }

    private void TryCloseWithLaser(string laserID)
    {
        if (!closeWhenLaserOff)
            return;

        if (string.IsNullOrEmpty(requiredLaserID))
            return;

        if (laserID == requiredLaserID)
            Close();
    }

    public void Open()
    {
        if (_isOpen) return;
        _isOpen = true;

        if (_animRoutine != null)
            StopCoroutine(_animRoutine);

        _animRoutine = StartCoroutine(MoveDoorRoutine(_openPos, false));
    }

    public void Close()
    {
        if (!_isOpen) return;
        _isOpen = false;

        if (_collider != null)
            _collider.enabled = true;

        if (_animRoutine != null)
            StopCoroutine(_animRoutine);

        _animRoutine = StartCoroutine(MoveDoorRoutine(_closedPos, true));
    }

    private IEnumerator MoveDoorRoutine(Vector3 targetPos, bool closing)
    {
        Vector3 startPos = transform.position;
        float t = 0f;

        while (t < openDuration)
        {
            t += Time.deltaTime;
            float k = t / openDuration;

            transform.position = Vector3.Lerp(startPos, targetPos, k);
            yield return null;
        }

        transform.position = targetPos;

        if (!closing && _collider != null)
            _collider.enabled = false;
    }
}