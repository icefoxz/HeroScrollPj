using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using wode.HTTP;

public class HttpGetPostTest : MonoBehaviour
{
    [SerializeField]
    InputField InputField;

    public void GetFun()
    {
        string inputStr = InputField.text;
        Debug.Log(inputStr);
        string replyStr = HttpUitls.Get(inputStr);
        Debug.Log(replyStr);
    }
}