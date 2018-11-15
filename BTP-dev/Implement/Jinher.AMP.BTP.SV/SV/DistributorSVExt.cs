
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/1/26 14:13:29
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using System.Net;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class DistributorSV : BaseSv, IDistributor
    {

        /// <summary>
        /// 保存分销商关系
        /// </summary>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Guid> SaveDistributorRelationExt(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distribDto)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Guid> result = new Deploy.CustomDTO.ResultDTO<Guid>();
            try
            {
                if (distribDto == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                if (distribDto.UserId == Guid.Empty
                    || distribDto.EsAppId == Guid.Empty)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                if (!Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(distribDto.EsAppId))
                {
                    result.ResultCode = 3;
                    result.Message = "抱歉，暂不支持该功能";
                    return result;
                }

                Distributor disParent = null;
                //DistributorId为空，当前用户将成为馆的直接分销商（一级分销商）.
                if (distribDto.DistributorId == Guid.Empty)
                {
                    disParent = new Distributor();
                    disParent.Id = Guid.Empty;
                    disParent.Level = 0;
                    disParent.Key = "";
                }
                else
                {
                    disParent = (from dis in Distributor.ObjectSet()
                                 where dis.Id == distribDto.DistributorId
                                 select dis).FirstOrDefault();
                    if (disParent == null)
                    {
                        result.ResultCode = 2;
                        result.Message = "分销商信息未找到！";
                        return result;

                    }
                    if (disParent.EsAppId != distribDto.EsAppId)
                    {
                        result.ResultCode = 4;
                        result.Message = "请求错误，请检查后重试~";
                        return result;
                    }
                }


                Guid disQuery = (from dis in Distributor.ObjectSet()
                                 where dis.EsAppId == distribDto.EsAppId && dis.UserId == distribDto.UserId
                                 select dis.Id).FirstOrDefault();
                if (disQuery != null && disQuery != Guid.Empty)
                {
                    result.Data = disQuery;
                    result.ResultCode = 0;
                    result.Message = "用户已存在分销关系！";
                    return result;
                }

                var uniDto = Jinher.AMP.BTP.TPS.CBCSV.GetUserNameAndCode(distribDto.UserId);
                ContextSession contextSession = ContextFactory.CurrentThreadContext;

                Distributor distributor = Distributor.CreateDistributor();
                distributor.Id = Guid.NewGuid();
                distributor.UserName = uniDto.Item1;
                distributor.UserCode = uniDto.Item2;
                distributor.SubTime = DateTime.Now;
                distributor.ModifiedOn = DateTime.Now;
                distributor.EsAppId = distribDto.EsAppId;
                distributor.ParentId = disParent.Id;
                distributor.Level = disParent.Level + 1;
                distributor.Key = (disParent.Key + "." + distributor.Id).Trim('.');
                distributor.UserId = distribDto.UserId;
                distributor.UserSubTime = DateTime.Now;
                distributor.PicturePath = "";
                distributor.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(distributor);

                //DistributorProfits distributorProfits = new DistributorProfits();
                //distributorProfits.Id = distributor.Id;
                //distributorProfits.UserName = distributor.UserName;
                //distributorProfits.UserCode = distributor.UserCode;
                //distributorProfits.SubTime = DateTime.Now;
                //distributorProfits.ModifiedOn = DateTime.Now;
                //distributorProfits.OrderAmount = 0;
                //distributorProfits.CommissionAmount = 0;
                //distributorProfits.CommmissionUnPay = 0;
                //distributorProfits.UnderlingCount = 0;
                //distributorProfits.SubUnderlingCount = 0;
                //distributorProfits.EntityState = System.Data.EntityState.Added;

                //contextSession.SaveObject(distributorProfits);

                contextSession.SaveChanges();

                result.Data = distributor.Id;
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("SaveDistributorRelationExt异常，异常信息：{0}", ex);
                LogHelper.Error(errMsg);
            }

            return result;
        }

        /// <summary>
        /// 查询分销统计信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsResultDTO GetDistributorProfitsExt(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsSearchDTO search)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributorProfitsResultDTO result = new Deploy.CustomDTO.DistributorProfitsResultDTO();

            if (search == null)
            {
                return result;
            }
            //查询等级。0：本人；1：一级；2：二级
            if (search.SearchType == 0)
            {
                var getResult = DSSBP.Instance.GetDistributorProfits(search);
                if (getResult != null && getResult.ResultCode == 200 && getResult.Data != null)
                {
                    result = getResult.Data;
                }
                if (result.DistributorProfitsInfoList == null || !result.DistributorProfitsInfoList.Any())
                {
                    result.DistributorProfitsInfoList = new List<DistributorProfitsInfoDTO>();
                    result.DistributorProfitsInfoList.Add(new DistributorProfitsInfoDTO());
                }
                //待收益佣金、已收益佣金改为btp实时计算
                //本人只返回一条数据，这里只处理第一条数据
                result.DistributorProfitsInfoList.First().CommmissionUnPay =
                     (from orderShare in OrderShare.ObjectSet()
                      join order in CommodityOrder.ObjectSet() on orderShare.OrderId equals order.Id
                      join os in CommodityOrderService.ObjectSet() on orderShare.OrderId equals os.Id into tmp
                      from orderService in tmp.DefaultIfEmpty()
                      where new[] { 9, 10, 11 }.Contains(orderShare.PayeeType)
                      &&
                      (
                         new[] { 1, 2, 8, 9, 10, 12, 13, 14 }.Contains(order.State)
                         ||
                          (
                          order.State == 3 && new[] { 3, 5, 10, 12, 13 }.Contains(orderService.State)
                          )
                      )
                      &&
                      orderShare.PayeeId == search.UserId
                      &&
                      order.AppId == search.AppId
                      select orderShare.Commission).ToList().Sum();

                result.DistributorProfitsInfoList.First().CommissionAmount =
                    (from orderShare in OrderShare.ObjectSet()
                     join order in CommodityOrder.ObjectSet() on orderShare.OrderId equals order.Id
                     join os in CommodityOrderService.ObjectSet() on orderShare.OrderId equals os.Id into tmp
                     from orderService in tmp.DefaultIfEmpty()
                     where new[] { 9, 10, 11 }.Contains(orderShare.PayeeType) && order.State == 3
                     &&
                     (
                        orderService.Id == null
                        ||
                        orderService.State == 15
                     )
                     &&
                     orderShare.PayeeId == search.UserId
                     &&
                     order.AppId == search.AppId
                     select orderShare.Commission).ToList().Sum();
            }
            else if (search.SearchType == 1)
            {
                var getResult = DSSBP.Instance.GetDistributorList(search);
                if (getResult != null && getResult.ResultCode == 200 && getResult.Data != null)
                {
                    result = getResult.Data;
                }
            }
            else if (search.SearchType == 2)
            {
                var getResult = DSSBP.Instance.GetDistributorList(search);
                if (getResult != null && getResult.ResultCode == 200 && getResult.Data != null)
                {
                    result = getResult.Data;
                }
            }

            return result;
        }

        /// <summary>
        ///更新分销商的用户信息。
        /// </summary>
        /// <param name="uinfo">用户信息</param>
        /// <returns>操作结果</returns>
        public ResultDTO UpdateDistributorUserInfoExt(UserSDTO uinfo)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                if (uinfo == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数错误，参数不能为空！";
                    return result;
                }
                if (uinfo.UserId == Guid.Empty)
                {
                    result.ResultCode = 2;
                    result.Message = "参数错误，参数UserId不能为空！";
                    return result;
                }

                var distrQuery = (from d in Distributor.ObjectSet()
                                  where d.UserId == uinfo.UserId
                                  select d).ToList();
                if (distrQuery.Any())
                {
                    result.ResultCode = 0;
                    result.Message = "未找到当前用户相关分销商！";
                    return result;
                }
                foreach (Distributor d in distrQuery)
                {
                    d.UserName = uinfo.UserName;
                    d.UserCode = uinfo.LoginAccount;
                    d.PicturePath = uinfo.PicUrl;
                }
                ContextFactory.CurrentThreadContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("UpdateDistributorUserInfoExt异常，异常信息：{0}", ex);
                LogHelper.Error(errMsg);

                result.ResultCode = -1;
                result.Message = "服务异常，请稍后重试！";
            }
            return result;
        }
        /// <summary>
        /// 判断用户是否为三级分销商
        /// </summary>
        /// <param name="cuinfo"></param>
        /// <returns></returns>
        public ResultDTO CheckDistributorUserInfoExt(UserSDTO cuinfo)
        {
            ResultDTO result = new ResultDTO();
            try
            {
                if (cuinfo == null)
                {
                    result.ResultCode = 1;
                    result.Message = "参数不能为空！";
                    return result;
                }
                if (cuinfo.UserId == Guid.Empty)
                {
                    result.ResultCode = 2;
                    result.Message = "用户ID不能为空！";
                    return result;
                }
                bool capp = Jinher.AMP.BTP.TPS.BACBP.CheckAppDistribute(cuinfo.AppId);
                if (capp)
                {
                    var cdistor = Distributor.ObjectSet().Where(c => c.UserId == cuinfo.UserId && c.EsAppId == cuinfo.AppId).ToList();
                    if (cdistor.Count > 0)
                    {
                        result.ResultCode = 0;
                        result.Message = "该用户是三级分销商";
                        return result;
                    }
                    result.ResultCode = 3;
                    result.Message = "该用户不是三级分销商";
                    return result;
                }
                result.ResultCode = 4;
                result.Message = "该应用未选用三级分销功能";
                return result;
            }
            catch (Exception ex)
            {
                string errMsg = string.Format("UpdateDistributorUserInfoExt异常，异常信息：{0}", ex);
                LogHelper.Error(errMsg);
                result.ResultCode = -1;
                result.Message = "服务异常，请稍后重试！";
            }
            return result;
        }

        /// <summary>
        /// 获取分销商信息
        /// </summary>
        /// <param name="distributorUserRelationDTO"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorInfoDTO GetDistributorInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.DistributorUserRelationDTO distributorUserRelationDTO)
        {
            if (distributorUserRelationDTO == null)
            {
                return null;
            }
            if (distributorUserRelationDTO.UserId == Guid.Empty
                || distributorUserRelationDTO.EsAppId == Guid.Empty)
            {
                return null;
            }
            var distributor = (from dis in Distributor.ObjectSet()
                               where dis.UserId == distributorUserRelationDTO.UserId && dis.EsAppId == distributorUserRelationDTO.EsAppId
                               select dis).FirstOrDefault();

            if (distributor == null)
            {
                return null;
            }
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributorInfoDTO result = new DistributorInfoDTO()
            {
                Id = distributor.Id,
                UserName = distributor.UserName,
                UserCode = distributor.UserCode,
                SubTime = distributor.SubTime,
                ModifiedOn = distributor.ModifiedOn,
                EsAppId = distributor.EsAppId,
                ParentId = distributor.ParentId,
                Level = distributor.Level,
                Key = distributor.Key,
                UserId = distributor.UserId,
                UserSubTime = distributor.UserSubTime,
                PicturePath = distributor.PicturePath,
            };
            return result;
        }

        /// <summary>
        /// 获取分销商佣金入账信息
        /// </summary>
        /// <param name="distributeMoneySearch"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.DistributorMoneyResultDTO GetDistributorMoneyInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.DistributeMoneySearch distributeMoneySearch)
        {
            Jinher.AMP.BTP.Deploy.CustomDTO.DistributorMoneyResultDTO result = new DistributorMoneyResultDTO();

            if (distributeMoneySearch == null || distributeMoneySearch.DistributorId == Guid.Empty || distributeMoneySearch.PageIndex < 0 || distributeMoneySearch.PageSize < 1)
            {
                return result;
            }
            var payeeType = new List<int>() { 9, 10, 11 };
            var orderStates = new List<int>() { 1, 2, 8, 9, 10, 12, 13, 14 };
            var orderAfterStates = new List<int>() { 3, 5, 10, 12, 13 };

            var distributorId = distributeMoneySearch.DistributorId.ToString();
            //佣金累计 查已收益佣金
            if (distributeMoneySearch.SearchType == 1)
            {
                var query = from os in OrderShare.ObjectSet()
                            join orderService in CommodityOrderService.ObjectSet()
                                on os.OrderId equals orderService.Id
                            where
                                payeeType.Contains(os.PayeeType) &&
                                os.ShareKey == distributorId && os.Commission > 0 &&
                                orderService.State == 15
                            select new DistributorMoneyDTO
                                {
                                    DistributorId = distributeMoneySearch.DistributorId,
                                    SortTime = orderService.EndTime.Value,
                                    Money = os.Commission,
                                    State = 0
                                };
                result.Count = query.Count();
                result.DistributorMoneyList = query.OrderByDescending(n => n.SortTime).Skip((distributeMoneySearch.PageIndex - 1) * distributeMoneySearch.PageSize).Take(distributeMoneySearch.PageSize).ToList();

            }
            //待收益
            else if (distributeMoneySearch.SearchType == 2)
            {
                var query = from os in OrderShare.ObjectSet()
                            join order in CommodityOrder.ObjectSet()
                                on os.OrderId equals order.Id
                            join dataS in CommodityOrderService.ObjectSet()
                                on os.OrderId equals dataS.Id
                                into tempS
                            from orderService in tempS.DefaultIfEmpty()
                            where
                                payeeType.Contains(os.PayeeType) &&
                                os.ShareKey == distributorId && os.Commission > 0 &&
                                (orderStates.Contains(order.State) ||
                                 (order.State == 3 && orderService.State != null && orderAfterStates.Contains(orderService.State)))
                            select new DistributorMoneyDTO
                                {
                                    DistributorId = distributeMoneySearch.DistributorId,
                                    SortTime = order.PaymentTime.Value,
                                    Money = os.Commission,
                                    State = 1
                                };
                result.Count = query.Count();
                result.DistributorMoneyList = query.OrderByDescending(n => n.SortTime).Skip((distributeMoneySearch.PageIndex - 1) * distributeMoneySearch.PageSize).Take(distributeMoneySearch.PageSize).ToList();

            }
            else
            {
                //
            }
            return result;
        }
        /// <summary>
        /// 分销信息校验
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public UserDistributionCheckResultDTO UserDistributionCheckExt(DistributionSearchDTO search)
        {
            throw new NotImplementedException();
        }

        private DistributionIdentitySetFullDTO GetApplySetExt(DistributionSearchDTO search)
        {
            throw new NotImplementedException();
        }

        private DistributApplyFullDTO GetApplyExt(DistributionSearchDTO search)
        {
            throw new NotImplementedException();
        }

        private ResultDTO SaveApplyExt(DistributApplyFullDTO dto)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 同步正式环境历史数据使用 勿调用
        /// </summary>
        /// <returns></returns>
        private ResultDTO SaveMicroshopExt()
        {
            var result = new ResultDTO { isSuccess = false };
            var query = from d in Distributor.ObjectSet() select d;
            foreach (var distributor in query)
            {
                var isE = Microshop.ObjectSet().FirstOrDefault(t => t.Key == distributor.Id);
                if (isE != null && isE.QRCodeUrl == "")
                {
                    var id = Guid.NewGuid();
                    MicroshopTypeVO mv = new MicroshopTypeVO() { Value = 1 };
                    var url = BTP.Common.CustomConfig.BtpDomain + string.Format("/Distribute/MicroshopIndex?appid={0}&microshopId={1}&distributorId={2}",
                        distributor.EsAppId, id, distributor.Id) + "&type=tuwen&source=share&SrcType=34&isshowsharebenefitbtn=1";

                    List<Guid> users = new List<Guid> { distributor.UserId };
                    var user = CBCSV.Instance.GetUserInfoWithAccountList(users)[0];
                    var logo = user.HeadIcon;
                    var userName = user.Name;

                    var qRCodeUrl = CreateQrCode(logo, url);
                    var ms = new Microshop
                    {
                        Id = id,
                        SubTime = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        AppId = distributor.EsAppId,
                        UserId = distributor.UserId,
                        Logo = logo,
                        Name = userName + "的小店",
                        Url = url,
                        Type = mv,
                        Key = distributor.Id,
                        QRCodeUrl = qRCodeUrl,
                        EntityState = EntityState.Added
                    };
                    ContextFactory.CurrentThreadContext.SaveObject(ms);
                }
            }

            int msCount = ContextFactory.CurrentThreadContext.SaveChanges();
            if (msCount > 0)
            {
                result.isSuccess = true;
                result.Message = "添加成功";
            }
            else
            {
                result.Message = "添加失败";
            }
            return result;
        }

        private string CreateQrCode(string fileImg, string replaceUrl)
        {
            string qrCode = "";
            try
            {
                //网络图片读取
                WebClient mywebclient = new WebClient();
                var imgfile = mywebclient.DownloadData(fileImg);

                var qRCodeWithIconDto = new Jinher.JAP.BaseApp.Tools.Deploy.CustomDTO.QRCodeWithIconDTO
                {
                    IconDate = imgfile,
                    Source = replaceUrl
                };

                //生成带图片的二维码
                var codepath = TPS.BaseAppToolsSV.Instance.GenQRCodeWithIcon(qRCodeWithIconDto);
                qrCode = CustomConfig.CommonFileServerUrl + codepath;
            }
            catch (Exception ex)
            {
                //string errStack = ex.Message + ex.StackTrace;
                //while (ex.InnerException != null)
                //{
                //    errStack += ex.InnerException.Message + ex.InnerException.StackTrace;
                //    ex = ex.InnerException;
                //}
                //LogHelper.Error("CreateQrCode异常，异常信息：" + errStack, ex);
                //系统默认生成的不带图片的二维码                    
                string codepath = BaseAppToolsSV.Instance.GenQRCode(replaceUrl);
                qrCode = CustomConfig.CommonFileServerUrl + codepath;
            }
            return qrCode;
        }
    }
}