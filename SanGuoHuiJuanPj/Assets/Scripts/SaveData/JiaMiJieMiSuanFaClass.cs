using System;
//using System.Data;
//using System.Configuration;
//using System.Web;
//using System.Web.Security;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using System.Web.UI.HtmlControls;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace sql_function
{

    /// <summary>
    /// 所有加密方法
    /// 定义字符串加密接口 [MD5加密,SHA1加密，字符串加密]    前台只需定这样定义：
    /// sql_function.InterFaceStringEncryptionDecryption QueryStringEncodeCodeAndDncodeCode =new sql_function.StringEncryptionDecryption();
    /// 说明：其中sql_function代表命名空间名称 InterFaceStringEncryptionDecryption代表接口名称   StringEncryptionDecryption代表类名
    /// </summary>
    public interface InterFaceStringEncryptionDecryption
    {

        #region (1) QueryString加密与解密 开始

        /// <summary>
        /// QueryString加密
        /// </summary>
        /// <param name="StringSQL"></param>
        /// <returns></returns>

        string QueryStringEncodeCode(string StringSQL);

        /// <summary>
        /// QueryString解密
        /// </summary>
        /// <param name="StringSQL"></param>
        /// <returns></returns>
        string QueryStringDncodeCode(string StringSQL);

        #endregion


        #region (2) Rijndael算法

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        string RijndaelEncrypt(string Source);

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        string RijndaelDecrypt(string Source);

        #endregion (2) Rijndael算法


        #region ( 3 ) Base64与UTF8混用

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="bb"></param>
        /// <returns></returns>
        string BUEncrypt(string bb);

        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        string BUDecrypt(string aa);

        #endregion

        #region ( 4 )固定密钥算法

        /// <summary>
        /// 字符串加密
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        string SKeyEncrypt(string strText);


        /// <summary>
        /// 字符串解密
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        string SKeyDecrypt(string strText);

        #endregion


        #region ( 5 )DES算法

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        string DESEncrypt(string strSource);

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        string DESDecrypt(string strSource);


        #endregion


        #region ( 6 )   加密密码MD5T和SHA1

        /// <summary>
        /// 加密密码MD5T和SHA1
        /// </summary>
        /// <param name="strSource">字符串</param>
        /// <param name="strFlag">加密类别</param>
        /// <param name="substringlen">加密长度</param>
        /// <returns></returns>
        string encrypting(string strSource, int strFlag, int substringlen);

        #endregion
    }

    /// <summary>
    /// 定义接口类 加密的类的方法
    /// </summary>
    public class StringEncryptionDecryption : InterFaceStringEncryptionDecryption
    {

        #region (1) QueryString加密与解密 开始

        /// <summary>
        /// QueryString加密
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string QueryStringEncodeCode(string code)
        {
            string result = "";
            if (code == null || code == "")
            {
                result = "";
            }
            else
            {
                result = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes("" + code + "")).Replace("+", "%2B");

            }
            return result;
        }

        /// <summary>
        /// QueryString解密
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string QueryStringDncodeCode(string code)
        {
            string result = "";

            if (code == null || code == "")
            {
                result = "";
            }
            else
            {
                try
                {
                    result = System.Text.Encoding.Default.GetString(Convert.FromBase64String(code.Replace("%2B", "+")));
                }
                catch (FormatException ex)///抛出异常 [错误信息“Base-64字符数组的无效长度”]
                {
                    result = "0";
                }
            }
            return result;
        }


        #endregion (1) QueryString加密与解密 结束


        #region ( 2 ) Rijndael算法
        private static SymmetricAlgorithm mobjCryptoService = new RijndaelManaged();

        private static string Key = "Guz(%&hj7x89H$yuBI0456FtmaT5&fvHUFCy76*h%(HilJ$lhj!y6&(*jkP87jH7";

        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private byte[] GetLegalKey()
        {
            string sTemp = Key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private byte[] GetLegalIV()
        {
            string sTemp = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%$hjk";
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }
        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="Source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        public string RijndaelEncrypt(string Source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(Source);
            MemoryStream ms = new MemoryStream();
            mobjCryptoService.Key = GetLegalKey();
            mobjCryptoService.IV = GetLegalIV();
            ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
            CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);
            cs.Write(bytIn, 0, bytIn.Length);
            cs.FlushFinalBlock();
            ms.Close();
            byte[] bytOut = ms.ToArray();
            return Convert.ToBase64String(bytOut);
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="Source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        public string RijndaelDecrypt(string Source)
        {
            try
            {
                byte[] bytIn = Convert.FromBase64String(Source);
                MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length);
                mobjCryptoService.Key = GetLegalKey();
                mobjCryptoService.IV = GetLegalIV();
                ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion


        #region ( 3 )Base64与UTF8混用

        //字符串加密
        public string BUEncrypt(string bb)
        {
            byte[] by = new byte[bb.Length];
            by = System.Text.Encoding.UTF8.GetBytes(bb);

            string r = Convert.ToBase64String(by);
            return r;
        }

        //字符串解密
        public string BUDecrypt(string aa)
        {
            try
            {
                byte[] by = Convert.FromBase64String(aa);

                string r = Encoding.UTF8.GetString(by);

                return r;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion


        #region ( 4 )固定密钥算法

        public static Byte[] Iv64 = { 11, 22, 33, 44, 55, 66, 77, 88 };

        public static Byte[] byKey64 = { 10, 20, 30, 40, 50, 60, 70, 80 };
        //字符串加密
        public string SKeyEncrypt(string strText)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                Byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey64, Iv64), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //字符串解密
        public string SKeyDecrypt(string strText)
        {
            Byte[] inputByteArray = new byte[strText.Length];
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey64, Iv64), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion


        #region ( 5 )DES算法

        public static byte[] DESKey = new byte[] { 0x82, 0xBC, 0xA1, 0x6A, 0xF5, 0x87, 0x3B, 0xE6, 0x59, 0x6A, 0x32, 0x64, 0x7F, 0x3A, 0x2A, 0xBB, 0x2B, 0x68, 0xE2, 0x5F, 0x06, 0xFB, 0xB8, 0x2D, 0x67, 0xB3, 0x55, 0x19, 0x4E, 0xB8, 0xBF, 0xDD };
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="strSource">待加密字串</param>
        /// <param name="key">32位Key值</param>
        /// <returns>加密后的字符串</returns>
        public string DESEncrypt(string strSource)
        {
            return DESEncryptF(strSource, DESKey);
        }

        private string DESEncryptF(string strSource, byte[] key)
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
        public string DESDecrypt(string strSource)
        {
            return DESDecryptF(strSource, DESKey);
        }

        private string DESDecryptF(string strSource, byte[] key)
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


        #region ( 6 )   加密密码MD5T和SHA1

        /// <summary>
        /// 加密密码MD5T和SHA1
        /// </summary>
        /// <param name="strSource">字符串</param>
        /// <param name="strFlag">加密类别</param>
        /// <param name="substringlen">加密长度</param>
        /// <returns></returns>
        /// 
        public string encrypting(string strSource, int strFlag, int substringlen)
        {
            string ss = "";
            //if (strFlag == 1)///MD5加密
            //{
            //    if (substringlen == 16)//16位MD5加密（取32位加密的9~25字符）
            //    {
            //        ss = FormsAuthentication.HashPasswordForStoringInConfigFile(strSource, "MD5").ToLower().Substring(8, 16);
            //    }
            //    else if (substringlen == 32)//32位加密
            //    {
            //        ss = FormsAuthentication.HashPasswordForStoringInConfigFile(strSource, "MD5").ToLower();
            //    }
            //}
            //else if (strFlag == 2)///SHA1加密
            //{
            //    if (substringlen == 16)//SHA1 16位加密
            //    {
            //        ss = FormsAuthentication.HashPasswordForStoringInConfigFile(strSource, "SHA1").ToLower().Substring(8, 16);
            //    }
            //    else if (substringlen == 32)//SHA1 40位加密
            //    {
            //        ss = FormsAuthentication.HashPasswordForStoringInConfigFile(strSource, "SHA1").ToLower();
            //    }
            //}
            //else
            //{
            //    ss = "";
            //}
            return ss;
        }

        #endregion
    }
}