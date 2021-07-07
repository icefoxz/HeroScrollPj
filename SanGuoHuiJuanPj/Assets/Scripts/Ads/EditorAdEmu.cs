using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditorAdEmu : MonoBehaviour
{
    public Button SuccessButton;
    public Button FailedButton;

    public void Set(UnityAction<bool,string> adAction)
    {
        SuccessButton.onClick.RemoveAllListeners();
        SuccessButton.onClick.AddListener(() =>
        {
            adAction(true,string.Empty);
            Off();
        });
        FailedButton.onClick.RemoveAllListeners();
        FailedButton.onClick.AddListener(() =>
        {
            adAction(false,string.Empty);
            Off();
        });
        gameObject.SetActive(true);
    }

    private void Off() => gameObject.SetActive(false);
}
