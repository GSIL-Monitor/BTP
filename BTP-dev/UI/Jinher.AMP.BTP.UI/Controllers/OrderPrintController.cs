using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.AppManage.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.UI.Util;
using System.Text;
using System.IO;
using Jinher.JAP.MVC.UIJquery.DataGrid;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Common;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace Jinher.AMP.BTP.UI.Controllers
{
    public class OrderPrintController : BaseController
    {

        public ActionResult PrintExpressOrder(Guid appId)
        {
            ViewBag.AppId = appId;
            return View();
        }
        public ActionResult PrintInvoice(Guid appId)
        {
            ViewBag.AppId = appId;
            return View();
        }

        /// <summary>
        /// 获取打印订单数据
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public ActionResult GetPrintOrders(string orderIds)
        {
            try
            {
                if (string.IsNullOrEmpty(orderIds)) return Json(new { Result = false, Messages = "订单编号为空" });
                var _orderIds = orderIds.Split(',').ToList();

                List<Guid> printOrderIds = new List<Guid>();
                Guid guidOrderId = Guid.Empty;
                _orderIds.ForEach(r =>
                {
                    if (Guid.TryParse(r, out guidOrderId))
                    {
                        printOrderIds.Add(guidOrderId);
                    }
                });

                OrderPrintFacade orderprintBP = new OrderPrintFacade();
                var orders = orderprintBP.GetPrintOrder(printOrderIds);
                var data = (from o in orders
                            select new
                            {
                                order = new
                                {
                                    OrderId = o.Orders.CommodityOrderId,
                                    Code = o.Orders.Code,
                                    ShipExpCo = o.Orders.ShipExpCo,
                                    ExpOrderNo = o.Orders.ExpOrderNo,
                                    Province = o.Orders.Province,
                                    City = o.Orders.City,
                                    District = o.Orders.District,
                                    ReceiptAddress = o.Orders.ReceiptAddress,
                                    ReceiptPhone = o.Orders.ReceiptPhone,
                                    ReceiptUserName = o.Orders.ReceiptUserName,
                                    RecipientsZipCode = o.Orders.RecipientsZipCode,
                                    ReceiptId = o.Orders.UserId,
                                    State = o.Orders.State,
                                    Details = o.Orders.Details,
                                    SellersRemark = o.Orders.SellersRemark,
                                    PaymentTime = o.Orders.PaymentTime.ToString(),
                                    SubTime = o.Orders.SubTime.ToString(),
                                    RealPrice = o.Orders.RealPrice,
                                    PayType = new PaySourceFacade().GetPaymentName(o.Orders.Payment)
                                },
                                orderItem = (from t in o.OrderItems
                                             select new
                                             {
                                                 CommodityName = t.CommodityName,
                                                 CommodityCode = t.CommodityCode,
                                                 Price = t.Price,
                                                 Number = t.Number,
                                                 CommodityAttributes = t.CommodityAttributes
                                             }).ToList()
                            }).ToList();

                return Json(new { Result = true, Order = data, Messages = "获取数据成功" });
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("获取打印订单数据异常。orderIds：{0}", orderIds), ex);
            }
            return Json(new { Result = false, Messages = "获取打印订单数据异常" });
        }

        /// <summary>
        /// 获取快递单的模板数据
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public ActionResult GetExpressOrderTemplate(Guid appId)
        {
            try
            {
                ExpressOrderTemplateFacade templateBP = new ExpressOrderTemplateFacade();
                var templates = templateBP.GetExpressOrderTemplate(appId);
                return Json(new { Result = true, Template = templates, Messages = "获取数据成功" });
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("获取快递单模板数据异常。appId：{0}", appId), ex);
            }
            return Json(new { Result = false, Messages = "获取快递单模板数据异常" });
        }

        /// <summary>
        /// 获取订单发件人信息
        /// </summary>
        /// <param name="orderIds"></param>
        /// <returns></returns>
        public ActionResult GetOrderSenders(Guid appId)
        {
            try
            {
                var senders = Jinher.AMP.BTP.TPS.ZPHSV.Instance.GetSenderByAppId(appId);
                return Json(new { Result = true, Sender = senders, Messages = "获取数据成功" });
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("获取订单发件人信息异常。appId：{0}", appId), ex);
            }
            return Json(new { Result = false, Messages = "获取订单发件人信息异常" });
        }

        /// <summary>
        /// 打印保存订单信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="shipName"></param>
        /// <param name="autoSend"></param>
        /// <param name="orderJson"></param>
        /// <returns></returns>
        public ActionResult SavePrintOrders(Guid appId, Guid userId, string shipName, int autoSend, string orderJson)
        {
            if (appId == Guid.Empty || userId == Guid.Empty || string.IsNullOrEmpty(shipName) || string.IsNullOrEmpty(orderJson))
                return Json(new { Result = false, Messages = "参数错误" });
            try
            {
                JavaScriptSerializer seri = new JavaScriptSerializer();
                seri.MaxJsonLength = int.MaxValue;
                List<UpdatePrintOrderDTO> orders = seri.Deserialize<List<UpdatePrintOrderDTO>>(orderJson);
                var printOrder = new UpdatePrintDTO()
                {
                    AppId = appId,
                    UserId = userId,
                    ShipName = shipName,
                    PrintType = 0,
                    AutoSend = autoSend == 1,
                    Orders = orders
                };
                OrderPrintFacade orderprintBP = new OrderPrintFacade();
                orderprintBP.SavePrintOrders(printOrder);
                return Json(new { Result = true, Messages = "保存成功" });
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("打印保存订单信息。SavePrintOrders：appId:{0},orderJson:{1}", appId, orderJson), ex);
            }
            return Json(new { Result = false, Messages = "保存失败" });
        }


        /// <summary>
        /// 打印保存订单信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="userId"></param>
        /// <param name="shipName"></param>
        /// <param name="autoSend"></param>
        /// <param name="orderJson"></param>
        /// <returns></returns>
        public ActionResult SavePrintInvoiceOrders(Guid appId, Guid userId, string orderJson)
        {
            if (appId == Guid.Empty || userId == Guid.Empty || string.IsNullOrEmpty(orderJson))
                return Json(new { Result = false, Messages = "参数错误" });
            try
            {
                JavaScriptSerializer seri = new JavaScriptSerializer();
                seri.MaxJsonLength = int.MaxValue;
                List<UpdatePrintOrderDTO> orders = seri.Deserialize<List<UpdatePrintOrderDTO>>(orderJson);
                var printOrder = new UpdatePrintDTO()
                {
                    AppId = appId,
                    UserId = userId,
                    PrintType = 1,
                    Orders = orders
                };
                OrderPrintFacade orderprintBP = new OrderPrintFacade();
                orderprintBP.SavePrintInvoiceOrders(printOrder);
                return Json(new { Result = true, Messages = "保存成功" });
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("打印保存订单信息。SavePrintOrders：appId:{0},orderJson:{1}", appId, orderJson), ex);
            }
            return Json(new { Result = false, Messages = "保存失败" });
        }

        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.Any, Duration = 43200, VaryByCustom = "downprintcontrol")]
        public FileContentResult DownPrintFile(string fileURL = @"jinherOrderPrint_Setup_for_Win32NT.exe", long startPosition = 0)
        {
            if (string.IsNullOrEmpty(fileURL))
            {
                Jinher.JAP.Common.Loging.LogHelper.Warn("提醒：未能从文件服务器上获取文件，所提供的文件URL为：" + fileURL);
                throw new HttpException(404, "文件未找到");
            }
            string ContentType = "exe";
            if (!string.IsNullOrEmpty(fileURL) && fileURL.LastIndexOf(".") != -1)
            {
                ContentType = fileURL.Substring(fileURL.LastIndexOf(".") + 1, (fileURL.Length - fileURL.LastIndexOf(".") - 1));
            }
            HttpResponseBase response = HttpContext.Response;
            response.Headers.Set("Content-Transfer-Encoding", "binary");
            response.Headers.Set("Accept-Ranges", "bytes");
            string fType = "";
            switch (ContentType.ToLower())
            {
                case "gz":
                    fType = "application/x-gzip";
                    break;
                case "tar":
                    fType = "application/x-tar";
                    break;
                case "rar":
                    fType = "application/x-rar";
                    break;
                case "apk":
                    fType = "application/vnd.android.package-archive";
                    break;
                default:
                    fType = "application/octet-stream";
                    break;
            }
            string filePath = string.Format("{0}\\{1}", CustomConfig.DownPrintControlPath, fileURL);
            byte[] fileData = ReadFile(filePath, startPosition);
            if (fileData == null)
            {
                throw new HttpException(404, "下载失败");
            }
            string filename = fileURL.Substring(fileURL.LastIndexOf("/") + 1);
            FileContentResult result = new FileContentResult(fileData, fType);
            result.FileDownloadName = HttpUtility.UrlEncode(filename).Replace("+", "%20").Replace("%2b", "+");
            return result;
        }

        private byte[] ReadFile(string filePath, long startPosition)
        {
            FileStream fs = null;
            byte[] fileData = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] bytes = new byte[fs.Length - startPosition];
                fs.Seek(startPosition, SeekOrigin.Current);
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length - startPosition));//开始读取 
                fileData = bytes;
            }
            catch (Exception ex)
            {
                Jinher.JAP.Common.Loging.LogHelper.Error(string.Format("下载文件{0}出错，异常信息为：{1}", filePath, ex.Message + ex.StackTrace));
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
            }
            return fileData;
        }

    }
}
