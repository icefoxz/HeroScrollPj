using UnityEngine;

/// <summary>
/// 颜色数据类
/// </summary>
public static class ColorDataStatic
{
    /// <summary>
    /// 升星绿色
    /// </summary>
    public static readonly Color deep_green = new Color(1f / 255f, 188f / 255f, 1f / 255f, 1);

    /// <summary>
    /// 灰色
    /// </summary>
    public static readonly Color name_gray = new Color(109f / 255f, 109f / 255f, 109f / 255f, 1);

    /// <summary>
    /// 灰色
    /// </summary>
    public static readonly Color name_black = new Color(0, 0, 0, 1);

    /// <summary>
    /// 绿色
    /// </summary>
    public static readonly Color name_green = new Color(27f / 255f, 105f / 255f, 0f / 255f, 1);

    /// <summary>
    /// 蓝色
    /// </summary>
    public static readonly Color name_blue = new Color(0f / 255f, 44f / 255f, 207f / 255f, 1);

    /// <summary>
    /// 紫色
    /// </summary>
    public static readonly Color name_purple = new Color(182f / 255f, 0f / 255f, 133f / 255f, 1);

    /// <summary>
    /// 橙色
    /// </summary>
    public static readonly Color name_orange = new Color(255f / 255f, 78f / 255f, 0f / 255f, 1);

    /// <summary>
    /// 红色
    /// </summary>
    public static readonly Color name_red = new Color(199f / 255f, 0f / 255f, 0f / 255f, 1);

    /// <summary>
    /// 暗红色
    /// </summary>
    public static readonly Color name_deepRed = new Color(199f / 255f, 0f / 255f, 0f / 255f, 1);

    /// <summary>
    /// 棕色
    /// </summary>
    public static readonly Color name_brown = new Color(255f / 255f, 78f / 255f, 0f / 255f, 1);

    /// <summary>
    /// 关卡按钮置灰
    /// </summary>
    public static readonly Color light_gray = new Color(75f / 255f, 75f / 255f, 75f / 255f, 1);

    /// <summary>
    /// 回复生命绿色
    /// </summary>
    public static readonly Color huiFu_green = new Color(0f / 255f, 152f / 255f, 0f / 255f, 1);
}

/// <summary>
/// 兵种状态名等类
/// </summary>
public static class StringNameStatic
{
    /// <summary>
    /// 铁骑连环状态图标
    /// </summary>
    public static readonly string StateIconPath_lianHuan = "lianHuanState";

    /// <summary>
    /// 眩晕状态图标
    /// </summary>
    public static readonly string StateIconPath_dizzy = "dizzyState";

    /// <summary>
    /// 坚盾状态图标
    /// </summary>
    public static readonly string StateIconPath_withStand = "withStandState";

    /// <summary>
    /// 无敌状态图标
    /// </summary>
    public static readonly string StateIconPath_invincible = "invincibleState";

    /// <summary>
    /// 灼烧状态图标
    /// </summary>
    public static readonly string StateIconPath_burned = "burnedState";

    /// <summary>
    /// 中毒状态图标
    /// </summary>
    public static readonly string StateIconPath_poisoned = "poisonedState";

    /// <summary>
    /// 流血状态图标
    /// </summary>
    public static readonly string StateIconPath_bleed = "bleedState";

    /// <summary>
    /// 禁锢状态图标
    /// </summary>
    public static readonly string StateIconPath_imprisoned = "imprisonedState";

    /// <summary>
    /// 怯战状态图标
    /// </summary>
    public static readonly string StateIconPath_cowardly = "cowardlyState";

    /// <summary>
    /// 卸甲状态图标
    /// </summary>
    public static readonly string StateIconPath_removeArmor = "removeArmorState";

    /// <summary>
    /// 内助状态图标
    /// </summary>
    public static readonly string StateIconPath_neizhu = "neizhuState";

    /// <summary>
    /// 神助状态图标
    /// </summary>
    public static readonly string StateIconPath_shenzhu = "shenzhuState";

    /// <summary>
    /// 神武兵种战意状态图标
    /// </summary>
    public static readonly string StateIconPath_willFight = "willFightState";
    
    /// <summary>
    /// 防护盾状态(辅佐)图标
    /// </summary>
    public static readonly string StateIconPath_shield = "shieldState";

    /// <summary>
    /// 战鼓台加成状态图标 10-13伤害加成塔
    /// </summary>
    public static readonly string StateIconPath_zhangutaiAddtion = "zhangutaiAddtion";

    /// <summary>
    /// 风神台加成状态图标
    /// </summary>
    public static readonly string StateIconPath_fengShenTaiAddtion = "fengShenTaiAddtion";

    /// <summary>
    /// 霹雳台加成状态图标
    /// </summary>
    public static readonly string StateIconPath_pilitaiAddtion = "pilitaiAddtion";

    /// <summary>
    /// 狼牙台加成状态图标
    /// </summary>
    public static readonly string StateIconPath_langyataiAddtion = "langyataiAddtion";

    /// <summary>
    /// 烽火台加成状态图标
    /// </summary>
    public static readonly string StateIconPath_fenghuotaiAddtion = "fenghuotaiAddtion";

    /// <summary>
    /// 奏乐台加成状态图标
    /// </summary>
    public static readonly string StateIconPath_zouyuetaiAddtion = "zouyuetaiAddtion";
    
    /// <summary>
    /// 迷雾阵加成状态图标
    /// </summary>
    public static readonly string StateIconPath_miWuZhenAddtion = "miWuZhenAddtion";

    /// <summary>
    /// 死战状态图标
    /// </summary>
    public static readonly string StateIconPath_deathFight = "deathFightState";
}

/// <summary>
/// 新手指引存档文本
/// </summary>
public static class StringForGuide
{
    /// <summary>
    /// 金宝箱
    /// </summary>
    public static readonly string guideJinBaoXiang = "JinBaoXiang";

    /// <summary>
    /// 战役宝箱
    /// </summary>
    public static readonly string guideZYBaoXiang = "ZYBaoXiang";

    /// <summary>
    /// 合成卡牌
    /// </summary>
    public static readonly string guideHeCheng = "HeChengCard";

    /// <summary>
    /// 开始战役
    /// </summary>
    public static readonly string guideStartZY = "StartZY";

    /// <summary>
    /// 开始关卡
    /// </summary>
    public static readonly string guideStartGQ = "StartGQ";

    /// <summary>
    /// 查看奇遇单位详情
    /// </summary>
    public static readonly string guideCheckCardInfo = "CheckInfo";

    /// <summary>
    /// 战役中升级战鼓
    /// </summary>
    public static readonly string guideShengJIZG = "ShengJiZG";
}

/// <summary>
/// 代码相应提示字符
/// </summary>
public static class StringForEditor
{
    /// <summary>
    /// 错误
    /// </summary>
    public static readonly string ERROR = "ERROR";

    
}

/// <summary>
/// 登录相关方法索引
/// </summary>
public enum LoginFunIndex
{
    /// <summary>
    /// 0 申请一个账号
    /// </summary>
    CREATE_ACCOUNT_NAME,
    /// <summary>
    /// 1 创建账号
    /// </summary>
    CREATE_ACCOUNT,
    /// <summary>
    /// 2 绑定手机
    /// </summary>
    BIND_PHONE,
    /// <summary>
    /// 3 账号登陆
    /// </summary>
    ACCOUNT_LOGIN,
    /// <summary>
    /// 4 上传存档
    /// </summary>
    UPLOAD_ARCHIVE
}

/// <summary>
/// 服务器返回的状态码
/// </summary>
public enum ServerBackCode
{
    SUCCESS = 0,
    ERR_NAME_EXIST = 1001,
    ERR_NAME_SHORT = 1002,
    /// <summary>
    /// 密码过短
    /// </summary>
    ERR_PASS_SHORT = 1003,
    /// <summary>
    /// 账号不存在
    /// </summary>
    ERR_NAME_NOT_EXIST = 1004,
    ERR_DATA_NOT_EXIST = 1005,
    ERR_PHONE_SHORT = 1006,
    /// <summary>
    /// 重复绑定手机
    /// </summary>
    ERR_ACCOUNT_BIND_OTHER_PHONE = 1007,
    ERR_NAME_ILLEGAL = 1008,
    /// <summary>
    /// 手机号错误 
    /// </summary>
    ERR_PHONE_ILLEGAL = 1009,
    /// <summary>
    /// 密码错误
    /// </summary>
    ERR_PW_ERROR = 1010,
    /// <summary>
    /// 该手机号绑定了其他账号
    /// </summary>
    ERR_PHONE_BIND_OTHER_ACCOUNT = 1011,
    /// <summary>
    /// 已经绑定过
    /// </summary>
    ERR_PHONE_ALREADY_BINDED = 1012
}