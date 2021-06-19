using System;
using Assets;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCharacterUi : MonoBehaviour
{
    public Image Avatar;
    public Image AvatarFrame;
    public Text MilitaryPower;
    public InputField Name;
    public InputField Nickname;
    public InputField Sign;
    public CharacterGenderUi GenderUi;
    public Button CloseButton;
    public Button SubmitButton;
    public CharacterDto Character { get; private set; }
    private bool isInit = false;

    private bool IsCharacterValid =>
        !string.IsNullOrWhiteSpace(Character.Name) && !string.IsNullOrWhiteSpace(Character.Nickname);

    void Start()
    {
        if(isInit)return;
        Character = new CharacterDto();
        GenderUi.OnNotifyChanged += value =>
        {
            var id = (int) value;
            Character.Gender = id;
            SetAvatar(id);
        };
        Name.onValueChanged.AddListener(value=> Character.Name = value);
        Nickname.onValueChanged.AddListener(value => Character.Nickname = value);
        Sign.onValueChanged.AddListener(value => Character.Sign = value);
        CloseButton.onClick.AddListener(()=>gameObject.SetActive(false));
        SubmitButton.onClick.AddListener(ReqCreateCharacter);
        gameObject.SetActive(false);
        isInit = true;
    }

    private void CheckInit()
    {
        if(!isInit)Start();
    }

    public void SetAvatar(int id)
    {
        CheckInit();
        Character.Avatar = id;
        if (GameResources.Instance != null)
            Avatar.sprite = GameResources.Instance.Avatar[id];
    }

    public void Show()
    {
        CheckInit();
        gameObject.SetActive(true);
        SetCharacter();
    }

    private void SetCharacter()
    {
        var cha = PlayerDataForGame.instance.Character;
        if (cha != null)
        {
            Name.text = cha.Name;
            Nickname.text = cha.Nickname;
            Sign.text = cha.Sign;
            GenderUi.SetGender((CharacterGender) cha.Gender);
        }

        SetAvailability(cha == null);
    }

    public void Off() => gameObject.SetActive(false);

    public void ReqCreateCharacter()
    {
        CheckInit();
        if (IsCharacterValid)
        {
            PlayerDataForGame.instance.Character = global::Character.Instance(Character);
            ApiPanel.instance.Invoke(OnCreateCharacterSuccess, PlayerDataForGame.instance.ShowStringTips,
                EventStrings.Req_CreateCharacter, ViewBag.Instance().PlayerCharacterDto(Character), false);
            return;
        }

        CheckEntry(Name);
        CheckEntry(Nickname);
    }

    private void CheckEntry(InputField inputField)
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            var text = inputField.placeholder.GetComponent<Text>();
            text.text = "请输入";
            text.color = Color.red;
        }
    }

    private void OnCreateCharacterSuccess(ViewBag vb)
    {
        Show();
        UIManager.instance.RefreshPlayerInfoUi();
    }

    private void SetAvailability(bool enable)
    {
        SubmitButton.enabled = enable;
        SubmitButton.gameObject.SetActive(enable);
        Name.interactable = enable;
        Nickname.interactable = enable;
        Sign.interactable = enable;
        GenderUi.SetAvailability(enable);
    }
}

public enum CharacterGender
{
    Female,
    Male
}