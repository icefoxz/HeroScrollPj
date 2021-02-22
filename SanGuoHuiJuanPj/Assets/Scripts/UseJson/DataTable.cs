 using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class DataTable : MonoBehaviour
{
    private const string TableSuffix = "Table";
    private const string DataSuffix = "Data";
    private static Type StringType = typeof(string);
    public static DataTable instance;
    private static Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> cache = new Dictionary<string, IReadOnlyList<IReadOnlyList<string>>>();

    private void ResolverInit()
    {
        playerInitialData = SpecificTableResolver(playerInitialData, typeof(PlayerInitialTable));
        assetData = SpecificTableResolver(assetData, typeof(AssetTable));
        heroData = SpecificTableResolver(heroData, typeof(HeroTable));
        playerLevelData = SpecificTableResolver(playerLevelData, typeof(PlayerLevelTable));
        soldierData = SpecificTableResolver(soldierData, typeof(SoldierTable));
        towerData = SpecificTableResolver(towerData, typeof(TowerTable));
        classData = SpecificTableResolver(classData, typeof(ClassTable));
        upGradeData = SpecificTableResolver(upGradeData, typeof(UpGradeTable));
        trapData = SpecificTableResolver(trapData, typeof(TrapTable));
        spellData = SpecificTableResolver(spellData, typeof(SpellTable));
        warChestData = SpecificTableResolver(warChestData, typeof(WarChestTable));
        warData = SpecificTableResolver(warData, typeof(WarTable));
        cityLevelData = SpecificTableResolver(cityLevelData, typeof(CityLevelTable));
        pointData = SpecificTableResolver(pointData, typeof(PointTable));
        battleEventData = SpecificTableResolver(battleEventData, typeof(BattleEventTable));
        enemyData = SpecificTableResolver(enemyData, typeof(EnemyTable));
        enemyUnitData = SpecificTableResolver(enemyUnitData, typeof(EnemyUnitTable));
        storyData = SpecificTableResolver(storyData, typeof(StoryTable));
        storyRData = SpecificTableResolver(storyRData, typeof(StoryRTable));
        testData = SpecificTableResolver(testData, typeof(TestTable));
        testRData = SpecificTableResolver(testRData, typeof(TestRTable));
        encounterData = SpecificTableResolver(encounterData, typeof(EncounterTable));
        shoppingData = SpecificTableResolver(shoppingData, typeof(ShoppingTable));
        choseWarData = SpecificTableResolver(choseWarData, typeof(ChoseWarTable));
        guideData = SpecificTableResolver(guideData, typeof(GuideTable));
        knowledgeData = SpecificTableResolver(knowledgeData, typeof(KnowledgeTable));
        rCodeData = SpecificTableResolver(rCodeData, typeof(RCodeTable));
        tiLiStoreData = SpecificTableResolver(tiLiStoreData, typeof(TiLiStoreTable));
        enemyBossData = SpecificTableResolver(enemyBossData, typeof(EnemyBOSSTable));
        stringTextData = SpecificTableResolver(stringTextData, typeof(StringTextTable));
        numParametersData = SpecificTableResolver(numParametersData, typeof(NumParametersTable));
        jiBanData = SpecificTableResolver(jiBanData, typeof(JiBanTable));
        shiLiData = SpecificTableResolver(shiLiData, typeof(ShiLiTable));
        baYeDiTuData = SpecificTableResolver(baYeDiTuData, typeof(BaYeDiTuTable));
        baYeShiJianData = SpecificTableResolver(baYeShiJianData, typeof(BaYeShiJianTable));
        baYeBattleData = SpecificTableResolver(baYeBattleData, typeof(BaYeBattleTable));
        baYeRenWuData = SpecificTableResolver(baYeRenWuData, typeof(BaYeRenWuTable));
        storyPoolData = SpecificTableResolver(storyPoolData, typeof(StoryPoolTable));
        storyIdData = SpecificTableResolver(storyIdData, typeof(StoryIdTable));
        baYeTvData = SpecificTableResolver(baYeTvData, typeof(BaYeTVTable));
        baYeNameData = SpecificTableResolver(baYeNameData, typeof(BaYeNameTable));
    }

    private static Dictionary<int, IReadOnlyList<string>> playerInitialData;
    private static Dictionary<int, IReadOnlyList<string>> assetData;
    private static Dictionary<int, IReadOnlyList<string>> heroData;
    private static Dictionary<int, IReadOnlyList<string>> playerLevelData;
    private static Dictionary<int, IReadOnlyList<string>> soldierData;
    private static Dictionary<int, IReadOnlyList<string>> towerData;
    private static Dictionary<int, IReadOnlyList<string>> classData;
    private static Dictionary<int, IReadOnlyList<string>> upGradeData;
    private static Dictionary<int, IReadOnlyList<string>> trapData;
    private static Dictionary<int, IReadOnlyList<string>> spellData;
    private static Dictionary<int, IReadOnlyList<string>> warChestData;
    private static Dictionary<int, IReadOnlyList<string>> warData;
    private static Dictionary<int, IReadOnlyList<string>> cityLevelData;
    private static Dictionary<int, IReadOnlyList<string>> pointData;
    private static Dictionary<int, IReadOnlyList<string>> battleEventData;
    private static Dictionary<int, IReadOnlyList<string>> enemyData;
    private static Dictionary<int, IReadOnlyList<string>> enemyUnitData;
    private static Dictionary<int, IReadOnlyList<string>> storyData;
    private static Dictionary<int, IReadOnlyList<string>> storyRData;
    private static Dictionary<int, IReadOnlyList<string>> testData;
    private static Dictionary<int, IReadOnlyList<string>> testRData;
    private static Dictionary<int, IReadOnlyList<string>> encounterData;
    private static Dictionary<int, IReadOnlyList<string>> shoppingData;
    private static Dictionary<int, IReadOnlyList<string>> choseWarData;
    private static Dictionary<int, IReadOnlyList<string>> guideData;
    private static Dictionary<int, IReadOnlyList<string>> knowledgeData;
    private static Dictionary<int, IReadOnlyList<string>> rCodeData;
    private static Dictionary<int, IReadOnlyList<string>> tiLiStoreData;
    private static Dictionary<int, IReadOnlyList<string>> enemyBossData;
    private static Dictionary<int, IReadOnlyList<string>> stringTextData;
    private static Dictionary<int, IReadOnlyList<string>> numParametersData;
    private static Dictionary<int, IReadOnlyList<string>> jiBanData;
    private static Dictionary<int, IReadOnlyList<string>> shiLiData;
    private static Dictionary<int, IReadOnlyList<string>> baYeDiTuData;
    private static Dictionary<int, IReadOnlyList<string>> baYeShiJianData;
    private static Dictionary<int, IReadOnlyList<string>> baYeBattleData;
    private static Dictionary<int, IReadOnlyList<string>> baYeRenWuData;
    private static Dictionary<int, IReadOnlyList<string>> storyPoolData;
    private static Dictionary<int, IReadOnlyList<string>> storyIdData;
    private static Dictionary<int, IReadOnlyList<string>> baYeTvData;
    private static Dictionary<int, IReadOnlyList<string>> baYeNameData;

    public TextAsset playerInitialTable;
    public TextAsset heroTable;
    public TextAsset playerLevelTable;
    public TextAsset assetTable;
    public TextAsset soldierTable;
    public TextAsset upGradeTable;
    public TextAsset warChestTable;
    public TextAsset pointTable;
    public TextAsset battleEventTable;
    public TextAsset enemyTable;
    public TextAsset enemyUnitTable;
    public TextAsset storyTable;
    public TextAsset storyRTable;
    public TextAsset testTable;
    public TextAsset testRTable;
    public TextAsset encounterTable;
    public TextAsset shoppingTable;
    public TextAsset choseWarTable;
    public TextAsset guideTable;
    public TextAsset knowledgeTable;
    public TextAsset rCodeTable;
    public TextAsset tiLiStoreTable;
    public TextAsset enemyBOSSTable;
    public TextAsset stringTextTable;
    public TextAsset numParametersTable;
    public TextAsset jiBanTable;
    public TextAsset shiLiTable;
    public TextAsset baYeDiTuTable;
    public TextAsset baYeShiJianTable;
    public TextAsset baYeBattleTable;
    public TextAsset baYeRenWuTable;
    public TextAsset storyPoolTable;
    public TextAsset storyIdTable;
    public TextAsset baYeTVTable;
    public TextAsset baYeNameTable;
    public TextAsset towerTable;
    public TextAsset classTable;
    public TextAsset trapTable;
    public TextAsset spellTable;
    public TextAsset warTable;
    public TextAsset cityLevelTable;

    public static IReadOnlyDictionary<int, IReadOnlyList<string>> PlayerInitial => playerInitialData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Asset         => assetData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Hero          => heroData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> PlayerLevel   => playerLevelData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Soldier       => soldierData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Tower         => towerData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Class         => classData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> UpGrade       => upGradeData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Trap          => trapData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Spell         => spellData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> WarChest      => warChestData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> War           => warData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> CityLevel     => cityLevelData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Point         => pointData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> BattleEvent   => battleEventData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Enemy         => enemyData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> EnemyUnit     => enemyUnitData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Story         => storyData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> StoryR        => storyRData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Test          => testData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> TestR         => testRData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Encounter     => encounterData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Shopping      => shoppingData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> ChoseWar      => choseWarData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Guide         => guideData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> Knowledge     => knowledgeData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> RCode         => rCodeData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> TiLiStore     => tiLiStoreData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> EnemyBoss     => enemyBossData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> StringText    => stringTextData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> NumParameters => numParametersData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> JiBan         => jiBanData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> ShiLi         => shiLiData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> BaYeDiTu      => baYeDiTuData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> BaYeShiJian   => baYeShiJianData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> BaYeBattle    => baYeBattleData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> BaYeRenWu     => baYeRenWuData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> StoryPool     => storyPoolData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> StoryId       => storyIdData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> BaYeTv        => baYeTvData;
    public static IReadOnlyDictionary<int, IReadOnlyList<string>> BaYeName      => baYeNameData;

    /// <summary>
    /// 势力初始数据
    /// </summary>
    public static IReadOnlyList<IReadOnlyList<string>> PlayerInitialData => ValueCache();

    /// <summary>                                                  
    /// 基础资源类型数据                                            
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> AssetData => ValueCache();
    /// <summary>                                                  
    /// 武将基础信息表数据                                           
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> HeroData => ValueCache();
    /// <summary>                                                  
    /// 玩家等级表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> PlayerLevelData => ValueCache();
    /// <summary>                                                  
    /// 士兵基础信息表数据                                           
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> SoldierData => ValueCache();
    /// <summary>                                                  
    /// 塔基础信息表数据                                            
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> TowerData => ValueCache();
    /// <summary>                                                  
    /// 兵种信息表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> ClassData => ValueCache();
    /// <summary>                                                  
    /// 升级碎片表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> UpGradeData => ValueCache();
    /// <summary>                                                  
    /// 辅助单位卡牌表数据                                           
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> TrapData => ValueCache();
    /// <summary>                                                  
    /// 辅助技能表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> SpellData => ValueCache();
    /// <summary>                                                  
    /// 宝箱表数据                                                  
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> WarChestData => ValueCache();
    /// <summary>                                                  
    /// 战役表数据                                                  
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> WarData => ValueCache();
    /// <summary>                                                  
    /// 城池等级表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> CityLevelData => ValueCache();
    /// <summary>                                                  
    /// 关卡表数据                                                  
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> PointData => ValueCache();
    /// <summary>                                                  
    /// 战斗事件表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> BattleEventData => ValueCache();
    /// <summary>                                                  
    /// 敌方位置表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> EnemyData => ValueCache();
    /// <summary>                                                  
    /// 敌方信息表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> EnemyUnitData => ValueCache();
    /// <summary>                                                  
    /// 非战斗事件-故事数据                                          
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> StoryData => ValueCache();
    /// <summary>                                                  
    /// 非战斗事件-故事奖励数据                                      
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> StoryRData => ValueCache();
    /// <summary>                                                  
    /// 非战斗事件-答题数据                                          
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> TestData => ValueCache();
    /// <summary>                                                  
    /// 非战斗事件-答题奖励数据                                      
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> TestRData => ValueCache();
    /// <summary>                                                  
    /// 非战斗事件-三选单位数据                                      
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> EncounterData => ValueCache();
    /// <summary>                                                  
    /// 非战斗事件-购买单位数据                                      
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> ShoppingData => ValueCache();
    /// <summary>                                                  
    /// 难度选择表数据                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> ChoseWarData => ValueCache();
    /// <summary>                                                  
    /// 新手引导配置表                                              
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> GuideData => ValueCache();
    /// <summary>                                                  
    /// 酒馆锦囊表                                                  
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> KnowledgeData => ValueCache();
    /// <summary>                                                  
    /// 兑换码表                                                    
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> RCodeData => ValueCache();
    /// <summary>                                                  
    /// 体力商店表                                                  
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> TiLiStoreData => ValueCache();

    /// <summary>                                                  
    /// 敌人BOSS固定布阵表                                          
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> EnemyBossData => ValueCache();
    /// <summary>                                                  
    /// 文本内容表                                                  
    /// </summary>                                                 
    private static IReadOnlyList<IReadOnlyList<string>> StringTextData => ValueCache();
    /// <summary>                                                  
    /// 游戏数值表                                                  
    /// </summary>                                                 
    private static IReadOnlyList<IReadOnlyList<string>> NumParametersData => ValueCache();
    /// <summary>                                                  
    /// 羁绊数据表                                                  
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> JiBanData => ValueCache();
    /// <summary>                                                  
    ///势力表                                                       
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> ShiLiData => ValueCache();
    /// <summary>                                                  
    ///霸业地图表                                                   
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> BaYeDiTuData => ValueCache();
    /// <summary>                                                  
    ///霸业事件表                                                   
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> BaYeShiJianData => ValueCache();
    /// <summary>                                                  
    ///霸业战役难度表                                               
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> BaYeBattleData => ValueCache();
    /// <summary>                                                  
    ///霸业任务表                                                   
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> BaYeRenWuData => ValueCache();
    /// <summary>                                                  
    /// 霸业故事点表                                                
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> StoryPoolData => ValueCache();
    /// <summary>                                                  
    ///霸业故事id表                                                 
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> StoryIdData => ValueCache();
    /// <summary>                                                  
    /// 霸业TV表                                                    
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> BaYeTvData => ValueCache();
    /// <summary>                                                  
    /// 霸业TV人名表                                                
    /// </summary>                                                 
    public static IReadOnlyList<IReadOnlyList<string>> BaYeNameData => ValueCache();

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
        playerInitialData = Json.DeserializeList<List<string>>(playerInitialTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        assetData = Json.DeserializeList<List<string>>(assetTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        heroData = Json.DeserializeList<List<string>>(heroTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        playerLevelData = Json.DeserializeList<List<string>>(playerLevelTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        soldierData = Json.DeserializeList<List<string>>(soldierTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        towerData = Json.DeserializeList<List<string>>(towerTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        classData = Json.DeserializeList<List<string>>(classTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        upGradeData = Json.DeserializeList<List<string>>(upGradeTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        trapData = Json.DeserializeList<List<string>>(trapTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        spellData = Json.DeserializeList<List<string>>(spellTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        warChestData = Json.DeserializeList<List<string>>(warChestTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        warData = Json.DeserializeList<List<string>>(warTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        cityLevelData = Json.DeserializeList<List<string>>(cityLevelTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        pointData = Json.DeserializeList<List<string>>(pointTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        battleEventData = Json.DeserializeList<List<string>>(battleEventTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        enemyData = Json.DeserializeList<List<string>>(enemyTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        enemyUnitData = Json.DeserializeList<List<string>>(enemyUnitTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        storyData = Json.DeserializeList<List<string>>(storyTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        storyRData = Json.DeserializeList<List<string>>(storyRTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        testData = Json.DeserializeList<List<string>>(testTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        testRData = Json.DeserializeList<List<string>>(testRTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        encounterData = Json.DeserializeList<List<string>>(encounterTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        shoppingData = Json.DeserializeList<List<string>>(shoppingTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        choseWarData = Json.DeserializeList<List<string>>(choseWarTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        guideData = Json.DeserializeList<List<string>>(guideTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        knowledgeData = Json.DeserializeList<List<string>>(knowledgeTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        rCodeData = Json.DeserializeList<List<string>>(rCodeTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        tiLiStoreData = Json.DeserializeList<List<string>>(tiLiStoreTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        enemyBossData = Json.DeserializeList<List<string>>(enemyBOSSTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        stringTextData = Json.DeserializeList<List<string>>(stringTextTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        numParametersData = Json.DeserializeList<List<string>>(numParametersTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        jiBanData = Json.DeserializeList<List<string>>(jiBanTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        shiLiData = Json.DeserializeList<List<string>>(shiLiTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        baYeDiTuData = Json.DeserializeList<List<string>>(baYeDiTuTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        baYeShiJianData = Json.DeserializeList<List<string>>(baYeShiJianTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        baYeBattleData = Json.DeserializeList<List<string>>(baYeBattleTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        baYeRenWuData = Json.DeserializeList<List<string>>(baYeRenWuTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        storyPoolData = Json.DeserializeList<List<string>>(storyPoolTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        storyIdData = Json.DeserializeList<List<string>>(storyIdTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        baYeTvData = Json.DeserializeList<List<string>>(baYeTVTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);
        baYeNameData = Json.DeserializeList<List<string>>(baYeNameTable.text).ToDictionary(row => int.Parse(row[0]), row => row as IReadOnlyList<string>);

        ResolverInit();
    }

    private static IReadOnlyList<IReadOnlyList<string>> ValueCache([CallerMemberName]string methodName = "")
    {
        if (cache.ContainsKey(methodName)) return cache[methodName];
        var type = typeof(DataTable);
        var fieldInfo = type.GetField(methodName.FirstCharToLower(), BindingFlags.NonPublic | BindingFlags.Static);
        var field = (Dictionary<int,IReadOnlyList<string>>)fieldInfo.GetValue(null);
        cache.Add(methodName,field.OrderBy(m=>m.Key).Select(m=>m.Value).ToList());
        return cache[methodName];
    }

    /// <summary>
    /// 根据id获取文本内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetStringText(int id) => StringText[id][1];

    /// <summary>
    /// 根据id获取游戏数值内容
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static int GetGameValue(int id) => int.Parse(NumParameters[id][1]);

    private static Dictionary<int,IReadOnlyList<string>> SpecificTableResolver(Dictionary<int,IReadOnlyList<string>> field, Type type)
    {
        var row = field.Values.First();
        var props = type.GetProperties();
        if (row.Count != props.Length)
            throw XDebug.Throw($"表类型[{type.Name}][{props.Length}]与表实例[{row.Count}]长度不一致！",nameof(DataTable));
        var newMap = field.ToDictionary(kv => kv.Key, kv => kv.Value.ToList());
        foreach (var map in newMap)
        {
            for (var index = 0; index < props.Length; index++)
            {
                var info = props[index];
                var cell = map.Value[index];

                if (info.PropertyType != StringType && string.IsNullOrWhiteSpace(cell))
                {   //如果类型不是string并且内容为空 将实例一个预设值类型
                    map.Value[index] = Activator.CreateInstance(info.PropertyType).ToString();
                }   //如果是string并内容是null值，给个string.Empty
                else if (cell == null) map.Value[index] = string.Empty;
            }
        }
        return newMap.ToDictionary(kv=>kv.Key,kv=>kv.Value as IReadOnlyList<string>);
    }
}

public static class StringConverExtension
{
    public static string FirstCharToUpper(this string input)
    {
        switch (input)
        {
            case null: throw new ArgumentNullException(nameof(input));
            case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default: return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
    public static string FirstCharToLower(this string input)
    {
        switch (input)
        {
            case null: throw new ArgumentNullException(nameof(input));
            case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default: return input.First().ToString().ToLower() + input.Substring(1);
        }
    }
}