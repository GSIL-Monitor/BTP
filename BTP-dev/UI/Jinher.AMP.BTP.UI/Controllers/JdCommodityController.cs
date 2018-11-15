using System;
using System.Web.Mvc;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.IBP.Facade;
using Jinher.AMP.BTP.UI.Filters;
using Jinher.JAP.MVC.Controller;
using Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee;
using Jinher.AMP.BTP.Deploy;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Web;
using Jinher.AMP.BTP.Common;
using System.IO;
using Jinher.JAP.Common.Loging;
using NPOI.SS.UserModel;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Jinher.AMP.BTP.BE;
using System.Linq;
using Jinher.AMP.BTP.TPS;
using Jinher.AMP.BTP.Deploy.Enum;
using Jinher.AMP.BTP.TPS.Helper;
using Jinher.AMP.BTP.ISV.Facade;
using System.Diagnostics;

namespace Jinher.AMP.BTP.UI.Controllers
{
    /// <summary>
    /// 京东三期
    /// </summary>
    public class JdCommodityController : BaseController
    {
        [CheckAppId]
        public ActionResult Index()
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return HttpNotFound();
            }
            InnerCategoryFacade innerCatefa = new InnerCategoryFacade();
            var facade = new JdCommodityFacade();
            ViewBag.InnerCategoryList = JsonHelper.JsonSerializer(facade.GetCategories(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId));
            ViewBag.CategoryList = JsonHelper.JsonSerializer(facade.GetCategoryList(Jinher.AMP.YJB.Deploy.CustomDTO.YJBConsts.YJAppId));
            ViewBag.AppId = appId;
            ViewBag.IsYPK = CustomConfig.YPKAppIdList.Contains(appId);
            return View();
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetData(JdCommoditySearchDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            //if (ContextDTO.LoginUserID == Guid.Empty)
            //{
            //    return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            //}
            dto.AppId = appId;
            var facade = new JdCommodityFacade();
            return Json(facade.GetJdCommodityList(dto));
        }
        /// <summary>
        /// 创建商品信息
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(JdCommodityDTO dto)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            //if (ContextDTO.LoginUserID == Guid.Empty)
            //{
            //    return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            //}
            dto.AppId = appId;
            ResultDTO result = new ResultDTO();
            if (ThirdECommerceHelper.IsJingDongDaKeHu(appId))
            {
                var facade = new JdCommodityFacade();
                result = facade.AddJdCommodity(dto);

            }
            else if (ThirdECommerceHelper.IsWangYiYanXuan(appId))
            {
                var facade = new YXCommodityFacade();
                result = facade.AddYXCommodity(dto);
            }
            else if (ThirdECommerceHelper.IsSuNingYiGou(appId))
            {
                //苏宁易购
                var facade = new SNCommodityFacade();
                result = facade.AddSNCommodity(dto);
            }
            else if (ThirdECommerceHelper.IsYiPaiKe(appId))
            {
                //易派客
                var facade = new YPKCommodityFacade();
                result = facade.AddYPKCommodity(dto);
            }
            else
            {
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "非第三方电商" });
            }
            return Json(result);
        }
        /// <summary>
        /// 获取税收编号
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ActionResult SearchSingleCode(JdCommoditySearchDTO dto)
        {
            double taxrate = -1.0;
            int pageIndex = dto.PageIndex;
            int pageSize = dto.PageSize;
            string name = dto.name == null || dto.name == "" ? "" : dto.name.Trim();
            taxrate = dto.taxrate;

            IBP.Facade.CommodityFacade facade = new IBP.Facade.CommodityFacade();
            var retInfo = facade.GetSingleCommodityCode(name, taxrate, pageIndex, pageSize);
            return Json(retInfo);
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public ActionResult DeleteAll(List<Guid> Ids)
        {
            var appId = (Guid)Session["APPID"];
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            //if (ContextDTO.LoginUserID == Guid.Empty)
            //{
            //    return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            //}
            var facade = new JdCommodityFacade();
            return Json(facade.DelJdCommodityAll(Ids));
        }
        /// <summary>
        /// 自动同步京东商品信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public ActionResult AutoSyncCommodity(Guid appId, List<Guid> Ids)
        {
            if (appId == Guid.Empty)
            {
                return Json(new ResultDTO { isSuccess = false, Message = "未获取到商城ID" });
            }
            //if (ContextDTO.LoginUserID == Guid.Empty)
            //{
            //    return Json(new ResultDTO { isSuccess = false, Message = "获取用户信息失败" });
            //}
            ResultDTO result = new ResultDTO();
            if (ThirdECommerceHelper.IsJingDongDaKeHu(appId))
            {
                var facade = new JdCommodityFacade();
                result = facade.AutoSyncCommodityInfo(appId, Ids);

            }
            else if (ThirdECommerceHelper.IsWangYiYanXuan(appId))
            {
                var facade = new YXCommodityFacade();
                result = facade.AutoSyncYXCommodityInfo(appId, Ids);
            }
            else if (ThirdECommerceHelper.IsSuNingYiGou(appId))
            {
                //苏宁易购
                var facade = new SNCommodityFacade();
                result = facade.AutoSyncSNCommodityInfo(appId, Ids);
            }
            else if (ThirdECommerceHelper.IsYiPaiKe(appId))
            {
                //易派客
                var facade = new YPKCommodityFacade();
                result = facade.AutoSyncYPKCommodityInfo(appId, Ids);
            }
            else
            {
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "非第三方电商" });
            }
            return Json(result);
        }
        /// <summary>
        /// 全量同步（易派客）
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        public ActionResult AutoYPKSyncCommodity(Guid appId)
        {
            ResultDTO result = new ResultDTO();
            var facade = new YPKCommodityFacade();
            result = facade.AutoYPKSyncCommodity(appId);
            return Json(result);
        }
        /// <summary>
        /// 导出商品信息
        /// </summary>
        /// <param name="JdCode"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportJdCommodity(string JdCode, Guid AppId)
        {
            JdCommoditySearchDTO dto = new JdCommoditySearchDTO();
            dto.JDCode = JdCode;
            dto.AppId = AppId;

            var facade = new JdCommodityFacade();
            var result = facade.ExportJdCommodityData(dto);
            if (!result.isSuccess)
            {
                return Json(result);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("备注编码", typeof(string));
            dt.Columns.Add("商品类目", typeof(string));
            dt.Columns.Add("商城品类", typeof(string));
            dt.Columns.Add("税收编码", typeof(string));
            dt.Columns.Add("销项税", typeof(string));
            dt.Columns.Add("进项税", typeof(string));
            if (CustomConfig.YPKAppIdList.Contains(AppId))
            {
                dt.Columns.Add("商品售价", typeof(string));
            }
            foreach (var d in result.Data.List)
            {
                if (CustomConfig.YPKAppIdList.Contains(AppId))
                {
                    dt.Rows.Add(d.JDCode, d.CategoryName, d.VideoName, d.TaxClassCode, d.TaxRate, d.InputRax, d.Price);
                }
                else
                {
                    dt.Rows.Add(d.JDCode, d.CategoryName, d.VideoName, d.TaxClassCode, d.TaxRate, d.InputRax);
                }
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("export_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        #region
        /// <summary>
        /// 导入商品信息
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportExcel()
        {
            var appId = (Guid)Session["APPID"];
            var file = Request.Files["file"];
            if (file == null) return Json(new { Success = false, Messages = "请选择文件~" });
            string excelType = GetExcelFileType(file.FileName).ToString();
            if (excelType != "xlsx")
            {
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "只能上传.xlsx格式文件" });
            }
            var dt = ExcelHelper.Import(file.InputStream, excelType);
            if (dt != null && dt.Rows.Count > 0)
            {
                if (CustomConfig.YPKAppIdList.Contains(appId))
                {
                    if (!(dt.Columns.Contains("备注编码") && dt.Columns.Contains("商品类目") && dt.Columns.Contains("商城品类") && dt.Columns.Contains("税收编码") && dt.Columns.Contains("销项税") && dt.Columns.Contains("进项税") && dt.Columns.Contains("商品售价")))
                    {
                        return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "上传文件模板错误~" });
                    }
                }
                else
                {
                    if (!(dt.Columns.Contains("备注编码") && dt.Columns.Contains("商品类目") && dt.Columns.Contains("商城品类") && dt.Columns.Contains("税收编码") && dt.Columns.Contains("销项税") && dt.Columns.Contains("进项税")))
                    {
                        return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "上传文件模板错误~" });
                    }
                }

            }
            else
            {
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "上传文件没有数据~" });
            }
            StringBuilder errorMsg = new StringBuilder(); // 错误信息
            bool flag = true;
            try
            {
                // 1.2解析文件，存放到一个List集合里
                List<JdCommodityDTO> enlist = new List<JdCommodityDTO>();
                var ypkIndex = 2;
                if (CustomConfig.YPKAppIdList.Contains(appId))
                {
                    foreach (DataRow d in dt.Rows)
                    {
                        var e = new JdCommodityDTO();
                        string errorMsgStr = "第" + ypkIndex + "行数据检测异常：";
                        if (string.IsNullOrWhiteSpace(d["备注编码"].ToString()) || string.IsNullOrWhiteSpace(d["商品类目"].ToString()) ||
                            string.IsNullOrWhiteSpace(d["税收编码"].ToString()) || string.IsNullOrEmpty(d["税收编码"].ToString()) || string.IsNullOrWhiteSpace(d["销项税"].ToString()) ||
                            string.IsNullOrWhiteSpace(d["进项税"].ToString()) || string.IsNullOrWhiteSpace(d["商品售价"].ToString()))
                        {
                            errorMsgStr += "行数据不完整";
                            // 若必填项有值未填
                            flag = false;
                            errorMsg.AppendLine(errorMsgStr);
                        }
                        decimal? TaxRate = null;
                        decimal? InputRax = null;
                        decimal? Price = null;
                        if (!string.IsNullOrWhiteSpace(d["销项税"].ToString()))
                        {
                            if (IsInteger(d["销项税"].ToString()))
                            {
                                TaxRate = Convert.ToDecimal(d["销项税"]);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(d["进项税"].ToString()))
                        {
                            if (IsInteger(d["进项税"].ToString()))
                            {
                                InputRax = Convert.ToDecimal(d["进项税"]);
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(d["商品售价"].ToString()))
                        {
                            if (IsInteger(d["商品售价"].ToString()))
                            {
                                Price = Convert.ToDecimal(d["商品售价"]);
                            }
                        }
                        e = new JdCommodityDTO { JDCode = d["备注编码"].ToString(), CategoryName = d["商品类目"].ToString(), VideoName = d["商城品类"].ToString(), TaxClassCode = d["税收编码"].ToString(), TaxRate = TaxRate, InputRax = InputRax, Price = Price, AppId = appId };
                        enlist.Add(e);
                        ypkIndex++;
                    }

                }
                else
                {
                    foreach (DataRow d in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(d["备注编码"].ToString()))
                        {
                            var e = new JdCommodityDTO { JDCode = d["备注编码"].ToString(), CategoryName = d["商品类目"].ToString(), VideoName = d["商城品类"].ToString(), TaxClassCode = d["税收编码"].ToString(), TaxRate = Convert.ToDecimal(d["销项税"]), InputRax = Convert.ToDecimal(d["进项税"]), AppId = appId };
                            enlist.Add(e);
                        }
                    }
                }
                #region 2.对List集合进行有效性校验

                #region 2.1检测必填项是否必填
                //for (int i = 0; i < enlist.Count; i++)
                //{
                //    JdCommodityDTO en = enlist[i];
                //    string errorMsgStr = "第" + (i + 2) + "行数据检测异常：";
                //    bool isHaveNoInputValue = false; // 是否含有未输入项
                //    if (string.IsNullOrEmpty(en.JDCode) || string.IsNullOrEmpty(en.CategoryName) || string.IsNullOrWhiteSpace(en.VideoName) || string.IsNullOrEmpty(en.TaxClassCode) || en.TaxRate == null || en.InputRax == null || en.Price == null)
                //    {
                //        errorMsgStr += "行数据不完整";
                //        isHaveNoInputValue = true;
                //    }
                //    if (isHaveNoInputValue) // 若必填项有值未填
                //    {
                //        flag = false;
                //        errorMsg.AppendLine(errorMsgStr);
                //    }
                //}
                #endregion

                #region 2.2检测Excel中是否有重复对象
                for (int i = 0; i < enlist.Count; i++)
                {
                    JdCommodityDTO enA = enlist[i];
                    for (int j = i + 1; j < enlist.Count; j++)
                    {
                        JdCommodityDTO enB = enlist[j];
                        // 判断必填列是否全部重复
                        if (enA.JDCode == enB.JDCode)
                        {
                            flag = false;
                            errorMsg.AppendLine("第" + (i + 2) + "行与第" + (j + 2) + "行备注编码重复了~");
                        }
                    }
                }

                #endregion

                #region 校验税收编码是否有效
                //取出所有的税收编码
                var TaxRateList = CommodityTaxRate.ObjectSet().ToList();
                var RateList = new List<decimal> { 0, 6, 10, 16 };
                for (int i = 0; i < enlist.Count; i++)
                {
                    JdCommodityDTO en = enlist[i];
                    string errorMsgStr = "第" + (i + 1) + "行数据检测异常：";

                    if (TaxRateList.FirstOrDefault(p => p.Code == en.TaxClassCode) == null)
                    {
                        flag = false;
                        errorMsg.AppendLine("第" + (i + 2) + "行税收编码无效~");
                    }
                    if (RateList.IndexOf(en.InputRax ?? -1) < 0 || RateList.IndexOf(en.TaxRate ?? -1) < 0)
                    {
                        flag = false;
                        errorMsg.AppendLine("第" + (i + 2) + "行税率请填写正整数，如0、6、10、16");
                    }
                    if (CustomConfig.YPKAppIdList.Contains(appId))
                    {
                        if (!IsInteger(en.Price.ToString()))
                        {
                            flag = false;
                            errorMsg.AppendLine("第" + (i + 2) + "行商品售价请填写正确的金额最多保留2位小数~");
                        }
                    }
                }
                #endregion

                #endregion

                // 3.TODO：对List集合持久化存储操作。如：存储到数据库

                // 4.返回操作结果

                if (errorMsg.Length == 0)
                {
                    ResultDTO result = new ResultDTO();
                    if (ThirdECommerceHelper.IsJingDongDaKeHu(appId))
                    {
                        var facade = new JdCommodityFacade();
                        result = facade.ImportJdCommodityData(enlist, appId);

                    }
                    else if (ThirdECommerceHelper.IsWangYiYanXuan(appId))
                    {
                        var facade = new YXCommodityFacade();
                        result = facade.ImportYXCommodityData(enlist, appId);
                    }
                    else if (ThirdECommerceHelper.IsSuNingYiGou(appId))
                    {
                        //苏宁易购
                        var facade = new SNCommodityFacade();
                        result = facade.ImportSNCommodityData(enlist, appId);
                    }
                    else if (ThirdECommerceHelper.IsYiPaiKe(appId))
                    {
                        //易派客
                        var facade = new YPKCommodityFacade();
                        var list = facade.ImportYPKCommodityData(enlist, appId);
                        #region 将异常数据存入excel
                        if (list.Data.ExceptionData.Count > 0)
                        {
                            DataTable dts = new DataTable();
                            dts.Columns.Add("备注编码", typeof(string));
                            dts.Columns.Add("商品类目", typeof(string));
                            dts.Columns.Add("商城品类", typeof(string));
                            dts.Columns.Add("税收编码", typeof(string));
                            dts.Columns.Add("销项税", typeof(string));
                            dts.Columns.Add("进项税", typeof(string));
                            dts.Columns.Add("商品售价", typeof(string));
                            dts.Columns.Add("备注说明", typeof(string));
                            foreach (var d in list.Data.ExceptionData)
                            {
                                dts.Rows.Add(d.SkuId, d.CategoryName, d.VideoName, d.TaxClassCode, d.TaxRate, d.InputRax, d.Price, d.Remark);
                            }
                            var fileName = string.Format("export_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss"));
                            var path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Content\\ExcelTemplate\\ExceptionExcel\\";
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            var fileNow = "/Content/ExcelTemplate/ExceptionExcel/" + fileName;
                            var filePath = Server.MapPath("~" + fileNow);
                            list.Data.FilePath = fileName;
                            var fileContents = Jinher.AMP.BTP.Common.ExcelHelper.Export(dts);
                            WriteBuffToFile(fileContents, filePath);
                            #endregion
                        }
                        result = list;
                    }
                    else
                    {
                        return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "非第三方电商" });
                    }
                    return Json(result);
                }
                else
                {
                    return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = errorMsg.ToString() + "请核对后重新上传" });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("JdCommodityController.ImportExcel 异常", ex);
                return Json(new ResultDTO<Jinher.AMP.BTP.Deploy.CustomDTO.YJEmployee.JdCommoditySearchDTO> { Data = new JdCommoditySearchDTO(), ResultCode = 2, isSuccess = false, Message = "服务异常,稍候再试~" });
            }
        }
        /// <summary>
        /// 判断整数且最多保留两位小数
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected static bool IsInteger(string value)
        {
            string pattern = @"(^[0-9]{1,9}$)|(^[0-9]{1,9}[\.]{1}[0-9]{1,2}$)";
            return System.Text.RegularExpressions.Regex.IsMatch(value, pattern);
        }
        /// <summary>
        /// 写字节数组到文件
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="filePath"></param>
        public static void WriteBuffToFile(byte[] buff, string filePath)
        {
            WriteBuffToFile(buff, 0, buff.Length, filePath);
        }
        /// <summary>
        /// 写字节数组到文件
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="offset">开始位置</param>
        /// <param name="len"></param>
        /// <param name="filePath"></param>
        public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(output);
            writer.Write(buff, offset, len);
            writer.Flush();
            writer.Close();
            output.Close();
        }

        private enum ExcelExtType
        {
            xls,
            xlsx,
        }
        private static Nullable<ExcelExtType> GetExcelFileType(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (!string.IsNullOrWhiteSpace(ext) && (ext.ToLower() == ".xls" || ext.ToLower() == ".xlsx"))
                return ext.ToLower() == ".xls" ? ExcelExtType.xls : ExcelExtType.xlsx;
            else
                return null;
        }
        #endregion
        /// <summary>
        /// 全量获取严选价格信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoUpdateYXCom()
        {
            Jinher.AMP.BTP.TPS.Helper.YXCommodityHelper.AutoUpdateYXComInfo();
            return Content("ok");
        }
        /// <summary>
        /// 全量同步严选库存信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoUpdateYXStock()
        {
            Jinher.AMP.BTP.TPS.Helper.YXCommodityHelper.AutoSyncAllStockNum();
            return Content("ok");
        }
        /// <summary>
        /// 全量获取严选SPU
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoUpdateYXSPU()
        {
            Jinher.AMP.BTP.TPS.Helper.YXCommodityHelper.AutoGetAllSPU();
            return Content("ok");
        }
        /// <summary>
        /// 根据sku获取严选库存
        /// </summary>
        /// <param name="SkuList"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetYxStockByIds(string Skus)
        {
            List<string> SkuList = new List<string>(Skus.Split(','));
            var result = Jinher.AMP.BTP.TPS.Helper.YXCommodityHelper.GetYXStockById(SkuList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据sku获取商品详情
        /// </summary>
        /// <param name="SkuList"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetYxComByIds(string Skus)
        {
            List<string> SkuList = new List<string>(Skus.Split(','));
            var result = Jinher.AMP.BTP.TPS.Helper.YXCommodityHelper.GetYXComInfo(SkuList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //全量同步严选商品价格
        [HttpGet]
        public ActionResult SyncYXStockAndPrice()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            YXCommodityHelper.AutoUpdateYXComInfo();
            timer.Stop();
            LogHelper.Info(string.Format("YXCommodityHelper.AutoUpdateYXComInfo：耗时：{0}。", timer.ElapsedMilliseconds));
            return Content("ok");
        }
        /// <summary>
        /// 全量同步严选价格信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewSysYXPriceInfo()
        {
            var result = Jinher.AMP.BTP.TPS.Helper.YXCommodityHelper.AutoUpdateYXComPrice();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 全量同步严选库存信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult NewSysYXStockInfo()
        {
            var result = Jinher.AMP.BTP.TPS.Helper.YXCommodityHelper.AutoSynYXStockNum();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 全量更新苏宁列表图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpdateComPic()
        {
            var facade = new SNCommodityFacade();
            var result = facade.UpdateComPic();
            if (result.ResultCode == 0)
            {
                return Content("ok");
            }
            else
            {
                return Content("false");
            }
        }
        /// <summary>
        /// 获取京东库存
        /// </summary>
        /// <param name="Skus"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetJDStockByIds(string Skus, string area)
        {
            List<string> SkuList = new List<string>(Skus.Split(','));
            var result = Jinher.AMP.BTP.TPS.JDSV.GetStockById(SkuList, area);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取京东最新价格信息
        /// </summary>
        /// <param name="Skus"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetJDNewPriceByIds(string Skus)
        {
            List<string> SkuList = new List<string>(Skus.Split(','));
            var result = Jinher.AMP.BTP.TPS.JDSV.GetPrice(SkuList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取京东最新价格信息
        /// </summary>
        /// <param name="Skus"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetJDDetailByIds(string Sku)
        {
            var result = Jinher.AMP.BTP.TPS.JDSV.GetJdDetail(Sku);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取严选商品详情信息
        /// </summary>
        /// <param name="Skus"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetYXDetailByIds(string Skus)
        {
            List<string> SkuList = new List<string>(Skus.Split(','));
            var result = Jinher.AMP.BTP.TPS.YXSV.GetComDetailList(SkuList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }        
        /// <summary>
        /// 导出商品信息
        /// </summary>
        /// <param name="JdCode"></param>
        /// <param name="AppId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportPriceChangeOrderItem(Guid appId, string StarTime, string EndTime)
        {
            var facade = new CommodityForYJBFacade();
            var result = facade.GetOrderInfoByAppId(appId, StarTime, EndTime);
            DataTable dt = new DataTable();
            dt.Columns.Add("供应商名称", typeof(string));
            dt.Columns.Add("店铺名称", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("数量", typeof(string));
            dt.Columns.Add("进货价", typeof(string));
            dt.Columns.Add("进货金额", typeof(string));
            dt.Columns.Add("交易状态", typeof(string));
            dt.Columns.Add("订单编码", typeof(string));
            dt.Columns.Add("订单时间", typeof(string));
            dt.Columns.Add("订单运费金额", typeof(string));
            foreach (var d in result)
            {
                dt.Rows.Add(d.Unit, d.PicturesPath, d.Name, d.Number.ToString(), d.CostPrice.ToString(), d.YjbPrice.ToString(), d.State_Value, d.Code,
                    d.SubTime.ToString(), d.FreightPrice.ToString());
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("order_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }

        /// <summary>
        /// 导出苏宁价格差异数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExportSnDiffCostprice()
        {
            var facade = new SNCommodityFacade();
            var result = facade.GetSNDiffCostPrice();
            DataTable dt = new DataTable();
            dt.Columns.Add("商品id", typeof(string));
            dt.Columns.Add("苏宁编码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("易捷进货价", typeof(decimal));
            dt.Columns.Add("苏宁进货价", typeof(decimal));
            dt.Columns.Add("状态", typeof(int));

            foreach (var d in result)
            {
                dt.Rows.Add(d.Id.ToString(), d.JdCode, d.Name, d.CostPrice, d.SNCostprice, d.state);
            }
            return File(Jinher.AMP.BTP.Common.ExcelHelper.Export(dt), "application/vnd.ms-excel", string.Format("SNexport_{0}.xls", DateTime.Now.ToString("yyyyMMddHHmmss")));
        }
        /// <summary>
        /// 全量同步苏宁进货价
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SysSnCostPrice()
        {
            var facade = new SNCommodityFacade();
            var result = facade.SynSNCostPrice();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取苏宁最新价格信息
        /// </summary>
        /// <param name="Skus"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSNNewPriceBySku(string Skus)
        {
            List<string> SkuList = new List<string>(Skus.Split(','));
            var result = Jinher.AMP.BTP.TPS.SuningSV.GetPrice(SkuList);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///获取京东的订单物流信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetJDExpressRouteInfo(string JdOrderId)
        {
            //查询京东物流的订单信息
            var jdwuliu = JdHelper.orderTrack( JdOrderId);
            List<ExpressTraceDTO> result = new List<ExpressTraceDTO>();
            if (jdwuliu != null)
            {
                JArray objson = JArray.Parse(jdwuliu);
                foreach (var item in objson)
                {
                    ExpressTraceDTO entity = new ExpressTraceDTO();
                    entity.Id = Guid.NewGuid();
                    entity.ExpRouteId = Guid.NewGuid();
                    entity.AcceptTime = DateTime.Parse(item["msgTime"].ToString());
                    entity.AcceptStation = item["content"].ToString();
                    entity.Remark = null;
                    result.Add(entity);
                }               
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///获取苏宁的订单物流信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSNExpressRouteInfo(Guid orderItemId)
        {
            SNExpressTraceFacade snExpress = new SNExpressTraceFacade();
            var result = snExpress.GetExpressTrace("", orderItemId.ToString());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///获取网易严选的订单物流信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetYXExpressRouteInfo(Guid orderItemId)
        {
            Guid AppId = new Guid("1d769e14-f870-4b19-82ab-875a9e8678e4");
            var result = ThirdECommerceHelper.GetOrderItemExpressTrace(AppId, orderItemId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取严选库存
        /// </summary>
        /// <param name="skuIds"></param>
        /// <returns></returns>
        public ActionResult GetYXStock(string skuIds)
        {
            var result = YXSV.GetStockNum(skuIds.Split(',').ToList());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
