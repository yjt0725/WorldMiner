using System;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    public static RegionManager I { get; private set; }

    public RegionProgress CurrentProgress { get; private set; }
    public string CurrentRegionId => SaveManager.I.Data.currentRegionId;

    // (나중에 UI 갱신용)
    public event Action<string> OnRegionChanged;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Save가 먼저 로드되어 있어야 함 (SaveBootstrap이 LoadOrCreate 호출)
        LoadCurrentRegion();
    }

    public void LoadCurrentRegion()
    {
        if (SaveManager.I == null || SaveManager.I.Data == null)
        {
            Debug.LogWarning("[RegionManager] Save data not ready yet");
            return;
        }

        var data = SaveManager.I.Data;
        CurrentProgress = data.GetOrCreateRegion(data.currentRegionId);
        TouchActiveTime(CurrentProgress);
    }

    /// <summary>
    /// 지역 이동(A안): 지역별 진행도 분리 저장/복원
    /// </summary>
    public void ChangeRegion(string targetRegionId)
    {
        if (string.IsNullOrWhiteSpace(targetRegionId)) return;

        var data = SaveManager.I.Data;
        var fromId = data.currentRegionId;

        if (fromId == targetRegionId)
        {
            // 이미 현재 지역이면 아무 것도 안 함
            return;
        }

        // 1) 현재 지역 진행도 저장 전 마지막 활동 시간 갱신
        var fromProgress = data.GetOrCreateRegion(fromId);
        TouchActiveTime(fromProgress);

        // 2) (여기서 "일반 광물 자동 판매" 같은 정책을 넣을 수 있음)
        // SellAllNormalOres(fromProgress);  // 지금은 MVP라 생략

        // 3) currentRegion 변경
        data.currentRegionId = targetRegionId;

        // 4) 대상 지역 진행도 로드/생성
        var toProgress = data.GetOrCreateRegion(targetRegionId);
        TouchActiveTime(toProgress);

        // 5) 런타임 CurrentProgress 갱신
        CurrentProgress = toProgress;

        // 6) 저장
        SaveManager.I.Save();

        // 7) UI/씬에 알림
        OnRegionChanged?.Invoke(targetRegionId);

        Debug.Log($"[RegionManager] Region changed: {fromId} -> {targetRegionId}");
    }

    private void TouchActiveTime(RegionProgress rp)
    {
        rp.lastActiveUtcTicks = DateTime.UtcNow.Ticks;
    }
}
