
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2018/9/7 9:39:04
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.ISV.IService;

namespace Jinher.AMP.BTP.ISV.Facade
{
    public class CommodityForYJBFacade : BaseFacade<ICommodityForYJB>
    {

        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="input">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ListResult<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityListOutPut>> GetCommodities(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchInput input)
        {
            base.Do();
            return this.Command.GetCommodities(input);
        }
        /// <summary>
        /// 根据馆下所有商品
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="commodityCategory">栏目名称</param>
        /// <param name="commodityName">商品名称</param>
        /// <param name="pageIndex">第几页（从1开始）</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchResultDTO GetAllCommodities(System.Guid appId, string commodityCategory, string commodityName, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetAllCommodities(appId, commodityCategory, commodityName, pageIndex, pageSize);
        }
        /// <summary>
        /// 根据店铺名称获取商品列表
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="appId">店铺id</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO>> GetAppIdCommodity(string name, System.Guid appId, decimal price, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetAppIdCommodity(name, appId, price, pageIndex, pageSize);
        }
        /// <summary>
        /// 根据id显示商品信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO>> GetCommodityById(System.Guid appid, System.Collections.Generic.List<System.Guid> ids, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetCommodityById(appid, ids, pageIndex, pageSize);
        }
        /// <summary>
        /// 根据id获取商品信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ComAttrDTO> GetCommodityByIds(System.Guid appid, System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.Command.GetCommodityByIds(appid, ids);
        }
        /// <summary>
        /// 定时修改商品价格
        /// </summary>
        /// <param name="CkPriceList"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityPrice(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo)
        {
            base.Do();
            return this.Command.UpdateCommodityPrice(CkPriceInfo);
        }
        /// <summary>
        /// 审核通过后撤销.编辑,恢复已变更的数据
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO RecoverCommodityPrice(Jinher.AMP.YJB.Deploy.CustomDTO.ChangePriceDetailDTO CkPriceInfo)
        {
            base.Do();
            return this.Command.RecoverCommodityPrice(CkPriceInfo);
        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyInfoList(System.Guid EsappId)
        {
            base.Do();
            return this.Command.GetMallApplyInfoList(EsappId);
        }
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierInfoList(System.Guid appId)
        {
            base.Do();
            return this.Command.GetSupplierInfoList(appId);
        }
        /// <summary>
        /// 查询商城信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyList()
        {
            base.Do();
            return this.Command.GetMallApplyList();
        }
        /// <summary>
        /// 查询供应商信息
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.SupplierDTO> GetSupplierList()
        {
            base.Do();
            return this.Command.GetSupplierList();
        }
        /// <summary>
        ///  查询App入驻信息
        /// </summary>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.MallApplyDTO> GetMallApplyByIds(System.Guid esAppId, System.Collections.Generic.List<System.Guid> appIds)
        {
            base.Do();
            return this.Command.GetMallApplyByIds(esAppId, appIds);
        }
        /// <summary>
        /// 获取所有的严选appId
        /// </summary>
        public System.Collections.Generic.List<System.Guid> GetYXappIds()
        {
            base.Do();
            return this.Command.GetYXappIds();
        }
        /// <summary>
        /// 导出定时改价未改变价格的订单信息
        ///  </summary>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> GetOrderItemList(string StarTime, string EndTime)
        {
            base.Do();
            return this.Command.GetOrderItemList(StarTime, EndTime);
        }
        /// <summary>
        /// 导出定时改价未改变价格的订单信息
        ///  </summary>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.OrderItemDTO> GetOrderInfoByAppId(System.Guid AppId, string StarTime, string EndTime)
        {
            base.Do();
            return this.Command.GetOrderInfoByAppId(AppId, StarTime, EndTime);
        }
    }
}