using System.Collections;
using UnityEngine;

public class Door2D : MonoBehaviour
{
    [Header("Анимация открытия")]
    public Vector3 openOffset = new Vector3(0f, 2f, 0f);
    public float openDuration = 0.5f;              

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
        Vector3 start = _closedPos;
        Vector3 target = _openPos;

        while (t < openDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / openDuration);
            transform.position = Vector3.Lerp(start, target, k);
            yield return null;
        }

        transform.position = target;

        if (_collider != null)
            _collider.enabled = false;

        _animRoutine = null;
    }
}
