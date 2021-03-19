using System.Collections;
using System.Collections.Generic;
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
    }

    public void Disconnect() => Client.Disconnect();
}
