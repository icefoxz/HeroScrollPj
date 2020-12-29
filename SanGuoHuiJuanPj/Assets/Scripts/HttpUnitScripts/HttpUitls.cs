using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using UnityEngine;

namespace wode.HTTP
{
    class HttpUitls
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string Get(string Url)
        {
            string retString = string.Empty;
            try
            {
                //System.GC.Collect();
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Proxy = null;
                request.KeepAlive = false;
                request.Method = "GET";
                //request.ContentType = "application/json; charset=UTF-8";
                request.ContentType = "application/x-www-form-urlencoded";//窗体数据被编码为名称/值对形式
                request.AutomaticDecompression = DecompressionMethods.GZip;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                myResponseStream.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                retString = HttpResponse.ERROR;
            }
            return retString;
        }

        /// <summary>
        /// Post请求可用-键值对格式
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string PostForValue(string Url, Dictionary<string, object> dic)
        {
            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                //request.Referer = Referer;
                request.ContentType = "application/x-www-form-urlencoded";  //窗体数据被编码为名称/值对形式

                //添加Post参数
                StringBuilder builder = new StringBuilder();
                int i = 0;
                foreach (var item in dic)
                {
                    if (i > 0)
                    {
                        builder.Append("&");
                    }
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
                request.ContentLength = bytes.Length;
                Stream myResponseStream = request.GetRequestStream();
                myResponseStream.Write(bytes, 0, bytes.Length);
                myResponseStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                retString = HttpResponse.ERROR;
            }
            return retString;
        }

        /// <summary>
        /// Post请求可用-json格式
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Data"></param>
        /// <param name="Referer"></param>
        /// <returns></returns>
        public static string PostForJson(string Url, string Data, string Referer)
        {
            string retString = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                //request.Referer = Referer;
                //request.Proxy = new WebProxy("192.168.1.12",80);
                byte[] bytes = Encoding.UTF8.GetBytes(Data);
                request.ContentType = "application/json;charset=UTF-8";  //窗体数据被编码为名称/值对形式
                                                                          //request.ContentType = "application/json";
                request.ContentLength = bytes.Length;
                Stream myResponseStream = request.GetRequestStream();
                myResponseStream.Write(bytes, 0, bytes.Length);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                StreamReader myStreamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                retString = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                myResponseStream.Close();

                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                retString = HttpResponse.ERROR;
            }
            return retString;
        }
    }
}