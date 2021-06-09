using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterGenderUi:MonoBehaviour
{
    public event Action<CharacterGender> OnNotifyChanged;
    public CharacterGender Gender { get; }
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

    void Start() => SetGender(CharacterGender.Female);

    public void SetGender(CharacterGender gender)
    {
        foreach (var item in ToggleSet) item.Value.isOn = item.Key == gender;
        OnNotifyChanged?.Invoke(gender);
    }
}