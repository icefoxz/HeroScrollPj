using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using UnityEngine;

 public class DataTable : MonoBehaviour
{
    private const string TableSuffix = "Table";
    private const string DataSuffix = "Data";
    private static Type IntType = typeof(int);
    private static Type StringType = typeof(string);
    private static Type DataType = typeof(Dictionary<int, IReadOnlyList<string>>);
    private static Type TextAssetType = typeof(TextAsset);
    private static Type ArrayType = typeof(Array);

    public static DataTable instance;
    private static TableData Data { get; set; }

    public static IReadOnlyDictionary<int, PlayerInitialConfigTable> PlayerInitialConfig=>Data.PlayerInitialConfigSet;
    public static IReadOnlyDictionary<int, ResourceConfigTable> ResourceConfig=>          Data.ResourceConfigSet;
    public static IReadOnlyDictionary<int, HeroTable> Hero=>                              Data.HeroSet;
    public static IReadOnlyDictionary<int, PlayerLevelConfigTable> PlayerLevelConfig=>    Data.PlayerLevelConfigSet;
    public static IReadOnlyDictionary<int, TowerTable> Tower=>                            Data.TowerSet;
    public static IReadOnlyDictionary<int, MilitaryTable> Military=>                      Data.MilitarySet;
    public static IReadOnlyDictionary<int, CardLevelTable> CardLevel=>                    Data.CardLevelSet;
    public static IReadOnlyDictionary<int, TrapTable> Trap=>                              Data.TrapSet;
    public static IReadOnlyDictionary<int, WarChestTable> WarChest=>                      Data.WarChestSet;
    public static IReadOnlyDictionary<int, WarTable> War=>                                Data.WarSet;
    public static IReadOnlyDictionary<int, BaseLevelTable> BaseLevel=>                    Data.BaseLevelSet;
    public static IReadOnlyDictionary<int, CheckpointTable> Checkpoint=>                  Data.CheckpointSet;
    public static IReadOnlyDictionary<int, BattleEventTable> BattleEvent=>                Data.BattleEventSet;
    public static IReadOnlyDictionary<int, EnemyTable> Enemy=>                            Data.EnemySet;
    public static IReadOnlyDictionary<int, EnemyUnitTable> EnemyUnit=>                    Data.EnemyUnitSet;
    public static IReadOnlyDictionary<int, QuestTable> Quest=>                            Data.QuestSet;
    public static IReadOnlyDictionary<int, QuestRewardTable> QuestReward=>                Data.QuestRewardSet;
    public static IReadOnlyDictionary<int, MercenaryTable> Mercenary=>                    Data.MercenarySet;
    public static IReadOnlyDictionary<int, GameModeTable> GameMode=>                      Data.GameModeSet;
    public static IReadOnlyDictionary<int, GuideTable> Guide=>                            Data.GuideSet;
    public static IReadOnlyDictionary<int, TipsTable> Tips=>                              Data.TipsSet;
    public static IReadOnlyDictionary<int, RCodeTable> RCode=>                            Data.RCodeSet;
    public static IReadOnlyDictionary<int, ChickenTable> Chicken=>                        Data.ChickenSet;
    public static IReadOnlyDictionary<int, StaticArrangementTable> StaticArrangement=>    Data.StaticArrangementSet;
    public static IReadOnlyDictionary<int, TextTable> Text=>                              Data.TextSet;
    public static IReadOnlyDictionary<int, NumericalConfigTable> NumericalConfig=>        Data.NumericalConfigSet;
    public static IReadOnlyDictionary<int, JiBanTable> JiBan=>                            Data.JiBanSet;
    public static IReadOnlyDictionary<int, ForceTable> Force=>                            Data.ForceSet;
    public static IReadOnlyDictionary<int, BaYeCityTable> BaYeCity=>                      Data.BaYeCitySet;
    public static IReadOnlyDictionary<int, BaYeCityEventTable> BaYeCityEvent=>            Data.BaYeCityEventSet;
    public static IReadOnlyDictionary<int, BaYeLevelMappingTable> BaYeLevelMapping=>      Data.BaYeLevelMappingSet;
    public static IReadOnlyDictionary<int, BaYeTaskTable> BaYeTask=>                      Data.BaYeTaskSet;
    public static IReadOnlyDictionary<int, BaYeStoryPoolTable> BaYeStoryPool=>            Data.BaYeStoryPoolSet;
    public static IReadOnlyDictionary<int, BaYeStoryEventTable> BaYeStoryEvent=>          Data.BaYeStoryEventSet;
    public static IReadOnlyDictionary<int, BaYeTvTable> BaYeTv=>                          Data.BaYeTvSet;
    public static IReadOnlyDictionary<int, BaYeNameTable> BaYeName=>                      Data.BaYeNameSet;

    public TextAsset PlayerInitialConfigTable;
    public TextAsset ResourceConfigTable;
    public TextAsset HeroTable;
    public TextAsset PlayerLevelConfigTable;
    public TextAsset TowerTable;
    public TextAsset MilitaryTable;
    public TextAsset CardLevelTable;
    public TextAsset TrapTable;
    public TextAsset WarChestTable;
    public TextAsset WarTable;
    public TextAsset BaseLevelTable;
    public TextAsset CheckpointTable;
    public TextAsset BattleEventTable;
    public TextAsset EnemyTable;
    public TextAsset EnemyUnitTable;
    public TextAsset QuestTable;
    public TextAsset QuestRewardTable;
    public TextAsset MercenaryTable;
    public TextAsset GameModeTable;
    public TextAsset GuideTable;
    public TextAsset TipsTable;
    public TextAsset RCodeTable;
    public TextAsset ChickenTable;
    public TextAsset StaticArrangementTable;
    public TextAsset TextTable;
    public TextAsset NumericalConfigTable;
    public TextAsset JiBanTable;
    public TextAsset ForceTable;
    public TextAsset BaYeCityTable;
    public TextAsset BaYeCityEventTable;
    public TextAsset BaYeLevelMappingTable;
    public TextAsset BaYeTaskTable;
    public TextAsset BaYeStoryPoolTable;
    public TextAsset BaYeStoryEventTable;
    public TextAsset BaYeTvTable;
    public TextAsset BaYeNameTable;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        var type = GetType();
        var data = type.GetFields().Where(f => f.FieldType == TextAssetType)
            .ToDictionary(p => p.Name.Replace(TableSuffix,string.Empty).ToLower(), p => ConvertText(p.GetValue(this).ToString()));
        Data = new TableData(data);
        //var staticData = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(f => f.FieldType == DataType)
        //    .ToDictionary(p => p.Name.Replace(DataSuffix, string.Empty).ToLower(), p => p);
        //textAssets.Join(staticData,tx=>tx.Key,st=>st.Key, (tx, st) =>new {Asset = tx.Value, Data = st.Value}).ToList().ForEach(
        //    map =>
        //    {
        //        var textAsset = (TextAsset) map.Asset.GetValue(this);
        //        map.Data.SetValue(null,ConvertText(textAsset));
        //    });
        //var tableData = new TableData(new Dictionary<string, Dictionary<int, IReadOnlyList<string>>>());
    }

    private Dictionary<int, IReadOnlyList<string>> ConvertText(string text)
    {
        text = text.Replace(@"\\", @"\");
        return Json.DeserializeList<List<string>>(text).ToDictionary(row => int.Parse(row[0]), row =>row as IReadOnlyList<string>);
    }

    /// <summary>
    /// 根据id获取文本内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetStringText(int id) => Text[id].Text;

    /// <summary>
    /// 根据id获取游戏数值内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static int GetGameValue(int id) => NumericalConfig[id].Value;

}