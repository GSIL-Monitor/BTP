using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO.YX;
using Jinher.JAP.Common.Loging;
using System;
using System.Data;
using System.Linq;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 严选job
    /// </summary>
    public static class YXJob
    {
        /// <summary>
        /// 严选物流绑单回调补偿机制
        /// </summary>
        public static void AutoDeliverOrder()
        {
            var yesterday = DateTime.Now.AddDays(-5);
            var ComOrders = (from co in CommodityOrder.ObjectSet()
                             join yo in YXOrder.ObjectSet() on co.Id equals yo.OrderId
                             where CustomConfig.YxAppIdList.Contains(co.AppId) && co.State == 1 && co.PaymentTime >= yesterday
                             select co.Id).ToList();
            LogHelper.Info("YXJob.AutoDeliverOrder 严选物流绑单回调补偿机制订单Id:" + string.Join(",", ComOrders.Select(c => c.ToString()).ToArray()));
            ComOrders.ForEach(p =>
            {
                string jsonstr = string.Empty;
                var orderOut = YXSV.GetPaidOrder(p.ToString(), ref jsonstr);
                if (orderOut == null || orderOut.orderPackages == null) return;
                var orderPackages = orderOut.orderPackages.Select(o => new JobOrderPackage
                {
                    orderId = p.ToString(),
                    packageId = long.Parse(o.packageId),
                    expressDetailInfos = o.expressDetailInfos,
                    expCreateTime = ConvertDataTimeToLong(Convert.ToDateTime(o.expCreateTime))
                }).ToList();
                orderPackages.ForEach(op =>
                {
                    var package = SerializationHelper.JsonSerialize(op);
                    YXOrderHelper.DeliverOrder(new YXSign(), package, true);
                });
            });
        }
        /// <summary>
        /// 将DateTime转换成long
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertDataTimeToLong(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
            return timeStamp;
        }
    }
}
