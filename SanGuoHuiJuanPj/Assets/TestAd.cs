using System;
using System.Collections;
using System.Collections.Generic;
using Donews.mediation;
using UnityEngine;
using UnityEngine.UI;

public class TestAd : MonoBehaviour
{
    public Text statusText;

    public AdmobController controller;

    void Update()
    {
        if(!controller)return;
        statusText.text = controller.Status.ToString();
    }

    //public void Show() => controller.RequestShow();
    //
    public void Load() => AdmobAgent.Instance.CallAd(success=>Debug.Log($"Ad call reward = {success}"));

}
