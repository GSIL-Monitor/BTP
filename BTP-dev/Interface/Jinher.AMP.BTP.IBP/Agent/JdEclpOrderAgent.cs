
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/6/14 20:09:00
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{

    public class JdEclpOrderAgent : BaseBpAgent<IJdEclpOrder>, IJdEclpOrder
    {
        public void CreateOrder(System.Guid orderId, string eclpOrderNo)
        {

            try
            {
                //调用代理方法
                base.Channel.CreateOrder(orderId, eclpOrderNo);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public void SendPayInfoToHaiXin(System.Guid orderId)
        {

            try
            {
                //调用代理方法
                base.Channel.SendPayInfoToHaiXin(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public void SendRefundInfoToHaiXin(System.Guid orderId)
        {

            try
            {
                //调用代理方法
                base.Channel.SendRefundInfoToHaiXin(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public void SendSingleRefundInfoToHaiXin(System.Guid orderId, System.Guid orderItemId)
        {

            try
            {
                //调用代理方法
                base.Channel.SendSingleRefundInfoToHaiXin(orderId, orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public void SendASRefundInfoToHaiXin(System.Guid orderId)
        {

            try
            {
                //调用代理方法
                base.Channel.SendASRefundInfoToHaiXin(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public void SendASSingleRefundInfoToHaiXin(System.Guid orderId, System.Guid orderItemId)
        {

            try
            {
                //调用代理方法
                base.Channel.SendASSingleRefundInfoToHaiXin(orderId, orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public Jinher.AMP.BTP.Deploy.JDEclpOrderDTO GetOrderInfo(System.Guid orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.JDEclpOrderDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOrderInfo(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public void CreateJDEclpRefundAfterSales(System.Guid orderId, System.Guid orderItemId, string servicesNo)
        {

            try
            {
                //调用代理方法
                base.Channel.CreateJDEclpRefundAfterSales(orderId, orderItemId, servicesNo);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public bool ISEclpOrder(System.Guid orderId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.ISEclpOrder(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesDTO GetJdEclpOrderRefundAfterSale(System.Guid orderId, System.Guid orderItemId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJdEclpOrderRefundAfterSale(orderId, orderItemId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDStockJournalDTO>> GetJDStockJourneyList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDStockJournalDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJDStockJourneyList(searcharg);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp.JDEclpJourneyExtendDTO>> GetJDEclpOrderJournalList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.JdEclp.JDEclpJourneyExtendDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJDEclpOrderJournalList(searcharg);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesJournalDTO>> GetJDEclpOrderRefundAfterSalesJournalList(Jinher.AMP.BTP.Deploy.CustomDTO.JourneyDTO searcharg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.JDEclpOrderRefundAfterSalesJournalDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJDEclpOrderRefundAfterSalesJournalList(searcharg);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> GetExpOrderNo(System.Guid orderId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<string> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetExpOrderNo(orderId);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }            //返回结果
            return result;
        }

        public void RetranPayInfoToHaiXin(DateTime startTime, DateTime endTime)
        {

            try
            {
                //调用代理方法
                base.Channel.RetranPayInfoToHaiXin(startTime, endTime);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public void RetranRefundInfoToHaiXin(DateTime startTime, DateTime endTime)
        {

            try
            {
                //调用代理方法
                base.Channel.RetranRefundInfoToHaiXin(startTime, endTime);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
        public void RetranASSingleRefundInfoToHaiXin(DateTime startTime, DateTime endTime)
        {

            try
            {
                //调用代理方法
                base.Channel.RetranASSingleRefundInfoToHaiXin(startTime, endTime);

            }
            catch
            {
                //抛异常
                throw;
            }
            finally
            {
                //关链接
                ChannelClose();
            }
        }
    }
}
