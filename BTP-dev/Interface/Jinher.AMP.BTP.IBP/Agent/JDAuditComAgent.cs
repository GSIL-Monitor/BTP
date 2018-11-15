
/***************
功能描述: BTPProxy
作    者: 
创建时间: 2018/3/15 14:28:31
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

    public class JDAuditComAgent : BaseBpAgent<IJDAuditCom>, IJDAuditCom
    {
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetEditPriceList(System.Guid AppId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetEditPriceList(AppId, Name, JdCode, AuditState, MinRate, MaxRate, EditStartime, EditEndTime, Action, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetEditPriceMode(System.Guid Appid, int ModeStatus)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetEditPriceMode(Appid, ModeStatus);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetEditCostPriceMode(System.Guid Appid, int ModeStatus)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetEditCostPriceMode(Appid, ModeStatus);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditJDPrice(System.Collections.Generic.List<System.Guid> ids, int state, decimal SetPrice, string AuditRemark, int JdAuditMode)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AuditJDPrice(ids, state, SetPrice, AuditRemark, JdAuditMode);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditJDCostPrice(System.Collections.Generic.List<System.Guid> ids, int state, string AuditRemark, int Dispose, int JdAuditMode)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.AuditJDCostPrice(ids, state, AuditRemark, Dispose, JdAuditMode);

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
        public Jinher.AMP.BTP.Deploy.JDAuditModeDTO GetAuditMode(System.Guid AppId)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.JDAuditModeDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.GetAuditMode(AppId);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> ExportPriceList(System.Guid AppId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.ExportPriceList(AppId, Name, JdCode, AuditState, MinRate, MaxRate, EditStartime, EditEndTime, Action);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetOffSaleAndNoStockList(System.Guid AppId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.GetOffSaleAndNoStockList(AppId, Name, JdCode, AuditState, JdStatus, EditStartime, EditEndTime, Action, pageIndex, pageSize);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetOffAndNoStockMode(System.Guid Appid, int ModeStatus)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetOffAndNoStockMode(Appid, ModeStatus);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> ExportOffSaleAndNoStockData(System.Guid AppId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> result;

            try
            {
                //调用代理方法
                result = base.Channel.ExportOffSaleAndNoStockData(AppId, Name, JdCode, AuditState, JdStatus, EditStartime, EditEndTime, Action);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetInStore(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetInStore(ids, JdAuditMode);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetPutaway(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetPutaway(ids, JdAuditMode);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetNoStock(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetNoStock(ids, JdAuditMode);

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
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetOffShelf(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            //定义返回值
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO result;

            try
            {
                //调用代理方法
                result = base.Channel.SetOffShelf(ids, JdAuditMode);

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
