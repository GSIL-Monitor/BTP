
/***************
功能描述: BTPBP
作    者: 
创建时间: 2018/3/15 14:28:29
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
    public partial class JDAuditComBP : BaseBP, IJDAuditCom
    {

        /// <summary>
        /// 京东售价审核列表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetEditPriceList(System.Guid AppId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetEditPriceListExt(AppId, Name, JdCode, AuditState, MinRate, MaxRate, EditStartime, EditEndTime, Action, pageIndex, pageSize);
        }
        /// <summary>
        /// 设置售价审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetEditPriceMode(System.Guid Appid, int ModeStatus)
        {
            base.Do();
            return this.SetEditPriceModeExt(Appid, ModeStatus);
        }
        /// <summary>
        /// 设置进货价审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetEditCostPriceMode(System.Guid Appid, int ModeStatus)
        {
            base.Do();
            return this.SetEditCostPriceModeExt(Appid, ModeStatus);
        }
        /// <summary>
        /// 审核京东售价
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditJDPrice(System.Collections.Generic.List<System.Guid> ids, int state, decimal SetPrice, string AuditRemark, int JdAuditMode)
        {
            base.Do();
            return this.AuditJDPriceExt(ids, state, SetPrice, AuditRemark, JdAuditMode);
        }
        /// <summary>
        /// 审核京东进货价
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO AuditJDCostPrice(System.Collections.Generic.List<System.Guid> ids, int state, string AuditRemark, int Dispose, int JdAuditMode)
        {
            base.Do();
            return this.AuditJDCostPriceExt(ids, state, AuditRemark, Dispose, JdAuditMode);
        }
        /// <summary>
        /// 获取商铺审核方式
        /// </summary>
        /// <param name="AuditManage"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.JDAuditModeDTO GetAuditMode(System.Guid AppId)
        {
            base.Do();
            return this.GetAuditModeExt(AppId);
        }
        /// <summary>
        /// 导出京东进货价审核列表
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> ExportPriceList(System.Guid AppId, string Name, string JdCode, int AuditState, decimal MinRate, decimal MaxRate, string EditStartime, string EditEndTime, int Action)
        {
            base.Do();
            return this.ExportPriceListExt(AppId, Name, JdCode, AuditState, MinRate, MaxRate, EditStartime, EditEndTime, Action);
        }
        /// <summary>
        /// 获取下架无货商品审核列表
        /// </summary>
        /// <param name="AppIds"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="EditStartime"></param>
        /// <param name="EditEndTime"></param>
        /// <param name="Action"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> GetOffSaleAndNoStockList(System.Guid AppId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action, int pageIndex, int pageSize)
        {
            base.Do();
            return this.GetOffSaleAndNoStockListExt(AppId, Name, JdCode, AuditState, JdStatus, EditStartime, EditEndTime, Action, pageIndex, pageSize);
        }
        /// <summary>
        /// 设置下架无货商品审核方式
        ///  </summary>
        /// <param name="Appid"></param>
        /// <param name="ModeStatus"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetOffAndNoStockMode(System.Guid Appid, int ModeStatus)
        {
            base.Do();
            return this.SetOffAndNoStockModeExt(Appid, ModeStatus);
        }
        /// <summary>
        /// 导出下架无货商品审核列表数据
        /// </summary>
        /// <param name="AppIds"></param>
        /// <param name="Name"></param>
        /// <param name="AuditState"></param>
        /// <param name="EditStartime"></param>
        /// <param name="EditEndTime"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO>> ExportOffSaleAndNoStockData(System.Guid AppId, string Name, string JdCode, int AuditState, int JdStatus, string EditStartime, string EditEndTime, int Action)
        {
            base.Do();
            return this.ExportOffSaleAndNoStockDataExt(AppId, Name, JdCode, AuditState, JdStatus, EditStartime, EditEndTime, Action);
        }
        /// <summary>
        /// 置为有货
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetInStore(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            base.Do();
            return this.SetInStoreExt(ids, JdAuditMode);
        }
        /// <summary>
        /// 置为上架
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetPutaway(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            base.Do();
            return this.SetPutawayExt(ids, JdAuditMode);
        }
        /// <summary>
        /// 置为售罄
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetNoStock(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            base.Do();
            return this.SetNoStockExt(ids, JdAuditMode);
        }
        /// <summary>
        /// 置为下架
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetOffShelf(System.Collections.Generic.List<System.Guid> ids, int JdAuditMode)
        {
            base.Do();
            return this.SetOffShelfExt(ids, JdAuditMode);
        }
    }
}