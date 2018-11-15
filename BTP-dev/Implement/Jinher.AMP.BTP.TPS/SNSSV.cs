using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.SNS.Deploy.CustomDTO;
using Jinher.AMP.SNS.Deploy.Enum;
using Jinher.JAP.Common.Loging;
using Jinher.AMP.SNS.Deploy;
using EvaluationDTO = Jinher.AMP.BTP.Deploy.CustomDTO.EvaluationDTO;
using ScoreDTO = Jinher.AMP.BTP.Deploy.CustomDTO.ScoreDTO;

namespace Jinher.AMP.BTP.TPS
{
    /*
     * 请注意：！！！！！！！！！！！！！！！
     * 外部引用请将代码写到TPS.XXXFacade中，自定义扩展请写到TPS.XXXSV、TPS.XXXBP中
     */

    public class SNSSV : OutSideServiceBase<SNSSVFacade>
    {
        /// <summary>
        /// 获取商品评价
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="commodityId"></param>
        /// <returns></returns>
        public static BtpScoreDTO GetComFirstScore(Guid appId, Guid commodityId)
        {
            string lastRecordTime = "9946704778000";
            return toBtpScoreDto(Instance.GetScoreList(appId, commodityId, 5, lastRecordTime, Pager.New, EvaluateType.All));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private static BtpScoreDTO toBtpScoreDto(CSoreNewDTO dto)
        {
            BtpScoreDTO result = new BtpScoreDTO();
            if (dto != null)
            {
                if (dto.LastRecordTime > DateTime.MinValue)
                    result.LastRecordTime = dto.LastRecordTime;
                result.Score = dto.Score;
                result.TotalCount = dto.TotalCount;

                if (dto.Evaluate != null)
                {
                    result.Evaluate = new BTP.Deploy.CustomDTO.EvaluationDTO()
                    {
                        BadCount = dto.Evaluate.BadCount,
                        MediumCount = dto.Evaluate.MediumCount,
                        GoodCount = dto.Evaluate.GoodCount
                    };
                }
                if (dto.Records != null && dto.Records.Any())
                {
                    foreach (var scoreDTO in dto.Records)
                    {
                        result.Records.Add(new ScoreDTO()
                        {
                            Anonymous = scoreDTO.Anonymous,
                            Content = scoreDTO.Content,
                            Id = scoreDTO.Id,
                            PhotosArr = scoreDTO.PhotosArr,
                            ShowTime = scoreDTO.ShowTime,
                            UserId = scoreDTO.UserId,
                            UserName = scoreDTO.UserName,
                            Icon = scoreDTO.Icon
                        });
                    }
                }
            }
            return result;
        }
    }

    public class SNSSVFacade : OutSideFacadeBase
    {
        /// <summary>
        /// SNSSV接口
        /// </summary>
        /// <param name="userList"></param>
        /// <param name="message"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<bool> PushSysMessageToUsers(List<string> userList, string message, string domain)
        {
            ReturnInfoDTO<bool> returnDTO = null;
            try
            {
                Jinher.AMP.SNS.IBP.Facade.MessageQueryFacade messAge = new SNS.IBP.Facade.MessageQueryFacade();
                messAge.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                returnDTO = messAge.PushSysMessageToUsers(userList, message, domain);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNSSV.PushSysMessageToUsers服务异常:获取应用信息异常。 userList：{0},message:{1},domain{2}", userList, message, domain), ex);
            }
            return returnDTO;
        }
        [BTPAopLogMethod]
        public ReturnInfoDTO<List<AppSceneUserApiDTO>> GetSceneUserInfo(Guid appId, Guid userId)
        {
            ReturnInfoDTO<List<AppSceneUserApiDTO>> appDTO = null;
            try
            {
                Jinher.AMP.SNS.ISV.Facade.AppSceneUserQueryFacade sceneUser = new SNS.ISV.Facade.AppSceneUserQueryFacade();
                sceneUser.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                appDTO = sceneUser.GetSceneUserInfo(appId, userId);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNSSV.GetSceneUserInfo服务异常:获取应用信息异常。 appId：{0},userId:{1}", appId, userId), ex);
            }
            return appDTO;
        }
        [BTPAopLogMethod]
        public ReturnInfoDTO<Guid> GetShareUserId(string key)
        {
            ReturnInfoDTO<Guid> reDTO = null;
            try
            {
                Jinher.AMP.SNS.ISV.Facade.ShareFacade shareFacade = new SNS.ISV.Facade.ShareFacade();
                shareFacade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                reDTO = shareFacade.GetShareUserId(key);

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNSSV.GetShareUserId服务异常:获取应用信息异常。 key：{0}", key), ex);
            }
            return reDTO;
        }

        ///// <summary>
        ///// 获取商品的评价
        ///// </summary>
        ///// <param name="appId"></param>
        ///// <param name="productId"></param>
        ///// <returns></returns>
        //public static int GetBusinessInfo(Guid appId,Guid productId)
        //{
        //    try
        //    {
        //        Jinher.AMP.SNS.IBP.Facade.ScoreFacade scoreFacade = new SNS.IBP.Facade.ScoreFacade();
        //        ReturnInfoDTO<SocreBusinessInfoDTO> sbiResult = scoreFacade.GetBusinessInfo(appId, productId);
        //        int rcount = (int)sbiResult.Data;
        //        return rcount;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error("SNSSV.GetBusinessInfo异常，异常信息：",ex);
        //    }
        //    return 0;
        //}

        [BTPAopLogMethod]
        public ReturnInfoDTO<List<DistrictDTO>> GetAllDistrict(Guid appId)
        {
            ReturnInfoDTO<List<DistrictDTO>> reDTO = null;
            try
            {
                Jinher.AMP.SNS.ISV.Facade.AppDistrictQueryFacade facade = new SNS.ISV.Facade.AppDistrictQueryFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                reDTO = facade.GetAllDistrict(appId);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNSSV.GetAllDistrict服务异常:获取LBS管理设置的区域。 appId：{0}", appId), ex);
            }
            return reDTO;
        }
        /// <summary>
        /// 获取商品评价列表
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="productId"></param>
        /// <param name="pageSize"></param>
        /// <param name="lastRecordTime"></param>
        /// <param name="pager"></param>
        /// <param name="evaluateType"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public CSoreNewDTO GetScoreList(Guid appId, Guid productId, int pageSize, string lastRecordTime, Jinher.AMP.SNS.Deploy.Enum.Pager pager, Jinher.AMP.SNS.Deploy.Enum.EvaluateType evaluateType)
        {
            CSoreNewDTO reDTO = null;
            try
            {
                Jinher.AMP.SNS.IBP.Facade.ScoreFacade facade = new SNS.IBP.Facade.ScoreFacade();
                facade.ContextDTO = AuthorizeHelper.InitAuthorizeInfo();
                var result = facade.GetScoreNewList(appId, productId, pageSize, lastRecordTime, pager, evaluateType);
                if (result != null && result.Code == "0")
                {
                    reDTO = result.Content;
                }

            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNSSV.GetScoreList服务异常:获取商品评价列表。 appId：{0},productId：{1},pageSize：{2},lastRecordTime：{3},pager：{4},evaluateType：{5}", appId, productId, pageSize, lastRecordTime, pager, evaluateType), ex);
            }
            return reDTO;
        }
        /// <summary>
        /// 获取订单项的是否评论数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ReturnInfoDTO<List<string>> GetScoreStatusSByUser(string userid)
        {
            ReturnInfoDTO<List<string>> reDTO = null;
            try
            {
                Jinher.AMP.SNS.IBP.Facade.ScoreFacade sc = new SNS.IBP.Facade.ScoreFacade();
                var result = sc.getScoreStatusSByUser(userid);
                if (result != null && result.Code == "0")
                {
                    reDTO.Content = result.Content;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SNSSV.GetScoreStatusSByUser服务异常:获取订单项的是否评论数据。 userid：{0}", userid), ex);
            }
            return reDTO;
        }
    }
}
