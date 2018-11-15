
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/2/24 13:07:46
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

    public class AuditCommodityAgent : BaseBpAgent<IAuditCommodity>, IAuditCommodity
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetApplyCommodityList(System.Collections.Generic.List<System.Guid> AppidList, string Name, string CateNames, int AuditState, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetApplyCommodityList(AppidList, Name, CateNames, AuditState, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetAuditCommodityList(System.Guid EsAppId, System.Collections.Generic.List<System.Guid> AppidList, string Name, int AuditState, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAuditCommodityList(EsAppId, AppidList, Name, AuditState, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO GetAuditCommodity(System.Guid Id, System.Guid CommodityId, System.Guid AppId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAuditCommodity(Id, CommodityId, AppId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddAuditCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO com)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AddAuditCommodity(com);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditAuditCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO com)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.EditAuditCommodity(com);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetModeStatus(System.Guid Appid, int ModeStatus)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetModeStatus(Appid, ModeStatus);

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
        public Jinher.AMP.BTP.Deploy.AuditModeDTO GetModeStatus(System.Guid Appid)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.AuditModeDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetModeStatus(Appid);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditApply(System.Collections.Generic.List<System.Guid> ids, int state, string AuditRemark)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AuditApply(ids, state, AuditRemark);

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
        public System.Collections.Generic.List<System.Guid> GetYiJieAppids()
        {
            //定义返回值
            System.Collections.Generic.List<System.Guid> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetYiJieAppids();

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
        public bool IsAuditAppid(System.Guid AppId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.IsAuditAppid(AppId);

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
        public bool IsAutoModeStatus(System.Guid EsAppId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.IsAutoModeStatus(EsAppId);

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
        public bool IsExistCom(System.Guid CommodityId, System.Guid AppId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.IsExistCom(CommodityId, AppId);

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
        public bool IsNeedAudit(System.Guid EsAppId)
        {
            //定义返回值
            bool result;

            try
            {
                //调用代理方法
                result = base.Channel.IsNeedAudit(EsAppId);

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
        public Jinher.AMP.BTP.Deploy.AuditCommodityDTO GetApplyCommodityInfo(System.Guid CommodityId, System.Guid AppId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.AuditCommodityDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetApplyCommodityInfo(CommodityId, AppId);

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
