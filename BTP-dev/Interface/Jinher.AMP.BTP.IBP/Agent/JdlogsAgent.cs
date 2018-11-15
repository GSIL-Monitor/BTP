/***************
功能描述: BTPProxy
作    者: 
创建时间: 2017/9/21 15:02:29
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
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.Agent
{
    public class JdlogsAgent : BaseBpAgent<IJdlogs>, IJdlogs
    {

        /// <summary>
        /// 查询所有的京东日志信息
        /// </summary>
        /// <param name="search">查询类</param>
        public List<Jinher.AMP.BTP.Deploy.JdlogsDTO> GetALLJdlogsList(Jinher.AMP.BTP.Deploy.CustomDTO.JdlogsDTO model)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.JdlogsDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetALLJdlogsList(model);

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
        /// 根据Id获取京东的日志内容
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.JdlogsDTO GetJdlogs(Guid Id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.JdlogsDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetJdlogs(Id);

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
        /// 保存京东日志信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveJdlogs(Jinher.AMP.BTP.Deploy.JdlogsDTO model)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveJdlogs(model);

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
        ///修改京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO UpdateJdlogs(Jinher.AMP.BTP.Deploy.JdlogsDTO model)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateJdlogs(model);

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
        /// 根据id删除京东日志信息
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ResultDTO DeleteJdlogs(Guid id)
        {
            //定义返回值
            ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteJdlogs(id);

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
