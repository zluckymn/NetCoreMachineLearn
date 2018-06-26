using System.Collections.Generic;
namespace System
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 根据ISO8601标准 判断当前日期是一年中的第几周 -1表示明年第一周 -50+表示去年最后一周
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int GetISOWeekNumOfYear(this DateTime dateTime)
        {
            var day = Convert.ToInt32(dateTime.DayOfWeek);
            var curYear = dateTime.Year;
            if (day != 4)
            {
                dateTime = dateTime.AddDays(4 - day);
            }
            var week = dateTime.DayOfYear / 7 + 1;
            return dateTime.Year == curYear ? week : -week;
        }

        /// <summary>
        /// 根据ISO8601标准 判断当前日期是一年中的第几周 -1表示明年第一周 -50+表示去年最后一周
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="firstDayOfWeek">设置一周第一天 （无参为周日）</param>
        /// <returns></returns>
        public static int GetISOWeekNumOfYear(this DateTime dateTime, DayOfWeek firstDayOfWeek)
        {
            var day = Convert.ToInt32(dateTime.DayOfWeek);
            var firDay = Convert.ToInt32(firstDayOfWeek);
            var curYear = dateTime.Year;
            if (day != 4)
            {
                dateTime = dateTime.AddDays(4 - day);
                if (firDay <= 4 && day < firDay)
                {
                    dateTime = dateTime.AddDays(-7);
                }
                else if (day >= firDay)
                {
                    dateTime = dateTime.AddDays(7);
                }
            }
            var week = dateTime.DayOfYear / 7 + 1;
            return dateTime.Year == curYear ? week : -week;
        }

        /// <summary>
        /// 根据ISO8601标准获取当前周所在年和周数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static KeyValuePair<int,int> GetISOYearWeekNumDicOfYear(this DateTime dateTime)
        {
            var day = Convert.ToInt32(dateTime.DayOfWeek);
            if (day != 4)
            {
                dateTime = dateTime.AddDays(4 - day);
            }
            var week = dateTime.DayOfYear / 7 + 1;
            return new KeyValuePair<int, int>(dateTime.Year, week);
        }

        /// <summary>
        /// 根据ISO8601标准获取当前周所在年和周数
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="firstDayOfWeek"></param>
        /// <returns></returns>
        public static KeyValuePair<int, int> GetISOYearWeekNumDicOfYear(this DateTime dateTime, DayOfWeek firstDayOfWeek)
        {
            var day = Convert.ToInt32(dateTime.DayOfWeek);
            var firDay = Convert.ToInt32(firstDayOfWeek);
            if (day != 4)
            {
                dateTime = dateTime.AddDays(4 - day);
                if (firDay <= 4 && day < firDay)
                {
                    dateTime = dateTime.AddDays(-7);
                }
                else if (day >= firDay)
                {
                    dateTime = dateTime.AddDays(7);
                }
            }
            var week = dateTime.DayOfYear / 7 + 1;

            return new KeyValuePair<int, int>(dateTime.Year, week);
        }

        /// <summary>
        /// 根据ISO8601标准获取当前天所在周
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="firstDayOfWeek"></param>
        /// <returns></returns>
        public static DateTime GetWeekEndDate(this DateTime dateTime)
        {
            DateTime endDateTime=dateTime ; 
            var curDayOfWeek = dateTime.DayOfWeek;
           
            while ((int)endDateTime.DayOfWeek < (int)DayOfWeek.Saturday&&endDateTime.Month==dateTime.Month)
            {
              endDateTime=endDateTime.AddDays(1);
            }
           
            if(endDateTime.Month!=dateTime.Month)
            {
            return endDateTime.AddDays(-1);
            }
            return endDateTime;
        }

        /// <summary>
        /// 根据ISO8601标准获取当前天该月份的周
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="firstDayOfWeek"></param>
        /// <returns></returns>
        public static List<DateTime> GetWeekBeginDayList(this DateTime dateTime)
        {
            var firDay = DateTime.Parse(string.Format("{0}-{1}-1", dateTime.Year, dateTime.Month));
            List<DateTime> result = new List<DateTime>();
            var endDay = firDay.GetWeekEndDate();
            result.Add(firDay);
            if (endDay != firDay)
            {
                while (endDay.Month == firDay.Month)
                {
                    endDay = endDay.AddDays(1);
                    if (endDay.Month == firDay.Month)
                    {
                        result.Add(endDay);
                        endDay = endDay.GetWeekEndDate();
                    }
                }
            }
           return result;
        }

    }
}
