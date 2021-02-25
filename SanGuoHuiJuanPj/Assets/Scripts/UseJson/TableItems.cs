public class PlayerInitialTable
{
    public int id { get; set; }
    public string power { get; set; }
    public string initialHero { get; set; }
    public string initialSoldier { get; set; }
    public string initialTower { get; set; }
    public string powerIntro { get; set; }
}
public class AssetTable
{
    public int id { get; set; }
    public string type { get; set; }
    public int startValue { get; set; }
    public int limit { get; set; }
}
public class HeroTable
{
    public int id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public int rarity { get; set; }
    public int price { get; set; }
    public int classes { get; set; }
    public int powers { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }
    public int hpr { get; set; }
    public int dod { get; set; }
    public int def { get; set; }
    public int cri { get; set; }
    public int criDamage { get; set; }
    public int huixin { get; set; }
    public int huixinDamage { get; set; }
    public int icon { get; set; }
    public int tag1 { get; set; }
    public int tag2 { get; set; }
    public string chestZyBox { get; set; }
    public string chestBxBox { get; set; }
    public int chanChu { get; set; }
    public int fuMianMianYi { get; set; }
    public int wuLiMianShang { get; set; }
    public int faShuMianShang { get; set; }
    public string fettersRelated { get; set; }
}
public class PlayerLevelTable
{
    public int level { get; set; }
    public int exp { get; set; }
    public int heroLimit { get; set; }
    public int companionLimit { get; set; }
    public int homeHp { get; set; }
    public int yuanZheng { get; set; }
    public int unLockShiLi { get; set; }
    public string BaYeCombat { get; set; }
    public string BaYeNonCombat { get; set; }
    public int BaYeBattle { get; set; }
    public string BaYeJunTuan { get; set; }
    public int BaYeBattleLevel { get; set; }

}
public class SoldierTable
{
    public int id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public int rarity { get; set; }
    public int price { get; set; }
    public int classes { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }//已修正
    public int hpr { get; set; }
    public int dod { get; set; }
    public int def { get; set; }
    public int cri { get; set; }
    public int criDamage { get; set; }
    public int icon { get; set; }
    public int tag1 { get; set; }
    public int tag2 { get; set; }
    public int onShow { get; set; }
}
public class TowerTable
{
    public int id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public int rarity { get; set; }
    public int price { get; set; }
    public string shortName { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }
    public int hpr { get; set; }
    public string scope { get; set; }
    public int icon { get; set; }
    public string chestZyBox { get; set; }
    public string chestBxBox { get; set; }
    public string intro2 { get; set; }
    public int chanChu { get; set; }
    public int shiLi { get; set; }
}
public class ClassTable
{
    public int id { get; set; }
    public string type { get; set; }
    public string skill { get; set; }
    public string shortName { get; set; }
    public string info { get; set; }
    public int bingZhongXi { get; set; }
}
public class UpGradeTable
{
    public int starLevel { get; set; }
    public int partsDemand { get; set; }
    public int yuanbaoDemand { get; set; }
}
public class TrapTable
{
    public int id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public int rarity { get; set; }
    public int price { get; set; }
    public string shortName { get; set; }
    public string damage { get; set; }
    public string hp { get; set; }
    public int hpr { get; set; }
    public int icon { get; set; }
    public string chestZyBox { get; set; }
    public string chestBxBox { get; set; }
    public string intro2 { get; set; }
    public int chanChu { get; set; }
    public int shiLi { get; set; }
}
public class SpellTable
{
    public int id { get; set; }
    public string name { get; set; }
    public string intro { get; set; }
    public int rarity { get; set; }//已修正
    public int price { get; set; }//已修正
    public string shortName { get; set; }
    public int icon { get; set; }//已修正
    public string chest1 { get; set; }//已修正
    public string chest2 { get; set; }//已修正
    public string intro2 { get; set; }//已修正
    public int onShow { get; set; }//已修正
}
public class WarChestTable
{
    public int id { get; set; }
    public string chestType { get; set; }//已修正
    public string chestName { get; set; }
    public int exp { get; set; }
    public string shuoyu { get; set; }
    public string yuanbao { get; set; }
    public string trap { get; set; }
    public string tower { get; set; }
    public string hero { get; set; }
}
public class WarTable
{
    public int warId { get; set; }
    public string warName { get; set; }
    public string warIntro { get; set; }
    public int startPoint { get; set; }
    public int counts { get; set; }
    public string reward1 { get; set; }
    public string reward2 { get; set; }
    public string reward3 { get; set; }
    public int YuanBaoR { get; set; }
    public int YuQueR { get; set; }
    public int TiLiR { get; set; }
    public string JunTuan { get; set; }
    public string JunTuanIntro { get; set; }
}
public class CityLevelTable
{
    public int cityLevel { get; set; }
    public int cost { get; set; }
    public int cityDur { get; set; }
    public int heroFightLimit { get; set; }
    public int companionFightLimit { get; set; }

}
public class PointTable
{
    public int pointId { get; set; }
    public string nextPoint { get; set; }
    public string pointName { get; set; }
    public int eventType { get; set; }
    public int eventId { get; set; }
    public string pointStory { get; set; }
    public int cityIcon { get; set; }
    public int battleBG { get; set; }
    public int battleBGM { get; set; }
    public string flag { get; set; }
}
public class BattleEventTable
{
    public int battleId { get; set; }
    public string enemyPower { get; set; }
    public string enemyId { get; set; }
    public int homeHp { get; set; }
    public string warChest { get; set; }
    public int goldReward { get; set; }
    public int type { get; set; }
}
public class EnemyTable
{
    public int enemyId { get; set; }
    public int pos1 { get; set; }
    public int pos2 { get; set; }
    public int pos3 { get; set; }
    public int pos4 { get; set; }
    public int pos5 { get; set; }
    public int pos6 { get; set; }
    public int pos7 { get; set; }
    public int pos8 { get; set; }
    public int pos9 { get; set; }
    public int pos10 { get; set; }
    public int pos11 { get; set; }
    public int pos12 { get; set; }
    public int pos13 { get; set; }
    public int pos14 { get; set; }
    public int pos15 { get; set; }
    public int pos16 { get; set; }
    public int pos17 { get; set; }
    public int pos18 { get; set; }
    public int pos19 { get; set; }
    public int pos20 { get; set; }
}
public class EnemyUnitTable
{
    public int enemyUnit { get; set; }
    public int unitType { get; set; }
    public int rarity { get; set; }
    public int unitLevel { get; set; }
    public int goldReward { get; set; }
    public string warChest { get; set; }
}
public class StoryTable
{
    public int id { get; set; }
    public string story { get; set; }
    public string storyBody { get; set; }
    public string exitText { get; set; }
    public string option1 { get; set; }
    public string option2 { get; set; }
    public int option1ToEnding { get; set; }
    public int option2ToEnding { get; set; }
}
public class StoryRTable
{
    public int id { get; set; }
    public string ending { get; set; }
    public int unitType { get; set; }
    public int unitId { get; set; }
    public int unitLevel { get; set; }
    public int unitCount { get; set; }
    public int goldReward { get; set; }
}
public class TestTable
{
    public int id { get; set; }
    public string question { get; set; }
    public int truth { get; set; }
    public string answer1 { get; set; }
    public string answer2 { get; set; }
    public string answer3 { get; set; }
    public int weightValue { get; set; }
}
public class TestRTable
{
    public int id { get; set; }
    public int weightValue { get; set; }
    public int type { get; set; }
    public int rarity { get; set; }
    public int level { get; set; }
}
public class EncounterTable
{
    public int id { get; set; }
    public int weightValue { get; set; }//已修正
    public int type { get; set; }//已修正
    public int rarity { get; set; }//已修正
    public int level { get; set; }//已修正
    public int cost { get; set; }//已修正

}
public class ShoppingTable
{
    public int id { get; set; }
    public int weightValue { get; set; }
    public int type { get; set; }
    public int rarity { get; set; }
    public int level { get; set; }
}
public class ChoseWarTable
{
    public int id { get; set; }
    public string difficulty { get; set; }
    public string warList { get; set; }
    public int unlock { get; set; }//已修正
    public string tiliCost { get; set; }
    public string instructions { get; set; }
}
public class GuideTable
{
    public int id { get; set; }
    public int title { get; set; }
    public string story { get; set; }
    public string card1 { get; set; }
    public string card2 { get; set; }
    public string card3 { get; set; }
    public string card4 { get; set; }
    public string card5 { get; set; }
    public int homeHp1 { get; set; }
    public int homeHp2 { get; set; }
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
public class KnowledgeTable
{
    public int id { get; set; }
    public int type { get; set; }//已修正
    public string textStr { get; set; }
    public int tiLiR { get; set; }//已修正
    public string yuanBaoR { get; set; }
    public string Name { get; set; }

}
public class RCodeTable
{
    public int id { get; set; }
    public string code { get; set; }
    public string TheLastTime { get; set; }
    public string Info { get; set; }
    public int YuQue { get; set; }//已修正
    public int YuanBao { get; set; }//已修正
    public int TiLi { get; set; }//已修正
    public string SuiPian { get; set; }
}
public class TiLiStoreTable
{
    public int id { get; set; }
    public int TiLi { get; set; }//已修正
    public int YuQue { get; set; }//已修正
}
public class EnemyBOSSTable
{
    public int enemyId { get; set; }//已修正
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
public class StringTextTable
{
    public int id { get; set; }
    public string stringContent { get; set; }
}
public class NumParametersTable
{
    public int id { get; set; }
    public int numContent { get; set; }//已修正
}
public class JiBanTable
{
    public int id { get; set; }
    public string jiBanMing { get; set; }
    public int isOpen { get; set; }//已修正
    public string heroId { get; set; }
    public string jiBanXiaoGuo { get; set; }
    public string BOSSId { get; set; }
}
public class ShiLiTable 
{
    public int id { get; set; }
    public string ShiLi { get; set; }
    public int Zi { get; set; }//已修正
    public int Qi { get; set; }//已修正
    public int UnlockLevel { get; set; }//已修正
    public string JunTuanInfo { get; set; }
    public string JunTuanLeader { get; set; }
}
public class BaYeDiTuTable
{
    public int id { get; set; }
    public int type { get; set; }//已修正
    public string events { get; set; }
    public string name { get; set; }
}
public class BaYeShiJianTable
{
    public int id { get; set; }
    public int QuanZhong { get; set; }//已修正
    public string BaYeJingYan { get; set; }
    public int BaYeBattle { get; set; }//已修正
    public int Flag { get; set; }//已修正
    public string FlagWord { get; set; }
}
public class BaYeBattleTable
{
    public int id { get; set; }
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
public class BaYeRenWuTable
{ 
    public int id { get; set; }
    public int jingYan { get; set; }//已修正
    public int jiangLi { get; set; }//已修正
}
public class StoryPoolTable 
{
    public int poolId { get; set; }//已修正
    public string storyId { get; set; }
}
public class StoryIdTable 
{
    public int storyId { get; set; }//已修正
    public int weight { get; set; }//已修正
    public int storyType { get; set; }//已修正
    public string jinBiR { get; set; }
    public string jingYanR { get; set; }
    public string YuQueR { get; set; }
    public string YuanBaoR { get; set; }
    public string JunTuanR { get; set; }
    public string Time { get; set; }
    public int WarId { get; set; }//已修正
}
public class BaYeTVTable 
{
    public int id { get; set; }
    public int weight { get; set; }//已修正
    public string text { get; set; }
    public string time { get; set; }
    public int type { get; set; }//已修正
}
public class BaYeNameTable
{
    public int id { get; set; }
    public int weight { get; set; }//已修正
    public string name { get; set; }
}