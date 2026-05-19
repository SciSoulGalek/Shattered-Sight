using UnityEngine;

public class PlatformPathDots : MonoBehaviour
{
    [Header("Points Parent")]
    [SerializeField] private Transform platformPoints;

    [Header("Dots")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private float spacing = 0.5f;

    private void Start()
    {
        GenerateDots();
    }

    private void GenerateDots()
    {
        if (platformPoints == null || platformPoints.childCount < 2)
            return;

        int pointCount = platformPoints.childCount;

        for (int i = 0; i < pointCount; i++)
        {
            Transform start = platformPoints.GetChild(i);

            // loop back to first point
            Transform end = platformPoints.GetChild((i + 1) % pointCount);

            CreateDotsBetween(start.position, end.position);
        }
    }

    private void CreateDotsBetween(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);

        int count = Mathf.FloorToInt(distance / spacing);

        for (int i = 0; i <= count; i++)
        {
            float t = (float)i / count;

            Vector2 pos = Vector2.Lerp(start, end, t);

            GameObject dot = Instantiate(dotPrefab, pos, Quaternion.identity, transform);

            dot.layer = LayerMask.NameToLayer("Shard");
        }
    }
}