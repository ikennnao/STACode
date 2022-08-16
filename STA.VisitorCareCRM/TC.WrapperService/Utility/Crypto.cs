using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TC.WrapperService.Utility
{
    public class Crypto
    {
        public static string DESEncrypt(string strEncryptText)
        {
            string strEncryptedTxt = string.Empty;
            string strDESSaltKey = ConfigurationManager.ConnectionStrings["DESSaltKey"] != null ? ConfigurationManager.ConnectionStrings["DESSaltKey"].ConnectionString : "#$%dso$4";

            if (!string.IsNullOrWhiteSpace(strEncryptText) && !string.IsNullOrWhiteSpace(strDESSaltKey))
            {
                byte[] rgbIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                var rgbKey = Encoding.UTF8.GetBytes(strDESSaltKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                byte[] inputArray = Encoding.UTF8.GetBytes(strEncryptText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputArray, 0, inputArray.Length);
                cs.FlushFinalBlock();
                strEncryptedTxt = Convert.ToBase64String(ms.ToArray());
            }

            return strEncryptedTxt;
        }

        public static string DESDecrypt(string strDecryptText)
        {
            string strDecryptedTxt = string.Empty;
            string strDESSaltKey = ConfigurationManager.ConnectionStrings["DESSaltKey"] != null ? ConfigurationManager.ConnectionStrings["DESSaltKey"].ConnectionString : "#$%dso$4";

            if (!string.IsNullOrWhiteSpace(strDecryptText) && !string.IsNullOrWhiteSpace(strDESSaltKey))
            {
                byte[] rgbIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                var rgbKey = Encoding.UTF8.GetBytes(strDESSaltKey);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                byte[] inputByteArray = Convert.FromBase64String(strDecryptText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                strDecryptedTxt = encoding.GetString(ms.ToArray());
            }

            return strDecryptedTxt;
        }

        public static string TripleDESEncrypt(string strEncryptText)
        {
            string strEncryptedTxt = string.Empty;
            string strTripleDESSaltKey = ConfigurationManager.ConnectionStrings["TripleDESSaltKey"] != null ? ConfigurationManager.ConnectionStrings["TripleDESSaltKey"].ConnectionString : "sta!2021@vc#crm$v9.0%sau";

            if (!string.IsNullOrWhiteSpace(strEncryptText) && !string.IsNullOrWhiteSpace(strTripleDESSaltKey))
            {
                byte[] rgbIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                var rgbKey = Encoding.UTF8.GetBytes(strTripleDESSaltKey);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                byte[] inputArray = Encoding.UTF8.GetBytes(strEncryptText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputArray, 0, inputArray.Length);
                cs.FlushFinalBlock();
                strEncryptedTxt = Convert.ToBase64String(ms.ToArray());
            }

            return strEncryptedTxt;
        }

        public static string TripleDESDecrypt(string strDecryptText)
        {
            string strDecryptedTxt = string.Empty;
            string strTripleDESSaltKey = ConfigurationManager.ConnectionStrings["TripleDESSaltKey"] != null ? ConfigurationManager.ConnectionStrings["TripleDESSaltKey"].ConnectionString : "sta!2021@vc#crm$v9.0%sau";

            if (!string.IsNullOrWhiteSpace(strDecryptText) && !string.IsNullOrWhiteSpace(strTripleDESSaltKey))
            {
                byte[] rgbIv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                var rgbKey = Encoding.UTF8.GetBytes(strTripleDESSaltKey);
                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                byte[] inputByteArray = Convert.FromBase64String(strDecryptText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                strDecryptedTxt = encoding.GetString(ms.ToArray());
            }

            return strDecryptedTxt;
        }
    }
}