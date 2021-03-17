using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalRMessageHandler : MonoBehaviour
{
    public Text notify;
    public void OnNotify(string notifyText)
    {
        notify.text = notifyText;
    }
}
