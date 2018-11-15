
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/12/6 16:55:07
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

    public class CateringSettingAgent : BaseBpAgent<ICateringSetting>, ICateringSetting
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddCateringSetting(settingDTO);

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

        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCateringSetting(Deploy.CustomDTO.FCYSettingCDTO settingDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateCateringSetting(settingDTO);

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

        public Deploy.CustomDTO.FCYSettingCDTO GetCateringSetting(Guid storeId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.FCYSettingCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCateringSetting(storeId);

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



        public Jinher.AMP.BTP.Deploy.CustomDTO.FCYSettingCDTO GetCateringSettingByAppId(System.Guid appId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.FCYSettingCDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCateringSettingByAppId(appId);

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


        public List<Deploy.CustomDTO.FCYSettingCDTO> GetCateringSettingByStoreIds(List<Guid> storeIds)
        {
            //定义返回值
            List<Deploy.CustomDTO.FCYSettingCDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetCateringSettingByStoreIds(storeIds);

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
