using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShardOutlineTrigger : MonoBehaviour
{
    [Header("Shard Outline")]
    public GameObject outlineObject;

    [Header("Player")]
    public string playerTag = "Player";

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnEnable()
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (outlineObject == null) return;

        if (other.CompareTag(playerTag))
        {
            outlineObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (outlineObject == null) return;

        if (other.CompareTag(playerTag))
        {
            outlineObject.SetActive(false);
        }
    }
}
