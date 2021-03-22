﻿using System.Threading;
using Beebyte.Obfuscator;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class JinNangUI: MonoBehaviour
{
    public Button continueBtn;
    public Button doubleAdBtn;
    public ScrollRect rewardsView;
    public JinNangRewardUI yuanBao;
    public JinNangRewardUI yvQue;
    public JinNangRewardUI exp;
    public JinNangRewardUI stamina;
    public Image window;
    public Text continueText;
    public Text jinNangContent;
    public Text characterName;

    public void OnReward(string content, Color contentColor, string character, int staminaValue, int yuanBaoValue)
    {
        AudioController0.instance.ChangeAudioClip(11);
        AudioController0.instance.PlayAudioSource(0);

        continueText.gameObject.SetActive(false);
        rewardsView.gameObject.SetActive(false);
        doubleAdBtn.gameObject.SetActive(false);
        jinNangContent.gameObject.SetActive(true);
        window.gameObject.SetActive(true);
        window.color = Opacity(window.color, 0);
        characterName.color = Opacity(characterName.color, 0);
        jinNangContent.text = content;
        jinNangContent.color = Opacity(contentColor, 0);
        characterName.text = character;
        window.DOFade(1, 0.5f).OnComplete(() =>
        {
            characterName.DOFade(1, 1.5f);
            jinNangContent.DOFade(1, 1.5f).OnComplete(() =>
            {
                continueText.gameObject.SetActive(true);
                continueBtn.onClick.AddListener(()=>OnContinueClick(yuanBaoValue,staminaValue));
            });
        });
        gameObject.SetActive(true);
    }

    [Skip] private void OnContinueClick(int yuanBaoValue, int staminaValue)
    {
        UIManager.instance.PlayOnClickMusic();
        characterName.DOFade(0, 1f);
        jinNangContent.DOFade(0, 1f).OnComplete(() =>
        {
            //展示奖励内容
            DisplayReward(yuanBaoValue, staminaValue);
            doubleAdBtn.gameObject.SetActive(true);
            continueBtn.onClick.RemoveAllListeners();
            doubleAdBtn.onClick.RemoveAllListeners();
            //点击背景领取并退出锦囊
            continueBtn.onClick.AddListener(() =>
            {
                UIManager.instance.PlayOnClickMusic();
                if (yuanBaoValue > 0)
                    ConsumeManager.instance.AddYuanBao(yuanBaoValue);
                if (staminaValue > 0)
                    TimeSystemControl.instance.AddTiLiNums(staminaValue);
                PlayerDataForGame.instance.Redemption(PlayerDataForGame.RedeemTypes.JinNang);
                PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(43));
                continueBtn.onClick.RemoveAllListeners();
                gameObject.SetActive(false);
            });
            //点击广告双倍奖励
            doubleAdBtn.onClick.AddListener(() =>
            {
                UIManager.instance.PlayOnClickMusic();
                //背景按钮无效
                continueBtn.enabled = false;
                doubleAdBtn.enabled = false;
                AdAgent.instance.BusyRetry(() =>
                {
                    PlayerDataForGame.instance.ShowStringTips("翻倍成功！");
                    //奖励翻倍
                    yuanBaoValue *= 2;
                    staminaValue *= 2;
                    DisplayReward(yuanBaoValue, staminaValue);
                    continueBtn.enabled = true;
                    doubleAdBtn.gameObject.SetActive(false);
                    doubleAdBtn.enabled = true;
                    doubleAdBtn.onClick.RemoveAllListeners();
                }, () =>
                {
                    PlayerDataForGame.instance.ShowStringTips(DataTable.GetStringText(6));
                    continueBtn.enabled = true;
                    doubleAdBtn.enabled = true;
                });
            });
        });
    }


    private void DisplayReward(int yuanBaoAmt, int staminaAmt)
    {
        rewardsView.gameObject.SetActive(true);
        yuanBao.gameObject.SetActive(yuanBaoAmt > 0);
        yuanBao.amount.text = "×" + yuanBaoAmt;
        stamina.gameObject.SetActive(staminaAmt > 0);
        stamina.amount.text = "×" + staminaAmt;
    }

    private Color Opacity(Color baseColor,float opacity) => new Color(baseColor.r, baseColor.g, baseColor.b, opacity);
}