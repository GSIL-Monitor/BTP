
/***************
功能描述: BTPBP
作    者: 
创建时间: 2016/5/29 11:37:09
***************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.IService;
using Jinher.AMP.BTP.TPS;
using Jinher.JAP.BF.BP.Base;
using Jinher.JAP.PL;
using System.Data.Objects;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Deploy;
using Jinher.JAP.Common.Loging;
using NPOI.SS.Formula.Functions;
using AppExtensionDTO = Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO;

namespace Jinher.AMP.BTP.BP
{

    /// <summary>
    /// 
    /// </summary>
    public partial class InvoiceBP : BaseBP, IInvoice
    {
        /// <summary>
        /// 查询发票信息
        /// </summary>
        /// <param name="search">查询类</param>
        /// <returns>结果</returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceResultDTO> GetInvoiceInfoListExt(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            if (search == null || search.AppId == Guid.Empty || search.PageIndex < 0 || search.PageSize < 0)
            {
                return new ResultDTO<InvoiceResultDTO>() { ResultCode = 1, Message = "参数不能为空" };
            }
            var result = new ResultDTO<InvoiceResultDTO>();
            result.Data = new InvoiceResultDTO();

            var query = from invoice in Invoice.ObjectSet()
                        join commodityOrder in CommodityOrder.ObjectSet()
                            on invoice.CommodityOrderId equals commodityOrder.Id
                        join dataS in CommodityOrderService.ObjectSet()
                                               on invoice.CommodityOrderId equals dataS.Id
                                               into tempS
                        from tbS in tempS.DefaultIfEmpty()
                        where commodityOrder.AppId == search.AppId && commodityOrder.State > 0 && invoice.State > 0 && commodityOrder.State != 16 && commodityOrder.State != 17 && commodityOrder.IsDel != 2 && commodityOrder.IsDel != 3
                        select new
                            {
                                Invoice = invoice,
                                CommodityOrder = new InvoiceCommodityOrderInfo()
                                    {
                                        State = commodityOrder.State,
                                        Code = commodityOrder.Code,
                                        PaymentTime = commodityOrder.PaymentTime.Value,
                                        ReceiptUserName = commodityOrder.ReceiptUserName,
                                        ReceiptPhone = commodityOrder.ReceiptPhone,
                                        ReceiptAddress = commodityOrder.ReceiptAddress,
                                        RealPrice = commodityOrder.RealPrice.Value,
                                        Payment = commodityOrder.Payment,
                                        GoldPrice = commodityOrder.GoldPrice,
                                        GoldCoupon = commodityOrder.GoldCoupon,
                                        Province = commodityOrder.Province,
                                        City = commodityOrder.City,
                                        District = commodityOrder.District,
                                        StateAfterSales = tbS.State == null ? -1 : tbS.State,
                                        SelfTakeFlag=commodityOrder.SelfTakeFlag
                                    }
                            };

            if (search.Category > -1)
            {
                query = query.Where(t => t.Invoice.Category == search.Category);
            }
            if (search.State > -1)
            {
                query = query.Where(t => t.Invoice.State == search.State);
            }
            if (!string.IsNullOrWhiteSpace(search.CommodityOrderState) && search.CommodityOrderState != "null" && search.CommodityOrderState != "-1")
            {
                if (search.CommodityOrderState.Contains(","))
                {
                    if (search.CommodityOrderState == "8,9,10,12,14")   //退款中
                    {
                        List<int> beforeState = new List<int>() { 8, 9, 10, 12, 14 };
                        List<int> afterState = new List<int>() { 5, 10, 12 };
                        query = query.Where(n => beforeState.Contains(n.CommodityOrder.State) || afterState.Contains(n.CommodityOrder.StateAfterSales));
                    }
                    else
                    {
                        int[] arrystate = Array.ConvertAll<string, int>(search.CommodityOrderState.Split(','), s => int.Parse(s));

                        //等发货且自提
                        if (arrystate.Contains(1) && arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State));
                        }
                        else if (arrystate.Contains(1))
                        {
                            if (arrystate.Contains(11))
                            {
                                int[] exceptTmp = new int[] { 1, 11 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State) || ((a.CommodityOrder.State == 1 || a.CommodityOrder.State == 11) && a.CommodityOrder.SelfTakeFlag == 0));
                            }
                            else
                            {
                                int[] exceptTmp = new int[] { 1 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State) || (a.CommodityOrder.State == 1 && a.CommodityOrder.SelfTakeFlag == 0));
                            }
                        }
                        else if (arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State) || ((a.CommodityOrder.State == 1 || a.CommodityOrder.State == 11) && a.CommodityOrder.SelfTakeFlag == 1));
                        }
                        else
                        {
                            query = query.Where(a => arrystate.Contains(a.CommodityOrder.State));
                        }
                    }
                }
                else
                {
                    if (search.CommodityOrderState != "-1" && search.CommodityOrderState != null && search.CommodityOrderState != "")
                    {
                        int _state = int.Parse(search.CommodityOrderState);
                        //待发货的
                        if (_state == 1)
                        {
                            query = query.Where(n => n.CommodityOrder.State == _state && n.CommodityOrder.SelfTakeFlag == 0);
                        }
                        //待自提的
                        else if (_state == 99)
                        {
                            query = query.Where(n => (n.CommodityOrder.State == 1 || n.CommodityOrder.State == 11) && n.CommodityOrder.SelfTakeFlag == 1);
                        }
                        else if (search.CommodityOrderState == "3") //交易成功
                        {
                            query = query.Where(n => n.CommodityOrder.State == 3 && (n.CommodityOrder.StateAfterSales == 3 || n.CommodityOrder.StateAfterSales == 15 || n.CommodityOrder.StateAfterSales == -1));
                        }
                        else if (search.CommodityOrderState == "7")
                        {
                            query = query.Where(n => n.CommodityOrder.State == 7 || n.CommodityOrder.StateAfterSales == 7);
                        }
                        else
                        {
                            query = query.Where(n => n.CommodityOrder.State == _state);
                        }
                        //countquery = countquery.Where(n => n.State == _state);
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(search.SeacrhContent))
            {
                //if (System.Text.RegularExpressions.Regex.IsMatch(search.SeacrhContent, "^[0-9]+$"))
                //{
                //    if (search.SeacrhContent.Length>11)
                //    {
                //        query = query.Where(p => p.CommodityOrder.Code.Contains(search.SeacrhContent));
                //    }
                //    else
                //    {
                //        query = query.Where(p => p.CommodityOrder.ReceiptPhone.Contains(search.SeacrhContent));
                //    }
                //}
                //else
                //{
                //    query = query.Where(p => p.CommodityOrder.ReceiptUserName.Contains(search.SeacrhContent));
                //}
                query = query.Where(p =>(p.CommodityOrder.Code.Contains(search.SeacrhContent) || p.CommodityOrder.ReceiptPhone.Contains(search.SeacrhContent) || p.CommodityOrder.ReceiptUserName.Contains(search.SeacrhContent) || p.Invoice.InvoiceTitle.Contains(search.SeacrhContent)));
            }
            result.Data.Count = query.Count();
            var searchResult = query.OrderByDescending(n => n.CommodityOrder.PaymentTime).Skip((search.PageIndex - 1) * search.PageSize).Take(search.PageSize).ToList();

            //地址附值
            if (searchResult.Any())
            {
                foreach (var item in searchResult)
                {
                    item.CommodityOrder.Address = string.Format("{0}{1}{2}{3}", item.CommodityOrder.Province, item.CommodityOrder.City, item.CommodityOrder.District, item.CommodityOrder.ReceiptAddress);
                }
            }
            //由于优惠券与积分存在另一表，所以单独取
            if (searchResult.Any())
            {
                var orderIds = searchResult.Select(t => t.Invoice.CommodityOrderId).ToList();
                //优惠券与花费积分抵现金额 CouponValue SpendScoreCost
                var orderPayDetail = OrderPayDetail.ObjectSet().Where(t => orderIds.Contains(t.OrderId)).ToList();
                if (orderPayDetail.Count > 0)
                {
                    foreach (var item in searchResult)
                    {
                        var couponValue = orderPayDetail.Where(t => t.OrderId == item.Invoice.CommodityOrderId && t.ObjectType == 1).Select(t => t.Amount).FirstOrDefault();
                        item.CommodityOrder.CouponValue = couponValue;
                        var spendScoreMoney = orderPayDetail.Where(t => t.OrderId == item.Invoice.CommodityOrderId && t.ObjectType == 2).Select(t => t.Amount).FirstOrDefault();
                        item.CommodityOrder.SpendScoreMoney = spendScoreMoney;
                    }
                }
            }
            result.Data.InvoiceInfoList = new List<InvoiceInfoDTO>();
            foreach (var item in searchResult)
            {
                InvoiceInfoDTO model = new InvoiceInfoDTO();
                model.Id = item.Invoice.Id;
                model.CommodityOrderId = item.Invoice.CommodityOrderId;
                model.InvoiceTitle = item.Invoice.InvoiceTitle;
                model.InvoiceContent = item.Invoice.InvoiceContent;
                model.InvoiceType = item.Invoice.InvoiceType;
                model.SubTime = item.Invoice.SubTime;
                model.ModifiedOn = item.Invoice.ModifiedOn;
                model.ReceiptPhone = item.Invoice.ReceiptPhone;
                model.ReceiptEmail = item.Invoice.ReceiptEmail;
                model.State = item.Invoice.State;
                model.Remark = item.Invoice.Remark;
                model.Category = item.Invoice.Category;
                model.SubId = item.Invoice.SubId;
                model.Code = item.Invoice.Code;

                InvoiceCommodityOrderInfo orderInfo = new InvoiceCommodityOrderInfo();
                orderInfo.Code = item.CommodityOrder.Code;
                orderInfo.Payment = item.CommodityOrder.Payment;
                orderInfo.CouponValue = item.CommodityOrder.CouponValue;
                orderInfo.GoldPrice = item.CommodityOrder.GoldPrice;
                orderInfo.GoldCoupon = item.CommodityOrder.GoldCoupon;
                orderInfo.PaymentTime = item.CommodityOrder.PaymentTime;
                orderInfo.RealPrice = item.CommodityOrder.RealPrice;
                orderInfo.ReceiptAddress = item.CommodityOrder.ReceiptAddress;
                orderInfo.ReceiptPhone = item.CommodityOrder.ReceiptPhone;
                orderInfo.ReceiptUserName = item.CommodityOrder.ReceiptUserName;
                orderInfo.Province = item.CommodityOrder.Province;
                orderInfo.City = item.CommodityOrder.City;
                orderInfo.District = item.CommodityOrder.District;
                orderInfo.Address = item.CommodityOrder.Address;
                orderInfo.State = item.CommodityOrder.State;
                orderInfo.StateAfterSales = item.CommodityOrder.StateAfterSales;
                orderInfo.SelfTakeFlag = item.CommodityOrder.SelfTakeFlag;

                model.commodityOrderInfo = orderInfo;
                result.Data.InvoiceInfoList.Add(model);
            }
            result.ResultCode = 0;
            result.Message = "Success";
            return result;
        }
        /// <summary>
        /// 保存增值税发票资质信息
        /// </summary>
        /// <param name="VatInvoiceProof"></param>
        /// <returns></returns>
        public ResultDTO SaveVatInvoiceProofExt(Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO vatInvoiceP)
        {
            try
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                VatInvoiceProof vatInvoiceProof = VatInvoiceProof.ObjectSet().FirstOrDefault(n => n.Id == vatInvoiceP.Id);
                if (vatInvoiceProof == null)
                {
                    VatInvoiceProof vatInvoiceProo = VatInvoiceProof.CreateVatInvoiceProof();
                    vatInvoiceProo.Id = vatInvoiceP.Id;
                    vatInvoiceProo.CompanyName = vatInvoiceP.CompanyName;
                    vatInvoiceProo.Address = vatInvoiceP.Address;
                    vatInvoiceProo.BankCode = vatInvoiceP.BankCode;
                    vatInvoiceProo.BankName = vatInvoiceP.BankName;
                    vatInvoiceProo.BusinessLicence = vatInvoiceP.BusinessLicence;
                    vatInvoiceProo.PersonalProof = vatInvoiceP.PersonalProof;
                    vatInvoiceProo.Phone = vatInvoiceP.Phone;
                    vatInvoiceProo.IdentifyNo = vatInvoiceP.IdentifyNo;
                    vatInvoiceProo.TaxRegistration = vatInvoiceP.TaxRegistration;
                    contextSession.SaveObject(vatInvoiceProo);
                }
                else
                {
                    
                    vatInvoiceProof.EntityState = System.Data.EntityState.Modified;
                    vatInvoiceProof.Id = vatInvoiceP.Id;
                    vatInvoiceProof.CompanyName = vatInvoiceP.CompanyName;
                    vatInvoiceProof.Address = vatInvoiceP.Address;
                    vatInvoiceProof.BankCode = vatInvoiceP.BankCode;
                    vatInvoiceProof.BankName = vatInvoiceP.BankName;
                    vatInvoiceProof.BusinessLicence = vatInvoiceP.BusinessLicence;
                    vatInvoiceProof.PersonalProof = vatInvoiceP.PersonalProof;
                    vatInvoiceProof.Phone = vatInvoiceP.Phone;
                    vatInvoiceProof.IdentifyNo = vatInvoiceP.IdentifyNo;
                    vatInvoiceProof.TaxRegistration = vatInvoiceP.TaxRegistration;
                    contextSession.SaveObject(vatInvoiceProof);
                }

                contextSession.SaveChanges();
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("保存增值税发票资质信息异常。vatInvoiceP：{0}", vatInvoiceP), ex);
                return new ResultDTO { ResultCode = 1, Message = "Error" };
            }
            return new ResultDTO { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 设置发票类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO SetInvoiceCategoryExt(Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO model)
        {
            if (model == null || model.Id == Guid.Empty)
            {
                return new ResultDTO<AppExtensionDTO>() { ResultCode = 1, Message = "参数不能为空" };
            }
            var appExt = AppExtension.ObjectSet().Where(t => t.Id == model.Id).FirstOrDefault();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            if (appExt == null)
            {
                var appName = APPSV.GetAppName(model.Id);
                appExt = AppExtension.CreateAppExtension();
                appExt.Id = model.Id;
                appExt.AppName = appName;
                appExt.SubTime = DateTime.Now;
                appExt.ModifiedOn = DateTime.Now;
                appExt.IsShowSearchMenu = false;
                appExt.IsShowAddCart = false;
                appExt.IsDividendAll = null;
                appExt.SharePercent = 0;
                appExt.DistributeL1Percent = null;
                appExt.DistributeL2Percent = null;
                appExt.DistributeL3Percent = null;
                appExt.IsCashForScore = false;

                appExt.EntityState = System.Data.EntityState.Added;
                contextSession.SaveObject(appExt);
            }
            appExt.InvoiceDefault = model.InvoiceDefault;
            appExt.InvoiceValues = model.InvoiceValues;
            appExt.ModifiedOn = DateTime.Now;

            appExt.EntityState = EntityState.Modified;
            contextSession.SaveChanges();

            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO() { ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 修改发票
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO UpdateInvoiceExt(List<Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceUpdateDTO> list)
        {
            if (list == null || list.Count < 1)
            {
                return new ResultDTO() { ResultCode = 1, Message = "参数不能为空" };
            }
            var ids = list.Select(t => t.Id).ToList();
            var dataList = Invoice.ObjectSet().Where(t => ids.Contains(t.Id)).ToList();
            ContextSession contextSession = ContextFactory.CurrentThreadContext;
            foreach (var invoiceUpdateDto in list)
            {
                var model = dataList.Where(t => t.Id == invoiceUpdateDto.Id).FirstOrDefault();
                if (model != null)
                {
                    if (invoiceUpdateDto.ModifyType == 1)
                    {
                        if (checkStateChange(model.State, invoiceUpdateDto.State))
                        {
                            int oldState = model.State;
                            model.State = invoiceUpdateDto.State;
                            model.ModifiedOn = DateTime.Now;
                            model.EntityState = EntityState.Modified;

                            InvoiceJounal invoiceJounal = InvoiceJounal.CreateInvoiceJounal();
                            invoiceJounal.InvoiceId = invoiceUpdateDto.Id;
                            invoiceJounal.SubId = this.ContextDTO.LoginUserID;
                            invoiceJounal.StateFrom = oldState;
                            invoiceJounal.StateTo = model.State;
                            contextSession.SaveObject(invoiceJounal);
                        }
                    }
                    else if (invoiceUpdateDto.ModifyType == 2)
                    {
                        model.Remark = invoiceUpdateDto.Remark;
                        model.ModifiedOn = DateTime.Now;
                        model.EntityState = EntityState.Modified;
                    }
                }
            }
            contextSession.SaveChanges();
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO() { ResultCode = 0, Message = "Success" };
        }
        /// <summary>
        /// 获取全局设置的发票类型
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO> GetInvoiceCategoryExt(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                return new ResultDTO<AppExtensionDTO>() { ResultCode = 1, Message = "参数不能为空" };
            }
            var appEx = AppExtension.ObjectSet().Where(t => t.Id == appId).FirstOrDefault();
            if (appEx == null)
            {
                ContextSession contextSession = ContextFactory.CurrentThreadContext;
                appEx = AppExtension.CreateAppExtension();
                appEx.SetInvoiceValues(true, false, false);

                contextSession.SaveObject(appEx);
                contextSession.SaveChanges();

            }
            var data = new Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO();

            data.Id = appEx.Id;
            data.AppName = appEx.AppName;
            data.SubTime = appEx.SubTime;
            data.ModifiedOn = appEx.ModifiedOn;
            data.IsShowSearchMenu = appEx.IsShowSearchMenu;
            data.IsShowAddCart = appEx.IsShowAddCart;
            data.IsDividendAll = appEx.IsDividendAll;
            data.SharePercent = appEx.SharePercent;
            data.IsCashForScore = appEx.IsCashForScore;
            data.DistributeL1Percent = appEx.DistributeL1Percent;
            data.DistributeL2Percent = appEx.DistributeL2Percent;
            data.DistributeL3Percent = appEx.DistributeL3Percent;
            data.InvoiceDefault = appEx.InvoiceDefault;
            data.InvoiceValues = appEx.InvoiceValues;


            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.AppExtensionDTO>() { Data = data, ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 发票状态：0:待付款，1:待开票,2:已开票,3:已发出,4:已作废
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        private bool checkStateChange(int oldState, int newState)
        {
            if (oldState == 1)
            {
                if (newState == 2 || newState == 4)
                {
                    return true;
                }
            }
            else if (oldState == 2)
            {
                if (newState == 3)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 显示增值税发票信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProofExt(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return new ResultDTO<VatInvoiceProofInfoDTO>() { ResultCode = 1, Message = "参数不能为空" };
            }
            var query = VatInvoiceProof.ObjectSet().Where(n => n.Id == userId).FirstOrDefault();
            if (query == null)
            {
                return new ResultDTO<VatInvoiceProofInfoDTO>() { ResultCode = 2, Message = "没有可显示的增值税发票" };
            }
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO();
            result.Id = query.Id;
            result.IdentifyNo = query.IdentifyNo;
            result.PersonalProof = query.PersonalProof;
            result.Phone = query.Phone;
            result.CompanyName = query.CompanyName;
            result.BankCode = query.BankCode;
            result.BankName = query.BankName;
            result.BusinessLicence = query.BusinessLicence;
            result.Address = query.Address;
            result.TaxRegistration = query.TaxRegistration;
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO>() { Data = result, ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 显示增值税发票信息 金采支付使用
        /// </summary>
        /// <param name="jcActivityId"></param>
        /// <returns></returns>
        public Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO> ShowVatInvoiceProofIIExt(Guid jcActivityId)
        {
            if (jcActivityId == Guid.Empty)
            {
                return new ResultDTO<VatInvoiceProofInfoDTO>() { ResultCode = 1, Message = "参数不能为空" };
            }
            var query = TPS.ZPHSV.Instance.GetJCInvoiceByActivityId(jcActivityId);
            if (query == null)
            {
                return new ResultDTO<VatInvoiceProofInfoDTO>() { ResultCode = 2, Message = "没有可显示的增值税发票" };
            }
            var result = new Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO
            {
                Id = query.Data.id,
                IdentifyNo = query.Data.IdentifyNumber,
                PersonalProof = "",
                Phone = query.Data.RegisteredTel,
                CompanyName = query.Data.HeadValue,
                BankCode = query.Data.BankAccount,
                BankName = query.Data.DepositBank,
                BusinessLicence = "",
                Address = query.Data.RegisteredAddress,
                TaxRegistration = ""
            };
            return new Jinher.AMP.BTP.Deploy.CustomDTO.ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.VatInvoiceProofInfoDTO>() { Data = result, ResultCode = 0, Message = "Success" };
        }

        /// <summary>
        /// 获取导出的Excel数据
        /// </summary>
        public List<InvoiceExportDTO> GetInvoiceExportExt(InvoiceExportDTO search)
        {
            var query = from t1 in Invoice.ObjectSet()
                        join t2 in CommodityOrder.ObjectSet()
                        on t1.CommodityOrderId equals t2.Id
                        join t3 in OrderItem.ObjectSet()
                        on t1.CommodityOrderId equals t3.CommodityOrderId
                        select new
                        {
                           t2.AppId,t1.State,t1.InvoiceType, t1.SubTime,t1.InvoiceTitle, t3.Name, t2.RealPrice ,t2.Code
                        };
            if (search.AppId!=Guid.Empty)
            {
                query = query.Where(p => p.AppId == search.AppId);
            }
            if (search.StartTime.ToString() != "0001/1/1 0:00:00" && search.EndTime.ToString() != "0001/1/1 0:00:00")
            {
                query = query.Where(p =>(p.SubTime >= search.StartTime && p.SubTime <= search.EndTime));
            }
            if (search.InvoiceState!=0)
            {
                query = query.Where(p => p.State == search.InvoiceState);
            }
            if (search.InvoiceType != 0)
            {
                query = query.Where(p => p.InvoiceType == search.InvoiceType);
            }      
            List<InvoiceExportDTO> objlist = new List<InvoiceExportDTO>();
            foreach (var item in query.ToList())
            {
                InvoiceExportDTO model = new InvoiceExportDTO();
                model.Year = int.Parse(item.SubTime.Year.ToString());
                model.Month = int.Parse(item.SubTime.Month.ToString());
                model.SubTime =DateTime.Parse(item.SubTime.ToString("yyyy-MM-dd"));
                model.InvoiceTitle = item.InvoiceTitle;
                model.Content = item.Name;
                model.RealPrice = item.RealPrice;
                model.Code = item.Code;
                objlist.Add(model);
            }
            return objlist;
        }


        /// <summary>
        /// 获取导出的电子发票的详细数据
        /// </summary>
        public List<ElectronicInvoiceDTO> GetInvoiceExportDetailExt(Jinher.AMP.BTP.Deploy.CustomDTO.InvoiceSearchDTO search)
        {
            
            var query = from t1 in Invoice.ObjectSet()
                        join t2 in CommodityOrder.ObjectSet()
                        on t1.CommodityOrderId equals t2.Id
                        join dataS in CommodityOrderService.ObjectSet()
                                               on t1.CommodityOrderId equals dataS.Id
                                               into tempS
                        from tbS in tempS.DefaultIfEmpty()
                        where t2.AppId == search.AppId && t2.State > 0 && t1.State > 0 && t2.State != 16 && t2.State != 17 && t2.IsDel != 2 && t2.IsDel != 3
                        select new
                        {  
                            t1,
                            t2.Id,
                            CommodityOrder = new InvoiceCommodityOrderInfo()
                                    {
                                        State = t2.State,
                                        Code = t2.Code,
                                        PaymentTime = t2.PaymentTime.Value,
                                        ReceiptUserName = t2.ReceiptUserName,
                                        ReceiptPhone = t2.ReceiptPhone,
                                        ReceiptAddress = t2.ReceiptAddress,
                                        RealPrice = t2.RealPrice.Value,
                                        Payment = t2.Payment,
                                        GoldPrice = t2.GoldPrice,
                                        GoldCoupon = t2.GoldCoupon,
                                        Province = t2.Province,
                                        City = t2.City,
                                        District = t2.District,
                                        StateAfterSales = tbS.State == null ? -1 : tbS.State,
                                        SelfTakeFlag=t2.SelfTakeFlag
                                    },
                            model = new ElectronicInvoiceDTO()
                            {
                                AppId = t2.AppId,
                                Code = t2.Code,
                                ReceiptEmail = t1.ReceiptEmail,
                                ReceiptPhone = t1.ReceiptPhone,
                                InvoiceTitle = t1.InvoiceTitle,
                                BuyerCode = t1.Code,
                                ReceiptAddress =(t2.Province+t2.City+t2.District+ t2.ReceiptAddress),
                                RealPrice = t2.RealPrice,
                                Freight=t2.Freight
                            }
                        };

            if (search.Category > -1)
            {
                query = query.Where(p => p.t1.Category == search.Category);
            }
            if (search.State > -1)
            {
                query = query.Where(p => p.t1.State == search.State);
            }
            if (!string.IsNullOrWhiteSpace(search.CommodityOrderState) && search.CommodityOrderState != "null" && search.CommodityOrderState != "-1")
            {
                if (search.CommodityOrderState.Contains(","))
                {
                    if (search.CommodityOrderState == "8,9,10,12,14")   //退款中
                    {
                        List<int> beforeState = new List<int>() { 8, 9, 10, 12, 14 };
                        List<int> afterState = new List<int>() { 5, 10, 12 };
                        query = query.Where(p => beforeState.Contains(p.CommodityOrder.State) || afterState.Contains(p.CommodityOrder.StateAfterSales));
                    }
                    else
                    {
                        int[] arrystate = Array.ConvertAll<string, int>(search.CommodityOrderState.Split(','), s => int.Parse(s));

                        //等发货且自提
                        if (arrystate.Contains(1) && arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State));
                        }
                        else if (arrystate.Contains(1))
                        {
                            if (arrystate.Contains(11))
                            {
                                int[] exceptTmp = new int[] { 1, 11 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State) || ((a.CommodityOrder.State == 1 || a.CommodityOrder.State == 11) && a.CommodityOrder.SelfTakeFlag == 0));
                            }
                            else
                            {
                                int[] exceptTmp = new int[] { 1 };
                                int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                                query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State) || (a.CommodityOrder.State == 1 && a.CommodityOrder.SelfTakeFlag == 0));
                            }
                        }
                        else if (arrystate.Contains(99))
                        {
                            int[] exceptTmp = new int[] { 99 };
                            int[] arrystateTmp = arrystate.Except(exceptTmp).ToArray();
                            query = query.Where(a => arrystateTmp.Contains(a.CommodityOrder.State) || ((a.CommodityOrder.State == 1 || a.CommodityOrder.State == 11) && a.CommodityOrder.SelfTakeFlag == 1));
                        }
                        else
                        {
                            query = query.Where(a => arrystate.Contains(a.CommodityOrder.State));
                        }
                    }
                }
                else
                {
                    if (search.CommodityOrderState != "-1" && search.CommodityOrderState != null && search.CommodityOrderState != "")
                    {
                        int _state = int.Parse(search.CommodityOrderState);
                        //待发货的
                        if (_state == 1)
                        {
                            query = query.Where(n => n.CommodityOrder.State == _state && n.CommodityOrder.SelfTakeFlag == 0);
                        }
                        //待自提的
                        else if (_state == 99)
                        {
                            query = query.Where(n => (n.CommodityOrder.State == 1 || n.CommodityOrder.State == 11) && n.CommodityOrder.SelfTakeFlag == 1);
                        }
                        else if (search.CommodityOrderState == "3") //交易成功
                        {
                            query = query.Where(n => n.CommodityOrder.State == 3 && (n.CommodityOrder.StateAfterSales == 3 || n.CommodityOrder.StateAfterSales == 15 || n.CommodityOrder.StateAfterSales == -1));
                        }
                        else if (search.CommodityOrderState == "7")
                        {
                            query = query.Where(n => n.CommodityOrder.State == 7 || n.CommodityOrder.StateAfterSales == 7);
                        }
                        else
                        {
                            query = query.Where(n => n.CommodityOrder.State == _state);
                        }
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(search.SeacrhContent))
            {
                query = query.Where(p => (p.CommodityOrder.Code.Contains(search.SeacrhContent) || p.CommodityOrder.ReceiptPhone.Contains(search.SeacrhContent) || p.CommodityOrder.ReceiptUserName.Contains(search.SeacrhContent) || p.t1.InvoiceTitle.Contains(search.SeacrhContent)));
            }
            List<ElectronicInvoiceDTO> objlist = new List<ElectronicInvoiceDTO>();
            foreach (var item in query)
            {
                var serarch = from t1 in OrderItem.ObjectSet()
                              join t2 in Commodity.ObjectSet()
                              on t1.CommodityId equals t2.Id
                              where (t1.CommodityOrderId == item.Id)
                              select new { 
                                 t1.RealPrice,
                                 t1.Name,
                                 t1.Number,
                                 t2.TaxClassCode,
                                 t2.No_Code,
                                 t2.CostPrice,
                                 t2.TaxRate,
                                 t2.InputRax
                              };

                ElectronicInvoiceDTO model = new ElectronicInvoiceDTO();
                if (!string.IsNullOrEmpty(item.model.Code))
                {
                    model.Code = "jh" + item.model.Code;
                }
                else
                {
                    model.Code = null;
                }
                model.AppId = item.model.AppId;
                model.ReceiptEmail = item.model.ReceiptEmail;
                model.ReceiptPhone = item.model.ReceiptPhone;
                model.InvoiceTitle = item.model.InvoiceTitle;
                model.BuyerCode = item.model.BuyerCode;
                model.ReceiptAddress = item.model.ReceiptAddress;
                model.BuyerPhone = item.model.BuyerPhone;
                model.BuyerBankNumber = item.model.BuyerBankNumber;
                model.Specifications = item.model.Specifications;
                model.ProjectUnit = item.model.ProjectUnit;
                model.TallageMark = 1;//含税标志固定为1
                model.RealPrice = item.model.RealPrice;
                model.TaxRate = 0.16;//税率固定为0.17
                model.Remark = item.model.Remark;
                model.InvoicelineProperty = 0;//发票行性质国定为0
                model.PolicyMark = 0;//优惠政策国定为0
                model.ZeroTaxRateMark = item.model.ZeroTaxRateMark;
                model.SpecialParticular = item.model.SpecialParticular;
                model.Freight = item.model.Freight;
                if (serarch.Count()>0)
                {
                    List<SmallInvoiceDTO> objInvoice = new List<SmallInvoiceDTO>();
                    foreach (var _item in serarch.ToList())
                    {
                        SmallInvoiceDTO entity = new SmallInvoiceDTO();
                        entity.Price = _item.RealPrice;
                        entity.Name = _item.Name;
                        entity.Number = _item.Number;
                        entity.TaxClassCode = _item.TaxClassCode;
                        //如果商品编号不为空截取0前面的数据 
                        if (!string.IsNullOrWhiteSpace(_item.TaxClassCode))
                        {
                            entity.No_Code = _item.TaxClassCode.TrimEnd(new char[] { '0' });
                        }
                        if (string.IsNullOrWhiteSpace(_item.CostPrice.ToString()))
                        {
                            entity.CostPrice = 0;
                            entity.TaxRate = _item.TaxRate;
                        }
                        else
                        {
                            entity.CostPrice = _item.CostPrice;
                        }
                        objInvoice.Add(entity);
                    }
                    model.SmallInvoice = objInvoice;
                }
                objlist.Add(model);
            }
            return objlist;
        }

    }
}