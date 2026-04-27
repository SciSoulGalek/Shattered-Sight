using System;

public static class GameEvents
{
    public static event Action<string> OnKeyCollected;

    public static event Action<string> OnLaserActivated;
    public static event Action<string> OnLaserDeactivated;

    public static event System.Action<string> OnPlateActivated;
    public static event System.Action<string> OnPlateDeactivated;

    public static void KeyCollected(string keyID)
    {
        OnKeyCollected?.Invoke(keyID);
    }

    public static void LaserActivated(string laserID)
    {
        OnLaserActivated?.Invoke(laserID);
    }

    public static void LaserDeactivated(string laserID)
    {
        OnLaserDeactivated?.Invoke(laserID);
    }

    public static void PlateActivated(string plateID)
    {
        OnPlateActivated?.Invoke(plateID);
    }
    
    public static void PlateDeactivated(string plateID)
    {
        OnPlateDeactivated?.Invoke(plateID);
    }
}