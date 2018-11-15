
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/9/9 15:11:33
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Cache;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 优惠卷信息
    /// </summary>
    public partial class OrderPayDetailSV : BaseSv, IOrderPayDetail
    {
        /// <summary>
        /// 查询OrderPayDetail信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public List<Jinher.AMP.BTP.Deploy.OrderPayDetailDTO> GetOrderPayDetailListExt(Guid objectid)
        {
            var orderpayDetail = OrderPayDetail.ObjectSet().Where(p => p.ObjectId == objectid).ToList();
            List<Jinher.AMP.BTP.Deploy.OrderPayDetailDTO> orderpayDetaillist = new List<OrderPayDetailDTO>();
            foreach (var item in orderpayDetail)
            {
                Jinher.AMP.BTP.Deploy.OrderPayDetailDTO model = new OrderPayDetailDTO();
                model.Id = item.Id;
                model.OrderId = item.OrderId;
                model.CommodityId=item.CommodityId;
                model.ObjectType=item.ObjectType;
                model.Amount=item.Amount;
                model.ObjectId = item.ObjectId;
                model.UseType = item.UseType;
                model.CouponType = item.CouponType;
                model.CommodityIds = item.CommodityIds;
                model.SubTime = item.SubTime;
                model.ModifiedOn = item.ModifiedOn;
                model.SubCode = item.SubCode;
                orderpayDetaillist.Add(model);
            }
            return orderpayDetaillist;
        }
        
    }
}