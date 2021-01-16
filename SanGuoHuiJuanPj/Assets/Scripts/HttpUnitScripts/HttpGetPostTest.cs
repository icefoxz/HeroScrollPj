using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HttpGetPostTest : MonoBehaviour
{
    [SerializeField]
    InputField InputField;

    public async void GetFun()
    {
        string inputStr = InputField.text;
        Debug.Log(inputStr);
        string replyStr = await Http.GetAsync(inputStr);
        Debug.Log(replyStr);
    }
}