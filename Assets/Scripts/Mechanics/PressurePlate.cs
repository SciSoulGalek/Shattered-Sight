using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate2D : MonoBehaviour
{
    [Header("Activator")]
    public string[] activatorTags = { "Player", "Box" };

    [Header("Platform")]
    public MovingPlatform2D platformToControl;

    [Header("Plate Animation")]
    public Vector3 pressedOffset = new Vector3(0f, -0.1f, 0f);
    public float moveSpeed = 10f;                              

    private int _objectsOnPlate = 0;
    private bool _isPressed;

    private Vector3 _initialPos;
    private Vector3 _pressedPos;

    private void Awake()
    {
        _initialPos = transform.position;
        _pressedPos = _initialPos + pressedOffset;

        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Update()
    {
        Vector3 target = _isPressed ? _pressedPos : _initialPos;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);
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

        _isPressed = _objectsOnPlate > 0;

        if (platformToControl != null)
        {
            platformToControl.SetActive(true);
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsActivator(other)) return;

        _objectsOnPlate = Mathf.Max(0, _objectsOnPlate - 1);
        _isPressed = _objectsOnPlate > 0;

        if (_objectsOnPlate == 0 && platformToControl != null)
        {
            platformToControl.SetActive(false);
        }

    }
}
