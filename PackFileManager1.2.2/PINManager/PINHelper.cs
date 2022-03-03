
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace PackFileManager.PINManager
{
    public class PINHelper
    {

        #region 字符串加解密

        private static string AESKey = "[11/*YUEDin.g.;]";
        private static string DESKey = "HZCHOME";

        public static string RSADKey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAKoYLW9ev5DKBrkCLKb9pcFLsqKSfQvCHEP4zx2PZpUHDEYEwnYeQiNLfVgDFmv8P+7WUTa/zOdHyxa/A5GiulchkxsCysJKAlyOXBEbEIVqfno+AcG6h0diho5X0iC1errUd2OGINIVGhmrRH1cir51nhCsmV5jb+Nn5pZpaEttAgMBAAECgYEAoWENAoRPlmzHEhMJEGrJL+rFEk+Pym8haDAROYeLmUs1jt0HLxAoSdpekvli9ZM/iTfXl+1D2A8alXsnRK6ywVWJm6M3Q00D4IZP42rJOBa6snB59dBnHwHcArTB9JAMCGj3UOoXEiQ4Qu6duUgeT/myDYzbZ1KKKGAr0DuSZAECQQDzjcAK54lhC1C/AxtuXdb85A0q6hFoS7VIlcYgOSdVdz9+IFp6+ir05szWyptX5npRN0nFJXxNbLQ2/zePkOkBAkEAsslosyCm8IklGtu97ga2e0K7+p1NISPe6CGZ032U2MEyrpeVnRRDw/h2G+baD3Jco6EfyAOrazPTHBYzja8WbQJAUjFjPwrWdPahoGTHDB8FH8FCpFnr1/u8ySsqetNu78vXlJQMlPX6Kz38oPwtIqP4YZI8BhlZcrOdufW1ZXDtAQJBAJo4iowZ2mcaplsougkc2UQyKJziG6o+mwV6FosMhN0EqodYUsKQvHPLW3ZuCCPS7n5nela6c5+hsLVFoWrSYXECQGd1cA81KchSnqFjZN+orAqQSw6cWNpy3a5JPpeC5nuKeklNntNuV0hBebWTNMqDP1wYiHIS3PLB46DplS33mvc=";


        public static string AesEncrypt(string toEncrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }


        public static string AesDecrypt(string toDecrypt, string key)
        {
            byte[] keyArray = UTF8Encoding.UTF8.GetBytes(key);
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        /// <summary> 
        /// AES加密 
        /// </summary>
        public static string AESEncrypt(string value, string _aeskey = null)
        {
            if (string.IsNullOrEmpty(_aeskey))
            {
                _aeskey = AESKey;
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary> 
        /// AES解密 
        /// </summary>
        public static string AESDecrypt(string value, string _aeskey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_aeskey))
                {
                    _aeskey = AESKey;
                }
                byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
                byte[] toEncryptArray = Convert.FromBase64String(value);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary> 
        /// DES加密 
        /// </summary>
        public static string DESEncrypt(string value, string _deskey = null)
        {
            if (string.IsNullOrEmpty(_deskey))
            {
                _deskey = DESKey;
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(_deskey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

            DESCryptoServiceProvider rDel = new DESCryptoServiceProvider();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary> 
        /// DES解密 
        /// </summary>
        public static string DESDecrypt(string value, string _deskey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_deskey))
                {
                    _deskey = DESKey;
                }
                byte[] keyArray = Encoding.UTF8.GetBytes(_deskey);
                byte[] toEncryptArray = Convert.FromBase64String(value);

                DESCryptoServiceProvider rDel = new DESCryptoServiceProvider();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }




        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>

        public static string Encrypt(string Text, string sKey = "yueding")
        {
            if (Text == null) return "ERROR";
            DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(Text);
            descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.FlushFinalBlock();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in memoryStream.ToArray())
            {
                stringBuilder.AppendFormat("{0:X2}", b);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Decrypt(string Text, string sKey = "yueding")
        {
            if (Text == null) return "ERROR";
            try
            {
                DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
                int num = Text.Length / 2;
                byte[] array = new byte[num];
                for (int i = 0; i < num; i++)
                {
                    int num2 = Convert.ToInt32(Text.Substring(i * 2, 2), 16);
                    array[i] = (byte)num2;
                }
                descryptoServiceProvider.Key = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                descryptoServiceProvider.IV = Encoding.ASCII.GetBytes(FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(), CryptoStreamMode.Write);
                cryptoStream.Write(array, 0, array.Length);
                cryptoStream.FlushFinalBlock();
                return Encoding.Default.GetString(memoryStream.ToArray());
            }
            catch { return "ERROR"; }
        }



        public static string MD5(string value)
        {
            byte[] result = Encoding.UTF8.GetBytes(value);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "");
        }

        public static string HMACMD5(string value, string hmacKey)
        {
            HMACSHA1 hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(hmacKey));
            byte[] result = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] output = hmacsha1.ComputeHash(result);


            return BitConverter.ToString(output).Replace("-", "");
        }

        /// <summary>
        /// base64编码
        /// </summary>
        /// <returns></returns>
        public static string Base64Encode(string value)
        {
            string result = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            return result;
        }
        /// <summary>
        /// base64解码
        /// </summary>
        /// <returns></returns>
        public static string Base64Decode(string value)
        {
            string result = "";
            try
            {
                result = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }
            catch
            {
                result = "非法";
            }

            return result;
        }
        #endregion
    }

    public static class Base64Helper
    {
        /// <summary>
        /// URL的操作类
        /// </summary>
        static System.Text.Encoding encoding = System.Text.Encoding.UTF8;


        //public static string Base64DesEncrypt(string strPath)
        //{
        //    Encoding encode = Encoding.ASCII;
        //    byte[] bytedata = encode.GetBytes(strPath);
        //    string returnData = Convert.ToBase64String(bytedata, 0, bytedata.Length);
        //    return returnData;
        //}
        #region URL的64位编码

        public static string Base64Encrypt(string sourthUrl)
        {
            string eurl = HttpUtility.UrlEncode(sourthUrl);
            eurl = Convert.ToBase64String(encoding.GetBytes(eurl));
            return eurl;
        }

        #endregion

        #region URL的64位解码

        public static string Base64Decrypt(string eStr)
        {
            if (!IsBase64(eStr))
            {
                return eStr;
            }
            byte[] buffer = Convert.FromBase64String(eStr);
            string sourthUrl = encoding.GetString(buffer);
            sourthUrl = HttpUtility.UrlDecode(sourthUrl);
            return sourthUrl;
        }

        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(string eStr)
        {
            if ((eStr.Length % 4) != 0)
            {
                return false;
            }
            if (!Regex.IsMatch(eStr, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// 添加URL参数
        /// </summary>
        public static string AddParam(string url, string paramName, string value)
        {
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.Query))
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "?" + paramName + "=" + eval);
            }
            else
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "&" + paramName + "=" + eval);
            }
        }

        /// <summary>
        /// 更新URL参数
        /// </summary>
        public static string UpdateParam(string url, string paramName, string value)
        {
            string keyWord = paramName + "=";
            int index = url.IndexOf(keyWord) + keyWord.Length;
            int index1 = url.IndexOf("&", index);
            if (index1 == -1)
            {
                url = url.Remove(index, url.Length - index);
                url = string.Concat(url, value);
                return url;
            }
            url = url.Remove(index, index1 - index);
            url = url.Insert(index, value);
            return url;
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            nvc = new NameValueCollection();
            baseUrl = "";
            if (url == "")
                return;
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);
            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);
            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }


    }

}
