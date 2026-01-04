using UnityEngine;

public class DevRegionButtons : MonoBehaviour
{
    public void GoNA()
    {
        RegionManager.I.ChangeRegion("NA");
    }

    public void GoME()
    {
        RegionManager.I.ChangeRegion("ME");
    }
}
