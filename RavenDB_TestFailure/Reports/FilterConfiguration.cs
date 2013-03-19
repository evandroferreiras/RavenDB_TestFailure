using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReportsEverywhereClass.Database_structure;

namespace ReportsEverywhereClass.Reports
{
    public class FilterConfiguration
    {
        public enum ComparatorType
        {
            LessThan,
            GreaterThan,
            LessThanOrEqualThan,
            GreaterThanOrEqualTo,
            Equals,
            NotEqualTo,
            Between,
            NotBetween,
            Like,
            NotLike,
            In,
            NotIn,
        }

        public static String ComparatorTypeToString(ComparatorType comparatorType)
        {
            switch (comparatorType)
            {
                case ComparatorType.LessThan:
                    return "<";
                    break;
                case ComparatorType.GreaterThan:
                    return ">";
                    break;
                case ComparatorType.LessThanOrEqualThan:
                    return "<=";
                    break;
                case ComparatorType.GreaterThanOrEqualTo:
                    return ">=";
                    break;
                case ComparatorType.Equals:
                    return "=";
                    break;
                case ComparatorType.NotEqualTo:
                    return "<>";
                    break;
                case ComparatorType.Between:
                    return "BETWEEN";
                    break;
                case ComparatorType.NotBetween:
                    return "NOT BETWEEN";
                    break;
                case ComparatorType.Like:
                    return "LIKE";
                    break;
                case ComparatorType.NotLike:
                    return "NOT LIKE";
                    break;
                case ComparatorType.In:
                    return "IN";
                    break;
                case ComparatorType.NotIn:
                    return "NOT IN";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("comparatorType");
            }
        }

        public enum OperatorType
        {
            And,
            Or
        }

        public static String OperatorTypeToString(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.And:
                    return "AND";
                    break;
                case OperatorType.Or:
                    return "OR";
                    break;
                default:
                    return "";
            }
        }

        public DSColumn Column { get; set; }
        public ComparatorType Comparator { get; set; }
        public List<String> Values { get; set; }
        public OperatorType Operator { get; set; }
    }
}
