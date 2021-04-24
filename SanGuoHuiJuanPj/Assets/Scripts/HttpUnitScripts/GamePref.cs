using UnityEngine;

public static class GamePref
{
    public static string Username => PlayerPrefs.GetString(AccountIdString);
    public static string Password => PlayerPrefs.GetString(PasswordString);
    public static string IsFirstPlay => PlayerPrefs.GetString(IsFirstPlayString);

    private const string AccountIdString = "accountName";
    private const string PasswordString = "Password";
    public const string PhoneNumber = "Phone";
    public const string IsFirstPlayString = "IsFirstPlay";

    public static void SetUsername(string username) => PlayerPrefs.SetString(AccountIdString,username);

    public static void SetPassword(string password) => PlayerPrefs.SetString(PasswordString, password);

    public static void SetIsFirstPlay(bool firstPlay)
    {
        PlayerPrefs.SetInt(IsFirstPlayString, firstPlay ? 1 : 0);
        if (firstPlay)
        {
            PlayerPrefs.SetInt(StringForGuide.guideJinBaoXiang, 0);
            PlayerPrefs.SetInt(StringForGuide.guideZYBaoXiang, 0);
            PlayerPrefs.SetInt(StringForGuide.guideHeCheng, 0);
            PlayerPrefs.SetInt(StringForGuide.guideStartZY, 0);
            PlayerPrefs.SetInt(StringForGuide.guideStartGQ, 0);
            PlayerPrefs.SetInt(StringForGuide.guideCheckCardInfo, 0);
            PlayerPrefs.SetInt(StringForGuide.guideShengJIZG, 0);
        }
        else
        {
            PlayerDataForGame.instance.GuideObjsShowed[0] = PlayerPrefs.GetInt(StringForGuide.guideJinBaoXiang);
            PlayerDataForGame.instance.GuideObjsShowed[1] = PlayerPrefs.GetInt(StringForGuide.guideZYBaoXiang);
            PlayerDataForGame.instance.GuideObjsShowed[2] = PlayerPrefs.GetInt(StringForGuide.guideHeCheng);
            PlayerDataForGame.instance.GuideObjsShowed[3] = PlayerPrefs.GetInt(StringForGuide.guideStartZY);
            PlayerDataForGame.instance.GuideObjsShowed[4] = PlayerPrefs.GetInt(StringForGuide.guideStartGQ);
            PlayerDataForGame.instance.GuideObjsShowed[5] = PlayerPrefs.GetInt(StringForGuide.guideCheckCardInfo);
            PlayerDataForGame.instance.GuideObjsShowed[6] = PlayerPrefs.GetInt(StringForGuide.guideShengJIZG);
        }
    }
}