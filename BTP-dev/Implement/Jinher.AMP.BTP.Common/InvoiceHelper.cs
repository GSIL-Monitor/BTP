using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.BE;

namespace Jinher.AMP.BTP.Common
{
    /// <summary>
    /// 中国石化电子发票统一判断方法
    /// </summary>
    public class InvoiceHelper
    {
        /// <summary>
        /// 易捷北京自营商品申请开电子发票
        /// </summary>
        /// <returns></returns>
        public static bool IsZshInvoice(CommodityOrder commodityOrder)
        {
            bool isZshInvoice = false;
            Guid esAppId = new Guid(CustomConfig.InvoiceAppId);
            //易捷北京的自营或者门店自营
            MallApply mallApply = MallApply.ObjectSet().FirstOrDefault(t => t.EsAppId == esAppId && t.AppId == commodityOrder.AppId && (t.Type == 0 || t.Type == 2));
            if (mallApply != null)
            {
                var invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == commodityOrder.Id);
                if (invoice != null && invoice.Category == 2)
                {
                    isZshInvoice = true;
                }
            }
            return isZshInvoice;
        }
    }
}
