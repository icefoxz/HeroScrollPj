using System;
using System.Collections.Generic;
using Assets;
using CorrelateLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerCharacterUi : MonoBehaviour
{
    private enum States
    {
        NewCharacter,
        Registered
    }
    public Image Avatar;
    public Image AvatarFrame;
    public Text MilitaryPower;
    public InputField Name;
    public InputField Nickname;
    public InputField Sign;
    public CharacterGenderUi GenderUi;
    public Button CloseButton;
    public Button SubmitButton;
    public ApiConsumeUi UpdateNameApi;
    public ApiConsumeUi UpdateNicknameApi;
    public ApiConsumeUi UpdateGenderApi;
    public ApiConsumeUi UpdateSignApi;
    public Character Character { get; private set; }
    private List<ApiConsumeUi> consumeUis;

    private bool isInit = false;
    private States state;

    public void Init()
    {
        if(isInit)return;
        consumeUis = new List<ApiConsumeUi> { UpdateNameApi, UpdateNicknameApi, UpdateGenderApi, UpdateGenderApi, UpdateSignApi };
        consumeUis.ForEach(ui=>ui.Init());
        CloseButton.onClick.AddListener(()=>gameObject.SetActive(false));
        gameObject.SetActive(false);
        InitCharacter();
        isInit = true;
    }

    private void ResetApiUpdateUis()
    {
        var isCharacterValid = Character != null && Character.IsValidCharacter();
        consumeUis.ForEach(ui => ui.gameObject.SetActive(isCharacterValid));
        if (!isCharacterValid) return;
        UpdateNameApi.SetEditInteractive(new[] { Name });
        UpdateNameApi.SetCost(ApiConsumeUi.Currencies.YvQue, 100);
        UpdateNameApi.ResetApi(() => !string.IsNullOrWhiteSpace(Name.text) && !Name.text.Equals(Character.Name),
            EventStrings.Req_UpdateCharacterName, OnSuccessUpdateCharacter);
        UpdateNicknameApi.SetEditInteractive(new[] { Nickname });
        UpdateNicknameApi.SetCost(ApiConsumeUi.Currencies.YvQue, 100);
        UpdateNicknameApi.ResetApi(
            () => !string.IsNullOrWhiteSpace(Nickname.text) && !Nickname.text.Equals(Character.Nickname),
            EventStrings.Req_UpdateCharacterNickname, OnSuccessUpdateCharacter);
        UpdateGenderApi.SetEditInteractive(new[] { GenderUi.Male, GenderUi.Female });
        UpdateGenderApi.SetCost(ApiConsumeUi.Currencies.YvQue, 100);
        UpdateGenderApi.ResetApi(() => (int) GenderUi.Gender != Character.Gender,
            EventStrings.Req_UpdateCharacterGender, OnSuccessUpdateCharacter);
        UpdateSignApi.SetEditInteractive(new[] { Sign });
        UpdateSignApi.SetCost(ApiConsumeUi.Currencies.YuanBao, 200);
        UpdateSignApi.ResetApi(() => !string.IsNullOrWhiteSpace(Sign.text) && Sign.text.Equals(Character.Sign),
            EventStrings.Req_UpdateCharacterSign, OnSuccessUpdateCharacter);
    }

    private void OnSuccessUpdateCharacter(ViewBag vb)
    {
        var cha = vb.GetPlayerCharacterDto();
        PlayerDataForGame.instance.UpdateCharacter(cha);
        ResetApiUpdateUis();
    }

    private void SetAvatar(int id)
    {
        if (GameResources.Instance != null)
            Avatar.sprite = GameResources.Instance.Avatar[id];
    }

    public void Show()
    {
        gameObject.SetActive(true);
        InitCharacter();
    }

    private void InitCharacter()
    {
        if (Character == null)
        {
            Character = PlayerDataForGame.instance.Character;
            state = Character == null ? States.NewCharacter : States.Registered;
        }

        GenderUi.OnNotifyChanged.RemoveAllListeners();
        GenderUi.OnNotifyChanged.AddListener(g => SetAvatar((int) g));
        SetState();
        if (Character != null)
        {
            Name.text = Character.Name;
            Nickname.text = Character.Nickname;
            Sign.text = Character.Sign;
            GenderUi.SetGender((CharacterGender) Character.Gender);
        }
        ResetApiUpdateUis();
    }

    private void SetState()
    {
        SubmitButton.onClick.RemoveAllListeners();
        SubmitButton.onClick.AddListener(ReqCreateCharacter);
        var isNewCharacter = state == States.NewCharacter;
        SubmitButton.enabled = isNewCharacter;
        SubmitButton.gameObject.SetActive(isNewCharacter);
        Name.interactable = isNewCharacter;
        Nickname.interactable = isNewCharacter;
        Sign.interactable = isNewCharacter;
        GenderUi.SetAvailability(isNewCharacter);
        if(state == States.Registered) ResetApiUpdateUis();
    }

    public void Off() => gameObject.SetActive(false);

    private void ReqCreateCharacter()
    {
        Character.Name = Name.text;
        Character.Nickname = Nickname.text;
        Character.Gender = (int)GenderUi.Gender;
        Character.Sign = Sign.text;
        if (Character.IsValidCharacter())//完整信息才请求
        {
            PlayerDataForGame.instance.Character = global::Character.Instance(Character);
            ApiPanel.instance.Invoke(OnCreateCharacterSuccess, PlayerDataForGame.instance.ShowStringTips,
                EventStrings.Req_CreateCharacter, ViewBag.Instance().PlayerCharacterDto(Character.ToDto()), false);
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
}

public enum CharacterGender
{
    Female,
    Male
}