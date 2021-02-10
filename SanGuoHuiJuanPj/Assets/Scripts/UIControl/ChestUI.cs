using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    public Sprite opened;

    public Sprite closed;

    public Animator animator;

    public Button button;
    public Text Text;

    public void Opened()
    {
        button.image.color = new Color(1,1,1,1);
        button.interactable = false;
        button.image.sprite = opened;
        animator.enabled = false;
    }

    public void Ready()
    {
        button.image.color = new Color(1,1,1,1);
        button.interactable = true;
        button.image.sprite = closed;
        animator.enabled = true;
    }

    public void Disabled()
    {
        button.image.color = new Color(0.7f, 0.7f, 0.7f, 0.9f);
        button.interactable = false;
        button.image.sprite = closed;
        animator.enabled = false;
    }
}
