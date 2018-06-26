using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;


///<summary>
///MongoDB基础及其重载
///</summary>
namespace MZ.Extension
{
    /// <summary>
    /// BsonDoc方法重载
    /// </summary>
    public static class BsonDocumentExtension
    {
       

        #region 获取List<BsonDocument>  2015-10-16
       

        


         /// <summary>
        /// 获取List<BsonDocument>
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<BsonDocument> GetBsonDocumentList(this BsonDocument bsonDoc, string name)
        {
            try
            {
                if (bsonDoc.ContainsColumn(name))
                {
                    return MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<BsonDocument>>(
                        bsonDoc.String(name));
                }
                else
                {
                    return new List<BsonDocument>();
                }
            }
            catch (Exception ex)
            {
                return new List<BsonDocument>();
            }
        }
       
        #endregion


        #region 通用获取
        /// <summary>
        /// 获取文本值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Text(this BsonDocument bsonDoc, string name)
        {
            return String(bsonDoc, name);
        }
        
        /// <summary>
        ///  获取文本值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string Text(this BsonDocument bsonDoc, string name, string defaultValue)
        {
            return String(bsonDoc, name, defaultValue);
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string String(this BsonDocument bsonDoc, string name)
        {
            return String(bsonDoc, name, "");
        }

        /// <summary>
        /// 向对象新增key，如果key已经存在，则修改原先key对应的值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="key">key值</param>
        /// <param name="val">要新增的对象</param>
        public static void TryAdd(this BsonDocument bsonDoc, string key, string val)
        {
            if (bsonDoc.Contains(key))
            {
                bsonDoc[key] = val;
            }
            else
            {
                bsonDoc.Add(key, val);
            }
        }

        /// <summary>
        ///  获取字符串值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string String(this BsonDocument bsonDoc, string name, string defaultValue)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                return bsonDoc.GetValue(name).ToString();
            }

            return defaultValue;
        }

        /// <summary>
        /// 获取整形值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int Int(this BsonDocument bsonDoc, string name)
        {
            return Int(bsonDoc, name, 0);
        }

        /// <summary>
        /// 获取整形值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int Int(this BsonDocument bsonDoc, string name, int defaultValue)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                int temp = new int();

                if (int.TryParse(bsonDoc.GetValue(name).ToString(), out temp))
                {
                    return bsonDoc.GetValue(name).ToInt32();
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 货币的格式展示
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Money(this BsonDocument bsonDoc, string name)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                decimal value;
                string str = bsonDoc.GetValue(name).ToString();
                if (!string.IsNullOrEmpty(str) && decimal.TryParse(str, out value))
                {
                    return value.ToMoney();
                }
            }
            return string.Empty;
            
        }

        /// <summary>
        /// 货币的格式展示
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Money_Z(this BsonDocument bsonDoc, string name)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                decimal value;
                string str = bsonDoc.GetValue(name).ToString();
                if (!string.IsNullOrEmpty(str) && decimal.TryParse(str, out value))
                {
                    return value.ToMoney_ZEX();
                }
            }
            return "0";

        }
        /// <summary>
        /// 货币的格式展示
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string Money_ZOT(this BsonDocument bsonDoc, string name)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                decimal value;
                string str = bsonDoc.GetValue(name).ToString();
                if (str == "0" || str == "0.00") 
                {
                    return "0";
                }
                else  if (!string.IsNullOrEmpty(str) && decimal.TryParse(str, out value))
                {
                    return value.ToMoney();
                }
            }
            return "";
        }




        /// <summary>
        /// 获取日期值,无值则返回DateTime.MinValue
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime Date(this BsonDocument bsonDoc, string name)
        {
            return Date(bsonDoc, name, default(DateTime));
        }



        /// <summary>
        /// 获取日期值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime Date(this BsonDocument bsonDoc, string name, DateTime defaultValue)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                if (bsonDoc[name].IsDateTime == false)
                {
                    DateTime temp;
                    if (DateTime.TryParse(bsonDoc.GetValue(name).ToString(), out temp))
                    {
                        return temp;
                    }
                    else
                    {
                        return defaultValue;
                    }
                }
                else {
                    return bsonDoc[name].AsDateTime;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static double Double(this BsonDocument bsonDoc, string name)
        {
            return Double(bsonDoc, name, 0);
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double Double(this BsonDocument bsonDoc, string name, double defaultValue)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                double temp = new double();

                if (double.TryParse(bsonDoc.GetValue(name).ToString(), out temp))
                {
                    return temp;
                }
                else if (double.TryParse(Regex.Replace(bsonDoc.GetValue(name).ToString(), @"[,，]", ""),out temp))
                {
                    return temp;
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal Decimal(this BsonDocument bsonDoc, string name)
        {
            return Decimal(bsonDoc, name, 0);
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static decimal RoundDecimal(this BsonDocument bsonDoc, string name)
        {
            return decimal.Round(Decimal(bsonDoc, name, 0), 2);
        }

        /// <summary>
        /// 获取浮点值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal Decimal(this BsonDocument bsonDoc, string name, decimal defaultValue)
        {
            if (bsonDoc != null && bsonDoc.Contains(name))
            {
                decimal temp = new decimal();

                if (decimal.TryParse(bsonDoc.GetValue(name).ToString(), out temp))
                {
                    return temp;
                }
                else if (decimal.TryParse(Regex.Replace(bsonDoc.GetValue(name).ToString(), @"[,，]", ""), out temp))
                {
                    return temp;
                }
            }

            return defaultValue;
        }


        /// <summary>
        /// 获取当前记录的是否包含列
        /// </summary>
        /// <returns></returns>
        public static bool ContainsColumn(this BsonDocument bsonDoc, string name)
        {
            return bsonDoc.Elements.Where(c => c.Name.Trim()==name.Trim()).Count()>0;
        }

        /// <summary>
        /// 获取当前记录的是否包含列
        /// </summary>
        /// <returns></returns>
        public static int  ColumnCount(this BsonDocument bsonDoc)
        {
            return bsonDoc.Elements.Count();
        }


        #endregion

        #region 快捷获取
        /// <summary>
        /// 获取当前记录的时间
        /// </summary>
        /// <returns></returns>
        public static string CreateDate(this BsonDocument bsonDoc)
        {
            return bsonDoc.Date("createDate").ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取当前记录的创建人
        /// </summary>
        /// <returns></returns>
        public static string UpdateDate(this BsonDocument bsonDoc)
        {
            return bsonDoc.Date("updateDate").ToString("yyyy-MM-dd");
        }

        

        /// <summary>
        /// 获取日期值,无值则返回DateTime.MinValue
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name">字段名</param>
        /// <param name="formate">例如"yyyy-MM-dd"</param>
        /// <returns></returns>
        public static string DateFormat(this BsonDocument bsonDoc, string name, string formate)
        {
            return bsonDoc.Date(name) != DateTime.MinValue ? bsonDoc.Date(name).ToString(formate) : "";
        }

        /// <summary>
        /// 获取日期值,无值则返回DateTime.MinValue
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ShortDate(this BsonDocument bsonDoc, string name)
        {
            var formate = "yyyy-MM-dd";
            return bsonDoc.DateFormat(name, formate);
        }

        

        /// <summary>
        /// 获取主键字段名
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static string  TableName(this BsonDocument bsonDoc)
        {
            var tbName = string.Empty;
            if (bsonDoc.Contains("underTable"))
            {
                tbName = bsonDoc.GetValue("underTable").ToString();      //当前记录所属表
            }

            return tbName;
        }

         
        #endregion

        #region 扩展方法
        /// <summary>
        /// 判断一个BsonDocument是否为空或为NULL
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this BsonDocument bsonDoc)
        {
            if (bsonDoc == null) return true;

            if (bsonDoc.Elements.Count() == 0) return true;

            return false;
        }

        /// <summary>
        /// 通用设置接口
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="fieldName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void SetValue(BsonDocument bsonDoc, string fieldName, string obj)
        {
            if (bsonDoc.Contains(fieldName))
            {
                bsonDoc[fieldName] = obj;
            }
            else
            {
                bsonDoc.Add(fieldName,obj);
            }
        }
       
        /// <summary>
        /// 获取文本值
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BsonArray Array(this BsonDocument bsonDoc, string name)
        {
            BsonArray dataArray = new BsonArray();
            if (bsonDoc.ContainsColumn(name) && !string.IsNullOrEmpty(bsonDoc.Text(name)))
            {
                dataArray = bsonDoc[name] as BsonArray;
            }
            return dataArray;
        }
        /// <summary>
        /// 将数组对象转化为bsonArray
        /// </summary>
        /// <param name="bsonDoc"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static BsonDocument InitBsonFromString(BsonDocument bsonDoc, string jsonStr)
        {
            bsonDoc = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(jsonStr);
            return bsonDoc;
        }
        #endregion

    }
}
