using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public static class Http
{
    public static async Task<string> GetAsync(string url)
    {
        var error = $"{nameof(Http)} : ";
        try
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            error += $"Code[{response.StatusCode}]";
#if DEBUG
        }
        catch (Exception e)
        {
            error += e.ToString();
        }
        Debug.LogError(error);
#endif
        return HttpResponse.ERROR;
    }
    public static async Task<string> PostAsync(string url,string content)
    {
        var client = new HttpClient();
        var response = await client.PostAsync(url, new StringContent(content));
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();
#if DEBUG
        Debug.LogError($"{nameof(Http)}:Code[{response.StatusCode}]  ");
#endif
        return HttpResponse.ERROR;
    }
}