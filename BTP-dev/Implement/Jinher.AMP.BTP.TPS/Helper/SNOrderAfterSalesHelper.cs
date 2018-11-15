using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO.AfterSales;
using System.Web.Script.Serialization;
using System.IO;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.Enum;
using System.Data.Objects;
using System.Data;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.TPS.Helper
{
    /// <summary>
    /// 苏宁售后帮助类
    /// </summary>
    public class SNOrderAfterSalesHelper
    {
        /// <summary>
        /// 苏宁客服电话
        /// </summary>
        public const string SuNingCustomerServiceTelephone = "4008516516";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="order"></param>
        /// <param name="orderItem"></param>
        /// <param name="refund"></param>
        /// <param name="refundDTO"></param>
        /// <returns></returns>
        public static ResultDTO SNSubmitRefund(ContextSession contextSession, CommodityOrder order, OrderItem orderItem, OrderRefundAfterSales refund, SubmitOrderRefundDTO refundDTO)
        {

            //if (address == null)
            //{
            //    return new ResultDTO { isSuccess = false, Message = "取件地址不能为空。" };
            //}
            var snOrderItem = SNOrderItem.ObjectSet()
                .Where(_ => _.OrderItemId == refundDTO.OrderItemId).FirstOrDefault();
            if (snOrderItem == null)
            {
                return new ResultDTO { isSuccess = false, ResultCode = 1, Message = "该订单不是苏宁订单。" };
            }
            SNReturnPartOrderDTO retDto = new SNReturnPartOrderDTO
            {
                OrderId = snOrderItem.CustomOrderId,
                SkusList = new List<SNReturnPartOrderAddSkusDTO>() { new SNReturnPartOrderAddSkusDTO() { SkuId = snOrderItem.CustomSkuId, Num = orderItem.Number.ToString() } }
            };

            ResultDTO result = new ResultDTO { isSuccess = false, ResultCode = 1, Message = "退货失败" };
            SNFactoryDeliveryEnum fac = GetOrderFactoryDeliveryType(orderItem.CommodityOrderId, snOrderItem.CustomSkuId);
            if (fac == SNFactoryDeliveryEnum.NonFactoryDelivery)
            {

                result = SuningSV.ReturnPartOrder(retDto); ;
                if (result.isSuccess)
                {
                    // 保存到 SNOrderRefundAfterSales
                    SNOrderRefundAfterSales snOrderRefundAfterSales = new SNOrderRefundAfterSales
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        AppId = order.AppId,
                        OrderRefundAfterSalesId = refund.Id,
                        OrderId = orderItem.CommodityOrderId,
                        OrderItemId = orderItem.Id,
                        CustomOrderId = snOrderItem.CustomOrderId,
                        CommodityId = orderItem.CommodityId,
                        CommodityNum = orderItem.Number,
                        CustomSkuId = snOrderItem.CustomSkuId,
                        Cancel = 1,
                        //取件方式(必填 1 上门取件-非厂送-自营    2快递寄回-厂送 )
                        PickwareType = fac.GetHashCode(),

                        CustomerContactName = refundDTO.Address.customerContactName,
                        CustomerTel = refundDTO.Address.customerTel,
                        PickwareAddress = refundDTO.Address.ProviceCityStr + refundDTO.Address.pickwareAddress,
                        //暂时没用
                        AfsServiceId = "",
                        AfsServiceStep = 10,
                        AfsServiceStepName = "申请阶段",

                        EntityState = EntityState.Added
                    };

                    contextSession.SaveObject(snOrderRefundAfterSales);

                    UpdateApplyOrderStatus(contextSession, 3, orderItem.Id);
                }
                else
                {
                    UpdateApplyOrderStatus(contextSession, 4, orderItem.Id);
                    var log = new Deploy.JdlogsDTO
                    {
                        Id = Guid.NewGuid(),
                        SubTime = DateTime.Now,
                        ThirdECommerceType = ThirdECommerceTypeEnum.SuNingYiGou.GetHashCode(),
                        Content = "【苏宁售后】----【" + order.Code + "】中的" + orderItem.Name + "商品【" + snOrderItem.CustomSkuId + "】，提交售后申请失败，失败原因：【" + result.ResultCode + ":" + result.Message + "】"
                    };
                    JdlogsFacade facade = new JdlogsFacade();
                    facade.SaveJdlogs(log);
                }
            }
            else
            {
                // 保存到 SNOrderRefundAfterSales
                SNOrderRefundAfterSales snOrderRefundAfterSales = new SNOrderRefundAfterSales
                {
                    Id = Guid.NewGuid(),
                    SubTime = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    AppId = order.AppId,
                    OrderRefundAfterSalesId = refund.Id,
                    OrderId = orderItem.CommodityOrderId,
                    OrderItemId = orderItem.Id,
                    CustomOrderId = snOrderItem.CustomOrderId,
                    CommodityId = orderItem.CommodityId,
                    CommodityNum = orderItem.Number,
                    CustomSkuId = snOrderItem.CustomSkuId,
                    Cancel = 1,
                    //取件方式(必填 1 上门取件-非厂送-自营    2快递寄回-厂送 )
                    PickwareType = fac.GetHashCode(),

                    CustomerContactName = refundDTO.Address.customerContactName,
                    CustomerTel = refundDTO.Address.customerTel,
                    PickwareAddress = refundDTO.Address.ProviceCityStr + refundDTO.Address.pickwareAddress,
                    //暂时没用
                    AfsServiceId = "",
                    AfsServiceStep = 10,
                    AfsServiceStepName = "申请阶段",

                    EntityState = EntityState.Added
                };

                contextSession.SaveObject(snOrderRefundAfterSales);

                UpdateApplyOrderStatus(contextSession, 3, orderItem.Id);
                result = new ResultDTO { isSuccess = true, ResultCode = 0, Message = "退款申请成功" };
            }

            return result;
        }
        /// <summary>
        /// 更新订单售后状态
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="state">0未退款1退款中2已退款3售后-退货达成协议4售后-拒绝退款申请5售后-拒绝收货</param>
        private static void UpdateApplyOrderStatus(ContextSession contextSession, int state, Guid orderItemId)
        {
            try
            {
                var orderItems = OrderItem.FindByID(orderItemId);
                //如果是厂送，更新订单表的状态为  售后达成协议

                orderItems.State = state;
                orderItems.ModifiedOn = DateTime.Now;
                orderItems.EntityState = EntityState.Modified;

                contextSession.SaveObject(orderItems);
            }
            catch (Exception ex)
            {
                LogHelper.Error("SNOrderAfterSalesHelper.UpdateApplyOrderStatus 【苏宁-售后】更新订单售后状态", ex);
            }
        }

        /// <summary>
        /// 获取指定订单的Sku的退货状态(厂送-非厂送)
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <param name="skuId">skuId</param>
        /// <returns>取件方式(必填3异常   1 上门取件-非厂送-自营    2快递寄回-厂送 )</returns>
        private static SNFactoryDeliveryEnum GetOrderFactoryDeliveryType(Guid orderId, string skuId)
        {
            try
            {
                var judgeIsFactoryDelivery = SNJudgeIsFactoryDeliveryByOrderId(orderId);
                if (judgeIsFactoryDelivery != null && judgeIsFactoryDelivery.Any())
                {
                    SNFactoryDeliveryReturnListDTO listDto = judgeIsFactoryDelivery.Where(p => p.SkuId.Equals(skuId)).FirstOrDefault();
                    return (listDto.IsFactorySend ? SNFactoryDeliveryEnum.FactoryDelivery : SNFactoryDeliveryEnum.NonFactoryDelivery);
                }
                return SNFactoryDeliveryEnum.Error;

            }
            catch (Exception ex)
            {
                LogHelper.Error("SNOrderAfterSalesHelper.GetOrderFactoryDeliveryType 【苏宁-售后】退货状态(厂送-非厂送)", ex);
                return SNFactoryDeliveryEnum.Error;
            }
        }



        /// <summary>
        /// 根据订单Id  获取苏宁所有商品  判断是否是厂送
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns>判断是否为null即可</returns>
        public static List<SNFactoryDeliveryReturnListDTO> SNJudgeIsFactoryDeliveryByOrderId(Guid orderId)
        {
            List<SNFactoryDeliveryReturnListDTO> retDto = new List<SNFactoryDeliveryReturnListDTO>();
            //获取主订单
            ISV.Facade.CommodityOrderFacade commodityOrderFacade = new ISV.Facade.CommodityOrderFacade();
            var orders = commodityOrderFacade.GetCommodityOrder(new List<Guid>() { orderId }).FirstOrDefault();

            if (orders != null)
            {
                List<SNApplyRejectedSkusDTO> skuDto = new List<SNApplyRejectedSkusDTO>();
                //获取苏宁子订单
                SNAfterSaleFacade an = new SNAfterSaleFacade();
                List<SNOrderItemDTO> listItem = an.GetSNOrderItemList(new SNOrderItemDTO() { OrderId = orderId });
                listItem.ForEach(p =>
                {
                    skuDto.Add(new SNApplyRejectedSkusDTO() { SkuId = p.CustomSkuId });
                });
                //如果存在子订单
                if (skuDto.Any())
                {
                    retDto = SNJudgeIsFactoryDelivery(orders.Province, orders.City, skuDto);
                }
            }
            return retDto;

        }


        /// <summary>
        /// 判断是否是厂送
        /// </summary>
        /// <param name="provinceName">省名称</param>
        /// <param name="cityName">城市名称</param>
        /// <param name="skuIds">9位或者11位商品编码</param>
        /// <returns></returns>
        public static List<SNFactoryDeliveryReturnListDTO> SNJudgeIsFactoryDelivery(string provinceName, string cityName, List<SNApplyRejectedSkusDTO> skuIds)
        {
            SNAreaDTO areaDto = GetCityId(provinceName, cityName);
            if (areaDto != null)
            {
                SNFactoryDeliveryDTO dto = new SNFactoryDeliveryDTO
                {
                    CityId = areaDto.A,
                    SkuIds = skuIds
                };

                SNFactoryDeliveryReturnDTO ty = SuningSV.JudgeIsFactoryDelivery(dto);
                if (ty != null && ty.IsSuccess)
                {
                    return ty.ResultsList;
                }
            }
            return new List<SNFactoryDeliveryReturnListDTO>();
        }


        public static List<SNAreaDTO> GetCityId()
        {
            //通过省市名称 查询出城市的编码
            string strFileName = System.AppDomain.CurrentDomain.BaseDirectory + @"dist\lib\AreaJson.json";

            string filetxt = ReadData(strFileName);

            return JsonDeserialize<List<SNAreaDTO>>(filetxt);
        }
        /// <summary>
        /// 根据省市名称，获取城市编码
        /// </summary>
        /// <param name="provinceName">省</param>
        /// <param name="cityName">市</param>
        /// <returns></returns>
        private static SNAreaDTO GetCityId(string provinceName, string cityName)
        {

            try
            {
                //通过省市名称 查询出城市的编码
                string strFileName = System.AppDomain.CurrentDomain.BaseDirectory + @"dist\lib\AreaJson.json";

                string filetxt = ReadData(strFileName);

                List<SNAreaDTO> listArea = JsonDeserialize<List<SNAreaDTO>>(filetxt);
                //获取省
                SNAreaDTO listSheng = listArea.Where(
                    p => p.L.Trim().Equals("1")
                        && p.N.Trim().Equals(provinceName.Trim())).FirstOrDefault();

                if (listSheng != null)
                {
                    //查询市
                    SNAreaDTO listShi = listArea.Where(p => p.L.Trim().Equals("2")
                        && p.N.Trim().Equals(cityName.Trim())
                        && p.P.Trim().Equals(listSheng.A.Trim())).FirstOrDefault();
                    return listShi;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(@"SNOrderAfterSalesHelper.GetCityId 【苏宁-售后】dist\lib\AreaJson.json 文件不存在 ", ex);
            }
            return null;
        }





        /// <summary>
        /// json反序列化（非二进制方式）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private static T JsonDeserialize<T>(string jsonString)
        {
            JavaScriptSerializer jsonSerialize = new JavaScriptSerializer
            {
                MaxJsonLength = Int32.MaxValue
            };
            return (T)jsonSerialize.Deserialize<T>(jsonString);
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string ReadData(string path)
        {
            //文件在那里,对文件如何 处理,对文件内容采取的处理方式     
            try
            {
                System.Text.Encoding code = System.Text.Encoding.GetEncoding("gb2312");
                return ReadFile(path, code);
            }
            catch (Exception)
            {
                System.Text.Encoding code = System.Text.Encoding.UTF8;
                return ReadFile(path, code);
            }
        }
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string ReadFile(string path, System.Text.Encoding code)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                //仅 对文本 执行  读写操作     
                using (StreamReader sr = new StreamReader(fs, code))
                {
                    //定位操作点,begin 是一个参考点     
                    sr.BaseStream.Seek(0, SeekOrigin.Begin);
                    //读一下，看看文件内有没有内容，为下一步循环 提供判断依据     
                    //sr.ReadLine() 这里是 StreamReader的要领  可不是 console 中的~      
                    //假如  文件有内容     
                    return sr.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// 获取苏宁订单售后状态状态
        /// </summary>
        /// <param name="refund"></param>
        /// <param name="orderId"></param>
        /// <param name="orderItemId"></param>
        public static void GetSNRefundInfo(SubmitOrderRefundDTO refund, Guid orderId, Guid orderItemId)
        {
            try
            {
                var snOrderRefundAfterSales = SNOrderRefundAfterSales.ObjectSet().Where(_ => _.OrderRefundAfterSalesId == refund.Id).FirstOrDefault();
                if (snOrderRefundAfterSales != null)
                {
                    refund.SnOrderRefundInfo = new SNOrderRefundDto
                    {
                        ServiceId = snOrderRefundAfterSales.AfsServiceId,
                        Cancel = snOrderRefundAfterSales.Cancel,
                        CustomerContactName = snOrderRefundAfterSales.CustomerContactName,
                        CustomerTel = snOrderRefundAfterSales.CustomerTel,
                        PickwareAddress = snOrderRefundAfterSales.PickwareAddress,
                        PickwareType = snOrderRefundAfterSales.PickwareType
                    };
                }
                //判断是否是苏宁订单  防止售后表没数据，造成退款详情页异常
                SNOrderItemDTO snModel = new SNOrderItemDTO
                {
                    OrderId = orderId,
                    OrderItemId = orderItemId
                };
                var snOrderItemList = new SNAfterSaleFacade().GetSNOrderItemList(snModel).ToList();
                if (snOrderItemList.Count() > 0)
                {
                    refund.IsSNOrder = true;
                }


            }
            catch (Exception ex)
            {
                LogHelper.Error(@"SNOrderAfterSalesHelper.GetSNRefundInfo 【苏宁-售后】获取苏宁订单售后状态状态  失败 ", ex);
            }
        }
    }
}
