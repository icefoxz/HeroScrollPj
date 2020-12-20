using System.Collections.Generic;

/// <summary>
/// 控制数据表Root
/// </summary>
public class Roots
{
    /// <summary>
    /// 势力初始表
    /// </summary>
    public List<PlayerInitialTableItem> PlayerInitialTable { get; set; }
    /// <summary>
    /// 基础资源表
    /// </summary>
    public List<AssetTableItem> AssetTable { get; set; }
    /// <summary>
    /// 武将信息表
    /// </summary>
    public List<HeroTableItem> HeroTable { get; set; }
    /// <summary>
    /// 玩家等级表
    /// </summary>
    public List<PlayerLevelTableItem> PlayerLevelTable { get; set; }
    /// <summary>
    /// 士兵信息表
    /// </summary>
    public List<SoldierTableItem> SoldierTable { get; set; }
    /// <summary>
    /// 塔信息表
    /// </summary>
    public List<TowerTableItem> TowerTable { get; set; }
    /// <summary>
    /// 兵种信息表
    /// </summary>
    public List<ClassTableItem> ClassTable { get; set; }
    /// <summary>
    /// 升级碎片表
    /// </summary>
    public List<UpGradeTableItem> UpGradeTable { get; set; }
    /// <summary>
    /// 辅助单位卡牌表
    /// </summary>
    public List<TrapTableItem> TrapTable { get; set; }
    /// <summary>
    /// 辅助技能表
    /// </summary>
    public List<SpellTableItem> SpellTable { get; set; }
    /// <summary>
    /// 宝箱表
    /// </summary>
    public List<WarChestTableItem> WarChestTable { get; set; }
    /// <summary>
    /// 战役表
    /// </summary>
    public List<WarTableItem> WarTable { get; set; }
    /// <summary>
    /// 城池等级表
    /// </summary>
    public List<CityLevelTableItem> CityLevelTable { get; set; }
    /// <summary>
    /// 关卡表
    /// </summary>
    public List<PointTableItem> PointTable { get; set; }
    /// <summary>
    /// 战斗事件表
    /// </summary>
    public List<BattleEventTableItem> BattleEventTable { get; set; }
    /// <summary>
    /// 敌人位置表
    /// </summary>
    public List<EnemyTableItem> EnemyTable { get; set; }
    /// <summary>
    /// 敌人信息表
    /// </summary>
    public List<EnemyUnitTableItem> EnemyUnitTable { get; set; }
    /// <summary>
    /// 非战斗事件-故事表
    /// </summary>
    public List<StoryTableItem> StoryTable { get; set; }
    /// <summary>
    /// 非战斗事件-故事奖励表
    /// </summary>
    public List<StoryRTableItem> StoryRTable { get; set; }
    /// <summary>
    /// 非战斗事件-答题表
    /// </summary>
    public List<TestTableItem> TestTable { get; set; }
    /// <summary>
    /// 非战斗事件-答题奖励表
    /// </summary>
    public List<TestRTableItem> TestRTable { get; set; }
    /// <summary>
    /// 非战斗事件-三选
    /// </summary>
    public List<EncounterTableItem> EncounterTable { get; set; }
    /// <summary>
    /// 非战斗时间-购买
    /// </summary>
    public List<ShoppingTableItem> ShoppingTable { get; set; }
    /// <summary>
    /// 难度选择表
    /// </summary>
    public List<ChoseWarTableItem> ChoseWarTable { get; set; }
    /// <summary>
    /// 新手引导配置表
    /// </summary>
    public List<GuideTableItem> GuideTable { get; set; }
    /// <summary>
    /// 酒馆锦囊表
    /// </summary>
    public List<KnowledgeTableItem> KnowledgeTable { get; set; }
    /// <summary>
    /// 兑换码表
    /// </summary>
    public List<RCodeTableItem> RCodeTable { get; set; }
    /// <summary>
    /// 体力商店表
    /// </summary>
    public List<TiLiStoreTableItem> TiLiStoreTable { get; set; }
    /// <summary>
    /// 敌人BOSS固定敌人表
    /// </summary>
    public List<EnemyBOSSTableItem> EnemyBOSSTable { get; set; }
    /// <summary>
    /// 文本内容表
    /// </summary>
    public List<StringTextTableItem> StringTextTable { get; set; }
    /// <summary>
    /// 游戏数值表
    /// </summary>
    public List<NumParametersTableItem> NumParametersTable { get; set; }
    /// <summary>
    /// 羁绊表
    /// </summary>
    public List<JiBanTableItem> JiBanTable { get; set; }
    /// <summary>
    /// 势力表
    /// </summary>
    public List<ShiLiTableItem> ShiLiTable { get; set; }
    /// <summary>
    /// 霸业地图表
    /// </summary>
    public List<BaYeDiTuTableItem> BaYeDiTuTable { get; set; }
    /// <summary>
    /// 霸业事件表
    /// </summary>
    public List<BaYeShiJianTableItem> BaYeShiJianTable { get; set; }
    /// <summary>
    /// 霸业战役难度表
    /// </summary>
    public List<BaYeBattleTableItem> BaYeBattleTable { get; set; }
}