﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ForceSelectorUi : MonoBehaviour
{
    [Serializable]
    public enum Modes
    {
        仅显示可用,
        全显值暗
    }
    public Button buttonPrefab;
    public HorizontalLayoutGroup content;
    public Modes mode;
    [Header("-1为没音效")]
    public int onSelectedAudioId = -1;

    private Dictionary<int, ForceFlagUI> data = new Dictionary<int, ForceFlagUI>();
    private Dictionary<int, Button> btnData = new Dictionary<int, Button>();
    /// <summary>
    /// Key = forceId, value = ForceFlagUI
    /// </summary>
    public IReadOnlyDictionary<int, ForceFlagUI> Data => data;
    public IReadOnlyDictionary<int, Button> BtnData => btnData;
    public virtual void Init(PlayerDataForGame.WarTypes warType)
    {
        var totalUnlock = DataTable.PlayerLevelConfig[PlayerDataForGame.instance.pyData.Level].UnlockForces;
        var totalDisplayForce = mode == Modes.仅显示可用
            ? totalUnlock + 1
            : DataTable.Force.Count;

        for (int i = 0; i < totalDisplayForce; i++)
        {
            int forceId = i;
            Button btn;
            ForceFlagUI forceFlag;
            if (!data.ContainsKey(forceId))
            {
                btn = Instantiate(buttonPrefab, content.transform);//复制按钮
                btn.transform.localScale = Vector3.one;
                btn.gameObject.name = forceId.ToString();
                forceFlag = btn.GetComponentInChildren<ForceFlagUI>(true);
                forceFlag.Set((ForceFlags) forceId);
                data.Add(forceId, forceFlag);
                btnData.Add(forceId, btn);
            }
            else
            {
                forceFlag = data[forceId];
                btn = btnData[forceId];
            }
            ButtonUiReset(btn);
            switch (mode)
            {
                case Modes.全显值暗:
                {
                    var isEnable=i <= totalUnlock;
                    btn.interactable = isEnable;
                    forceFlag.Interaction(isEnable,DataTable.Force[forceId].UnlockLevel + "级");
                    if (btn.enabled) btn.onClick.AddListener(() => OnSelected(warType, forceId));
                    break;
                }
                case Modes.仅显示可用:
                {
                    btn.onClick.AddListener(() => OnSelected(warType, forceId));
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
            btn.gameObject.SetActive(true);
        }
        buttonPrefab.gameObject.SetActive(false);
        OnSelected(warType, PlayerDataForGame.instance.WarForceMap[warType]);
    }

    private void ButtonUiReset(Button btn)
    {
        btn.onClick.RemoveAllListeners();
        btn.interactable = true;
        btn.enabled = true;
        btn.gameObject.SetActive(false);
    }

    public virtual void OnSelected(PlayerDataForGame.WarTypes warType,int forceId = -1)
    {
        if (onSelectedAudioId > -1)
        {
            AudioController0.instance.ForcePlayAudio(onSelectedAudioId);
        }
        foreach (var obj in data)
        {
            var ui = obj.Value;
            var force = obj.Key;
            ui.Select(force == forceId);
        }
        PlayerDataForGame.instance.WarForceMap[warType] = forceId;
    }

}
