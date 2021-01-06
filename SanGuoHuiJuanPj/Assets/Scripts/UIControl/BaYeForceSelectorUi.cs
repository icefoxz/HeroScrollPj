using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaYeForceSelectorUi : MonoBehaviour
{
    public Button button;
    public Image image;
    public ForceFlagUI forceFlag;

    public bool displayLing;
    // Start is called before the first frame update
    public void Start() => DisplayLing(false);
    public void DisplayLing(bool yes)
    {
        displayLing = yes;
        image.gameObject.SetActive(yes);
    }
}
