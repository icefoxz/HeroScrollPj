using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryEffect : MonoBehaviour
{
    [SerializeField]
    float existTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, existTime);
    }
}
