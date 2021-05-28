using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CoObj : MonoBehaviour
{
    private UnityAction action;
    public void Set(UnityAction action)
    {
        this.action = action;
    }

    public void StartAction()
    {
        StartCoroutine(InvokeAction());
    }

    private IEnumerator InvokeAction()
    {
        action?.Invoke();
        yield return null;
    }
}