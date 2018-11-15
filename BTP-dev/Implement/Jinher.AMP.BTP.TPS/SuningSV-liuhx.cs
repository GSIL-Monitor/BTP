extern alias snsdk;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest;
using snsdk.suning_api_sdk.BizResponse.CustomGovbusResponse;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO.SN;

namespace Jinher.AMP.BTP.TPS
{
    public partial class SuningSV
    {

        /// <summary>
        /// suning.govbus.category.get 获取商品目录接口 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetCategory()
        {
            try
            {
                List<string> list = new List<string>();
                LogHelper.Info("SuningSV.GetCategory 获取Category");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.CategoryGetRequest();
                var result = SuningClient.Execute(request);
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        list.Add(item.categoryId);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetCategory 获取Category 异常", ex);
            }
            return new List<string>();
        }
        /// <summary>
        /// suning.govbus.prodpool.query 获取商品池 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public static List<string> GetProdPool(string categoryId)
        {
            try
            {
                List<string> list = new List<string>();
                LogHelper.Info("SuningSV.GetProdPool 获取prodpool");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdPoolQueryRequest();
                request.categoryId = categoryId;
                var result = SuningClient.Execute(request);
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        list.Add(item.skuId);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetProdPool 获取prodpool 异常", ex);
            }
            return new List<string>();
        }
        /// <summary>
        /// suning.govbus.proddetail.get 获取商品详情接口
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static SNComDetailDto GetSNDetail(string skuId)
        {
            SNComDetailDto SnDetail = new SNComDetailDto();
            try
            {

                LogHelper.Info("SuningSV.GetSNDetail 获取proddetail");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdDetailGetRequest();
                request.skuId = skuId;
                var result = SuningClient.Execute(request);
                if (!string.IsNullOrEmpty(result.name))
                {
                    SnDetail.brand = result.brand;
                    SnDetail.category = result.category;
                    SnDetail.image = result.image;
                    SnDetail.introduction = result.introduction;
                    SnDetail.packlisting = result.packlisting;
                    SnDetail.model = result.model;
                    SnDetail.name = result.name;
                    SnDetail.productArea = result.productArea;
                    SnDetail.saleUnit = result.saleUnit;
                    SnDetail.skuId = result.skuId;
                    SnDetail.state = result.state;
                    SnDetail.upc = result.upc;
                    SnDetail.weight = result.weight;
                    List<ProdParams> pros = new List<ProdParams>();
                    List<Param> pa = new List<Param>();
                    if (result.prodParams != null)
                    {
                        foreach (var item in result.prodParams)
                        {
                            ProdParams pro = new ProdParams();
                            pro.desc = item.desc;
                            if (item.param != null)
                            {
                                foreach (var model in item.param)
                                {
                                    Param p = new Param();
                                    p.coreFlag = model.coreFlag;
                                    p.snparameterCode = model.snparameterCode;
                                    p.snparameterdesc = model.snparameterdesc;
                                    p.snparametersCode = model.snparametersCode;
                                    p.snparametersDesc = model.snparametersDesc;
                                    p.snparameterSequence = model.snparameterSequence;
                                    p.snparameterVal = model.snparameterVal;
                                    p.snsequence = model.snsequence;
                                    pro.param.Add(p);
                                }
                            }
                            pros.Add(pro);
                        }
                        SnDetail.prodParams = pros;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetProdPool 获取prodpool 异常", ex);
            }
            return SnDetail;
        }
        /// <summary>
        /// suning.govbus.prodimage.query 获取商品图片接口 
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<SNComPicturesDto> GetComPictures(List<string> skuId)
        {
            List<SNComPicturesDto> list = new List<SNComPicturesDto>();
            try
            {
                LogHelper.Info("SuningSV.GetComPictures 批量获取苏宁图片");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdImageQueryRequest();
                request.skuIds = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdImageQuerySkuIdsReq>();
                foreach (var item in skuId)
                {
                    var sku = new snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdImageQuerySkuIdsReq();
                    sku.skuId = item;
                    request.skuIds.Add(sku);
                }
                var result = SuningClient.Execute(request);
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        if (item.urls != null)
                        {
                            foreach (var model in item.urls)
                            {
                                SNComPicturesDto picture = new SNComPicturesDto();
                                picture.skuId = item.skuId;
                                picture.path = model.path;
                                picture.primary = model.primary;
                                list.Add(picture);
                            }
                        }
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetComPictures 批量获取苏宁图片异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.price.query 批量查询商品价格接口
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<SNPriceDto> GetPrice(List<string> skuId)
        {
            List<SNPriceDto> list = new List<SNPriceDto>();
            try
            {
                LogHelper.Info("SuningSV.GetPrice 批量获取苏宁价格");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.PriceQueryRequest();
                //全国统一价 传取价的城市 010-北京
                request.cityId = "010";
                request.skus = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.PriceQuerySkusReq>();
                foreach (var item in skuId)
                {
                    var sku = new snsdk.suning_api_sdk.Models.CustomGovbusModel.PriceQuerySkusReq();
                    sku.skuId = item;
                    request.skus.Add(sku);
                }
                var result = SuningClient.Execute(request);
                if (result.skus != null)
                {
                    foreach (var item in result.skus)
                    {
                        SNPriceDto price = new SNPriceDto();
                        price.skuId = item.skuId;
                        price.price = item.price;
                        price.snPrice = item.snPrice;
                        price.discountRate = item.discountRate;
                        list.Add(price);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetPrice 批量获取苏宁价格异常", ex);
            }

            return list;
        }
        /// <summary>
        /// suning.govbus.prodextend.get 查询商品扩展信息接口 
        /// 通过此接口可获取商品是否支持开增票及是否支持无理由退货
        /// 1、商品是否支持开增票，可通过接口“suning.govbus.prodextend.get”获取；
        /// 2、商品是否支持无理由退货，可通过接口“suning.govbus.prodextend.get”获取；
        /// </summary>
        /// <param name="skuId"></param>
        /// <returns></returns>
        public static List<SNComExtendDto> GetComExtend(List<SNPriceDto> prices)
        {
            List<SNComExtendDto> list = new List<SNComExtendDto>();
            try
            {
                LogHelper.Info("SuningSV.GetComExtend 批量获取苏宁商品是否支持开增票及是否支持无理由退货");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdextendGetRequest();
                request.skus = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq>();
                foreach (var item in prices)
                {
                    var price = new snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq();
                    price.price = item.price;
                    price.skuId = item.skuId;
                    request.skus.Add(price);
                }
                var result = SuningClient.Execute(request);
                if (result.resultInfo != null)
                {
                    foreach (var item in result.resultInfo)
                    {
                        var model = new SNComExtendDto();
                        model.increaseTicket = item.increaseTicket;
                        model.noReasonLimit = item.noReasonLimit;
                        model.noReasonTip = item.noReasonTip;
                        model.returnGoods = item.returnGoods;
                        model.skuId = item.skuId;
                        list.Add(model);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetComExtend 批量获取苏宁商品是否支持开增票及是否支持无理由退货异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.batchprodsalestatus.get 商品上下架状态查询接口
        /// </summary>
        /// <param name="skus"></param>
        /// <returns></returns>
        public static List<SNSkuStateDto> GetSkuState(List<string> skus)
        {
            List<SNSkuStateDto> list = new List<SNSkuStateDto>();
            try
            {
                LogHelper.Info("SuningSV.GetSkuState 批量获取苏宁商品上下架状态");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.BatchProdSaleStatusGetRequest();
                request.skuIds = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.BatchProdSaleStatusGetSkuIdsReq>();
                foreach (var item in skus)
                {
                    var state = new snsdk.suning_api_sdk.Models.CustomGovbusModel.BatchProdSaleStatusGetSkuIdsReq();
                    state.skuId = item;
                    request.skuIds.Add(state);
                }
                var result = SuningClient.Execute(request);
                if (result.onShelvesList != null)
                {
                    foreach (var item in result.onShelvesList)
                    {
                        var skuState = new SNSkuStateDto();
                        skuState.skuId = item.skuId;
                        skuState.state = item.state;
                        list.Add(skuState);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetSkuState 批量获取苏宁商品上下架状态异常", ex);
            }
            return list;
        }
        /// <summary>
        /// suning.govbus.pricemessage.query 价格变动消息查询接口 
        /// </summary>
        /// <returns></returns>
        public static List<SNPriceMessageDto> GetPriceMessage()
        {
            List<SNPriceMessageDto> list = new List<SNPriceMessageDto>();
            try
            {
                LogHelper.Info("SuningSV.GetPriceMessage 苏宁商品价格变动消息查询");
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.PricemessageQueryRequest();
                var result = SuningClient.Execute(request);
                if (result.result != null)
                {
                    foreach (var item in result.result)
                    {
                        var priceMsg = new SNPriceMessageDto();
                        priceMsg.cmmdtyCode = item.cmmdtyCode;
                        priceMsg.cityId = item.cityId;
                        priceMsg.time = Convert.ToDateTime(item.time);
                        list.Add(priceMsg);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetPriceMessage 苏宁商品价格变动消息查询异常", ex);
            }
            return list;
        }
    }
}
