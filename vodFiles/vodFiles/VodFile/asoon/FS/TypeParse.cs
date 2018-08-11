namespace VodFile
{
    using System;

    public class TypeParse
    {
        public static T ParseValue<T>(object expression)
        {
            object now = null;
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.DBNull:
                case TypeCode.String:
                    now = string.Empty;
                    break;

                case TypeCode.Boolean:
                    now = false;
                    break;

                case TypeCode.Char:
                    now = '\0';
                    break;

                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    if ((expression != null) && !(expression.ToString() == string.Empty))
                    {
                        now = -1;
                        break;
                    }
                    now = 0;
                    break;

                case TypeCode.Single:
                    now = 0f;
                    break;

                case TypeCode.Double:
                    now = 0.0;
                    break;

                case TypeCode.Decimal:
                    now = 0M;
                    break;

                case TypeCode.DateTime:
                    now = DateTime.Now;
                    break;
            }
            return ParseValue<T>(expression, (T) now);
        }

        public static T ParseValue<T>(object expression, T defaultValue)
        {
            if ((expression == null) || (expression == DBNull.Value))
            {
                expression = defaultValue;
            }
            else
            {
                if (expression.ToString().Length == 0)
                {
                    return defaultValue;
                }
                try
                {
                    expression = Convert.ChangeType(expression, typeof(T));
                }
                catch
                {
                    expression = defaultValue;
                }
            }
            return (T) expression;
        }

        public static bool ToBool<T>(T expression)
        {
            if (typeof(T) != typeof(string))
            {
                return ParseValue<bool>(expression);
            }
            return (expression.ToString() == "1");
        }

        public static DateTime ToDateTime<T>(T expression)
        {
            if (typeof(T) == typeof(DateTime))
            {
                return Convert.ToDateTime(expression);
            }
            return ParseValue<DateTime>(expression, DateTime.Now);
        }

        public static string ToDateTime<T>(T expression, string format)
        {
            return ParseValue<DateTime>(expression, DateTime.Now).ToString(format);
        }

        public static Guid ToGuid<T>(T expression)
        {
            return ParseValue<Guid>(expression, Guid.Empty);
        }

        public static int ToInt<T>(T expression)
        {
            if (typeof(T) == typeof(int))
            {
                return Convert.ToInt32(expression);
            }
            return ParseValue<int>(expression);
        }

        public static long ToLong<T>(T expression)
        {
            if (typeof(T) == typeof(long))
            {
                return Convert.ToInt64(expression);
            }
            return (long) ParseValue<int>(expression);
        }

        public static string ToString<T>(T expression)
        {
            if (typeof(T) == typeof(string))
            {
                return expression.ToString();
            }
            return ParseValue<string>(expression, string.Empty);
        }

        public static string ToString<T>(T expression, string defaultValue)
        {
            return ParseValue<string>(expression, defaultValue);
        }
    }
}

