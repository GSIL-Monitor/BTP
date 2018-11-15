
/***************
功能描述: BTPIService
作    者: 
创建时间: 2014/3/20 19:40:00
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

namespace Jinher.AMP.BTP.ISV.IService
{
    /// <summary>
    /// 购物车接口
    /// </summary>
    [ServiceContract]
    public interface IShoppingCart : ICommand
    {
        /// <summary>
        /// 添加购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/SaveShoppingCart
        /// </para>
        /// </summary>
        /// <param name="shoppingCartItemsSDTO">购物车商品实体</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveShoppingCart(ShoppingCartItemSDTO shoppingCartItemsSDTO, Guid userId, Guid appId);

        /// <summary>
        /// 获取购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/GetShoppongCartItems
        /// </para>
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppongCartItems", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommoditySDTO> GetShoppongCartItems(Guid userId, Guid appId);

        /// <summary>
        /// 获取购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/GetShoppongCartItemsNew
        /// </para>
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppongCartItemsNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommoditySDTO> GetShoppongCartItemsNew(Guid userId, Guid appId);

        /// <summary>
        /// 编辑购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/UpdateShoppingCart
        /// </para>
        /// </summary>
        /// <param name="shopCartCommodityUpdateDTOs">购物车编辑实体</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateShoppingCart(List<ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs, Guid userId, Guid appId);

        /// <summary>
        /// 删除购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/DeleteShoppingCart
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteShoppingCart(Guid shopCartItemId, Guid userId, Guid appId);

        /// <summary>
        /// 购物车商品数量 -- 可以不用提供，因为直接返回所有的购物车商品，直接调用商品list.count获取总数
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/GetShoppingCartNum
        /// </para>
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppingCartNum", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        NumResultSDTO GetShoppingCartNum(Guid userId, Guid appId);


        /// <summary>
        /// 分享订单-将订单商品复制到购物车当中
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CopyOrderToShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO CopyOrderToShoppingCart(Guid userId, Guid orderId, Guid appId);

        /// <summary>
        /// 分享的订单复制订单中的商品到购物车
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="orderId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/CopyShareOrderToShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<List<Guid>> CopyShareOrderToShoppingCart(Guid userId, Guid orderId, Guid appId);


        /// <summary>
        /// 获取我的购物车 --- 厂家直销
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetMyShoppongCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<CommoditySDTO> GetMyShoppongCart(Guid userId);

        /// <summary>
        /// 添加到购物车 --- 厂家直销
        /// </summary>
        /// <param name="shoppingCartItemSDTO">shoppingCartItemSDTOID</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveSetShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO SaveSetShoppingCart(ShoppingCartItemSDTO shoppingCartItemSDTO, Guid userId);

        /// <summary>
        /// 修改购物车 --- 厂家直销
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="shopCartCommodityUpdateDTOs">shopCartCommodityUpdateDTOs</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateSetShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO UpdateSetShoppingCart(Guid userId, List<ShopCartCommodityUpdateDTO> shopCartCommodityUpdateDTOs);

        /// <summary>
        /// 查询购物车商品数量 --- 厂家直销
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetSetShoppingCartNum", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        NumResultSDTO GetSetShoppingCartNum(Guid userId);
        /// <summary>
        /// 厂家直营获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppongCartList", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCarCommodityListDTO> GetShoppongCartList
           (System.Guid userId, System.Guid appId);


        /// <summary>
        /// 删除购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/DeleteCommoditysFromShoppingCart
        /// </para>
        /// </summary>
        /// <param name="commodityId">商品Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteCommoditysFromShoppingCart", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteCommoditysFromShoppingCart(List<Guid> shopCartItemIds, Guid userId, Guid appId);


        /// <summary>
        /// 添加购物车
        /// <para>Service Url: http://devbtp.sv.iuoooo.com/Jinher.AMP.BTP.SV.ShoppingCartSV.svc/SaveShoppingCartNew
        /// </para>
        /// </summary>
        /// <param name="shoppingCartItemsSDTO">购物车商品实体</param>
        ///<param name="sscDto">加入购物车的商品、数量等信息</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/SaveShoppingCartNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<Guid> SaveShoppingCartNew(SaveShoppingCartParamDTO sscDto);


        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppongCartItemsNew2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommoditySDTO> GetShoppongCartItemsNew2(System.Guid userId, System.Guid appId);

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户的Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppongCartItemsNew3", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartListDTO> GetShoppongCartItemsNew3(System.Guid userId, System.Guid appId);

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="shopCartItemIds">购物车Ids</param>
        /// <param name="userId">用户Id</param>
        /// <param name="appId">appId</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/DeleteShoppingCart2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO DeleteShoppingCart2(List<Guid> shopCartItemIds, System.Guid userId, System.Guid appId);

        /// <summary>
        /// 获取商品的属性和优惠信息
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppongCartItemAttribute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttribute(System.Guid commodityId, Guid userId);

        /// <summary>
        /// 获取商品的属性和优惠信息新（支持单属性SKU）
        /// </summary>
        /// <param name="commodityId">商品id</param>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/GetShoppongCartItemAttributeNew", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ShopCartCommodityAttrDTO> GetShoppongCartItemAttributeNew(System.Guid commodityId, Guid userId);

        /// <summary>
        /// 编辑购物车项数量
        /// </summary>
        /// <param name="shopCartCommodityUpdateDto">购物车编辑实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateShoppingCart2", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingCart2(Jinher.AMP.BTP.Deploy.CustomDTO.ShopCartCommodityUpdateDTO shopCartCommodityUpdateDto);

        /// <summary>
        /// 编辑购物车项属性
        /// </summary>
        /// <param name="shopAttribute">商品属性对实体</param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateShoppingAttribute", BodyStyle = WebMessageBodyStyle.WrappedRequest, ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        [OperationContract]
        ResultDTO<ShopCartUpdateResultDTO> UpdateShoppingAttribute(ShopAttributeCommodityUpdateDto shopAttribute);
    }
}
