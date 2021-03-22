using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

//征战页面
public class Expedition : MonoBehaviour
{
    public Text warIntroText; //战役介绍文本obj
    public Image stageTipUi;//模式说明
    public Text stageTipForceName;
    public Button[] difficultyButtons;
    public Button yuanZhengButton;
    public WarStageBtnUi warStageBtnPrefab;
    public ScrollRect warStageScrollRect;
    private const int YuanZhengIndex = 6;//远征id


    public ForceSelectorUi warForceSelectorUi; //战役势力选择器
    private int lastAvailableStageIndex; //最远可战的战役索引
    private List<WarStageBtnUi> stages;
    private int recordedExpeditionWarId = -1;//当前选择战役的WarId
    public int RecordedExpeditionWarId => recordedExpeditionWarId;
    /// <summary>
    /// key = btn index, value = warStageId
    /// </summary>
    private Dictionary<int, GameModeTable> indexWarModeMap; //按键和难度的映射
    private GameModeTable currentMode;//当前选择的难度
    public StaminaCost SelectedWarStaminaCost => currentMode.StaminaCost;

    public void Init()
    {
        warForceSelectorUi.Init(PlayerDataForGame.WarTypes.Expedition);
        indexWarModeMap = new Dictionary<int, GameModeTable>();
        stages = new List<WarStageBtnUi>();
        InitWarModes();
    }

    private void InitWarModes()
    {
        lastAvailableStageIndex = 0;
        var warModes = DataTable.GameMode.Values.Where(m => m.WarList != null).ToList();
        //新手-困难关卡入口初始化
        for (int i = 0; i < difficultyButtons.Length; i++)
        {
            var uiBtn = difficultyButtons[i];
            uiBtn.gameObject.SetActive(i < warModes.Count);
            if (!uiBtn.gameObject.activeSelf)
            {
                continue;
            }
            var warMode = warModes[i];
            indexWarModeMap.Add(i, warMode);
            var textUi = uiBtn.GetComponentInChildren<Text>();
            textUi.text = warMode.Title;
            var isUnlock = IsWarUnlock(warMode);
            textUi.color = isUnlock ? Color.white : Color.gray;
            if (isUnlock)
            {
                lastAvailableStageIndex = i;
                var index = i;
                currentMode = warMode;
                uiBtn.onClick.AddListener(() =>
                {
                    InitWarsListInfo(index);
                    UIManager.instance.PlayOnClickMusic();
                });
            }
            else
            {
                uiBtn.onClick.AddListener(() =>
                {
                    PlayerDataForGame.instance.ShowStringTips(warMode.Intro);
                    UIManager.instance.PlayOnClickMusic();
                });
            }
        }

        InitWarsListInfo(lastAvailableStageIndex, true);

        //远征关卡
        
        var yuanZhengMode = DataTable.GameMode[YuanZhengIndex];
        indexWarModeMap.Add(YuanZhengIndex, yuanZhengMode);
        var isYuanZhengUnlock = IsWarUnlock(yuanZhengMode);
        yuanZhengButton.gameObject.SetActive(isYuanZhengUnlock);
        if (isYuanZhengUnlock)
        {
            var textUi = yuanZhengButton.GetComponentInChildren<Text>();
            textUi.text = yuanZhengMode.Title;
            textUi.color = Color.white;
            yuanZhengButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                InitWarsListInfo(YuanZhengIndex);
                UIManager.instance.PlayOnClickMusic();
            });
        }

        bool IsWarUnlock(GameModeTable warMode)
        {
            if (warMode.Unlock == default) return true;//解锁关卡为(id = 0)初始关卡
            var theWarBeforeUnlockWar = warMode.Unlock - 1;//当前需要解锁关卡的前一个关卡id
            var lastWarTotalStages = DataTable.War[theWarBeforeUnlockWar].CheckPoints;//上一个war的总关卡
            return PlayerDataForGame.instance.warsData.warUnlockSaveData
                .Single(w => w.warId == theWarBeforeUnlockWar).unLockCount >= lastWarTotalStages;//是否上一关已经完成
        }
    }

    /// <summary>
    /// 初始化征战关卡(左边)列表
    /// </summary>
    private void InitWarsListInfo(int btnIndex,bool forceRefresh = false)
    {
        //防止跳转界面时切换关卡
        if (UIManager.instance.IsJumping)
            return;

        if (!forceRefresh && currentMode == indexWarModeMap[btnIndex])
            return;
        currentMode = indexWarModeMap[btnIndex];
        //右侧难度选择按钮刷新大小
        OnSelectDifficultyUiScale(btnIndex);
        var warMode = indexWarModeMap[btnIndex];

        //删除战役列表
        foreach (var ui in stages) Destroy(ui.gameObject);
        stages.Clear();

        int startWarId,lastWarId = 0;
        if (warMode.Id == 6)
        {
            startWarId = lastWarId = DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level].YuanZhengWarTableId;
        }
        else
        {
            startWarId = warMode.WarList.Min;
            lastWarId = warMode.WarList.IncMax;
        }

        int index = startWarId;
        for (; index <= lastWarId; index++)
        {
            var warId = PlayerDataForGame.instance.warsData.warUnlockSaveData[index].warId;
            var warsCount = DataTable.War[warId].CheckPoints;
            var warStageUi = Instantiate(warStageBtnPrefab, warStageScrollRect.content);
            warStageUi.boundWarId = warId;
            stages.Add(warStageUi);
            var playerStageProgress = PlayerDataForGame.instance.warsData.warUnlockSaveData[index];
            var isFirstRewardTaken = playerStageProgress.isTakeReward;
            warStageUi.boxButton.gameObject.SetActive(!isFirstRewardTaken);
            var anim = warStageUi.boxButton.GetComponent<Animator>();
            anim.enabled = false;
            if (!isFirstRewardTaken)
            {
                var isUnlock = playerStageProgress.unLockCount >= warsCount;
                warStageUi.boxButton.interactable = isUnlock;
                if (isUnlock)
                {
                    anim.enabled = true;
                    warStageUi.boxButton.onClick.AddListener(() =>
                    {
                        UIManager.instance.GetWarFirstRewards(warId);
                        warStageUi.boxButton.onClick.RemoveAllListeners();
                        warStageUi.boxButton.gameObject.SetActive(false);
                    });
                }
                else
                {
                    warStageUi.boxButton.onClick.AddListener(() =>
                        PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(26)));
                }
            }
            //战役列拼接
            warStageUi.title.text = DataTable.War[index].Title + "\u2000\u2000\u2000\u2000"
                + Mathf.Min(PlayerDataForGame.instance.warsData.warUnlockSaveData[index].unLockCount, warsCount) + "/" +
                warsCount;
            warStageUi.button.onClick.AddListener(() =>
            {
                OnClickChangeWarsFun(warId);
                UIManager.instance.PlayOnClickMusic();
            });
            if(playerStageProgress.unLockCount < warsCount)break;
        }

        var lastStageWarId = PlayerDataForGame.instance.warsData.warUnlockSaveData[index > lastWarId ? lastWarId : index]
            .warId;
        //默认选择最后一个关卡
        OnClickChangeWarsFun(lastStageWarId);
        //如果UIManager在初始化，代表是场景转跳，不需要动画显示
        warStageScrollRect.DOVerticalNormalizedPos(0f, 0.3f);
    }

    private void OnSelectDifficultyUiScale(int index)
    {
        var scaleUp = new Vector2(1.2f, 1.2f);
        yuanZhengButton.transform.localScale = index == YuanZhengIndex ? scaleUp : Vector2.one;
        for (int i = 0; i < difficultyButtons.Length; i++)
            difficultyButtons[i].transform.localScale = index == i ? scaleUp : Vector2.one;
    }

    //选择战役的改变
    public void OnClickChangeWarsFun(int warId)
    {
        for (int i = 0; i < stages.Count; i++)
        {
            var boundWarId = stages[i].boundWarId;
            stages[i].selectedImage.enabled = warId == boundWarId;
        }
        var isLimitedForce = DataTable.War[warId].ForceLimit.Length > 0;
        stageTipUi.gameObject.SetActive(isLimitedForce);
        stageTipForceName.text = string.Empty;
        warForceSelectorUi.Init(PlayerDataForGame.WarTypes.Expedition);

        if (isLimitedForce)
        {
            var forces = DataTable.War[warId].ForceLimit.ToArray();
            foreach (var flagUi in warForceSelectorUi.Data)
            {
                var ui = flagUi.Value;
                var enable = forces.Contains(flagUi.Key);
                ui.Interaction(enable);

                if(!enable) warForceSelectorUi.BtnData[flagUi.Key].onClick.RemoveAllListeners();
            }

            stageTipForceName.text = forces.Length > 0 ? DataTable.War[warId].ForceIntro : string.Empty;
            warForceSelectorUi.OnSelected(PlayerDataForGame.WarTypes.Expedition);
        }

        PlayerDataForGame.instance.zhanYiColdNums = 10;
        PlayerDataForGame.instance.selectedWarId = warId;
        recordedExpeditionWarId = warId;
        //战役介绍
        warIntroText.DOPause();
        warIntroText.text = string.Empty;
        warIntroText.color = new Color(warIntroText.color.r, warIntroText.color.g, warIntroText.color.b, 0);
        warIntroText.DOFade(1, 3f);
        warIntroText.DOText(("\u2000\u2000\u2000\u2000" + DataTable.War[warId].Intro), 3f).SetEase(Ease.Linear)
            .SetAutoKill(false);
    }

    //private class WarMode
    //{
    //    public int id;
    //    public string title;
    //    public int[] warList;
    //    public int unlockWarId;
    //    public int[] tili;
    //    public string intro;

    //    public WarMode(IReadOnlyList<string> map)
    //    {
    //        id = int.Parse(map[0]);
    //        title = map[1];
    //        warList = string.IsNullOrWhiteSpace(map[2]) ? new int[0] : map[2].TableStringToInts().ToArray();
    //        unlockWarId = int.Parse(map[3]);
    //        tili = map[4].TableStringToInts().ToArray();
    //        intro = map[5];
    //    }
    //}

}
