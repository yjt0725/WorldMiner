using UnityEngine;
using TMPro;

public class MiningUI : MonoBehaviour
{
    [Header("Texts")]
    public TMP_Text depthText;
    public TMP_Text ironText;

    // ❌ MiningProgress 아님
    // ✅ RegionProgress 사용
    private RegionProgress progress;

    private void Start()
    {
        // 초기화 순서 문제 방지를 위해 한 프레임 뒤에 바인딩
        Invoke(nameof(TryBindAndRefresh), 0f);
    }

    /// <summary>
    /// SaveData가 준비되었는지 확인하고
    /// RegionProgress를 바인딩한 뒤 UI를 갱신한다.
    /// </summary>
    private void TryBindAndRefresh()
    {
        if (SaveManager.I == null || SaveManager.I.Data == null)
            return;

        // 현재 지역 진행 상태 참조
        progress = SaveManager.I.Data.currentRegionProgress;

        Refresh();
    }

    /// <summary>
    /// UI 텍스트를 현재 데이터 기준으로 갱신한다.
    /// </summary>
    public void Refresh()
    {
        if (progress == null)
            return;

        // 깊이 표시
        depthText.text = $"Depth : {progress.depth}";

        // 철 광석 개수 표시
        int iron = InventoryHelper.GetAmount("iron");
        ironText.text = $"Iron : {iron}";
    }
}
