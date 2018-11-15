
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/2/24 13:07:44
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using System.ServiceModel.Activation;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public partial class AuditCommodityBP : BaseBP, IAuditCommodity
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
            base.Do(false);
            return this.GetApplyCommodityListExt(AppidList, Name, CateNames, AuditState, pageIndex, pageSize);
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
            base.Do(false);
            return this.GetAuditCommodityListExt(EsAppId, AppidList, Name, AuditState, pageIndex, pageSize);
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
            base.Do(false);
            return this.GetAuditCommodityExt(Id, CommodityId, AppId);
        }
        /// <summary>
        /// 发布的商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AddAuditCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO com)
        {
            base.Do(false);
            return this.AddAuditCommodityExt(com);
        }
        /// <summary>
        /// 编辑的商品插入AuditCommodity表
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO EditAuditCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO com)
        {
            base.Do(false);
            return this.EditAuditCommodityExt(com);
        }
        /// <summary>
        /// 设置审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetModeStatus(System.Guid Appid, int ModeStatus)
        {
            base.Do(false);
            return this.SetModeStatusExt(Appid, ModeStatus);
        }
        /// <summary>
        /// 获取设置的审核方式
        /// </summary>
        /// <param name="Appid"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.AuditModeDTO GetModeStatus(System.Guid Appid)
        {
            base.Do(false);
            return this.GetModeStatusExt(Appid);
        }
        /// <summary>
        /// 手动审核商品
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditApply(System.Collections.Generic.List<System.Guid> ids, int state, string AuditRemark)
        {
            base.Do(false);
            return this.AuditApplyExt(ids, state, AuditRemark);
        }
        /// <summary>
        /// 获取易捷馆及入住商铺的Appids
        /// </summary>
        /// <returns></returns>
        public System.Collections.Generic.List<System.Guid> GetYiJieAppids()
        {
            base.Do(false);
            return this.GetYiJieAppidsExt();
        }
        /// <summary>
        /// 判断该商铺商品是否需要被审核
        /// </summary>
        /// <returns></returns>
        public bool IsAuditAppid(System.Guid AppId)
        {
            base.Do(false);
            return this.IsAuditAppidExt(AppId);
        }
        /// <summary>
        /// 根据AppId获取审核方式
        /// </summary>
        /// <returns></returns>
        public bool IsAutoModeStatus(System.Guid EsAppId)
        {
            base.Do(false);
            return this.IsAutoModeStatusExt(EsAppId);
        }
        /// <summary>
        /// 判断商品是否存在
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <returns></returns>
        public bool IsExistCom(System.Guid CommodityId, System.Guid AppId)
        {
            base.Do(false);
            return this.IsExistComExt(CommodityId, AppId);
        }
        /// <summary>
        /// 判断馆或店铺是否需要审核
        /// </summary>
        /// <returns></returns>
        public bool IsNeedAudit(System.Guid EsAppId)
        {
            base.Do(false);
            return this.IsNeedAuditExt(EsAppId);
        }
        /// <summary>
        /// 取出最后提交的待审核商品信息
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.AuditCommodityDTO GetApplyCommodityInfo(System.Guid CommodityId, System.Guid AppId)
        {
            base.Do(false);
            return this.GetApplyCommodityInfoExt(CommodityId, AppId);
        }
    }
}