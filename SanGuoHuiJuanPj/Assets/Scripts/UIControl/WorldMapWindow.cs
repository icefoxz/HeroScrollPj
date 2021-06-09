using System;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapWindow : MonoBehaviour
{
    private enum Switches
    {
        Character,
        World
    }
    public CityInfoUi CityInfo;
    public PlayerCharacterUi PlayerCharacter;
    public WorldMapUi WorldMap;
    public Button CharacterButton;
    public Button WorldButton;

    void Start()
    {
        CharacterButton.onClick.AddListener(()=>OnSwitch(Switches.Character));
        WorldButton.onClick.AddListener(()=>OnSwitch(Switches.World));
    }

    private void OnSwitch(Switches state)
    {
        switch (state)
        {
            case Switches.Character:
                WorldMap.Off();
                PlayerCharacter.Show();
                break;
            case Switches.World:
                PlayerCharacter.Off();
                WorldMap.Show();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}