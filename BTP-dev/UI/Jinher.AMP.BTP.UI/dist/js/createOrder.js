/**********************YJB-Begin**********************/
window.YJB = (function (mod, $, undefined) {
    // 是否启用易捷币
    mod.useYjb = false;
    // 易捷币兑换比例
    mod.yjbRate = 0;
    // 是否互斥
    mod.mutex = false;
    mod.noSelected = true;
    mod.active = "yjb-none";
    mod.tempActive = mod.active;
    mod.yjbPrice = 0;
    mod.youkaPrice = 0;
    mod.yjbValue = 0;

    mod.canUse = false;

    // 初始化易捷币信息
    mod.initYjbInfo = function (data) {
        if (data.Enabled) {
            $("#yjb_balance").show();
            // 用户拥有的YJB金额
            $("#yjb-user-amount").text(data.TotalCashCount.toFixed(2));
            $("#userYJBCount").text(data.TotalCashCount.toFixed(2));
            // 本单可用的YJB金额
            $("#yjb-order-amount").text(data.InsteadCashAmount.toFixed(2));
            $("#orderYJBAmount").text(data.InsteadCashAmount.toFixed(2));
            mod.yjbPrice = data.InsteadCashAmount;
            mod.yjbRate = data.Rate;

            if (data.InsteadCashCount > 0) {
                mod.canUse = true;
                $("#toggleYJB").css("pointer-events", "");
            } else {
                mod.canUse = false;
                //让开关失效。
                $("#toggleYJB").css("pointer-events", "none");
                $("#yjb_balance").hide();
            }

            mod.initSelect();
        } else {
            // TODO
            $("#yjbBar").hide();
            $("#yjb_balance").hide();
        }

        // // 是否启用易捷币
        // if (data.Enabled) {
        //     $('#yjbUl').removeClass('hide');
        //     $("#yjb_balance").show();
        //     $("#userYJBCount").text(data.TotalCashCount);
        //     $("#orderYJB").text(data.InsteadCashCount);
        //     $("#orderYJBAmount").text(data.InsteadCashAmount);
        //     YJB.yjbRate = data.Rate;
        // } else {
        //     $("#yjb_balance").hide();
        // }
        // if (data.InsteadCashCount > 0) {
        //     $("#toggleYJB").css("pointer-events", "");
        //     YJB.useYjb = true;
        // } else {
        //     //让开关失效。
        //     $("#toggleYJB").css("pointer-events", "none");
        //     YJB.useYjb = false;
        // }   
    }

    mod.initSelect = function () {
        if (mod.active == "yjb-none") {
            $("#yjb-yjb").removeClass('active');
            $("#yjb-youka").removeClass('active');
            $("#yjb-none").addClass('active');
            if (YJCOUPON.currentCouponeIds > 0) {//COUPON.useCoupon ||
                $("#yjbBar .shop-info-list-text").html('<span class="unselect">当前优惠不可叠加使用</span>');
            } else {
                if (mod.noSelected) {
                    $("#yjbBar .shop-info-list-text").html('<span class="unselect">请选择</span>');
                } else {
                    $("#yjbBar .shop-info-list-text").html('<span class="unselect">不使用该优惠</span>');
                }
            }
        } else if (mod.active == "yjb-yjb") {
            $("#yjb-yjb").addClass('active');
            $("#yjb-youka").removeClass('active');
            $("#yjb-none").removeClass('active');
            $("#yjbBar .shop-info-list-text").html('<span class="red">易捷币抵现 ￥' + mod.yjbPrice + '</span>');
        } else if (mod.active == "yjb-youka") {
            $("#yjb-yjb").removeClass('active');
            $("#yjb-youka").addClass('active');
            $("#yjb-none").removeClass('active');
            $("#yjbBar .shop-info-list-text").html('<span class="red">赠送 ￥' + mod.youkaPrice + '油卡兑换券</span>');
        }
    }

    // 设置互斥状态
    mod.setMutex = function (isMutex) {
        //if (COUPON.useCoupon || YJCOUPON.currentCouponeIds > 0) {
        if (isMutex) {
            $("#yjbBar .shop-info-list-text").html('<span class="unselect">当前优惠不可叠加使用</span>');
        } else {
            $("#yjbBar .shop-info-list-text").html('<span class="unselect">请选择</span>');
        }
        mod.mutex = isMutex;
    }

    // 禁用易捷币
    mod.disable = function () {
        mod.tempActive = "yjb-none";
        mod.confirmYJBModal();
    }

    mod.showYJBModal = function () {
        $('#yjb-modal').removeClass('hide');
    }

    mod.hideYJBModal = function () {
        $('#yjb-modal').addClass('hide');
        mod.initSelect();
    }

    // 选择易捷币 or 油卡 or 不使用该优惠
    mod.confirmYJBModal = function () {
        if (mod.tempActive != mod.active) {
            mod.active = mod.tempActive;
            mod.noSelected = false;
            mod.clearOtherState();
        }
        mod.hideYJBModal();
    }

    mod.clearOtherState = function () {
        // 是否启用易捷币
        if (mod.active == "yjb-yjb") {
            _pageCache.toggleYJB = 1;
            mod.useYjb = true;
            YJCOUPON.setMutex(true);
            YJCOUPON.disableIfCan();
            COUPON.setMutex(true);
            COUPON.disable(true);

        } else {
            _pageCache.toggleYJB = 0;
            mod.useYjb = false;
        }
        // 是否启用油卡
        if (mod.active == "yjb-youka") {
            YouKa.enable();
            YJCOUPON.setMutex(true);
            YJCOUPON.disableIfCan();
            COUPON.setMutex(true);
            COUPON.disable(true);
        } else {
            YouKa.disable();
        }

        if (mod.active == "yjb-none") {
            YJCOUPON.setMutex(false);
            COUPON.setMutex(false);
            $("#yjbBar .shop-info-list-text").html('<span class="unselect">不使用该优惠</span>');
            $("#toggleYJB").removeClass("active");
        }

        sessionStorage.CreateOrderInfo = JSON.stringify(_pageCache);
        // calcFreight(_hasAddress, 1);
        CalcPayPrice();
    }

    return mod;
})(window.YJB || {}, window.Zepto);

$(function () {
    // 启用易捷币开关
    $("#toggleYJB").on('click', function (e) {
        setTimeout(function () {
            if ($("#toggleYJB").is(".active")) {
                YJB.tempActive = 'yjb-yjb';
                YJB.confirmYJBModal();
            } else {
                YJB.tempActive = 'yjb-none';
                YJB.disable();
            }
        }, 100);
    });

    // // 启用易捷币开关
    // $("#toggleYJB").on('click', function (e) {
    //     setTimeout(function () {
    //         if ($("#toggleYJB").is(".active")) {
    //             _pageCache.toggleYJB = 1;
    //             YJB.useYjb = true;

    //             if (!_userYJCoupons.CanCombinabled) {
    //                 YJCOUPON.clear();
    //             }

    //         } else {
    //             _pageCache.toggleYJB = 0;
    //             YJB.useYjb = false;
    //         }
    //         sessionStorage.CreateOrderInfo = JSON.stringify(_pageCache);
    //         calcFreight(_hasAddress, 1);
    //         CalcPayPrice();
    //     }, 100);
    // });

    // 选择易捷币弹窗
    $('#yjb-modal').on('click', '.yjb-list', function () {
        var isSelect = $(this).hasClass('active');
        if (!isSelect) {
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');
            YJB.tempActive = $(this).attr("id");
        }
    });
});
/**********************YJB-End**********************/


/**********************YOUKA-油卡兑换券-Begin**********************/
window.YouKa = (function (mod, $, undefined) {

    var title = '此福利不能与其他优惠同时享受~~';

    // 可赠送油卡兑换券的店铺
    mod.appMap = {};

    //允许赠送油卡兑换券的店铺
    mod.enabledAppMap = {};

    mod.useYouka = false;

    mod.canUse = false;

    // 初始化
    mod.init = function (data) {
        if (data.AppYouKa && data.AppYouKa.length > 0) {
            mod.canUse = true;
            for (var i = 0; i < data.AppYouKa.length; i++) {
                var yk = data.AppYouKa[i];
                mod.appMap[yk.AppId] = yk;
            }
        }
        if (mod.canUse) {
            $("#youkaUl").removeClass("hide").find("span:last").text(title);
        } else {
            $("#youkaUl").hide();
        }
    };

    // // 初始化显示金额
    // mod.initDisplay = function (totalPrice) {
    //     var giveMoney = 0;
    //     for (var appId in mod.appMap) {
    //         if (sessionStorage.type != "gouwuche") giveMoney += mod.appMap[appId].YouKaPersent * totalPrice / 100;
    //         else giveMoney += mod.appMap[appId].GiveMoney;
    //     }
    //     if (giveMoney > 0) mod.setYoukaMoney(giveMoney);
    // };

    // 重置油卡卷显示状态
    mod.resetState = function (totalPrice) {
        // 不能与赠品 易捷币 抵现劵 优惠券同享

        if (IsPresell || isPromotion || _hasPresent || YJB.useYjb || COUPON.useCoupon() || YJCOUPON.isUse()) {
            if (isPromotion) {
                title = '此福利不能与活动同时享受~~';
            } else if (IsPresell) {
                title = '此福利不能与预售同时享受~~';
            } else if (_hasPresent) {
                title = '此福利不能与赠品同时享受~~';
            } else if (YJB.useYjb) {
                title = '此福利不能与易捷币同时享受~~';
            } else if (COUPON.useCoupon()) {
                title = '此福利不能与优惠券同时享受~~';
            } else if (YJCOUPON.isUse()) {
                title = '此福利不能与易捷抵现劵同时享受~~';
            }
            mod.disable();
        } else {
            mod.enable(totalPrice);
        }
    }

    // 启用
    mod.enable = function (totalPrice) {
        var giveMoney = 0;
        for (var appId in mod.appMap) {
            mod.enabledAppMap[appId] = 1;
            if (sessionStorage.type != "gouwuche") giveMoney += mod.appMap[appId].YouKaPersent * totalPrice / 100;
            else giveMoney += mod.appMap[appId].GiveMoney;
        }
        mod.setYoukaMoney(giveMoney);
        mod.useYouka = true;
    };

    // 禁用
    mod.disable = function () {
        for (var appId in mod.appMap) {
            if (mod.enabledAppMap.hasOwnProperty(appId)) delete mod.enabledAppMap[appId];
        }
        $("#youkaUl").find("span:last").text(title);
        mod.useYouka = false;
    };

    // 店铺是否赠送油卡券
    mod.hasApp = function (appId) {
        return mod.enabledAppMap.hasOwnProperty(appId);
    };

    mod.setYoukaMoney = function (giveMoney) {
        $("#yjb-user-youka-amount").text(giveMoney.toFixed(2));
        YJB.youkaPrice = giveMoney.toFixed(2);
        $("#youkaUl").removeClass("hide").find("span:last").text("支付后立即赠送" + getCookie("Currency") + giveMoney.toFixed(2) + "的油卡兑换券");
    }

    return mod;
})(window.YouKa || {}, window.Zepto)
/**********************YOUKA-油卡兑换券-End**********************/

/**********************YJCoupon-易捷抵用券-Begin**********************/
window.YJCOUPON = (function (mod, $, undefined) {
    // 当前选择的易捷抵用券    
    mod.currentCouponeIds = [];
    // 当前选择的易捷抵用券金额
    mod.currentCouponPrice = 0;
    // 当前易捷抵用券可抵用的金额
    mod.currentUseCouponPrice = 0;

    mod.isUse = function () {
        if (_userYJCoupons != null) {
            return !_userYJCoupons.CanCombinabled && mod.currentCouponeIds.length > 0;
        }
        else {
            return false;
        }
    }

    // 是否为互斥
    mod.mutex = false;

    mod.setMutex = function (isMutex) {
        if (mod.mutex != isMutex) {
            if (_userYJCoupons && !_userYJCoupons.CanCombinabled) {
                if (isMutex) {
                    $("#yjCouponUseInfo").html('<span class="unselect">当前优惠不可叠加使用</span>');
                } else {
                    $("#yjCouponUseInfo").html('<span class="unselect">请选择</span>');
                }
            }
            mod.mutex = isMutex;
        }
    }

    mod.disableIfCan = function () {
        if (_userYJCoupons && !_userYJCoupons.CanCombinabled) {
            YJCOUPON.clear();
        }
    }

    // 不使用易捷抵用券
    mod.clear = function () {
        if (this.currentCouponeIds.length > 0 || this.currentCouponPrice > 0) {
            this.currentCouponeIds = [];
            this.currentCouponPrice = 0;
            this.currentUseCouponPrice = 0;
            if (mod.mutex) {
                $("#yjCouponUseInfo").html('<span class="unselect">当前优惠不可叠加使用</span>');
            } else {
                $("#yjCouponUseInfo").html('<span class="unselect">请选择</span>');
            }
            $('#yj-coupon-lists-use').find('.icon').removeClass('checked');
        }
    };

    // 选择易捷抵用券金额
    mod.chooseYJCouponCore = function (ids, iscalcFreight) {
        this.currentCouponeIds = [];
        this.currentCouponPrice = 0;
        this.currentUseCouponPrice = 0;
        if (ids != null) {
            if (ids.length > 0) {
                $("#Inviter").removeClass("hide");
            }
            else {
                $("#Inviter").addClass("hide");
            }
        }
        if (ids) {
            _userYJCoupons.YJCoupons.filter(function (_) {
                return ids.indexOf(_.Id) >= 0
            }).forEach(function (_) {
                mod.currentCouponeIds.push(_.Id);
                mod.currentCouponPrice += (_.UsablePrice * 1000);
            });
            mod.currentCouponPrice = mod.currentCouponPrice / 1000;
            mod.currentUseCouponPrice = mod.currentCouponPrice;
        }
        if (this.currentCouponeIds.length > 0) {
            mod.mutex = false;
            $("#yjCouponUseInfo").html('<span class="red">抵用券抵现 ￥' + this.currentUseCouponPrice + '</span>');
            // 是否可以叠加使用
            if (!_userYJCoupons.CanCombinabled) {

                YJB.setMutex(true);
                YJB.disable();
                COUPON.setMutex(true);
                COUPON.disable();
            }
        } else {
            if (mod.mutex) {
                $("#yjCouponUseInfo").html('<span class="unselect">当前优惠不可叠加使用</span>');
            } else {
                $("#yjCouponUseInfo").html('<span class="unselect">请选择</span>');
            }
            YJB.setMutex(false);
            COUPON.setMutex(false);
        }
        CalcPayPrice();
    };
    mod.chooseYJCoupon = function () {
        $('#yj-coupon-wrap').removeClass('hide');
        $('body').addClass('lock');
    };

    mod.initEvent = function () {
        $('#yj-coupon-wrap').on('click', '.mk-navbar-list', function () {
            if ($(this).hasClass('active')) {
                return;
            }
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');
            $('#yj-coupon-wrap .yj-coupon-lists-wrap').addClass('hide').eq($(this).index()).removeClass('hide');
        });
        $('#yj-coupon-lists-use').on('click', '.coupon-lists', function () {
            var selectId = $(this).attr('id');
            if (_userYJCoupons.CanMultipuled) { //允许多张使用
                if ($(this).find('.icon').hasClass('checked')) {
                    $(this).find('.icon').removeClass('checked');
                    mod.currentCouponeIds.indexOf(selectId) !== -1 && mod.currentCouponeIds.splice(mod.currentCouponeIds.indexOf(selectId), 1)
                } else {
                    $(this).find('.icon').addClass('checked');
                    mod.currentCouponeIds.indexOf(selectId) == -1 && mod.currentCouponeIds.push(selectId)
                }

            } else { //只允许单张使用
                mod.currentCouponeIds = [selectId];
                $(this).parent().find('.icon').removeClass('checked');
                $(this).find('.icon').addClass('checked');
                $('#yj-is-use-coupon').hasClass('checked') && $('#yj-is-use-coupon').removeClass('checked');
                mod.closeYjCouponPage();
            }
        });
    };

    mod.closeYjCouponPage = function () {
        this.chooseYJCouponCore(this.currentCouponeIds, true);
        $('#yj-coupon-wrap').addClass('hide');
        $('body').removeClass('lock');
    };

    /**
     * 是否使用易捷抵用券
     */
    mod.isUseYJCoupon = function () {
        if ($('#yj-is-use-coupon').hasClass('checked')) {
            $('#yj-is-use-coupon').removeClass('checked');
        } else {
            $('#yj-is-use-coupon').addClass('checked');
            $('#yj-coupon-lists-use').find('.icon').removeClass('checked');
            this.currentCouponeIds = [];
        }
        this.closeYjCouponPage();
    };

    /**
     * 初始化抵用券数据
     */
    mod.initData = function (data) {
        if (!data.CanCombinabled) {
            YouKa.title = '此福利不能与赠品、优惠券、易捷抵现劵、易捷币同时享受~';
        }

        if (data.YJCoupons.length) {
            var html = '';
            for (var i = 0; i < data.YJCoupons.length; i++) {
                var item = data.YJCoupons[i];
                html += '<div class="coupon-lists pointer" appId="' + item.AppId + '" id="' + item.Id + '" value="' + item.Price + '">' +
                    '<div class="coupon-list">' +
                    '<div class="coupon-list-left">' +
                    (mod.currentCouponeIds.indexOf(item.Id) !== -1 ? '<div class="icon checked"></div>' : '<div class="icon"></div>') +
                    '<div class="context">' +
                    '<div class="name">' + item.Name + '</div>' +
                    '<div class="bottom">' +
                    '<div class="text">' + mod.textFilter(item.LimitAmount) + '</div>' +
                    '<div class="date">' + mod.dateFilter(item.EndTime) + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="coupon-list-right">' +
                    (item.Scope == 0 ? '<div class="title">商城通用</div>' : item.Scope == 1 ? '<div class="title">店铺优惠券</div>' : '<div class="title">商品优惠券</div>') +
                    '<div class="price">&#165 <span class="f30">' + item.Price + '</span></div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }
            $('#yj-coupon-lists-use').empty().append(html);
            if (_userYJCoupons.CanMultipuled) { //多选
                $('#yj-btn').removeClass('hide');
            } else { //单选
                $('#no-use-coupon').removeClass('hide');
                if (mod.currentCouponeIds.length) {
                    $('#yj-is-use-coupon').removeClass('checked');
                } else {
                    $('#yj-is-use-coupon').addClass('checked');
                }
            }
        } else {
            $('#yj-coupon-lists-use').empty().append('<p style="text-align: center;margin-top: 20px;">没有可用优惠券<p>');
        }
        if (data.UnusableYJCoupons.length) {
            var html = '';
            for (var i = 0; i < data.UnusableYJCoupons.length; i++) {
                var item = data.UnusableYJCoupons[i];
                html += '<div class="coupon-lists">' +
                    '<div class="coupon-list">' +
                    '<div class="coupon-list-left">' +
                    '<div class="context">' +
                    '<div class="name">' + item.Name + '</div>' +
                    '<div class="bottom">' +
                    '<div class="text">' + mod.textFilter(item.LimitAmount) + '</div>' +
                    '<div class="date">' + mod.dateFilter(item.EndTime) + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="coupon-list-right">' +
                    (item.Scope == 0 ? '<div class="title">商城通用</div>' : item.Scope == 1 ? '<div class="title">店铺优惠券</div>' : '<div class="title">商品优惠券</div>') +
                    '<div class="price">&#165 <span class="f30">' + item.Price + '</span></div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }
            if (html == '') {
                html = '<p>未找到相关优惠券<p>'
            }
            $('#yj-coupon-lists-unuse').empty().append(html);
        } else {
            $('#yj-coupon-lists-unuse').empty().append('<p style="text-align: center;margin-top: 20px;">没有可用优惠券<p>');
        }
        this.initEvent();
    };

    mod.textFilter = function (value) {
        if (value - 0 > 0) {
            return '满' + value + '元使用';
        } else {
            return '无门槛优惠券';
        }
    };

    mod.dateFilter = function (value) {
        var date = new Date(Number(value.split('Date(')[1].split(')/')[0]));
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        return '有效期至' + year + '年' + month + '月' + day + '日'
    };

    return mod;
})(window.YJCOUPON || {}, window.Zepto);
/**********************YJCoupon-易捷抵用券-End**********************/

/**********************Basket-购物车下单相关-Begin**********************/
window.Basket = (function (mod, $, undefined) {
    // 返回购物车
    mod.gotoBasket = function () {
        if (getQueryString("fromApp") == 1) {
            window.location.href = "jhoabrowser://closeCurrentPagetFuction?args=&tag=";
        } else {
            window.history.back();
        }
    };

    mod.invalidItemIds = [];
    mod.removeInvalidCommodity = function () {
        if (mod.invalidItemIds.length > 0) {
            var data = JSON.parse(sessionStorage.ShopCartDate);
            mod.invalidItemIds.forEach(function (id) {
                data.forEach(function (d, dindex) {
                    var index = d.ShoppingCartItemSDTO.findIndex(function (i) {
                        return i.ShopCartItemId == id;
                    });
                    if (index > -1) d.ShoppingCartItemSDTO.splice(index, 1);
                    if (d.ShoppingCartItemSDTO.length == 0) {
                        data.splice(dindex, 1);
                    }
                });
            });

            if (data.length == 0) {
                mod.gotoBasket();
                return;
            }

            sessionStorage.ShopCartDate = JSON.stringify(data);
            displayCartInfo();
            calcFreight(_hasAddress, 1);
            $("#sold-out-wrap").hide();
        }
    };

    return mod;
})(window.Basket || {}, window.Zepto);
/**********************Basket-购物车下单相关-End**********************/

/**********************Invoice-发票管理-Begin**********************/
window.Invoice = (function (mod, $, undefined) {
    // 打开发票管理页面
    mod.openInvoicePage = function () {
        logBTP(sessionStorage.SrcType, service_type, "0x0001", '', logOrderId);
        document.location.href = urlAppendCommonParams("/Mobile/InvoiceInfo?appIds=" + appIds);
    }

    mod.initDisplay = function () {
        var invoice = JsVilaDataNull(sessionStorage.InvoiceDTO) && $.parseJSON(sessionStorage.InvoiceDTO);
        if (invoice && sessionStorage._useInvoice) {
            var invText = '';
            if (invoice.Category == 1) {
                invText = "增值税普通发票";
            } else if (invoice.Category == 2) {
                invText = "电子发票";
            } else if (invoice.Category == 3) {
                invText = "不要发票";
            } else if (invoice.Category == 4) {
                invText = "增值税专用发票";
            }
            $("#invoiceBar .shop-info-list-text").html('<span class="red">' + invText + '</span>');
        } else {
            $("#invoiceBar .shop-info-list-text").html('<span class="unselect">暂不开发票</span>');
        }
    }
    return mod;
})(window.Invoice || {}, window.Zepto);
/**********************Invoice-发票管理-End**********************/

/**********************YJStoreCoupon-跨店满减券-Begin**********************/
window.StoreCoupon = (function (mod, $, undefined) {
    //跨店满减券,商品的金额合计
    mod.StoreCouponCommdityPrice = 0;
    //跨店满减券的商品的数量，用以拆单的时候，最后一单的计算
    mod.StoreCouponCommdityCount = 0;
    // 当前用的面值
    mod.StoreCouponPrice = 0;
    // 当前选择的    
    mod.currentCouponeIds = [];
    // 当前选择的
    mod.currentCouponPrice = 0;
    mod.useLists = [];
    mod.unuseLists = [];
    mod.Commodities = [];
    mod.CommodityDivid = [];

    //是否使用了跨店满减券
    mod.UseStoreCoupon = function () {
        return $("#store-coupon-wrap .coupon-lists-use .icon.checked").parents(".coupon-lists.pointer").attr("id");
    };

    mod.inte2 = function () {
        if (sessionStorage.type == "gouwuche") {
            for (var i = 0; i < _pageAllInfoData.Dutys.List.length; i++) {
                var list = _pageAllInfoData.Dutys.List[i];
                for (var j = 0; j < list.Coms.length; j++) {
                    var item = list.Coms[j];
                    var obj = {};
                    obj.CommodityId = item.CommodityId;
                    obj.Num = item.Num;
                    obj.RealPrice = item.RealPrice;
                    obj.AppId = item.AppId;
                    obj.isThirdParty = $('[commodityid="' + item.CommodityId + '"]').length == 0;//等于0 说明是第三方的[owner="self"]  这个判断在手机上会挂掉
                    mod.Commodities.push(obj);
                }
            }
        }

        getDataAjax2({
            url: "/Mobile/GetMyCoupon",//获取跨店铺满减券
            data: {
                UserId: getUserId(),
                EsAppId: getEsAppId(),
                Commodities: mod.Commodities,
                CouponTemplateType: "StoreCoupon" //1,对应C#的枚举
            },
            callback: function (result) {
                if (result.length > 0) {//跨店铺满减券的个数大于0
                    mod.showCanUseStoreCoupon(true);
                    mod.initData(result);//初始化优惠券列表
                    var m = false;
                    for (var j = 0; j < mod.Commodities.length; j++) {
                        var item = mod.Commodities[j]; 
                        if (!item.isThirdParty) {// 自营的。
                            m = true;
                            break;
                        }
                    }
                    mod.NoUse(m);
                }
                else {
                    mod.showCanUseStoreCoupon(false);
                }
            }
        });
    };

    //不使用跨店满减券的函数体
    mod.NoUse = function (Mutex) {
        $('#store-coupon-wrap .icon').removeClass('checked');//去除所有选中
        $("#store-coupon-wrap .no-use-coupon").find('.icon').addClass('checked');//把当前的选中。

        if (Mutex)//其它优惠券把它挤下去了
            $("#hasStoreCoupon .shop-info-list-text").text('当前优惠不可叠加使用');
        else
            $("#hasStoreCoupon .shop-info-list-text").text('请选择');
        mod.StoreCouponCommdityPrice = 0;
        mod.StoreCouponCommdityCount = 0;

    }
    mod.initEvent = function () {
        $('#store-coupon-wrap').on('click', '.mk-navbar-list', function () {//弹框的内容 可用跨店满减券，不可用跨店满减券
            if ($(this).hasClass('active')) {
                return;
            }
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');

            $('#store-coupon-wrap .coupon-lists-wrap').addClass('hide').eq($(this).index()).removeClass('hide');
        });

        $('#store-coupon-wrap .no-use-coupon').on('click', function () {//不使用用跨店满减
            if (!$(this).find('.icon').hasClass('checked')) {

                if (!mod.UseStoreCoupon())//没有使用跨店满减，直接跳过去
                    return;

                mod.NoUse();

                mod.closeCouponPage();

                if (!(COUPON.useCoupon())) {

                    YJB.setMutex(false);
                    YJCOUPON.setMutex(false);
                }

                showFreightCouponInfo(COUPON.freightMultiApps, 1);
            }
        });

        //选中某一个优惠券，切换优惠券
        $('#store-coupon-wrap .coupon-lists-use .coupon-lists').on('click', function () {
            var th = $(this).find('.icon');
            if (!th.hasClass('checked')) {     //点击的已经是选中的直接退出
                var couponValue = $(this).attr('value');//选中的优惠券的面值
                var LimitCondition = $(this).attr('LimitCondition');

                for (var j = 0; j < mod.Commodities.length; j++) {
                    var commdity = mod.Commodities[j];
                    var v = $("[commodityid='" + commdity.CommodityId + "']").parents("[owner='self']").find(".spanCouponAmount [co-tag='CouponAmountShop']").text();
                    if (!v) //为null
                        commdity.UsedCoupon = false;//这个订单没使用优惠券，设置为0
                    else if (parseFloat(v) == 0)
                        commdity.UsedCoupon = false;
                    else if (parseFloat(v) > 0)
                        commdity.UsedCoupon = true;//这个订单已经使用了，这个商品就是使用了优惠券，不能再使用跨店满减券
                }


                var yjCouponValue = YJCOUPON.currentUseCouponPrice;
                getDataAjax2({
                    url: "/Mobile/getStoreCouponCommidtyTotalPrice",
                    data: {
                        UserId: getUserId(),
                        EsAppId: getEsAppId(),
                        Commodities: mod.Commodities,
                        CouponTemplateType: "StoreCoupon", //1,对应C#的枚举,跨店满减券
                        CouponId: $(this).attr('id'),
                        CouponType: $(this).attr('CouponType'),
                        StoreCouponValue: couponValue //> yjCouponValue;
                    },

                    callback: function (result) {

                        mod.CommodityDivid = result.li;
                        if (result.StoreCouponCommdityPrice <= 0)
                            toast("没有可以使用跨店铺满减券的商品");
                        else {   //商品金额合计大于0 

                            if (result.StoreCouponCommdityPrice < LimitCondition) {//商品的价格合计小于 限制
                                toast("商品金额不满足该跨店满减券的使用条件");
                                return;
                            }


                            var coupons = COUPON.freightMultiApps.CouponResult.Data;
                            if (coupons) {
                                for (var i = 0; i < coupons.length; i++) {
                                    if ($(`[co-tag-appid="${coupons[i].ShopId}"][owner="self"]`).length > 0 || (sessionStorage.type != "gouwuche")) {//使用了跨店，不使用一般优惠券
                                        coupons[i].Cash = 0;
                                        coupons[i].CouponId = '';
                                    }
                                }
                            }

                            $('#store-coupon-wrap .icon').removeClass('checked');//去除所有选中
                            th.addClass('checked');//把当前的选中。

                            mod.StoreCouponCommdityPrice = result.StoreCouponCommdityPrice;
                            mod.StoreCouponCommdityCount = result.StoreCouponCommdityCount;
                            mod.StoreCouponPrice = couponValue;
                            var couponValueNode = $("#hasStoreCoupon .shop-info-list-text");//主界面上的优惠值的节点。
                            couponValueNode.html('-￥' + (mod.StoreCouponCommdityPrice > mod.StoreCouponPrice ? mod.StoreCouponPrice : mod.StoreCouponCommdityPrice));
                            couponValueNode.parents("li").show();

                            couponValueNode.parent().removeClass('hide');
                            couponValueNode.parent().next().addClass('hide');
                            // 禁用YJB
                            YJB.setMutex(true);
                            YJB.disable();
                            // 禁用抵用券
                            YJCOUPON.setMutex(true);
                            YJCOUPON.disableIfCan();

                            // COUPON.useCoupon = true;
                            showFreightCouponInfo(COUPON.freightMultiApps, 1);
                        }
                    }
                });
            }
            mod.closeCouponPage();
        });
    };

    mod.disable = function () {
        mod.NoUse();//不使用跨店满减券
    }



    ///给店铺增加是自营还是第三方的属性
    mod.AddTempalteTypeAttr = function () {
        mod.Commodities = [];//清空商品
        return new Promise(function (resolve, reject) {
            if (sessionStorage.type == "gouwuche") {
                var appids = new Array();
                for (var i = 0; i < $("#divShopCommoditys >div").length; i++) {
                    appids.push($($("#divShopCommoditys >div")[i]).attr("co-tag-appid"));
                }
                
                getDataAjax2({
                    url: "/Mobile/getShopType",
                    data: {
                        appId: appids,
                        EsAppId: getEsAppId()
                    },
                    callback: function (result) {
                        var i = 0;
                        var hasSelf = false;

                        mod.showCanUseStoreCoupon(false);
                        for (var i = 0; i < result.length; i++) {
                            var owner = result[i];
                            $($("#divShopCommoditys >div")[i]).attr("owner", owner);
                            if (owner == "self") //如果是自营的，显示出来
                            {
                                mod.showCanUseStoreCoupon(true);
                                hasSelf = true;
                            }
                        }
                        if (hasSelf)//如果有自营的，继续执行
                            mod.inte2();
                        YJCard.YjcCommodityInfo.appId = appids;
                        YJCard.YjcCommodityInfo.types = result;
                        CalcPayPrice();
                    }
                });
            }
            else {
                getDataAjax2({
                    url: "/Mobile/getShopType",
                    data: {
                        appId: [sessionStorage.appId],
                        EsAppId: getEsAppId()
                    },
                    callback: function (result) {
                        for (var i = 0; i < result.length; i++) {
                            var owner = result[i];
                            $("#siglecom").attr("owner", owner);

                            if (owner == "tp") {//这个返回 只有一个元素，如果这个是店铺第三方的，隐藏跨店满减券项
                                mod.showCanUseStoreCoupon(false);
                            }
                            else {
                                mod.showCanUseStoreCoupon(true);
                                mod.Commodities.push({
                                    AppId: sessionStorage.appId,
                                    RealPrice: commodityUpInfo.realPrice,
                                    Num: commodityUpInfo.number,
                                    CommodityId: sessionStorage.commodityId_2,
                                    isThirdParty: owner == "tp"// tp是第三方
                                });
                                _currentCouponAppId = sessionStorage.appId;
                                mod.inte2();
                            }
                        }
                        YJCard.YjcCommodityInfo.appId = [sessionStorage.appId];
                        YJCard.YjcCommodityInfo.types = result;
                        CalcPayPrice();
                    }
                });
            }
        });
    };

    //显示跨店满减券，是否可用，在主界面上
    mod.showCanUseStoreCoupon = function (show) {
        if (show) {
            $("#hasStoreCoupon").css("display", "flex");
            $('#hasStoreCoupon').next().css("display", "none");
        }
        else {
            $("#hasStoreCoupon").css("display", "none");
            $('#hasStoreCoupon').next().css("display", "flex");
        }
    }


    mod.init = function ()//初始化界面，打开界面的时候执行
    {
        mod.AddTempalteTypeAttr();
    };

    mod.AttrStr = function (name, value) {
        return ' ' + name + '="' + value + '"';
    }

    mod.initData = function (result) {
        var optional = 0;
        var index = 0;
        mod.useLists = [];
        mod.unuseLists = [];
        for (var i = 0; i < result.length; i++) {
            if (result[i].IsUsable) {
                mod.useLists.push(result[i]);
                if (optional < result[i].Cash)
                    optional = result[i].Cash;
            } else {
                mod.unuseLists.push(result[i]);
            }
        }

        if (this.useLists.length > 0) {
            var html = '';
            for (var i = 0; i < this.useLists.length; i++) {
                var item = this.useLists[i];
                html += '<div class="coupon-lists pointer" ' +

                    mod.AttrStr('LimitCondition', item.LimitCondition) + mod.AttrStr('CouponType', item.CouponType) + mod.AttrStr('appId', item.AppId) + mod.AttrStr('id', item.Id) + mod.AttrStr('value', item.Cash) + " >" +
                    '<div class="coupon-list" index>' +
                    '<div class="coupon-list-left">' +
                    (this.oldSelectId == item.Id ? '<div class="icon checked"></div>' : '<div class="icon"></div>') +
                    '<div class="context">' +
                    '<div class="name">' + item.Name + '</div>' +
                    '<div class="bottom">' +
                    '<div class="text">' + mod.textFilter(item.LimitCondition) + '</div>' +
                    '<div class="date">' + mod.dateFilter(item.EndTime) + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="coupon-list-right">' +
                    (item.Type == 0 ? '<div class="title">店铺优惠券</div>' : '<div class="title">商品优惠券</div>') +
                    '<div class="price">&#165 <span class="f30">' + item.Cash + '</span></div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }

            $('#store-coupon-wrap .coupon-lists-use').empty().append(html);
        }
        else
            $('#store-coupon-wrap .coupon-lists-use').empty().append('<p style="text-align: center;margin-top: 20px;">没有可用优惠券<p>');

        if (this.unuseLists.length > 0) {
            var html = '';
            for (var i = 0; i < this.unuseLists.length; i++) {
                var item = this.unuseLists[i];
                html += '<div class="coupon-lists">' +
                    '<div class="coupon-list">' +
                    '<div class="coupon-list-left">' +
                    '<div class="context">' +
                    '<div class="name">' + item.Name + '</div>' +
                    '<div class="bottom">' +
                    '<div class="text">' + mod.textFilter(item.LimitCondition) + '</div>' +
                    '<div class="date">' + mod.dateFilter(item.EndTime) + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="coupon-list-right">' +
                    (item.Type == 0 ? '<div class="title">店铺优惠券</div>' : '<div class="title">商品优惠券</div>') +
                    '<div class="price">&#165 <span class="f30">' + item.Cash + '</span></div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }

            if (html == '') {
                html = '<p>未找到相关优惠券<p>'
            }
            $('#store-coupon-wrap .coupon-lists-unuse').empty().append(html);
        }
        else
            $('#store-coupon-wrap .coupon-lists-unuse').empty().append('<p style="text-align: center;margin-top: 20px;">没有可用优惠券<p>');

        this.initEvent();
        return optional;//返回最大的优惠券的值
    };

    mod.isUse = function () {

        if (_userYJCoupons != null) {
            return !_userYJCoupons.CanCombinabled && mod.currentCouponeIds.length > 0;
        }
        else {
            return false;
        }
    }

    // 是否为互斥
    mod.mutex = false;

    mod.setMutex = function (isMutex) {
        if (mod.mutex != isMutex) {
            if (_userYJCoupons && !_userYJCoupons.CanCombinabled) {
                if (isMutex) {
                    $("#yjCouponUseInfo").html('<span class="unselect">当前优惠不可叠加使用</span>');
                } else {
                    $("#yjCouponUseInfo").html('<span class="unselect">请选择</span>');
                }
            }
            mod.mutex = isMutex;
        }
    }

    mod.disableIfCan = function () {
        if (_userYJCoupons && !_userYJCoupons.CanCombinabled) {
            YJCOUPON.clear();
        }
    }

    // 不使用
    mod.clear = function () {
        if (this.currentCouponeIds.length > 0 || this.currentCouponPrice > 0) {
            this.currentCouponeIds = [];
            this.currentCouponPrice = 0;
            this.currentUseCouponPrice = 0;
            if (mod.mutex) {
                $("#yjCouponUseInfo").html('<span class="unselect">当前优惠不可叠加使用</span>');
            } else {
                $("#yjCouponUseInfo").html('<span class="unselect">请选择</span>');
            }
            $('#yj-coupon-lists-use').find('.icon').removeClass('checked');
        }
    };

    //  弹窗
    mod.chooseStoreCoupon = function () {
        $('#store-coupon-wrap').removeClass('hide');
        $('body').addClass('lock');
        $($('#store-coupon-wrap .mk-navbar-list')[0]).addClass('active');
        $($('#store-coupon-wrap .mk-navbar-list')[1]).removeClass('active');
        $('#store-coupon-wrap .coupon-lists-wrap').addClass('hide').eq(0).removeClass('hide');
    };


    //如果有自营店铺使用优惠券,返回false； 没有，就是可以使用，返回true
    mod.CanUseStoreCoupon = function () {
        if (sessionStorage.type == "gouwuche") {
            var node = $("[owner='self'] .spanCouponAmount  span[co-tag]");
            for (var i = 0; i < node.length; i++) {
                if ($(node[i]).text() != '0')//如果有自营店铺使用了优惠券
                {
                    //不使用跨店满减券
                    return false;
                }
            }
            return true;
        }
        else {
            return $("#spanCouponAmount").text() == '0'; //0是不使用，返回true。可以使用跨店满减券
        }
    }

    mod.closeCouponPage = function () {
        $('#store-coupon-wrap').addClass('hide');
        $('body').removeClass('lock');
    };

    /**
     * 是否使用
     */
    mod.isUseYJCoupon = function () {
        if ($('#yj-is-use-coupon').hasClass('checked')) {
            $('#yj-is-use-coupon').removeClass('checked');
        } else {
            $('#yj-is-use-coupon').addClass('checked');
            $('#yj-coupon-lists-use').find('.icon').removeClass('checked');
            this.currentCouponeIds = [];
        }
        this.closeYjCouponPage();
    };



    mod.textFilter = function (value) {
        if (value - 0 > 0) {
            return '满' + value + '元使用';
        } else {
            return '无门槛优惠券';
        }
    };

    mod.dateFilter = function (value) {
        var date = new Date(Number(value.split('Date(')[1].split(')/')[0]));
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        return '有效期至' + year + '年' + month + '月' + day + '日'
    };

    return mod;
})(window.StoreCoupon || {}, window.Zepto);
/**********************YJStoreCoupon-跨店满减券-End**********************/


/**********************Coupon-优惠券-Begin**********************/
window.COUPON = (function (mod, $, undefined) {
    mod.currentId = '';
    mod.useLists = [];
    mod.unuseLists = [];
    mod.oldSelectId = '';
    mod.seleValue = 0;
    mod.el = null;
    mod.freightMultiApps = {}; //保存获取的优惠券运费等等信息

    mod.useCoupon = function () {
        if (sessionStorage.type == "gouwuche") {
            for (var i = 0; i < $("#divShopCommoditys .spanCouponAmount [co-tag='CouponAmountShop']").length; i++) {
                var p = $($("#divShopCommoditys .spanCouponAmount [co-tag='CouponAmountShop']")[i]).text();
                if (p > 0)
                    return true;
            }
        }
        else {
            if (parseFloat($("#spanCouponAmount").text()) > 0)
                return true;
        }
        if (StoreCoupon.UseStoreCoupon())
            return true;
        else
            return false;
    }

    ///弹窗是第三方的还是自营的。
    mod.popupReason = "";
    // 互斥
    mod.mutex = false;
    mod.setMutex = function (isMutex) {
        mod.mutex = isMutex;
        if (isMutex) {
            $("#coupon-wrap .notUseCoupon").text('当前优惠不可叠加使用');
        } else {
            $("#coupon-wrap .notUseCoupon").text('请选择');

            StoreCoupon.NoUse();//跨店铺满减券
        }
    };

    // 初始化显示
    mod.initDisplayWhenBasket = function (couponResult) {
        if (mod.useCoupon()) {
            if (couponResult != null && couponResult.length > 0) {
                YJB.setMutex(true);
                YJB.disable();
                YJCOUPON.setMutex(true);
                YJCOUPON.disableIfCan();
            } else {
                YJB.setMutex(false);
                YJCOUPON.setMutex(false);
            }
        }
    };
    mod.initDisplay = function (couponResult) {
        if (!mod.mutex) {
            if (couponResult != null && couponResult.length > 0) {
                if (mod.useCoupon()) {
                    YJB.setMutex(true);
                    YJB.disable();
                    YJCOUPON.setMutex(true);
                    YJCOUPON.disableIfCan();
                }
            } else {
                YJB.setMutex(false);
                YJCOUPON.setMutex(false);
            }
        }
    };


    // 已使用其他优惠，禁用优惠卷
    mod.disable = function (isMutex) {
        $('#divShopCommoditys .notUseCoupon').removeClass("hide");
        $('#divShopCommoditys [co-tag="CouponAmountShop"]').parent().addClass("hide");
        $('#divShopCommoditys [co-tag="CouponAmountShop"]').text("0");
        StoreCoupon.NoUse(true);//不使用跨店满减券
        $("#coupon-wrap .notUseCoupon").text('当前优惠不可叠加使用');

        if (mod.freightMultiApps.CouponResult.Data) {//这样写也没问题，优惠券信息会在弹窗弹出时重新获取。
            mod.freightMultiApps.CouponResult.Data.forEach(function (c) {
                c.Cash = 0;
                c.CouponId = '';
            });
        }
        showFreightCouponInfo(mod.freightMultiApps, 1);
        this.closeCouponPage();
        if (!isMutex) {
            YJB.setMutex(false);
            YJCOUPON.setMutex(false);
        }
    };

    /**
     * 弹出 优惠券窗口
     */
    mod.chooseCoupon = function (el) {
        var appId = '';
        var Commodities = [];
        if (sessionStorage.type == "gouwuche") {
            appId = $(el).parents('div').attr("co-tag-appid");
            for (var i = 0; i < _pageAllInfoData.Dutys.List.length; i++) {
                var list = _pageAllInfoData.Dutys.List[i];
                if (list.AppId == appId) {
                    var item = list.Coms;
                    for (var j = 0; j < item.length; j++) {
                        var obj = {};
                        obj.CommodityId = item[j].CommodityId;
                        obj.Num = item[j].Num;
                        obj.RealPrice = item[j].RealPrice;
                        obj.AppId = item[j].AppId;
                        Commodities.push(obj);
                    }
                    break;
                }
            }
            _currentCouponAppId = appId;
        } else { //直接下单
            Commodities.push({
                AppId: sessionStorage.appId,
                RealPrice: commodityUpInfo.realPrice,
                Num: commodityUpInfo.number,
                CommodityId: sessionStorage.commodityId_2
            });
            _currentCouponAppId = sessionStorage.appId;
        }
        var id = $(el).data('couponid');
        mod.getCoupon(id, Commodities);
        mod.popupReason = $(el).parents("[owner]").attr("owner");
    };

    mod.getCoupon = function (selectId, Commodities) {
        this.oldSelectId = selectId;
        getDataAjax2({
            url: "/Mobile/GetMyCoupon",
            data: {
                UserId: getUserId(),
                EsAppId: getEsAppId(),
                Commodities: Commodities,
                CouponTemplateType: 0
            },
            callback: function (result) {
                if (result.length) {
                    mod.useLists = [];
                    mod.unuseLists = [];
                    for (var i = 0; i < result.length; i++) {
                        if (result[i].IsUsable) {
                            mod.useLists.push(result[i]);
                        } else {
                            mod.unuseLists.push(result[i]);
                        }
                    }
                    mod.initDate();
                }
                $('#coupon-wrap').removeClass('hide');
                $('.coupon-lists-wrap').removeClass('hide');
                $('#coupon-lists-unuse').addClass('hide');
                $('.mk-navbar-list').removeClass('active');
                $('.mk-navbar-list').eq(0).addClass('active');
                $('body').addClass('lock');
            },
            error: function () {
                ajaxLoadingSingle.hide();
            }
        });
    };

    mod.initEvent = function () {
        $('#coupon-wrap').on('click', '.mk-navbar-list', function () {
            if ($(this).hasClass('active')) {
                return;
            }
            $(this).parent().find('.active').removeClass('active');
            $(this).addClass('active');
            $('#coupon-wrap .coupon-lists-wrap').addClass('hide').eq($(this).index()).removeClass('hide');
        });

        //选择改变优惠券
        $('#coupon-lists-use .coupon-lists').on('click', function () {
            if ($(this).find('.icon').hasClass('checked')) {
                mod.closeCouponPage();
                return;
            }

            var selectCouponId = $(this).attr('id');
            var seleValue = $(this).attr('value');
            var appId = $(this).attr('appId');
            if (mod.popupReason == "self")//($(`[co-tag-appid="${appId}"][owner="self"]`))//如果当前选择的是自营的优惠券
                StoreCoupon.NoUse(true);//不使用跨店满减券
            mod.setInfo(selectCouponId, seleValue, $(this).attr('appId'));
        });
    };

    mod.initDate = function () {
        if (this.useLists.length) {
            var html = '';
            for (var i = 0; i < this.useLists.length; i++) {
                var item = this.useLists[i];
                html += '<div class="coupon-lists pointer" appId="' + item.AppId + '" id="' + item.Id + '" value="' + item.Cash + '">' +
                    '<div class="coupon-list">' +
                    '<div class="coupon-list-left">' +
                    (this.oldSelectId == item.Id ? '<div class="icon checked"></div>' : '<div class="icon"></div>') +
                    '<div class="context">' +
                    '<div class="name">' + item.Name + '</div>' +
                    '<div class="bottom">' +
                    '<div class="text">' + mod.textFilter(item.LimitCondition) + '</div>' +
                    '<div class="date">' + mod.dateFilter(item.EndTime) + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="coupon-list-right">' +
                    (item.Type == 0 ? '<div class="title">店铺优惠券</div>' : '<div class="title">商品优惠券</div>') +
                    '<div class="price">&#165 <span class="f30">' + item.Cash + '</span></div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }
            $('#coupon-lists-use').empty().append(html);
            if (this.oldSelectId == '') {
                $('#is-use-coupon').addClass('checked');
            } else {
                $('#is-use-coupon').removeClass('checked');
            }
        } else {
            $('#coupon-lists-use').empty().append('<p style="text-align: center;margin-top: 20px;">没有可用优惠券<p>');
        }
        if (this.unuseLists.length) {
            var html = '';
            for (var i = 0; i < this.unuseLists.length; i++) {
                var item = this.unuseLists[i];
                html += '<div class="coupon-lists">' +
                    '<div class="coupon-list">' +
                    '<div class="coupon-list-left">' +
                    '<div class="context">' +
                    '<div class="name">' + item.Name + '</div>' +
                    '<div class="bottom">' +
                    '<div class="text">' + mod.textFilter(item.LimitCondition) + '</div>' +
                    '<div class="date">' + mod.dateFilter(item.EndTime) + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="coupon-list-right">' +
                    (item.Type == 0 ? '<div class="title">店铺优惠券</div>' : '<div class="title">商品优惠券</div>') +
                    '<div class="price">&#165 <span class="f30">' + item.Cash + '</span></div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            }
            if (html == '') {
                html = '<p>未找到相关优惠券<p>'
            }
            $('#coupon-lists-unuse').empty().append(html);
        } else {
            $('#coupon-lists-unuse').empty().append('<p style="text-align: center;margin-top: 20px;">没有可用优惠券<p>');
        }
        this.initEvent();
    };

    mod.NoUse_thirdParty = false;
    /**
     * 弹窗上  选择不使用优惠券
     */
    mod.isUseCoupon = function () {
        if ($(this).find('.icon').hasClass('checked')) {
            this.closeCouponPage();
            return;
        }

        var appid = $("#coupon-lists-unuse").prev().find(".coupon-lists.pointer").attr("appid");
        this.setInfo('', 0, appid);

        $("#coupon-wrap .notUseCoupon").text('不使用优惠券');

        mod.popupReason = '';
        // 不使用优惠卷，所有的优惠券都没有用，包含跨店满减券
        if (!(mod.useCoupon())) {
            YJB.setMutex(false);
            YJCOUPON.setMutex(false);
        }

    };

    mod.setInfo = function (selectCouponId, seleValue, appId) {
        var coupons = mod.freightMultiApps.CouponResult.Data;
        if (coupons) {
            for (var i = 0; i < coupons.length; i++) {
                if (mod.mutex || coupons[i].ShopId == appId) {//coupons[i].CouponId == mod.oldSelectId
                    coupons[i].Cash = seleValue;
                    coupons[i].CouponId = selectCouponId;
                    break;
                }
            }
        }
        if (selectCouponId) {
            // 禁用YJB
            YJB.setMutex(true);
            YJB.disable();
            // 禁用抵用券
            YJCOUPON.setMutex(true);
            YJCOUPON.disableIfCan();
        }

        showFreightCouponInfo(mod.freightMultiApps, 1);

        this.closeCouponPage();
    };

    mod.closeCouponPage = function () {
        $('#coupon-wrap').addClass('hide');
        $('body').removeClass('lock');
    };

    mod.textFilter = function (value) {
        if (value - 0) {
            return '满' + value + '元可用';
        } else {
            return '无门槛优惠券';
        }
    };
    mod.dateFilter = function (value) {
        var date = new Date(Number(value.split('Date(')[1].split(')/')[0]));
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        return '有效期至' + year + '年' + month + '月' + day + '日'
    };
    return mod;
})(window.COUPON || {}, window.Zepto)
/**********************Coupon-优惠券-End**********************/





//易捷卡
var YJCard = (function ($) {
    var yjc = {
        init: function () {
            var yjcid = getQueryString('yjcid'); if (yjcid) {
                sessionStorage.yjcid = yjcid;
                if (yjcid.split(',').length)
                    $("#yjctip_L1").show();
                $("#yjctip_L2").text(yjcid.split(',').length);
            } else {
                sessionStorage.removeItem("yjcid");
                sessionStorage.removeItem("yjcamt");
            }
            if (yjc.isShowYJC) {
                $("#YJCardSelect").show();
            } 
            //点击“易捷卡栏目”弹出选择易捷卡页面。
            $("#YJCardSelect").on("click", function () {
                window.location.href = 'http://testyjb.iuoooo.com/YJCard/CardSelector?backurl=' + location.href;
            });
            //点击“确定”将选中的易捷卡返回。
            $("#YJCardBack").on("click", function () {
            });
        },
        //todo提交订单前判断是否用易捷卡付款 是？：弹窗输入手机验证码
        yjcardSubmit: function () {
            //debugger
            if (sessionStorage.getItem('yjcid') && sessionStorage.getItem('yjcamt')) {//使用了易捷卡支付
                if (YJCard.Phone.isEnableValid && !YJCard.Phone.isValidate)//如果未验证手机
                {
                    //验证码弹窗
                    ajaxLoadingSingle.hide();
                    showDialog('#YjcPayDialog');
                    return false;
                }
                return true;
            }
            return true;
        },
        //todo 获取手机验证码
        GetValidateCode: function () {
            var yjcValidCode_cachekey = NewGuid();//生成一个guid作为缓存的key
            if (yjcValidCode_cachekey) { sessionStorage.yjcValidCode_cachekey = yjcValidCode_cachekey }
            $.post("/Mobile/GetYJCValidateCode", { "cachekey": sessionStorage.yjcValidCode_cachekey,
                YJCId: YJCard.YjcList[0].Id
            }, function (data) {
                //显示手机号
                var phone = YJCard.YjcList[0].Phone;
                if (!phone) {
                    toast("用户未绑定手机号或手机号格式不正确");
                    return;
                }
                $("#yjcPhone").text("我们已向您的手机号" + phone + "发送了一条验证码短信，请在下方填写进行校验");
                //debugger
                yjc._SetTime();
            })
        },
        //点击确定
        Validate: function (code) {
            //debugger
            if (!sessionStorage.getItem("yjcValidCode_cachekey")) {
                alert('请先获取验证码！')
                return;
            }
            if (!code) {
                toast("请输入验证码！");
                return;
            }
            //后台校验验证码是否正确 //todo
            $.post("/Mobile/CheckValidateCode", {
                "cachekey": sessionStorage.yjcValidCode_cachekey,
                "Code": code
            }, function (data) {
                //debugger
                if (data.isSuccess) {//todo 验证通过
                    yjc.Phone.isValidate = true;
                    sessionStorage.removeItem('yjcValidCode_cachekey');
                    //关闭弹窗
                    hideDialog('#YjcPayDialog');
                    toast("验证通过");
                } else {
                    //todo提示验证不通过的消息
                    toast("验证码输入不正确！");
                }
            })

        },
        //获取验证码60秒倒计时
        _SetTime: function () {
            //todo倒计时60秒
            if (yjc.Phone.countdown == 0) {
                $("#GetValidateCode").removeAttr("disabled");
                $("#GetValidateCode").attr("value", "获取验证码");
                yjc.Phone.countdown = 60;
            } else {
                $("#GetValidateCode").attr("disabled", true);
                $("#GetValidateCode").attr("value", "重新发送(" + yjc.Phone.countdown + ")");
                yjc.Phone.countdown--;
                setTimeout(function () {
                    yjc._SetTime()
                }, 1000)
            }
        },
        ExtendsYjcardList: function (orderList) {//提交到后台数据 易捷卡订单数据
            if (sessionStorage.yjcamt && YJCard.YjcList) {
                var totalBalance = 0;//易捷卡的总余额
                for (var i = 0; i < YJCard.YjcList.length; i++) {
                    totalBalance += YJCard.YjcList[i].Balance;
                }
                for (var i = 0; i < orderList.length; i++) {
                    orderList[i].YjCards = YJCard.YjcList;
                    orderList[i].YjCardPrice = sessionStorage.yjcamt;
                }
            }
        },
        Phone: {
            countdown: 60,//验证码倒计时
            isValidate: false,//手机验证码是否验证通过？
            isEnableValid:true,//是否启用手机验证码
        },
        YjcList: [],
        YjcCommodityInfo: { appId: [], types: [] },
        isShowYJC:false
    };
    $(function () {
        yjc.init();
    });
    return yjc;
})(window.Zepto)