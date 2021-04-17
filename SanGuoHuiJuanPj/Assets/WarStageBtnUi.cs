using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WarStageBtnUi : MonoBehaviour
{
    public Button button;
    public Image selectedImage;
    public Text title;
    public Button boxButton;
    public Image unlockImage;
    public int boundWarId;

    public void SetUi(int warId, string titleText, UnityAction onClick)
    {
        ResetUi();
        boundWarId = warId;
        //战役列拼接
        title.text = titleText;
        button.onClick.AddListener(onClick);
    }

    public void SetChest(UnityAction onClickChestAction)
    {
        boxButton.gameObject.SetActive(true);
        boxButton.GetComponent<Animator>().enabled = true;
        boxButton.interactable = true;
        boxButton.onClick.RemoveAllListeners();
        boxButton.onClick.AddListener(onClickChestAction);
    }

    private void ResetChest()
    {
        boxButton.gameObject.SetActive(false);
        boxButton.GetComponent<Animator>().enabled = false;
        boxButton.interactable = false;
        boxButton.onClick.AddListener(() => PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(26)));
    }

    public void ResetUi()
    {
        boundWarId = -1;
        title.text = string.Empty;
        button.onClick.RemoveAllListeners();
        ResetChest();
    }
}
