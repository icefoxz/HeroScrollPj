using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//todo 服务器信息必须存在unity的player.prefs ，不让任何人获取
public class Server
{
    public const string PhoneBindingApi = "";// => Get();
    public const string RegAccountApi = "";
    public const string InstanceIdApi = "";
    public const string GetSave3Api = "";
    public const string GetSave2Api = "";
    public const string GetSave1Api = "";
    public const string GetSave0Api = "";
    public const string UserLoginApi = "";

    private static string Get(string key)
    {
        var value = PlayerPrefs.GetString(key);
        if (string.IsNullOrWhiteSpace(value))
            throw new NotImplementedException($"PlayerPrefs获取不到信息: key={key}");
        return value;
    }

}