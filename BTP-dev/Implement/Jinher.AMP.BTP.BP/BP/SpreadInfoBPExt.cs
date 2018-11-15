
/***************
功能描述: BTPBP
作    者: 
创建时间: 2015/8/19 13:47:18
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    ///  推广信息
    /// </summary>
    public partial class SpreadInfoBP : BaseBP, ISpreadInfo
    {

        /// <summary>
        /// 保存推广信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SaveSpreadInfoExt(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadSaveDTO dto)
        {
            if (dto == null || dto.SpreadAppId == Guid.Empty || dto.HotshopId == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };

            Guid userId = Guid.Empty;
            Guid iwId = Guid.Empty;

            // 一级代理
            if (dto.SpreadType == 5)
            {
                if (dto.SubSpreadCount < 0 || dto.SubSpreadCount > 999)
                {
                    return new ResultDTO { ResultCode = 1, Message = "子代理数量仅允许输入0~999" };
                }
                if (dto.IWCode == null) return new ResultDTO { ResultCode = 1, Message = "参数为空" };
                iwId = EBCSV.GetOrgIdByIwCode(dto.IWCode);
                if (iwId == Guid.Empty)
                    return new ResultDTO { ResultCode = 2, Message = "该组织不存在" };
                // 同一个推广主或推广组织，同一个推广App，同一个旺铺仅能生成一个推广码；已存在则提示：已存在相同的推广码；
                var spreadInfoCount = SpreadInfo.ObjectSet().Count(c => c.IsDel != 1 && c.SpreadType == 5 && c.IWId == iwId && c.SpreadAppId == dto.SpreadAppId && c.HotshopId == dto.HotshopId);
                if (spreadInfoCount > 0)
                {
                    return new ResultDTO { ResultCode = 2, Message = "已存在相同的推广码" };
                }
            }
            // 二级代理
            else if (dto.SpreadType == 6)
            {
                if (dto.UserCode == null || dto.IWId == Guid.Empty || string.IsNullOrEmpty(dto.IWCode)) return new ResultDTO { ResultCode = 1, Message = "参数为空" };
                iwId = dto.IWId;
                userId = CBCSV.GetUserIdByAccount(dto.UserCode);
                if (userId == Guid.Empty) return new ResultDTO { ResultCode = 2, Message = "该用户未注册" };

                // 同一个推广主，同一个推广App，同一个旺铺仅能生成一个推广码；已存在则提示：已存在相同的推广码；
                var spreadUserCount = SpreadInfo.ObjectSet().Count(c => c.IsDel != 1 && c.SpreadType == 6 && c.SpreadId == userId && c.SpreadAppId == dto.SpreadAppId && c.HotshopId == dto.HotshopId);
                if (spreadUserCount > 0)
                {
                    return new ResultDTO { ResultCode = 2, Message = "已存在相同的推广码" };
                }

                // 查询一级代理
                var lv1Spread = SpreadInfo.ObjectSet().FirstOrDefault(s => s.IsDel != 1 && s.SpreadType == 5 && s.IWId == iwId && s.SpreadAppId == dto.SpreadAppId && s.HotshopId == dto.HotshopId);
                if (lv1Spread == null)
                {
                    return new ResultDTO { ResultCode = 2, Message = "未创建一级代理" };
                }
                // 同一组织、同一个推广App，同一个旺铺的子代理数量限制
                var lv2SpreadCount = SpreadInfo.ObjectSet().Count(c => c.IsDel != 1 && c.SpreadType == 6 && c.IWId == iwId && c.SpreadAppId == dto.SpreadAppId && c.HotshopId == dto.HotshopId);
                if (lv2SpreadCount >= lv1Spread.SubSpreadCount)
                {
                    return new ResultDTO { ResultCode = 2, Message = "生成的推广码已经达到限制数量" };
                }
            }
            else
            {
                if (dto.UserCode == null) return new ResultDTO { ResultCode = 1, Message = "参数为空" };
                userId = CBCSV.GetUserIdByAccount(dto.UserCode);
                if (userId == Guid.Empty)
                    return new ResultDTO { ResultCode = 2, Message = "该用户未注册" };
            }

            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                Guid spreadCode = Guid.NewGuid();
                string spreadUrl = getSpreadUrl(dto.SpreadAppId, dto.HotshopId, spreadCode);
                var qrCodeUrl = BaseAppToolsSV.GenerateQrCode(spreadUrl, dto.QrCodeFileImg);
                if (string.IsNullOrEmpty(qrCodeUrl))
                {
                    return new ResultDTO { ResultCode = 2, Message = "生成二维码失败" };
                }
                SpreadInfo spreadInfo = new SpreadInfo
                {
                    Id = Guid.NewGuid(),
                    SpreadId = userId,
                    SpreadUrl = ShortUrlSV.Instance.GenShortUrl(spreadUrl),
                    SpreadCode = spreadCode,
                    SpreadDesc = dto.SpreadDesc,
                    SpreadType = dto.SpreadType,
                    QrCodeUrl = qrCodeUrl,
                    SpreadAppId = dto.SpreadAppId,
                    HotshopId = dto.HotshopId,
                    UserCode = dto.UserCode,
                    Name = dto.Name ?? "",
                    IWId = iwId,
                    IWCode = dto.IWCode,
                    SubSpreadCount = dto.SubSpreadCount,
                    DividendPercent = dto.DividendPercent,
                    EntityState = EntityState.Added
                };
                contextSession.SaveObject(spreadInfo);

                updateRalationUserSpread(contextSession, spreadInfo);

                contextSession.SaveChanges();

                return new ResultDTO { isSuccess = true, ResultCode = 0, Message = "Success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadInfoBP.SaveSpreadInfoExt异常，dto={0}", JsonHelper.JsonSerializer(dto)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }

        public ResultDTO<ListResult<SpreadInfoShowDTO>> GetSpreadInfoListExt(SpreadSearchDTO search)
        {
            if (search == null)
                return new ResultDTO<ListResult<SpreadInfoShowDTO>> { ResultCode = 1, Message = "参数为空" };
            if (search.PageIndex == 0)
                search.PageIndex = 1;
            if (search.PageSize == 0)
                search.PageSize = 20;
            var query = SpreadInfo.ObjectSet().Where(c => c.IsDel != 1);
            if (search.SpreadType.HasValue)
            {
                query = query.Where(c => c.SpreadType == search.SpreadType.Value);
            }
            if (!string.IsNullOrEmpty(search.UserCode))
            {
                query = query.Where(c => c.UserCode.Contains(search.UserCode));
            }
            if (search.SpreadAppId.HasValue && search.SpreadAppId != Guid.Empty)
            {
                query = query.Where(c => c.SpreadAppId == search.SpreadAppId.Value);
            }
            if (search.IWId.HasValue)
            {
                query = query.Where(c => c.IWId == search.IWId.Value);
            }
            ListResult<SpreadInfoShowDTO> data = new ListResult<SpreadInfoShowDTO> { List = new List<SpreadInfoShowDTO>() };
            data.Count = query.Count();
            data.List = query.OrderByDescending(c => c.SubTime).Skip((search.PageIndex - 1) * search.PageSize).
                              Take(search.PageSize).Select(c => new SpreadInfoShowDTO
                                  {
                                      Id = c.Id,
                                      SpreadId = c.SpreadId,
                                      Account = c.UserCode,
                                      Name = c.Name,
                                      SpreadType = c.SpreadType,
                                      SpreadAppId = c.SpreadAppId,
                                      HotshopId = c.HotshopId,
                                      QrCodeUrl = c.QrCodeUrl,
                                      SpreadDesc = c.SpreadDesc,
                                      SpreadUrl = c.SpreadUrl,
                                      SubTime = c.SubTime,
                                      IsDel = c.IsDel,
                                      HotshopName = "",
                                      IsBindWeChatQrCode = false,
                                      SpreadAppName = "",
                                      SpreadTypeDesc = "",
                                      IWCode = c.IWCode,
                                      SubSpreadCount = c.SubSpreadCount,
                                      DividendPercent = c.DividendPercent

                                  }).ToList();
            if (data.List.Any())
            {
                var spreadTypes = data.List.Select(c => c.SpreadType).Distinct().ToList();
                var spreadCategoryList = SpreadCategory.ObjectSet()
                               .Where(c => spreadTypes.Contains(c.SpreadType))
                               .Select(m => new SpreadCategoryDTO { SpreadType = m.SpreadType, CategoryDesc = m.CategoryDesc }).ToList();

                var ids = data.List.Select(c => c.Id).ToList();
                var bindedIds = WeChatQRCode.ObjectSet().Where(c => ids.Contains(c.SpreadInfoId)).Select(m => m.SpreadInfoId).ToList();


                var appIds = data.List.Select(c => c.SpreadAppId).Distinct().ToList();
                appIds.AddRange(data.List.Select(c => c.HotshopId));
                appIds = appIds.Distinct().ToList();
                var appDict = APPSV.GetAppNameListByIds(appIds);
                foreach (var spreadInfoShowDTO in data.List)
                {
                    if (appDict.ContainsKey(spreadInfoShowDTO.SpreadAppId))
                    {
                        spreadInfoShowDTO.SpreadAppName = appDict[spreadInfoShowDTO.SpreadAppId];
                    }
                    if (appDict.ContainsKey(spreadInfoShowDTO.HotshopId))
                    {
                        spreadInfoShowDTO.HotshopName = appDict[spreadInfoShowDTO.HotshopId];
                    }
                    var spreadTypeDto = spreadCategoryList.FirstOrDefault(c => c.SpreadType == spreadInfoShowDTO.SpreadType);
                    if (spreadTypeDto != null)
                    {
                        spreadInfoShowDTO.SpreadTypeDesc = spreadTypeDto.CategoryDesc;
                    }
                    if (bindedIds.Contains(spreadInfoShowDTO.Id))
                    {
                        spreadInfoShowDTO.IsBindWeChatQrCode = true;
                    }
                }
            }

            return new ResultDTO<ListResult<SpreadInfoShowDTO>>
                {
                    isSuccess = true,
                    Message = "success",
                    Data = data
                };
        }

        public ResultDTO BindWeChatQrCodeExt(SpreadBindWeChatQrCodeDTO search)
        {
            if (search == null || search.Id == Guid.Empty || search.AppId == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var spreadInfo = SpreadInfo.ObjectSet().FirstOrDefault(c => c.IsDel != 1 && c.Id == search.Id);
                if (spreadInfo == null)
                    return new ResultDTO { ResultCode = 2, Message = "推广主不存在" };
                var weChatQrCode = WeChatQRCode.ObjectSet().Where(c => c.AppId == search.AppId && c.IsDel != 1 && !c.IsUse && c.QRType.Value == search.QRType && c.QRNo == search.QRNo).FirstOrDefault();
                if (weChatQrCode == null)
                    return new ResultDTO { ResultCode = 3, Message = "没有找到可使用的带参数二维码，请到“App后台-微信公众号”中生成二维码" };
                weChatQrCode.SpreadInfoId = search.Id;
                weChatQrCode.IsUse = true;
                weChatQrCode.Name = getQrCodeName(spreadInfo);
                weChatQrCode.ModifiedOn = DateTime.Now;
                weChatQrCode.EntityState = EntityState.Modified;
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadInfoBP.BindWeChatQrCodeExt异常，dto={0}", JsonHelper.JsonSerializer(search)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }
        private string getQrCodeName(SpreadInfo spreadInfo)
        {
            string result = string.Empty;
            if (spreadInfo == null)
                return result;
            List<Guid> appIds = new List<Guid>();
            if (spreadInfo.SpreadAppId != Guid.Empty)
                appIds.Add(spreadInfo.SpreadAppId);
            if (spreadInfo.HotshopId != Guid.Empty)
                appIds.Add(spreadInfo.HotshopId);
            var appDict = APPSV.GetAppNameListByIds(appIds);
            return SpreadSV.Instance.BuildQrCodeName(spreadInfo, appDict);
        }

        public ResultDTO UpdateStateExt(SpreadUpdateStateDTO search)
        {
            if (search == null || search.Id == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var spreadInfo = SpreadInfo.ObjectSet().FirstOrDefault(c => c.IsDel != 1 && c.Id == search.Id);
                if (spreadInfo == null)
                    return new ResultDTO { ResultCode = 2, Message = "推广主不存在" };

                spreadInfo.IsDel = search.IsDel ? 2 : 0;
                spreadInfo.EntityState = EntityState.Modified;
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadInfoBP.UpdateStateExt异常，search={0}", JsonHelper.JsonSerializer(search)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }

        private string getSpreadUrl(Guid appId, Guid hotShopId, Guid spreadCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(CustomConfig.H5HomePage, appId));
            sb.Append(string.Format("&speader={0}", spreadCode));
            if (hotShopId != Guid.Empty)
            {
                sb.Append(string.Format("&sai={0}", hotShopId));
            }
            return sb.ToString();
        }
        private void updateRalationUserSpread(ContextSession contextSession, SpreadInfo spreadInfo)
        {
            var createOrgUserId = EBCSV.GetOrgCreateUser(spreadInfo.SpreadId);

            var userSpreader = UserSpreader.ObjectSet().FirstOrDefault(c => c.UserId == createOrgUserId);
            if (userSpreader == null)
            {
                UserSpreader uSpreaderNew = UserSpreader.CreateUserSpreader();
                uSpreaderNew.UserId = createOrgUserId;
                uSpreaderNew.SpreaderId = spreadInfo.SpreadId;
                uSpreaderNew.SpreadCode = spreadInfo.SpreadCode;
                uSpreaderNew.CreateOrderId = new Guid("00000000-0000-0000-0000-000000000000");
                contextSession.SaveObject(uSpreaderNew);
            }
        }

        /// <summary>
        /// 修改子代理数量
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateSubCountExt(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadUpdateSubSpreadCountDTO dto)
        {
            if (dto == null || dto.Id == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            if (dto.Count < 0 || dto.Count > 999)
            {
                return new ResultDTO { ResultCode = 1, Message = "子代理数量仅允许输入0~999" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var spreadInfo = SpreadInfo.ObjectSet().FirstOrDefault(c => c.IsDel != 1 && c.Id == dto.Id);
                if (spreadInfo == null)
                    return new ResultDTO { ResultCode = 2, Message = "推广主不存在" };

                spreadInfo.SubSpreadCount = dto.Count;
                spreadInfo.EntityState = EntityState.Modified;
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadInfoBP.UpdateSubCountExt异常，search={0}", JsonHelper.JsonSerializer(dto)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }

        /// <summary>
        /// 修改总代分佣比例
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateDividendPercentExt(Jinher.AMP.BTP.Deploy.CustomDTO.SpreadUpdateDividendPercentDTO dto)
        {
            if (dto == null || dto.Id == Guid.Empty)
                return new ResultDTO { ResultCode = 1, Message = "参数为空" };
            if (dto.Percent < 0 || dto.Percent > 100)
            {
                return new ResultDTO { ResultCode = 1, Message = "分佣比例只能在0-100之间" };
            }
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                var spreadInfo = SpreadInfo.ObjectSet().FirstOrDefault(c => c.IsDel != 1 && c.Id == dto.Id);
                if (spreadInfo == null)
                    return new ResultDTO { ResultCode = 2, Message = "推广主不存在" };

                spreadInfo.DividendPercent = dto.Percent;
                spreadInfo.EntityState = EntityState.Modified;
                contextSession.SaveChanges();
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("SpreadInfoBP.UpdateDividendPercentExt异常，search={0}", JsonHelper.JsonSerializer(dto)), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
        }


        /// <summary>
        /// 查询一级代理推广App列表
        /// </summary>
        /// <param name="iwId">组织ID</param>
        /// <returns></returns>
        public ResultDTO<List<SpreadAppDTO>> GetLv1SpreadAppsExt(Guid iwId)
        {
            var data = new List<SpreadAppDTO>();
            var appIds = SpreadInfo.ObjectSet().
                Where(s => s.IsDel != 1 && s.SpreadType == 5 && s.IWId == iwId).
                Select(s => s.SpreadAppId).ToList();
            if (appIds.Count > 0)
            {
                var appDict = APPSV.GetAppNameListByIds(appIds);
                foreach (var item in appIds)
                {
                    data.Add(new SpreadAppDTO { Id = item, Name = appDict.ContainsKey(item) ? appDict[item] : null });
                }
            }
            return new ResultDTO<List<SpreadAppDTO>>
            {
                isSuccess = true,
                Message = "success",
                Data = data
            };
        }

        /// <summary>
        /// 查询一级代理指定APP的旺铺列表
        /// </summary>
        /// <param name="iwId">组织ID</param>
        /// <param name="appId">应用ID</param>
        /// <returns></returns>
        public ResultDTO<List<SpreadAppDTO>> GetLv1SpreadHotshopsExt(Guid iwId, Guid appId)
        {
            var data = new List<SpreadAppDTO>();
            var appIds = SpreadInfo.ObjectSet().
                Where(s => s.IsDel != 1 && s.SpreadType == 5 && s.IWId == iwId && s.SpreadAppId == appId).
                Select(s => s.HotshopId).ToList();
            if (appIds.Count > 0)
            {
                var appDict = APPSV.GetAppNameListByIds(appIds);
                foreach (var item in appIds)
                {
                    data.Add(new SpreadAppDTO { Id = item, Name = appDict.ContainsKey(item) ? appDict[item] : null });
                }
            }
            return new ResultDTO<List<SpreadAppDTO>>
            {
                isSuccess = true,
                Message = "success",
                Data = data
            };
        }
    }
}