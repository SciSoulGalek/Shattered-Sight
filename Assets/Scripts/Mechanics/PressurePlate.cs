using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate2D : MonoBehaviour
{
    [Header("Activator")]
    public string[] activatorTags = { "Player", "Box" };

    [Header("ID Broadcast")]
    [SerializeField] private string plateID;

    [Header("Plate Animation")]
    public Vector3 pressedOffset = new Vector3(0f, -0.1f, 0f);
    public float moveSpeed = 10f;

    private int _objectsOnPlate = 0;
    private bool _isPressed;

    private Vector3 _initialLocalPos;
    private Vector3 _pressedLocalPos;

    private void Awake()
    {
        _initialLocalPos = transform.localPosition;
        _pressedLocalPos = _initialLocalPos + pressedOffset;

        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        Vector3 target = _isPressed ? _pressedLocalPos : _initialLocalPos;

        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            target,
            Time.deltaTime * moveSpeed
        );
    }

    private bool IsActivator(Collider2D other)
    {
        foreach (var tag in activatorTags)
        {
            if (other.CompareTag(tag))
                return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsActivator(other)) return;

        _objectsOnPlate++;

        if (!_isPressed)
        {
            _isPressed = true;

            if (!string.IsNullOrEmpty(plateID))
                GameEvents.PlateActivated(plateID);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsActivator(other)) return;

        _objectsOnPlate = Mathf.Max(0, _objectsOnPlate - 1);

        if (_objectsOnPlate == 0 && _isPressed)
        {
            _isPressed = false;

            if (!string.IsNullOrEmpty(plateID))
                GameEvents.PlateDeactivated(plateID);
        }
    }
}