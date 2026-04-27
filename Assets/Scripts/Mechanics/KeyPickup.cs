using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [SerializeField] private string keyID;
    public string playerTag = "Player";

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("HIT: " + other.name);

        if (other.CompareTag(playerTag))
        {
            Debug.Log("KEY COLLECTED: " + keyID);

            GameEvents.KeyCollected(keyID);
            Destroy(gameObject);
        }
    }
}