using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;



namespace System
{
   /// <summary>
   /// 数据类型重载
   /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 金额的格式展示,去除小数 2014.9.21修改只有三盛需要，因为三盛突然改成需要取小数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToMoney_ZEX(this decimal val)
        {
           return val.ToString("#,##");
          
        }

        /// <summary>
        /// 金额的格式展示,去除小数 2014.9.21修改只有三盛需要，因为三盛突然改成需要取小数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToMoney_Z(this decimal val)
        {
          
             return val.ToString("#,##0.00");
           
        }
       
        /// <summary>
        /// 金额的格式展示，保留两位小数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToMoney(this decimal val)
        {
            return val.ToString("#,##0.00");
        }

        /// <summary>
        /// 金额的格式展示，保留一位小数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToMoney_O(this decimal val)
        {
            return val.ToString("#,##0.0");
        }

        /// <summary>
        /// 金额的格式展示，保留两位小数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToMoney_T(this decimal val)
        {
            return val.ToString("#,##0.00");
        }

        public static string ToRate(this decimal val)
        {
            return val.ToString("#0.##%");
        }

        /// <summary>
        /// 金额的格式展示，保留一位小数
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToMoney_ZOT(this decimal val)
        {
            if (val == 0)
            {
                return "0";
            }
            else
            {
                return val.ToString("#,##0.00");
            }
        }


        /// <summary>
        /// 换行
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToBr(this string val)
        {
            return val.Replace("\r\n", "<br />");
        }
    }
}
