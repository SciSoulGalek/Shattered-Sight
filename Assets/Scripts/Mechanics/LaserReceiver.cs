using UnityEngine;

public class LaserReceiver2D : MonoBehaviour
{
    [SerializeField] private string laserID;

    private bool isReceivingLaser;
    private bool wasReceivingLaser;

    public void ReceiveLaser()
    {
        isReceivingLaser = true;
    }

    private void LateUpdate()
    {
        if (isReceivingLaser && !wasReceivingLaser)
        {
            Debug.Log($"{gameObject.name}: Laser activated ID = {laserID}");
            GameEvents.LaserActivated(laserID);
        }

        if (!isReceivingLaser && wasReceivingLaser)
        {
            Debug.Log($"{gameObject.name}: Laser deactivated ID = {laserID}");
            GameEvents.LaserDeactivated(laserID);
        }

        wasReceivingLaser = isReceivingLaser;
        isReceivingLaser = false;
    }
}