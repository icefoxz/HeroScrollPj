using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameSystem GameSystem;
    public TaoYuan TaoYuan;

    void Start()
    {
        GameSystem.Init();
        LoadSaveData.instance.LoadByJson();
        Invoke(nameof(StartTaoYuan),2);
    }

    void StartTaoYuan() => TaoYuan.Init();
}