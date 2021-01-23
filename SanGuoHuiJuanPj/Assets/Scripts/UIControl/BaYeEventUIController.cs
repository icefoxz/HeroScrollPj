using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaYeEventUIController : MonoBehaviour
{
    public BaYeEventUI[] eventList;
    public void OnClickAudioPaly() 
    {
        AudioController0.instance.RandomPlayGuZhengAudio();//播放随机音效
    }

}
