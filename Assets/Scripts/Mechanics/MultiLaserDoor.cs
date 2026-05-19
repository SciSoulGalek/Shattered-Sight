using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLaserDoor : MonoBehaviour
{
    [Header("Animation")]
    public Vector3 openOffset = new Vector3(0f, 2f, 0f);
    public float openDuration = 0.5f;

    [Header("Required Lasers")]
    [SerializeField] private string[] requiredLaserIDs;

    private HashSet<string> activeLasers = new HashSet<string>();

    private Vector3 closedPos;
    private Vector3 openPos;

    private bool isOpen;
    private Coroutine animRoutine;
    private Collider2D col;

    private void Awake()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        GameEvents.OnLaserActivated += HandleLaserActivated;
        GameEvents.OnLaserDeactivated += HandleLaserDeactivated;
    }

    private void OnDisable()
    {
        GameEvents.OnLaserActivated -= HandleLaserActivated;
        GameEvents.OnLaserDeactivated -= HandleLaserDeactivated;
    }

    private void HandleLaserActivated(string laserID)
    {
        if (!IsRequiredLaser(laserID))
            return;

        activeLasers.Add(laserID);
        CheckDoorState();
    }

    private void HandleLaserDeactivated(string laserID)
    {
        if (!IsRequiredLaser(laserID))
            return;

        activeLasers.Remove(laserID);
        CheckDoorState();
    }

    private bool IsRequiredLaser(string laserID)
    {
        foreach (string id in requiredLaserIDs)
        {
            if (id == laserID)
                return true;
        }

        return false;
    }

    private void CheckDoorState()
    {
        bool allActive = true;

        foreach (string id in requiredLaserIDs)
        {
            if (!activeLasers.Contains(id))
            {
                allActive = false;
                break;
            }
        }

        if (allActive)
            Open();
        else
            Close();
    }

    public void Open()
    {
        if (isOpen) return;

        isOpen = true;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(MoveDoor(openPos, false));
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;

        if (col != null)
            col.enabled = true;

        if (animRoutine != null)
            StopCoroutine(animRoutine);

        animRoutine = StartCoroutine(MoveDoor(closedPos, true));
    }

    private IEnumerator MoveDoor(Vector3 targetPos, bool closing)
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

        if (!closing && col != null)
            col.enabled = false;
    }
}