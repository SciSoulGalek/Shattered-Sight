using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
    public Transform[] points;  
    public float speed = 2f;
    public bool isActive = false;

    private int _currentIndex = 0;

    private void Reset()
    {
        points = new Transform[1];
        GameObject p0 = new GameObject("Point0");
        p0.transform.position = transform.position;
        p0.transform.SetParent(transform.parent);
        points[0] = p0.transform;
    }

    private void Update()
    {
        if (!isActive) return;
        if (points == null || points.Length == 0) return;

        Transform target = points[_currentIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist < 0.01f)
        {
            _currentIndex++;
            if (_currentIndex >= points.Length)
                _currentIndex = 0;
        }
    }

    public void SetActive(bool value)
    {
        isActive = value;
    }
}
