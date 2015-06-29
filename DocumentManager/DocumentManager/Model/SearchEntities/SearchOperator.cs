using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DocumentManager.Model.SearchEntities
{
    public enum SearchOperator : uint
    {
        EQUAL = 0,
        CONTAINS = 1,
        NOTCONTAINS = 2,
        NOEQUAL = 3,
        GREATER = 4,
        LESS = 5,
        GREATEREQUAL = 6,
        LESSEQUAL = 7
    }

    public static class SearchOperatorString
    {
        public static string GetString(SearchOperator aSearchOperator)
        {
            string lRet = "";
            switch (aSearchOperator)
            {
                case SearchOperator.CONTAINS:
                    lRet = "包含";
                    break;
                case SearchOperator.NOTCONTAINS:
                    lRet = "不包含";
                    break;
                case SearchOperator.EQUAL:
                    lRet = "相等";
                    break;
                case SearchOperator.NOEQUAL:
                    lRet = "不相等";
                    break;
                case SearchOperator.GREATER:
                    lRet = "大于";
                    break;
                case SearchOperator.LESS:
                    lRet = "小于";
                    break;
                case SearchOperator.GREATEREQUAL:
                    lRet = "大于等于";
                    break;
                case SearchOperator.LESSEQUAL:
                    lRet = "小于等于";
                    break;
            }
            return lRet;
        }
    }
}
