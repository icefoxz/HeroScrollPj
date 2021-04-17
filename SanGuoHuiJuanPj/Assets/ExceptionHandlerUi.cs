using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExceptionHandlerUi : MonoBehaviour
{
    public Text ExceptionMessage;
    public Text StackTrace;

    public void Init()
    {
        XDebug.SubscribeHandler(this);
    }

    public void OnLogReceived(string condition, string stacktrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            gameObject.SetActive(true);
            ExceptionMessage.text = condition;
            StackTrace.text = stacktrace;
            Time.timeScale = 0;
        }
    }

    public void ApplicationQuit() => Application.Quit();
}
