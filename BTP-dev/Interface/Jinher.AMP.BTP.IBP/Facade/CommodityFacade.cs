
/***************
功能描述: BTPFacade
作    者: 
创建时间: 2017/6/8 18:52:47
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.JAP.BF.Facade.Base;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;

namespace Jinher.AMP.BTP.IBP.Facade
{
    public class CommodityFacade : BaseFacade<ICommodity>
    {

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO commodityAndCategoryDTO)
        {
            base.Do();
            return this.Command.SaveCommodity(commodityAndCategoryDTO);
        }
        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodity(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO commodityAndCategoryDTO)
        {
            base.Do();
            return this.Command.UpdateCommodity(commodityAndCategoryDTO);
        }
        /// <summary>
        /// 查询某个商家所有上架商品
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM> GetAllCommodityBySellerIDBySalesvolume(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, out int rowCount)
        {
            base.Do();
            return this.Command.GetAllCommodityBySellerIDBySalesvolume(commoditySearch,out rowCount);
        }
        /// <summary>
        /// 查询某个商家所有下架商品
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityVM> GetAllNoOnSaleCommodityBySellerIDBySalesvolume(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, out int rowCount)
        {
            base.Do();
            return this.Command.GetAllNoOnSaleCommodityBySellerIDBySalesvolume(commoditySearch, out rowCount);
        }
        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndCategoryDTO GetCommodity(System.Guid id, System.Guid appId)
        {
            base.Do();
            return this.Command.GetCommodity(id, appId);
        }
        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id">商品ID</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteGetCommodity(System.Guid id)
        {
            base.Do();
            return this.Command.DeleteGetCommodity(id);
        }
        /// <summary>
        /// 删除多个商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO DeleteCommoditys(System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.Command.DeleteCommoditys(ids);
        }
        /// <summary>
        /// 下架商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO OffShelves(System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.Command.OffShelves(ids);
        }
        /// <summary>
        /// 上架商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO Shelves(System.Collections.Generic.List<System.Guid> ids)
        {
            base.Do();
            return this.Command.Shelves(ids);
        }
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="price">价格</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdatePrice(System.Guid id, decimal price)
        {
            base.Do();
            return this.Command.UpdatePrice(id, price);
        }
        /// <summary>
        /// 修改库存
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="stock">库存</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateStock(System.Guid id, int stock)
        {
            base.Do();
            return this.Command.UpdateStock(id, stock);
        }
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSalesvolume(System.Guid id, int salesvolume)
        {
            base.Do();
            return this.Command.UpdateSalesvolume(id, salesvolume);
        }
        /// <summary>
        /// 修改商品名称
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="name">商品名称</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateName(System.Guid id, string name)
        {
            base.Do();
            return this.Command.UpdateName(id, name);
        }
        /// <summary>
        /// 获取商品类别
        /// </summary>
        /// <param name="id">商品ID</param>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryBycommodityId(System.Guid commodityId)
        {
            base.Do();
            return this.Command.GetCategoryBycommodityId(commodityId);
        }
        /// <summary>
        /// 编辑商品类别
        /// </summary>
        /// <param name="UCategoryVM">商品分类VM</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCategoryBycommodityId(Jinher.AMP.BTP.Deploy.CustomDTO.UCategoryVM uCategoryVM)
        {
            base.Do();
            return this.Command.UpdateCategoryBycommodityId(uCategoryVM);
        }
        /// <summary>
        /// 根据分类获取商品列表
        /// </summary>
        /// <param name="CommodityDTO">商品DTO</param>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetCommodityByCategoryId(System.Guid appId, System.Guid categoryId)
        {
            base.Do();
            return this.Command.GetCommodityByCategoryId(appId, categoryId);
        }
        /// <summary>
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPromVM> GetCommodityVM(Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO commoditySearch, System.Guid promotionId)
        {
            base.Do();
            return this.Command.GetCommodityVM(commoditySearch, promotionId);
        }
        /// <summary>
        /// 检查编号是否存在
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public bool IsExists(string code, System.Guid appId)
        {
            base.Do();
            return this.Command.IsExists(code, appId);
        }
        public int GetCommodityNum(System.Guid appId, bool isDel, int state)
        {
            base.Do();
            return this.Command.GetCommodityNum(appId, isDel, state);
        }
        public System.Collections.Generic.List<string> GetCommodityCodes(System.Guid appid, System.Collections.Generic.List<System.Guid> commodityIds)
        {
            base.Do();
            return this.Command.GetCommodityCodes(appid, commodityIds);
        }
        public string GetLastCommodityCode(System.Guid appId)
        {
            base.Do();
            return this.Command.GetLastCommodityCode(appId);
        }
        /// <summary>
        /// 保存商品排序结果
        /// </summary>
        /// <param name="comIds">商品Ids</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySortValue(System.Collections.Generic.List<System.Guid> comIds)
        {
            base.Do();
            return this.Command.SaveCommoditySortValue(comIds);
        }
        /// <summary>
        /// 商品置顶排序
        /// </summary>
        /// <param name="comId">商品Id</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityFirst(System.Guid comId)
        {
            base.Do();
            return this.Command.SetCommodityFirst(comId);
        }
        /// <summary>
        /// 关联商品列表
        /// </summary>
        /// <param name="comId">商品ID</param>
        /// <param name="search"></param>
        /// <returns></returns>
        public System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityPromVM> RelationCommodityList(System.Guid comId, Jinher.AMP.BTP.Deploy.CustomDTO.CommoditySearchDTO search)
        {
            base.Do();
            return this.Command.RelationCommodityList(comId, search);
        }
        /// <summary>
        /// 修改市场价
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="marketPrice">市场价</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateMarketPrice(System.Guid id, decimal? marketPrice)
        {
            base.Do();
            return this.Command.UpdateMarketPrice(id, marketPrice);
        }
        /// <summary>
        /// 修改进货价
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="costPrice">进货价</param>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCostPrice(System.Guid id, decimal? costPrice)
        {
            base.Do();
            return this.Command.UpdateCostPrice(id, costPrice);
        }
        /// <summary>
        /// 设置销售地区
        /// </summary>
        /// <param name="ids">商品Id列表</param>
        /// <param name="saleAreas">销售地区</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateCommodityListSaleAreas(System.Collections.Generic.List<System.Guid> ids, string saleAreas)
        {
            base.Do();
            return this.Command.UpdateCommodityListSaleAreas(ids, saleAreas);
        }
        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDividendListDTO> GetCommodityByName(Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam pdto)
        {
            base.Do();
            return this.Command.GetCommodityByName(pdto);
        }
        /// <summary>
        /// 保存商品分享分成比例。
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySharePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            base.Do();
            return this.Command.SaveCommoditySharePercent(dto);
        }
        /// <summary>
        /// 获取店铺的分享分成信息。
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO> GetCommoditySharePercentByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetCommoditySharePercentByAppId(appId);
        }
        /// <summary>
        /// 设置商品是否参加分销
        /// </summary>
        /// <param name="commodityIdList">商品Id列表</param>
        /// <param name="isDistribute">是否分销(false：取消分销。1：参加分销)</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityDistribution(System.Collections.Generic.List<System.Guid> commodityIdList, bool isDistribute)
        {
            base.Do();
            return this.Command.SetCommodityDistribution(commodityIdList, isDistribute);
        }
        /// <summary>
        /// 设置各分销商品佣金比例
        /// </summary>
        /// <param name="commodityDistributionList">佣金比例列表</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDistributionAccount(System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDistributionDTO> commodityDistributionList)
        {
            base.Do();
            return this.Command.SetDistributionAccount(commodityDistributionList);
        }
        /// <summary>
        /// 设置分销商品默认佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaulDistributionAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension)
        {
            base.Do();
            return this.Command.SetDefaulDistributionAccount(appExtension);
        }
        /// <summary>
        /// 获取分销商品默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulDistributionAccount(System.Guid appId)
        {
            base.Do();
            return this.Command.GetDefaulDistributionAccount(appId);
        }
        /// <summary>
        /// 上移一页保存商品
        /// </summary>
        /// <param name="comIds"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUpCommoditySortValue(System.Guid appId, System.Collections.Generic.List<System.Guid> comIds, System.Guid id)
        {
            base.Do();
            return this.Command.SaveUpCommoditySortValue(appId, comIds, id);
        }
        /// <summary>
        /// 下移一页保存商品
        /// </summary>
        /// <param name="comIds"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDownCommoditySortValue(System.Guid appId, System.Collections.Generic.List<System.Guid> comIds, System.Guid id)
        {
            base.Do();
            return this.Command.SaveDownCommoditySortValue(appId, comIds, id);
        }
        /// <summary>
        /// 保存商品积分抵用比例。
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityScorePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityScoreDTO dto)
        {
            base.Do();
            return this.Command.SaveCommodityScorePercent(dto);
        }
        /// <summary>
        /// 获取店铺的积分抵用信息。
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityScoreDTO> GetCommodityScorePercentByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetCommodityScorePercentByAppId(appId);
        }
        /// <summary>
        /// 根据商品Id获取商品餐盒信息，舌尖项目专用。
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CateringComdtyXDataDTO GetCommodityBoxInfo(System.Guid commodityId)
        {
            base.Do();
            return this.Command.GetCommodityBoxInfo(commodityId);
        }
        /// <summary>
        /// 获取推广主的分享分成信息。
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO> GetCommoditySpreadPercentByAppId(System.Guid appId)
        {
            base.Do();
            return this.Command.GetCommoditySpreadPercentByAppId(appId);
        }
        /// <summary>
        /// 保存推广主的分享分成比例。
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySpreadPercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto)
        {
            base.Do();
            return this.Command.SaveCommoditySpreadPercent(dto);
        }

        /// <summary>
        /// 获取商品税收编码列表。
        /// </summary>
        /// <returns></returns>
        public ResultDTO<List<CommodityTaxRateCDTO>> GetSingleCommodityCode(string name, double taxrate, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetSingleCommodityCode(name, taxrate, pageIndex, pageSize);
        }


        /// <summary>
        ///根据id获取商品详情
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CommodityDTO GetCommodityDetail(Guid CommodityId)
        {
            base.Do();
            return this.Command.GetCommodityDetail(CommodityId);
        }

     

        /// <summary>
        /// 设置退货运费物流模板
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <param name="TempId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityRefoundFreightTemp(System.Guid CommodityId, System.Guid TempId)
        {
            base.Do();
            return this.Command.SetCommodityRefoundFreightTemp(CommodityId, TempId);
        }
        /// <summary>
        /// 查询商品物流模板
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="GoodName"></param>
        /// <param name="State"></param>
        /// <param name="FreightID"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityAndTemplateDTO>> GetCommodityFreightTemplate(System.Guid AppId, string GoodName, string State, string FreightID, int pageIndex, int pageSize)
        {
            base.Do();
            return this.Command.GetCommodityFreightTemplate(AppId, GoodName, State, FreightID, pageIndex, pageSize);
        }
    }
}
