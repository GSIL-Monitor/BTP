
/***************
功能描述: BTPBP
作    者: 
创建时间: 2014/4/18 11:26:16
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 分享相关接口
    /// </summary>
    public partial class ShareQueryBP : BaseBP, IShareQuery
    {
        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityDTO GetCommodityExt(System.Guid commodityId)
        {
            //var test = from c in Commodity.ObjectSet()
            //                where c.Id == commodityId
            //                select c;

            var commodity = Commodity.ObjectSet().Where(c => c.Id == commodityId).FirstOrDefault();
            if (commodity != null)
            {
                return commodity.ToEntityData();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取订单中的商品列表
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public OrderForShareDTO GetOrderCommoditysExt(System.Guid orderId)
        {
            OrderForShareDTO orderForShareDTO = new OrderForShareDTO();

            //查询订单信息
            var order = CommodityOrder.ObjectSet().Where(o => o.Id == orderId).FirstOrDefault();
            if (order == null)
            {
                return null;
            }

            Guid userId = order.UserId;
            Guid appId = order.AppId;
            orderForShareDTO.AppId = appId;

            //查询购买者信息
            CommodityUser userInfo = CommodityUser.ObjectSet().Where(u => u.Id == userId && u.AppId == appId).FirstOrDefault();
            if (userInfo != null)
            {
                orderForShareDTO.UserName = userInfo.Name;
                orderForShareDTO.UserPhoto = userInfo.HeadPic;
            }

            //查询订单中商品数量
            var count = OrderItem.ObjectSet().Where(o => o.CommodityOrderId == orderId).Sum(o => o.Number);
            orderForShareDTO.Count = count;

            //查询订单中随机的一件商品
            OrderItem orderitem = OrderItem.ObjectSet().Where(o => o.CommodityOrderId == orderId).FirstOrDefault();
            if (orderitem != null)
            {
                Commodity commodity = Commodity.ObjectSet().Where(o => o.Id == orderitem.CommodityId).FirstOrDefault();
                orderForShareDTO.RealPrice = orderitem.RealPrice;
                orderForShareDTO.Days = 0;
                // 获取距离促销还剩下多少天 
                var days = (from i in Promotion.ObjectSet()
                            join y in PromotionItems.ObjectSet()
                            on i.Id equals y.PromotionId
                            where DateTime.Now > i.StartTime && DateTime.Now < i.EndTime &&
                            y.CommodityId == orderitem.CommodityId && !i.IsDel && i.IsEnable
                            select new PromotionSDTO
                            {
                                EndTime = i.EndTime
                            }).ToList();

                if (days.Count > 0)
                {
                    TimeSpan ts = days[0].EndTime - DateTime.Now;
                    if (ts.TotalSeconds > 0)
                    {
                        if (ts.TotalSeconds % (3600 * 24) > 0)
                        {
                            orderForShareDTO.Days = ts.Days + 1;
                        }
                        else
                        {
                            orderForShareDTO.Days = ts.Days;
                        }
                    }
                }
                if (commodity != null)
                {
                    orderForShareDTO.CommodityDTO = commodity.ToEntityData();
                }
            }

            return orderForShareDTO;
        }
    }
}