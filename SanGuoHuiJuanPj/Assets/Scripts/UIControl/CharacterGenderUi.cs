using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterGenderUi : MonoBehaviour
{
    private class GenderEvent : UnityEvent<CharacterGender>
    {
    }

    public UnityEvent<CharacterGender> OnNotifyChanged = new GenderEvent();
    public CharacterGender Gender { get; private set; }
    public Toggle Male;
    public Toggle Female;
    private Dictionary<CharacterGender, Toggle> toggleSet;

    private Dictionary<CharacterGender, Toggle> ToggleSet
    {
        get
        {
            if (toggleSet == null)
                toggleSet = new Dictionary<CharacterGender, Toggle>
                {
                    {CharacterGender.Male, Male},
                    {CharacterGender.Female, Female}
                };
            return toggleSet;
        }
    }

    public void Init()
    {
        Male.onValueChanged.AddListener(isOn => SetGender(isOn ? CharacterGender.Male : CharacterGender.Female));
        Female.onValueChanged.AddListener(isOn => SetGender(isOn ? CharacterGender.Female : CharacterGender.Male));
        SetGender(CharacterGender.Female);
    }

    public void SetAvailability(bool enable)
    {
        Male.interactable = enable;
        Female.interactable = enable;
    }

    public void SetGender(CharacterGender gender)
    {
        Gender = gender;
        foreach (var item in ToggleSet) item.Value.isOn = item.Key == gender;
        OnNotifyChanged?.Invoke(gender);
    }
}