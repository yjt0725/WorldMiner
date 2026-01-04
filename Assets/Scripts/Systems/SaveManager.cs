using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager I { get; private set; }

    [Header("Debug")]
    public bool prettyPrintJson = true;

    public SaveData Data { get; private set; }

    private string _savePath;

    // 버전 상수 (업데이트 시 올리기)
    private const int CURRENT_VERSION = 1;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        _savePath = Path.Combine(Application.persistentDataPath, "save.json");
        Debug.Log(Application.persistentDataPath);

    }

    /// <summary>
    /// 게임 시작 시 호출: 파일이 있으면 로드, 없으면 새로 생성
    /// </summary>
    public void LoadOrCreate()
    {
        if (!File.Exists(_savePath))
        {
            CreateNewSave();
            Save(); // 파일 생성
            return;
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            var loaded = JsonUtility.FromJson<SaveData>(json);

            if (loaded == null)
            {
                // 파싱 실패(비정상)
                CreateNewSave();
                Save();
                return;
            }

            Data = loaded;

            // 버전 마이그레이션(필요 시)
            MigrateIfNeeded();

            // 필수 값 보정(깨진 세이브 방지)
            Normalize();

            // 로드 성공 후 한 번 저장해서 정규화 반영(선택)
            Save();
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Load failed: {e.Message}\n{e.StackTrace}");
            // 로드 실패 시 새 세이브로 대체 (너무 공격적이면 백업 후 생성도 가능)
            CreateNewSave();
            Save();
        }
    }

    /// <summary>
    /// 즉시 저장
    /// </summary>
    public void Save()
    {
        if (Data == null) CreateNewSave();

        try
        {
            Data.version = CURRENT_VERSION;

            string json = JsonUtility.ToJson(Data, prettyPrintJson);

            // 안정성을 위해 임시 파일에 쓰고 교체 (저장 도중 앱 종료 대비)
            string tmpPath = _savePath + ".tmp";
            File.WriteAllText(tmpPath, json);
            File.Copy(tmpPath, _savePath, true);
            File.Delete(tmpPath);

            // Debug.Log($"[SaveManager] Saved to {_savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Save failed: {e.Message}\n{e.StackTrace}");
        }
    }

    /// <summary>
    /// 새 세이브 생성
    /// </summary>
    public void CreateNewSave()
    {
        Data = new SaveData
        {
            version = CURRENT_VERSION,
            globalMoney = 0,
            currentRegionId = "NA",
            globalSpecialInventory = new(),
            regions = new()
        };

        // 시작 지역 진행도 생성
        Data.GetOrCreateRegion(Data.currentRegionId);
    }

    /// <summary>
    /// 세이브 파일 삭제(디버그용)
    /// </summary>
    public void WipeSave()
    {
        Data = null;
        try
        {
            if (File.Exists(_savePath)) File.Delete(_savePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Wipe failed: {e.Message}");
        }
        CreateNewSave();
        Save();
    }

    // --------------------
    // Versioning / Migration
    // --------------------

    private void MigrateIfNeeded()
    {
        if (Data.version == CURRENT_VERSION) return;

        // 예시: v0 -> v1 마이그레이션
        // if (Data.version == 0) { ... Data.version = 1; }

        // 지금은 버전 1만 존재한다고 가정
        Data.version = CURRENT_VERSION;
    }

    // --------------------
    // Safety Normalize
    // --------------------

    private void Normalize()
    {
        if (Data.currentRegionId == null || Data.currentRegionId.Trim().Length == 0)
            Data.currentRegionId = "NA";

        if (Data.regions == null) Data.regions = new();
        if (Data.globalSpecialInventory == null) Data.globalSpecialInventory = new();

        // currentRegion이 regions에 없으면 생성
        Data.GetOrCreateRegion(Data.currentRegionId);

        // regions 내부 값 보정
        foreach (var r in Data.regions)
        {
            if (r == null) continue;

            if (r.regionId == null) r.regionId = "UNKNOWN";
            if (r.depth < 0) r.depth = 0;
            if (r.workers < 0) r.workers = 0;
            if (r.toolLevel < 1) r.toolLevel = 1;

            if (string.IsNullOrEmpty(r.equippedToolId))
                r.equippedToolId = "HAND";

            // lastActiveUtcTicks가 0이면 현재로 보정
            if (r.lastActiveUtcTicks <= 0)
                r.lastActiveUtcTicks = DateTime.UtcNow.Ticks;
        }
    }
}
