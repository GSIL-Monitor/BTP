
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/29 10:59:59
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class YJBDSFOrderInfoAgent : BaseBpAgent<IYJBDSFOrderInfo>, IYJBDSFOrderInfo
    {
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO> GetDSFOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInfoSearchDTO arg)
        {
            //定义返回值
            Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBDSFOrderInfoDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetDSFOrderInfo(arg);

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
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTODSFOrderInfo(Jinher.AMP.BTP.Deploy.CustomDTO.YJBDSFOrderInformationDTO model)
        {
            //定义返回值
            Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.InsertTODSFOrderInfo(model);

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
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO> GetCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg)
        {
            //定义返回值
            Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO> result;

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
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceRebate(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDTO model)
        {
            //定义返回值
            Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.InsertTOCarInsuranceRebate(model);

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
        public Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBCarInsuranceReportDTO> GetCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg)
        {
            //定义返回值
            Jinher.AMP.YJB.Deploy.CustomDTO.ListResultDTO<Jinher.AMP.BTP.Deploy.YJBCarInsuranceReportDTO> result;

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
        public Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO InsertTOCarInsuranceReport(Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO model)
        {
            //定义返回值
            Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.InsertTOCarInsuranceReport(model);

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
