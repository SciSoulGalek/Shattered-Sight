using System.Collections;
using UnityEngine;

public class RoomCameraController : MonoBehaviour
{
    public static RoomCameraController Instance { get; private set; }

    public float transitionDuration = 0.4f;

    private Camera cam;
    private Coroutine moveRoutine;

    private void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
    }

    public void MoveToRoom(CameraRoom room)
    {
        if (room == null || room.cameraPoint == null)
            return;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine(room));
    }

    private IEnumerator MoveRoutine(CameraRoom room)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = room.cameraPoint.position;
        targetPos.z = startPos.z;

        float startSize = cam.orthographicSize;
        float targetSize = room.cameraSize;

        float t = 0f;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float k = t / transitionDuration;
            k = k * k * (3f - 2f * k);

            transform.position = Vector3.Lerp(startPos, targetPos, k);
            cam.orthographicSize = Mathf.Lerp(startSize, targetSize, k);

            yield return null;
        }

        transform.position = targetPos;
        cam.orthographicSize = targetSize;
    }
}