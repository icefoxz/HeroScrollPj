//using System.Collections.Generic;
//using UnityEngine;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using Newtonsoft.Json;

//public class LoadJsonFile : MonoBehaviour
//{
//    public static LoadJsonFile instance;

//    //Resources文件夹下
//    public static readonly string Folder = "Jsons/";
//    //存放json数据名
//    private static readonly string tableNameStrs = "PlayerInitialTable;AssetTable;HeroTable;PlayerLevelTable;SoldierTable;" +
//        "TowerTable;ClassTable;UpGradeTable;TrapTable;SpellTable;WarChestTable;WarTable;CityLevelTable;PointTable;" +
//        "BattleEventTable;EnemyTable;EnemyUnitTable;StoryTable;StoryRTable;TestTable;TestRTable;EncounterTable;" +
//        "ShoppingTable;ChoseWarTable;GuideTable;KnowledgeTable;RCodeTable;TiLiStoreTable;EnemyBOSSTable;StringTextTable;" + "NumParametersTable;JiBanTable;ShiLiTable;BaYeDiTuTable;BaYeShiJianTable;BaYeBattleTable;BaYeRenWuTable;StoryPoolTable;StoryIdTable;BaYeTVTable;BaYeNameTable";


//    /// <summary>
//    /// 势力初始数据
//    /// </summary>
//    public static List<List<string>> playerInitialTableDatas;

//    /// <summary>
//    /// 基础资源类型数据
//    /// </summary>
//    public static List<AssetTableItem> assetTableDatas;

//    /// <summary>
//    /// 武将基础信息表数据
//    /// </summary>
//    public static List<List<string>> heroTableDatas;

//    /// <summary>
//    /// 玩家等级表数据
//    /// </summary>
//    public static List<List<string>> playerLevelTableDatas;

//    /// <summary>
//    /// 士兵基础信息表数据
//    /// </summary>
//    public static List<List<string>> soldierTableDatas;

//    /// <summary>
//    /// 塔基础信息表数据
//    /// </summary>
//    public static List<List<string>> towerTableDatas;

//    /// <summary>
//    /// 兵种信息表数据
//    /// </summary>
//    public static List<List<string>> classTableDatas;

//    /// <summary>
//    /// 升级碎片表数据
//    /// </summary>
//    public static List<List<string>> upGradeTableDatas;

//    /// <summary>
//    /// 辅助单位卡牌表数据
//    /// </summary>
//    public static List<List<string>> trapTableDatas;

//    /// <summary>
//    /// 辅助技能表数据
//    /// </summary>
//    public static List<List<string>> spellTableDatas;

//    /// <summary>
//    /// 宝箱表数据
//    /// </summary>
//    public static List<List<string>> warChestTableDatas;

//    /// <summary>
//    /// 战役表数据
//    /// </summary>
//    public static List<List<string>> warTableDatas;

//    /// <summary>
//    /// 城池等级表数据
//    /// </summary>
//    public static List<List<string>> cityLevelTableDatas;
    
//    // <summary>
//    /// 关卡表数据
//    /// </summary>
//    public static List<List<string>> pointTableDatas;

//    // <summary>
//    /// 战斗事件表数据
//    /// </summary>
//    public static List<List<string>> battleEventTableDatas;

//    // <summary>
//    /// 敌方位置表数据
//    /// </summary>
//    public static List<List<string>> enemyTableDatas;

//    // <summary>
//    /// 敌方信息表数据
//    /// </summary>
//    public static List<List<string>> enemyUnitTableDatas;

//    /// <summary>
//    /// 非战斗事件-故事数据
//    /// </summary>
//    public static List<List<string>> storyTableDatas;

//    /// <summary>
//    /// 非战斗事件-故事奖励数据
//    /// </summary>
//    public static List<List<string>> storyRTableDatas;

//    /// <summary>
//    /// 非战斗事件-答题数据
//    /// </summary>
//    public static List<List<string>> testTableDatas;

//    /// <summary>
//    /// 非战斗事件-答题奖励数据
//    /// </summary>
//    public static List<List<string>> testRTableDatas;

//    /// <summary>
//    /// 非战斗事件-三选单位数据
//    /// </summary>
//    public static List<List<string>> encounterTableDatas;

//    /// <summary>
//    /// 非战斗事件-购买单位数据
//    /// </summary>
//    public static List<List<string>> shoppingTableDatas;

//    /// <summary>
//    /// 难度选择表数据
//    /// </summary>
//    public static List<List<string>> choseWarTableDatas;

//    /// <summary>
//    /// 新手引导配置表
//    /// </summary>
//    public static List<List<string>> guideTableDatas;

//    /// <summary>
//    /// 酒馆锦囊表
//    /// </summary>
//    public static List<List<string>> knowledgeTableDatas;

//    /// <summary>
//    /// 兑换码表
//    /// </summary>
//    public static List<List<string>> rCodeTableDatas;

//    /// <summary>
//    /// 体力商店表
//    /// </summary>
//    public static List<List<string>> tiLiStoreTableDatas;

//    /// <summary>
//    /// 敌人BOSS固定布阵表
//    /// </summary>
//    public static List<List<string>> enemyBOSSTableDatas;

//    /// <summary>
//    /// 文本内容表
//    /// </summary>
//    private static List<List<string>> stringTextTableDatas;

//    /// <summary>
//    /// 游戏数值表
//    /// </summary>
//    private static List<List<string>> numParametersTableDatas;

//    /// <summary>
//    /// 羁绊数据表
//    /// </summary>
//    public static List<List<string>> jiBanTableDatas;
    
//    /// <summary>
//    ///势力表 
//    /// </summary>
//    public static List<List<string>> shiLiTableDatas;

//    /// <summary>
//    ///霸业地图表 
//    /// </summary>
//    public static List<List<string>> baYeDiTuTableDatas;

//    /// <summary>
//    ///霸业事件表 
//    /// </summary>
//    public static List<List<string>> baYeShiJianTableDatas;

//    /// <summary>
//    ///霸业战役难度表 
//    /// </summary>
//    public static List<List<string>> baYeBattleTableDatas;
    
//    /// <summary>
//    ///霸业任务表 
//    /// </summary>
//    public static List<List<string>> baYeRenWuTableDatas;
    
//    /// <summary>
//    /// 霸业故事点表
//    /// </summary>
//    public static List<List<string>> storyPoolTableDatas;
    
//    /// <summary>
//    ///霸业故事id表 
//    /// </summary>
//    public static List<List<string>> storyIdTableDatas;

//    /// <summary>
//    /// 霸业TV表
//    /// </summary>
//    public static List<List<string>> baYeTVTableDatas;
//    /// <summary>
//    /// 霸业TV人名表
//    /// </summary>
//    public static List<List<string>> baYeNameTableDatas;
//    /// <summary>
//    /// 加载json文件获取数据至链表中
//    /// </summary>
//    private void JsonDataToSheets(string[] tableNames)
//    {
//        //Json数据控制类
//        Roots root = new Roots();
//        //存放json数据
//        string jsonData = string.Empty;
//        //记录读取到第几个表
//        int indexTable = 0;

//        // 加载势力初始数据:PlayerInitialTable
//        {
//            //读取json文件数据
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            //解析数据存放至Root中
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            //实例化数据存储链表
//            playerInitialTableDatas = new List<List<string>>(root.PlayerInitialTable.Count);
//            for (int i = 0; i < root.PlayerInitialTable.Count; i++)
//            {
//                //依次按属性存值
//                playerInitialTableDatas.Add(new List<string>());
//                playerInitialTableDatas[i].Add(root.PlayerInitialTable[i].id);
//                playerInitialTableDatas[i].Add(root.PlayerInitialTable[i].power);
//                playerInitialTableDatas[i].Add(root.PlayerInitialTable[i].initialHero);
//                playerInitialTableDatas[i].Add(root.PlayerInitialTable[i].initialSoldier);
//                playerInitialTableDatas[i].Add(root.PlayerInitialTable[i].initialTower);
//                playerInitialTableDatas[i].Add(root.PlayerInitialTable[i].powerIntro);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载基础资源类型数据:AssetTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            assetTableDatas = new List<AssetTableItem>(root.AssetTable.Count);
//            for (int i = 0; i < root.AssetTable.Count; i++)
//            {
//                assetTableDatas.Add(root.AssetTable[i]);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载武将表数据:HeroTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            heroTableDatas = new List<List<string>>(root.HeroTable.Count);
//            for (int i = 0; i < root.HeroTable.Count; i++)
//            {
//                heroTableDatas.Add(new List<string>());
//                heroTableDatas[i].Add(root.HeroTable[i].id);
//                heroTableDatas[i].Add(root.HeroTable[i].name);
//                heroTableDatas[i].Add(root.HeroTable[i].intro);
//                heroTableDatas[i].Add(root.HeroTable[i].rarity);
//                heroTableDatas[i].Add(root.HeroTable[i].price);
//                heroTableDatas[i].Add(root.HeroTable[i].classes);
//                heroTableDatas[i].Add(root.HeroTable[i].powers);
//                heroTableDatas[i].Add(root.HeroTable[i].damage);
//                heroTableDatas[i].Add(root.HeroTable[i].hp);
//                heroTableDatas[i].Add(root.HeroTable[i].hpr);
//                heroTableDatas[i].Add(root.HeroTable[i].dod);
//                heroTableDatas[i].Add(root.HeroTable[i].def);
//                heroTableDatas[i].Add(root.HeroTable[i].cri);
//                heroTableDatas[i].Add(root.HeroTable[i].criDamage);
//                heroTableDatas[i].Add(root.HeroTable[i].huixin);
//                heroTableDatas[i].Add(root.HeroTable[i].huixinDamage);
//                heroTableDatas[i].Add(root.HeroTable[i].icon);
//                heroTableDatas[i].Add(root.HeroTable[i].tag1);
//                heroTableDatas[i].Add(root.HeroTable[i].tag2);
//                heroTableDatas[i].Add(root.HeroTable[i].chestZyBox);
//                heroTableDatas[i].Add(root.HeroTable[i].chestBxBox);
//                heroTableDatas[i].Add(root.HeroTable[i].chanChu);
//                heroTableDatas[i].Add(root.HeroTable[i].fuMianMianYi);
//                heroTableDatas[i].Add(root.HeroTable[i].wuLiMianShang);
//                heroTableDatas[i].Add(root.HeroTable[i].faShuMianShang); 
//                heroTableDatas[i].Add(root.HeroTable[i].fettersRelated); 
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载玩家等级表数据:PlayerLevelTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            playerLevelTableDatas = new List<List<string>>(root.PlayerLevelTable.Count);
//            for (int i = 0; i < root.PlayerLevelTable.Count; i++)
//            {
//                playerLevelTableDatas.Add(new List<string>());
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].level);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].exp);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].heroLimit);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].companionLimit);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].homeHp);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].yuanZheng);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].unLockShiLi);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].BaYeCombat);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].BaYeNonCombat);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].BaYeBattle);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].BaYeJunTuan);
//                playerLevelTableDatas[i].Add(root.PlayerLevelTable[i].BaYeBattleLevel);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载士兵表数据:SoldierTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            soldierTableDatas = new List<List<string>>(root.SoldierTable.Count);
//            for (int i = 0; i < root.SoldierTable.Count; i++)
//            {
//                soldierTableDatas.Add(new List<string>());
//                soldierTableDatas[i].Add(root.SoldierTable[i].id);
//                soldierTableDatas[i].Add(root.SoldierTable[i].name);
//                soldierTableDatas[i].Add(root.SoldierTable[i].intro);
//                soldierTableDatas[i].Add(root.SoldierTable[i].rarity);
//                soldierTableDatas[i].Add(root.SoldierTable[i].price);
//                soldierTableDatas[i].Add(root.SoldierTable[i].classes);
//                soldierTableDatas[i].Add(root.SoldierTable[i].damage);
//                soldierTableDatas[i].Add(root.SoldierTable[i].hp);
//                soldierTableDatas[i].Add(root.SoldierTable[i].hpr);
//                soldierTableDatas[i].Add(root.SoldierTable[i].dod);
//                soldierTableDatas[i].Add(root.SoldierTable[i].def);
//                soldierTableDatas[i].Add(root.SoldierTable[i].cri);
//                soldierTableDatas[i].Add(root.SoldierTable[i].criDamage);
//                soldierTableDatas[i].Add(root.SoldierTable[i].icon);
//                soldierTableDatas[i].Add(root.SoldierTable[i].tag1);
//                soldierTableDatas[i].Add(root.SoldierTable[i].tag2);
//                soldierTableDatas[i].Add(root.SoldierTable[i].onShow);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载塔表数据:TowerTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            towerTableDatas = new List<List<string>>(root.TowerTable.Count);
//            for (int i = 0; i < root.TowerTable.Count; i++)
//            {
//                towerTableDatas.Add(new List<string>());
//                towerTableDatas[i].Add(root.TowerTable[i].id);
//                towerTableDatas[i].Add(root.TowerTable[i].name);
//                towerTableDatas[i].Add(root.TowerTable[i].intro);
//                towerTableDatas[i].Add(root.TowerTable[i].rarity);
//                towerTableDatas[i].Add(root.TowerTable[i].price);
//                towerTableDatas[i].Add(root.TowerTable[i].shortName);
//                towerTableDatas[i].Add(root.TowerTable[i].damage);
//                towerTableDatas[i].Add(root.TowerTable[i].hp);
//                towerTableDatas[i].Add(root.TowerTable[i].hpr);
//                towerTableDatas[i].Add(root.TowerTable[i].scope);
//                towerTableDatas[i].Add(root.TowerTable[i].icon);
//                towerTableDatas[i].Add(root.TowerTable[i].chestZyBox);
//                towerTableDatas[i].Add(root.TowerTable[i].intro2);
//                towerTableDatas[i].Add(root.TowerTable[i].chestBxBox);
//                towerTableDatas[i].Add(root.TowerTable[i].chanChu);
//                towerTableDatas[i].Add(root.TowerTable[i].shiLi);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载兵种表数据:ClassTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            classTableDatas = new List<List<string>>(root.ClassTable.Count);
//            for (int i = 0; i < root.ClassTable.Count; i++)
//            {
//                classTableDatas.Add(new List<string>());
//                classTableDatas[i].Add(root.ClassTable[i].id);
//                classTableDatas[i].Add(root.ClassTable[i].type);
//                classTableDatas[i].Add(root.ClassTable[i].skill);
//                classTableDatas[i].Add(root.ClassTable[i].shortName);
//                classTableDatas[i].Add(root.ClassTable[i].info); 
//                classTableDatas[i].Add(root.ClassTable[i].bingZhongXi); 
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载升级碎片表数据:UpGradeTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            upGradeTableDatas = new List<List<string>>(root.UpGradeTable.Count);
//            for (int i = 0; i < root.UpGradeTable.Count; i++)
//            {
//                upGradeTableDatas.Add(new List<string>());
//                upGradeTableDatas[i].Add(root.UpGradeTable[i].starLevel);
//                upGradeTableDatas[i].Add(root.UpGradeTable[i].partsDemand);
//                upGradeTableDatas[i].Add(root.UpGradeTable[i].yuanbaoDemand);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载辅助单位卡牌表数据:TrapTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            trapTableDatas = new List<List<string>>(root.TrapTable.Count);
//            for (int i = 0; i < root.TrapTable.Count; i++)
//            {
//                trapTableDatas.Add(new List<string>());
//                trapTableDatas[i].Add(root.TrapTable[i].id);
//                trapTableDatas[i].Add(root.TrapTable[i].name);
//                trapTableDatas[i].Add(root.TrapTable[i].intro);
//                trapTableDatas[i].Add(root.TrapTable[i].rarity);
//                trapTableDatas[i].Add(root.TrapTable[i].price);
//                trapTableDatas[i].Add(root.TrapTable[i].shortName);
//                trapTableDatas[i].Add(root.TrapTable[i].damage);
//                trapTableDatas[i].Add(root.TrapTable[i].hp);
//                trapTableDatas[i].Add(root.TrapTable[i].icon);
//                trapTableDatas[i].Add(root.TrapTable[i].chestZyBox);
//                trapTableDatas[i].Add(root.TrapTable[i].hpr);
//                trapTableDatas[i].Add(root.TrapTable[i].intro2);
//                trapTableDatas[i].Add(root.TrapTable[i].chestBxBox);
//                trapTableDatas[i].Add(root.TrapTable[i].chanChu);
//                trapTableDatas[i].Add(root.TrapTable[i].shiLi);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载辅助技能表数据:SpellTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            spellTableDatas = new List<List<string>>(root.SpellTable.Count);
//            for (int i = 0; i < root.SpellTable.Count; i++)
//            {
//                spellTableDatas.Add(new List<string>());
//                spellTableDatas[i].Add(root.SpellTable[i].id);
//                spellTableDatas[i].Add(root.SpellTable[i].name);
//                spellTableDatas[i].Add(root.SpellTable[i].intro);
//                spellTableDatas[i].Add(root.SpellTable[i].rarity);
//                spellTableDatas[i].Add(root.SpellTable[i].price);
//                spellTableDatas[i].Add(root.SpellTable[i].shortName);
//                spellTableDatas[i].Add(root.SpellTable[i].icon);
//                spellTableDatas[i].Add(root.SpellTable[i].onShow);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载宝箱表数据:WarChestTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            warChestTableDatas = new List<List<string>>(root.WarChestTable.Count);
//            for (int i = 0; i < root.WarChestTable.Count; i++)
//            {
//                warChestTableDatas.Add(new List<string>());
//                warChestTableDatas[i].Add(root.WarChestTable[i].id);
//                warChestTableDatas[i].Add(root.WarChestTable[i].chestType);
//                warChestTableDatas[i].Add(root.WarChestTable[i].chestName);
//                warChestTableDatas[i].Add(root.WarChestTable[i].exp);
//                warChestTableDatas[i].Add(root.WarChestTable[i].shuoyu);
//                warChestTableDatas[i].Add(root.WarChestTable[i].yuanbao);
//                warChestTableDatas[i].Add(root.WarChestTable[i].trap);
//                warChestTableDatas[i].Add(root.WarChestTable[i].tower);
//                warChestTableDatas[i].Add(root.WarChestTable[i].hero);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载战役表数据:WarTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            warTableDatas = new List<List<string>>(root.WarTable.Count);
//            for (int i = 0; i < root.WarTable.Count; i++)
//            {
//                warTableDatas.Add(new List<string>());
//                warTableDatas[i].Add(root.WarTable[i].warId);
//                warTableDatas[i].Add(root.WarTable[i].warName);
//                warTableDatas[i].Add(root.WarTable[i].warIntro);
//                warTableDatas[i].Add(root.WarTable[i].startPoint);
//                warTableDatas[i].Add(root.WarTable[i].counts);
//                warTableDatas[i].Add(root.WarTable[i].reward1);
//                warTableDatas[i].Add(root.WarTable[i].reward2);
//                warTableDatas[i].Add(root.WarTable[i].reward3);
//                warTableDatas[i].Add(root.WarTable[i].YuanBaoR);
//                warTableDatas[i].Add(root.WarTable[i].YuQueR);
//                warTableDatas[i].Add(root.WarTable[i].TiLiR);
//                warTableDatas[i].Add(root.WarTable[i].JunTuan);

//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载城池等级表数据:CityLevelTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            cityLevelTableDatas = new List<List<string>>(root.CityLevelTable.Count);
//            for (int i = 0; i < root.CityLevelTable.Count; i++)
//            {
//                cityLevelTableDatas.Add(new List<string>());
//                cityLevelTableDatas[i].Add(root.CityLevelTable[i].cityLevel);
//                cityLevelTableDatas[i].Add(root.CityLevelTable[i].cost);
//                cityLevelTableDatas[i].Add(root.CityLevelTable[i].cityDur);
//                cityLevelTableDatas[i].Add(root.CityLevelTable[i].heroFightLimit);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载关卡表数据:PointTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            pointTableDatas = new List<List<string>>(root.PointTable.Count);
//            for (int i = 0; i < root.PointTable.Count; i++)
//            {
//                pointTableDatas.Add(new List<string>());
//                pointTableDatas[i].Add(root.PointTable[i].pointId);
//                pointTableDatas[i].Add(root.PointTable[i].nextPoint);
//                pointTableDatas[i].Add(root.PointTable[i].pointName);
//                pointTableDatas[i].Add(root.PointTable[i].eventType);
//                pointTableDatas[i].Add(root.PointTable[i].eventId);
//                pointTableDatas[i].Add(root.PointTable[i].pointStory);
//                pointTableDatas[i].Add(root.PointTable[i].cityIcon);
//                pointTableDatas[i].Add(root.PointTable[i].battleBG);
//                pointTableDatas[i].Add(root.PointTable[i].battleBGM);
//                pointTableDatas[i].Add(root.PointTable[i].flag);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载战斗事件表数据:BattleEventTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            battleEventTableDatas = new List<List<string>>(root.BattleEventTable.Count);
//            for (int i = 0; i < root.BattleEventTable.Count; i++)
//            {
//                battleEventTableDatas.Add(new List<string>());
//                battleEventTableDatas[i].Add(root.BattleEventTable[i].battleId);
//                battleEventTableDatas[i].Add(root.BattleEventTable[i].enemyPower);
//                battleEventTableDatas[i].Add(root.BattleEventTable[i].enemyId);
//                battleEventTableDatas[i].Add(root.BattleEventTable[i].homeHp);
//                battleEventTableDatas[i].Add(root.BattleEventTable[i].warChest);
//                battleEventTableDatas[i].Add(root.BattleEventTable[i].goldReward);
//                battleEventTableDatas[i].Add(root.BattleEventTable[i].type);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载敌方位置表数据:EnemyTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            enemyTableDatas = new List<List<string>>(root.EnemyTable.Count);
//            for (int i = 0; i < root.EnemyTable.Count; i++)
//            {
//                enemyTableDatas.Add(new List<string>());
//                enemyTableDatas[i].Add(root.EnemyTable[i].enemyId);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos1);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos2);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos3);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos4);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos5);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos6);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos7);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos8);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos9);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos10);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos11);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos12);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos13);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos14);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos15);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos16);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos17);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos18);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos19);
//                enemyTableDatas[i].Add(root.EnemyTable[i].pos20);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载敌方信息表数据:EnemyUnitTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            enemyUnitTableDatas = new List<List<string>>(root.EnemyUnitTable.Count);
//            for (int i = 0; i < root.EnemyUnitTable.Count; i++)
//            {
//                enemyUnitTableDatas.Add(new List<string>());
//                enemyUnitTableDatas[i].Add(root.EnemyUnitTable[i].enemyUnit);
//                enemyUnitTableDatas[i].Add(root.EnemyUnitTable[i].unitType);
//                enemyUnitTableDatas[i].Add(root.EnemyUnitTable[i].rarity);
//                enemyUnitTableDatas[i].Add(root.EnemyUnitTable[i].unitLevel);
//                enemyUnitTableDatas[i].Add(root.EnemyUnitTable[i].goldReward);
//                enemyUnitTableDatas[i].Add(root.EnemyUnitTable[i].warChest);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载故事数据:StoryTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            storyTableDatas = new List<List<string>>(root.StoryTable.Count);
//            for (int i = 0; i < root.StoryTable.Count; i++)
//            {
//                storyTableDatas.Add(new List<string>());
//                storyTableDatas[i].Add(root.StoryTable[i].id);
//                storyTableDatas[i].Add(root.StoryTable[i].story);
//                storyTableDatas[i].Add(root.StoryTable[i].storyBody);
//                storyTableDatas[i].Add(root.StoryTable[i].exitText);
//                storyTableDatas[i].Add(root.StoryTable[i].option1);
//                storyTableDatas[i].Add(root.StoryTable[i].option2);
//                storyTableDatas[i].Add(root.StoryTable[i].option1ToEnding);
//                storyTableDatas[i].Add(root.StoryTable[i].option2ToEnding);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载故事奖励数据：StoryRTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            storyRTableDatas = new List<List<string>>(root.StoryRTable.Count);
//            for (int i = 0; i < root.StoryRTable.Count; i++)
//            {
//                storyRTableDatas.Add(new List<string>());
//                storyRTableDatas[i].Add(root.StoryRTable[i].id);
//                storyRTableDatas[i].Add(root.StoryRTable[i].ending);
//                storyRTableDatas[i].Add(root.StoryRTable[i].unitType);
//                storyRTableDatas[i].Add(root.StoryRTable[i].unitId);
//                storyRTableDatas[i].Add(root.StoryRTable[i].unitLevel);
//                storyRTableDatas[i].Add(root.StoryRTable[i].unitCount);
//                storyRTableDatas[i].Add(root.StoryRTable[i].goldReward);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载答题数据：TestTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            testTableDatas = new List<List<string>>(root.TestTable.Count);
//            for (int i = 0; i < root.TestTable.Count; i++)
//            {
//                testTableDatas.Add(new List<string>());
//                testTableDatas[i].Add(root.TestTable[i].id);
//                testTableDatas[i].Add(root.TestTable[i].question);
//                testTableDatas[i].Add(root.TestTable[i].truth);
//                testTableDatas[i].Add(root.TestTable[i].answer1);
//                testTableDatas[i].Add(root.TestTable[i].answer2);
//                testTableDatas[i].Add(root.TestTable[i].answer3);
//                testTableDatas[i].Add(root.TestTable[i].weightValue);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载答题奖励数据：TestRTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            testRTableDatas = new List<List<string>>(root.TestRTable.Count);
//            for (int i = 0; i < root.TestRTable.Count; i++)
//            {
//                testRTableDatas.Add(new List<string>());
//                testRTableDatas[i].Add(root.TestRTable[i].id);
//                testRTableDatas[i].Add(root.TestRTable[i].weightValue);
//                testRTableDatas[i].Add(root.TestRTable[i].type);
//                testRTableDatas[i].Add(root.TestRTable[i].rarity);
//                testRTableDatas[i].Add(root.TestRTable[i].level);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载三选单位数据：EncounterTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            encounterTableDatas = new List<List<string>>(root.EncounterTable.Count);
//            for (int i = 0; i < root.EncounterTable.Count; i++)
//            {
//                encounterTableDatas.Add(new List<string>());
//                encounterTableDatas[i].Add(root.EncounterTable[i].id);
//                encounterTableDatas[i].Add(root.EncounterTable[i].weightValue);
//                encounterTableDatas[i].Add(root.EncounterTable[i].type);
//                encounterTableDatas[i].Add(root.EncounterTable[i].rarity);
//                encounterTableDatas[i].Add(root.EncounterTable[i].level);
//                encounterTableDatas[i].Add(root.EncounterTable[i].cost);

//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载购买单位数据：ShoppingTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            shoppingTableDatas = new List<List<string>>(root.ShoppingTable.Count);
//            for (int i = 0; i < root.ShoppingTable.Count; i++)
//            {
//                shoppingTableDatas.Add(new List<string>());
//                shoppingTableDatas[i].Add(root.ShoppingTable[i].id);
//                shoppingTableDatas[i].Add(root.ShoppingTable[i].weightValue);
//                shoppingTableDatas[i].Add(root.ShoppingTable[i].type);
//                shoppingTableDatas[i].Add(root.ShoppingTable[i].rarity);
//                shoppingTableDatas[i].Add(root.ShoppingTable[i].level);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载难度选择表数据：ChoseWarTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            choseWarTableDatas = new List<List<string>>(root.ChoseWarTable.Count);
//            for (int i = 0; i < root.ChoseWarTable.Count; i++)
//            {
//                choseWarTableDatas.Add(new List<string>());
//                choseWarTableDatas[i].Add(root.ChoseWarTable[i].id);
//                choseWarTableDatas[i].Add(root.ChoseWarTable[i].difficulty);
//                choseWarTableDatas[i].Add(root.ChoseWarTable[i].warList);
//                choseWarTableDatas[i].Add(root.ChoseWarTable[i].unlock);
//                choseWarTableDatas[i].Add(root.ChoseWarTable[i].tiliCost);
//                choseWarTableDatas[i].Add(root.ChoseWarTable[i].instructions);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载新手引导配置表数据：GuideTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            guideTableDatas = new List<List<string>>(root.GuideTable.Count);
//            for (int i = 0; i < root.GuideTable.Count; i++)
//            {
//                guideTableDatas.Add(new List<string>());
//                guideTableDatas[i].Add(root.GuideTable[i].id);
//                guideTableDatas[i].Add(root.GuideTable[i].title);
//                guideTableDatas[i].Add(root.GuideTable[i].story);
//                guideTableDatas[i].Add(root.GuideTable[i].card1);
//                guideTableDatas[i].Add(root.GuideTable[i].card2);
//                guideTableDatas[i].Add(root.GuideTable[i].card3);
//                guideTableDatas[i].Add(root.GuideTable[i].card4);
//                guideTableDatas[i].Add(root.GuideTable[i].card5);
//                guideTableDatas[i].Add(root.GuideTable[i].homeHp1);
//                guideTableDatas[i].Add(root.GuideTable[i].homeHp2);
//                guideTableDatas[i].Add(root.GuideTable[i].pos1);
//                guideTableDatas[i].Add(root.GuideTable[i].pos2);
//                guideTableDatas[i].Add(root.GuideTable[i].pos3);
//                guideTableDatas[i].Add(root.GuideTable[i].pos4);
//                guideTableDatas[i].Add(root.GuideTable[i].pos5);
//                guideTableDatas[i].Add(root.GuideTable[i].pos6);
//                guideTableDatas[i].Add(root.GuideTable[i].pos7);
//                guideTableDatas[i].Add(root.GuideTable[i].pos8);
//                guideTableDatas[i].Add(root.GuideTable[i].pos9);
//                guideTableDatas[i].Add(root.GuideTable[i].pos10);
//                guideTableDatas[i].Add(root.GuideTable[i].pos11);
//                guideTableDatas[i].Add(root.GuideTable[i].pos12);
//                guideTableDatas[i].Add(root.GuideTable[i].pos13);
//                guideTableDatas[i].Add(root.GuideTable[i].pos14);
//                guideTableDatas[i].Add(root.GuideTable[i].pos15);
//                guideTableDatas[i].Add(root.GuideTable[i].pos16);
//                guideTableDatas[i].Add(root.GuideTable[i].pos17);
//                guideTableDatas[i].Add(root.GuideTable[i].pos18);
//                guideTableDatas[i].Add(root.GuideTable[i].pos19);
//                guideTableDatas[i].Add(root.GuideTable[i].pos20);
//                guideTableDatas[i].Add(root.GuideTable[i].epos1);
//                guideTableDatas[i].Add(root.GuideTable[i].epos2);
//                guideTableDatas[i].Add(root.GuideTable[i].epos3);
//                guideTableDatas[i].Add(root.GuideTable[i].epos4);
//                guideTableDatas[i].Add(root.GuideTable[i].epos5);
//                guideTableDatas[i].Add(root.GuideTable[i].epos6);
//                guideTableDatas[i].Add(root.GuideTable[i].epos7);
//                guideTableDatas[i].Add(root.GuideTable[i].epos8);
//                guideTableDatas[i].Add(root.GuideTable[i].epos9);
//                guideTableDatas[i].Add(root.GuideTable[i].epos10);
//                guideTableDatas[i].Add(root.GuideTable[i].epos11);
//                guideTableDatas[i].Add(root.GuideTable[i].epos12);
//                guideTableDatas[i].Add(root.GuideTable[i].epos13);
//                guideTableDatas[i].Add(root.GuideTable[i].epos14);
//                guideTableDatas[i].Add(root.GuideTable[i].epos15);
//                guideTableDatas[i].Add(root.GuideTable[i].epos16);
//                guideTableDatas[i].Add(root.GuideTable[i].epos17);
//                guideTableDatas[i].Add(root.GuideTable[i].epos18);
//                guideTableDatas[i].Add(root.GuideTable[i].epos19);
//                guideTableDatas[i].Add(root.GuideTable[i].epos20);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载酒馆锦囊表数据：KnowledgeTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            knowledgeTableDatas = new List<List<string>>(root.KnowledgeTable.Count);
//            for (int i = 0; i < root.KnowledgeTable.Count; i++)
//            {
//                knowledgeTableDatas.Add(new List<string>());
//                knowledgeTableDatas[i].Add(root.KnowledgeTable[i].id);
//                knowledgeTableDatas[i].Add(root.KnowledgeTable[i].type);
//                knowledgeTableDatas[i].Add(root.KnowledgeTable[i].textStr);
//                knowledgeTableDatas[i].Add(root.KnowledgeTable[i].tiLiR);
//                knowledgeTableDatas[i].Add(root.KnowledgeTable[i].yuanBaoR);
//                knowledgeTableDatas[i].Add(root.KnowledgeTable[i].Name);

//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载兑换码表数据：RCodeTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            rCodeTableDatas = new List<List<string>>(root.RCodeTable.Count);
//            for (int i = 0; i < root.RCodeTable.Count; i++)
//            {
//                rCodeTableDatas.Add(new List<string>());
//                rCodeTableDatas[i].Add(root.RCodeTable[i].id);
//                rCodeTableDatas[i].Add(root.RCodeTable[i].code);
//                rCodeTableDatas[i].Add(root.RCodeTable[i].TheLastTime);
//                rCodeTableDatas[i].Add(root.RCodeTable[i].Info);
//                rCodeTableDatas[i].Add(root.RCodeTable[i].YuQue);
//                rCodeTableDatas[i].Add(root.RCodeTable[i].YuanBao);
//                rCodeTableDatas[i].Add(root.RCodeTable[i].TiLi);
//                rCodeTableDatas[i].Add(root.RCodeTable[i].SuiPian);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载体力商店表数据：TiLiStoreTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            tiLiStoreTableDatas = new List<List<string>>(root.TiLiStoreTable.Count);
//            for (int i = 0; i < root.TiLiStoreTable.Count; i++)
//            {
//                tiLiStoreTableDatas.Add(new List<string>());
//                tiLiStoreTableDatas[i].Add(root.TiLiStoreTable[i].id);
//                tiLiStoreTableDatas[i].Add(root.TiLiStoreTable[i].TiLi);
//                tiLiStoreTableDatas[i].Add(root.TiLiStoreTable[i].YuQue);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载敌方BOSS固定位置表数据:EnemyBOSSTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            enemyBOSSTableDatas = new List<List<string>>(root.EnemyBOSSTable.Count);
//            for (int i = 0; i < root.EnemyBOSSTable.Count; i++)
//            {
//                enemyBOSSTableDatas.Add(new List<string>());
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].enemyId);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos1);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos2);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos3);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos4);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos5);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos6);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos7);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos8);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos9);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos10);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos11);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos12);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos13);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos14);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos15);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos16);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos17);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos18);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos19);
//                enemyBOSSTableDatas[i].Add(root.EnemyBOSSTable[i].pos20);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载文本内容表数据：StringTextTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            stringTextTableDatas = new List<List<string>>(root.StringTextTable.Count);
//            for (int i = 0; i < root.StringTextTable.Count; i++)
//            {
//                stringTextTableDatas.Add(new List<string>());
//                stringTextTableDatas[i].Add(root.StringTextTable[i].id);
//                stringTextTableDatas[i].Add(root.StringTextTable[i].stringContent);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载游戏数值表数据：NumParametersTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            numParametersTableDatas = new List<List<string>>(root.NumParametersTable.Count);
//            for (int i = 0; i < root.NumParametersTable.Count; i++)
//            {
//                numParametersTableDatas.Add(new List<string>());
//                numParametersTableDatas[i].Add(root.NumParametersTable[i].id);
//                numParametersTableDatas[i].Add(root.NumParametersTable[i].numContent);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载羁绊数据表数据：JiBanTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            jiBanTableDatas = new List<List<string>>(root.JiBanTable.Count);
//            for (int i = 0; i < root.JiBanTable.Count; i++)
//            {
//                jiBanTableDatas.Add(new List<string>());
//                jiBanTableDatas[i].Add(root.JiBanTable[i].id);
//                jiBanTableDatas[i].Add(root.JiBanTable[i].jiBanMing);
//                jiBanTableDatas[i].Add(root.JiBanTable[i].isOpen);
//                jiBanTableDatas[i].Add(root.JiBanTable[i].heroId);
//                jiBanTableDatas[i].Add(root.JiBanTable[i].jiBanXiaoGuo);
//                jiBanTableDatas[i].Add(root.JiBanTable[i].BOSSId);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载势力表数据：ShiLiTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            shiLiTableDatas = new List<List<string>>(root.ShiLiTable.Count);
//            for (int i = 0; i < root.ShiLiTable.Count; i++)
//            {
//                shiLiTableDatas.Add(new List<string>());
//                shiLiTableDatas[i].Add(root.ShiLiTable[i].id);
//                shiLiTableDatas[i].Add(root.ShiLiTable[i].ShiLi);
//                shiLiTableDatas[i].Add(root.ShiLiTable[i].Zi);
//                shiLiTableDatas[i].Add(root.ShiLiTable[i].Qi);
//                shiLiTableDatas[i].Add(root.ShiLiTable[i].UnlockLevel);
//                shiLiTableDatas[i].Add(root.ShiLiTable[i].JunTuanInfo);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载霸业地图表数据：BaYeDiTuTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            baYeDiTuTableDatas = new List<List<string>>(root.BaYeDiTuTable.Count);
//            for (int i = 0; i < root.BaYeDiTuTable.Count; i++)
//            {
//                baYeDiTuTableDatas.Add(new List<string>());
//                baYeDiTuTableDatas[i].Add(root.BaYeDiTuTable[i].id);
//                baYeDiTuTableDatas[i].Add(root.BaYeDiTuTable[i].type);
//                baYeDiTuTableDatas[i].Add(root.BaYeDiTuTable[i].events);
//                baYeDiTuTableDatas[i].Add(root.BaYeDiTuTable[i].name);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载霸业事件表数据：BaYeShiJianTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            baYeShiJianTableDatas = new List<List<string>>(root.BaYeShiJianTable.Count);
//            for (int i = 0; i < root.BaYeShiJianTable.Count; i++)
//            {
//                baYeShiJianTableDatas.Add(new List<string>());
//                baYeShiJianTableDatas[i].Add(root.BaYeShiJianTable[i].id);
//                baYeShiJianTableDatas[i].Add(root.BaYeShiJianTable[i].QuanZhong);
//                baYeShiJianTableDatas[i].Add(root.BaYeShiJianTable[i].BaYeJingYan);
//                baYeShiJianTableDatas[i].Add(root.BaYeShiJianTable[i].BaYeBattle);
//                baYeShiJianTableDatas[i].Add(root.BaYeShiJianTable[i].Flag);
//                baYeShiJianTableDatas[i].Add(root.BaYeShiJianTable[i].FlagWord);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载霸业战役难度表数据：BaYeBattleTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            baYeBattleTableDatas = new List<List<string>>(root.BaYeBattleTable.Count);
//            for (int i = 0; i < root.BaYeBattleTable.Count; i++)
//            {
//                baYeBattleTableDatas.Add(new List<string>());
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].id);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level0);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level1);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level2);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level3);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level4);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level5);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level6);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level7);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level8);
//                baYeBattleTableDatas[i].Add(root.BaYeBattleTable[i].level9);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载霸业任务数据：BaYeRenWuTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            baYeRenWuTableDatas = new List<List<string>>(root.BaYeRenWuTable.Count);
//            for (int i = 0; i < root.BaYeRenWuTable.Count; i++) 
//            {
//                baYeRenWuTableDatas.Add(new List<string>());
//                baYeRenWuTableDatas[i].Add(root.BaYeRenWuTable[i].id);
//                baYeRenWuTableDatas[i].Add(root.BaYeRenWuTable[i].jingYan);
//                baYeRenWuTableDatas[i].Add(root.BaYeRenWuTable[i].jiangLi);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载霸业故事点数据：StoryPoolTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            storyPoolTableDatas = new List<List<string>>(root.StoryPoolTable.Count);
//            for (int i = 0; i < root.StoryPoolTable.Count; i++)
//            {
//                storyPoolTableDatas.Add(new List<string>());
//                storyPoolTableDatas[i].Add(root.StoryPoolTable[i].poolId);
//                storyPoolTableDatas[i].Add(root.StoryPoolTable[i].storyId);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载故事id数据：StoryIdTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            storyIdTableDatas = new List<List<string>>(root.StoryIdTable.Count);
//            for (int i = 0; i < root.StoryIdTable.Count; i++)
//            {
//                storyIdTableDatas.Add(new List<string>());
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].storyId);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].weight);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].storyType);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].jinBiR);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].jingYanR);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].YuQueR);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].YuanBaoR);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].JunTuanR);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].Time);
//                storyIdTableDatas[i].Add(root.StoryIdTable[i].WarId);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载霸业TV数据：BaYeTVTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            baYeTVTableDatas = new List<List<string>>(root.BaYeTVTable.Count);
//            for (int i = 0; i < root.BaYeTVTable.Count; i++)
//            {
//                baYeTVTableDatas.Add(new List<string>());
//                baYeTVTableDatas[i].Add(root.BaYeTVTable[i].id);
//                baYeTVTableDatas[i].Add(root.BaYeTVTable[i].weight);
//                baYeTVTableDatas[i].Add(root.BaYeTVTable[i].text);
//                baYeTVTableDatas[i].Add(root.BaYeTVTable[i].time);
//                baYeTVTableDatas[i].Add(root.BaYeTVTable[i].type);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }
//        //加载霸业TV人名表数据：BaYeNameTable
//        {
//            jsonData = LoadJsonByName(tableNames[indexTable]);
//            root = JsonConvert.DeserializeObject<Roots>(jsonData);
//            baYeNameTableDatas = new List<List<string>>(root.BaYeNameTable.Count);
//            for (int i = 0; i < root.BaYeNameTable.Count; i++)
//            {
//                baYeNameTableDatas.Add(new List<string>());
//                baYeNameTableDatas[i].Add(root.BaYeNameTable[i].id);
//                baYeNameTableDatas[i].Add(root.BaYeNameTable[i].weight);
//                baYeNameTableDatas[i].Add(root.BaYeNameTable[i].name);
//            }
//            //Debug.Log("Json文件加载成功---" + tableNames[indexTable] + ".Json");
//            indexTable++;
//        }

//        if (indexTable >= tableNames.Length)
//        {
//            //Debug.Log("所有Json数据加载成功。");
//        }
//        else
//        {
//            //Debug.Log("还有Json数据未进行加载。");
//        }
//    }

//    /// <summary>
//    /// 深拷贝List等
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    /// <param name="List"></param>
//    /// <returns></returns>
//    public static List<T> DeepClone<T>(object List)
//    {
//        using (System.IO.Stream objectStream = new System.IO.MemoryStream())
//        {
//            IFormatter formatter = new BinaryFormatter();
//            formatter.Serialize(objectStream, List);
//            objectStream.Seek(0, System.IO.SeekOrigin.Begin);
//            return formatter.Deserialize(objectStream) as List<T>;
//        }
//    }

//    /// <summary>
//    /// 根据id获取文本内容
//    /// </summary>
//    /// <param name="id"></param>
//    /// <returns></returns>
//    public static string GetStringText(int id) => stringTextTableDatas[id][1];

//    /// <summary>
//    /// 根据id获取游戏数值内容
//    /// </summary>
//    /// <param name="id"></param>
//    /// <returns></returns>
//    public static int GetGameValue(int id) => int.Parse(numParametersTableDatas[id][1]);


//    private void Awake()
//    {
//        if (instance != null)
//        {
//            Destroy(gameObject);
//        }
//        else
//        {
//            instance = this;
//        }
//        DontDestroyOnLoad(gameObject);

//        string[] arrStr = tableNameStrs.Split(';');
//        if (arrStr.Length > 0)
//        {
//            JsonDataToSheets(arrStr);  //传递Json文件名进行加载
//        }
//        else
//        {
//            //Debug.Log("////请检查Json表名");
//        }
//        DontDestroyOnLoad(gameObject);//跳转场景等不销毁
//    }


//    /// <summary>
//    /// 通过json文件名获取json数据
//    /// </summary>
//    /// <param name="fileName"></param>
//    /// <returns></returns>
//    public static string LoadJsonByName(string fileName)
//    {
//        string path = string.Empty;
//        string data = string.Empty;
//        if (Application.isPlaying)
//        {
//            path = System.IO.Path.Combine(Folder, fileName);  //合并文件路径
//            var asset = Resources.Load<TextAsset>(path);
//            //Debug.Log("Loading..." + fileName + "\nFrom:" + path);
//            if (asset == null)
//            {
//                Debug.LogError("No text asset could be found at resource path: " + path);
//                return null;
//            }
//            data = asset.text;
//            Resources.UnloadAsset(asset);
//        }
//        else
//        {
//#if UNITY_EDITOR
//            path = Application.dataPath + "/Resources/" + Folder + "/" + fileName + ".json";
//            //Debug.Log("Loading JsonFile " + fileName + " from: " + path);
//            var asset1 = System.IO.File.ReadAllText(path);
//            data = asset1;
//#endif
//        }
//        return data;
//    }

//}