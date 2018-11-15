using System;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.TPS;
using System.Collections.Generic;
using Jinher.AMP.BTP.UI.Filters;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.Util;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    ///  结算管理
    /// </summary>
    public class SettleAccountsController : Jinher.JAP.MVC.Controller.BaseController
    {
        /// <summary>
        /// 结算管理页面
        /// </summary>
        [CheckAppId]
        public ActionResult Index()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            if (ZPHSV.Instance.IsAppPavilion(appId))
            {
                return View("MallIndex");
            }
            else
            {
                return View("SellerIndex");
            }
        }

        /// <summary>
        /// 获取结算数据
        /// </summary>
        [HttpPost]
        public ActionResult GetMallData(SettleAccountSearchDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            dto.EsAppId = appId;
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetMallSettleAccounts(dto));
        }

        /// <summary>
        /// 获取结算数据
        /// </summary>
        [HttpPost]
        public ActionResult GetSellerData(SettleAccountSearchDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商户ID" });
            }
            dto.AppId = appId;
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetSellerSettleAccounts(dto));
        }

        /// <summary>
        /// 导入历史订单单
        /// </summary>
        public ActionResult Import(SettleAccountDetailsCreateDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            dto.EsAppId = appId;
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.CreateSettleAccountDetails(dto));
        }

        /// <summary>
        /// 生成结算单
        /// </summary>
        public ActionResult Create(List<Guid> ids)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.UpdateState(new SettleAccountUpdateStateDTO { Ids = ids, State = 1 })); ;
        }

        /// <summary>
        /// 获取商家银行信息
        /// </summary>
        [HttpGet]
        public ActionResult GetAppInfo(Guid id)
        {
            var ownerInfo = APPSV.Instance.GetAppOwnerInfo(id);
            if (ownerInfo == null)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "获取应用主ID失败" }, JsonRequestBehavior.AllowGet);
            }
            return Json(ExchangeSV.GetChargeAccounts(ownerInfo.OwnerId), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 确认结算单
        /// </summary>
        /// <param name="id">结算单ID</param>
        public ActionResult UpdateConfirmStatus(Guid id)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.UpdateState(new SettleAccountUpdateStateDTO { Ids = new List<Guid> { id }, State = 2 })); ;
        }

        /// <summary>
        /// 置为已打款
        /// </summary>
        /// <param name="id">结算单ID</param>
        public ActionResult UpdatePaidStatus(Guid id)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.UpdateState(new SettleAccountUpdateStateDTO { Ids = new List<Guid> { id }, State = 3 })); ;
        }

        #region 结算详情
        /// <summary>
        /// 查看结算单详细信息
        /// </summary>
        [CheckAppId]
        public ActionResult Details(Guid id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            if (ZPHSV.Instance.IsAppPavilion(appId))
            {
                ViewBag.IsMall = true;
            }
            else
            {
                ViewBag.IsMall = false;
            }
            ViewBag.IsFromHistory = Request.UrlReferrer.AbsolutePath.ToLower().Contains("history") ? true : false;
            SettleAccountFacade facade = new SettleAccountFacade();
            SettleAccountDetailsDTO result = facade.GetSettleAccountDetails(id).Data;

            if (result.SellerType == 1)
            {
                return View("ThirdPartDetails", result);
            }
            else
            {
                return View(result);
            }
        }

        /// <summary>
        /// 获取结算单订单数据
        /// </summary>
        public ActionResult GetOrderInfoes(SettleAccountOrderSearchDTO dto)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetSettleAccountOrders(dto), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取结算单订单项数据
        /// </summary>
        public ActionResult GetOrderItemInfoes(Guid id)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetSettleAccountOrderItems(id), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 结算历史
        /// <summary>
        /// 查看结算历史
        /// </summary>
        [CheckAppId]
        public ActionResult History()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            if (ZPHSV.Instance.IsAppPavilion(appId))
            {
                return View("MallHistory");
            }
            else
            {
                return View("SellerHistory");
            }
        }

        /// <summary>
        /// 获取结算历史数据
        /// </summary>
        [HttpPost]
        public ActionResult GetMallHistoryData(SettleAccountHistorySearchDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            dto.EsAppId = appId;
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetMallSettleAccountHistories(dto));
        }

        /// <summary>
        /// 获取结算历史数据
        /// </summary>
        [HttpPost]
        public ActionResult GetSellerHistoryData(SettleAccountHistorySearchDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商户ID" });
            }
            dto.AppId = appId;
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.GetSellerSettleAccountHistories(dto));
        }

        /// <summary>
        /// 导出结算单
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Export(string id)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }
            //var isMall = ZPHSV.Instance.IsAppPavilion(appId);
            List<ExportViewModel> model = new List<ExportViewModel>();
            SettleAccountFacade saFacade = new SettleAccountFacade();
            foreach (var cid in id.Split(','))
            {
                var gid = new Guid(cid);
                ExportViewModel m = new ExportViewModel();
                m.SettleAccount = saFacade.GetSettleAccountDetails(gid).Data;
                m.OrderInfoes = saFacade.GetSettleAccountOrders(new SettleAccountOrderSearchDTO
                {
                    Id = gid,
                    PageIndex = 1,
                    PageSize = int.MaxValue
                }).Data.List;
                m.ChargeAccount = new ChargeAccountDTO();
                var ownerInfo = APPSV.Instance.GetAppOwnerInfo(m.SettleAccount.AppId);
                var chargeInfo = ExchangeSV.GetChargeAccounts(ownerInfo.OwnerId);
                if (chargeInfo.isSuccess)
                {
                    m.ChargeAccount = chargeInfo.Data;
                }
                model.Add(m);
            }

            int currentRowNum = 0;
            short commonRowHeight = 30 * 20;
            //建立空白工作簿
            IWorkbook workbook = new HSSFWorkbook();
            //在工作簿中：建立空白工作表
            ISheet sheet = workbook.CreateSheet();

            // 通用样式字体
            IFont commonFont = workbook.CreateFont();
            commonFont.FontHeightInPoints = 11;
            commonFont.FontName = "宋体";

            // 通用样式
            ICellStyle commonStyle = workbook.CreateCellStyle();
            commonStyle.VerticalAlignment = VerticalAlignment.Center;
            commonStyle.BorderTop = BorderStyle.Thin;
            commonStyle.BorderRight = BorderStyle.Thin;
            commonStyle.BorderLeft = BorderStyle.Thin;
            commonStyle.BorderBottom = BorderStyle.Thin;
            commonStyle.SetFont(commonFont);

            // 标红样式
            ICellStyle redStyle = workbook.CreateCellStyle();
            redStyle.VerticalAlignment = VerticalAlignment.Center;
            redStyle.BorderTop = BorderStyle.Thin;
            redStyle.BorderRight = BorderStyle.Thin;
            redStyle.BorderLeft = BorderStyle.Thin;
            redStyle.BorderBottom = BorderStyle.Thin;
            // append
            IFont redFont = workbook.CreateFont();
            redFont.FontHeightInPoints = 11;
            redFont.FontName = "宋体";
            redFont.Color = HSSFColor.Red.Index;
            redStyle.SetFont(redFont);

            // 订单顶列表头
            ICellStyle orderItemHeadRowStyle = workbook.CreateCellStyle();
            orderItemHeadRowStyle.SetFont(commonFont);
            orderItemHeadRowStyle.VerticalAlignment = VerticalAlignment.Center;
            orderItemHeadRowStyle.BorderTop = BorderStyle.Thin;
            orderItemHeadRowStyle.BorderRight = BorderStyle.Thin;
            orderItemHeadRowStyle.BorderLeft = BorderStyle.Thin;
            orderItemHeadRowStyle.BorderBottom = BorderStyle.Thin;
            // append
            orderItemHeadRowStyle.Alignment = HorizontalAlignment.Center;
            orderItemHeadRowStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
            orderItemHeadRowStyle.FillPattern = FillPattern.SolidForeground;

            // 标题新式
            ICellStyle titleStyle = workbook.CreateCellStyle();
            //设置单元格的样式：水平对齐居中
            titleStyle.Alignment = HorizontalAlignment.Center;
            titleStyle.VerticalAlignment = VerticalAlignment.Center;
            //设置边框
            titleStyle.BorderTop = BorderStyle.Thin;
            titleStyle.BorderRight = BorderStyle.Thin;
            titleStyle.BorderLeft = BorderStyle.Thin;
            titleStyle.BorderBottom = BorderStyle.Thin;
            //新建一个字体样式对象
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 14;
            //设置字体加粗样式
            font.Boldweight = short.MaxValue;
            //使用SetFont方法将字体样式添加到单元格样式中 
            titleStyle.SetFont(font);

            //设置单元格的宽度
            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 25 * 256);
            sheet.SetColumnWidth(2, 22 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 22 * 256);
            sheet.SetColumnWidth(5, 27 * 256);

            foreach (var m in model)
            {
                #region 设置表头
                // 在工作表中：建立行，参数为行号，从0计
                IRow row = sheet.CreateRow(currentRowNum);
                row.Height = 30 * 20;

                // 在行中：建立单元格，参数为列号，从0计
                ICell cell = row.CreateCell(0);
                // 设置单元格内容
                var titel = m.SettleAccount.EsAppName + "【" + m.SettleAccount.AppName + "】- 结算单";
                cell.SetCellValue(titel);
                //将新的样式赋给单元格
                cell.CellStyle = titleStyle;

                // 合并单元格 使用上下左右定义CellRangeAddress区域
                CellRangeAddress region = new CellRangeAddress(currentRowNum, currentRowNum, 0, 5);
                sheet.AddMergedRegion(region);
                ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(region, BorderStyle.Thin, HSSFColor.Black.Index);
                #endregion

                #region 订单信息
                currentRowNum++;
                IRow firestOrderRow = sheet.CreateRow(currentRowNum);
                firestOrderRow.Height = commonRowHeight;

                ICell orderCodeCell = firestOrderRow.CreateCell(0);
                orderCodeCell.SetCellValue("结算单号： ");
                orderCodeCell.CellStyle = commonStyle;
                ICell orderCodeValueCell = firestOrderRow.CreateCell(1);
                orderCodeValueCell.SetCellValue(m.SettleAccount.Code);
                orderCodeValueCell.CellStyle = commonStyle;

                ICell orderAmountDateCell = firestOrderRow.CreateCell(2);
                orderAmountDateCell.SetCellValue("结算截止日期： ");
                orderAmountDateCell.CellStyle = commonStyle;
                ICell orderAmountDateValueCell = firestOrderRow.CreateCell(3);
                orderAmountDateValueCell.SetCellValue(m.SettleAccount.AmountDate.ToString("yyyy-MM-dd"));
                orderAmountDateValueCell.CellStyle = commonStyle;

                ICell orderStateCell = firestOrderRow.CreateCell(4);
                orderStateCell.SetCellValue("结算状态：");
                orderStateCell.CellStyle = commonStyle;
                ICell orderStateValueCell = firestOrderRow.CreateCell(5);
                orderStateValueCell.SetCellValue(GetStatusDesc(m.SettleAccount.State));
                orderStateValueCell.CellStyle = commonStyle;

                currentRowNum++;
                IRow secondOrderRow = sheet.CreateRow(currentRowNum);
                secondOrderRow.Height = commonRowHeight;

                ICell orderOrderAmountCell = secondOrderRow.CreateCell(0);
                orderOrderAmountCell.SetCellValue("订单总额：");
                orderOrderAmountCell.CellStyle = commonStyle;
                ICell orderOrderAmountValueCell = secondOrderRow.CreateCell(1);
                orderOrderAmountValueCell.SetCellValue("￥" + m.SettleAccount.OrderAmount.ToString("f2"));
                orderOrderAmountValueCell.CellStyle = commonStyle;

                ICell orderRealAmountCell = secondOrderRow.CreateCell(2);
                orderRealAmountCell.SetCellValue("实收款总额：");
                orderRealAmountCell.CellStyle = commonStyle;
                ICell orderRealAmountValueCell = secondOrderRow.CreateCell(3);
                orderRealAmountValueCell.SetCellValue("￥" + m.SettleAccount.OrderRealAmount.ToString("f2"));
                orderRealAmountValueCell.CellStyle = commonStyle;

                ICell orderSellerAmountCell = secondOrderRow.CreateCell(4);
                orderSellerAmountCell.SetCellValue("商家结算金额：");
                orderSellerAmountCell.CellStyle = commonStyle;
                ICell orderSellerAmountValueCell = secondOrderRow.CreateCell(5);
                orderSellerAmountValueCell.SetCellValue("￥" + m.SettleAccount.SellerAmount.ToString("f2"));
                if (!m.SettleAccount.SettleStatue)
                {
                    orderSellerAmountValueCell.CellStyle = redStyle;
                }
                else
                {
                    orderSellerAmountValueCell.CellStyle = commonStyle;
                }

                currentRowNum++;
                IRow thirdOrderRow = sheet.CreateRow(currentRowNum);
                thirdOrderRow.Height = commonRowHeight;

                ICell orderAppNameCell = thirdOrderRow.CreateCell(0);
                orderAppNameCell.SetCellValue("App名称：");
                orderAppNameCell.CellStyle = commonStyle;
                ICell orderAppNameValueCell = thirdOrderRow.CreateCell(1);
                orderAppNameValueCell.SetCellValue(m.SettleAccount.AppName);
                orderAppNameValueCell.CellStyle = commonStyle;

                ICell orderSellerTypeCell = thirdOrderRow.CreateCell(2);
                orderSellerTypeCell.SetCellValue("商家类型： ");
                orderSellerTypeCell.CellStyle = commonStyle;
                ICell orderSellerTypeValueCell = thirdOrderRow.CreateCell(3);
                orderSellerTypeValueCell.SetCellValue(Jinher.AMP.BTP.Common.MallTypeHelper.GetMallTypeString(m.SettleAccount.SellerType));
                orderSellerTypeValueCell.CellStyle = commonStyle;

                ICell orderBankAccountCell = thirdOrderRow.CreateCell(4);
                orderBankAccountCell.SetCellValue("银行账号：");
                orderBankAccountCell.CellStyle = commonStyle;
                ICell orderBankAccountValueCell = thirdOrderRow.CreateCell(5);
                orderBankAccountValueCell.SetCellValue(m.ChargeAccount.BankAccount);
                orderBankAccountValueCell.CellStyle = commonStyle;

                currentRowNum++;
                IRow fourthOrderRow = sheet.CreateRow(currentRowNum);
                fourthOrderRow.Height = 30 * 20;

                ICell orderAccountNameCell = fourthOrderRow.CreateCell(0);
                orderAccountNameCell.SetCellValue("开户名称：");
                orderAccountNameCell.CellStyle = commonStyle;
                ICell orderAccountNameValueCell = fourthOrderRow.CreateCell(1);
                orderAccountNameValueCell.SetCellValue(m.ChargeAccount.AccountName);
                orderAccountNameValueCell.CellStyle = commonStyle;

                ICell orderBankNameCell = fourthOrderRow.CreateCell(2);
                orderBankNameCell.SetCellValue("开户行名称： ");
                orderBankNameCell.CellStyle = commonStyle;
                ICell orderBankNameValueCell = fourthOrderRow.CreateCell(3);
                orderBankNameValueCell.SetCellValue(m.ChargeAccount.BankName);
                orderBankNameValueCell.CellStyle = commonStyle;
                // 合并单元格 使用上下左右定义CellRangeAddress区域
                var orderRegion = new CellRangeAddress(currentRowNum, currentRowNum, 3, 5);
                sheet.AddMergedRegion(orderRegion);
                ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(orderRegion, BorderStyle.Thin, HSSFColor.Black.Index);

                #endregion

                #region 订单项表头
                currentRowNum++;
                IRow orderItemTitleRow = sheet.CreateRow(currentRowNum);
                orderItemTitleRow.Height = 30 * 20;
                ICell orderItemTitleCell = orderItemTitleRow.CreateCell(0);
                orderItemTitleCell.SetCellValue("订单详情");
                orderItemTitleCell.CellStyle = titleStyle;
                // 合并单元格 使用上下左右定义CellRangeAddress区域
                var orderItemTitleRegion = new CellRangeAddress(currentRowNum, currentRowNum, 0, 5);
                sheet.AddMergedRegion(orderItemTitleRegion);
                ((HSSFSheet)sheet).SetEnclosedBorderOfRegion(orderItemTitleRegion, BorderStyle.Thin, HSSFColor.Black.Index);
                #endregion

                #region 订单项列表
                currentRowNum++;
                IRow orderItemHeadRow = sheet.CreateRow(currentRowNum);
                orderItemHeadRow.Height = 30 * 20;

                ICell orderItemHeadRowCell0 = orderItemHeadRow.CreateCell(0);
                orderItemHeadRowCell0.CellStyle = orderItemHeadRowStyle;
                orderItemHeadRowCell0.SetCellValue("序号");

                ICell orderItemHeadRowCell1 = orderItemHeadRow.CreateCell(1);
                orderItemHeadRowCell1.CellStyle = orderItemHeadRowStyle;
                orderItemHeadRowCell1.SetCellValue("订单编号");

                ICell orderItemHeadRowCell2 = orderItemHeadRow.CreateCell(2);
                orderItemHeadRowCell2.CellStyle = orderItemHeadRowStyle;
                orderItemHeadRowCell2.SetCellValue("下单时间");

                ICell orderItemHeadRowCell3 = orderItemHeadRow.CreateCell(3);
                orderItemHeadRowCell3.CellStyle = orderItemHeadRowStyle;
                orderItemHeadRowCell3.SetCellValue("订单金额");

                ICell orderItemHeadRowCell4 = orderItemHeadRow.CreateCell(4);
                orderItemHeadRowCell4.CellStyle = orderItemHeadRowStyle;
                orderItemHeadRowCell4.SetCellValue("实收款");

                ICell orderItemHeadRowCell5 = orderItemHeadRow.CreateCell(5);
                orderItemHeadRowCell5.CellStyle = orderItemHeadRowStyle;
                orderItemHeadRowCell5.SetCellValue("订单结算金额");

                int i = 0;
                foreach (var o in m.OrderInfoes)
                {
                    i++;
                    currentRowNum++;

                    IRow orderItemRow = sheet.CreateRow(currentRowNum);
                    orderItemRow.Height = 30 * 20;

                    ICell orderItemRowCell0 = orderItemRow.CreateCell(0);
                    orderItemRowCell0.CellStyle = commonStyle;
                    orderItemRowCell0.SetCellValue(i.ToString());

                    ICell orderItemRowCell1 = orderItemRow.CreateCell(1);
                    orderItemRowCell1.CellStyle = commonStyle;
                    orderItemRowCell1.SetCellValue(o.OrderCode);

                    ICell orderItemRowCell2 = orderItemRow.CreateCell(2);
                    orderItemRowCell2.CellStyle = commonStyle;
                    orderItemRowCell2.SetCellValue(o.OrderSubTime.ToString("yyyy-MM-dd"));

                    ICell orderItemRowCell3 = orderItemRow.CreateCell(3);
                    orderItemRowCell3.CellStyle = commonStyle;
                    orderItemRowCell3.SetCellValue("￥" + o.OrderAmount.ToString("f2"));

                    ICell orderItemRowCell4 = orderItemRow.CreateCell(4);
                    orderItemRowCell4.CellStyle = commonStyle;
                    orderItemRowCell4.SetCellValue("￥" + o.OrderRealAmount.ToString("f2"));

                    ICell orderItemRowCell5 = orderItemRow.CreateCell(5);
                    orderItemRowCell5.CellStyle = commonStyle;
                    orderItemRowCell5.SetCellValue("￥" + o.SellerAmount.ToString("f2"));
                }
                #endregion

                currentRowNum = currentRowNum + 3;
            }

            using (MemoryStream memoryStream = new MemoryStream())
            {
                workbook.Write(memoryStream);
                return File(memoryStream.ToArray(), "application/ms-excel", string.Format("结算对账单_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
            }

            //ViewBag.IsMall = isMall;
            //return File(
            //    System.Text.Encoding.UTF8.GetBytes(RenderViewToString(ControllerContext, "ExportTemplate", model)),
            //    "text/html; charset=utf-8",
            //    System.Web.HttpUtility.UrlEncode(string.Format("对账单_{0}.html", DateTime.Now.ToString("yyyyMMddHHmmss"))));
        }

        static string RenderViewToString(ControllerContext context, string viewName, object model)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = ViewEngines.Engines.FindView(context, viewName, null);
            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view, context.Controller.ViewData, context.Controller.TempData, sw);
                view.Render(ctx, sw);
                return sw.ToString();
            }
        }


        /// <summary>
        /// 修改计算结果
        /// </summary>
        /// <param name="dto"></param>
        public ActionResult UpdateSettleStatue(SettleAccountUpdateSettleStatueDto dto)
        {
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.UpdateSettleStatue(dto));
        }
        #endregion

        #region 结算周期
        /// <summary>
        /// 结算周期设置页面
        /// </summary>
        /// <returns></returns>
        [CheckAppId]
        public ActionResult Period()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                ViewBag.ErrorMessage = "未获取到商城ID";
            }
            SettleAccountFacade facade = new SettleAccountFacade();
            var result = facade.GetSettleAccountPeriod(appId);
            if (result.isSuccess)
            {
                ViewBag.NumOfDay = result.Data.NumOfDay;
                ViewBag.UseAfterSalesEndTime = result.Data.UseAfterSalesEndTime ? 1 : 0;
            }
            else
            {
                ViewBag.ErrorMessage = "获取结算周期异常：" + result.Message;
            }
            return View();
        }

        /// <summary>
        /// 保存结算周期
        /// </summary>
        /// <param name="numOfDay"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Period(int useAfterSalesEndTime, int? numOfDay)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            if (useAfterSalesEndTime == 0 && !numOfDay.HasValue)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "请设置结算周期" });
            }
            if (!ZPHSV.Instance.IsAppPavilion(appId))
            {
                return Json(new ResultDTO { isSuccess = false, Message = "只有商城才可以设置结算周期" });
            }
            SettleAccountFacade facade = new SettleAccountFacade();
            return Json(facade.UpdateSettleAccountPeriod(new SettleAccountPeriodDTO { EsAppId = appId, NumOfDay = numOfDay ?? 0, UserId = ContextDTO.LoginUserID, UseAfterSalesEndTime = useAfterSalesEndTime == 1 }));
        }
        #endregion

        private string GetStatusDesc(int status)
        {
            if (status == 0)
            {
                return "待结算";
            }
            else if (status == 1)
            {
                return "等待商家确认";
            }
            else if (status == 2)
            {
                return "待打款";
            }
            else if (status == 3)
            {
                return "已结算";
            }
            return "错误的状态";
        }

        public class ExportViewModel
        {
            public SettleAccountDetailsDTO SettleAccount { get; set; }
            public ChargeAccountDTO ChargeAccount { get; set; }
            public List<SettleAccountOrderDTO> OrderInfoes { get; set; }
        }
    }
}