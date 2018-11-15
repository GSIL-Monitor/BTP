using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using System.Text;
using Jinher.JAP.Common.Loging;
using Newtonsoft.Json.Linq;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class CarInsuranceController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [GridAction]
        public ActionResult GetCarInsuranceRebate(int page, int pageSize)
        {
            BTP.IBP.Facade.YJBDSFOrderInfoFacade yf = new IBP.Facade.YJBDSFOrderInfoFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO dto = new Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO();
            dto.PageIndex = page;
            dto.PageSize = pageSize;
            var result = yf.GetCarInsuranceRebate(dto);
            //foreach (var item in result.Data)
            //{
            //    if (item != null)
            //    {
            //        if (item.InsuranceCompanyCode != "")
            //        {
            //            item.InsuranceCompanyName = BE.InsuranceCompany.ObjectSet().Where(o => o.InsuranceCompanyCode == item.InsuranceCompanyCode).Select(x => x.Name).FirstOrDefault();
            //            var activity = BE.InsuranceCompanyActivity.ObjectSet().Where(o => o.InsuranceCompanyCode == item.InsuranceCompanyCode && o.IsAvailable == 1).FirstOrDefault();
            //            if (item.CompanyRebateMoney == 0)
            //            {
            //                if (activity != null) {
            //                    item.CompanyRebateMoney = Math.Round((item.BusinessInsuranceAmount.Value * (activity.BusinessRate / 100) + item.StrongInsuranceAmount.Value * (activity.StrongRate / 100) + item.CarShipAmount.Value * (activity.CarShipRate / 100)) / activity.TaxRate, 2);

            //                }
            //            }
            //            if(item.CompanyRebatePersent == 0)
            //            {
            //                item.CompanyRebatePersent = Math.Round(item.CompanyRebateMoney.Value / item.InsuranceAmount.Value, 2);
            //            }
            //            if(item.DouRebatePersent == 0)
            //            {
            //                item.DouRebatePersent = Math.Round(item.DouRebatePersent.Value / item.InsuranceAmount.Value, 2);
            //            }
            //            item.AfterTaxMoney = Math.Round(item.AfterTaxMoney.Value, 2);
            //            item.CompanyRemittanceMoney = Math.Round(item.CompanyRemittanceMoney.Value, 2);
            //        }
            //    }
            //}
            if (result != null && result.Data != null)
            {
                int id = 0;
                foreach (var item in result.Data)
                {
                    if (item != null)
                    {
                        id++;
                        item.Id = "trgrid" + id;
                        item.BusinessInsuranceAmount = item.BusinessInsuranceAmount == null ? 0 : item.BusinessInsuranceAmount;
                        var s = Math.Abs((item.CompanyRebateMoney.Value + item.DouRebateMoney.Value) - Math.Round((decimal)0.35 * item.BusinessInsuranceAmount.Value / (decimal)1.06, 2));
                        if (s >= 100)
                        {
                            item.IsCorrect = 0;
                        }
                        item.AfterTaxMoney = Math.Round(item.AfterTaxMoney.Value, 2);
                        item.CompanyRemittanceMoney = Math.Round(item.CompanyRemittanceMoney.Value, 2);
                    }
                }
            }
            IList<string> show = new List<string>();
            show.Add("Id");
            show.Add("RebateDate");
            show.Add("RebateNum");
            show.Add("InsuranceCompanyName");
            show.Add("AfterTaxMoney");
            show.Add("CompanyRemittanceMoney");
            show.Add("DouRebatePersent");
            show.Add("CompanyRebatePersent");
            show.Add("DouRebateMoney");
            show.Add("CompanyRebateMoney");
            show.Add("RebateMoney");
            show.Add("BusinessInsuranceAmount");
            show.Add("StrongInsuranceAmount");
            show.Add("CarShipAmount");
            show.Add("InsuranceAmount");
            show.Add("RemittanceNo");
            show.Add("AuditFlag");
            show.Add("IsCorrect");
            var gridobj = new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.CarInsurancePolymerizationDTO>(show, result.Data, result.ResultCode, page, pageSize, string.Empty);
            return View(gridobj);
        }

        public ActionResult CarInsuranceReport()
        {
            return View();
        }

        [HttpPost]
        [GridAction]
        public ActionResult GetCarInsuranceReport(int page, int pageSize, DateTime? begintime = null, DateTime? endtime = null, string name = "", string status = "")
        {
            IBP.Facade.YJBDSFOrderInfoFacade yf = new IBP.Facade.YJBDSFOrderInfoFacade();
            BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO arg = new BTP.Deploy.CustomDTO.YJBCarInsuranceReportSearchDTO();
            if (!string.IsNullOrEmpty(name))
            {
                var code = BE.InsuranceCompany.ObjectSet().Where(x => x.Name.Contains(name)).Select(x=>x.InsuranceCompanyCode).ToList();
                if (code != null&&code.Any())
                {
                    arg.CompanyCode = new List<string>();
                    foreach (var item in code)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            arg.CompanyCode.Add(item);
                        }
                    }
                }
            }
            arg.PageIndex = page;
            arg.PageSize = pageSize;
            arg.BeginTime = begintime;
            arg.EndTime = endtime;
            arg.Status = status;
            var result = yf.GetCarInsuranceReport(arg);
            IList<string> show = new List<string>();
            if (result.Data != null)
            {
                foreach (var item in result.Data)
                {
                    if (item != null && item.OrderNo != "")
                    {
                        item.InsuranceCompanyName = BE.InsuranceCompany.ObjectSet().Where(o => o.InsuranceCompanyCode == item.InsuranceCompanyCode).Select(x => x.Name).FirstOrDefault();
                        item.CompanyRebateMoney = BE.YJBCarInsuranceRebate.ObjectSet().Where(o => o.OrderNo == item.OrderNo).Select(o => o.CompanyRebateMoney).FirstOrDefault();
                        item.InsuranceRebateMoney = item.CompanyRebateMoney + (!string.IsNullOrEmpty(item.RecommendAmount) ? decimal.Parse(item.RecommendAmount) : 0);
                    }
                }
            }
            show.Add("Id");
            show.Add("State");
            show.Add("InsuranceCompanyName");
            show.Add("CompanyRebateMoney");
            show.Add("CustomAmount");
            show.Add("InsuranceRebateMoney");
            show.Add("OrderNo");
            show.Add("MemberPhone");
            show.Add("CustomPhone");
            show.Add("InsuranceAmount");
            show.Add("StrongInsuranceAmount");
            show.Add("BusinessAmount");
            show.Add("StrongInsuranceOrderId");
            show.Add("StrongInsuranceStartTime");
            show.Add("BusinessOrderId");
            show.Add("BusinessStartTime");
            show.Add("PlateNumber");
            show.Add("ChassisNumber");
            show.Add("EngineNumber");
            show.Add("CarTypeName");
            show.Add("RegisterTime");
            show.Add("IsTransfer");
            show.Add("CarOwnerName");
            show.Add("CarOwnerIdType");
            show.Add("CarOwnerId");
            show.Add("CarOwnerPhone");
            show.Add("CarOwnerAddress");
            show.Add("PolicyHolderName");
            show.Add("PolicyHolderIdType");
            show.Add("PolicyHolderId");
            show.Add("PolicyHolderPhone");
            show.Add("PolicyHolderAddress");
            show.Add("InsuranceTime");
            show.Add("RecommendName");
            show.Add("RecommendAmount");
            show.Add("CustomAmount");
            show.Add("SinopecAmount");
            show.Add("RebateState");
            show.Add("StrongInsurance_SI");
            show.Add("StrongInsurance_Car");
            show.Add("Business_Car");
            show.Add("Business_Three");
            show.Add("Business_Driver");
            show.Add("Business_Passenger");
            show.Add("Business_AllCar");
            show.Add("Business_Glass");
            show.Add("Business_Body");
            show.Add("Business_Engine");
            show.Add("Business_Natural");
            show.Add("Business_Garage");
            show.Add("Business_Third");
            show.Add("Business_Spirit");
            show.Add("NoDeductibles_Car");
            show.Add("NoDeductibles_Three");
            show.Add("NoDeductibles_Driver");
            show.Add("NoDeductibles_Passenger");
            show.Add("NoDeductibles_AllCar");
            show.Add("NoDeductibles_Body");
            show.Add("NoDeductibles_Engine");
            show.Add("NoDeductibles_Natural");
            show.Add("NoDeductibles__Spirit");
            show.Add("BusinessEndTime");
            show.Add("StrongInsuranceEndTime");
            var gridobj = new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceReportDTO>(show, result.Data, result.ResultCode, page, pageSize, string.Empty);
            return View(gridobj);
        }

        /// <summary>
        /// 返利数据审核
        /// </summary>
        /// <param name="RemittanceNo"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AuditRebate(string RemittanceNo)
        {
            //先判断手机号是否为易捷APP注册手机号，如果是，进行返利，如果不是，直接提示返利失败
            #region 根据汇款单号获取订单号
            BTP.IBP.Facade.YJBDSFOrderInfoFacade yf = new IBP.Facade.YJBDSFOrderInfoFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO searchDto = new Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO();
            searchDto.RemittanceNo = RemittanceNo;
            searchDto.PageIndex = 0;
            var result = yf.GetCarRebateByRemittanceNo(searchDto);
            List<string> OrderList = new List<string>();
            if (result.Data.Any() && result.Data != null)
            {
                foreach (var item in result.Data)
                {
                    OrderList.Add(item.OrderNo);
                }
            }
            #endregion
           
            #region 根据订单号获取订单表数据
            BTP.IBP.Facade.YJBDSFOrderInfoFacade of = new IBP.Facade.YJBDSFOrderInfoFacade();
            var orderResult = of.GetDSFOrderInfoByOrderNos(OrderList);
            #endregion
            LogHelper.Info("返利数据审核RemittanceNo为：" + RemittanceNo);
            var addresult = new Jinher.AMP.YJB.Deploy.CustomDTO.ResultDTO();
            addresult.IsSuccess = false;
            if (orderResult.Data.Any() && orderResult.Data != null && orderResult.Data.Count > 0)
            {
                LogHelper.Info("返利数据审核orderResult为：" + orderResult.Data.ToString());
                foreach (var item in orderResult.Data)
                {
                    var userid = TPS.CBCSV.GetUserIdByAccount(item.PhoneNum);
                    LogHelper.Info("返利数据审核userid为：" + userid);
                    if (userid == Guid.Empty)
                    {//返利失败
                        yf.UpdateCarRebateState(item.OrderNo,3);
                    }
                    else
                    {
                        Jinher.AMP.YJB.Deploy.CustomDTO.AddYouKaOrderDTO arg = new Jinher.AMP.YJB.Deploy.CustomDTO.AddYouKaOrderDTO();
                        arg.Type = 2;
                        arg.OrderCode = item.OrderNo;
                        arg.OrderMoney = item.OrderPayMoney;
                        arg.OrderSubTime = item.OrderPayDate;
                        arg.OrderCompleteTime = item.RebateDate;
                        //根据手机号查询UserId
                        arg.UserId = userid;
                        arg.GiveMoney = item.RebateMoney;
                        arg.UserAccount = item.PhoneNum;
                        arg.Source = 9;
                        arg.OrderId = Guid.NewGuid();
                        LogHelper.Info(item.PhoneNum + "对应UserId：" + userid);
                        var orderItem = new List<Jinher.AMP.YJB.Deploy.CustomDTO.AddYouKaOrderItemDTO>();
                        if (!string.IsNullOrEmpty(item.Commodity))
                        {
                            var commoditylist = JsonHelper.JsonDeserialize<List<Jinher.AMP.YJB.Deploy.CustomDTO.CommodityDTO>>(item.Commodity);
                            if (commoditylist.Any() && commoditylist.Count > 0)
                            {
                                foreach (var commodity in commoditylist)
                                {
                                    Jinher.AMP.YJB.Deploy.CustomDTO.AddYouKaOrderItemDTO itemdto = new Jinher.AMP.YJB.Deploy.CustomDTO.AddYouKaOrderItemDTO();
                                    itemdto.OrderCode = arg.OrderCode;
                                    itemdto.UserId = arg.UserId;
                                    itemdto.GiveMoney = 0;
                                    itemdto.CommodityName = commodity.Name;
                                    itemdto.Number = commodity.Num;
                                    itemdto.RealPrice = commodity.Price;
                                    itemdto.TotalPrice = itemdto.Number * itemdto.RealPrice;
                                    itemdto.Source = 9;
                                    itemdto.OrderId = Guid.NewGuid();
                                    orderItem.Add(itemdto);
                                }
                            }
                        }
                        arg.OrderItemList = orderItem;
                        addresult = TPS.CouponSV.AddYouKaByInsuranceRebate(arg);
                        LogHelper.Info("返利数据审核Message为：" + addresult.Message);
                        if (addresult.Message != "已赠送过")
                        {
                            LogHelper.Info("addresult.IsSuccess:" + addresult.IsSuccess);
                            if (addresult.IsSuccess)
                            {//返利成功
                                yf.UpdateCarRebateState(item.OrderNo, 2);
                            }
                            else
                            {//返利失败
                                yf.UpdateCarRebateState(item.OrderNo, 3);
                            }
                        }
                    }
                }
            }

            return Json(addresult);

        }

        /// <summary>
        /// 保险统计报表导出Excel
        /// </summary>
        /// <param name="JQgridTable"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public FileResult ExportExcel(string JQgridTable)
        {
            var list = JsonHelper.JsonDeserialize<List<Jinher.AMP.BTP.Deploy.ExportDTO.YJBCarInsuranceReportExportDTO>>(JQgridTable);
            //JArray array = JArray.Parse(JQgridTable.ToString());
            //var list = array.ToObject<List<Jinher.AMP.BTP.Deploy.ExportDTO.YJBCarInsuranceReportExportDTO>>();

            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> {  "状态","保险公司名称", "保险公司返利金额", "爱豆返利金额",  "返利总金额","订单编号", "会员手机号", "客户手机号", "保单金额", "交强险总保费", "商业险总保费", "交强险单号","交强险期限", "商业险单号",
            "商业险期限", "车牌号", "车架号", "发动机号", "车型名称", "注册日期", "是否过户车", "车主姓名", "车主证件类型",
            "车主证件号", "车主手机号", "车主地址", "投保人姓名", "投保人证件类型", "投保人证件号", "投保人手机号", "投保人地址", "时间",
             "推荐员工", "员工奖励金额", "客户返利金额", "石化佣金", "客户返利支付状态", "交强险（交强险）", "交强险（车船税）", "商业险（车损险）",
             "商业险（三责险）", "商业险（司机责任险）", "商业险（乘客责任险）", "商业险（全车盗抢险）", "商业险（玻璃破碎险）",
             "商业险（车身划痕险）", "商业险（发动机损失险）", "商业险（自燃损失险）", "商业险（专修厂特约）",
             "商业险（第三方特约）", "商业险（精神损害险）", "不计免赔（车损险）", "不计免赔（三责险）", "不计免赔（司机责任险）", "不计免赔（乘客责任险）", "不计免赔（全车盗抢险）", "不计免赔（车身划痕险）", "不计免赔（发动机损失险）", "不计免赔（自燃损失险）", "不计免赔（精神损害险）"};
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            foreach (var model in list)
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.State);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceCompanyName);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CompanyRebateMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CustomAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceRebateMoney);

                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OrderNo);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.MemberPhone);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CustomPhone);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StrongInsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.BusinessAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StrongInsuranceOrderId);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StrongInsuranceStartTime);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.BusinessOrderId);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.BusinessStartTime);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PlateNumber);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.ChassisNumber);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.EngineNumber);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarTypeName);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RegisterTime);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.IsTransfer);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarOwnerName);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarOwnerIdType);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarOwnerId);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarOwnerPhone);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarOwnerAddress);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PolicyHolderName);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PolicyHolderIdType);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PolicyHolderId);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PolicyHolderPhone);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PolicyHolderAddress);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceTime);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RecommendName);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RecommendAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CustomAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.SinopecAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateState);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StrongInsurance_SI);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StrongInsurance_Car);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Car);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Three);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Driver);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Passenger);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_AllCar);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Glass);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Body);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Engine);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Natural);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Garage);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Third);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.Business_Spirit);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_Car);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_Three);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_Driver);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_Passenger);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_AllCar);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_Body);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_Engine);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles_Natural);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.NoDeductibles__Spirit);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.UTF8.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("保险统计报表{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        /// <summary>
        /// 保险汇款单导出Excel
        /// </summary>
        /// <param name="JQgridTable"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public FileResult ExportRebateExcel(string JQgridTable)
        {
            var list = JsonHelper.JsonDeserialize<List<Jinher.AMP.BTP.Deploy.ExportDTO.YJBCarInsuranceRebateDTO>>(JQgridTable);
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "汇款日期", "笔数", "保险公司名称","爱豆汇款金额", "保险公司汇款金额", "爱豆返利比例", "保险公司返利比例", "爱豆返利金额", "保险公司返利金额", "返利总金额", "商险金额", "强险金额", "车船税","保单金额", "汇款单号"};
           
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            foreach (var model in list)
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateDate);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateNum);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceCompanyName);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.AfterTaxMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CompanyRemittanceMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DouRebatePersent);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CompanyRebatePersent);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DouRebateMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CompanyRebateMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.BusinessInsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StrongInsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarShipAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RemittanceNo);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.Default.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("保险汇款单{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        /// <summary>
        /// 保险汇款单详情页Excel导出
        /// </summary>
        /// <param name="JQgridTable"></param>
        /// <returns></returns>
        [HttpPost]
        public FileResult ExportRebateDetailExcel(string JQgridTable)
        {
            var list = JsonHelper.JsonDeserialize<List<Jinher.AMP.BTP.Deploy.ExportDTO.YJBCarInsuranceRebateDetailDTO>>(JQgridTable);
            var sbHtml = new StringBuilder();
            sbHtml.Append("<table border='1' cellspacing='0' cellpadding='0'>");
            sbHtml.Append("<tr>");
            var lstTitle = new List<string> { "汇款日期", "审核日期", "订单编号", "笔数", "保险公司名称", "爱豆汇款金额", "保险公司汇款金额", "爱豆返利比例", "保险公司返利比例", "爱豆返利金额", "保险公司返利金额", "返利总金额", "商险金额", "强险金额", "车船税", "保单金额", "手机号", "汇款单号", "状态" };
            foreach (var item in lstTitle)
            {
                sbHtml.AppendFormat("<td style='font-size: 14px;text-align:center;background-color: #DCE0E2; font-weight:bold;' height='25'>{0}</td>", item);
            }
            sbHtml.Append("</tr>");
            foreach (var model in list)
            {
                sbHtml.Append("<tr>");
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateDate);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.AuditDate);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.OrderNo);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateNum);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceCompanyName);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DouRemittanceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CompanyRemittanceMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DouRebatePersent);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CompanyRebatePersent);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.DouRebateMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CompanyRebateMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateMoney);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.BusinessInsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.StrongInsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.CarShipAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.InsuranceAmount);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.PhoneNum);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RemittanceNo);
                sbHtml.AppendFormat("<td style='font-size: 12px;height:20px;'>{0}</td>", model.RebateState);
                sbHtml.Append("</tr>");
            }
            sbHtml.Append("</table>");
            return File(System.Text.Encoding.Default.GetBytes(sbHtml.ToString()), "application/ms-excel", string.Format("保险汇款单详情{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        public ActionResult RebateDetail(string RebateId)
        {
            ViewBag.RebateId = RebateId;
            return View();
        }

        [HttpPost]
        [GridAction]
        public ActionResult GetRebateDetail(int page, int pageSize, string RebateId)
        {
            BTP.IBP.Facade.YJBDSFOrderInfoFacade yf = new IBP.Facade.YJBDSFOrderInfoFacade();
            Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO arg = new Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateSearchDTO();
            arg.PageIndex = page;
            arg.PageSize = pageSize;
            arg.RemittanceNo = RebateId;
            var result = yf.GetCarRebateByRemittanceNo(arg);
            if (result != null && result.Data != null)
            {
                int id = 0;
                foreach (var item in result.Data)
                {
                    if (item != null)
                    {
                        //if (item.InsuranceCompanyCode != "")
                        //{
                        //    item.InsuranceCompanyName = BE.InsuranceCompany.ObjectSet().Where(o => o.InsuranceCompanyCode == item.InsuranceCompanyCode).Select(x => x.Name).FirstOrDefault();
                        //    var activity = BE.InsuranceCompanyActivity.ObjectSet().Where(o => o.InsuranceCompanyCode == item.InsuranceCompanyCode && o.IsAvailable == 1).FirstOrDefault();
                        //    if (item.CompanyRebateMoney == 0)
                        //    {
                        //        if (activity != null)
                        //        {
                        //            item.CompanyRebateMoney = Math.Round((item.BusinessInsuranceAmount.Value * (activity.BusinessRate / 100) + item.StrongInsuranceAmount.Value * (activity.StrongRate / 100) + item.CarShipAmount.Value * (activity.CarShipRate / 100)) / activity.TaxRate, 2);

                        //        }
                        //    }
                        //    if (item.CompanyRebatePersent == 0)
                        //    {
                        //        item.CompanyRebatePersent = Math.Round(item.CompanyRebateMoney.Value / item.InsuranceAmount, 2);
                        //    }
                        //    if (item.DouRebatePersent == 0)
                        //    {
                        //        item.DouRebatePersent = Math.Round(item.DouRebatePersent.Value / item.InsuranceAmount, 2);
                        //    }
                        //}
                        id++;
                        item.Id = "trdetialgrid" + id;
                        item.BusinessInsuranceAmount = item.BusinessInsuranceAmount == null ? 0 : item.BusinessInsuranceAmount;
                        var s = Math.Abs((item.CompanyRebateMoney.Value + item.DouRebateMoney.Value) - Math.Round((decimal)0.35 * item.BusinessInsuranceAmount.Value / (decimal)1.06, 2));
                        if (s >= 100)
                        {
                            item.IsCorrect = 0;
                        }
                        item.DouRemittanceAmount = Math.Round(item.DouRemittanceAmount, 2);
                        item.CompanyRemittanceMoney = Math.Round(item.CompanyRemittanceMoney.Value, 2);
                    }
                }
            }
            List<string> show = new List<string>();
            show.Add("Id");
            show.Add("RebateDate");
            show.Add("AuditDate");
            show.Add("OrderNo");
            show.Add("RebateNum");
            show.Add("InsuranceCompanyName");
            show.Add("DouRemittanceAmount");
            show.Add("CompanyRemittanceMoney");
            show.Add("DouRebatePersent");
            show.Add("CompanyRebatePersent");
            show.Add("DouRebateMoney");
            show.Add("CompanyRebateMoney");
            show.Add("RebateMoney");
            show.Add("BusinessInsuranceAmount");
            show.Add("StrongInsuranceAmount");
            show.Add("CarShipAmount");
            show.Add("InsuranceAmount");
            show.Add("PhoneNum");
            show.Add("RemittanceNo");
            show.Add("RebateState");
            show.Add("IsCorrect");
            var gridobj = new GridModel<Jinher.AMP.BTP.Deploy.CustomDTO.YJBCarInsuranceRebateDataDTO>(show, result.Data, result.ResultCode, page, pageSize, string.Empty);
            return View(gridobj);
        }

    }
}
