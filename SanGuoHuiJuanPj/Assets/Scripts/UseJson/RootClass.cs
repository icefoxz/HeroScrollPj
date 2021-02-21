using System.Collections.Generic;

/// <summary>
/// 控制数据表Root
/// </summary>
public class Roots
{
    /// <summary>
    /// 势力初始表
    /// </summary>
    public List<PlayerInitialTable> PlayerInitialTable { get; set; }
    /// <summary>
    /// 基础资源表
    /// </summary>
    public List<AssetTable> AssetTable { get; set; }
    /// <summary>
    /// 武将信息表
    /// </summary>
    public List<HeroTable> HeroTable { get; set; }
    /// <summary>
    /// 玩家等级表
    /// </summary>
    public List<PlayerLevelTable> PlayerLevelTable { get; set; }
    /// <summary>
    /// 士兵信息表
    /// </summary>
    public List<SoldierTable> SoldierTable { get; set; }
    /// <summary>
    /// 塔信息表
    /// </summary>
    public List<TowerTable> TowerTable { get; set; }
    /// <summary>
    /// 兵种信息表
    /// </summary>
    public List<ClassTable> ClassTable { get; set; }
    /// <summary>
    /// 升级碎片表
    /// </summary>
    public List<UpGradeTable> UpGradeTable { get; set; }
    /// <summary>
    /// 辅助单位卡牌表
    /// </summary>
    public List<TrapTable> TrapTable { get; set; }
    /// <summary>
    /// 辅助技能表
    /// </summary>
    public List<SpellTable> SpellTable { get; set; }
    /// <summary>
    /// 宝箱表
    /// </summary>
    public List<WarChestTable> WarChestTable { get; set; }
    /// <summary>
    /// 战役表
    /// </summary>
    public List<WarTable> WarTable { get; set; }
    /// <summary>
    /// 城池等级表
    /// </summary>
    public List<CityLevelTable> CityLevelTable { get; set; }
    /// <summary>
    /// 关卡表
    /// </summary>
    public List<PointTable> PointTable { get; set; }
    /// <summary>
    /// 战斗事件表
    /// </summary>
    public List<BattleEventTable> BattleEventTable { get; set; }
    /// <summary>
    /// 敌人位置表
    /// </summary>
    public List<EnemyTable> EnemyTable { get; set; }
    /// <summary>
    /// 敌人信息表
    /// </summary>
    public List<EnemyUnitTable> EnemyUnitTable { get; set; }
    /// <summary>
    /// 非战斗事件-故事表
    /// </summary>
    public List<StoryTable> StoryTable { get; set; }
    /// <summary>
    /// 非战斗事件-故事奖励表
    /// </summary>
    public List<StoryRTable> StoryRTable { get; set; }
    /// <summary>
    /// 非战斗事件-答题表
    /// </summary>
    public List<TestTable> TestTable { get; set; }
    /// <summary>
    /// 非战斗事件-答题奖励表
    /// </summary>
    public List<TestRTable> TestRTable { get; set; }
    /// <summary>
    /// 非战斗事件-三选
    /// </summary>
    public List<EncounterTable> EncounterTable { get; set; }
    /// <summary>
    /// 非战斗时间-购买
    /// </summary>
    public List<ShoppingTable> ShoppingTable { get; set; }
    /// <summary>
    /// 难度选择表
    /// </summary>
    public List<ChoseWarTable> ChoseWarTable { get; set; }
    /// <summary>
    /// 新手引导配置表
    /// </summary>
    public List<GuideTable> GuideTable { get; set; }
    /// <summary>
    /// 酒馆锦囊表
    /// </summary>
    public List<KnowledgeTable> KnowledgeTable { get; set; }
    /// <summary>
    /// 兑换码表
    /// </summary>
    public List<RCodeTable> RCodeTable { get; set; }
    /// <summary>
    /// 体力商店表
    /// </summary>
    public List<TiLiStoreTable> TiLiStoreTable { get; set; }
    /// <summary>
    /// 敌人BOSS固定敌人表
    /// </summary>
    public List<EnemyBOSSTable> EnemyBOSSTable { get; set; }
    /// <summary>
    /// 文本内容表
    /// </summary>
    public List<StringTextTable> StringTextTable { get; set; }
    /// <summary>
    /// 游戏数值表
    /// </summary>
    public List<NumParametersTable> NumParametersTable { get; set; }
    /// <summary>
    /// 羁绊表
    /// </summary>
    public List<JiBanTable> JiBanTable { get; set; }
    /// <summary>
    /// 势力表
    /// </summary>
    public List<ShiLiTable> ShiLiTable { get; set; }
    /// <summary>
    /// 霸业地图表
    /// </summary>
    public List<BaYeDiTuTable> BaYeDiTuTable { get; set; }
    /// <summary>
    /// 霸业事件表
    /// </summary>
    public List<BaYeShiJianTable> BaYeShiJianTable { get; set; }
    /// <summary>
    /// 霸业战役难度表
    /// </summary>
    public List<BaYeBattleTable> BaYeBattleTable { get; set; }
    /// <summary>
    /// 霸业任务表
    /// </summary>
    public List<BaYeRenWuTable> BaYeRenWuTable { get; set; }
    /// <summary>
    /// 霸业故事点表
    /// </summary>
    public List<StoryPoolTable> StoryPoolTable { get; set; }
    /// <summary>
    /// 故事表
    /// </summary>
    public List<StoryIdTable> StoryIdTable { get; set; }
    /// <summary>
    /// 霸业TV表
    /// </summary>
    public List<BaYeTVTable> BaYeTVTable { get; set; }
    /// <summary>
    /// 霸业TV人名表
    /// </summary>
    public List<BaYeNameTable> BaYeNameTable { get; set; }
}