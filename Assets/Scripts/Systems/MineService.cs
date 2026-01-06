using UnityEngine;

public static class MineService
{
    /// <summary>
    /// 채굴을 1회 시도한다.
    /// 성공하면 true, 실패하면 false를 반환한다.
    /// </summary>
    public static bool TryMineOnce()
    {
        // SaveManager 또는 SaveData가 아직 준비되지 않았으면 중단
        if (SaveManager.I == null || SaveManager.I.Data == null)
        {
            Debug.LogWarning("[MineService] SaveData not ready");
            return false;
        }

        // ✅ 현재 지역의 진행 상태 (진실의 원본)
        var progress = SaveManager.I.Data.currentRegionProgress;

        // 1️⃣ 채굴 진행도 증가
        progress.depth += 1;              // 깊이 +1
        progress.minedTilesTotal += 1;    // 누적 채굴 횟수 +1

        // 2️⃣ 광석 지급 (임시: iron 1개)
        AddOre("iron", 1);

        // 3️⃣ 저장
        SaveManager.I.Save();

        return true;
    }

    /// <summary>
    /// 전역 광석 인벤토리에 광석을 추가한다.
    /// </summary>
    private static void AddOre(string oreId, int amount)
    {
        // 전역 광석 인벤토리 리스트
        var list = SaveManager.I.Data.globalOreInventory;

        // 이미 같은 광석이 있는지 찾는다
        var stack = list.Find(o => o.oreId == oreId);

        if (stack != null)
        {
            // 있으면 수량만 증가
            stack.amount += amount;
        }
        else
        {
            // 없으면 새로 추가
            list.Add(new OreStack(oreId, amount));
        }
    }
}
