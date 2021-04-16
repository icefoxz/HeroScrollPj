using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApiPanel : MonoBehaviour
{
    public static ApiPanel instance;
    public static bool IsBusy { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        instance = this;
        SetBusy(false);
    }

    public async void Invoke(Func<Task> task)
    {
        SetBusy(true);
        try
        {
            await task.Invoke();
        }
        catch (Exception e)
        {
#if UNITY_EDITOR
            Debug.LogError(e);
            throw;
#endif
        }
        SetBusy(false);
    }

    private void SetBusy(bool busy)
    {
        gameObject.SetActive(busy);
        IsBusy = busy;
    }
}
