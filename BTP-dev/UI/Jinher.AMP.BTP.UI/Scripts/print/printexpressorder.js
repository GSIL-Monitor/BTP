/*------------------------------------------页面初始化逻辑处理----------------------------------*/
function InitPrintPage(appId, orderIds) {
    _appId = appId;
    _orderIds = orderIds;
    getPrintOrderData();
    getPrintJi();
    if (Template == null) {
        getTemplateData();
    }

    if (Senders == null) {
        getSenders();
    }

    $("#ddlSender").change(function () {
        SelectSender($(this));
    });
    $("#ddlTemplate").change(function () {
        SelectTemplate($(this));
    });
}


function SelectTemplate(obj) {
    var v = obj.val();
    if (Template != null) {
        PrintTemplate = Template.find(function (item) {
            return item.Template.Id == v;
        })[0];
    }
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
    var _property = PrintTemplate.Property;
    var _template = PrintTemplate.Template;
    repeatOrders = [];
    var startNo = $("#txtStartExpressNo").val();
    var addNo = $("#ddlAddNo").val();
    for (var i = 0; i < OrderData.length; i++) {
        //检查和合并快递单
        if (checkOrderIsOneUser(OrderData[i])) continue;
        if (startNo != null && startNo != "") { //快递单递增处理
            OrderData[i].order.ExpressOrder = startNo;
            startNo = increaseExpressNo(startNo, addNo);
        }
        var _orderInfo = createOrderInfo(OrderData[i], Sender); //创建打印实体数据
        LODOP.NewPage();
//        if (ispre) {
           // LODOP.ADD_PRINT_IMAGE(_template.Top, _template.Left, _template.Width, _template.Height, "<img border='0' src='" + _template.ExpressImage + "' />");
           // LODOP.SET_PRINT_STYLE("Stretch", 2); //按原图比例(不变形)缩放模式 STYLE

            LODOP.ADD_PRINT_SETUP_BKIMG("<img border='0' src='" + _template.ExpressImage + "'>");
            LODOP.SET_SHOW_MODE("BKIMG_LEFT", _template.Left);
            LODOP.SET_SHOW_MODE("BKIMG_TOP", _template.Top);
            LODOP.SET_SHOW_MODE("BKIMG_WIDTH", _template.Width);
            LODOP.SET_SHOW_MODE("BKIMG_HEIGHT", _template.Height);
            LODOP.SET_SHOW_MODE("BKIMG_IN_PREVIEW", 1);		

//        }
        _property.each(function () {
            //LODOP.ADD_PRINT_TEXT(this.Top, this.Left, this.Width, this.Height, _orderInfo[this.PropertyName]);
            if (this.PropertyName.indexOf("DirectPrint") == 0) {
                LODOP.ADD_PRINT_TEXT(this.Top + this.Height / 2, this.Left, this.Width, this.Height, this.PropertyText);
            }
            //打印商品相关属性
            else if (this.PropertyName == "CommodityName" || this.PropertyName == "CommodityCode" || this.PropertyName == "CommodityAttributes" || this.PropertyName == "Number") {
                var itop = this.Top;
                for (var j = 0; j < OrderData[i].orderItem.length; j++) {
                    LODOP.ADD_PRINT_TEXT(itop + this.Height / 2, this.Left, this.Width, this.Height, OrderData[i].orderItem[j][this.PropertyName]);
                    itop += 6;
                }
            }
            else {
                LODOP.ADD_PRINT_TEXT(this.Top + this.Height / 2, this.Left, this.Width, this.Height, _orderInfo[this.PropertyName]);
            }
            setPrintFont();
        });
    }
}
//根据订单数据，发件人数据创建订单信息数据用于打印
function createOrderInfo(torder, tsender) {
    var _torder = torder.order;
    var needData = {
        SenderCompany: tsender.SenderCompany,
        SendTime: "",
        SenderName: tsender.SenderName,
        SenderPhone: tsender.Phone,
        SenderTel: "",
        SendPostCode: tsender.PostCode,
        SenderAddress: tsender.ProvinceName + tsender.CityName + tsender.CountyName + tsender.DetailAddress,
        SenderProvince: tsender.ProvinceName,
        SenderCity: tsender.CityName,
        SenderCounty: tsender.CountyName,
        SenderDetailAddress: tsender.DetailAddress,
        ReceiptCompany: "",
        ReceiptUserName: _torder.ReceiptUserName,
        ReceiptAddress: _torder.Province + _torder.City + _torder.District + _torder.ReceiptAddress,
        ReceiptProvince:_torder.Province,
        ReceiptCity: _torder.City,
        ReceiptCounty: _torder.District,
        ReceiptDetailAddress: _torder.ReceiptAddress,
        ReceiptPhone: _torder.ReceiptPhone,
        ReceiptTel: "",
        RecipientsZipCode: _torder.RecipientsZipCode,
        OrderCode: _torder.Code,
        SellersRemark: _torder.SellersRemark,
        BuyersRemark: _torder.Details,
        BuyersNickName: "",
        Other: ""
    };
    return needData;
}

//合并订单处理
function checkOrderIsOneUser(rorder) {
    var isMerge = $("#btnMerge").is(':checked');
    if (!isMerge) return false;
    var canMerge = false;
    for (var i = 0; i < repeatOrders.length; i++) {
        var repeats = repeatOrders[i];
        var len = repeats.length;
        for (var j = 0; j < len; j++) {
            var repeat = repeats[j];
            if (compareOrder(repeat, rorder)) {
                canMerge = true;
                rorder.order.ExpressOrder = repeat.order.ExpressOrder;
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
    var _template = PrintTemplate.Template;
   // LODOP.PRINT_INIT("快递单打印-金和电商-Task:" + Math.floor(Math.random() * 100));
    LODOP.PRINT_INITA(0, 0, _template.Width, _template.Height, "快递单打印-金和电商-Task:" + Math.floor(Math.random() * 100));
    setPrintConfig();
   // _template.Width, _template.Height
    LODOP.SET_PRINT_PAGESIZE(1, (_template.Width / 96) * 25.4 * 10, (_template.Height / 96) * 25.4 * 10, "");
    LODOP.SET_PRINT_MODE("POS_BASEON_PAPER", true);
    LODOP.SET_PRINT_MODE("FULL_WIDTH_FOR_OVERFLOW", true);
    LODOP.SET_PRINT_MODE("FULL_HEIGHT_FOR_OVERFLOW", true);
}
function setPrintConfig() {
    var printji = parseInt($("#ddlPrintJi").val());
    LODOP.SET_PRINTER_INDEXA(printji);
}
function setPrintFont() {
    LODOP.SET_PRINT_STYLEA(0, "PenWidth", 11);
    LODOP.SET_PRINT_STYLEA(0, "FontSize", 11);
    LODOP.SET_PRINT_STYLEA(0, "FontName", "宋体");
    LODOP.SET_PRINT_STYLEA(0, "Bold", 1);
    LODOP.SET_PRINT_STYLEA(0, "Italic", 0);
}

/*------------------------------------------保存打印等按钮业务操作----------------------------------*/

function checkInput() {
    var dllsender = $("#ddlSender").val();
    if (dllsender == "-1"||Sender == null) {
        alert("请先设置发件人信息");
        return false;
    }
    var dllTemplate = $("#ddlTemplate").val();
    if (dllTemplate == "-1"||PrintTemplate == null) {
        alert("请选择打印模板");
        return false;
    }
    var dllShip = $("ddlShip").val();
    if (dllShip == "-1") {
        alert("请选择物流公司");
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
        orderIds.push({ OrderId: OrderData[i].order.OrderId, ExpressOrder: OrderData[i].order.ExpressOrder ? OrderData[i].order.ExpressOrder : "" });
    }
    var isAutoSend = $("#btnAutoSend").is(":checked") ? 1 : 0;
    var ship = $("#ddlShip").find("option:selected").text();

    $.ajax({
        url: '/OrderPrint/SavePrintOrders',
        type: 'post',
        data: {
            appId: _appId,
            userId: getQueryString("userId"),
            shipName: ship,
            autoSend: isAutoSend,
            orderJson: JSON.stringify(orderIds)
        },
        success: function (data) {
            if (data.Result) {
                //刷新父级页面
                window.parent.PrintRefreshPage(orderIds, "printOrderE");
            }
        },
        error: function (e) {
            var eee = e;
        }
    });

}

function cancelPrint() {
    window.parent.ClosePrintOrder("printOrderE");
}