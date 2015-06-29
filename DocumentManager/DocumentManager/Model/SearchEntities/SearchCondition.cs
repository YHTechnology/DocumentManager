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
    public enum SearchCondition : uint
    {
        KNULL = 0,
        AND = 1,
        OR = 2,
    }

    public static class SearchConditionString
    {
        public static string GetString(SearchCondition aSearchCondition)
        {
            string lRet = "";
            switch (aSearchCondition)
            {
                case SearchCondition.KNULL:
                    lRet = "";
                    break;
                case SearchCondition.AND:
                    lRet = "与";
                    break;
                case SearchCondition.OR:
                    lRet = "或";
                    break;
            }
            return lRet;
        }
    }
}
