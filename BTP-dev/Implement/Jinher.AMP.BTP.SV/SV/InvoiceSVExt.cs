
/***************
功能描述: BTPSV
作    者: 
创建时间: 2016/6/6 11:41:57
***************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.ISV.IService;
using Jinher.JAP.BF.SV.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.AMP.ZPH.Deploy.CustomDTO.AppSearchComdty;
using Jinher.JAP.Common.CallChain;
using Jinher.JAP.Common.Loging;
using AppExtensionDTO = Jinher.AMP.BTP.Deploy.AppExtensionDTO;

namespace Jinher.AMP.BTP.SV
{

    /// <summary>
    /// 
    /// </summary>
    public partial class InvoiceSV : BaseSv, IInvoice
    {

        /// <summary>
        /// 获取订单
        /// </summary>
        /// <param name="search">发票设置必传参数，AppIds，UserId</param>
        /// <returns></returns>
        public ResultDTO<InvoiceSettingDTO> GetInvoiceSettingExt(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            if (search == null || search.AppIds == null || !search.AppIds.Any() || search.UserId == Guid.Empty)
                return new ResultDTO<InvoiceSettingDTO>() { ResultCode = 1, Message = "参数为空" };
            InvoiceSettingDTO resultDto = new InvoiceSettingDTO() { };
            var apps = AppExtension.ObjectSet()
                                   .Where(c => search.AppIds.Contains(c.Id)).ToList();
                        //.Select(a => new AppExtension
                        //    {
                        //        Id = a.Id,
                        //        InvoiceValues = a.InvoiceValues,
                        //        InvoiceDefault = a.InvoiceDefault
                        //    }).ToList();
            AppExtension tempApp = new AppExtension() { InvoiceValues = 7, InvoiceDefault = 7 };
            foreach (var appId in search.AppIds)
            {
                int appInvoiceValues = 1;
                int appInvoiceDefault = 1;
                var app = apps.FirstOrDefault(c => c.Id == appId);
                if (app != null)
                {
                    appInvoiceValues = app.InvoiceValues;
                    appInvoiceDefault = app.InvoiceDefault;
                }
                tempApp.InvoiceValues = tempApp.InvoiceValues & appInvoiceValues;
                tempApp.InvoiceDefault = tempApp.InvoiceDefault & appInvoiceDefault;
            }
            resultDto.InvoiceDefault = (InvoiceCategoryEnum)tempApp.InvoiceDefault;
            resultDto.IsOrdinaryInvoice = tempApp.IsOrdinaryInvoice();
            resultDto.IsElectronicInvoice = tempApp.IsElectronicInvoice();
            resultDto.IsVATInvoice = tempApp.IsVATInvoice();
            resultDto.IsVatInvoiceProof = VatInvoiceProof.ObjectSet().Count(c => c.Id == search.UserId) > 0;
            return new ResultDTO<InvoiceSettingDTO> { Data = resultDto };
        }


        /// <summary>
        /// 获取发票历史数据
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="category">发票类型 1:增值税专用发票,2:电子发票,4:增值税专用发票</param>
        /// <returns></returns>
        public Deploy.CustomDTO.ResultDTO<List<InvoiceInfoDTO>> GetInvoiceInfoListExt(Guid appId, Guid userId,int category)
        {
            LogHelper.Debug("开始进入获取发票历史数据接口GetInvoiceInfoList：appId：" + appId + ",userId:" + userId + ",category:" + category);
            if (appId == null || userId == null || userId == Guid.Empty || appId == Guid.Empty)
                return new ResultDTO<List<InvoiceInfoDTO>>() { isSuccess = false, Message = "参数为空" };
            //判断appId是否是馆 是的话获取所有的入驻app集合 不是的话只获取当前app下的发票集合
            var appids = TPS.ZPHSV.Instance.GetAppIdlist(new List<Guid>() { appId }).Select(t => t.AppId).ToList();
            appids.Add(appId);

            List<InvoiceInfoDTO> resultDto = new List<InvoiceInfoDTO>();

            var temp = (from c in CommodityOrder.ObjectSet()
                       join i in Invoice.ObjectSet() on c.Id equals i.CommodityOrderId
                        where appids.Contains(c.AppId) && i.SubId == userId && i.Category == category && i.InvoiceType == 2
                        orderby i.SubTime descending 
                       select i).Distinct();

            LogHelper.Debug("开始进入获取发票历史数据接口GetInvoiceInfoList：temp：" + JsonHelper.JsSerializer(temp));

            foreach (var invoice in temp)
            {
                InvoiceInfoDTO rInfoDto = new InvoiceInfoDTO
                {
                    Id = invoice.Id,
                    InvoiceTitle = invoice.InvoiceTitle,
                    Code = invoice.Code,
                    SubTime = invoice.SubTime
                };
                var tem = resultDto.Where(t => t.InvoiceTitle == invoice.InvoiceTitle && t.Code == invoice.Code);
                if (!tem.Any())
                {
                    resultDto.Add(rInfoDto);
                }
            }

            return new ResultDTO<List<InvoiceInfoDTO>>
            {
                isSuccess = true,
                Data = resultDto.OrderByDescending(t => t.SubTime).ToList()
            };
        }
    }
}