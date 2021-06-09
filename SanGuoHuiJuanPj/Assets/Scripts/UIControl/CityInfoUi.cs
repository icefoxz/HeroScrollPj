using System.Collections.Generic;
using System.Linq;
using CorrelateLib;
using UnityEngine;
using UnityEngine.UI;

public class CityInfoUi : MonoBehaviour
{
    public int Id;
    public Text Master;
    public Text Level;
    public Text MilitaryPower;
    public Text Population;
    public Text Intro;
    public WorldMapCityUi CityUi;
    public CityCharacterUi[] Characters;
    public CityCharacterUi CharacterPrefab;
    public Transform Content;

    public void Init()
    {
        CharacterPrefab.gameObject.SetActive(false);
    }

    //public void SetCity(int cityId) => CityUi.Set(cityId, OnCityClicked);

    

    public void SetInfo(string master,int level,int population,int militaryPower,string intro)
    {
        Master.text = master;
        Level.text = level.ToString();
        Population.text = population.ToString();
        MilitaryPower.text = militaryPower.ToString();
        Intro.text = intro;
    }

    public void SetCharacters(IEnumerable<ICharacter> characters)
    {
        if (Characters.Length > 0)
        {
            foreach (var ui in Characters) Destroy(ui.gameObject);
        }

        Characters = characters.Select(c =>
        {
            var obj = Instantiate(CharacterPrefab, Content);
            obj.Set(c);
            return obj;
        }).ToArray();
    }
}