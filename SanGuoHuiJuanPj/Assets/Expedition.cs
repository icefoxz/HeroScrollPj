﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Expedition : MonoBehaviour
{
    public Text warIntroText; //战役介绍文本obj
    public Button[] difficultyButtons;
    public Button yuanZhengButton;
    public WarStageBtnUi warStageBtnPrefab;
    public ScrollRect warStageScrollRect;

    public ForceSelectorUi warForceSelectorUi; //战役势力选择器
    private int lastAvailableStageIndex; //最远可战的战役索引
    private List<WarStageBtnUi> stages;
    private int recordedExpeditionWarId = -1;//当前选择战役的WarId
    /// <summary>
    /// key = btn index, value = warStageId
    /// </summary>
    private Dictionary<int, WarMode> indexWarStageMap; //按键和难度的映射
    private WarMode currentMode;//当前选择的难度
    public int[] SelectedWarStaminaCost => indexWarStageMap[recordedExpeditionWarId].tili;

    public void Init()
    {
        warForceSelectorUi.Init(PlayerDataForGame.WarTypes.Expedition);
        indexWarStageMap = new Dictionary<int, WarMode>();
        stages = new List<WarStageBtnUi>();
        InitWarDifBtnShow();
        recordedExpeditionWarId = lastAvailableStageIndex;
    }

    private void InitWarDifBtnShow()
    {
        lastAvailableStageIndex = 0;
        var warModes = DataTable.ChoseWar.Where(map => !string.IsNullOrWhiteSpace(map.Value[2]))
            .Select(map => new WarMode(map.Value)).ToList();
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
            indexWarStageMap.Add(i, warMode);
            var textUi = uiBtn.GetComponentInChildren<Text>();
            textUi.text = warMode.title;
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
                    PlayerDataForGame.instance.ShowStringTips(warMode.intro);
                    UIManager.instance.PlayOnClickMusic();
                });
            }
        }

        InitWarsListInfo(lastAvailableStageIndex, true);

        //远征关卡
        const int YuanZhengIndex = 6;
        var yuanZhengMode = new WarMode(DataTable.ChoseWar[YuanZhengIndex]);
        var isYuanZhengUnlock = IsWarUnlock(yuanZhengMode);
        yuanZhengButton.gameObject.SetActive(isYuanZhengUnlock);
        if (isYuanZhengUnlock)
        {
            var textUi = yuanZhengButton.GetComponentInChildren<Text>();
            textUi.text = yuanZhengMode.title;
            textUi.color = Color.white;
            yuanZhengButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                InitWarsListInfo(YuanZhengIndex);
                UIManager.instance.PlayOnClickMusic();
            });
        }

        bool IsWarUnlock(WarMode warMode)
        {
            if (warMode.unlockWarId == default) return true;//解锁关卡为(id = 0)初始关卡
            var theWarBeforeUnlockWar = warMode.unlockWarId - 1;//当前需要解锁关卡的前一个关卡id
            var lastWarTotalStages = int.Parse(DataTable.War[theWarBeforeUnlockWar][4]);//上一个war的总关卡
            return PlayerDataForGame.instance.warsData.warUnlockSaveData
                .Single(w => w.warId == theWarBeforeUnlockWar).unLockCount >= lastWarTotalStages;//是否上一关已经完成
        }
    }

    /// <summary>
    /// 选择难度按钮以改变战役列表
    /// </summary>
    private void InitWarsListInfo(int btnIndex,bool forceRefresh = false)
    {
        //防止跳转界面时切换关卡
        if (UIManager.instance.IsJumping)
            return;

        if (!forceRefresh && currentMode == indexWarStageMap[btnIndex])
            return;
        currentMode = indexWarStageMap[btnIndex];
        //右侧难度选择按钮刷新大小
        OnSelectDifficultyUiScale(btnIndex);
        var warStage = indexWarStageMap[btnIndex];

        //删除战役列表
        foreach (var ui in stages) Destroy(ui.gameObject);
        stages.Clear();

        int startWarId,lastWarId = 0;
        if (warStage.id == 6)
        {
            startWarId = lastWarId = int.Parse(DataTable.PlayerLevelData[PlayerDataForGame.instance.pyData.Level - 1][5]);
        }
        else
        {
            startWarId = warStage.warList[0];
            lastWarId = warStage.warList[1];
        }

        int index = startWarId;
        for (; index <= lastWarId; index++)
        {
            var warId = PlayerDataForGame.instance.warsData.warUnlockSaveData[index].warId;
            var warsCount = int.Parse(DataTable.WarData[warId][4]);
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
            warStageUi.title.text = DataTable.WarData[index][1] + "\u2000\u2000\u2000\u2000"
                + Mathf.Min(PlayerDataForGame.instance.warsData.warUnlockSaveData[index].unLockCount, warsCount) + "/" +
                warsCount;
            warStageUi.button.onClick.AddListener(() =>
            {
                OnClickChangeWarsFun(warId);
                UIManager.instance.PlayOnClickMusic();
            });
            if(playerStageProgress.unLockCount < warsCount)break;
        }

        //默认选择最后一个关卡
        OnClickChangeWarsFun(PlayerDataForGame.instance.warsData.warUnlockSaveData[index > lastWarId? lastWarId: index].warId);
        warStageScrollRect.DOVerticalNormalizedPos(0f, 0.3f);
    }

    private void OnSelectDifficultyUiScale(int index)
    {
        for (int i = 0; i < difficultyButtons.Length; i++)
            difficultyButtons[i].transform.localScale = index == i ? new Vector2(1.2f, 1.2f) : Vector2.one;
    }

    //选择战役的改变
    public void OnClickChangeWarsFun(int warsId = -1)
    {
        if (warsId == -1) warsId = recordedExpeditionWarId;
        for (int i = 0; i < stages.Count; i++)
        {
            var boundWarId = stages[i].boundWarId;
            stages[i].selectedImage.enabled = warsId == boundWarId;
        }

        PlayerDataForGame.instance.zhanYiColdNums = 10;
        PlayerDataForGame.instance.selectedWarId = warsId;
        //战役介绍
        warIntroText.DOPause();
        warIntroText.text = string.Empty;
        warIntroText.color = new Color(warIntroText.color.r, warIntroText.color.g, warIntroText.color.b, 0);
        warIntroText.DOFade(1, 3f);
        warIntroText.DOText(("\u2000\u2000\u2000\u2000" + DataTable.War[warsId][2]), 3f).SetEase(Ease.Linear)
            .SetAutoKill(false);
    }

    private class WarMode
    {
        public int id;
        public string title;
        public int[] warList;
        public int unlockWarId;
        public int[] tili;
        public string intro;

        public WarMode(IReadOnlyList<string> map)
        {
            id = int.Parse(map[0]);
            title = map[1];
            warList = string.IsNullOrWhiteSpace(map[2]) ? new int[0] : map[2].TableStringToInts().ToArray();
            unlockWarId = int.Parse(map[3]);
            tili = map[4].TableStringToInts().ToArray();
            intro = map[5];
        }
    }

}
