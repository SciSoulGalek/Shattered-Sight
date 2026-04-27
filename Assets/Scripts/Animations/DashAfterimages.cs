using UnityEngine;

public class DashAfterimage : MonoBehaviour
{
    public float lifetime = 0.25f;

    private SpriteRenderer sr;
    private float timer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Setup(Sprite sprite, Vector3 scale, Color color, int sortingOrder)
    {
        sr.sprite = sprite;
        transform.localScale = scale;
        sr.color = color;
        sr.sortingOrder = sortingOrder;

        timer = lifetime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        Color c = sr.color;
        c.a = timer / lifetime;
        sr.color = c;

        if (timer <= 0f)
            Destroy(gameObject);
    }
}