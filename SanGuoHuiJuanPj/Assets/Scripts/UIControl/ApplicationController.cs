using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    private void OnApplicationPause(bool focus)
    {
        if (focus)
        {
            Debug.Log("切出游戏，OnApplicationPause(true)");
        }
        else
        {
            Debug.Log("切出游戏再次进入，OnApplicationPause(false)");
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Debug.Log("暂停状态后进入游戏内，OnApplicationFocus(true)");
        }
        else
        {
            Debug.Log("切出游戏画面进入暂停状态，OnApplicationFocus(false)");
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("正常退出游戏,OnApplicationQuit()");
    }
}