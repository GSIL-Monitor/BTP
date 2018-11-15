//获取数据初始化

//获取打印的订单相关数据
function getPrintOrderData() {
    OrderData = null;
    $.ajax({
        url: '/OrderPrint/GetPrintOrders',
        type: 'post',
        data: {
            orderIds: _orderIds
        },
        success: function (data) {
            if (data.Result) {
                OrderData = data.Order;
                var t = $("#divPrintOrderTemplate").html();
                var innerHTML = "";
                for (var i = 0; i < OrderData.length; i++) {
                    var _order = OrderData[i].order;
                    var _orderItems = OrderData[i].orderItem;
                    var len = _orderItems.length > 0 ? 1 : 0;
                    for (var j = 0; j < len; j++) {
                        var _orderItem = _orderItems[j];
                        innerHTML += t.replaceAll("{OrderNo}", _order.Code)
                                      .replaceAll("{Name}", _orderItem.CommodityName)
                                      .replaceAll("{Express}", _order.ShipExpCo == null ? "" : _order.ShipExpCo)
                                      .replaceAll("{ExpressNo}", _order.ExpOrderNo == null ? "" : _order.ExpOrderNo);
                    }
                }
                $("#divPrintOrder").html(innerHTML);
                orderIsOneUserCount();
            }
            else {
                alert(data.Messages);
            }
        },
        error: function () {
        }
    });
}

//获取发件人数据
function getSenders() {
    $.ajax({
        url: '/OrderPrint/GetOrderSenders',
        type: 'post',
        data: {
            appId: _appId
        },
        success: function (data) {
            if (data.Result) {
                Senders = data.Sender;
                // var html = '<option value="-1"></option>';
                var html = "";
                if (Senders != null) {
                    Senders.each(function () {
                        var name = this.SenderName + " " + this.ProvinceName + this.CityName + this.CountyName + this.DetailAddress;
                        if (name.length > 34) {
                            name = name.substring(0, 34) + "...";
                        }
                        html += '<option value="' + this.Id + '">' + name + '</option>';
                    });
                }
                $("#ddlSender").html(html);
                SelectSender($("#ddlSender"));
            }
        },
        error: function () {
        }
    });
}

//获取模板数据
function getTemplateData() {
    $.ajax({
        url: '/OrderPrint/GetExpressOrderTemplate',
        type: 'post',
        data: {
            appId: _appId
        },
        success: function (data) {
            if (data.Result) {
                Template = data.Template;
               // var html = '<option value="-1"></option>';
                var html = "";
                if (Template != null) {
                    Template.each(function () {
                        html += '<option value="' + this.Template.Id + '">' + this.Template.TemplateName + '</option>';
                    });
                }
                $("#ddlTemplate").html(html);
                SelectTemplate($("#ddlTemplate"));
            }
        },
        error: function () {
        }
    });
}

function getPrintJi() {
    if (needCLodop()) {
        window.On_CLodop_Opened = function () {
            doGetetPrintJi();
            window.On_CLodop_Opened = null;
        };
    }
    else {
        doGetetPrintJi();
    }
}

function doGetetPrintJi() {
    LODOP = getLodop();
    var iCount = LODOP.GET_PRINTER_COUNT();
    var html = "";
    for (var i = 0; i < iCount; i++) {
        var name = LODOP.GET_PRINTER_NAME(i);
        html += '<option value="' + i + '">' + name + '</option>';
    }
    $("#ddlPrintJi").html(html);
}

function orderIsOneUserCount() {
    var sameOrders = [];
    for (var n = 0; n < OrderData.length; n++) {
        var canMerge = false;
        var rorder = OrderData[n];
        for (var i = 0; i < sameOrders.length; i++) {
            var repeats = sameOrders[i];
            var len = repeats.length;
            for (var j = 0; j < len; j++) {
                var repeat = repeats[j];
                if (compareOrder(repeat, rorder)) {
                    repeats.push(rorder);
                    canMerge = true;
                    break;
                }
            }
        }
        if (!canMerge) { sameOrders.push([rorder]); }
    }
    $("#txtOrderCount").html(sameOrders.length);
}

function compareOrder(repeat, rorder) {
    return (repeat.order.ReceiptId == rorder.order.ReceiptId
                 && repeat.order.ReceiptUserName == rorder.order.ReceiptUserName
                 && repeat.order.ReceiptPhone == rorder.order.ReceiptPhone
                 && repeat.order.ReceiptAddress == rorder.order.ReceiptAddress
                 && repeat.order.Province == rorder.order.Province
                 && repeat.order.City == rorder.order.City
                 && repeat.order.District == rorder.order.District
                 && repeat.order.RecipientsZipCode == rorder.order.RecipientsZipCode
                 && checkState(repeat.order, rorder.order));
}

function SelectSender(obj) {
    var v = obj.val();
    if (Senders != null) {
        Sender = Senders.GetFirstElement("Id", v);
    }
}
