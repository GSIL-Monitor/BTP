var flex = function () {
    var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function () {
    flex();
};
$(function () {
    saveContextDTOByUrl();
    DealLoginPartial.initPartialPage();
});
var vm = new Vue({
    el: '#app',
    created: function () {
        this.orderState = getQueryString('orderState') || null;
        this.initNav(this.orderState);
    },
    mounted: function () {
        this.$nextTick(function () {
            //是否显示标题
            if (!isWeiXin() && (sessionStorage.source == "share" || getQueryString("source") == "share")) {
                this.headerShow = true;
            }
            this.loadDate();
            window.addEventListener('scroll', this.handleEvent, false);
        });
    },
    data: function () {
        return {
            clocked: false,
            headerShow: true,
            navList: ['全部', '待付款', '待发货', '待收货', '待评价'],
            selectNav: 0,
            orderState: null, //订单状态 null：全部 0:待付款 1：待发货 2：待收货 3：待评价
            pageIndex: 1, //页码
            pageSize: 10, //每页数量
            isEndPage: false, //是否是最后一页
            allowScroll: true,
            orderList: [],
            orderListBtn: [],
            orderListState: [],
            orderBtnAll: [],
            tips: false,
            empty: false,
            showConfirm: false, //是否显示删除弹窗
            showtixing: false, //是否显示提醒发货
            selectIndex: null, //删除订单的索引值
            selectOrderId: null, //删除订单的ID
            showConfirmDate: false, //是否显示延长收货弹窗
            reasons: ['不想买了', '信息填写错误，重新拍', '商家缺货', '太贵了，不划算', '其他原因'], //取消订单原因
            reasonsValue: ["1", "2", "3", "4", "5"],
            showModal: false, //取消订单弹窗
            selectCancelOrderId: null, //取消订单的ID
            selectCancelAppId: null, //取消订单的APPID
            selectDelayShipOrderId: null, //延长收货的订单ID
            txCountStr: '', //提醒发货标识
            service_type: "0x0007"
        }
    },
    methods: {
        loadDate: function () {
            this.$http({
                url: '/Mobile/GetOrder',
                method: 'GET',
                params: {
                    userId: getUserId(),
                    pageIndex: this.pageIndex,
                    pageSize: this.pageSize,
                    state: this.orderState,
                    esAppId: getEsAppId()
                },
                before: function () {
                    this.$loading.open();
                }
            }).then(function (response) {
                this.$loading.close();
                var data = response.data;
                if (data.length) {
                    this.isEndPage = data.length - this.pageSize < 0 ? true : false;
                    if (this.isEndPage) {
                        this.tips = true;
                    }
                    this.orderList = this.orderList.concat(data);
                    this.getorderListBtn(data);
                    this.getorderListState(data);
                } else {
                    if (this.pageIndex == 1) {
                        this.orderList = [];
                        this.orderListBtn = [];
                        this.orderListState = [];
                        this.empty = true;
                    } else {
                        this.tips = true;
                    }
                }
            }, function (response) {
                this.$loading.close();
                this.$toast('服务器繁忙，请稍后再试');
            });
        },
        //订单列表中的所有操作按钮
        getorderListBtn: function (data) {
            for (var i = 0; i < data.length; i++) {
                var state = this.orderBtnAll = getButtonByState(data[i], data[i].PayType);
                //                for (var j = 0; j < state.length; j++) {
                //                    if (state[j] == 2) {
                //                        state.splice(j, 1);
                //                    }
                //                }
                this.orderListBtn.push(state);
            }
        },
        //获取所有订单的状态
        getorderListState: function (data) {
            for (var i = 0; i < data.length; i++) {
                this.orderListState.push(getOrderStateTextForList(data[i].State, data[i].StateAfterSales));
            }
        },
        isAllReview: function (data) {
            for (var i = 0; i < data.length; i++) {
                if (!data[0].HasReview) {
                    return true;
                }
            }
            return false;
        },
        //滚动条滚动到底部加载数据
        handleEvent: function () {
            var scrollTope = document.body.scrollTop;
            var scrollHeight = document.body.scrollHeight;
            var clientHeight = document.documentElement.clientHeight;
            if (scrollHeight - scrollTope - clientHeight <= 10) {
                if (!this.isEndPage) {
                    this.pageIndex++;
                    this.loadDate();
                }
            }
        },
        //删除订单
        deletOrder: function () {
            var oid = this.selectOrderId.toString();
            this.$loading.open('正在提交');
            this.$http({
                url: '/Mobile/DelOrder',
                method: 'GET',
                params: {
                    orderId: oid,
                    isDel: 1
                }
            }).then(function (response) {
                //删除该订单
                this.orderList.splice(this.selectIndex, 1);
                this.orderListBtn.splice(this.selectIndex, 1);
                this.orderListState.splice(this.selectIndex, 1);
                this.cancelConfirm();
                this.$loading.close();
            }, function (err) {
                this.$loading.close();
                this.$toast('订单删除失败！');
            });
        },
        goPage: function (state, data) {
            if (!data.IsThirdOrder) {
                if (state == "orderDetail") {//订单详情，付款按钮，进入详情按钮
                    window.location.href = urlAppendCommonParams("/Mobile/MyOrderDetail?orderId=" + data.ShoppingCartItemSDTO[0].OrderId + "&shopId=" + data.AppId + "&orderState=" + getQueryString("orderState") + "&sessionId=" + getQueryString("SessionId"));
                } else if (state == "orderCancelReason") {//取消订单按钮
                    window.location.href = urlAppendCommonParams("/Mobile/OrderCancelReason?shopId=" + data.AppId + "&orderId=" + data.ShoppingCartItemSDTO[0].OrderId);
                } else if (state == "expressRoute") {//查看物流按钮
                    window.location.href = urlAppendCommonParams("/ExpressRoute/Index?shipExpCo=" + data.ShipExpCo + "&expOrderNo=" + data.ExpOrderNo + "&backUrl=" + encodeURIComponent(document.location.href));
                } else if (state == "refundInfo") {//查看退款详情
                    var allDisplayBtns = getButtonByState(data, data.PayType).join(',');
                    window.location.href = urlAppendCommonParams("/Mobile/RefundInfo?orderId=" + data.ShoppingCartItemSDTO[0].OrderId + "&shopId=" + data.AppId) + "&isAfterSale=0&allDisplayBtns=" + allDisplayBtns + "&RefundExpCo=" + escape(data.RefundExpCo) + "&RefundExpNo=" + escape(data.RefundExpOrderNo);
                } else if (state == "refundPage") {//申请退款
                    if (confirm('建议先与商家进行沟通，协商一致后再申请退款。确定继续申请？')) {
                        var _showtype = undefined; var pay = data.PayType;
                        switch (data.State) {//判断订单状态
                            case 0:
                                _showtype = 1;
                            case 1:
                                if (pay == 1)
                                    _showtype = 1;
                                else
                                    _showtype = 2;
                            case 13:
                                if (pay != 1)
                                    _showtype = 3;
                            case 2:
                                if (pay != 1)
                                    _showtype = 3;
                            case 7:
                                _showtype = 4;
                            case 8:
                                _showtype = 5;
                            case 14:
                                _showtype = 5;
                            case 10:
                                _showtype = 4;
                            case 11:
                                _showtype = 2;
                        }
                        window.location.href = urlAppendCommonParams("/Mobile/RefundOrder?" + "shopId=" + data.AppId + "&orderId=" + data.ShoppingCartItemSDTO[0].OrderId + "&pri=" + data.Price + "&state=" + data.State + "&pay=" + data.PayType + "&type=" + _showtype + "&isAfterSale=0&spendScoreMoney=" + data.ScorePrice + "&spendYJBMoney=" + data.YJBPrice);
                    }

                    //new
                    //                var _showtype = 23;
                    //                    if(data.JdEclpOrder) {//进销存订单
                    //                        if(data.JdEclpOrder.EclpOrderState!=2000 && data.JdEclpOrder.EclpOrderState<=10033){
                    //                            toast("已发货不能申请退款，可拒收~");
                    //                            return;
                    //                        }else if(data.JdEclpOrder.EclpOrderState===10034){
                    //                            if(confirm('建议先与商家进行沟通，协商一致后再申请退款。确定继续申请？')){
                    //                                window.location.href = urlAppendCommonParams("/Mobile/ReEchrfundOrder?userId=" + getUserId() + "&shopId=" + data.AppId + "&orderId=" + data.ShoppingCartItemSDTO[0].OrderId + "&pri=" + data.Price + "&state=" + data.Stat + "&pay=" + data.PayType + "&type=" + _showtype + "&isAfterSale=1&spendScoreMoney=" + data.ScorePrice + "&spendYJBMoney=" +  data.YJBPrice + "&orderItemId=" + orderItemId);
                    //                            }
                    //                            return;
                    //                        }
                    //                    }
                    //                    // 判断是否为京东订单
                    //                    var currentCommodityOrderItemData = null;
                    //                    $.each(data.ShoppingCartItemSDTO, function() {
                    //                        if (this.Id == orderItemId) {
                    //                            currentCommodityOrderItemData = this;
                    //                            return false;
                    //                        }
                    //                    });
                    //                    if (currentCommodityOrderItemData && currentCommodityOrderItemData.JdOrderid) {
                    //                        if (currentCommodityOrderItemData.JdOrderStatus == 3) {


                    //                            if (confirm('建议先与商家进行沟通，协商一致后再申请退款。确定继续申请？')) {
                    //                                window.location.href = urlAppendCommonParams("/Mobile/RefundOrder?userId=" + getUserId() + "&shopId=" + appId + "&orderId=" + getQueryString('orderId') + "&pri=" + $("#hidPrice").val() + "&state=" + $("#hidState").val() + "&pay=" + $("#hidPayState").val() + "&type=" + _showtype + "&isAfterSale=1&spendScoreMoney=" + _orderDetail.data.ScorePrice + "&spendYJBMoney=" + _orderDetail.data.YJBPrice + "&spendYJCouponMoney=" + _orderDetail.data.YJCouponPrice + "&spendCouponMoney=" + _orderDetail.data.CouponValue + "&orderItemId=" + orderItemId);
                    //                            }

                    ////                            if (confirm('建议先与商家进行沟通，协商一致后再申请退款。确定继续申请？')) {
                    ////                                var a = $("#hidState").val();
                    ////                                if (a == 3) {
                    ////                                    window.location.href = urlAppendCommonParams("/Mobile/RefundOrder?userId=" + getUserId() + "&shopId=" + appId + "&orderId=" + getQueryString('orderId') + "&pri=" + $("#hidPrice").val() + "&state=" + $("#hidState").val() + "&pay=" + $("#hidPayState").val() + "&type=" + _showtype + "&isAfterSale=1&spendScoreMoney=" + _orderDetail.data.ScorePrice + "&spendYJBMoney=" + _orderDetail.data.YJBPrice + "&orderItemId=" + orderItemId);
                    ////                                } else {
                    ////                                    window.location.href = urlAppendCommonParams("/Mobile/RefundOrder?userId=" + getUserId() + "&shopId=" + appId + "&orderId=" + getQueryString('orderId') + "&pri=" + $("#hidPrice").val() + "&state=" + $("#hidState").val() + "&pay=" + $("#hidPayState").val() + "&type=" + _showtype + "&isAfterSale=0&spendScoreMoney=" + _orderDetail.data.ScorePrice + "&spendYJBMoney=" + _orderDetail.data.YJBPrice + "&orderItemId=" + orderItemId);
                    ////                                }
                    ////                            }
                    //                        } else {
                    //                            // 检查是否可以发起京东退款
                    //                            $.get("Url.Action("CheckJdRefundIsAvailable")?jdorderId=" + currentCommodityOrderItemData.JdOrderid + "&skuId=" + currentCommodityOrderItemData.JdSkuId, function(result) {
                    //                                if (result.isSuccess) {
                    //                                    if (confirm('建议先与商家进行沟通，协商一致后再申请退款。确定继续申请？')) {
                    //                                        window.location.href = urlAppendCommonParams("/Mobile/RefundJdOrder?userId=" + getUserId() + "&shopId=" + appId + "&orderId=" + getQueryString('orderId') + "&pri=" + $("#hidPrice").val() + "&state=" + $("#hidState").val() + "&pay=" + $("#hidPayState").val() + "&type=" + _showtype + "&isAfterSale=1&spendScoreMoney=" + _orderDetail.data.ScorePrice + "&spendYJBMoney=" + _orderDetail.data.YJBPrice+ "&spendYJCouponMoney=" + _orderDetail.data.YJCouponPrice  + "&spendCouponMoney=" + _orderDetail.data.CouponValue + "&orderItemId=" + orderItemId);
                    //                                    }
                    //                                } else {
                    //                                    alert("抱歉，该商品不符合售后条件~");
                    //                                }
                    //                            });
                    //                        }
                    //                    } else {
                    //                        if (confirm('建议先与商家进行沟通，协商一致后再申请退款。确定继续申请？')) {
                    //                            window.location.href = urlAppendCommonParams("/Mobile/RefundOrder?userId=" + getUserId() + "&shopId=" + appId + "&orderId=" + getQueryString('orderId') + "&pri=" + $("#hidPrice").val() + "&state=" + $("#hidState").val() + "&pay=" + $("#hidPayState").val() + "&type=" + _showtype + "&isAfterSale=1&spendScoreMoney=" + _orderDetail.data.ScorePrice + "&spendYJBMoney=" + _orderDetail.data.YJBPrice + "&spendYJCouponMoney=" + _orderDetail.data.YJCouponPrice + "&spendCouponMoney=" + _orderDetail.data.CouponValue + "&orderItemId=" + orderItemId);
                    //                        }
                    //                    }
                }
            }
            else {
                if (state == "orderDetail" && data.AppName != "保险推荐") {//订单详情
                    window.location.href = data.ThirdMobileUrl + "?OrderNo=" + data.OrderCode;
                }
                else {
                    if (data.DSFStateName == '待支付') {
                        var url = "http://eurl.idoutec.cn/hdbb/car/preOrder.html?channel=H5_DBB_ZHONGSH&state=preorder&userId=" + data.UserId + "&orderNo=" + data.OrderCode + "";
                        window.location.href = url;
                    }
                }
            }
        },
        navChangeFun: function (value) {
            var os = this.orderState;
            switch (value) {
                case 0:
                    this.orderState = null;
                    break;
                case 1:
                    this.orderState = 0;
                    break;
                case 2:
                    this.orderState = 1;
                    break;
                case 3:
                    this.orderState = 2;
                    break;
                case 4:
                    this.orderState = 3;
                    break;
            }
            this.selectNav = value;
            this.pageIndex = 1;
            this.isEndPage = false;
            this.orderList = [];
            this.orderListBtn = [];
            this.orderListState = [];
            this.empty = false;
            this.allowScroll = false;
            this.loadDate();
            if (window.history) {
                var u = '';
                if (window.location.href.indexOf('orderState') > -1)
                    u = window.location.href.replace("orderState=" + os, "orderState=" + this.orderState);
                else
                    u = window.location.href + "&orderState=" + this.orderState;
                history.pushState(null, null, u)
                document.title = '我的订单';
            }
        },
        initNav: function (value) {
            switch (value) {
                case '0':
                    this.selectNav = 1;
                    break;
                case '1':
                    this.selectNav = 2;
                    break;
                case '2':
                    this.selectNav = 3;
                    break;
                case '3':
                    this.selectNav = 4;
                    break;
                default:
                    this.selectNav = 0;
            }
        },
        ChangeDateFormat: function (cellval, state) {
            try {
                var date = new Date(parseInt(cellval.replace("/Date(", "").replace(")/", ""), 10));
                var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
                var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                var hour = date.getHours() < 10 ? "0" + date.getHours() : date.getHours();
                var minu = date.getMinutes() < 10 ? "0" + date.getMinutes() : date.getMinutes();
                if (state == 1) {
                    return date.getFullYear() + "-" + month + "-" + currentDate + " " + hour + ":" + minu;
                } else {
                    return date.getFullYear() + "-" + month + "-" + currentDate;
                }
            } catch (e) {
                return "";
            }
        },
        openDialog: function (data, index) {
            this.selectOrderId = data.ShoppingCartItemSDTO[0].OrderId;

            this.selectIndex = index;
            this.showConfirm = true;
            //this.clocked = true;
        },
        cancelConfirm: function () {
            this.selectOrderId = null;
            this.selectDelayShipOrderId = null;
            this.selectCancelOrderId = null;
            this.selectIndex = null;
            this.showConfirm = false;
            this.clocked = false;
        },
        /**
        * 点击确认延长收货按钮
        */
        submitConfirmDate: function () {
            this.$toast('延长收货时间成功！');
            this.$loading.open('正在提交');
            var oid = this.selectDelayShipOrderId;
            this.$http({
                url: '/Mobile/DelayShip',
                method: 'GET',
                params: {
                    orderId: oid
                }
            }).then(function (response) {
                this.cancelConfirm();
                this.$loading.close();
            }, function (err) {
                this.$loading.close();
                this.$toast('订单延长收货失败！');
            });
            this.hideConfirmDateFun();
        },
        /**
        * 显示确认延长收货
        */
        showConfirmDateFun: function (data, index) {
            this.selectDelayShipOrderId = data.ShoppingCartItemSDTO[0].OrderId;
            this.showConfirmDate = true;
            //this.clocked = true;
        },
        /**
        * 隐藏确认延长收货
        */
        hideConfirmDateFun: function () {
            this.selectDelayShipOrderId = null;
            this.showConfirmDate = false;
            this.clocked = false;
        },
        /**
        * 显示提醒收货
        */
        showTiXingConfirm: function (data, index) {
            var oid = data.CommodityOrderId;
            if (this.txCountStr.indexOf(oid + ':1') > -1) {
                //$('#txtext').text('已提醒卖家发货');
                this.$toast("已提醒卖家发货");
            } else {
                this.$loading.open('正在提交');
                //发消息通知商家 提醒发货 TODO
                this.$http({
                    url: '/Mobile/ShipmentRemind',
                    method: 'GET',
                    params: {
                        orderId: oid
                    }
                }).then(function (response) {
                    if (response) {
                        //$('#txtext').text('提醒发货成功');
                        this.$loading.close();
                        this.$toast("提醒发货成功");
                        if (this.txCountStr.indexOf(oid) > -1)
                            this.txCountStr.replace(oid + ':0', oid + ':1');
                        else
                            this.txCountStr = this.txCountStr + ',' + oid + ':1';
                    }
                }, function (err) {
                    //$('#txtext').text('提醒发货失败！');
                    this.$toast("提醒发货失败");
                });


            }
            //this.showtixing = true;
            //this.clocked = true;
        },
        /**
        * 隐藏提醒收货
        */
        hideTiXingConfirm: function () {
            this.showtixing = false;
            this.clocked = false;
        },
        /**
        * 显示取消订单弹窗
        */
        showModalFun: function (data, index) {
            this.selectCancelOrderId = data.ShoppingCartItemSDTO[0].OrderId;
            this.selectCancelAppId = data.AppId;
            this.selectIndex = index;
            //this.clocked = true;
            this.showModal = true;
        },
        /**
        * 隐藏取消订单弹窗
        */
        hideModal: function () {
            this.clocked = false;
            this.showModal = false;
            this.selectCancelOrderId = null;
            this.selectCancelAppId = null;
        },
        changeReason: function (re) {
            console.log(re);
            var reasonsindex = this.reasons.indexOf(re);
            var reasonsVal = this.reasonsValue[reasonsindex];
            //提交数据
            var oid = this.selectCancelOrderId;
            var aid = this.selectCancelAppId;
            this.$loading.open('正在提交');
            this.$http({
                url: '/Mobile/ClickOKCancelOrder',
                method: 'GET',
                params: {
                    orderId: oid, userId: getUserId(), appId: aid, mess: reasonsVal
                }
            }).then(function (response) {
                this.$loading.close();
                if (response.data.ResultCode == 0) {
                    this.$toast("操作成功");
                    this.orderList.splice(this.selectIndex, 1);
                    this.orderListBtn.splice(this.selectIndex, 1);
                    this.orderListState.splice(this.selectIndex, 1);
                }
                else { this.$toast(response.data.Message); }
                this.selectIndex = null;
            }, function (err) {
                this.$loading.close();
                this.selectIndex = null;
                this.$toast('取消订单失败！');
            });
            this.hideModal();
        },
        chakanwuliu: function (data, index) {//查看物流

                        var orderItemId = data.ShoppingCartItemSDTO[0].Id;
                        var JdOrderId = data.ShoppingCartItemSDTO[0].JdOrderid;
                        //alert(orderItemId); alert(JdOrderId);
                        var EsAppId = getEsAppId();
                        if (EsAppId == "8B4D3317-6562-4D51-BEF1-0C05694AC3A6" || EsAppId == "8b4d3317-6562-4d51-bef1-0c05694ac3a6") {

                            if (IsCheckJdorder(data.ShoppingCartItemSDTO) == true) {

                            } else {
                                var ExpOrderNo = data.ExpOrderNo;
                                if (ExpOrderNo != null && ExpOrderNo != '' && ExpOrderNo != undefined) {
            						getDataAjax({
            							url: '/Mobile/UpdateZSH',
            							data: { ExpOrderNo: ExpOrderNo, userId: getUserId() },
            							callback: function (res) {
            							},
            							beforeSend: function () {
            								//ajaxLoading('22', '');
            								//this.$loading.open('正在跳转...');
            							},
            							complete: function () {
            								//$("#ajaxLoadBlind").remove();
            								//this.$loading.close();
            							}
            						});
                                }
                            }
                        }
                        var backUrl = document.location.href;
                        var shipExpCo = data.ShipExpCo ? data.ShipExpCo : "";
                        var expOrderNo = data.ExpOrderNo ? data.ExpOrderNo : "";
                        var url = urlAppendCommonParams("/ExpressRoute/Index?shipExpCo=" + shipExpCo + "&expOrderNo=" + expOrderNo + "&backUrl=" + encodeURIComponent(backUrl)) + "&CommodityOrderId=" + data.CommodityOrderId;

            //            if (IsCheckJdorder(data.ShoppingCartItemSDTO) == true)
            //                url += "&JdOrderId=" + JdOrderId;
            //            else
            //                url += "&JdOrderId=";
                        if(JdOrderId==null){JdOrderId=''}

                        url += "&JdOrderId=" + JdOrderId;
                        url += "&OrderItemId=" + orderItemId;
                        url += "&OrderAppId=" + data.AppId;
            document.location.href = url;
        },
        orderComReview: function (data, index) {//评价

            var ordeItemId = data.ShoppingCartItemSDTO[0].Id;
            var logOrderId = 'orderid:' + data.CommodityOrderId + '|';
            //行为记录->用户评分
            //logBTP(sessionStorage.SrcType, this.service_type, "0x0013", '', logOrderId);

            //            var scis = _orderDetail.data.ShoppingCartItemSDTO;
            //            var sci = scis.GetOnlyElement("Id", ordeItemId);


            var pObj = new Object();
            pObj.appId = data.AppId;
            pObj.userId = getUserId();
            pObj.sessionId = getSessionId();
            pObj.changeOrg = getChangeOrg();
            pObj.businessId = ordeItemId;
            //pObj.businessId = data.CommodityOrderId;
            pObj.productId = data.ShoppingCartItemSDTO[0].CommodityId;
            pObj.title = formatLongString(data.ShoppingCartItemSDTO[0].Name, 20);
            var notifyUrl = window.location.protocol + "//" + window.location.host + "/Review/ReviewSuccessNotify";
            pObj.callbackURL = encodeURIComponent(notifyUrl);
            pObj.redirectTo = encodeURIComponent(location.href);
            pObj.ordercode = data.OrderCode;
            pObj.esappid = getEsAppId();
            pObj.OrderId = data.CommodityOrderId;

            //var reviewUrl = snsURL + "/Evaluate/Add?appId={appId}&userId={userId}&sessionId={sessionId}&changeOrg={changeOrg}&businessId={businessId}&productId={productId}&title={title}&productType=21&callbackURL={callbackURL}&redirectTo={redirectTo}";
            var reviewUrl = snsURL + "/EvaluateNew/index?commodityId={productId}&appId={appId}&userid={userId}&businessid={businessId}&orderid={OrderId}&esappid={esappid}&ordercode={ordercode}";
            window.location.href = reviewUrl.format(pObj);

        },
        confirmReceipt: function (data, index) {
            var oid = data.ShoppingCartItemSDTO[0].OrderId;
            this.$http({
                url: '/Mobile/UpdateCommodityOrderc',
                method: 'GET',
                params: {
                    orderId: oid,
                    userId: getUserId(),
                    appId: data.AppId,
                    payment: data.PayType,
                    goldpwd: hex_md5(this.goldpwd)
                },
                before: function () {
                    this.$loading.open();
                }
            }).then(function (response) {
                this.$loading.close();
                if (response.data.ResultCode == 1) {
                    this.$toast(response.data.Message);
                } else {
                    this.orderDetail.State = 3;
                    if (this.orderDetail.Payment != 1) {
                        this.orderDetail.StateAfterSales = 3;
                    }
                    this.showOrderDetails();
                }
            }, function (err) {
                this.$loading.close();
                this.$toast('确认收货失败！');
            });
        }
    },
    filters: {
        sizeFilter: function (value) {
            if (!value) return;
            if (value.indexOf(',') == 0) {
                value = value.substring(1);
            }
            if (value.length > 5)
                return value.substr(0, 5) + "...";
            else
                return value;
        },
        nameFilter: function (value) {
            if (!value) return;
            value = value.toString();
            if (value.length > 33)
                return value.substr(0, 33) + "...";
            else
                return value;
        }
    }
});

function IsCheckJdorder(data) {
    var flag = false;
    for (var i = 0; i < data.length; i++) {
        if (data[i].JdOrderid != null && data[i].JdOrderid != "") {
            flag = true;
        }
    }
    return flag;
}