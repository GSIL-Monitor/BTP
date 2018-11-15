
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/7/11 15:07:54
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

    public class MallApplyAgent : BaseBpAgent<IMallApply>, IMallApply
    {
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SellerInfoDTO> GetYJSellerInfoes(System.Collections.Generic.List<System.Guid> appIds)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SellerInfoDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetYJSellerInfoes(appIds);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> GetMallApplyInfoList(System.Guid appId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallApply.MallApplyDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallApplyInfoList(appId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO GetCommodityListV3(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListSearchDTO search)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ComdtyListResultCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCommodityListV3(search);

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
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallTypeDTO> GetMallTypeListByEsAppId(System.Guid esAppId)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.MallTypeDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetMallTypeListByEsAppId(esAppId);

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
