using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Jinher.AMP.BTP.BE;
using Jinher.AMP.BTP.Common;
using Jinher.AMP.BTP.Deploy.CustomDTO;
using Jinher.AMP.BTP.TPS.eInvoService;
using Jinher.JAP.BaseApp.FileServer.Deploy.CustomDTO;
using Jinher.JAP.Common.Loging;
using Jinher.JAP.PL;
using O2S.Components.PDFRender4NET;

namespace Jinher.AMP.BTP.TPS.Invoic
{
    //jh 代表正常开票
    //tk 代表全额退款开红票
    //pr 代表部分金额退款之后，重新开的正常票
    /// <summary>
    /// 外部接口
    /// </summary>
    public class InvoicManage
    {
        /// <summary>    
        /// 创建节点    
        /// </summary>    
        /// <param name="xmlDoc"></param>  xml文档  
        /// <param name="parentNode"></param>父节点    
        /// <param name="name"></param>  节点名  
        /// <param name="value"></param>  节点值  
        ///   
        private void CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }

        /// <summary>
        /// 创建开票接口所需xml节点
        /// </summary>
        /// <param name="commodityOrder"></param>
        /// <param name="isRefund">0正常开票 1全额退款 2部分退款</param>
        /// <returns></returns>
        private string CreateXmlFile(CommodityOrder commodityOrder, int isRefund)
        {
            try
            {
                Invoice invoice = Invoice.ObjectSet().FirstOrDefault(t => t.CommodityOrderId == commodityOrder.Id);
                var orderItems = OrderItem.ObjectSet().Where(t => t.CommodityOrderId == commodityOrder.Id);

                XmlDocument xmlDoc = new XmlDocument();
                //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDoc.AppendChild(node);
                //创建根节点  
                XmlNode root = xmlDoc.CreateElement("InvoInfo");
                xmlDoc.AppendChild(root);
                if (isRefund == 0)
                {
                    CreateNode(xmlDoc, root, "swno", "jh" + commodityOrder.Code); //流水号
                }
                else if (isRefund == 1)
                {
                    CreateNode(xmlDoc, root, "swno", "tk" + commodityOrder.Code); //流水号                    
                }
                else if (isRefund == 2)
                {
                    CreateNode(xmlDoc, root, "swno", "pr" + commodityOrder.Code); //流水号                    
                }
                CreateNode(xmlDoc, root, "saleTax", CustomConfig.saleTax); //销方税号    测试税号：110101TRDX8RQU1
                if (invoice.InvoiceType == 1)
                {
                    CreateNode(xmlDoc, root, "custName", "个人"); //购方名称 
                    CreateNode(xmlDoc, root, "custType", "03"); //购货方企业类型 03：个人
                }
                else if (invoice.InvoiceType == 2)
                {
                    CreateNode(xmlDoc, root, "custName", invoice.InvoiceTitle); //购方名称
                    CreateNode(xmlDoc, root, "custTaxNo", invoice.Code); //购方税号
                    CreateNode(xmlDoc, root, "custType", "01"); //购货方企业类型 01：企业
                }
                CreateNode(xmlDoc, root, "custTelephone", invoice.ReceiptPhone); //购方固定电话
                CreateNode(xmlDoc, root, "custEmail", invoice.ReceiptEmail); //购方邮箱
                CreateNode(xmlDoc, root, "invType", "3"); //发票类型 电子票（3）
                CreateNode(xmlDoc, root, "specialRedFlag", "0"); //特殊冲红标志 0：正常冲红（电子发票）
                if (isRefund == 0 || isRefund == 2)
                {
                    CreateNode(xmlDoc, root, "billType", "1");//开票类型 1：正票 
                    CreateNode(xmlDoc, root, "operationCode", "10"); //操作代码 10：正票正常开具
                }
                else if (isRefund == 1)
                {
                    HTJSInvoice htjsInvoice = HTJSInvoice.FindByID(commodityOrder.Id);
                    CreateNode(xmlDoc, root, "invoMemo", "对应正数发票代码:" + htjsInvoice.Fpdm + "号码:" + htjsInvoice.Fphm + "");//备注 开具负数发票时，必须在备注中注明“对应正数发票代码：XXXXXXXXX 号码：YYYYYYYYY”
                    CreateNode(xmlDoc, root, "thdh", commodityOrder.Code);//退货单号
                    CreateNode(xmlDoc, root, "billType", "2");//开票类型 2：红票
                    CreateNode(xmlDoc, root, "operationCode", "21"); //操作代码 21：错票重开红票
                    CreateNode(xmlDoc, root, "yfpdm", htjsInvoice.Fpdm);//原发票代码
                    CreateNode(xmlDoc, root, "yfphm", htjsInvoice.Fphm);//原发票号码
                    CreateNode(xmlDoc, root, "chyy", "退款"); //冲红原因
                }
                CreateNode(xmlDoc, root, "kpy", CustomConfig.kpy); //开票员
                CreateNode(xmlDoc, root, "sky", CustomConfig.kpy);//收款员
                CreateNode(xmlDoc, root, "fhr", CustomConfig.kpy); //复核人   

                XmlNode nodeOrders = xmlDoc.CreateNode(XmlNodeType.Element, "Orders", null);
                XmlNode nodeOrder = xmlDoc.CreateNode(XmlNodeType.Element, "Order", null);
                CreateNode(xmlDoc, nodeOrder, "billNo", commodityOrder.Code); //订单号

                XmlNode nodeItems = xmlDoc.CreateNode(XmlNodeType.Element, "Items", null);
                //正常开票
                if (isRefund == 0 || isRefund == 2)
                {
                    foreach (var item in orderItems)
                    {
                        Commodity commodity = Commodity.FindByID(item.CommodityId);
                        XmlNode nodeItem = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        string commodityName = commodity.Name;
                        if (commodityName.Length > 25)
                        {
                            commodityName = commodityName.Substring(0, 24);
                        }
                        CreateNode(xmlDoc, nodeItem, "name", commodityName); //商品名称
                        CreateNode(xmlDoc, nodeItem, "code", commodity.TaxClassCode); //商品编号（税收分类编码）
                        CreateNode(xmlDoc, nodeItem, "unit", commodity.Unit); //计量单位
                        var dtaxRate = Math.Round((decimal)(commodity.TaxRate / 100), 2);
                        CreateNode(xmlDoc, nodeItem, "taxRate", Convert.ToString(dtaxRate, CultureInfo.InvariantCulture));//销项税 保留两位小数
                        //yjbjOrderItem.PayMoney为姜少辉计算的该订单下该商品的“实际支付总金额（含税）”
                        YJBJOrderItem yjbjOrderItem = YJBJOrderItem.ObjectSet().FirstOrDefault(t => t.OrderId == commodityOrder.Id && t.CommodityId == commodity.Id && t.RefundMoney <= 0);
                        if (yjbjOrderItem != null)
                        {
                            //有优惠开折扣行 0：正常行 2：被折扣行
                            if (yjbjOrderItem.DiscountMoney > 0)
                            {
                                CreateNode(xmlDoc, nodeItem, "lineType", "2");
                                CreateNode(xmlDoc, nodeItem, "taxPrice", Convert.ToString(Math.Round(Math.Round(yjbjOrderItem.ShouldMoney, 2) / item.Number, 2), CultureInfo.InvariantCulture)); //单价
                                CreateNode(xmlDoc, nodeItem, "quantity", Convert.ToString(item.Number)); //数量
                                CreateNode(xmlDoc, nodeItem, "totalAmount", Convert.ToString(Math.Round(yjbjOrderItem.ShouldMoney, 2), CultureInfo.InvariantCulture));
                            }
                            else
                            { 
                                //2018-06-20添加抵用券去除的需求
                                var taxPrice = Math.Round(Math.Round(yjbjOrderItem.PayMoney, 2) / item.Number, 2);
                                var totalAmount = Math.Round(yjbjOrderItem.PayMoney, 2);
                                if (item.YJCouponPrice != null && item.YJCouponPrice > 0)
                                {
                                    taxPrice = taxPrice - Math.Round(Math.Round((decimal)item.YJCouponPrice, 2) / item.Number, 2);
                                    totalAmount = totalAmount - Math.Round((decimal)item.YJCouponPrice, 2);
                                }
                                CreateNode(xmlDoc, nodeItem, "lineType", "0");
                                CreateNode(xmlDoc, nodeItem, "taxPrice", Convert.ToString(taxPrice, CultureInfo.InvariantCulture)); //单价
                                CreateNode(xmlDoc, nodeItem, "quantity", Convert.ToString(item.Number)); //数量
                                CreateNode(xmlDoc, nodeItem, "totalAmount", Convert.ToString(totalAmount, CultureInfo.InvariantCulture));//合计
                            }
                        }
                        else
                        {
                            LogHelper.Info(string.Format("创建开票接口yjbjOrderItem为null，commodityOrder.Id{0},commodity.Id:{1}", commodityOrder.Id, commodity.Id));
                            return "";
                        }

                        CreateNode(xmlDoc, nodeItem, "yhzcbs", "0"); //税收优惠政策标志 0：不使用
                        nodeItems.AppendChild(nodeItem);

                        //如果有优惠 单起一行作为优惠行 yjbjOrderItem.ShouldMoney为姜少辉计算的该订单下该商品的“优惠总金额”
                        //或者有部分退款金额 则一起算到优惠价中
                        var sumDiscountMoney = yjbjOrderItem.DiscountMoney;
                        YJBJOrderItem refundyjbjOrderItem = YJBJOrderItem.ObjectSet().FirstOrDefault(t => t.OrderId == commodityOrder.Id && t.CommodityId == commodity.Id && t.RefundMoney > 0);
                        if (refundyjbjOrderItem != null)
                        {
                            sumDiscountMoney = sumDiscountMoney + refundyjbjOrderItem.RefundMoney;
                        }
                        else
                        {
                            if (isRefund == 2)
                            {
                                //部分退款没有拿到退款额 暂时不开票
                                LogHelper.Info(string.Format("部分退款没有拿到退款额 暂时不开票，commodityOrder.Id{0},commodity.Id:{1}", commodityOrder.Id, commodity.Id));
                                return "";
                            }
                        }
                        if (sumDiscountMoney > 0)
                        {
                            XmlNode fnodeItem = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                            CreateNode(xmlDoc, fnodeItem, "name", commodityName); //商品名称
                            CreateNode(xmlDoc, fnodeItem, "code", commodity.TaxClassCode); //商品编号（税收分类编码）
                            CreateNode(xmlDoc, fnodeItem, "lineType", "1"); //发票行性质 1：折扣行
                            CreateNode(xmlDoc, fnodeItem, "unit", "件"); //计量单位
                            CreateNode(xmlDoc, fnodeItem, "taxRate", Convert.ToString(dtaxRate, CultureInfo.InvariantCulture)); //销项税 保留两位小数
                            CreateNode(xmlDoc, fnodeItem, "taxPrice", Convert.ToString(sumDiscountMoney, CultureInfo.InvariantCulture)); //单价
                            CreateNode(xmlDoc, fnodeItem, "quantity", "-1"); //数量
                            CreateNode(xmlDoc, fnodeItem, "totalAmount", Convert.ToString(-sumDiscountMoney, CultureInfo.InvariantCulture));
                            CreateNode(xmlDoc, fnodeItem, "yhzcbs", "0"); //税收优惠政策标志 0：不使用
                            nodeItems.AppendChild(fnodeItem);
                        }
                    }
                    if (commodityOrder.Freight > 0)
                    {
                        //运费单独开一行
                        XmlNode fnodeItem = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        CreateNode(xmlDoc, fnodeItem, "name", "邮费"); //商品名称
                        CreateNode(xmlDoc, fnodeItem, "code", "3020101020000000000"); //商品编号（税收分类编码）
                        CreateNode(xmlDoc, fnodeItem, "lineType", "0"); //发票行性质 0：正常行
                        CreateNode(xmlDoc, fnodeItem, "unit", "件"); //计量单位
                        CreateNode(xmlDoc, fnodeItem, "taxRate", Convert.ToString("0.11", CultureInfo.InvariantCulture));//销项税 保留两位小数
                        CreateNode(xmlDoc, fnodeItem, "taxPrice", Convert.ToString(commodityOrder.Freight, CultureInfo.InvariantCulture)); //单价
                        CreateNode(xmlDoc, fnodeItem, "quantity", "1"); //数量
                        CreateNode(xmlDoc, fnodeItem, "totalAmount", Convert.ToString(commodityOrder.Freight, CultureInfo.InvariantCulture));
                        CreateNode(xmlDoc, fnodeItem, "yhzcbs", "0"); //税收优惠政策标志 0：不使
                        nodeItems.AppendChild(fnodeItem);
                    }
                }
                else if (isRefund == 1)
                {
                    foreach (var item in orderItems)
                    {
                        Commodity commodity = Commodity.FindByID(item.CommodityId);
                        XmlNode nodeItem = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        string commodityName = commodity.Name;
                        if (commodityName.Length > 25)
                        {
                            commodityName = commodityName.Substring(0, 24);
                        }
                        CreateNode(xmlDoc, nodeItem, "name", commodityName); //商品名称
                        CreateNode(xmlDoc, nodeItem, "code", commodity.TaxClassCode); //商品编号（税收分类编码）
                        CreateNode(xmlDoc, nodeItem, "unit", commodity.Unit);//计量单位
                        var dtaxRate = Math.Round((decimal)(commodity.TaxRate / 100), 2);
                        CreateNode(xmlDoc, nodeItem, "taxRate", Convert.ToString(dtaxRate, CultureInfo.InvariantCulture)); //销项税 保留两位小数

                        Decimal discountMoney = 0;
                        var yjbjOrderItem = YJBJOrderItem.ObjectSet().FirstOrDefault(t => t.OrderId == commodityOrder.Id && t.CommodityId == commodity.Id);
                        if (yjbjOrderItem != null)
                        {
                            //有优惠开折扣行 0：正常行 2：被折扣行
                            if (yjbjOrderItem.DiscountMoney > 0)
                            {
                                CreateNode(xmlDoc, nodeItem, "lineType", "2");
                                CreateNode(xmlDoc, nodeItem, "taxPrice", Convert.ToString(Math.Round(Math.Round(yjbjOrderItem.ShouldMoney, 2) / item.Number, 2), CultureInfo.InvariantCulture)); //单价
                                CreateNode(xmlDoc, nodeItem, "quantity", Convert.ToString(-item.Number)); //数量
                                CreateNode(xmlDoc, nodeItem, "totalAmount", Convert.ToString(-Math.Round(yjbjOrderItem.ShouldMoney, 2), CultureInfo.InvariantCulture));  //含税金额 红票是为负
                            }
                            else
                            {
                                //2018-06-20添加抵用券去除的需求
                                var taxPrice = Math.Round(Math.Round(yjbjOrderItem.PayMoney, 2) / item.Number, 2);
                                var totalAmount = Math.Round(yjbjOrderItem.PayMoney, 2);
                                if (item.YJCouponPrice != null && item.YJCouponPrice > 0)
                                {
                                    taxPrice = taxPrice - Math.Round(Math.Round((decimal)item.YJCouponPrice, 2) / item.Number, 2);
                                    totalAmount = totalAmount - Math.Round((decimal)item.YJCouponPrice, 2);
                                }
                                CreateNode(xmlDoc, nodeItem, "lineType", "0");
                                CreateNode(xmlDoc, nodeItem, "taxPrice", Convert.ToString(taxPrice, CultureInfo.InvariantCulture)); //单价
                                CreateNode(xmlDoc, nodeItem, "quantity", Convert.ToString(-item.Number)); //数量
                                CreateNode(xmlDoc, nodeItem, "totalAmount", Convert.ToString(-totalAmount, CultureInfo.InvariantCulture));//合计
                            }
                        }
                        else
                        {
                            LogHelper.Info(string.Format("创建开票接口yjbjOrderItem为null，commodityOrder.Id{0},commodity.Id:{1}", commodityOrder.Id, commodity.Id));
                            return "";
                        }

                        CreateNode(xmlDoc, nodeItem, "yhzcbs", "0"); //税收优惠政策标志 0：不使用
                        nodeItems.AppendChild(nodeItem);

                        //如果有优惠 单起一行作为优惠行 yjbjOrderItem.ShouldMoney为姜少辉计算的该订单下该商品的“优惠总金额”
                        //或者有部分退款金额 则一起算到优惠价中
                        var sumDiscountMoney = yjbjOrderItem.DiscountMoney;
                        YJBJOrderItem refundyjbjOrderItem = YJBJOrderItem.ObjectSet().FirstOrDefault(t => t.OrderId == commodityOrder.Id && t.CommodityId == commodity.Id && t.RefundMoney > 0);
                        if (refundyjbjOrderItem != null)
                        {
                            sumDiscountMoney = sumDiscountMoney + refundyjbjOrderItem.RefundMoney;
                        }
                        else
                        {
                            if (isRefund == 2)
                            {
                                //部分退款没有拿到退款额 暂时不开票
                                LogHelper.Info(string.Format("部分退款没有拿到退款额 暂时不开票，commodityOrder.Id{0},commodity.Id:{1}", commodityOrder.Id, commodity.Id));
                                return "";
                            }
                        }
                        //如果有优惠 单起一行作为优惠行
                        if (sumDiscountMoney > 0)
                        {
                            XmlNode fnodeItem = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                            CreateNode(xmlDoc, fnodeItem, "name", commodityName); //商品名称
                            CreateNode(xmlDoc, fnodeItem, "code", commodity.TaxClassCode); //商品编号（税收分类编码）
                            CreateNode(xmlDoc, fnodeItem, "lineType", "1"); //发票行性质 1：折扣行
                            CreateNode(xmlDoc, fnodeItem, "unit", "件");//计量单位
                            CreateNode(xmlDoc, fnodeItem, "taxRate", Convert.ToString(dtaxRate, CultureInfo.InvariantCulture)); //销项税 保留两位小数
                            CreateNode(xmlDoc, fnodeItem, "taxPrice", Convert.ToString(sumDiscountMoney, CultureInfo.InvariantCulture));//单价
                            CreateNode(xmlDoc, fnodeItem, "quantity", "1"); //数量
                            CreateNode(xmlDoc, fnodeItem, "totalAmount", Convert.ToString(sumDiscountMoney, CultureInfo.InvariantCulture)); //含税金额 红票是为负
                            CreateNode(xmlDoc, fnodeItem, "yhzcbs", "0"); //税收优惠政策标志 0：不使用
                            nodeItems.AppendChild(fnodeItem);
                        }
                    }
                    if (commodityOrder.Freight > 0)
                    {
                        //运费单独开一行
                        XmlNode fnodeItem = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
                        CreateNode(xmlDoc, fnodeItem, "name", "邮费"); //商品名称
                        CreateNode(xmlDoc, fnodeItem, "code", "3020101020000000000"); //商品编号（税收分类编码）
                        CreateNode(xmlDoc, fnodeItem, "lineType", "0"); //发票行性质 0：正常行
                        CreateNode(xmlDoc, fnodeItem, "unit", "件");//计量单位
                        CreateNode(xmlDoc, fnodeItem, "taxRate", Convert.ToString("0.11", CultureInfo.InvariantCulture)); //销项税 保留两位小数
                        CreateNode(xmlDoc, fnodeItem, "taxPrice", Convert.ToString(commodityOrder.Freight, CultureInfo.InvariantCulture));//单价
                        CreateNode(xmlDoc, fnodeItem, "quantity", "-1"); //数量
                        CreateNode(xmlDoc, fnodeItem, "totalAmount", Convert.ToString(-commodityOrder.Freight, CultureInfo.InvariantCulture)); //含税金额 红票是为负
                        CreateNode(xmlDoc, fnodeItem, "yhzcbs", "0"); //税收优惠政策标志 0：不使用
                        nodeItems.AppendChild(fnodeItem);
                    }
                }

                nodeOrder.AppendChild(nodeItems);
                nodeOrders.AppendChild(nodeOrder);
                root.AppendChild(nodeOrders);

                LogHelper.Info(string.Format("创建开票接口所需xml节点。电子发票订单号：{0},xmlContent:{1}", commodityOrder.Id, xmlDoc.InnerXml));
                return xmlDoc.InnerXml;
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("创建开票接口所需xml节点。接口异常。订单号:{0}", commodityOrder.Id), ex);
                return "";
            }
        }

        ///// <summary>
        ///// 调用开票接口保存相关的返回信息
        ///// </summary>
        ///// <returns></returns>
        //public void aaaaaa()
        //{
        //    CommodityOrder commodityOrder = CommodityOrder.FindByID(new Guid("37F2A865-67BE-49F7-8C56-E1AD749A74E3"));
        //    ContextSession contextSession = ContextFactory.CurrentThreadContext;
        //    DownloadInvoic(commodityOrder, false, contextSession);
        //}

        /// <summary>
        /// 调用开票接口保存相关的返回信息 0正常开票 1全额退款 2部分退款
        /// </summary>
        /// <param name="contextSession"></param>
        /// <param name="commodityOrder"></param>
        /// <param name="isRefund">0正常开票 1全额退款 2部分退款</param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO CreateInvoic(ContextSession contextSession, CommodityOrder commodityOrder, int isRefund)
        {
            LogHelper.Info(string.Format("开始进入开票接口。订单ID（commodityOrder.id：{0}）, 是否是开正票（isRefund:{1}）", commodityOrder.Id, isRefund));
            ResultDTO resultDto = new ResultDTO() { ResultCode = 1 };
            try
            {
                string fMsgCode = "";
                string fMsg = "";

                string strXml = CreateXmlFile(commodityOrder, isRefund);
                if (strXml == "")
                {
                    resultDto.Message = "保存失败";
                    return resultDto;
                }
                //EInvoiceServiceClient eInvoice = new EInvoiceServiceClient("EInvoiceServicePort");
                //var returnInfo = eInvoice.submitEInvoiceInfo(strXml);
                //LogHelper.Info(string.Format("开票接口返回xml结果。XML：{0}", returnInfo));
                //<ReturnMsg> <msgCode>0000</msgCode> <msg>发票开具数据保存成功</msg> </ReturnMsg>
                Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO returnInfo = YJBJMQSV.CreateInvoic(strXml);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(returnInfo.Message);
                XmlNode rootNode = xmlDoc.SelectSingleNode("ReturnMsg");
                if (rootNode != null)
                    foreach (XmlNode xxNode in rootNode.ChildNodes)
                    {
                        if ("msgCode".Equals(xxNode.Name))
                        {
                            fMsgCode = xxNode.InnerText;
                        }
                        else if ("msg".Equals(xxNode.Name))
                        {
                            fMsg = xxNode.InnerText;
                        }
                    }

                LogHelper.Info(string.Format("开票接口返回xml结果。fMsgCode：{0}，fMsg：{1}", fMsgCode, fMsg));

                HTJSInvoice htjsInvoice = HTJSInvoice.FindByID(commodityOrder.Id);
                //新增 
                if (htjsInvoice == null)
                {
                    htjsInvoice = new HTJSInvoice
                    {
                        Id = commodityOrder.Id,
                        SubId = commodityOrder.SubId,
                        SubTime = DateTime.Now,
                        SwNo = "jh" + commodityOrder.Code,
                        RefundType = 0,
                        ModifiedOn = DateTime.Now,
                        FMsgCode = fMsgCode,
                        FMsg = fMsg,
                        SMsgCode = "1111",//给下载发票一个默认错误码
                        EntityState = EntityState.Added
                    };
                }
                //补发 处理返回错误码的情况
                else
                {
                    if (isRefund == 0)
                    {
                        htjsInvoice.SwNo = "jh" + commodityOrder.Code;
                    }
                    else if (isRefund == 1)
                    {
                        htjsInvoice.SwNo = "tk" + commodityOrder.Code;
                        htjsInvoice.SMsgCode = "1111"; //给下载发票一个默认错误码
                    }
                    else if (isRefund == 2)
                    {
                        htjsInvoice.SwNo = "pr" + commodityOrder.Code;
                        htjsInvoice.SMsgCode = "1111"; //给下载发票一个默认错误码
                    }
                    htjsInvoice.RefundType = isRefund;
                    htjsInvoice.ModifiedOn = DateTime.Now;
                    htjsInvoice.FMsgCode = fMsgCode;
                    htjsInvoice.FMsg = fMsg;
                    htjsInvoice.EntityState = EntityState.Modified;
                }
                contextSession.SaveObject(htjsInvoice);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("调用开票接口保存相关的返回信息异常。订单id：{0}", commodityOrder.Id), ex);
                resultDto.Message = "处理异常";
            }
            return resultDto;
        }


        /// <summary>
        /// 下载电子发票
        /// </summary>
        /// <param name="commodityOrder"></param>
        /// <param name="isRefund">0正常开票 1全额退款 2部分退款</param>
        /// <param name="contextSession"></param>
        /// <returns></returns>
        [BTPAopLogMethod]
        public ResultDTO DownloadInvoic(CommodityOrder commodityOrder, int isRefund, ContextSession contextSession)
        {
            ResultDTO resultDto = new ResultDTO() { ResultCode = 1 };
            try
            {
                string fpdm = "";
                string fphm = "";
                string kprq = "";
                string pdfContent = "";
                string pdfMd5 = "";
                string msgCode = "";
                string msg = "";

                XmlDocument xmlDoc = new XmlDocument();
                //创建类型声明节点  
                XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
                xmlDoc.AppendChild(node);
                //创建根节点  
                XmlNode root = xmlDoc.CreateElement("InvoInfo");
                xmlDoc.AppendChild(root);
                if (isRefund == 0)
                {
                    CreateNode(xmlDoc, root, "swno", "jh" + commodityOrder.Code); //流水号 
                }
                else if (isRefund == 1)
                {
                    CreateNode(xmlDoc, root, "swno", "tk" + commodityOrder.Code); //流水号 
                }
                else if (isRefund == 2)
                {
                    CreateNode(xmlDoc, root, "swno", "pr" + commodityOrder.Code); //流水号 
                }
                CreateNode(xmlDoc, root, "saleTax", CustomConfig.saleTax); //销方税号    测试税号：110101TRDX8RQU1
                LogHelper.Info(string.Format("下载电子发票接口所需xml节点。电子发票订单号：{0},xmlContent:{1}", commodityOrder.Id, xmlDoc.InnerXml));

                string strXml = xmlDoc.InnerXml;
                //EInvoiceServiceClient eInvoice = new EInvoiceServiceClient("EInvoiceServicePort");
                //var returnInfo = eInvoice.downloadEInvoiceInfo(strXml);
                //LogHelper.Info(string.Format("下载电子发票接口返回xml结果。XML：{0}", returnInfo));
                Jinher.AMP.YJBJMQ.Deploy.CustomDTO.ResultDTO returnInfo = YJBJMQSV.DownloadInvoic(strXml);

                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(returnInfo.Message);
                XmlNode rootNode = xmlDoc.SelectSingleNode("DownloadInfo");
                if (rootNode != null)
                    foreach (XmlNode xxNode in rootNode.ChildNodes)
                    {
                        if ("fpdm".Equals(xxNode.Name))
                        {
                            fpdm = xxNode.InnerText;
                        }
                        else if ("fphm".Equals(xxNode.Name))
                        {
                            fphm = xxNode.InnerText;
                        }
                        else if ("kprq".Equals(xxNode.Name))
                        {
                            kprq = xxNode.InnerText;
                        }
                        else if ("pdfContent".Equals(xxNode.Name))
                        {
                            pdfContent = xxNode.InnerText;
                        }
                        else if ("pdfMd5".Equals(xxNode.Name))
                        {
                            pdfMd5 = xxNode.InnerText;
                        }
                    }

                XmlNode returnMsgnode = xmlDoc.SelectSingleNode("DownloadInfo/returnMsg");
                if (returnMsgnode != null)
                    foreach (XmlNode xxNode in returnMsgnode.ChildNodes)
                    {
                        if ("msgCode".Equals(xxNode.Name))
                        {
                            msgCode = xxNode.InnerText;
                        }
                        else if ("msg".Equals(xxNode.Name))
                        {
                            msg = xxNode.InnerText;
                        }
                    }

                LogHelper.Info(string.Format(
                        "下载电子发票接口返回xml结果。fpdm：{0}，fphm：{1}，kprq：{2}，pdfContent：{3}，pdfMd5：{4}，msgCode：{5}，msg：{6},电子发票订单号:{7}",
                        fpdm, fphm, kprq, pdfContent, pdfMd5, msgCode, msg, commodityOrder.Id));

                HTJSInvoice htjsInvoice = HTJSInvoice.FindByID(commodityOrder.Id);
                htjsInvoice.Fpdm = fpdm;
                htjsInvoice.Fphm = fphm;
                htjsInvoice.Kprq = Convert.ToDateTime(kprq);
                htjsInvoice.PdfContent = pdfContent;
                htjsInvoice.PdfMd5 = pdfMd5;
                htjsInvoice.SMsgCode = msgCode;
                htjsInvoice.SMsg = msg;
                htjsInvoice.EntityState = EntityState.Modified;
                contextSession.SaveObject(htjsInvoice);
            }
            catch (Exception ex)
            {
                LogHelper.Error(string.Format("调用下载电子发票接口保存相关的返回信息异常。订单id：{0}", commodityOrder.Id), ex);
                resultDto.Message = "处理异常";
            }
            return resultDto;
        }

        /// <summary>
        /// 解析下载发票信息
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="isRefund">0正常开票 1全额退款 2部分退款</param>
        /// <param name="pdfPath"></param>
        /// <returns></returns>
        public string AnalyzeInvoicInfo(string orderCode, int isRefund, out string pdfPath)
        {
            pdfPath = "";
            //string imgPath = Path.GetTempPath() + "Pic\\" + orderCode + ".Png";
            var filePath = Path.GetTempPath() + "Pic\\";
            //不存在文件夹
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = filePath + orderCode + ".pdf";
            HTJSInvoice htjsInvoice = null;
            if (isRefund == 0)
            {
                htjsInvoice = HTJSInvoice.ObjectSet().FirstOrDefault(t => t.SwNo == "jh" + orderCode);
            }
            else if (isRefund == 1)
            {
                htjsInvoice = HTJSInvoice.ObjectSet().FirstOrDefault(t => t.SwNo == "tk" + orderCode);
            }
            else if (isRefund == 2)
            {
                htjsInvoice = HTJSInvoice.ObjectSet().FirstOrDefault(t => t.SwNo == "pr" + orderCode);
            }
            if (htjsInvoice != null)
            {
                byteArrayToImage(htjsInvoice.PdfContent, filePath);
                pdfPath = filePath;
            }
            //ConvertPdf2Image(filePath, Path.GetTempPath() + "Pic\\", orderCode, 1, 1, ImageFormat.Png, Definition.Two);
            //LogHelper.Error(string.Format("下载发票pdf文件转为png图片，服务器临时路径为：{0}", imgPath));
            return UploadFile(pdfPath);
        }

        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="pdfContent">字节数组</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>图片</returns>
        private void byteArrayToImage(string pdfContent, string filePath)
        {
            byte[] bytes = Convert.FromBase64String(pdfContent);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                FileStream fs = new FileStream(filePath, FileMode.Append);
                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 上传到图片服务器
        /// </summary>
        /// <param name="realPath">文件全路径</param>
        /// <returns></returns> 
        private string UploadFile(string realPath)
        {
            var imageUrl = string.Empty;
            if (File.Exists(realPath))
            {
                using (FileStream stream = new FileStream(realPath, FileMode.Open))
                {
                    int fileLength = Convert.ToInt32(stream.Length);
                    FileDTO file = new FileDTO();
                    file.UploadFileName = Path.GetFileName(realPath);
                    byte[] fileData = new byte[fileLength];
                    stream.Read(fileData, 0, fileLength);
                    file.FileData = fileData;
                    file.FileSize = fileData.Length;
                    file.StartPosition = 0;
                    file.IsClient = false;
                    //上传文件获得url
                    imageUrl = BTPFileSV.Instance.UploadFile(file);
                    if (!string.IsNullOrEmpty(imageUrl)) //上传成功
                    {
                        imageUrl = CustomConfig.FileServerUrl + imageUrl;
                    }

                }
            }
            return imageUrl;
        }

        /// <summary>
        /// 转换的图片清晰度，1最不清醒，10最清晰
        /// </summary>
        public enum Definition
        {
            One = 1, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10
        }

        /// <summary>
        /// 将PDF文档转换为图片的方法
        /// </summary>
        /// <param name="pdfInputPath">PDF文件路径</param>
        /// <param name="imageOutputPath">图片输出路径</param>
        /// <param name="imageName">生成图片的名字</param>
        /// <param name="startPageNum">从PDF文档的第几页开始转换</param>
        /// <param name="endPageNum">从PDF文档的第几页开始停止转换</param>
        /// <param name="imageFormat">设置所需图片格式</param>
        /// <param name="definition">设置图片的清晰度，数字越大越清晰</param>
        public static void ConvertPdf2Image(string pdfInputPath, string imageOutputPath, string imageName, int startPageNum, int endPageNum, ImageFormat imageFormat, Definition definition)
        {
            if (File.Exists(pdfInputPath))
            {
                PDFFile pdfFile = PDFFile.Open(pdfInputPath);
                if (!Directory.Exists(imageOutputPath))
                {
                    Directory.CreateDirectory(imageOutputPath);
                }
                if (startPageNum <= 0)
                {
                    startPageNum = 1;
                }
                if (endPageNum > pdfFile.PageCount)
                {
                    endPageNum = pdfFile.PageCount;
                }
                if (startPageNum > endPageNum)
                {
                    startPageNum = endPageNum;
                    endPageNum = startPageNum;
                }
                for (int i = startPageNum; i <= endPageNum; i++)
                {
                    Bitmap pageImage = pdfFile.GetPageImage(i - 1, 56 * (int)definition);
                    int canKao = pageImage.Width > pageImage.Height ? pageImage.Height : pageImage.Width;
                    int newHeight = canKao > 1080 ? pageImage.Height / 2 : pageImage.Height;
                    int newWidth = canKao > 1080 ? pageImage.Width / 2 : pageImage.Width;
                    Bitmap newPageImage = new Bitmap(newWidth, newHeight);

                    Graphics g = Graphics.FromImage(newPageImage);
                    g.InterpolationMode = InterpolationMode.Default;

                    g.DrawImage(pageImage, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, pageImage.Width, pageImage.Height), GraphicsUnit.Pixel);
                    newPageImage.Save(imageOutputPath + imageName + "." + imageFormat, imageFormat);
                    g.Dispose();
                    newPageImage.Dispose();
                    pageImage.Dispose();
                }
                pdfFile.Dispose();
            }
        }
    }
}
