
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/6/1 16:16:23
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

    public class SelectCommodityAgent : BaseBpAgent<ISelectCommodity>, ISelectCommodity
    {
        public Deploy.CustomDTO.AppSetAppGridDTO GetAppList(Deploy.CustomDTO.AppSetSearch2DTO search)
        {
            //定义返回值
            Deploy.CustomDTO.AppSetAppGridDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAppList(search);

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
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            //定义返回值
            Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.SearchCommodity(search);

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
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodityII(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            //定义返回值
            Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.SearchCommodityII(search);

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
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodityI(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            //定义返回值
            Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.SearchCommodityII(search);

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
        public Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> SearchCommodity2(Deploy.CustomDTO.ComdtySearch4SelCDTO search)
        {
            //定义返回值
            Deploy.CustomDTO.ResultDTO<List<Deploy.CustomDTO.ComdtyList4SelCDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.SearchCommodity2(search);

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
