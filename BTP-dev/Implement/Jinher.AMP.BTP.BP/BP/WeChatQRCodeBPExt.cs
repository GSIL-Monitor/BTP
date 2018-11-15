
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/12/10 16:36:37
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.CustomDTO.WeChat;
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
    /// 
    /// </summary>
    public partial class WeChatQRCodeBP : BaseBP, IWeChatQRCode
    {

        /// <summary>
        /// 创建公众号带参二维码
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO CreateWeChatQRCodeExt(Jinher.AMP.BTP.Deploy.CustomDTO.CateringDTO.WeChatQRCodeDTO dto)
        {
            BTP.Deploy.CustomDTO.ResultDTO ret = new Deploy.CustomDTO.ResultDTO();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            BE.WeChatQRCode qrCode = new WeChatQRCode();
            qrCode.Id = dto.id == Guid.Empty ? Guid.NewGuid() : dto.id;
            qrCode.WeChatPublicCode = dto.weChatPublicCode;
            qrCode.WeChatAppId = dto.WeChatAppId;
            qrCode.WeChatSecret = dto.weChatSecret;
            qrCode.WeChatTicket = dto.WeChatTicket;
            qrCode.AppId = dto.appId;
            qrCode.StoreId = dto.StoreId;
            qrCode.QRNo = dto.qrNo;
            qrCode.QRType.Value = (int)dto.QrType;
            qrCode.IsDel = 0;
            qrCode.IsUse = false;
            qrCode.SpreadInfoId = Guid.Empty;
            qrCode.EntityState = System.Data.EntityState.Added;

            contextSession.SaveObject(qrCode);
            try
            {
                int changes = contextSession.SaveChanges();
                ret.isSuccess = changes > 0;
                ret.Message = "添加成功";
                JAP.Common.Loging.LogHelper.Info("CreateWeChatQRCode:添加成功");
            }
            catch (Exception ex)
            {
                ret.isSuccess = false;
                ret.Message = ex.Message;
                JAP.Common.Loging.LogHelper.Error(ex.Message, ex);
            }

            return ret;
        }

        /// <summary>
        /// 获取最大自增号
        /// </summary>
        /// <returns></returns>
        public int GetWeChatQRNoExt()
        {
            var maxQRNo = (from qrCode in BE.WeChatQRCode.ObjectSet() orderby qrCode.QRNo descending select new { QrNo = qrCode.QRNo }).FirstOrDefault();

            return maxQRNo == null ? 1 : (int)maxQRNo.QrNo + 1;
        }

        /// <summary>
        /// 添加微信菜单
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="menuJson"></param>
        /// <returns></returns>
        public bool AddWeChatMenuExt(System.Guid appId, string menuJson)
        {
            return TPS.WCPSV.Instance.AddWXMenu(appId, menuJson);
        }

        private ResultDTO<List<QrTypeDTO>> GetQrCodeTypeListExt(WeChatQRCodeSearchDTO search)
        {
            IEnumerable<WeChatQrCodeType> query = WeChatQrCodeType.ObjectSet().AsQueryable();

            //if (!search.IsShowCatering)
            //{
            //    query = WeChatQrCodeType.ObjectSet().Where(c => c.UseTye != 1);
            //}

            List<QrTypeDTO> resultData = query.Select(c => new QrTypeDTO { Type = c.Type, Description = c.Description }).ToList();
            return new ResultDTO<List<QrTypeDTO>>
                {
                    isSuccess = true,
                    Message = "success",
                    Data = resultData
                };
        }

        private ResultDTO<ListResult<WeChatQRCodeShowDTO>> GetWechatQrCodeListExt(WeChatQRCodeSearchDTO search)
        {
            var result = new ListResult<WeChatQRCodeShowDTO>();
            //var query = BE.WeChatQRCode.ObjectSet().Where(c => c.IsDel != 1 && c.AppId == search.AppId);
            var query = from code in WeChatQRCode.ObjectSet()
                        join type in WeChatQrCodeType.ObjectSet() on code.QRType.Value equals type.Type
                        where code.IsDel != 1 && code.AppId == search.AppId
                        select new
                            {
                                Id = code.Id,
                                WeChatPublicCode = code.WeChatPublicCode,
                                AppId = code.AppId,
                                QRNo = code.QRNo,
                                WeChatTicket = code.WeChatTicket,
                                StoreId = code.StoreId,
                                IsDel = code.IsDel,
                                IsUse = code.IsUse,
                                SpreadInfoId = code.SpreadInfoId,
                                Description = code.Description,
                                QRType = code.QRType,
                                SubTime = code.SubTime,
                                QRTypeDesc = type.Description,
                                QRTypeUseType = type.UseTye,
                                Name = code.Name
                            };

            if (search.QRType.HasValue)
            {
                query = query.Where(q => q.QRType.Value == search.QRType.Value);
            }
            //else if (!search.IsShowCatering)
            //{
            //    query = query.Where(c => c.QRTypeUseType != 1);
            //}
            if (!string.IsNullOrEmpty(search.WeChatPublicCode))
            {
                query = query.Where(q => q.WeChatPublicCode == search.WeChatPublicCode);
            }
            if (search.IsUse.HasValue)
            {
                query = query.Where(q => q.IsUse == search.IsUse.Value);
            }

            result.Count = query.Count();
            result.List = query.OrderByDescending(q => q.SubTime).
                Skip((search.PageIndex - 1) * search.PageSize).
                Take(search.PageSize).
                Select(q => new WeChatQRCodeShowDTO
                {
                    Id = q.Id,
                    WeChatPublicCode = q.WeChatPublicCode,
                    AppId = q.AppId,
                    QRNo = q.QRNo,
                    WeChatTicket = q.WeChatTicket,
                    StoreId = q.StoreId,
                    IsDel = q.IsDel,
                    IsUse = q.IsUse,
                    SpreadInfoId = q.SpreadInfoId,
                    Description = q.Description,
                    QRType = q.QRType.Value,
                    SubTime = q.SubTime,
                    QrTypeDesc = q.QRTypeDesc,
                    Name = q.Name
                }).
                ToList();
            if (result.List.Any())
            {
                foreach (var weChatQrCodeShowDTO in result.List)
                {
                    if (string.IsNullOrEmpty(weChatQrCodeShowDTO.Name))
                    {
                        weChatQrCodeShowDTO.Name = weChatQrCodeShowDTO.QRNo.ToString(CultureInfo.InvariantCulture);
                    }

                }
            }
            return new ResultDTO<ListResult<WeChatQRCodeShowDTO>>() { isSuccess = true, Data = result };
        }

        private ResultDTO CreateWeChatQrCodeBatchExt(QrCodeCreateDTO dto)
        {
            if (dto == null || dto.AppId == Guid.Empty || dto.CreateNo <= 0)
                return new ResultDTO() { ResultCode = 1, Message = "参数为空" };
            try
            {
                WeChatQrCodeSV worker = new WeChatQrCodeSV();
                var developerInfo = WCPSV.Instance.GetDeveloperInfo(dto.AppId);
                if (developerInfo == null || string.IsNullOrEmpty(developerInfo.WAppId) || string.IsNullOrEmpty(developerInfo.WSecret))
                    return new ResultDTO() { ResultCode = 3, Message = "未配置公众号" };
                var tokenInfo = worker.GetToken(dto.AppId);
                ResultDTO ret = new ResultDTO();
                //获取token失败
                if (tokenInfo == null)
                {
                    ret.ResultCode = 2;
                    ret.Message = "获取微信access_token失败";
                    return ret;
                }
                if (!tokenInfo.isSuccess)
                {
                    ret.ResultCode = 2;
                    ret.Message = tokenInfo.Message;
                    return ret;
                }
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                int successCount = 0;
                bool hasError = false;
                string errorMess = string.Empty;
                for (int i = 0; i < dto.CreateNo; i++)
                {
                    var id = Guid.NewGuid();
                    var createResult = worker.CreateForeverQrcode(new ForeverQrcodeDTO()
                    {
                        SceneStr = id.ToString()
                    }, tokenInfo);
                    if (createResult == null)
                    {
                        hasError = true;
                        errorMess = "";
                        break;
                    }
                    if (!createResult.isSuccess)
                    {
                        hasError = true;
                        errorMess = createResult.Message;
                        break;
                    }

                    WeChatQRCode qrCode = new WeChatQRCode
                        {
                            Id = id,
                            WeChatPublicCode = dto.WeChatPublicCode,
                            WeChatAppId = developerInfo.WAppId,
                            WeChatSecret = developerInfo.WSecret,
                            AppId = dto.AppId,
                            QRNo = GetWeChatQRNoExt(),
                            QRType = { Value = dto.QrType },
                            SpreadInfoId = Guid.Empty,
                            WeChatTicket = createResult.Data,
                            Description = dto.Description,
                            EntityState = EntityState.Added
                        };
                    contextSession.SaveObject(qrCode);
                    contextSession.SaveChanges();
                    successCount++;
                }
                if (hasError)
                {

                    if (successCount > 0)
                    {
                        ret.ResultCode = 5;
                        ret.isSuccess = true;
                        ret.Message = string.Format("本次应生成二维码数量：{0}，实际生成数量：{1},失败原因：{2}", dto.CreateNo, successCount, errorMess);
                    }
                    else
                    {
                        ret.ResultCode = 4;
                        ret.Message = errorMess;
                    }
                    return ret;
                }
                return new ResultDTO { isSuccess = true, Message = "success" };
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("WeChatQRCodeBP.CreateWeChatQrCodeBatchExt异常，入参:{0}", JsonHelper.JsSerializer(dto)), ex);
                return new ResultDTO { Message = "异常，请重试！" };
            }
        }

        private ResultDTO UpdateStateExt(WeChatQRCodeUpdateStateDTO search)
        {
            BTP.Deploy.CustomDTO.ResultDTO ret = new Deploy.CustomDTO.ResultDTO();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            //WeChatQRCode code = new WeChatQRCode() { Id = search.Id, IsDel = search.IsDel ? 1 : 0 };
            var code = BE.WeChatQRCode.FindByID(search.Id);
            code.IsDel = search.IsDel ? 2 : 0;
            contextSession.SaveObject(code);
            try
            {
                int changes = contextSession.SaveChanges();
                ret.isSuccess = changes > 0;
                ret.Message = "更新成功";
                JAP.Common.Loging.LogHelper.Info("UpdateState:更新成功");
            }
            catch (Exception ex)
            {
                ret.isSuccess = false;
                ret.Message = ex.Message;
                JAP.Common.Loging.LogHelper.Error(ex.Message, ex);
            }
            return ret;
        }
    }
}