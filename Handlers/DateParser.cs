using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitContentCreater.Handlers
{
    internal class DateParser
    {
        public static string GetNormalizedDate(string date, Action<string>? callback)
        {
            try
            {
                date = date.Replace("Date:", "").Trim();

                var dateElements = date.Split(" ").ToArray();

                if (dateElements.Length == 2)
                {
                    return date;
                }
                else
                {
                    return $"{_GetYearMonthDay(dateElements[1], dateElements[2], dateElements[4])} {dateElements[3]}";
                }
            }
            catch
            {
                callback?.Invoke($"Ошибка определения даты: {date}");

                return "<Ошибка определения даты>";
            }

        }

        private static string _GetYearMonthDay(string month, string day, string year)
        {
            int _year = int.Parse(year);
            int _month = 0;
            switch (month)
            {
                case "Jan":
                    _month = 1;
                    break;
                case "Feb":
                    _month = 2;
                    break;
                case "Mar":
                    _month = 3;
                    break;
                case "Apr":
                    _month = 4;
                    break;
                case "May":
                    _month = 5;
                    break;
                case "Jun":
                    _month = 6;
                    break;
                case "Jul":
                    _month = 7;
                    break;
                case "Aug":
                    _month = 8;
                    break;
                case "Sep":
                    _month = 9;
                    break;
                case "Oct":
                    _month = 10;
                    break;
                case "Nov":
                    _month = 11;
                    break;
                case "Dec":
                    _month = 12;
                    break;
            }
            int _day = int.Parse(day);

            return (new DateTime(_year, _month, _day)).ToShortDateString();
        }
    }
}
