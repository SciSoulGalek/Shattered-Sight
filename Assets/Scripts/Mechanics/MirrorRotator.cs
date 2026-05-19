using UnityEngine;

public class MirrorRotator2D : MonoBehaviour
{
    [Header("Plate IDs")]
    [SerializeField] private string clockwisePlateID;
    [SerializeField] private string counterClockwisePlateID;

    [Header("Rotation")]
    [SerializeField] private float rotationStep = 45f;

    [Header("Cooldown")]
    [SerializeField] private float rotationCooldown = 0.25f;

    private float cooldownTimer;

    private void OnEnable()
    {
        GameEvents.OnPlateActivated += HandlePlateActivated;
    }

    private void OnDisable()
    {
        GameEvents.OnPlateActivated -= HandlePlateActivated;
    }

    private void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    private void HandlePlateActivated(string plateID)
    {
        if (cooldownTimer > 0f)
            return;

        cooldownTimer = rotationCooldown;

        if (plateID == clockwisePlateID)
        {
            transform.Rotate(0f, 0f, -rotationStep);
        }

        if (plateID == counterClockwisePlateID)
        {
            transform.Rotate(0f, 0f, rotationStep);
        }
    }
}