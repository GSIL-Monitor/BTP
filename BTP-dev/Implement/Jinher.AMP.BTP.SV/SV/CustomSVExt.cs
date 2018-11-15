
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/9/9 15:11:33
***************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.CBC.Deploy.CustomDTO;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.Common.Loging;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 客服服务
    /// </summary>
    public partial class CustomSV : BaseSv, ICustom
    {

        static List<string> allowIPs = new List<string>();        
        /// <summary>
        /// 根据userid获取用户信息
        /// </summary>
        public ResultDTO<UserInfoWithAccountDTO> GetCustomInfoExt(Guid userId)
        {
            LogHelper.Debug("进入客服服务，调用根据userid获取用户信息接口GetCustomInfo：userId：" + userId);

            ResultDTO<UserInfoWithAccountDTO> userInfo = new ResultDTO<UserInfoWithAccountDTO>() { isSuccess = false };


            try
            {
                var userInfos = CBCSV.Instance.GetUserInfoWithAccountList(new List<Guid> { userId });
                if (userInfos != null && userInfos.Count() > 0)
                {
                    userInfo.isSuccess = true;
                    userInfo.Message = "获取成功";
                    userInfo.Data = userInfos.First();
                    LogHelper.Debug("获取用户信息如下：" + JsonHelper.JsSerializer(userInfo));
                }
                else
                {
                    userInfo.isSuccess = false;
                    userInfo.Message = "获取失败";
                    userInfo.Data = null;
                }

            }
            catch (Exception ex)
            {
                userInfo.Message = "获取异常";
                LogHelper.Error("CustomSV.GetCustomInfoExt。根据userid获取用户信息。", ex);
            }
            return userInfo;
        }

        /// <summary>
        /// 根据commodityId获取商品信息
        /// </summary>
        public ResultDTO<CommodityInfoListDTO> GetCommodityInfoExt(Guid commodityId)
        {
            LogHelper.Debug("进入客服服务，调用根据commodityId获取商品信息接口GetCommodityInfo：commodityId：" + commodityId);

            ResultDTO<CommodityInfoListDTO> commodityInfo = new ResultDTO<CommodityInfoListDTO>() { isSuccess = false };

            try
            {
                CommodityInfoListDTO commoditySdto = new CommodityInfoListDTO();
                //获取商品实体对象
                Commodity com = Commodity.FindByID(commodityId);
                //商品基本信息
                if (com != null && com.Id != Guid.Empty)
                {
                    LogHelper.Debug("comID" + com.Id + "  " + "Guid.Empty:" + Guid.Empty);
                    LogHelper.Debug("比较结果" + (Guid.Empty == com.Id));
                    commoditySdto.CommodityId = com.Id;
                    commoditySdto.CommodityName = com.Name;
                    commoditySdto.Pic = com.PicturesPath;
                    commoditySdto.Price = com.Price;
                    commoditySdto.MarketPrice = com.MarketPrice;
                    commodityInfo.isSuccess = true;
                    commodityInfo.Message = "获取成功";
                    commodityInfo.Data = commoditySdto;
                }
                else
                {
                    commodityInfo.isSuccess = false;
                    commodityInfo.Message = "获取失败";
                    commodityInfo.Data = null;
                }

            }
            catch (Exception ex)
            {
                commodityInfo.Message = "获取异常";
                LogHelper.Error(string.Format("获取商品信息异常。commodityId：{0}", commodityId), ex);
            }
            return commodityInfo;
        }

        /// <summary>
        /// 根据orderId获取订单信息
        /// </summary>
        public ResultDTO<CommodityOrderDTO> GetCommodityOrderExt(Guid orderId)
        {
            LogHelper.Debug("进入客服服务，调用根据orderId获取订单信息接口GetCommodityOrder：orderId：" + orderId);
            ResultDTO<CommodityOrderDTO> orderInfo = new ResultDTO<CommodityOrderDTO>() { isSuccess = false };

            try
            {
                CommodityOrderDTO orderDto = new CommodityOrderDTO();
                //获取订单实体对象
                CommodityOrder order = CommodityOrder.FindByID(orderId);
                //订单基本信息
                if (order != null && order.Id != Guid.Empty)
                {
                    orderDto.Id = order.Id;
                    orderDto.Name = order.Name;
                    orderDto.Code = order.Code;
                    orderDto.SubId = order.SubId;
                    orderDto.SubTime = order.SubTime;
                    orderDto.UserId = order.UserId;
                    orderDto.AppId = order.AppId;
                    orderDto.State = order.State;
                    orderDto.PaymentTime = order.PaymentTime;
                    orderDto.ConfirmTime = order.ConfirmTime;
                    orderDto.ShipmentsTime = order.ShipmentsTime;
                    orderDto.ReceiptUserName = order.ReceiptUserName;
                    orderDto.ReceiptPhone = order.ReceiptPhone;
                    orderDto.ReceiptAddress = order.ReceiptAddress;
                    orderDto.Details = order.Details;
                    orderDto.Payment = order.Payment;
                    orderDto.MessageToBuyer = order.MessageToBuyer;
                    orderDto.Province = order.Province;
                    orderDto.City = order.City;
                    orderDto.District = order.District;
                    orderDto.Street = order.Street;
                    orderDto.IsModifiedPrice = order.IsModifiedPrice;
                    orderDto.ModifiedOn = order.ModifiedOn;
                    orderDto.ModifiedBy = order.ModifiedBy;
                    orderDto.ModifiedId = order.ModifiedId;
                    orderDto.PaymentState = order.PaymentState;
                    orderDto.Price = order.Price;
                    orderDto.RealPrice = order.RealPrice;
                    orderInfo.isSuccess = true;
                    orderInfo.Message = "获取成功";
                    orderInfo.Data = orderDto;
                }
                else
                {
                    orderInfo.isSuccess = false;
                    orderInfo.Message = "获取失败";
                    orderInfo.Data = null;
                }
            }
            catch (Exception ex)
            {
                orderInfo.Message = "获取异常";
                orderInfo.Data = null;
                LogHelper.Error(string.Format("获取订单信息异常。orderId：{0}", orderId), ex);
            }
            return orderInfo;
        }
        /// <summary>
        /// 商家的移动坐席数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.AppSceneUserDTO>> GetAppSceneContentExt(int pageIndex)
        {
            LogHelper.Debug("进入客服服务，调用根据appId获取商家的移动坐席数据：请求时间：" + DateTime.Now);
            ResultDTO<List<AppSceneUserDTO>> Result = new ResultDTO<List<AppSceneUserDTO>> { isSuccess = false, Message = "查询失败" };            
            try
            {
                Jinher.AMP.SNS.ISV.Facade.AppSceneUserQueryFacade Facade = new SNS.ISV.Facade.AppSceneUserQueryFacade();
                int pageSize = 100;
                var YJAppList = MallApply.ObjectSet().Where(p => p.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId && p.State.Value == 2).Select(s => s.AppId);//获取分页数据
                List<AppSceneUserDTO> AppSceneList = new List<AppSceneUserDTO>();

                foreach (var item in YJAppList)
                {
                    var AppSceneUser = Facade.GetAppSceneUser(item);
                    if (AppSceneUser.Code == "0" && AppSceneUser.Content.Any())
                    {
                        /*var AppScene = AppSceneUser.Content.Select(s => new AppSceneUserDTO
                        {
                            appId = item,
                            SceneId = s.SceneId,
                            SceneName = s.SceneName,
                            UserId = s.UserId,
                            UserName = s.UserName,
                            Account = GetCustomInfoExt(s.UserId).Data.Account,
                            Birthday = GetCustomInfoExt(s.UserId).Data.Birthday,
                            HeadIcon = GetCustomInfoExt(s.UserId).Data.HeadIcon
                        }).ToList();*/

                        var AppScene = new List<AppSceneUserDTO>();
                        foreach (var content in AppSceneUser.Content)
                        {
                            if (!string.IsNullOrEmpty(content.UserId.ToString()))
                            {
                                var user = GetCustomInfoExt(content.UserId);
                                var userdto = new AppSceneUserDTO
                                {
                                    appId = item,
                                    SceneId = content.SceneId,
                                    SceneName = content.SceneName,
                                    UserId = content.UserId,
                                    UserName = content.UserName,
                                    Account = user.Data.Account,
                                    Birthday = user.Data.Birthday,
                                    HeadIcon = user.Data.HeadIcon
                                };
                                AppScene.Add(userdto);
                            }
                            else
                            {
                                var userdto = new AppSceneUserDTO
                                {
                                    appId = item,
                                    SceneId = content.SceneId,
                                    SceneName = content.SceneName,
                                    UserId = content.UserId,
                                    UserName = content.UserName
                                };
                                AppScene.Add(userdto);
                            }
                        }
                        AppSceneList.AddRange(AppScene);
                    }
                }

                if (AppSceneList.Any())
                {
                    Result.Data = AppSceneList.Skip((pageIndex -1) * pageSize).Take(pageSize).ToList(); //返回分页数据
                    Result.isSuccess = true;
                    Result.Message = "获取成功";
                }
            }
            catch (Exception ex)
            {
                Result.Message = "获取异常";
                LogHelper.Error("CustomSV.GetAppSceneContentExt。根据appId获取商家的移动坐席数据:", ex);
            }
            return Result;
        }
        /// <summary>
        /// 获取易捷北京下所有店铺信息
        ///  </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<System.Collections.Generic.List<Jinher.AMP.BTP.Deploy.CustomDTO.YJBJAppInfo>> GetYJAppInfoExt(int pageIndex)
        {
            LogHelper.Debug("进入客服服务，获取易捷北京下所有店铺信息：请求时间：" + DateTime.Now);
            ResultDTO<List<YJBJAppInfo>> Result = new ResultDTO<List<YJBJAppInfo>> { isSuccess = false, Message = "获取失败" };
            try
            {
                int pageSize = 100;
                var YJAppInfo = MallApply.ObjectSet().Where(p => p.EsAppId == Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId).OrderBy(o=>o.SubTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                if (!YJAppInfo.Any())
                {
                    Result.Message = "未获取到任何数据";
                    return Result;
                }
                List<YJBJAppInfo> YJAppList = new List<YJBJAppInfo>();
                foreach (var item in YJAppInfo)
                {
                    YJBJAppInfo AppInfo = new YJBJAppInfo();
                    AppInfo.appId = item.AppId;
                    AppInfo.AppName = item.AppName;
                    if (item.State.Value == 2)
                    {
                        AppInfo.state = 0;
                    }
                    else
                    {
                        AppInfo.state = 1;
                    }
                    if (item.Type == 0)
                    {
                        AppInfo.type = "自营他配";
                    }
                    else if (item.Type == 1)
                    {
                        AppInfo.type = "第三方";
                    }
                    else if (item.Type == 2)
                    {
                        AppInfo.type = "自营自配自采";
                    }
                    else if (item.Type == 3)
                    {
                        AppInfo.type = "自营自配统采";
                    }
                    else
                    {
                        AppInfo.type = "未知类型";
                    }
                    YJAppList.Add(AppInfo);
                }
                Result.Data = YJAppList;
                Result.isSuccess = true;
                Result.Message = "获取成功";
            }
            catch (Exception ex)
            {
                Result.Message = "获取异常";
                LogHelper.Error("CustomSV.GetAppSceneContentExt。根据appId获取商家的移动坐席数据:", ex);
            }
            return Result;
        }
        
        public int GetNoInfoCountExt(string guid)
        {
            var request = RequestHelper.CreateRequest<ResultDTO, string>(new RequestDTO<string>
            {
                ServiceUrl = string.Format("{0}", "http://wechat.hollyuc.com:9000/foreignInterface/getWechatUnReadMsg?userId=" + guid),
                RequestData = SerializationHelper.JsonSerialize(new { userId = guid })
            });

            return request.ResultCode;
        }
    }
}