public class PlayerInitialTableItem
{
    public string id { get; set; }
    public string power { get; set; }
    public string initialHero { get; set; }
    public string initialSoldier { get; set; }
    public string initialTower { get; set; }
    public string powerIntro { get; set; }
}
public class AssetTableItem
{
    public string id { get; set; }
    public string type { get; set; }
    public string startValue { get; set; }
}
public class HeroTableItem
{
    public string id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public string rarity { get; set; }
    public string price { get; set; }
    public string classes { get; set; }
    public string powers { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }
    public string hpr { get; set; }
    public string dod { get; set; }
    public string def { get; set; }
    public string cri { get; set; }
    public string criDamage { get; set; }
    public string huixin { get; set; }
    public string huixinDamage { get; set; }
    public string icon { get; set; }
    public string tag1 { get; set; }
    public string tag2 { get; set; }
    public string chestZyBox { get; set; }
    public string chestBxBox { get; set; }
    public string chanChu { get; set; }
    public string fuMianMianYi { get; set; }
    public string wuLiMianShang { get; set; }
    public string faShuMianShang { get; set; }
    public string fettersRelated { get; set; }
}
public class PlayerLevelTableItem
{
    public string level { get; set; }
    public string exp { get; set; }
    public string heroLimit { get; set; }
    public string companionLimit { get; set; }
    public string homeHp { get; set; }
    public string yuanZheng { get; set; }
    public string unLockShiLi { get; set; }
    public string BaYeCombat { get; set; }
    public string BaYeNonCombat { get; set; }
    public string BaYeBattle { get; set; }
}
public class SoldierTableItem
{
    public string id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public string rarity { get; set; }
    public string price { get; set; }
    public string classes { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }
    public string hpr { get; set; }
    public string dod { get; set; }
    public string def { get; set; }
    public string cri { get; set; }
    public string criDamage { get; set; }
    public string icon { get; set; }
    public string tag1 { get; set; }
    public string tag2 { get; set; }
    public string onShow { get; set; }
}
public class TowerTableItem
{
    public string id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public string rarity { get; set; }
    public string price { get; set; }
    public string shortName { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }
    public string hpr { get; set; }
    public string scope { get; set; }
    public string icon { get; set; }
    public string chestZyBox { get; set; }
    public string chestBxBox { get; set; }
    public string intro2 { get; set; }
    public string chanChu { get; set; }
    public string shiLi { get; set; }
}
public class ClassTableItem
{
    public string id { get; set; }
    public string type { get; set; }
    public string skill { get; set; }
    public string shortName { get; set; }
    public string info { get; set; }
    public string bingZhongXi { get; set; }
}
public class UpGradeTableItem
{
    public string starLevel { get; set; }
    public string partsDemand { get; set; }
    public string yuanbaoDemand { get; set; }
}
public class TrapTableItem
{
    public string id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public string rarity { get; set; }
    public string price { get; set; }
    public string shortName { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }
    public string hpr { get; set; }
    public string icon { get; set; }
    public string chestZyBox { get; set; }
    public string chestBxBox { get; set; }
    public string intro2 { get; set; }
    public string chanChu { get; set; }
    public string shiLi { get; set; }
}
public class SpellTableItem
{
    public string id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public string rarity { get; set; }
    public string price { get; set; }
    public string shortName { get; set; }
    public string icon { get; set; }
    public string onShow { get; set; }
}
public class WarChestTableItem
{
    public string id { get; set; }
    public string chestType { get; set; }
    public string chestName { get; set; }
    public string exp { get; set; }
    public string shuoyu { get; set; }
    public string yuanbao { get; set; }
    public string trap { get; set; }
    public string tower { get; set; }
    public string hero { get; set; }
}
public class WarTableItem
{
    public string warId { get; set; }
    public string warName { get; set; }
    public string warIntro { get; set; }
    public string startPoint { get; set; }
    public string counts { get; set; }
    public string reward1 { get; set; }
    public string reward2 { get; set; }
    public string reward3 { get; set; }
    public string YuanBaoR { get; set; }
    public string YuQueR { get; set; }
    public string TiLiR { get; set; }
}
public class CityLevelTableItem
{
    public string cityLevel { get; set; }
    public string cost { get; set; }
    public string cityDur { get; set; }
    public string heroFightLimit { get; set; }
}
public class PointTableItem
{
    public string pointId { get; set; }
    public string nextPoint { get; set; }
    public string pointName { get; set; }
    public string eventType { get; set; }
    public string eventId { get; set; }
    public string pointStory { get; set; }
    public string cityIcon { get; set; }
    public string battleBG { get; set; }
    public string battleBGM { get; set; }
    public string flag { get; set; }
}
public class BattleEventTableItem
{
    public string battleId { get; set; }
    public string enemyPower { get; set; }
    public string enemyId { get; set; }
    public string homeHp { get; set; }
    public string warChest { get; set; }
    public string goldReward { get; set; }
    public string type { get; set; }
}
public class EnemyTableItem
{
    public string enemyId { get; set; }
    public string pos1 { get; set; }
    public string pos2 { get; set; }
    public string pos3 { get; set; }
    public string pos4 { get; set; }
    public string pos5 { get; set; }
    public string pos6 { get; set; }
    public string pos7 { get; set; }
    public string pos8 { get; set; }
    public string pos9 { get; set; }
    public string pos10 { get; set; }
    public string pos11 { get; set; }
    public string pos12 { get; set; }
    public string pos13 { get; set; }
    public string pos14 { get; set; }
    public string pos15 { get; set; }
    public string pos16 { get; set; }
    public string pos17 { get; set; }
    public string pos18 { get; set; }
    public string pos19 { get; set; }
    public string pos20 { get; set; }
}
public class EnemyUnitTableItem
{
    public string enemyUnit { get; set; }
    public string unitType { get; set; }
    public string rarity { get; set; }
    public string unitLevel { get; set; }
    public string goldReward { get; set; }
    public string warChest { get; set; }
}
public class StoryTableItem
{
    public string id { get; set; }
    public string story { get; set; }
    public string storyBody { get; set; }
    public string exitText { get; set; }
    public string option1 { get; set; }
    public string option2 { get; set; }
    public string option1ToEnding { get; set; }
    public string option2ToEnding { get; set; }
}
public class StoryRTableItem
{
    public string id { get; set; }
    public string ending { get; set; }
    public string unitType { get; set; }
    public string unitId { get; set; }
    public string unitLevel { get; set; }
    public string unitCount { get; set; }
    public string goldReward { get; set; }
}
public class TestTableItem
{
    public string id { get; set; }
    public string question { get; set; }
    public string truth { get; set; }
    public string answer1 { get; set; }
    public string answer2 { get; set; }
    public string answer3 { get; set; }
    public string weightValue { get; set; }
}
public class TestRTableItem
{
    public string id { get; set; }
    public string weightValue { get; set; }
    public string type { get; set; }
    public string rarity { get; set; }
    public string level { get; set; }
}
public class EncounterTableItem
{
    public string id { get; set; }
    public string weightValue { get; set; }
    public string type { get; set; }
    public string rarity { get; set; }
    public string level { get; set; }
    public string cost { get; set; }

}
public class ShoppingTableItem
{
    public string id { get; set; }
    public string weightValue { get; set; }
    public string type { get; set; }
    public string rarity { get; set; }
    public string level { get; set; }
}
public class ChoseWarTableItem
{
    public string id { get; set; }
    public string difficulty { get; set; }
    public string warList { get; set; }
    public string unlock { get; set; }
    public string tiliCost { get; set; }
    public string instructions { get; set; }
}
public class GuideTableItem
{
    public string id { get; set; }
    public string title { get; set; }
    public string story { get; set; }
    public string card1 { get; set; }
    public string card2 { get; set; }
    public string card3 { get; set; }
    public string card4 { get; set; }
    public string card5 { get; set; }
    public string homeHp1 { get; set; }
    public string homeHp2 { get; set; }
    public string pos1 { get; set; }
    public string pos2 { get; set; }
    public string pos3 { get; set; }
    public string pos4 { get; set; }
    public string pos5 { get; set; }
    public string pos6 { get; set; }
    public string pos7 { get; set; }
    public string pos8 { get; set; }
    public string pos9 { get; set; }
    public string pos10 { get; set; }
    public string pos11 { get; set; }
    public string pos12 { get; set; }
    public string pos13 { get; set; }
    public string pos14 { get; set; }
    public string pos15 { get; set; }
    public string pos16 { get; set; }
    public string pos17 { get; set; }
    public string pos18 { get; set; }
    public string pos19 { get; set; }
    public string pos20 { get; set; }
    public string epos1 { get; set; }
    public string epos2 { get; set; }
    public string epos3 { get; set; }
    public string epos4 { get; set; }
    public string epos5 { get; set; }
    public string epos6 { get; set; }
    public string epos7 { get; set; }
    public string epos8 { get; set; }
    public string epos9 { get; set; }
    public string epos10 { get; set; }
    public string epos11 { get; set; }
    public string epos12 { get; set; }
    public string epos13 { get; set; }
    public string epos14 { get; set; }
    public string epos15 { get; set; }
    public string epos16 { get; set; }
    public string epos17 { get; set; }
    public string epos18 { get; set; }
    public string epos19 { get; set; }
    public string epos20 { get; set; }
}
public class KnowledgeTableItem
{
    public string id { get; set; }
    public string type { get; set; }
    public string textStr { get; set; }
    public string tiLiR { get; set; }
    public string yuanBaoR { get; set; }
    public string Name { get; set; }

}
public class RCodeTableItem
{
    public string id { get; set; }
    public string code { get; set; }
    public string TheLastTime { get; set; }
    public string Info { get; set; }
    public string YuQue { get; set; }
    public string YuanBao { get; set; }
    public string TiLi { get; set; }
    public string SuiPian { get; set; }
}
public class TiLiStoreTableItem
{
    public string id { get; set; }
    public string TiLi { get; set; }
    public string YuQue { get; set; }
}
public class EnemyBOSSTableItem
{
    public string enemyId { get; set; }
    public string pos1 { get; set; }
    public string pos2 { get; set; }
    public string pos3 { get; set; }
    public string pos4 { get; set; }
    public string pos5 { get; set; }
    public string pos6 { get; set; }
    public string pos7 { get; set; }
    public string pos8 { get; set; }
    public string pos9 { get; set; }
    public string pos10 { get; set; }
    public string pos11 { get; set; }
    public string pos12 { get; set; }
    public string pos13 { get; set; }
    public string pos14 { get; set; }
    public string pos15 { get; set; }
    public string pos16 { get; set; }
    public string pos17 { get; set; }
    public string pos18 { get; set; }
    public string pos19 { get; set; }
    public string pos20 { get; set; }
}
public class StringTextTableItem
{
    public string id { get; set; }
    public string stringContent { get; set; }
}
public class NumParametersTableItem
{
    public string id { get; set; }
    public string numContent { get; set; }
}
public class JiBanTableItem
{
    public string id { get; set; }
    public string jiBanMing { get; set; }
    public string isOpen { get; set; }
    public string heroId { get; set; }
    public string jiBanXiaoGuo { get; set; }
    public string BOSSId { get; set; }
}
public class ShiLiTableItem 
{
    public string id { get; set; }
    public string ShiLi { get; set; }
    public string Zi { get; set; }
    public string Qi { get; set; }
}
public class BaYeDiTuTableItem
{
    public string id { get; set; }
    public string type { get; set; }
    public string events { get; set; }
    public string name { get; set; }
}
public class BaYeShiJianTableItem
{
    public string id { get; set; }
    public string QuanZhong { get; set; }
    public string BaYeJingYan { get; set; }
    public string BaYeBattle { get; set; }
}
public class BaYeBattleTableItem
{
    public string id { get; set; }
    public string level0 { get; set; }
    public string level1 { get; set; }
    public string level2 { get; set; }
    public string level3 { get; set; }
    public string level4 { get; set; }
    public string level5 { get; set; }
    public string level6 { get; set; }
    public string level7 { get; set; }
    public string level8 { get; set; }
    public string level9 { get; set; }
}
public class BaYeRenWuTableItem
{ 
    public string id { get; set; }
    public string jingYan { get; set; }
    public string jiangLi { get; set; }
}