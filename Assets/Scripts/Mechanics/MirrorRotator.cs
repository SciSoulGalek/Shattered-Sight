using UnityEngine;

public class MirrorRotator2D : MonoBehaviour
{
    [Header("Plate IDs")]
    [SerializeField] private string clockwisePlateID;
    [SerializeField] private string counterClockwisePlateID;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f;

    private int rotationDirection = 0;

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

    private void Update()
    {
        if (rotationDirection == 0)
            return;

        transform.Rotate(0f, 0f, rotationDirection * rotationSpeed * Time.deltaTime);
    }

    private void HandlePlateActivated(string plateID)
    {
        if (plateID == clockwisePlateID)
            rotationDirection = -1;

        if (plateID == counterClockwisePlateID)
            rotationDirection = 1;
    }

    private void HandlePlateDeactivated(string plateID)
    {
        if (plateID == clockwisePlateID && rotationDirection == -1)
            rotationDirection = 0;

        if (plateID == counterClockwisePlateID && rotationDirection == 1)
            rotationDirection = 0;
    }
}