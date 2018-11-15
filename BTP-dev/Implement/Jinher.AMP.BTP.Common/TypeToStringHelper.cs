using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    public static class MallTypeHelper
    {
        public static string GetMallTypeString(int type)
        {
            if (type == 0)
            {
                return "自营他配";
            }
            else if (type == 1)
            {
                return "第三方";
            }
            else if (type == 2)
            {
                return "自营自配";
            }
            else if (type == null)
            {
                return "第三方";
            }
            else
            {
                return "未知类型";
            }
        }
    }

    public static class TypeToStringHelper
    {
        public static string MallTypeToString(int type)
        {
            if (type == 0)
            {
                return "自营他配";
            }
            else if (type == 1)
            {
                return "第三方";
            }
            else if (type == 2)
            {
                return "自营自配";
            }
            else if (type == null)
            {
                return "第三方";
            }
            else
            {
                return "未知类型";
            }
        }

        public static string CancleOrderReasonTypeToString(int? type)
        {
            switch (type)
            {
                case null:
                case 0: return "";
                case 1: return "不想买了";
                case 2: return "信息填写错误，重新拍";
                case 3: return "商家缺货";
                case 4: return "太贵了，不划算";
                case 5: return "其他原因";
                default: return "未知原因";
            }
        }
    }
}
