
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/6/14 20:08:57
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class JdEclpOrderFacade : BaseFacade<IJdEclpOrder>
    {

        /// <summary>
        /// 创建京东订单
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="eclpOrderNo">京东订单编号,京东接口失败补录数据用</param>
        public void CreateOrder(System.Guid orderId, string eclpOrderNo)
        {
            base.Do();
            this.Command.CreateOrder(orderId, eclpOrderNo);
        }
        /// <summary>
        /// 发送支付信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public void SendPayInfoToHaiXin(System.Guid orderId)
        {
            base.Do();
            this.Command.SendPayInfoToHaiXin(orderId);
        }
        /// <summary>
        /// 发送售中整单退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendRefundInfoToHaiXin(System.Guid orderId)
        {
            base.Do();
            this.Command.SendRefundInfoToHaiXin(orderId);
        }
        /// <summary>
        /// 发送售中单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public void SendSingleRefundInfoToHaiXin(System.Guid orderId, System.Guid orderItemId)
        {
            base.Do();
            this.Command.SendSingleRefundInfoToHaiXin(orderId, orderItemId);
        }
        /// <summary>
        /// 发送售后整单退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        public void SendASRefundInfoToHaiXin(System.Guid orderId)
        {
            base.Do();
            this.Command.SendASRefundInfoToHaiXin(orderId);
        }
        /// <summary>
        /// 发送售后单品退款信息到海信
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public void SendASSingleRefundInfoToHaiXin(System.Guid orderId, System.Guid orderItemId)
        {
            base.Do();
            this.Command.SendASSingleRefundInfoToHaiXin(orderId, orderItemId);
        }
        /// <summary>
        /// 获取京东订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.JDEclpOrderDTO GetOrderInfo(System.Guid orderId)
        {
            base.Do();
            return this.Command.GetOrderInfo(orderId);
        }
        /// <summary>
        /// 创建进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <param name="servicesNo">京东服务单编号,京东接口失败补录数据用</param>
        public void CreateJDEclpRefundAfterSales(System.Guid orderId, System.Guid orderItemId, string servicesNo)
        {
            base.Do();
            this.Command.CreateJDEclpRefundAfterSales(orderId, orderItemId, servicesNo);
        }
        /// <summary>
        /// 是否进销存京东订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public bool ISEclpOrder(System.Guid orderId)
        {
            base.Do();
            return this.Command.ISEclpOrder(orderId);
        }
        /// <summary>
        /// 获取进销存京东订单售后信息
        /// </summary>
        /// <param name="orderId">金和订单id</param>
        /// <param name="orderItemId">金和订单项id</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesDTO GetJdEclpOrderRefundAfterSale(System.Guid orderId, System.Guid orderItemId)
        {
            base.Do();
            return this.Command.GetJdEclpOrderRefundAfterSale(orderId, orderItemId);
        }
        /// <summary>
        /// 进销存-京东商品库存同步日志
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDStockJournalDTO>> GetJDStockJourneyList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            base.Do();
            return this.Command.GetJDStockJourneyList(searcharg);
        }
        /// <summary>
        /// 进销存-京东订单日志
        /// </summary>
        /// <param name="searcharg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp.JDEclpJourneyExtendDTO>> GetJDEclpOrderJournalList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            base.Do();
            return this.Command.GetJDEclpOrderJournalList(searcharg);
        }
        /// <summary>
        /// 进销存-京东服务单日志
        /// </summary>
        /// <param name="searcharg"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesJournalDTO>> GetJDEclpOrderRefundAfterSalesJournalList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            base.Do();
            return this.Command.GetJDEclpOrderRefundAfterSalesJournalList(searcharg);
        }
        /// <summary>
        /// 进销存-获取订单物流单号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetExpOrderNo(System.Guid orderId)
        {
            base.Do();
            return this.Command.GetExpOrderNo(orderId);
        }

        /// <summary>
        /// 重新发送支付信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public void RetranPayInfoToHaiXin(DateTime startTime, DateTime endTime)
        {
            base.Do();
            this.Command.RetranPayInfoToHaiXin(startTime,endTime);
        }
        /// <summary>
        /// 重新发送售中整单退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void RetranRefundInfoToHaiXin(DateTime startTime, DateTime endTime)
        {
            base.Do();
            this.Command.RetranRefundInfoToHaiXin(startTime, endTime);
        }
        /// <summary>
        /// 重新发送售后单品退款信息到海信
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public void RetranASSingleRefundInfoToHaiXin(DateTime startTime, DateTime endTime)
        {
            base.Do();
            this.Command.RetranASSingleRefundInfoToHaiXin(startTime, endTime);
        }
        
            
    }
}
