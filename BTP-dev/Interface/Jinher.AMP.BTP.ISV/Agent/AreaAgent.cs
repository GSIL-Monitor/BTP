
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2016/8/2 15:47:29
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.BP.Agent.Base;
using Jinher.AMP.BTP.Deploy.CustomDTO.ThirdECommerce;

namespace Jinher.AMP.BTP.ISV.Agent
{

    public class AreaAgent : BaseBpAgent<IArea>, IArea
    {

        /// <summary>
        /// 获取一级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetProvince()
        {
            //定义返回值
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetProvince();

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


        /// <summary>
        /// 获取二级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCity(string Code)
        {

            //定义返回值
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCity(Code);

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



        /// <summary>
        /// 获取三级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetCounty(string Code)
        {
            //定义返回值
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetCounty(Code);

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


        /// <summary>
        /// 获取四级地址
        /// </summary>
        /// <returns></returns>
        public ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> GetTown(string Code)
        {
            //定义返回值
            ThirdResponse<Jinher.AMP.BTP.Deploy.CustomDTO.JD.AreaDTO> result;
            try
            {
                //调用代理方法
                result = base.Channel.GetTown(Code);

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
