
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/6/14 20:08:58
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
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class JdEclpOrderBP : BaseBP, IJdEclpOrder
    {

        /// <summary>
        /// 创建京东订单
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="eclpOrderNo">京东订单编号,京东接口失败补录数据用</param>
        public void CreateOrder(System.Guid orderId, string eclpOrderNo)
        {
            base.Do(false);
            this.CreateOrderExt(orderId, eclpOrderNo);
        }
        /// <summary>
        /// 发送支付信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendPayInfoToHaiXin(System.Guid orderId)
        {
            base.Do(false);
            this.SendPayInfoToHaiXinExt(orderId);
        }
        /// <summary>
        /// 发送售中整单退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendRefundInfoToHaiXin(System.Guid orderId)
        {
            base.Do(false);
            this.SendRefundInfoToHaiXinExt(orderId);
        }
        /// <summary>
        /// 发送售中单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public void SendSingleRefundInfoToHaiXin(System.Guid orderId, System.Guid orderItemId)
        {
            base.Do(false);
            this.SendSingleRefundInfoToHaiXinExt(orderId, orderItemId);
        }
        /// <summary>
        /// 发送售后整单退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendASRefundInfoToHaiXin(System.Guid orderId)
        {
            base.Do(false);
            this.SendASRefundInfoToHaiXinExt(orderId);
        }
        /// <summary>
        /// 发送售后单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public void SendASSingleRefundInfoToHaiXin(System.Guid orderId, System.Guid orderItemId)
        {
            base.Do(false);
            this.SendASSingleRefundInfoToHaiXinExt(orderId, orderItemId);
        }
        /// <summary>
        /// 获取京东订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.JDEclpOrderDTO GetOrderInfo(System.Guid orderId)
        {
            base.Do(false);
            return this.GetOrderInfoExt(orderId);
        }
        /// <summary>
        /// 创建进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <param name="servicesNo">京东服务单编号,京东接口失败补录数据用</param>
        public void CreateJDEclpRefundAfterSales(System.Guid orderId, System.Guid orderItemId, string servicesNo)
        {
            base.Do(false);
            this.CreateJDEclpRefundAfterSalesExt(orderId, orderItemId, servicesNo);
        }
        /// <summary>
        /// 是否进销存京东订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool ISEclpOrder(System.Guid orderId)
        {
            base.Do(false);
            return this.ISEclpOrderExt(orderId);
        }
        /// <summary>
        /// 获取进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesDTO GetJdEclpOrderRefundAfterSale(System.Guid orderId, System.Guid orderItemId)
        {
            base.Do(false);
            return this.GetJdEclpOrderRefundAfterSaleExt(orderId, orderItemId);
        }
        /// <summary>
        /// 进销存-京东商品库存同步日志
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDStockJournalDTO>> GetJDStockJourneyList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            base.Do(false);
            return this.GetJDStockJourneyListExt(searcharg);
        }
        /// <summary>
        /// 进销存-京东订单日志
        /// </summary>
        /// <param name="searcharg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp.JDEclpJourneyExtendDTO>> GetJDEclpOrderJournalList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            base.Do(false);
            return this.GetJDEclpOrderJournalListExt(searcharg);
        }
        /// <summary>
        /// 进销存-京东服务单日志
        /// </summary>
        /// <param name="searcharg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesJournalDTO>> GetJDEclpOrderRefundAfterSalesJournalList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            base.Do(false);
            return this.GetJDEclpOrderRefundAfterSalesJournalListExt(searcharg);
        }
        /// <summary>
        /// 进销存-获取订单物流单号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetExpOrderNo(System.Guid orderId)
        {
            base.Do(false);
            return this.GetExpOrderNoExt(orderId);
        }
        /// <summary>
        /// 重新发送支付信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public void RetranPayInfoToHaiXin(DateTime startTime,DateTime endTime)
        {
            base.Do(false);
            this.RetranPayInfoToHaiXinExt(startTime,endTime);
        }

        /// <summary>
        /// 重新发送售中整单退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void RetranRefundInfoToHaiXin(DateTime startTime, DateTime endTime)
        {
            base.Do();
            this.RetranRefundInfoToHaiXinExt(startTime, endTime);
        }
        /// <summary>
        /// 重新发送售后单品退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void RetranASSingleRefundInfoToHaiXin(DateTime startTime, DateTime endTime)
        {
            base.Do();
            this.RetranASSingleRefundInfoToHaiXinExt(startTime, endTime);
        }
    }
}
