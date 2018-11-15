
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/31 16:47:26
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

    public class YJBDSFOrderInfoAgent : BaseBpAgent<IYJBDSFOrderInfo>, IYJBDSFOrderInfo
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO>> GetDSFOrderInfoByOrderNos(System.Collections.Generic.List<string> OrderNos)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.OrderInfoAndCarRebateDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDSFOrderInfoByOrderNos(OrderNos);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCarInsuranceRebate(arg);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>> GetCarRebateByRemittanceNo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCarRebateByRemittanceNo(arg);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCarInsuranceReport(arg);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCarRebateState(string OrderNO, int State)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCarRebateState(OrderNO, State);

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
