
/*------------------------------------------页面初始化逻辑处理----------------------------------*/
function InitPrintPage(appId, orderIds) {
    _appId = appId;
    _orderIds = orderIds;
    getPrintOrderData();
    getPrintJi();
    if (Senders == null) {
        getSenders();
    }

    $("#ddlSender").change(function () {
        SelectSender($(this));
    });
}

/*------------------------------------------打印业务逻辑处理----------------------------------*/
//打印订单
function printOrder() {
    createPrintHandler();
    SetPrintOrderData(false);
    LODOP.PRINT();
}

//打印预览订单
function previewOrder() {
    if (!checkInput()) return;
    createPrintHandler();
    SetPrintOrderData(true);
    LODOP.PREVIEW();
}

var repeatOrders = []; //合并订单用到的
//设置打印数据
function SetPrintOrderData(ispre) {
    var pageSize = getPageSize();
    var _template = "";
    repeatOrders = [];
    for (var i = 0; i < OrderData.length; i++) {
        //检查和合并快递单
        checkOrderIsOneUser(OrderData[i]);
    }
    for (var i = 0; i < repeatOrders.length; i++) {
        LODOP.NewPage();
        var html = setTemplateData(repeatOrders[i]);
       // LODOP.ADD_PRINT_RECT(0, 0, pageSize.w, pageSize.h, 0, 0);
        LODOP.ADD_PRINT_HTM(0, 0, pageSize.w, pageSize.h, html);
    }
}

function setTemplateData(orderInfos) {
    var html = "";
    var t_html = $("#divInvoiceTemplate").html();
    var t_html2 = $(t_html).find("#ProductHTML").html();
    for (var i = 0; i < orderInfos.length; i++) {
        var _order = orderInfos[i].order;
        var _orderItems = orderInfos[i].orderItem;

        var pcount = 0;
        for (var j = 0; j < _orderItems.length; j++) {
            pcount += _orderItems[j].Number;
        }
        //替换基本信息
        var r_html = t_html.replaceAll("{OrderNo}", _order.Code)
                               .replaceAll("{OrderTime}", _order.SubTime)
                               .replaceAll("{PayType}", _order.PayType)
                               .replaceAll("{PayTime}", _order.PaymentTime)
                               .replaceAll("{ReceiptUserName}", _order.ReceiptUserName)
                               .replaceAll("{ReceiptPhone}", _order.ReceiptPhone)
                               .replaceAll("{ReceiptAddress}", _order.Province + _order.City + _order.District + _order.ReceiptAddress)
                               .replaceAll("{RecipientsZipCode}", _order.RecipientsZipCode == null ? "" : _order.RecipientsZipCode)
                               .replaceAll("{ShipName}", _order.ShipExpCo == null ? "" : _order.ShipExpCo)
                               .replaceAll("{ExpressOrder}", _order.ExpOrderNo == null ? "" : _order.ExpOrderNo)
                               .replaceAll("{BuyersRemark}", _order.Details == null ? "" : _order.Details)
                               .replaceAll("{SellersRemark}", _order.SellersRemark == null ? "" : decodeURIComponent(_order.SellersRemark))
                               .replaceAll("{TotalCount}", pcount)
                               .replaceAll("{TotalPrice}", _order.RealPrice);

        var p_html = setProductData(_orderItems, t_html2);
        var invoice = $(r_html);
        if (i > 0) {
            invoice.find("#divSender1").remove();
            invoice.find("#divSender2").remove();
            invoice.find("#divHead").remove();
        }
        invoice.find("#ProductHTML").html(p_html);
        html += invoice.prop('outerHTML');
    }
    return html;
}

function setProductData(orderItems, t_html) {
    var p_html = "";
    //替换商品信息
    for (var j = 0; j < orderItems.length; j++) {
        var _orderItem = orderItems[j];
        p_html += t_html.replaceAll("{Num}", j + 1)
                                .replaceAll("{ProductNo}", _orderItem.CommodityCode)
                                .replaceAll("{ProductName}", _orderItem.CommodityName)
                                .replaceAll("{ProductPirce}", _orderItem.Price)
                                .replaceAll("{ProductCount}", _orderItem.Number)
                                .replaceAll("{ProductTotalPirce}", _orderItem.Number * _orderItem.Price);
    }
    return p_html;
}

//合并订单处理
function checkOrderIsOneUser(rorder) {
    var isMerge = $("#btnMerge").is(':checked');
    if (!isMerge) {
        repeatOrders.push([rorder]);
        return false;
    }
    var canMerge = false;
    for (var i = 0; i < repeatOrders.length; i++) {
        var repeats = repeatOrders[i];
        var len = repeats.length;
        for (var j = 0; j < len; j++) {
            var repeat = repeats[j];
            if (compareOrder(repeat, rorder)) {
                canMerge = true;
                repeats.push(rorder);
                break;
            }
        }
    }
    if (canMerge) return true;
    repeatOrders.push([rorder]);
    return false;
}

//判断订单状态是否符合合并打印
function checkState(repeat, order) {
    return (repeat.State == 1 || repeat.State == 13) && (order.State == 1 || order.State == 13);
}

function createPrintHandler() {
    LODOP = getLodop();
   // LODOP.PRINT_INIT("发货单打印-金和电商-Task:" + Math.floor(Math.random() * 100));
    var pageSize = getPageSize();
    LODOP.PRINT_INITA(0, 0, pageSize.w, pageSize.h, "发货单打印-金和电商-Task:" + Math.floor(Math.random() * 100));
    setPrintConfig();
   // LODOP.SET_PRINT_PAGESIZE(1, 0, 0, $("#ddlPageType").val());
   // var pageSize = getPageSize();
    LODOP.SET_PRINT_PAGESIZE(1, pageSize.w, pageSize.h, "");
    // LODOP.SET_PRINT_MODE("POS_BASEON_PAPER", true);
    LODOP.SET_PRINT_MODE("RESELECT_PAGESIZE", true);
    LODOP.SET_PRINT_MODE("RESELECT_ORIENT", true);
    LODOP.SET_PRINT_MODE("FULL_WIDTH_FOR_OVERFLOW", true);
    LODOP.SET_PRINT_MODE("FULL_HEIGHT_FOR_OVERFLOW", true);
}
function getPageSize() {
    var pageSize = $("#ddlPageType").find("option:selected");
    return { w: pageSize.attr("w"), h: pageSize.attr("h") };
}
function setPrintConfig() {
    var printji = parseInt($("#ddlPrintJi").val());
    LODOP.SET_PRINTER_INDEXA(printji);
}
function setPrintFont() {
    LODOP.SET_PRINT_STYLEA(0, "PenWidth", 11);
    LODOP.SET_PRINT_STYLEA(0, "FontName", "宋体");
    LODOP.SET_PRINT_STYLEA(0, "Bold", 1);
    LODOP.SET_PRINT_STYLEA(0, "Italic", 0);
}

/*------------------------------------------保存打印等按钮业务操作----------------------------------*/

function checkInput() {
    var dllsender = $("#ddlSender").val();
    if (dllsender == "-1" || Sender == null) {
        alert("请先设置发件人信息");
        return false;
    }
    return true;
}


//疑问：是同时进行还是打印完成之后在保存，顺序问题
function printAndSave() {
    if (!checkInput()) return;
    var printOK = true;
    try {
        printOrder();
    }
    catch (e) {
        printOK = false;
    }

    if (printOK) {
        try {
            updatePrintOrderInfo();
        }
        catch (e) {
        }
    }
}

//打印保存逻辑
function updatePrintOrderInfo() {
    var orderIds = [];
    for (var i = 0; i < OrderData.length; i++) {
        orderIds.push({ OrderId: OrderData[i].order.OrderId, ExpressOrder: OrderData[i].order.ExpOrderNo ? OrderData[i].order.ExpOrderNo : "" });
    }
    $.ajax({
        url: '/OrderPrint/SavePrintInvoiceOrders',
        type: 'post',
        data: {
            appId: _appId,
            userId: getQueryString("userId"),
            orderJson: JSON.stringify(orderIds)
        },
        success: function (data) {
            if (data.Result) {
                //刷新父级页面
                window.parent.PrintRefreshPage(orderIds, "printOrderV");
            }
        },
        error: function (e) {
            var eee = e;
        }
    });

}

function cancelPrint() {
    window.parent.ClosePrintOrder("printOrderV");
}