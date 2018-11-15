
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2014/4/16 17:22:19
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.JAP.BF.BE.Deploy.Base;
using Jinher.JAP.BF.IService.Interface;
using System.ServiceModel;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Agent.Base;

namespace Jinher.AMP.BTP.IBP.Agent
{
    public class SelfTakeStationAgent : BaseBpAgent<ISelfTakeStation>, ISelfTakeStation
    {
        /// <summary>
        /// 添加自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SaveSelfTakeStation(selfTakeStationDTO);

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
        /// 修改自提点
        /// </summary>
        /// <param name="selfTakeStationDTO">自提点实体</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationAndManagerDTO selfTakeStationDTO)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.UpdateSelfTakeStation(selfTakeStationDTO);

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
        /// 删除多个自提点
        /// </summary>
        /// <param name="ids">自提点ID集合</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStations(System.Collections.Generic.List<System.Guid> ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteSelfTakeStations(ids);

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
        /// 查询自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult> GetAllSelfTakeStation(Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationSearchSDTO selfTakeStationSearch, out int rowCount)
        {
            //定义返回值
            System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllSelfTakeStation(selfTakeStationSearch, out rowCount);

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
        /// 获取自提点信息
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult GetSelfTakeStationById(System.Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.SelfTakeStationResult result;

            try
            {
                //调用代理方法
                result = base.Channel.GetSelfTakeStationById(id);

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
        /// 查询负责人是否已存在
        /// </summary>
        /// <param name="userId">负责人IU平台用户ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CheckSelfTakeStationManagerByUserId(System.Guid userId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.CheckSelfTakeStationManagerByUserId(userId);

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
        /// 删除负责人信息
        /// </summary>
        /// <param name="ids">id列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteSelfTakeStationManagerById(System.Collections.Generic.List<System.Guid> ids)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteSelfTakeStationManagerById(ids);

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
        /// 获取所有自提点
        /// </summary>
        /// <param name="AppId">卖家id</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public List<AppSelfTakeStationResultDTO> GetAllAppSelfTakeStation(Guid AppId, int pageSize, int pageIndex, out int rowCount)
        {
            //定义返回值
            List<AppSelfTakeStationResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllAppSelfTakeStation(AppId, pageSize, pageIndex, out rowCount);

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
        /// 删除自提点
        /// </summary>
        /// <param name="id">自提点ID</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteAppSelfTakeStation(Guid id)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.DeleteAppSelfTakeStation(id);

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
        /// 根据条件查询所有自提点
        /// </summary>
        /// <param name="AppId">卖家ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="rowCount"></param>
        /// <param name="Name"></param>
        /// <param name="provice"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <returns></returns>
        public List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationResultDTO> GetAllAppSelfTakeStationByWhere(Guid appId, int pageSize, int pageIndex, out int rowCount, string Name, string province, string city, string district)
        {
            //定义返回值
            List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSelfTakeStationResultDTO> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAllAppSelfTakeStationByWhere(appId, pageSize, pageIndex, out rowCount, Name, province, city, district);

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
