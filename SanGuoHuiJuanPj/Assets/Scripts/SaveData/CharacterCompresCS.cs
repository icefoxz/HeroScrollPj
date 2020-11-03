using UnityEngine;
using UnityEditor;
using System.Text;
using System;
using System.IO;
using System.IO.Compression;

public class CharacterCompresCS
{
    /// <summary>
    /// 压缩字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string CompressString(string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length == 0)
        {
            return "";
        }
        else
        {
            byte[] rawData = Encoding.UTF8.GetBytes(str.ToString());
            byte[] zippedData = Compress(rawData);
            return (string)(Convert.ToBase64String(zippedData));
        }

        //var compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
        //var compressAfterByte = Compress(compressBeforeByte);
        //string compressString = Convert.ToBase64String(compressAfterByte);
        //return compressString;
    }

    /// <summary>
    /// 解压缩字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string DecompressString(string str)
    {
        //DataSet ds = new DataSet();
        string CC = GZipDecompressString(str);
        //System.IO.StringReader Sr = new System.IO.StringReader(CC);
        //ds.ReadXml(Sr);
        return CC;

        //var compressBeforeByte = Convert.FromBase64String(str);
        //var compressAfterByte = Decompress(compressBeforeByte);
        //string compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
        //return compressString;
    }

    /// <summary>
    /// 将传入的二进制字符串资料以GZip算法解压缩
    /// </summary>
    /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
    /// <returns>原始未压缩字符串</returns>
    public static string GZipDecompressString(string zippedString)
    {
        if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
        {
            return "";
        }
        else
        {
            byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
            return Encoding.UTF8.GetString(Decompress(zippedData));
        }
    }

    /// <summary>
    /// Compress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Compress(byte[] data)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(data, 0, data.Length);
            compressedzipStream.Close();
            return ms.ToArray();

            //var ms = new MemoryStream();
            //var zip = new GZipStream(ms, CompressionMode.Compress, true);
            //zip.Write(data, 0, data.Length);
            //zip.Close();
            //var buffer = new byte[ms.Length];
            //ms.Position = 0;
            //ms.Read(buffer, 0, buffer.Length);
            //ms.Close();
            //return buffer;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    /// <summary>
    /// Decompress
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static byte[] Decompress(byte[] data)
    {
        try
        {

            MemoryStream ms = new MemoryStream(data);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();

            //var ms = new MemoryStream(data);
            //var zip = new GZipStream(ms, CompressionMode.Decompress, true);
            //var msreader = new MemoryStream();
            //var buffer = new byte[0x1000];
            //while (true)
            //{
            //    var reader = zip.Read(buffer, 0, buffer.Length);
            //    if (reader <= 0)
            //    {
            //        break;
            //    }
            //    msreader.Write(buffer, 0, reader);
            //}
            //zip.Close();
            //ms.Close();
            //msreader.Position = 0;
            //buffer = msreader.ToArray();
            //msreader.Close();
            //return buffer;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}