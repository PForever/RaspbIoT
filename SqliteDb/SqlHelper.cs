using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace SqliteDb
{
    public static class SqlHelper
    {
        public const string DeviceCode = "DEVICE_CODE";
        public const string DeviceName = "DEVICE_NAME";
        public const string MacAddress = "MAC_ADDRESS";
        public const string PropName = "PROP_NAME";
        public const string PropCode = "PROPCODE";
        public const string PropType = "TYPE";
        public const string IsSetter = "IS_SETTER";
        public const string Description = "DESCRIPTION";
        public const string TimeMark = "TIME_MARK";
        public const string PropValue = "PROP_VALUE";

        public const string MonQ = "'";
        public const string DuoQ = "\"";

        public static StringBuilder Append(this StringBuilder builder, IList<string> list, string quotes)
        {
            var preCountParams = list.Count - 1;
            builder.Append("(");
            for (var index = 0; index < preCountParams; index++)
            {
                builder.Append(quotes).Append(list[index]).Append(quotes).Append(", ");
            }
            builder.Append(quotes).Append(list.Last()).Append(quotes).Append(")");

            return builder;
        }

        public static StringBuilder SqlCondition(IList<string> columns, IList<string> values)
        {
            StringBuilder builder = new StringBuilder("\r\n WHERE (\r\n");
            var preCountParams = columns.Count - 1;
            for (var index = 0; index < preCountParams; index++)
            {
                if (String.IsNullOrEmpty(values[index])) continue;
                builder.Append(columns[index]).Append("='").Append(values[index]).Append("' AND ");
            }

            if (!String.IsNullOrEmpty(values.Last())) builder.Append(columns.Last()).Append("='").Append(values.Last()).Append("')");
            
            return builder.Length > "\r\n WHERE (\r\n".Length ? builder : builder.Clear();
        }

        public static StringBuilder ToSb(this IList<string> list, string quotes = MonQ)
        {
            return new StringBuilder(list.Count).Append(list, quotes);
        }

        public static string Concat(this IList<string> list)
        {
            return new StringBuilder().Append(list).ToString();
        }

        public static string StringBuild(params object[] parts)
        {
            StringBuilder builder = new StringBuilder(parts.Length);
            foreach (object part in parts) builder.Append(part);
            return builder.ToString();
        }

        private static readonly DateTime UnixTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static string TimeFormater(this DateTime date)
        {
            return date.ToUniversalTime().Subtract(UnixTime).TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
        }
        public static DateTime TimeFormater(this string date)
        {
            double millisec = date.Contains(',') ? double.Parse(date, CultureInfo.GetCultureInfo("Ru-ru")) : double.Parse(date, CultureInfo.InvariantCulture);
            return UnixTime.AddMilliseconds(millisec);
        }

        public static int IntFormater(this string value)
        {
            return int.Parse(value);
        }
        public static bool BoolFormater(this string value)
        {
            if (value == "0") return false;
            else if (value == "1") return true;
            else throw new FormatException($"Value {value} is mess. Expect 0 or 1");
        }

        public static T EnumFotmater<T>(this string value) where T : struct 
        {
            if(!Enum.TryParse(value, out T val)) throw new FormatException();
            return val;
        }
    }

    enum Tables
    {
        DEVICE,
        PROPERTIES
    }
}