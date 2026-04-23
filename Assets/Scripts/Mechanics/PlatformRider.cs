using UnityEngine;

public class PlatformRider2D : MonoBehaviour
{
    public string platformTag = "MovingPlatform";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(platformTag))
        {
            transform.SetParent(collision.collider.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(platformTag))
        {
            if (transform.parent == collision.collider.transform)
            {
                transform.SetParent(null);
            }
        }
    }
}
