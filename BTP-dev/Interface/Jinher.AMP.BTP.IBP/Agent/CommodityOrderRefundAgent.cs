
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/6/28 19:39:22
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
using Jinher.AMP.BTP.Deploy;
namespace Jinher.AMP.BTP.IBP.Agent
{

    public class CommodityOrderRefundAgent : BaseBpAgent<ICommodityOrderRefund>, ICommodityOrderRefund
    {
        public bool Insert(Jinher.AMP.BTP.Deploy.CommodityOrderRefundDTO model)
        {

            try
            {
                //调用代理方法
              return   base.Channel.Insert(model);

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
        public List<CommodityOrderRefundDTO> GetListByCommodityOrderId(Guid CommodityOrderId)
        {
            //定义返回值
            List<CommodityOrderRefundDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetListByCommodityOrderId(CommodityOrderId);

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
        public List<CommodityOrderRefundDTO> GetListByOther(Jinher.AMP.BTP.Deploy.Enum.RefundTypeEnum RefundType, System.DateTime StartTime, System.DateTime EndTime)
        {
            //定义返回值
            List<CommodityOrderRefundDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetListByOther(RefundType, StartTime, EndTime);

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
    }
}
