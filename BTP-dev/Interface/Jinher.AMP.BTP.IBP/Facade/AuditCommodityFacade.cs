
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/2/24 13:07:41
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class AuditCommodityFacade : BaseFacade<IAuditCommodity>
    {

        /// <summary>
        /// 获取审核商品信息(商铺提交)
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="Name"></param>
        /// <param name="CateNames"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetApplyCommodityList(System.Collections.Generic.List<System.Guid> AppidList, string Name, string CateNames, int AuditState, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetApplyCommodityList(AppidList, Name, CateNames, AuditState, pageIndex, pageSize);
        }
        /// <summary>
        /// 获取审核信息(馆审核)
        /// </summary>
        /// <param name="Appid"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetAuditCommodityList(System.Guid EsAppId, System.Collections.Generic.List<System.Guid> AppidList, string Name, int AuditState, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetAuditCommodityList(EsAppId, AppidList, Name, AuditState, pageIndex, pageSize);
        }
        /// <summary>
        /// 获取商品详情
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CommodityId"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO GetAuditCommodity(System.Guid Id, System.Guid CommodityId, System.Guid AppId)
        {
            base.Do();
            return this.Command.GetAuditCommodity(Id, CommodityId, AppId);
        }
        /// <summary>
        /// 发布的商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddAuditCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO com)
        {
            base.Do();
            return this.Command.AddAuditCommodity(com);
        }
        /// <summary>
        /// 编辑的商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditAuditCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO com)
        {
            base.Do();
            return this.Command.EditAuditCommodity(com);
        }
        /// <summary>
        /// 设置审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetModeStatus(System.Guid Appid, int ModeStatus)
        {
            base.Do();
            return this.Command.SetModeStatus(Appid, ModeStatus);
        }
        /// <summary>
        /// 获取设置的审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.AuditModeDTO GetModeStatus(System.Guid Appid)
        {
            base.Do();
            return this.Command.GetModeStatus(Appid);
        }
        /// <summary>
        /// 手动审核商品
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditApply(System.Collections.Generic.List<System.Guid> ids, int state, string AuditRemark)
        {
            base.Do();
            return this.Command.AuditApply(ids, state, AuditRemark);
        }
        /// <summary>
        /// 获取易捷馆及入住商铺的Appids
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetYiJieAppids()
        {
            base.Do();
            return this.Command.GetYiJieAppids();
        }
        /// <summary>
        /// 判断该商铺商品是否需要被审核
        /// </summary>
        /// <returns></returns>
        public bool IsAuditAppid(System.Guid AppId)
        {
            base.Do();
            return this.Command.IsAuditAppid(AppId);
        }
        /// <summary>
        /// 根据AppId获取审核方式
        /// </summary>
        /// <returns></returns>
        public bool IsAutoModeStatus(System.Guid EsAppId)
        {
            base.Do();
            return this.Command.IsAutoModeStatus(EsAppId);
        }
        /// <summary>
        /// 判断商品是否存在
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <returns></returns>
        public bool IsExistCom(System.Guid CommodityId, System.Guid AppId)
        {
            base.Do();
            return this.Command.IsExistCom(CommodityId, AppId);
        }
        /// <summary>
        /// 判断馆或店铺是否需要审核
        /// </summary>
        /// <returns></returns>
        public bool IsNeedAudit(System.Guid EsAppId)
        {
            base.Do();
            return this.Command.IsNeedAudit(EsAppId);
        }
        /// <summary>
        /// 取出最后提交的待审核商品信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.AuditCommodityDTO GetApplyCommodityInfo(System.Guid CommodityId, System.Guid AppId)
        {
            base.Do();
            return this.Command.GetApplyCommodityInfo(CommodityId, AppId);
        }
    }
}