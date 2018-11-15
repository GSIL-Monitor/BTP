using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 订单帮助类
    /// </summary>
    public class OrderHelper
    {
        /// <summary>
        /// 获取订单状态的文本表示。
        /// </summary>
        /// <param name="state">订单状态</param>
        /// <returns></returns>
        public static string GetOrderStateText(int state)
        {
            string text = "";
            switch (state)
            {
                case 0:
                    text = "未支付";
                    break;
                case 1:
                    text = "未发货";
                    break;
                case 2:
                    text = "已发货";
                    break;
                case 3:
                    text = "确认收货";
                    break;
                case 4:
                    text = "商家取消订单";
                    break;
                case 5:
                    text = "客户取消订单";
                    break;
                case 6:
                    text = "超时交易关闭";
                    break;
                case 7:
                    text = "已退款";
                    break;
                case 8:
                    text = "待发货退款中";
                    break;
                case 9:
                    text = "已发货退款中";
                    break;
                case 10:
                    text = "已发货退款中商家同意退款，商家未收到货";
                    break;
                case 11:
                    text = "付款中";
                    break;
                case 12:
                    text = "金和处理退款中";
                    break;
                case 13:
                    text = "出库中";
                    break;
                case 14:
                    text = "出库中退款中";
                    break;
            }
            return text;
        }
    }
}
