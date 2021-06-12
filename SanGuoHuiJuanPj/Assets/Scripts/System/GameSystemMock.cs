public class GameSystemMock : GameSystem
{
    void Start()
    {
        Init();
        if(UIManager.instance!=null) InitEnqueue(UIManager.instance.Init);
    }
}