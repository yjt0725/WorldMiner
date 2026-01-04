using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int version = 1;

    // Global
    public long globalMoney = 0;
    public string currentRegionId = "NA"; // 기본 시작 지역
    public List<OreStack> globalSpecialInventory = new(); // 지역 전용 재료(글로벌)

    // Regions
    public List<RegionProgress> regions = new();

    public RegionProgress GetOrCreateRegion(string regionId)
    {
        for (int i = 0; i < regions.Count; i++)
        {
            if (regions[i].regionId == regionId) return regions[i];
        }

        var rp = RegionProgress.CreateNew(regionId);
        regions.Add(rp);
        return rp;
    }

    public RegionProgress GetRegion(string regionId)
    {
        for (int i = 0; i < regions.Count; i++)
        {
            if (regions[i].regionId == regionId) return regions[i];
        }
        return null;
    }
}

[Serializable]
public class RegionProgress
{
    public string regionId;

    // 진행도
    public int depth = 0;                // 현재 깊이(타일 or m)
    public long minedTilesTotal = 0;     // 총 캔 타일(통계/해금조건)

    // 인력/장비
    public int workers = 0;
    public string equippedToolId = "HAND"; // HAND, TOY_SHOVEL, SHOVEL, PICKAXE...
    public int toolDurability = -1;        // HAND는 -1(무한) 같은 규칙 가능
    public int toolLevel = 1;

    // 소모품
    public int tntCount = 0;

    // (선택) 지역 내부 광물 보관이 필요하면 활성화
    // public List<OreStack> oreInventory = new();

    // 오프라인 수익 계산용 (UTC ticks)
    public long lastActiveUtcTicks = 0;

    public static RegionProgress CreateNew(string regionId)
    {
        return new RegionProgress
        {
            regionId = regionId,
            depth = 0,
            minedTilesTotal = 0,
            workers = 0,
            equippedToolId = "HAND",
            toolDurability = -1,
            toolLevel = 1,
            tntCount = 0,
            lastActiveUtcTicks = DateTime.UtcNow.Ticks
        };
    }
}

[Serializable]
public class OreStack
{
    public string oreId;
    public int amount;

    public OreStack(string oreId, int amount)
    {
        this.oreId = oreId;
        this.amount = amount;
    }
}
