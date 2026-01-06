using UnityEngine;
using TMPro;

public class DevMiningButtons : MonoBehaviour
{
    public TMP_Text statusText;

    private void Start()
    {
        // 초기화 순서 문제 방지: 첫 프레임 뒤에 Refresh
        Invoke(nameof(Refresh), 0f);

        if (RegionManager.I != null)
            RegionManager.I.OnRegionChanged += OnRegionChanged;
    }

    private void OnDestroy()
    {
        if (RegionManager.I != null)
            RegionManager.I.OnRegionChanged -= OnRegionChanged;
    }

    private void OnRegionChanged(string _)
    {
        Refresh();
    }

    
    public void AddDepth()
    {
        if (SaveManager.I == null || SaveManager.I.Data == null)
        {
            Debug.LogWarning("[DevMiningButtons] SaveData not ready");
            return;
        }

        

        Refresh();
    }


    public void Refresh()
    {
        if (statusText == null) return;
        if (SaveManager.I == null || SaveManager.I.Data == null) return;
        if (RegionManager.I == null || RegionManager.I.CurrentProgress == null) return;

        var data = SaveManager.I.Data;
        var rp = RegionManager.I.CurrentProgress;

        statusText.text =
            $"Region: {data.currentRegionId}\n" +
            $"Depth: {rp.depth} m\n" +
            $"Money: {data.globalMoney}";
    }
}
