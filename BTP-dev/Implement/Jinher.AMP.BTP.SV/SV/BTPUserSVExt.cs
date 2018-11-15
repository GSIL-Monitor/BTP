using System;
using System.Linq;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using Jinher.AMP.BTP.BE;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using System.Runtime.Serialization;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 用户信息接口类
    /// </summary>
    public partial class BTPUserSV : BaseSv, IBTPUser
    {
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDTO">用户信息DTO</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateUserExt(Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO userDTO)
        {
            try
            {
                //ContextSession contextSession = ContextFactory.CurrentThreadContext;
                ////CommodityUser com = CommodityUser.ObjectSet().Where(n => n.UserId == userDTO.UserId && n.AppId == userDTO.AppId).FirstOrDefault();
                //CommodityUser com = CommodityUser.ObjectSet().Where(n => n.UserId == userDTO.UserId).FirstOrDefault();
                //if (com == null)
                //{
                //    CommodityUser commodityuser = new CommodityUser();
                //    commodityuser.Id = Guid.NewGuid();
                //    commodityuser.EntityState = System.Data.EntityState.Added;
                //    commodityuser.UserName = userDTO.UserName;
                //    commodityuser.HeadPic = userDTO.PicUrl == null ? "" : userDTO.PicUrl;
                //    commodityuser.Sex = userDTO.Sex;
                //    commodityuser.Details = userDTO.Details == null ? "": userDTO.Details;
                //    commodityuser.SubTime = DateTime.Now;
                //    commodityuser.Name = "用户信息";
                //    commodityuser.SubId = userDTO.UserId;
                //    commodityuser.UserId = userDTO.UserId;
                //    commodityuser.AppId = userDTO.AppId;
                //    commodityuser.ModifiedOn = DateTime.Now;//修改时间
                //    contextSession.SaveObject(commodityuser);
                //    contextSession.SaveChanges();
                //}
                //else
                //{
                //    com.UserName = userDTO.UserName;
                //    com.HeadPic = userDTO.PicUrl;
                //    com.Sex = userDTO.Sex;
                //    com.Details = userDTO.Details;
                //    contextSession.SaveObject(com);
                //    contextSession.SaveChanges();
                //}
                CBC.Deploy.CustomDTO.ModifyDTO userModiDTO = new CBC.Deploy.CustomDTO.ModifyDTO();
                userModiDTO.ID = userDTO.UserId;
                userModiDTO.List = new System.Collections.Generic.List<CBC.Deploy.CustomDTO.KeyValueDTO>();
                userModiDTO.List.Add(new CBC.Deploy.CustomDTO.KeyValueDTO("Name", userDTO.UserName));
                userModiDTO.List.Add(new CBC.Deploy.CustomDTO.KeyValueDTO("HeaderIcon", userDTO.PicUrl));
                userModiDTO.List.Add(new CBC.Deploy.CustomDTO.KeyValueDTO("Gender", userDTO.Sex == 0 ? "男" : "女"));
                userModiDTO.List.Add(new CBC.Deploy.CustomDTO.KeyValueDTO("Description", userDTO.Details));

                CBC.Deploy.CustomDTO.ReturnInfoDTO resultDTO = CBCSV.Instance.UpdateUserInfoByID(userModiDTO);

                if (!resultDTO.IsSuccess)
                {
                    LogHelper.Error(string.Format("更新CBC用户服务异常。userDTO：{0}，错误消息：{1}", JsonHelper.JsonSerializer(userDTO), resultDTO.Message));
                    return new ResultDTO { ResultCode = 1, Message = "Error" };
                }
                else
                {
                    return new ResultDTO { ResultCode = 0, Message = "Success" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("更新CBC用户服务异常。userDTO：{0}", JsonHelper.JsonSerializer(userDTO)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }

        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="appId">APPID</param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.UserSDTO GetUserExt(System.Guid userId, System.Guid appId)
        {
            //CommodityUser commodityuser = CommodityUser.ObjectSet().Where(n => n.UserId ==userId).FirstOrDefault();
            //if (commodityuser == null) 
            //{
            //    return null;
            //}
            //UserSDTO userDTO = new UserSDTO();
            //userDTO.AppId = appId;
            //userDTO.UserId = userId;
            //userDTO.UserName = commodityuser.UserName;
            //userDTO.PicUrl = commodityuser.HeadPic;
            //userDTO.Sex = commodityuser.Sex;
            //userDTO.Details = commodityuser.Details;

            CBC.Deploy.CustomDTO.UserBasicInfoDTO commodityuser = CBCSV.Instance.GetUserBasicInfoNew(userId);
            UserSDTO userDTO = new UserSDTO();
            userDTO.UserName = "";
            userDTO.PicUrl = "";
            userDTO.Sex = 0;
            userDTO.Details = "";
            if (commodityuser == null)
            {
                return userDTO;
            }

            userDTO.AppId = appId;
            userDTO.UserId = userId;
            userDTO.UserName = string.IsNullOrEmpty(commodityuser.Name) ? "" : commodityuser.Name;
            userDTO.PicUrl = string.IsNullOrEmpty(commodityuser.HeadIcon) ? "" : commodityuser.HeadIcon;
            userDTO.Sex = commodityuser.Gender == "男" ? 0 : 1;
            userDTO.Details = string.IsNullOrEmpty(commodityuser.Description) ? "" : commodityuser.Description;
            userDTO.LoginAccount = "";

            return userDTO;
        }

        /// <summary>
        /// 待自提订单数量
        /// </summary>
        /// <param name="userId">自提点管理员</param>
        /// <returns>待自提订单数量</returns>
        public ResultDTO<int> GetSelfTakeManagerExt(Guid userId)
        {
            try
            {
                // 返回 是否管理员，待自提订单数量
                if (userId == Guid.Empty)
                    return new ResultDTO<int> { Data = 0, ResultCode = 1, Message = "管理员用户ID非法." };
                var managerInfo = (from p in AppStsManager.ObjectSet()
                                   join r in AppSelfTakeStation.ObjectSet() on p.SelfTakeStationId equals r.Id
                                   where p.UserId == userId && p.IsDel == false && r.IsDel == false
                                   select p.SelfTakeStationId
                                    ).Distinct();
                if (!managerInfo.Any())
                {
                    LogHelper.Info(string.Format("该用户不是自提点管理员或没有与自提点绑定，userId：{0}", userId));
                    return new ResultDTO<int> { Data = 0, ResultCode = -1, Message = "抱歉，您暂时没有权限查看此信息" };
                }

                IQueryable<CommodityOrder> query = CommodityOrder.ObjectSet().Where(n => (n.State == 1 || n.State == 11) && n.IsDel != 1 && n.IsDel != 3);

                var commodityorderListCount = (from r in managerInfo
                                               join t in AppOrderPickUp.ObjectSet() on r equals t.SelfTakeStationId
                                               join p in query on t.Id equals p.Id
                                               select t.Id).Count();

                if (commodityorderListCount != 0)
                {
                    return new ResultDTO<int> { Data = commodityorderListCount, ResultCode = 0, Message = "sucess" };
                }
                else
                    return new ResultDTO<int> { Data = 0, ResultCode = -2, Message = "订单数量为0" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("BTPUserSV.GetSelfTakeManagerExt获取待自提订单数量异常。userId：{0}，ex:{1}", userId, ex));
                return new ResultDTO<int> { Data = 0, ResultCode = -3, Message = "Exception" };
            }
        }
        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VipPromotionDTO> GetVipInfoExt(Guid userId, Guid appId)
        {
            if (userId == Guid.Empty || appId == Guid.Empty)
            {
                return new ResultDTO<VipPromotionDTO> { ResultCode = 1, Message = "参数错误" };
            }
            var vipInfo = Jinher.AMP.BTP.TPS.AVMSV.GetVipIntensity(appId, userId);
            return new ResultDTO<VipPromotionDTO> { Data = vipInfo, ResultCode = 0, Message = "Success" };
        }
    }
}
