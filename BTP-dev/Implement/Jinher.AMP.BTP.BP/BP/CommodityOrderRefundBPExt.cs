
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/28 19:39:21
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
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using Jinher.AMP.BTP.TPS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.JAP.Common.Loging;
namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class CommodityOrderRefundBP : BaseBP, ICommodityOrderRefund
    {

        /// <summary>
        /// 添加回款记录
        /// </summary>
        /// <param name="model">回款实体模型</param>
        public bool InsertExt(Jinher.AMP.BTP.Deploy.CommodityOrderRefundDTO model)
        {
            CommodityOrderRefund modeladd = new CommodityOrderRefund();
            modeladd.Id = model.Id;
            modeladd.SubTime = model.SubTime;
            modeladd.SubId = model.SubId;
            modeladd.CommodityOrderId = model.CommodityOrderId;
            modeladd.OrderItemId = model.OrderItemId;
            modeladd.RefundType = model.RefundType;
            modeladd.RefundDate = model.RefundDate;
            modeladd.RefundAmount = model.RefundAmount;
            modeladd.Remark = model.Remark;
            modeladd.ModifiedBy = "";
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            modeladd.EntityState = System.Data.EntityState.Added;
            contextSession.SaveObject(modeladd);
            if (contextSession.SaveChanges()==0)
            {
                return false;
            }
           
            return true;
        }
        /// <summary>
        /// 根据CommodityOrderId 获取回款记录
        /// </summary>
        /// <param name="CommodityOrderId"></param>
        /// <returns>回款记录列表</returns>
        public List<CommodityOrderRefundDTO> GetListByCommodityOrderIdExt(Guid CommodityOrderId)
        {
            try
            {
                var List = from obj in CommodityOrderRefund.ObjectSet()
                           where obj.CommodityOrderId == CommodityOrderId
                           select new CommodityOrderRefundDTO {
                           Id=obj.Id,
                           SubId=obj.SubId,
                           RefundDate=obj.RefundDate,
                           RefundAmount=obj.RefundAmount,
                           Remark=obj.Remark,
                           RefundType=obj.RefundType
                           };
                return (List<CommodityOrderRefundDTO>)List.ToList();
            }
            catch(Exception e)
            {
                LogHelper.Error(string.Format("根据CommodityOrderId 获取回款记录。CommodityOrderId：{0}", CommodityOrderId), e);
                return null;
            }
            
        }
        /// <summary>
        /// 根据类型，时间获取回款记录
        /// </summary>
        /// <param name="RefundType">类型0电汇1支票2内部挂帐</param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public List<CommodityOrderRefundDTO> GetListByOtherExt(Jinher.AMP.BTP.Deploy.Enum.RefundTypeEnum RefundType, System.DateTime StartTime, System.DateTime EndTime)
        {
            throw new NotImplementedException();
        }
    }
}