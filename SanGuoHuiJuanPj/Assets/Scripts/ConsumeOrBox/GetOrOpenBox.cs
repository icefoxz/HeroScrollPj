using System;
using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine;
using UnityEngine.UI;

public class GetOrOpenBox : MonoBehaviour
{
    [SerializeField]
    int openJiuTanYBNums;   //开酒坛所需元宝

    int[] boxNeedYvQueNums = new int[4];

    private void Start()
    {
        InitBoxShow();
    }

    //刷新宝箱免费开启时间的显示
    public void UpdateOpenTimeTips(string str0, int indexBox, bool isCanMianQi)
    {
        if (isCanMianQi)
        {
            if (UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(2).gameObject.activeSelf)
            {
                UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(2).gameObject.SetActive(false);
                if (indexBox == 0)
                {
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(1).gameObject.SetActive(true);
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(3).gameObject.SetActive(true);
                }
                else
                {
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (str0 != UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(2).GetComponent<Text>().text)
            {
                UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(2).GetComponent<Text>().text = str0;
            }
            if (!UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(2).gameObject.activeSelf)
            {
                UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(2).gameObject.SetActive(true);
                if (indexBox == 0)
                {
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(1).gameObject.SetActive(false);
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(3).gameObject.SetActive(false);
                }
                else
                {
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                    UIManager.instance.boxBtnObjs[indexBox].transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    //初始化宝箱的展示
    private void InitBoxShow()
    {
        isBoxOpening = false;

        boxNeedYvQueNums[0] = LoadJsonFile.GetGameValue(3);
        boxNeedYvQueNums[1] = LoadJsonFile.GetGameValue(4);
        boxNeedYvQueNums[2] = LoadJsonFile.GetGameValue(5);
        boxNeedYvQueNums[3] = 0;

        for (int i = 1; i < UIManager.instance.boxBtnObjs.Length; i++)
        {
            if (i != 3)
            {
                UIManager.instance.boxBtnObjs[i].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = "× " + boxNeedYvQueNums[i];
            }
            else
            {
                UIManager.instance.boxBtnObjs[i].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = string.Format(LoadJsonFile.GetStringText(4), PlayerDataForGame.instance.gbocData.fightBoxs.Count);
            }
        }
    }

    /// <summary>
    /// 观看视频免费开启酒坛
    ///// </summary>
    [Skip]
    public void WatchAdOpenJiuTan()
    {
        //获得酒坛消耗元宝按钮
        Button getFreeBoxBtn = UIManager.instance.adBtnForFreeBox.transform.parent.GetChild(1).GetComponent<Button>();
        UIManager.instance.adBtnForFreeBox.enabled = false;
        getFreeBoxBtn.enabled = false;
        DoNewAdController.instance.GetReWardVideo(
            //if (!AdController.instance.ShowVideo(
            delegate ()
            {
                int index = openJiuTanYBNums;
                openJiuTanYBNums = 0;
                OpenTaoYuanBox(0);
                openJiuTanYBNums = index;
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(5));
                UIManager.instance.adBtnForFreeBox.enabled = true;
                getFreeBoxBtn.enabled = true;
            },
            delegate ()
            {
                PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(6));
                UIManager.instance.adBtnForFreeBox.enabled = true;
                getFreeBoxBtn.enabled = true;
            });
    }

    /// <summary>
    /// 打开桃园宝箱
    /// </summary>
    /// <param name="boxNumber"></param>
    public void OpenTaoYuanBox(int boxNumber)
    {
        bool isZyBox = false;   //标记是否为战役宝箱
        int openBoxIndex = boxNumber;
        if (isBoxOpening)
        {
            return;
        }

        AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[13], AudioController0.instance.audioVolumes[13]);

        int cutYvQueNums = 0;
        if (boxNumber == 0)   //免费百箱
        {
            if (openJiuTanYBNums <= PlayerDataForGame.instance.pyData.YuanBao && TimeSystemControl.instance.OnClickToGetJiuTan())
            {
                cutYvQueNums = openJiuTanYBNums;
                PlayerDataForGame.instance.Redemption(PlayerDataForGame.RedeemTypes.JiuTan);
            }
            else
            {
                if (openJiuTanYBNums > PlayerDataForGame.instance.pyData.YuanBao)
                {
                    PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(0));
                }
                AudioController0.instance.PlayAudioSource(0);
                return;
            }
        }
        else
        {
            if (boxNumber == 1)
            {
                if (TimeSystemControl.instance.OnClickToGetFreeBox1())
                {
                    cutYvQueNums = 0;
                }
                else
                {
                    cutYvQueNums = boxNeedYvQueNums[boxNumber];
                }
            }
            if (boxNumber == 2)
            {
                if (TimeSystemControl.instance.OnClickToGetFreeBox2())
                {
                    cutYvQueNums = 0;
                }
                else
                {
                    cutYvQueNums = boxNeedYvQueNums[boxNumber];
                }
                UIManager.instance.ShowOrHideGuideObj(0, false);
            }
        }

        if ((boxNumber != 0) ? ConsumeManager.instance.DeductYuQue(cutYvQueNums) : ConsumeManager.instance.DeductYuanBao(cutYvQueNums))
        {
            if (boxNumber == 3)   //战役宝箱
            {
                isZyBox = true;
                if (PlayerDataForGame.instance.gbocData.fightBoxs.Count <= 0)
                {
                    PlayerDataForGame.instance.ShowStringTips(LoadJsonFile.GetStringText(7));
                    AudioController0.instance.PlayAudioSource(0);
                    return;
                }
                else
                {
                    boxNumber = PlayerDataForGame.instance.gbocData.fightBoxs[0];
                    PlayerDataForGame.instance.gbocData.fightBoxs.Remove(PlayerDataForGame.instance.gbocData.fightBoxs[0]);
                    UIManager.instance.boxBtnObjs[3].transform.GetChild(1).GetChild(0).GetComponent<Text>().text = string.Format(LoadJsonFile.GetStringText(4), PlayerDataForGame.instance.gbocData.fightBoxs.Count);
                    UIManager.instance.ShowOrHideGuideObj(1, false);
                }
            }

            int addExpNums = int.Parse(LoadJsonFile.warChestTableDatas[boxNumber][3]);
            UIManager.instance.GetPlayerExp(addExpNums);
            var addYuanBaoNums = RewardManager.instance.GetYuanBao(boxNumber);
            var addYvQueNums = RewardManager.instance.GetYvQue(boxNumber);
            ConsumeManager.instance.AddYuQue(addYvQueNums);
            ConsumeManager.instance.AddYuanBao(addYuanBaoNums);
            int cardId = 0;
            int chips = 0;

            var rewards = RewardManager.instance.GetCards(boxNumber, isZyBox);
            PlayerDataForGame.instance.isNeedSaveData = true;
            LoadSaveData.instance.SaveGameData();

            UIManager.instance.ShowRewardsThings(addYuanBaoNums, addYvQueNums, addExpNums, 0, rewards, 1.5f);
            isBoxOpening = true;
            UIManager.instance.boxBtnObjs[openBoxIndex].transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            UIManager.instance.boxBtnObjs[openBoxIndex].transform.GetChild(0).GetChild(2).gameObject.SetActive(true);

            AudioController0.instance.ChangeAudioClip(AudioController0.instance.audioClips[0], AudioController0.instance.audioVolumes[0]);
        }
        AudioController0.instance.PlayAudioSource(0);
    }

    bool isBoxOpening;  //是否有宝箱正在开启
    public void CloseBoxChange()
    {
        isBoxOpening = false;
    }

}