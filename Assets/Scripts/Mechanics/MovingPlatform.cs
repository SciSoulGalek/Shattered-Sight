using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] points;
    public float speed = 2f;
    public bool isActive = false;

    [Header("Pressure Plate Signal")]
    [SerializeField] private string requiredPlateID;
    [SerializeField] private bool controlledByPlate = true;

    public Vector3 DeltaMovement { get; private set; }

    private Vector3 _lastPosition;
    
    private int _currentIndex = 0;

    private void Start()
    {
        _lastPosition = transform.position;
    }

    private void OnEnable()
    {
        GameEvents.OnPlateActivated += HandlePlateActivated;
        GameEvents.OnPlateDeactivated += HandlePlateDeactivated;
    }

    private void OnDisable()
    {
        GameEvents.OnPlateActivated -= HandlePlateActivated;
        GameEvents.OnPlateDeactivated -= HandlePlateDeactivated;
    }

    private void Reset()
    {
        points = new Transform[1];

        GameObject p0 = new GameObject("Point0");
        p0.transform.position = transform.position;
        p0.transform.SetParent(transform.parent);

        points[0] = p0.transform;
    }

    private void FixedUpdate()
    {
        DeltaMovement = transform.position - _lastPosition;

        if (!isActive) return;
        if (points == null || points.Length == 0) return;

        Transform target = points[_currentIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            _currentIndex++;

            if (_currentIndex >= points.Length)
                _currentIndex = 0;
        }

        _lastPosition = transform.position;
    }

    private void HandlePlateActivated(string plateID)
    {
        if (!controlledByPlate)
            return;

        if (plateID == requiredPlateID)
            SetActive(true);
    }

    private void HandlePlateDeactivated(string plateID)
    {
        if (!controlledByPlate)
            return;

        if (plateID == requiredPlateID)
            SetActive(false);
    }

    public void SetActive(bool value)
    {
        isActive = value;
    }
}