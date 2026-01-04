using UnityEngine;

public class SaveBootstrap : MonoBehaviour
{
    private void Start()
    {
        // SaveManager 오브젝트가 씬에 존재한다고 가정
        SaveManager.I.LoadOrCreate();
    }

    // 앱이 백그라운드/종료될 때 저장 (모바일 방치형 필수)
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveManager.I.Save();
        }
    }

    private void OnApplicationQuit()
    {
        SaveManager.I.Save();
    }
}
