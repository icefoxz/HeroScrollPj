public class GameSystemMock : GameSystem
{
    void Start()
    {
        Init();
        InitEnqueue(UIManager.instance.Init);
    }
}