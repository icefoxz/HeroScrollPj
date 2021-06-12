using CorrelateLib;
using UnityEngine;
using UnityEngine.UI;

public class CityCharacterUi : MonoBehaviour
{
    public Button UiBtn;
    public Text Name;

    public void Set(ICharacter character)
    {
        Name.text = character.Name;
    }
}