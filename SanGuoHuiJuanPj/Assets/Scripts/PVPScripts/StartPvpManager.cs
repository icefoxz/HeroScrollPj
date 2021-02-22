//using DG.Tweening;
//using UnityEngine;
//using UnityEngine.UI;

//public class StartPvpManager : MonoBehaviour
//{
//    public static StartPvpManager instance;

//    [SerializeField]
//    Button startBtn;

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//    }

//    private void Start()
//    {
//        startBtn.onClick.AddListener(delegate() {
//            FightManagerForPvp.instance.InitEnemyCardForFight();
//        });
//    }
//}