var vm = new Vue({
    el: '#app',
    //已成功挂载，相当ready()
    mounted(){
        this.host = window.location.host.substring(0,window.location.host.indexOf('btp'));
        //判断是否缓存了订单ID值.当没有时从地址栏获取并缓存
        sessionStorage.orderId = getQueryString('orderId');
        saveContextDTOByUrl();
        this.couponCode = getQueryString("couponCodes");
        this.couponCount = eval(getQueryString("couponCount"));
        if (JsVilaDataNull(this.couponCode) && this.couponCount > 0) {
            this.couponInfo = "使用" + this.couponCode.split(';').length + "张";
        }
        //是否显示标题
        if(!isWeiXin() && (sessionStorage.source == "share" || getQueryString("source") == "share")) {
            this.headerShow = true;
        }
        this.loadData();
    },
    data(){
        return{
            headerShow:false,
            host:'',
            text: null,
            appName: '',
            orderDetail:{}, //订单详情
            orderBtnAll:[],//订单需要显示的所有按钮
            shoppingListBtn: [],//商品列表上显示的操作按钮
            orderState: '', //订单状态
            orderStateTips: '', //订单状态下面的提示内容
            selfTakeTime: null,//取货时间
            goldCouponFlag: false, //支付方式(金币支付、代金券)是否显示
            payPatternData: {}, //获取支付方式返回的数据
            couponCode: null,
            couponCount: 0,
            couponInfo: "未使用", //使用代金券的个数
            freight: '0', //运费
            goldPrice: 0,//金币抵用
            goldbalance: 0,//可抵用的金币总数
            goldCoupon: 0,//代金券抵用
            payPrice: 0, //实付款
            payMethod: '在线支付',//选择的支付方式
            toggle: null,
            switchChecked: false, //开关组件是否是选中状态
            switchDisabled: false, //开关组件是否是禁用状态
            showPayDialog: false,
            setPwdDialog: false,
            goldpwd: '', // 金币密码
            expireTime: null,
            bigCodeShow: false, //提货二维码大图是否显示
            IsCheck:false, //是否是京东的单子
            OrderDetailState:0,
            OrderDetailRefundExpOrderNo:false
        }
    },
    methods:{
        /**
         * 获取订单详情数据
         */
        loadData:function(){
            this.$http({
                url: '/Mobile/GetOrderDetails',
                method: 'GET',
                params:{
                    orderId: sessionStorage.orderId,
                    userId: getUserId()
                },
                before: function(){
                    this.$loading.open();
                }
            }).then(function(response){
                this.$loading.close();
                this.appName = response.data.AppName;
                this.orderDetail = response.data.data;
                for (var i = 0; i < this.orderDetail.ShoppingCartItemSDTO.length; i++) {
                    if (this.orderDetail.ShoppingCartItemSDTO[i].JdOrderid!=null&&this.orderDetail.ShoppingCartItemSDTO[i].JdOrderid!="") {
                        this.IsCheck=true;
                    }
                }
                this.OrderDetailState=this.orderDetail.State;
                this.OrderDetailRefundExpOrderNo=this.orderDetail.RefundExpOrderNo;
                this.showOrderDetails();
            },function(response){
                this.$loading.close();
                this.$toast('服务器繁忙，请稍后重试！');
                
            });
        },
        /**
         * 初始化数据
         */
        showOrderDetails: function(){
            //获取订单状态
            this.orderState = getOrderStateText(this.orderDetail.State,this.orderDetail.StateAfterSales);
            this.orderBtnAll = getButtonByState(this.orderDetail,this.orderDetail.Payment);//订单显示的所有操作按钮
            this.shoppingListBtn = this.getShoppingListBtn(); //商品列表上显示的操作按钮
            this.orderStateTips = this.showOrderStateTips();//订单状态提示
            if(this.orderDetail.State == 2){
                var eDate = dateFormat(new Date());
                var sDate=null;
                if (this.orderDetail.ShipmentsTime!=null&&this.orderDetail.ShipmentsTime!="") {
                   sDate = dateFormat(new Date(parseInt(this.orderDetail.ShipmentsTime.replace("/Date(", "").replace(")/", ""), 10)));
                }
                var result = (new Date(eDate) - new Date(sDate)) / (24 * 60 * 60 * 1000) + 1;
                result = result > 9 ? 9 : result;
                if (result > 9) {
                    result = 9;
                }
                this.orderState += " (第" + result + "天)"
            }
            //自提时间
            this.selfTakeTime = this.SetselfTakeTime();
            /*设置支付方式*/
            if(this.orderDetail.State == 0 && this.orderDetail.payment != 1 && this.orderDetail.payment != 2){
                this.checkPayPattern();//获取支付方式
            }else{
                this.goldCouponFlag = false;
            }
            //运费
            if(this.orderDetail.IsModifiedPrice){
                this.freight = "0";
            }else{
                this.freight = Math.abs(this.orderDetail.Freight).toFixed(2);
            }
            this.payPrice = this.orderDetail.Price.toFixed(2);
            //金币/代金券抵用的金额
            if(this.orderDetail.GoldPrice > 0 || this.orderDetail.GoldCoupon > 0){
                this.goldPrice = this.orderDetail.GoldPrice;
                this.goldCoupon = this.orderDetail.GoldCoupon;
            }
        },
        //订单状态提示内容
        showOrderStateTips: function(){
            var text = "";
            switch(this.orderDetail.State){
                case 2:
                case 13:
                    text =  "发货后9天,若未确认收货,订单状态会自动变为已收货。";
                    break;
                case 3:
                    if(this.orderDetail.StateAfterSales == 7){
                        var rsm = this.orderDetail.RefundScoreMoney > 0 ? "(含" + this.orderDetail.RefundScoreMoney + "元积分)" : "";
                        text = "已成功退款" + this.orderDetail.RefundMoney + "元" + rsm + "，请到付款账号中核实~";
                    }
                    break;
                case 7:
                    var rsm = this.orderDetail.RefundScoreMoney > 0 ? "(含" + this.orderDetail.RefundScoreMoney + "元积分)" : "";
                    text = "已成功退款" + this.orderDetail.RefundMoney + "元" + rsm + "，请到付款账号中核实~";
                    break;
                case 8:
                    text = "买家申请退款，10天内商家未响应，自动达成同意退款/退货申请协议。"
                    break;
                case 9:
                case 14:
                    text = "买家申请退款/退货，10天内商家未响应，自动达成同意退款/退货申请协议。";
                    break;
                case 10:
                    text = "卖家同意退款/退货，请在7天之内发货。";
                    break;
            }
            return text;
        },
        getShowType: function(){
            var type = '';
            switch(this.orderDetail.State){
                case 0:
                    type = '1';
                    break;
                case 1:
                    if(this.orderDetail.Payment == 1){
                        type = "1";
                    }else{
                        type = "2";
                    }
                    break;
                case 2:
                case 11:
                    type = "3";
                    break;
                case 3:
                    if(this.orderDetail.StateAfterSales == 3 && this.orderDetail.OrderType == 0){
                        type = "23";
                    }
                    break;
                case 7:
                case 10:
                    type = "4";
                    break;
                case 8:
                case 14:
                    type = "5";
                    break;
                case 13:
                    if(this.orderDetail.Payment != 1){
                        type = "3";
                    }
                    break;
            }
            return type;
        },
        /**
         * 设置自提的提货时间
         */
        SetselfTakeTime:function(){
            if(this.orderDetail.PickUpTime){
                
                var date=null;
                if (this.orderDetail.PickUpTime!=null&&this.orderDetail.PickUpTime!="") {
                   date = new Date(parseInt(this.orderDetail.PickUpTime.replace("/Date(", "").replace(")/", ""), 10));
                }
                var month = date.getMonth() + 1 < 10 ? "0" + (date.getMonth() + 1) : date.getMonth() + 1;
                var currentDate = date.getDate() < 10 ? "0" + date.getDate() : date.getDate();
                this.selfTakeTime = date.getFullYear() + '-' + month + '-' + currentDate;
            }
            if(this.orderDetail.PickUpStartTime && this.orderDetail.PickUpEndTime){
                var minutesStar = this.orderDetail.PickUpStartTime.Minutes < 10 ? ('0' + this.orderDetail.PickUpStartTime.Minutes) : this.orderDetail.PickUpStartTime.Minutes;
                var minutesEnd = this.orderDetail.PickUpEndTime.Minutes < 10 ? ('0' + this.orderDetail.PickUpEndTime.Minutes) : this.orderDetail.PickUpEndTime.Minutes;
                this.selfTakeTime += '  ' + minutesStar + '~' + minutesEnd;
            }
        },
        /**
         * 获取支付方式
         */
        checkPayPattern:function(){
            var param = {
                userId: getUserId(),
                sessionId: getSessionId(),
                esAppId: getEsAppId(),
                appId: this.orderDetail.AppId
            };
            this.$http.post('/mobile/CheckPayPattern',param,{emulateJSON:true}).then(function(response){
                if (!response.data) {return;}
                this.payPatternData = response.data;
                this.calcPayPrice();
                if(response.data.TradeType == 0){
                    this.goldCouponFlag = true;
                    this.goldbalance =(Math.floor(response.data.GoldBal * 0.1) * 0.01).toFixed(2);
                    if(this.goldbalance == 0){
                        this.switchDisabled = true;
                        this.switchChecked = false;
                    }else{
                        if(window.sessionStorage.goldpay == 'true'){
                            this.switchChecked = true;
                            this.switchDisabled = false;
                        }
                    }
                }
            },function(response){
                var data = {"Pattern":0,"GoldBal":789,"GoldCouponCount":10,"IsAllAppInZPH":true,"TradeType":0,"IsHdfk":false};
                if (!data) {return;}
                this.payPatternData = data;
                this.calcPayPrice();
                if(data.TradeType == 0){
                    this.goldCouponFlag = true;
                    this.goldbalance =(Math.floor(data.GoldBal * 0.1) * 0.01).toFixed(2);
                    if(this.goldbalance == 0){
                        this.switchDisabled = true;
                        this.switchChecked = false;
                    }else{
                        if(window.sessionStorage.goldpay == 'true'){
                            this.switchChecked = true;
                            this.switchDisabled = false;
                        }
                    }
                }
            });
        },
        /**
         * 计算支付信息
         */
        calcPayPrice: function(){
            var payCouponValue = 0,payPriceValue = this.orderDetail.Price;
            this.goldPrice  = 0;
            if (this.couponCount && this.payPatternData.IsAllAppInZPH){
                payCouponValue = this.couponCount >= payPriceValue ? payPriceValue: this.couponCount;
                this.goldCoupon = payCouponValue > 0 ? eval(payCouponValue).toFixed(2) : '0';
            }else{
                this.goldCoupon = 0;
            }
            payPriceValue = (payPriceValue - payCouponValue).toFixed(2);
            if(this.toggle){
                this.goldPrice  = (this.goldbalance >= payPriceValue) ? payPriceValue : this.goldbalance;
            }
            this.goldPrice = this.goldPrice > 0 ? this.goldPrice : 0;
            //实付款
            this.payPrice = (payPriceValue - this.goldPrice).toFixed(2);
        },
        /**
         * 跳转页面
         */
        goPage: function(state,JdOrderid){
            var backUrl = document.location.href;
            switch (state){
                case "coupon":
                    if (backUrl.indexOf("&couponCount") > 0) {
                        backUrl = backUrl.substr(0, backUrl.indexOf("&couponCount"));
                    }
                    this.couponCode = JsVilaDataNull(this.couponCode) ? getQueryString("couponCodes") : '';
                    //如果金币已经设置了金币抵用支付，则存储
                    if(this.toggle){
                        window.sessionStorage.goldpay = 'true'
                    }else{
                        window.sessionStorage.goldpay = 'false';
                    }
                   window.location.href = PromotionUrl + "/MyVouchers/VoucherList?srcApp=btp&userId=" + getUserId() + "&sessionId=" + getSessionId() + "&selectedVCodes=" + this.couponCode + "&orderAmount=" + this.orderDetail.Price + "&ChangeOrg=00000000-0000-0000-0000-000000000000&esAppId=" + getEsAppId() + "&srcUrl=" + encodeURIComponent(backUrl);
                    break;
                case "exp":
                    if (JdOrderid!=null) {
                       document.location.href = urlAppendCommonParams("/ExpressRoute/Index?shipExpCo=" + this.orderDetail.ShipExpCo + "&expOrderNo=" +this.orderDetail.ExpOrderNo + "&backUrl=" + encodeURIComponent(backUrl))+ "&CommodityOrderId=" + this.orderDetail.CommodityOrderId + "&JdOrderId="+JdOrderid;
                    }
                    else {
                       document.location.href = urlAppendCommonParams("/ExpressRoute/Index?shipExpCo=" + this.orderDetail.ShipExpCo + "&expOrderNo=" +this.orderDetail.ExpOrderNo + "&backUrl=" + encodeURIComponent(backUrl))+ "&CommodityOrderId=" + this.orderDetail.CommodityOrderId + "&JdOrderId=";
                    }
                   
                    break;
                case "contact":
                    this.$loading.open();
                    this.$http.post('/Mobile/GetZPHContractUrl',{esAppId: getEsAppId()},{emulateJSON:true}).then(function(response){
                        this.$loading.close();
                        var contactAppId = response.data.ContactObj ? this.orderDetail.AppId : (getEsAppId() || this.orderDetail.AppId);
                        if (response.data.Code == 0 && JsVilaDataNull(response.data.Data) && response.data.Data.indexOf("http") >= 0) { //取到联系地址
                            window.location.href = response.data.Data;
                        } else if (sessionStorage.source == "share") {
                            window.location.href = WebImUrl.format(contactAppId, getUserId(), getSessionId(), getChangeOrg());
                        } else {
                            try {
                                if (navigator.userAgent.toLowerCase().indexOf("iphone") > -1) {
                                    window.location.href = "/Mobile/ContactAppOwner?type=goutong&appId=" + contactAppId;
                                } else {
                                    window.contactStoreOwner.startServiceList(contactAppId);
                                }
                            } catch (e) {
                                this.$toast("商家暂不支持此功能~");
                            }
                        }
                        //行为记录->联系商家操作
                        logBTP(sessionStorage.SrcType, service_type, "0x000f", '', logOrderId);
                    },function(){
                        this.$loading.close();
                    });
                    break;
                case "refundInfo":
                    var allDisplayBtns = this.orderBtnAll.join(',');
                    window.location.href = urlAppendCommonParams("/Mobile/RefundInfo?orderId=" + getQueryString('orderId') + "&shopId=" + this.orderDetail.AppId) + "&isAfterSale=0&allDisplayBtns=" + allDisplayBtns + "&RefundExpCo=" + escape(this.orderDetail.RefundExpCo) + "&RefundExpNo=" + escape(this.orderDetail.RefundExpOrderNo);
                    break;
            }
        },
        /**
         *  获取商品列表上的操作按钮
         * @returns {Array}
         */
        getShoppingListBtn: function(){
            var arr = [];
            var btnAll = getButtonByState(this.orderDetail,this.orderDetail.Payment);
            for(var i =0; i < btnAll.length; i++){
                if(btnAll[i] == "2" || btnAll[i] == "3" || btnAll[i] == "7" || btnAll[i] == "8" || btnAll[i] == "25" || btnAll[i] == "31"){
                    arr.push(btnAll[i]);
                    continue;
                }
            }
            return arr;
        },
        /**
         * 确认按最新价支付
         */
        confirmPayPrice: function(){
            //行为记录->在线支付
            logBTP(sessionStorage.SrcType, service_type, this.orderDetail.Payment == 1 ? "0x0015" : "0x0014", '', logOrderId);
            this.$loading.open();
            this.$http.post('/Jinher.AMP.BTP.SV.CommodityOrderSV.svc/ConfirmPayPrice',{userId: getUserId(),commodityOrderId: getQueryString('orderId')},{emulateJSON:true}).then(function(response){
                switch(response.data.ResultCode){
                    case 0:
                        this.expireTime = response.data.ExpireTime;
                        if(this.payMethod == "货到付款"){//货到付款
                            this.hdfkPay();
                        }else{
                            this.orderDetail.Price = response.data.Message;
                            this.jinherWebPay();
                        }
                        break;
                    case 1:
                        this.$loading.close();
                        this.$toast(response.data.Message);
                        break;
                    case 2:
                        this.$loading.close();
                        this.orderDetail.Price = response.data.Message;
                        this.orderDetail.Freight = response.data.Freight;
                        this.orderDetail.CouponValue = response.data.CouponAmount;
                        this.calcPayPrice();
                        var text = '当前商品价格已发生变化,是否继续?<br/>最新订单价格为(￥'+ response.data.Message + ')';
                        let _this = this;
                        this.$confirm(text,function(){
                            _this.$loading.close();
                            _this.jinherWebPay();
                        });
                        break;
                    case 6:
                        this.$loading.close();
                        this.orderDetail.State = 6;
                        this.showOrderDetails();
                        break;
                }
            },function(err){
                this.$loading.close();
                this.$toast('服务器繁忙，请稍后重试！');

                var data = {"Message":"33.80","ResultCode":2,"isSuccess":false,"CouponAmount":0,"Duty":0,"ExpireTime":null,"Freight":0.00,"IsModifiedPrice":0}
                switch(data.ResultCode){
                    case 0:
                        this.expireTime = data.ExpireTime;
                        if(this.payMethod == "货到付款"){//货到付款
                            this.hdfkPay();
                        }else{
                            this.orderDetail.Price = data.Message;
                            this.jinherWebPay();
                        }
                        break;
                    case 1:
                        this.$loading.close();
                        this.$toast(data.Message);
                        break;
                    case 2:
                        this.$loading.close();
                        this.orderDetail.Price = data.Message;
                        this.orderDetail.Freight = data.Freight;
                        this.orderDetail.CouponValue = data.CouponAmount;
                        this.calcPayPrice();
                        var text = '当前商品价格已发生变化,是否继续?<br/>最新订单价格为(￥'+ data.Message + ')';
                        var _this = this;
                        this.$confirm(text,function(){
                            _this.$loading.close();
                            _this.jinherWebPay();
                        });
                        break;
                    case 6:
                        this.$loading.close();
                        this.orderDetail.State = 6;
                        this.showOrderDetails();
                        break;
                }
            });
        },
        jinherWebPay: function(){
            if(this.goldbalance > 0 && this.toggle){
                this.checkgoldpwd();
                return;
            }
            this.orderPay();
        },
        //订单支付
        orderPay: function(){
            if (this.payPrice == 0 && getEsAppId() != "8b4d3317-6562-4d51-bef1-0c05694ac3a6"){
                var goldData = {
                    orderId: getQueryString('orderId'),
                    userId: getUserId(),
                    appId: this.orderDetail.AppId,
                    goldpwd: hex_md5(this.goldpwd),
                    realprice: this.payPrice,
                    comName: this.orderDetail.ShoppingCartItemSDTO[0].Name,
                    sessionId: getSessionId(),
                    gold: this.goldPrice,
                    couponCount: this.goldCoupon,
                    couponCodes: this.couponCode
                };
                this.$loading.open();
                this.$http.post('/Mobile/GoldPayCommodityOrder',{
                    goldDTO:JSON.stringify(goldData)
                },{emulateJSON:true}).then(function(response){
                    this.$loading.close();
                    if(response.data.ResultCode){
                        this.$toast(response.data.Message);
                    }else{
                        this.$toast('支付成功!');
                        window.location.href = urlAppendCommonParams("/Mobile/PaySuccess?user=" + getUserId() + "&orderId=" + sessionStorage.orderId + "&shopId=" + this.orderDetail.appId + "&type=shuaxin&orderState=1");
                    }
                },function(err){
                    this.$loading.close();
                    this.$toast('支付失败!');
                })
            }else{
                if (!isLogin()) return;
                var payOrderToFspDto = {
                    OrderId: getQueryString('orderId'),
                    GoldCoupon: this.goldCoupon,
                    GoldPrice: this.goldPrice,
                    Source: (JsVilaDataNull(sessionStorage.source) ? sessionStorage.source : 'internal'),
                    SrcType: getSysName(sessionStorage.SrcType),
                    WxOpenId: bridge.getData('WeChatKey') || "",
                    FirstCommodityName: "电商" + this.orderDetail.ShoppingCartItemSDTO[0].Name,
                    EsAppId:  getEsAppId(),
                    ExpireTime: this.expireTime,
                    TradeType: this.payPatternData.TradeType
                };
                if (JsVilaDataNull(getQueryString("couponCodes"))) {
                    payOrderToFspDto.GoldCouponIds = getQueryString("couponCodes");
                }
                this.$http.post('/Mobile/GetPayUrl',payOrderToFspDto,{emulateJSON:true}).then(function(response){
                    if (!response.data) {
                        this.$toast("生成在线支付地址失败");
                        return;
                    }
                    if (response.data.ResultCode != 0) {
                        this.$toast(response.data.Message);
                        return;
                    }
                    if (response.data.Data == "") {
                       this.$toast("在线支付地址获得的值为空");
                        return;
                    }
                    window.location.href = response.data.Data;
                },function(err){
                    this.$loading.close();
                    this.$toast("获取在线支付地址失败");
                });
            }
        },
        //货到付款
        hdfkPay(){
            this.$http({
                url: '/Mobile/UpdateCommodityOrder',
                method:'GET',
                params:{
                    orderId: getQueryString('orderId'),
                    userId: getUserId(),
                    appId: this.orderDetail.AppId
                }
            }).then(function(response){
                this.$loading.close();
                if(response.data.ResultCode == 0){
                   this.$toast('提交成功！');
                    this.orderDetail.State = 1;
                    this.orderDetail.Payment = 1;
                    this.showOrderDetails(this.orderDetail);
                }else{
                    this.$toast(response.data.Message);
                }
            },function(response){
                this.$loading.close();
                this.$toast('服务器繁忙，请稍候再试！')
            });
        },
        /**
         * 检查是否设置过金币密码
         */
        checkgoldpwd: function(){
            this.$http({
                url:'/Mobile/CheckGoldPwd',
                method: 'GET',
                params:{
                    userId: getUserId()
                }
            }).then(function(response){
                this.$loading.close();
                if(response.data.Code == 1){ //未设置过密码
                    this.setPwdDialog = true;
                }else{//已经设置过密码
                    this.showPayDialog = true;
                }
            },function(response){
                this.$loading.close();
                this.$toast('获取是否设置过金币密码失败！');
            });
        },
        /**
         * 支付弹窗确认
         */
        payDialogOk: function(event){
            if(event[0]){
                this.goldpwd = event[0];
                // 校验金币密码
                this.$loading.open();
                this.$http.post('/Mobile/CheckGoldPayPwdVal',{
                    userId: getUserId(),
                    sessionId: getSessionId(),
                    pwd: hex_md5(event[0])
                },{emulateJSON:true}).then(function(response){
                    if(response.data.Code == 0){
                        if(this.orderDetail.State == 0){//支付
                            this.$loading.close();
                            this.orderPay();
                        }else{//确认收货
                            if(this.orderDetail.Payment == 0){
                                this.$http({
                                    url: '/Mobile/ConfirmOrder',
                                    method: 'GET',
                                    params:{
                                        commodityOrderId: getQueryString('orderId'),
                                        password: hex_md5(this.goldpwd)
                                    },
                                    before:function(){
                                        this.$loading.open();
                                    }
                                }).then(function(response){
                                    this.$loading.close();
                                    if(response.data.ResultCode == 1){
                                        this.$toast(response.data.Message);
                                    }else{
                                        this.orderDetail.State = 3;
                                        if(this.orderDetail.Payment != 1){
                                            this.orderDetail.StateAfterSales = 3;
                                        }
                                        this.showOrderDetails();
                                    }
                                },function(err){
                                    this.$loading.close();
                                })
                            }else if(",3,6,7,9,5,4,".indexOf("," + this.orderDetail.Payment+ ",") > -1){
                                this.confirmReceipt();
                            }
                        }
                    }else{
                        this.$toast(response.data.Message);
                    }
                },function(err){
                    this.$toast("金币支付失败");
                    this.$loading.close();

                });
            }else{
                this.$toast("请输入密码");
            }
        },
        /**
         * 支付弹窗取消
         */
        payDialogClose: function(){
            this.showPayDialog = false;
        },
        /**
         * 设置密码弹窗确认
         */
        setPwdDialogOk:function(event){
            if(event[0]== "" && event[1] == ""){
                this.$toast('请输入金币交易密码');
                return;
            }
            if(event[0] != event[1]){
                this.$toast("两次密码输入不一致");
                return;
            }
            this.setPwdDialog = false;
            this.showPayDialog = true;
            this.$http({
                url: '/Mobile/SetGoldPayPwd',
                method: 'GET',
                params:{
                    userId: getUserId(),
                    pwd: hex_md5(event[0])
                }
            }).then(function(response){
                if(response.data.Code == 0){
                    this.setPwdDialog = false;
                    this.showPayDialog = true;
                    this.$toast(response.data.Message);
                }else{
                    this.$toast(response.data.Message);
                }
            },function(err){
                this.$toast('设置交易密码失败');
            });
        },
        //设置密码弹窗取消
        setPwdDialogClose:function(){
            this.setPwdDialog = false;
        },
        //删除订单
        deletOrder:function(){
            this.$http({
                url: '/Mobile/DelOrder',
                method: 'GET',
                params:{
                    orderId: sessionStorage.orderId
                },
                before:function(){
                    this.$loading.open();
                }
            }).then(function(response){
                this.$loading.close();
                if(response.data.ResultCode == 0){
                    this.$toast('订单已删除');
                    window.location.href = '/Mobile/MyOrderList';
                }else{
                    this.$toast(response.data.Message)
                }
            },function(err){
                this.$loading.close();
                this.$toast('服务器繁忙，请稍候再试！')
            });
        },
        refund:function(){
            window.location.href = urlAppendCommonParams("/Mobile/RefundOrder?" + "shopId=" + this.orderDetail.AppId + "&orderId=" + getQueryString('orderId') + "&pri=" + this.orderDetail.Price + "&state=" + this.orderDetail.State + "&pay=" + this.orderDetail.Payment + "&type=" + this.getShowType() + "&isAfterSale=0&spendScoreMoney=" + this.orderDetail.ScorePrice);
        },
        refundclick:function(){
           if (this.orderDetail.AppId=="630af8fc-41e9-4bd1-a436-2dd4197f076b"||this.orderDetail.AppId=="cf063155-e6e9-4019-ba12-6b44b704243f") {
              this.$confirm('如需退款，请加智力圈客服凯文咨询。</br>微信ID：kevinmeiguo')
           }
           else if(this.IsCheck==true){
               var obj = {
                   message: '已发货，不能申请退款，可拒收~',
                   showCancelButton: false
               };
              this.$confirm(obj);
           }
           else {
              this.$confirm('建议先与商家进行沟通，协商一致后再申请退款。</br>确定继续申请？',this.refund);
           }
           
        
        },
        //点击提货二维码，显示大图
        clickCode: function(){
            this.bigCodeShow = true;
        },
        //隐藏预览大图提货码
        bigCodeClick: function(){
            this.bigCodeShow = false;
        },
        //点击确认收货按钮
        confirmReceiptClick:function(){
            //行为记录->确认收货
            logBTP(sessionStorage.SrcType, service_type, "0x0022", '', logOrderId);
            if(",3,6,0,7,9,5,4,".indexOf("," + this.orderDetail.Payment + ",") > -1){ //担保支付宝
                this.checkgoldpwd();
            }else if(",1003,1011,1007,1009,1010,1006,1004,1005,".indexOf("," + this.orderDetail.Payment + ",") > -1) { //直接到账
                this.$confirm('您是否收到该订单商品？',this.confirmReceipt)
            }else{
                this.confirmReceipt();
            }
        },
        //确认收货
        confirmReceipt: function(){
            this.$http({
                url: '/Mobile/UpdateCommodityOrderc',
                method: 'GET',
                params:{
                    orderId: getQueryString('orderId'),
                    userId: getUserId(),
                    appId: this.orderDetail.AppId,
                    payment: this.orderDetail.Payment,
                    goldpwd: hex_md5(this.goldpwd)
                },
                before: function(){
                    this.$loading.open();
                }
            }).then(function(response){
                this.$loading.close();
                if(response.data.ResultCode == 1){
                    this.$toast(response.data.Message);
                }else{
                    this.orderDetail.State = 3;
                    if(this.orderDetail.Payment != 1){
                        this.orderDetail.StateAfterSales = 3;
                    }
                    this.showOrderDetails();
                }
            },function(err){
                this.$loading.close();
                this.$toast('确认收货失败！');
            });
        },
        //延长收货时间
        delayShip: function(){
            //行为记录->延长收货时间操作
            logBTP(sessionStorage.SrcType, service_type, "0x000d", '', logOrderId);
            this.$http({
                url: '/Mobile/DelayShip',
                method: 'GET',
                params:{
                    orderId: sessionStorage.orderId
                },
                before:function(){
                    this.$loading.open();
                }
            }).then(function(response){
                this.$loading.close();
                if(response.data.ResultCode == 0){
                    this.orderStateTips = "发货12天后，如果用户未确认，系统将自动确认收货。";
                    this.$toast('已经延长收货3天');
                    this.orderBtnAll.splice(this.orderBtnAll.indexOf(11),1);
                }else{
                    this.$toast(response.data.Message);
                }
            },function(err){
                this.$loading.close();
                this.$toast('延长收货时间失败！');
            });
        },
        //评价
        orderComReview(id,commodityId,name){
            //行为记录->用户评分
            logBTP(sessionStorage.SrcType, service_type, "0x0013", '', logOrderId);
            var callbackURL = encodeURIComponent(window.location.protocol + "//" + window.location.host + "/Review/ReviewSuccessNotify");
            var redirectTo = encodeURIComponent(location.href);
            var param = "appId="+ this.orderDetail.AppId +"&userId="+getUserId()+"&sessionId="+getSessionId()+"&changeOrg="+getChangeOrg()+"&businessId="+id+"&productId="+commodityId+"&title="+formatLongString(name,20)+"&productType=21&callbackURL="+callbackURL+"&redirectTo="+redirectTo;
            window.location.href = "http://" + this.host + "sns.iuoooo.com" + "/Evaluate/Add?" + param;

        },
        forgotPassword(){
            // 实例化
            var base64 = new Base64();
            // 给字符串加密
            window.location.href = FSPUr + "/PasswordProtect/GetPassQuestion?userId=" + getUserId() + "&sessionId=" + getSessionId() + "&ChangeOrg=" + getChangeOrg() + "&token=12345678&clientType=web&callback=" + base64.encode(window.location.href);
        }
    },
    watch:{
        toggle: function(){ //监听开关(switch)change事件
            this.calcPayPrice();
        }
    },
    //过滤器
    filters:{
        phoneHref(value){
            if(!value) return;
            return "rel:" + value
        },
        timeFilter(value){
            if(!value) return;
            return dateFormat(new Date(parseInt(value.replace('/Date(','').replace(')/',''),10)),1);
        },
        goldFilter(value){
            if(!value) return;
            return '￥' + eval(Math.floor(value * 0.1) * 0.01).toFixed(2);
        },
        paymentFilter(value){
            return value == 1 ? '货到付款' : '在线支付';
        },
        currency(value,negative){
            if(value === null || value === "" || value === undefined || value === "undefined") return;
            var rel = "";
            if(negative){ //需要显示“负”价格
                rel =  value > 0 ? '-￥' + eval(value).toFixed(2) : '￥0.00';
            }else{
                rel =  '￥' + eval(value).toFixed(2);
            }return rel;
        },
        totalPriceFilter(value,CouponValue,ScorePrice){//订单金额过滤器
            return '￥' + eval(value + CouponValue + ScorePrice).toFixed(2);
        },
        goPageCancelOrder(value,appId){
            return urlAppendCommonParams("/Mobile/OrderCancelReason?shopId=" + appId + "&orderId=" + value);
        },
        pickUpTimeFilter(value,endtime){
            if(value == null || endtime == null){
                return;
            }
            var minutesStar = value.Minutes < 10 ? ('0' + value.Minutes) : value.Minutes;
            var minutesEnd = endtime.Minutes < 10 ? ('0' + endtime.Minutes) : endtime.Minutes;
            return value.Hours + ":" + minutesStar + '~' + endtime.Hours + ":" + minutesEnd;
        },
        pickUpDateFilter(value){
            if(!value) return;
            return dateFormat(new Date(parseInt(value.replace('/Date(','').replace(')/',''),10)));
        }
    }
});