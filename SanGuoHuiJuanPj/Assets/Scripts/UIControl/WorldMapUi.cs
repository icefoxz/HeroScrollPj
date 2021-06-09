using UnityEngine;
using UnityEngine.UI;

public class WorldMapUi : MonoBehaviour
{
    public WorldMapCityUi[] Cities;
    public Button MigrateButton;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Off()
    {
        gameObject.SetActive(false);
    }
}