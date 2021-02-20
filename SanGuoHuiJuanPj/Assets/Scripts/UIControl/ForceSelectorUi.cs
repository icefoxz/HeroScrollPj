using System;
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
    /// <summary>
    /// Key = forceId, value = ForceFlagUI
    /// </summary>
    public IReadOnlyDictionary<int, ForceFlagUI> Data => data;
    public virtual void Init(PlayerDataForGame.WarTypes warType)
    {
        var totalUnlock = int.Parse(LoadJsonFile.playerLevelTableDatas[PlayerDataForGame.instance.pyData.Level - 1][6]);
        var totalDisplayForce = mode == Modes.仅显示可用
            ? totalUnlock + 1
            : LoadJsonFile.shiLiTableDatas.Count;

        for (int i = 0; i < totalDisplayForce; i++)
        {
            int forceIndex = i;
            Button btn;
            ForceFlagUI forceFlag;
            if (!data.ContainsKey(forceIndex))
            {
                btn = Instantiate(buttonPrefab, content.transform);//复制按钮
                btn.transform.localScale = Vector3.one;
                btn.gameObject.name = forceIndex.ToString();
                forceFlag = btn.GetComponentInChildren<ForceFlagUI>(true);
                forceFlag.Set((ForceFlags) forceIndex);
                data.Add(forceIndex, forceFlag);
            }
            else
            {
                forceFlag = data[forceIndex];
                btn = forceFlag.GetComponentInParent<Button>();
            }

            switch (mode)
            {
                case Modes.全显值暗:
                {
                    var isEnable=i <= totalUnlock;
                    btn.interactable = isEnable;
                    forceFlag.Interaction(isEnable,LoadJsonFile.shiLiTableDatas[forceIndex][4] + "级");
                    if (btn.enabled) btn.onClick.AddListener(() => OnSelected(warType, forceIndex));
                    break;
                }
                case Modes.仅显示可用:
                {
                    btn.onClick.AddListener(() => OnSelected(warType, forceIndex));
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

    protected virtual void OnSelected(PlayerDataForGame.WarTypes warType,int forceId)
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
