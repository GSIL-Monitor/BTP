extern alias snsdk;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;

namespace Jinher.AMP.BTP.TPS
{

    public partial class SuningSV
    {

        /// <summary>
        /// 部分商品退货
        /// http://open.suning.com/ospos/apipage/toApiMethodDetailMenu.do?interCode=suning.govbus.returnpartorder.add
        /// </summary>
        /// <param name="reqDto">实体数据</param>
        /// <returns></returns>
        public static ResultDTO ReturnPartOrder(SNReturnPartOrderDTO reqDto)
        {
            ResultDTO ret = new ResultDTO();

            try
            {
                #region 将系统中自有的实体转换为苏宁自己的实体传入
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ReturnpartorderAddSkusReq> skusReqList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ReturnpartorderAddSkusReq>();

                reqDto.SkusList.ForEach(p =>
                {
                    skusReqList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.ReturnpartorderAddSkusReq() { skuId = p.SkuId, num = p.Num });
                });

                snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ReturnpartorderAddRequest request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ReturnpartorderAddRequest()
                {
                    orderId = reqDto.OrderId,
                    skus = skusReqList
                };
                #endregion


                snsdk.suning_api_sdk.BizResponse.CustomGovbusResponse.ReturnpartorderAddResponse response = SuningClient.Execute(request);
                #region 构造返回实体
                if (response != null)
                {

                    if (response.isSuccess.Equals("Y"))
                    {
                        return new ResultDTO { isSuccess = true };
                    }
                    else
                    {
                        return new ResultDTO { isSuccess = false, ResultCode = 1, Message =response.infoList.First().unableReason };
                    }

                    //List<SNReturnPartOrderReturnListDTO> listRet = new List<SNReturnPartOrderReturnListDTO>();

                    //response.infoList.ForEach(p =>
                    //{
                    //    listRet.Add(new SNReturnPartOrderReturnListDTO() { SkuId = p.skuId, Status = p.status, UnableReason = p.unableReason });
                    //});
                    //ret = new SNReturnPartOrderReturnDTO() { OrderId = response.orderId, InfoList = listRet, IsSuccess = response.isSuccess.Equals("Y") ? true : false };
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.ReturnPartOrder 部分商品退货", ex);
            }
            return ret;
        }


        /// <summary>
        /// 整单退货接口申请
        /// http://open.suning.com/ospos/apipage/toApiMethodDetailMenu.do?interCode=suning.govbus.applyrejected.add
        /// </summary>
        /// <param name="reqDto"></param>
        /// <returns></returns>
        public static SNApplyRejectedReturnDTO ApplyRejected(SNApplyRejectedDTO reqDto)
        {
            SNApplyRejectedReturnDTO ret = new SNApplyRejectedReturnDTO();
            try
            {

                #region 将系统中自有的实体转换为苏宁自己的实体传入
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ApplyRejectedAddSkusReq> listApplyRejList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ApplyRejectedAddSkusReq>();

                reqDto.SkusList.ForEach(p =>
                {
                    listApplyRejList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.ApplyRejectedAddSkusReq() { skuId = p.SkuId });
                });
                snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ApplyRejectedAddRequest request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ApplyRejectedAddRequest()
                {
                    orderId = reqDto.OrderId,
                    skus = listApplyRejList
                };

                #endregion

                snsdk.suning_api_sdk.BizResponse.CustomGovbusResponse.ApplyRejectedAddResponse response = SuningClient.Execute(request);

                #region 构造返回实体
                if (response != null)
                {
                    List<SNApplyRejectedReturnListDTO> listRet = new List<SNApplyRejectedReturnListDTO>();

                    response.infoList.ForEach(p =>
                    {
                        listRet.Add(new SNApplyRejectedReturnListDTO() { SkuId = p.skuId, Status = p.status, UnableReason = p.unableReason });
                    });

                    ret = new SNApplyRejectedReturnDTO() { OrderId = response.orderId, InfoList = listRet };
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.ApplyRejected 整单退货接口申请", ex);
            }
            return ret;
        }


        /// <summary>
        /// 判断商品是否厂送
        /// https://open.suning.com/ospos/apipage/toApiMethodDetailMenu.do?interCode=suning.govbus.judgefacproduct.get
        /// </summary>
        /// <param name="factoryDeliverDto"></param>
        /// <returns></returns>
        public static SNFactoryDeliveryReturnDTO JudgeIsFactoryDelivery(SNFactoryDeliveryDTO factoryDeliverDto)
        {
            SNFactoryDeliveryReturnDTO ret = new SNFactoryDeliveryReturnDTO();

            try
            {


                #region 将系统中自有的实体转换为苏宁自己的实体传入
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.JudgefacproductGetSkuIdsReq> listApplyRejList = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.JudgefacproductGetSkuIdsReq>();

                factoryDeliverDto.SkuIds.ForEach(p =>
                {
                    listApplyRejList.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.JudgefacproductGetSkuIdsReq() { skuId = p.SkuId });
                });

                snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.JudgefacproductGetRequest request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.JudgefacproductGetRequest()
                {
                    cityId = factoryDeliverDto.CityId,
                    skuIds = listApplyRejList
                };
                #endregion
                snsdk.suning_api_sdk.BizResponse.CustomGovbusResponse.JudgefacproductGetResponse response = SuningClient.Execute(request);

                #region 构造返回实体

                if (response != null)
                {

                    List<SNFactoryDeliveryReturnListDTO> listRet = new List<SNFactoryDeliveryReturnListDTO>();

                    response.results.ForEach(p =>
                    {
                        listRet.Add(new SNFactoryDeliveryReturnListDTO() { IsFactorySend = p.isFactorySend.Equals("01") ? true : false, SkuId = p.skuId });
                    });

                    ret = new SNFactoryDeliveryReturnDTO() { IsSuccess = response.isSuccess.Equals("Y") ? true : false, ResultsList = listRet };
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.JudgeIsFactoryDelivery 判断商品是否厂送", ex);
            }
            return ret;
        }
        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="snOrderId"></param>
        /// <returns></returns>
        public static SNOrderStatusDTO SNGetOrderStatus(string snOrderId)
        {
            SNOrderStatusDTO orderstatus = new SNOrderStatusDTO();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderStatusGetRequest()
                {
                    orderId = snOrderId
                };
                snsdk.suning_api_sdk.BizResponse.CustomGovbusResponse.OrderStatusGetResponse response = SuningClient.Execute(request);
                if (response != null)
                {
                    List<SNOrderItemInfo> snOrderItemInfoList = new List<SNOrderItemInfo>();
                    response.orderItemInfoList.ForEach(p =>
                    {
                        snOrderItemInfoList.Add(new SNOrderItemInfo()
                        {
                            OrderItemId = p.orderItemId,
                            SkuId = p.skuId,
                            StatusName = p.statusName
                        });
                    });
                    orderstatus = new SNOrderStatusDTO() { OrderId = response.orderId, OrderItemInfoList = snOrderItemInfoList, OrderStatus = response.orderStatus };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.SNGetOrderStatus 获取订单状态", ex);
            }
            return orderstatus;
        }

        /// <summary>
        /// 判断商品是否支持退款
        /// </summary>
        /// <param name="orderServerDto"></param>
        public static List<SNGetOrderServiceReturnDTO> SNJudgeOrderServiceType(List<SNGetOrderServiceDTO> orderServerDto)
        {
            List<SNGetOrderServiceReturnDTO> retDto = new List<SNGetOrderServiceReturnDTO>();

            try
            {
                List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq> list = new List<snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq>();
                orderServerDto.ForEach(p =>
                {
                    list.Add(new snsdk.suning_api_sdk.Models.CustomGovbusModel.ProdextendGetSkusReq()
                    {
                        price = p.Price,
                        skuId = p.SkuId
                    });
                });

                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.ProdextendGetRequest()
                {
                    skus = list
                };
                var response = SuningClient.Execute(request);
                if (response != null)
                {
                    if (response.isSuccess.Equals("Y"))
                    {
                        response.resultInfo.ForEach(p =>
                        {
                            retDto.Add(new SNGetOrderServiceReturnDTO()
                            {
                                ReturnGoods = p.returnGoods,
                                SkuId = p.skuId,
                                IncreaseTicket = p.increaseTicket,
                                NoReasonTip = p.noReasonTip,
                                NoReasonLimit = p.noReasonLimit
                            });
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.SNJudgeOrderServiceType 判断商品是否支持退款", ex);
            }
            return retDto;
        }

        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="snOrderId"></param>
        /// <returns></returns>
        public static SNAfterOrderDetailDTO SNGetOrderDetailById(string snOrderId)
        {
            SNAfterOrderDetailDTO retDto = null;
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderDetailGetRequest()
                {
                    orderId = snOrderId
                };

                var response = SuningClient.Execute(request);
                if (response != null)
                {
                    List<SNAfterOrderDetailListDTO> orderDetail = new List<SNAfterOrderDetailListDTO>();

                    response.orderItemList.ForEach(p =>
                    {
                        orderDetail.Add(new SNAfterOrderDetailListDTO()
                        {
                            /// <summary>
                            /// 商品的品牌名称
                            /// </summary>
                            BrandName = p.brandName,
                            /// <summary>
                            /// 商品编码
                            /// </summary>
                            CommdtyCode = p.commdtyCode,
                            /// <summary>
                            /// 	商品名称
                            /// </summary>
                            CommdtyName = p.commdtyName,
                            /// <summary>
                            /// 希望送达时间（yyyy-MM-dd HH:mm:ss）
                            /// </summary>
                            HopeArriveTime = p.hopeArriveTime,
                            /// <summary>
                            /// 苏宁订单行号
                            /// </summary>
                            OrderItemId = p.orderItemId,
                            /// <summary>
                            /// 商品总金额=商品数量*商品单价（含运费分摊）
                            /// </summary>
                            SkuAmt = p.skuAmt,
                            /// <summary>
                            /// 商品的购买数量
                            /// </summary>
                            SkuNum = p.skuNum,
                            /// <summary>
                            /// 	商品单价
                            /// </summary>
                            UnitPrice = p.unitPrice
                        });
                    });

                    retDto = new SNAfterOrderDetailDTO()
                    {

                        AccountName = response.accountName,

                        CompanyName = response.companyName,

                        CreateTime = response.createTime,

                        OrderAmt = response.orderAmt,

                        OrderId = response.orderId,

                        OrderItemList = orderDetail,

                        ReceiverAddress = response.receiverAddress,

                        ReceiverTel = response.receiverTel
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.SNGetOrderDetailById 获取订单详情", ex);
            }
            return retDto;
        }


        /// <summary>
        /// 获取物流信息
        /// </summary>
        /// <param name="orderLogist"></param>
        /// <returns></returns>
        public static SNOrderLogistOutPutDTO SNGetOrderLogist(SNOrderLogistInputDTO orderLogist)
        {
            SNOrderLogistOutPutDTO retDto = new SNOrderLogistOutPutDTO();
            try
            {
                var request = new snsdk.suning_api_sdk.BizRequest.CustomGovbusRequest.OrderLogistGetRequest()
                {
                    orderId = orderLogist.OrderId,
                    orderItemId = orderLogist.OrderItemId,
                    skuId = orderLogist.SkuId
                };
                var response = SuningClient.Execute(request);
                if (response != null)
                {
                    List<SNOrderLogistStatusResDTO> OrderLogisticStatusList = new List<SNOrderLogistStatusResDTO>();

                    response.orderLogisticStatus.ForEach(p =>
                    {
                        OrderLogisticStatusList.Add(new SNOrderLogistStatusResDTO()
                        {
                            OperateState = p.operateState,
                            OperateTime = p.operateTime
                        });
                    });

                    retDto = new SNOrderLogistOutPutDTO()
                    {
                        OrderId = response.orderId,
                        OrderItemId = response.orderItemId,
                        SkuId = response.skuId,
                        ShippingTime = response.shippingTime,
                        ReceiveTime = response.receiveTime,
                        OrderLogisticStatus = OrderLogisticStatusList
                    };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("SuningSV.GetOrderLogist 获取物流信息", ex);
            }
            return retDto;

        }

    }
}
