using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterUi : MonoBehaviour
{
    public Image Avatar;
    public Image AvatarFrame;
    public Text MilitaryPower;
    public InputField Surname;
    public InputField Name;
    public InputField Nickname;
    public CharacterGender Gender;
    public InputField Sign;
    public CharacterGenderUi GenderUi;

    void Start()
    {
        GenderUi.OnNotifyChanged += NotifyGenderChanged;
    }

    private void NotifyGenderChanged(CharacterGender gender) => Gender = gender;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Off() => gameObject.SetActive(false);
}

public enum CharacterGender
{
    Female,
    Male
}