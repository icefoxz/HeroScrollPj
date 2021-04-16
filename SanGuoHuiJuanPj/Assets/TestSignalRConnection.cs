using System.Collections;
using System.Collections.Generic;
using CorrelateLib;
using UnityEngine;

public class TestSignalRConnection : MonoBehaviour
{
    public string Username;
    public string Password;
    public SignalRClient Client;

    public void Login()
    {
        Client.Login((isSuccess, code) => Debug.Log($"SignalR连接: {isSuccess}, StatusCode[{code}]!"), Username,
            Password);
        var player = new PlayerData {Exp = 100};
        PlayerDataForGame.instance.pyData = player;
    }

    public async void TestRequest()
    {
        //var card = new GameCardDto(14, GameCardType.Hero, 3, 2);
        //var result = await Client.Invoke("Req_CardSell", new object[] {14, 0, Json.Serialize(card)});
        await Client.SynchronizePlayerData();
        //XDebug.Log<TestSignalRConnection>(result);
    }

    public void Disconnect() => Client.Disconnect();

}
