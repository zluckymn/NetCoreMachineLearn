using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Xml;
using System.Web;
using System.Collections;
using System.Globalization;
using MongoDB.Bson;
///<summary>
///系统功能重载
///</summary>
namespace System
{
    /// <summary>
    /// 字符串重载
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 功能：区分中英文截取字符串。
        /// </summary>
        /// <param name="Str">欲截取字串</param>
        /// <param name="iLength">截取字数</param>
        /// <param name="sAnd">截取后跟随字串，例：".."也可为空""</param>
        /// <returns>返回截取后的字符串</returns>
        public static string CutStr(this string source, int iLength, string sAnd)
        {
            if (string.IsNullOrEmpty(source))
            { 
                return string.Empty;
            }

            if (source.Length <= iLength)
            {
                return source;
            }
            else
            {
                return source.Substring(0, iLength) + sAnd;
            }
        }

        /// <summary>
        /// 切割字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options"></param>
        /// <param name="separators"></param>
        /// <returns></returns>
        public static string[] SplitParam(this string source, StringSplitOptions options, params string[] separators)
        {
            return source.Split(separators, options);
        }

        /// <summary>
        /// 切割字符串，默认删除空白项
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separators">分隔符</param>
        /// <returns></returns>
        public static string[] SplitParam(this string source, params string[] separators)
        {
            return source.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 分割字符串并转换成int类型序列，默认空白项
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="separator">分割符</param>
        /// <returns>返回List<int>类型</returns>
        public static List<int> SplitToIntList(this string source, params string[] separator)
        {
            return source.SplitToIntList(StringSplitOptions.RemoveEmptyEntries, separator);
        }

        /// <summary>
        /// 将字符串切割，成Int类型列表
        /// </summary>
        /// <param name="source"></param>
        /// <param name="options">切割选项</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static List<int> SplitToIntList(this string source, StringSplitOptions options, params string[] separator)
        {
            List<int> idList = new List<int>();
            if (!string.IsNullOrEmpty(source))
            {
                string[] ids = source.Split(separator, options);
                foreach (string id in ids)
                {
                    int temp = default(int);
                    if (int.TryParse(id, out temp))
                    {
                        idList.Add(temp);
                    }
                }
            }
            return idList;
        }

        /// <summary>
        /// 获得url字符串中指定值
        /// </summary>
        /// <param name="paramStr"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static string GetParamString(this string paramStr, string strName)
        {
            string ret = string.Empty;
            var dic = ParamStringSplit(paramStr);
            if (dic.ContainsKey(strName))
            {
                ret = dic[strName];
            }
            return ret;
        }

        /// <summary>
        /// 获得url字符串中指定值 整型
        /// </summary>
        /// <param name="paramStr"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        public static int GetParamInt(this string paramStr, string strName)
        {
            string str = GetParamString(paramStr, strName);
            int ret = 0;
            int.TryParse(str, out ret);
            return ret;
        }

        /// <summary>
        /// url字符串拆分
        /// </summary>
        /// <param name="paramStr"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParamStringSplit(this string paramStr)
        {
            string[] array = paramStr.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string,string> dic = new Dictionary<string,string>();
            try
            {
                if (array.Length > 0)
                {
                    foreach (var item in array)
                    {
                        string[] subStr = item.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                        if (subStr.Length > 1)
                        {
                            dic.Add(subStr[0], subStr[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
            
            }
            return dic;
        }

        public static string ParamConbine(this string paramStr,Dictionary<string, string>  parmDic)
        {
            var str = string.Empty;
            var idc = ParamStringSplit(paramStr);
            foreach (var item in idc)
            {
                string temp = string.Format("{0}={1}&", item.Key, item.Value);
                str += temp;
            }
            foreach (var item in parmDic)
            {
                string temp = string.Format("{0}={1}&", item.Key, item.Value);
                str += temp;
            }
            str = str.TrimEnd('&');
            return str;
        }

        /// <summary>
        /// 替换树节点前面的0字符（000001.000001 =》1.1）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RepaceStr(string str)
        {
            string[] arrStr = str.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sbResult = new StringBuilder();
            foreach (var strIndex in arrStr)
            {
                int index = 0;
                int.TryParse(strIndex, out index);
                sbResult.AppendFormat(".{0}", index);
            }
            if (sbResult.Length > 0)
            {
                sbResult = sbResult.Remove(0, 1);
            }
            return sbResult.ToString();
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string Reverse(this string src)
        {
            if (src != null)
            {
                return new string(src.ToCharArray().Reverse().ToArray());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 判断字符串是否纯数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDigital(this string str)
        {
            if (string.IsNullOrEmpty(str))    // 空字符串
            {
                return false;
            }
            for (var i = 0; i < str.Length; i++)
            {
                if (!Char.IsNumber(str[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 将字符串转成BsonArray
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BsonArray ToBsonArray(this string source)
        {
           
            try
            {
                List<object> objs = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<object>>(source);
                return BsonArray.Create(objs);
            }
            catch (Exception ex)
            {

            }
            return new BsonArray();
        }

        /// <summary>
        /// 将字符串转成List<BsonDocument>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<BsonDocument> ToBsonDocumentList(this string source)
        {
            
            try
            {
                List<object> objs = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<object>>(source);
                return objs.Select(a => BsonDocument.Create(a)).ToList();
            }
            catch (Exception ex)
            {

            }
            return new List<BsonDocument>();
        }

        /// <summary>  
        /// 计算文本长度，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="Text">需计算长度的字符串</param>
        /// <returns>int</returns>
        public static int TextLength(string Text)
        {

            int len = 0;

            for (int i = 0; i < Text.Length; i++)
            {

                byte[] byte_len = Encoding.Default.GetBytes(Text.Substring(i, 1));

                if (byte_len.Length > 1)

                    len += 2; //如果长度大于1，是中文，占两个字节，+2

                else

                    len += 1;  //如果长度等于1，是英文，占一个字节，+1

            }

            return len;

        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="result"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string CutSubStr(this string result, string begin, string end)
        {
            var startIndex = result.IndexOf(begin);
            var endIndex = result.IndexOf(end, startIndex);
            if (startIndex != -1 && endIndex != -1)
            {
                var value = result.Substring(startIndex + begin.Length, endIndex - startIndex - begin.Length);
                return value;
            }
            return string.Empty;
        }

        /// <summary>
        /// 字符串转为为Base64字符串
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncodeBase64(string text)
        {
            byte[] bytes = Encoding.Default.GetBytes(text);
            string base64String = Convert.ToBase64String(bytes);
            return base64String.Replace("+", " ");
        }

        /// <summary>
        /// Base64字符串解码
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        public static string DecodeBase64(string text)
        {
            text = text.Replace(" ", "+");
            byte[] outputb = Convert.FromBase64String(text);
            return Encoding.Default.GetString(outputb);
        }
    }
}
