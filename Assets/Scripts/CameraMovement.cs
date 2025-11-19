using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject target;

    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(target.transform.position.x, transform.position.y, -10);    
    }
}
