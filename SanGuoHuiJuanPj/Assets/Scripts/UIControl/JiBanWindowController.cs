using System.Linq;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JiBanWindowController : MonoBehaviour
{
    public JiBanButtonUi BtnPrefab;
    public JiBanWindowUi JiBanWindowPrefab;
    public Transform BtnContent;
    public Transform JiBanWindowsParent;
    public Button CloseButton;
    private ComponentStateSwitch<int, JiBanWindowUi> winMapper = new ComponentStateSwitch<int, JiBanWindowUi>();

    public void Init()
    {
        BtnPrefab.gameObject.SetActive(false);
        JiBanWindowPrefab.gameObject.SetActive(false);
        SetCloseButton(Off);
        var jiBans = DataTable.JiBan.Values.Where(j => j.IsOpen > 0);
        foreach (var jiBan in jiBans)
        {
            var win = NewJiBanWindow(jiBan.Id);
            var btn = NewJiBanBtn(jiBan.Id, () => ShowJiBan(jiBan.Id));
            btn.gameObject.SetActive(true);
            winMapper.Add(jiBan.Id, win);
        }
    }

    private void SetCloseButton(UnityAction action)
    {
        CloseButton.onClick.RemoveAllListeners();
        CloseButton.onClick.AddListener(action);
    }

    private JiBanButtonUi NewJiBanBtn(int id,UnityAction onclickAction)
    {
        var btn = Instantiate(BtnPrefab, BtnContent);
        btn.TitleImage.sprite = GameResources.Instance.JiBanVText[id];
        btn.Button.onClick.RemoveAllListeners();
        btn.Button.onClick.AddListener(onclickAction);
        return btn;
    }

    private JiBanWindowUi NewJiBanWindow(int id)
    {
        var ui = Instantiate(JiBanWindowPrefab, JiBanWindowsParent);
        ui.Set(id);
        return ui;
    }

    public void Show() => gameObject.SetActive(true);

    public void ShowJiBan(int id)
    {
        BtnContent.gameObject.SetActive(false);
        winMapper.Set(id);
        SetCloseButton(() =>
        {
            winMapper.Set(-1);
            BtnContent.gameObject.SetActive(true);
            SetCloseButton(Off);
        });
    }

    public void Off()
    {
        winMapper.Set(-1);
        gameObject.SetActive(false);
    }
}