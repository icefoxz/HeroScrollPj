﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class EncryptDecipherTool
{
    #region DES加密解密算法

    private static readonly byte[] DESKey = new byte[] { 0x82, 0xBC, 0xA1, 0x6A, 0xF5, 0x87, 0x3B, 0xE6, 0x59, 0x6A, 0x32, 0x64, 0x7F, 0x3A, 0x2A, 0xBB, 0x2B, 0x68, 0xE2, 0x5F, 0x06, 0xFB, 0xB8, 0x2D, 0x67, 0xB3, 0x55, 0x19, 0x4E, 0xB8, 0xBF, 0xDD };
    
    /// <summary>
    /// DES加密
    /// </summary>
    /// <param name="strSource">待加密字串</param>
    /// <param name="key">32位Key值</param>
    /// <returns>加密后的字符串</returns>
    public static string DESEncrypt(string strSource)
    {
        return DESEncryptF(strSource, DESKey);
    }

    private static string DESEncryptF(string strSource, byte[] key)
    {
        SymmetricAlgorithm sa = Rijndael.Create();
        sa.Key = key;
        sa.Mode = CipherMode.ECB;
        sa.Padding = PaddingMode.Zeros;
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write);
        byte[] byt = Encoding.Unicode.GetBytes(strSource);
        cs.Write(byt, 0, byt.Length);
        cs.FlushFinalBlock();
        cs.Close();
        return Convert.ToBase64String(ms.ToArray());
    }

    /// <summary>
    /// DES解密
    /// </summary>
    /// <param name="strSource">待解密的字串</param>
    /// <param name="key">32位Key值</param>
    /// <returns>解密后的字符串</returns>
    public static string DESDecrypt(string strSource)
    {
        return DESDecryptF(strSource, DESKey);
    }

    private static string DESDecryptF(string strSource, byte[] key)
    {
        try
        {
            SymmetricAlgorithm sa = Rijndael.Create();
            sa.Key = key;
            sa.Mode = CipherMode.ECB;
            sa.Padding = PaddingMode.Zeros;
            ICryptoTransform ct = sa.CreateDecryptor();
            byte[] byt = Convert.FromBase64String(strSource);
            MemoryStream ms = new MemoryStream(byt);
            CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cs, Encoding.Unicode);
            return sr.ReadToEnd();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
    #endregion

}