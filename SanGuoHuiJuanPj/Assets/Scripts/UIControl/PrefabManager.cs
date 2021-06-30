using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    #region Props
    [SerializeField] GameCardUi gameCardUi;
    public static GameCardUi GameCardUi { get; private set; }
    #endregion

    public void Init()
    {
        Instance = this;
        GameCardUi = gameCardUi;
    }

    public static GameCardUi NewGameCardUi(Transform parent) => Instantiate(GameCardUi, parent);
}