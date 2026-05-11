using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTrigger : MonoBehaviour
{
    public enum DoorwayAxis
    {
        Horizontal,
        Vertical
    }

    public DoorwayAxis axis = DoorwayAxis.Horizontal;

    public CameraRoom roomA;
    public CameraRoom roomB;
    
    public float crossingOffset = 0.6f;

    public string playerTag = "Player";

    private bool isTransitioning;

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
            return;

        if (!other.CompareTag(playerTag))
            return;

        bool cameFromNegativeSide;

        if (axis == DoorwayAxis.Horizontal)
            cameFromNegativeSide = other.transform.position.x < transform.position.x;
        else
            cameFromNegativeSide = other.transform.position.y < transform.position.y;

        CameraRoom targetRoom = cameFromNegativeSide ? roomB : roomA;

        Vector2 pushDirection;

        if (axis == DoorwayAxis.Horizontal)
            pushDirection = cameFromNegativeSide ? Vector2.right : Vector2.left;
        else
            pushDirection = cameFromNegativeSide ? Vector2.up : Vector2.down;

        RoomCameraController.Instance.MoveToRoom(targetRoom);

        other.transform.position += (Vector3)(pushDirection.normalized * crossingOffset);

        PlayerController2D player = other.GetComponent<PlayerController2D>();

        if (player != null)
        {
            player.LockControls(0.5f);
        }

        isTransitioning = true;
        Invoke(nameof(ResetTransition), 0.5f);
    }

    private void ResetTransition()
    {
        isTransitioning = false;
    }
}