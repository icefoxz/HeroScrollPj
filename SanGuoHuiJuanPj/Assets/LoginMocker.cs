using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginMocker : MonoBehaviour
{
    public SignInUi Ui;

    public void EmptyAction()
    {
        Ui.IsReadyAction();
    }

    public void SetMode(int mode) => Ui.SetMode((SignInUi.Modes) mode);
}
