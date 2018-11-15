
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/18 15:06:51
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Jinher.JAP.BF.IService.Interface;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;

namespace Jinher.AMP.BTP.IBP.IService
{

    [ServiceContract]
    public interface ICommodity : ICommand
    {

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveCommodity(CommodityAndCategoryDTO commodityAndCategoryDTO);

        /// <summary>
        /// 修改商品
        /// </summary>
        /// <param name="commodityAndCategoryDTO">商品扩展实体</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCommodity(CommodityAndCategoryDTO commodityAndCategoryDTO);

        /// <summary>
        /// 查询某个商家所有上架商品
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllCommodityBySellerID", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityVM> GetAllCommodityBySellerIDBySalesvolume(CommoditySearchDTO commoditySearch, out int rowCount);

        /// <summary>
        /// 查询某个商家所有下架商品
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetAllNoOnSaleCommodityBySellerID", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityVM> GetAllNoOnSaleCommodityBySellerIDBySalesvolume(CommoditySearchDTO commoditySearch, out int rowCount);

        /// <summary>
        /// 商品详情
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CommodityAndCategoryDTO GetCommodity(Guid id, Guid appId);

        /// <summary>
        /// 删除商品
        /// </summary>
        /// <param name="id">商品ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteGetCommodity", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteGetCommodity(Guid id);

        /// <summary>
        /// 删除多个商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCommoditys", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCommoditys(List<Guid> ids);

        /// <summary>
        /// 下架商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        [WebInvoke(Method = "POST", UriTemplate = "/OffShelves", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO OffShelves(List<Guid> ids);

        /// <summary>
        /// 上架商品
        /// </summary>
        /// <param name="ids">商品ID集合</param>
        [WebInvoke(Method = "POST", UriTemplate = "/Shelves", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO Shelves(List<Guid> ids);
        /// <summary>
        /// 修改价格
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="price">价格</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdatePrice", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdatePrice(Guid id, decimal price);

        /// <summary>
        /// 修改库存
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="stock">库存</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateStock", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateStock(Guid id, int stock);

        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSalesvolume", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateSalesvolume(System.Guid id, int salesvolume);

        /// <summary>
        /// 修改商品名称
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="name">商品名称</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateName", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateName(Guid id, string name);

        /// <summary>
        /// 获取商品类别
        /// </summary>
        /// <param name="id">商品ID</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCategoryBycommodityId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        [Obsolete("已废弃", false)]
        List<Jinher.AMP.BTP.Deploy.CategoryDTO> GetCategoryBycommodityId(Guid commodityId);
        /// <summary>
        /// 编辑商品类别
        /// </summary>
        /// <param name="UCategoryVM">商品分类VM</param>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCategoryBycommodityId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCategoryBycommodityId(UCategoryVM uCategoryVM);
        /// <summary>
        /// 根据分类获取商品列表
        /// </summary>
        /// <param name="CommodityDTO">商品DTO</param>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByCategoryId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CommodityDTO> GetCommodityByCategoryId(Guid appId, Guid categoryId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">商家ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityVM", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityPromVM> GetCommodityVM(CommoditySearchDTO commoditySearch, Guid promotionId);

        /// <summary>
        /// 检查编号是否存在
        /// </summary>
        /// <param name="code">编号</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        bool IsExists(string code, Guid appId);

        int GetCommodityNum(Guid appId, bool isDel, int state);

        List<string> GetCommodityCodes(Guid appid, List<Guid> commodityIds);

        string GetLastCommodityCode(Guid appId);

        /// <summary>
        /// 保存商品排序结果
        /// </summary>
        /// <param name="comIds">商品Ids</param>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommoditySortValue", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveCommoditySortValue(List<Guid> comIds);

        /// <summary>
        /// 商品置顶排序
        /// </summary>
        /// <param name="comId">商品Id</param>
        [WebInvoke(Method = "POST", UriTemplate = "/SetCommodityFirst", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SetCommodityFirst(Guid comId);

        /// <summary>
        /// 关联商品列表
        /// </summary>
        /// <param name="comId">商品ID</param>
        /// <param name="search"></param>
        /// <returns></returns>
        //[WebInvoke(Method = "POST", UriTemplate = "/RelationCommodityList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommodityPromVM> RelationCommodityList(Guid comId, CommoditySearchDTO search);

        /// <summary>
        /// 修改市场价
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="marketPrice">市场价</param>
        [OperationContract]
        ResultDTO UpdateMarketPrice(System.Guid id, decimal? marketPrice);

        /// <summary>
        /// 修改进货价
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="costPrice">进货价</param>
        [OperationContract]
        ResultDTO UpdateCostPrice(System.Guid id, decimal? costPrice);

        /// <summary>
        /// 设置销售地区
        /// </summary>
        /// <param name="ids">商品Id列表</param>
        /// <param name="saleAreas">销售地区</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateCommodityListSaleAreas", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateCommodityListSaleAreas(System.Collections.Generic.List<System.Guid> ids, string saleAreas);


        /// <summary>
        /// 根据商品名称获取商品列表
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityByName", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityDividendListDTO> GetCommodityByName(Jinher.AMP.BTP.Deploy.CustomDTO.GetCommodityByNameParam pdto);

        /// <summary>
        /// 保存商品分享分成比例。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommodityShareMoney", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySharePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto);



        /// <summary>
        /// 获取店铺的分享分成信息。
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommoditySharePercentByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityShareInfoDTO> GetCommoditySharePercentByAppId(Guid appId);

        /// <summary>
        /// 设置商品是否参加分销
        /// </summary>
        /// <param name="commodityIdList">商品Id列表</param>
        /// <param name="isDistribute">是否分销(false：取消分销。1：参加分销)</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetCommodityDistribution", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityDistribution(List<Guid> commodityIdList, bool isDistribute);

        /// <summary>
        /// 设置各分销商品佣金比例
        /// </summary>
        /// <param name="commodityDistributionList">佣金比例列表</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetDistributionAccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDistributionAccount(List<Jinher.AMP.BTP.Deploy.CustomDTO.CommodityDistributionDTO> commodityDistributionList);

        /// <summary>
        /// 设置分销商品默认佣金比例
        /// </summary>
        /// <param name="appExtension">佣金比例</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetDefaulDistributionAccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetDefaulDistributionAccount(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO appExtension);

        /// <summary>
        /// 获取分销商品默认佣金比例
        /// </summary>
        /// <param name="appId">AppId</param>
        /// <returns>结果</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetDefaulDistributionAccount", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO GetDefaulDistributionAccount(Guid appId);
        /// <summary>
        /// 上移一页保存商品
        /// </summary>
        /// <param name="comIds"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveUpCommoditySortValue", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveUpCommoditySortValue(Guid appId, List<Guid> comIds, Guid id);
        /// <summary>
        /// 下移一页保存商品
        /// </summary>
        /// <param name="comIds"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveDownCommoditySortValue", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveDownCommoditySortValue(Guid appId, List<Guid> comIds, Guid id);


        /// <summary>
        /// 保存商品积分抵用比例。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommodityScorePercent", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommodityScorePercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityScoreDTO dto);



        /// <summary>
        /// 获取店铺的积分抵用信息。
        /// </summary>
        /// <param name="appId">appId</param>
        /// <returns>商品列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityScorePercentByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityScoreDTO> GetCommodityScorePercentByAppId(Guid appId);


                /// <summary>
        /// 根据商品Id获取商品餐盒信息，舌尖项目专用。
        /// </summary>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityBoxInfo", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        CateringComdtyXDataDTO GetCommodityBoxInfo(Guid commodityId);


        /// <summary>
        /// 获取推广主的分享分成信息。
        /// </summary>
        /// <param name="pdto">参数dto</param>
        /// <returns>商品列表</returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommoditySpreadPercentByAppId", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<CommodityShareInfoDTO> GetCommoditySpreadPercentByAppId(Guid appId);


        /// <summary>
        /// 保存推广主的分享分成比例。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveCommoditySpreadPercent", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveCommoditySpreadPercent(Jinher.AMP.BTP.Deploy.CustomDTO.CommodityShareInfoDTO dto);


        /// <summary>
        /// 获取商品税收编码列表。
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSingleCommodityCode", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityTaxRateCDTO>> GetSingleCommodityCode(string name, double taxrate, int pageIndex, int pageSize);


        /// <summary>
        /// 根据id获取商品详情
        /// </summary>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityDetail", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CommodityDTO GetCommodityDetail(Guid CommodityId);



        /// <summary>
        /// 设置退货运费物流模板
        /// </summary>
        /// <param name="CommodityId"></param>
        /// <param name="TempId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SetCommodityRefoundFreightTemp", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetCommodityRefoundFreightTemp(Guid CommodityId, Guid TempId);

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
        [WebInvoke(Method = "POST", UriTemplate = "/GetCommodityFreightTemplate", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<List<CommodityAndTemplateDTO>> GetCommodityFreightTemplate(Guid AppId, String GoodName, String State, String FreightID, int pageIndex, int pageSize);
    }
}
