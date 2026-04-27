using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserEmitter2D : MonoBehaviour
{
    [Header("Laser")]
    public float maxDistance = 30f;
    public int maxBounces = 5;
    public LayerMask laserHitMask;

    private LineRenderer line;

    public Color laserColor = Color.red;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = 0;

        line.startColor = laserColor;
        line.endColor = laserColor;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.material.color = Color.white;
    }

    private void Update()
    {
        ShootLaser();
    }

    private void ShootLaser()
    {
        Vector2 origin = transform.position;
        Vector2 direction = transform.right;

        line.positionCount = 1;
        line.SetPosition(0, origin);

        for (int i = 0; i <= maxBounces; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                origin,
                direction,
                maxDistance,
                laserHitMask
            );

            if (hit.collider == null)
            {
                AddLaserPoint(origin + direction * maxDistance);
                break;
            }

            AddLaserPoint(hit.point);

            LaserReceiver2D receiver = hit.collider.GetComponent<LaserReceiver2D>();
            if (receiver != null)
            {
                receiver.ReceiveLaser();
                break;
            }

            LaserMirror2D mirror = hit.collider.GetComponent<LaserMirror2D>();
            if (mirror != null)
            {
                direction = Vector2.Reflect(direction, hit.normal);
                origin = hit.point + direction * 0.02f;
                continue;
            }

            break;
        }
    }

    private void AddLaserPoint(Vector2 point)
    {
        line.positionCount++;
        line.SetPosition(line.positionCount - 1, point);
    }
}