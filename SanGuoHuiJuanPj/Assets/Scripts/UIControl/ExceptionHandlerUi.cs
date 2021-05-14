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
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                gameObject.SetActive(true);
                ExceptionMessage.text = condition;
                StackTrace.text = $"v+{Application.version}\n{stacktrace}";
                Time.timeScale = 0;
                break;
        }
    }

    public void ApplicationQuit() => Application.Quit();
}
