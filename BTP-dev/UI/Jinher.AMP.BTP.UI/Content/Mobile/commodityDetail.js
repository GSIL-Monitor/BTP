var shareData = "";
var goutongData = "";
var _assumeULikeUrl = '';
//读取新浪接口返回数据设置当前所在省
function setProvince(myFn) {
    try {
        if (JsVilaDataNull(sessionStorage.province)) {
            myFn && myFn();
            return;
        }
        getMapData();
        function getMapData() {
            //实例化城市查询类
            var citysearch = new AMap.CitySearch();
            //自动获取用户IP，返回当前城市
            citysearch.getLocalCity(function (status, result) {
                if (status === 'complete' && result.info === 'OK') {
                    if (result && result.province) {
                        var province = result.province;
                        if (province.indexOf("香港") >= 0) {
                            sessionStorage.province = "香港";
                        } else if (province.indexOf("澳门") >= 0) {
                            sessionStorage.province = "澳门";
                        } else if (province.indexOf("广西") >= 0) {
                            sessionStorage.province = "广西";
                        } else if (province.indexOf("新疆") >= 0) {
                            sessionStorage.province = "新疆";
                        } else if (province.indexOf("内蒙") >= 0) {
                            sessionStorage.province = "内蒙";
                        } else if (province.indexOf("宁夏") >= 0) {
                            sessionStorage.province = "宁夏";
                        } else {
                            sessionStorage.province = province;
                        }

                    }
                }
                myFn && myFn();
            });
        }

    } catch (e) {
        sessionStorage.province = "北京市";
        myFn && myFn();
    }
    
}

function setByHash() {
    if (window.location.hash == "#review") {
        $("#img").css("display", "none");
        $("#property").css("display", "none");
        $("#content").css("display", "none");
        $("#add_sql").css("display", "none");
        $("#connection").css("display", "none");
        $("#RelationProduct").css("display", "none");
        $("#items").css("display", "none");
        $("#summaryEnter2").css("display", "none");
        $("#morReview").show();
        $(".add_btb").find("i").addClass("show_down");
        $("#footer").hide();


        getCommodityInfoReplays({ type: true, loadFromCache: true, limit: 0 });
        //  mainScrollTop = document.body.scrollTop;
        // window.scrollTo(0, 0);
    } else {
        $("#img").show();
        $("#property").show();
        $("#content").show();
        $("#add_sql").show();
        $("#connection").show();
        $("#RelationProduct").show();
        $("#items").show();
        $("#summaryEnter2").show();
        $("#morReview").css("display", "none");
        $(".add_btb").find("i").removeClass("show_down");
        $("#footer").show();
        window.scrollTo(0, mainScrollTop);
        getCommodityInfoReplays({ type: true, loadFromCache: true, limit: 3 });
        //window.scrollTo(0, 0);
    }
}

//显示 商品属性选择框。
function showMasIframe(operType) {
    var commodityInfo = JSON.parse(sessionStorage.commodityInfo);
    //window.frames[0].postMessage('getcolor', 'http://testbtp.iuoooo.com/');
    //document.getElementById("iframeMas").window.showByCommodity(commodityInfo, commodityInfo, operType);
    document.getElementById("iframeMas").contentWindow.postMessage({ commodityInfo: commodityInfo, operType: operType }, "*");
    $("#iframeMas").width($(window).width());
    $("#iframeMas").height($(window).height());
    setTimeout(function () {
        $("#divMasRoot").show();
        $("#divMasRoot").css("z-index", "2000");
        //隐藏窗口的滚动条。
        $("body").css("overflow-y", "hidden");
    }, 200);
}
//加入购物车
function addtoshopcartMain() {
    logBTP(sysname || sessionStorage.SrcType, service_type, "0x0004", sessionStorage.commodityId_2 || getQueryString('commodityId'));

    var postData = {
        CommodityId: getQueryString('commodityId'),         //商品ID       
        CommodityNumber: _masSelectedResult.number, //商品数量
        SizeAndColorId: (_masSelectedResult.size ? _masSelectedResult.size : '') + ',' + (_masSelectedResult.color ? _masSelectedResult.color : ''), //尺寸颜色
        UserId: getUserId(),
        AppId: sessionStorage.appId,
        CommodityStockId: _masSelectedResult.CommodityStockId,
        EsAppId: getEsAppId()
    };

    //添加到购物车
    getDataAjax2({
        url: '/Mobile/SaveShoppingCart',
        contentType: "application/json",
        dataType: 'json',
        type: 'POST',
        async: true,
        data: JSON.stringify(postData),
        callback: function (d) {
            if (d.ResultCode == 0) {
                toast('加入购物车成功');
            } else {
                toast(d.Message);
            }
        },
        error: function () {
        }
    });
}
//购买
function buyNow() {
    // 4 结束，5 抢光
    if (sessionStorage.isSecKill) {
        toast("活动已开始，不能加入购物车");
        return;
    }

    if (sessionStorage.PromotionState === "4") {
        toast("活动已结束");
        return;
    }
    if (sessionStorage.RealStock === "0") {
        toast("已抢光");
        return;
    }

    getDataAjax({
        url: '/Mobile/CheckSecKillBuy',
        data: { commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId') },
        callback: function (result) {
            if (result) {
                showMasIframe(2);
            } else {
                toast("秒杀马上开始！");
            }
        }
    });
}

function addToCart() {
    // 4 结束，5 抢光
    if (sessionStorage.isSecKill) {
        toast("活动已开始，不能加入购物车");
        return;
    }

    if (sessionStorage.PromotionState === "4") {
        toast("活动已结束");
        return;
    }
    if (sessionStorage.RealStock === "0") {
        toast("已抢光");
        return;
    }

    if (sessionStorage.PromotionType === "1" || sessionStorage.PromotionType === "2") {
        toast("活动已开始，不能加入购物车");
        return;
    }
    //行为记录->加入购物车操作
    logBTP(sysname || sessionStorage.SrcType, service_type, "0x0004", sessionStorage.commodityId_2 || getQueryString('commodityId'));
    showMasIframe(1);
}
//填写数量后确认购买。
function buyNowOk(seledResult) {
    logBTP(sysname || sessionStorage.SrcType, service_type, "0x0005", sessionStorage.commodityId_2 || getQueryString('commodityId'));
    var checkdata = "{\"diyGroupId\":\"00000000-0000-0000-0000-000000000000\" ,\"promotionType\":\"-1\" ,\"UserID\":\"" + getUserId() + "\" ,\"CommodityIdAndStockIds\":[" + "{\"CommodityId\":\"" + getQueryString('commodityId') + "\" ,\"CommodityStockId\":\"" + sessionStorage.CommodityStockId + "\" ,\"OutPrommotionId\":\"" + sessionStorage.OutPromotionId + "\"}" + "]}";
    //获取数据.
    getDataAjax2({
        url: '/Mobile/CheckCommodity',
        data: checkdata,
        callback: function (data) {

            var span_number = _masSelectedResult.number;

            if (data[0].State == 1) {
                toast('商品已下架，不能购买');
            } else if (data[0].State == 3) {
                toast('商品已删除，不能购买');
            } else if (data[0].IsNeedPresell && !data[0].IsPreselled) {
                toast('未预约，不能购买');
            } else if (data[0].LimitBuyEach > 0 && span_number > data[0].LimitBuyEach) {
                toast('超过每人限购数量，不能购买');
            } else if (span_number > data[0].Stock || (data[0].LimitBuyTotal > 0 && span_number > (data[0].LimitBuyTotal - data[0].SurplusLimitBuyTotal))) {
                toast('数量不足或超出限购数量.不能购买');
            } else {
                sessionStorage.Stock = data[0].Stock;
                var sessionData = JSON.parse(sessionStorage.commodityUpInfo);
                if (eval(sessionData.realPrice).toFixed(2) != data[0].Price.toFixed(2)) {
                    setSessionStorage("commodityUpInfo", "realPrice", data[0].Price);
                    toast("商品价格发生变化");
                }
                gotoCreateOrder("goumai");
                return;
            }
            $("#ajaxLoadBlind").remove();
            $("#span_number").val("1");
        },
        beforeSend: function () {
            ajaxLoading('22', '');
        },
        error: function () {
            $("#ajaxLoadBlind").remove();
        }
    });
}

function diyGroupOK(seledResult) {
    ajaxLoading('22', '');
    var data = {
        appId: sessionStorage.appId || getQueryString('appId'),
        commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId'),
        userId: getUserId()
    };

    var promotionType = seledResult.operType == 3 ? 3 : -1;
    var diyGroupId = getQueryString('diyGroupId');
    if (!JsVilaDataNull(diyGroupId)) {
        diyGroupId = "00000000-0000-0000-0000-000000000000";
    }
    var checkdata = "{\"diyGroupId\":\"" + diyGroupId + "\" ,\"promotionType\":\"" + promotionType + "\" ,\"UserID\":\"" + getUserId() + "\" ,\"CommodityIdAndStockIds\":[" + "{\"CommodityId\":\"" + getQueryString('commodityId') + "\" ,\"CommodityStockId\":\"" + seledResult.CommodityStockId + "\"}" + "]}";
    //获取数据.
    getDataAjax2({
        url: '/Mobile/CheckCommodity',
        data: checkdata,
        callback: function (data) {
            var diyGroupId = getQueryString('diyGroupId');
            if (!JsVilaDataNull(diyGroupId)) {
                diyGroupId = "00000000-0000-0000-0000-000000000000";
            }
            ajaxLoading('22', '');
            //var span_number = $('.span_number').val();
            var span_number = seledResult.number;

            if (data[0].State == 1) {
                toast('商品已下架，不能购买');
            } else if (data[0].State == 3) {
                toast('商品已删除，不能购买');
            } else if (span_number > data[0].Stock || (data[0].LimitBuyTotal > 0 && span_number > (data[0].LimitBuyTotal - data[0].SurplusLimitBuyTotal))) {
                toast('数量不足或超出限购数量.不能购买');
            } else {
                sessionStorage.Stock = data[0].Stock;
                var sessionData = JSON.parse(sessionStorage.commodityUpInfo);
                if (eval(sessionData.realPrice).toFixed(2) != data[0].Price.toFixed(2)) {
                    setSessionStorage("commodityUpInfo", "realPrice", data[0].Price);
                }
                if (sessionStorage.btntype = "diygroup") {
                    sessionStorage.type = "diygroup";
                }
                gotoCreateOrder("diygroup", diyGroupId);
                return;
            }

            $("#ajaxLoadBlind").remove();
            $("#span_number").val("1");
        },
        beforeSend: function () {
        },
        error: function () {
            $("#ajaxLoadBlind").remove();
        }
    });
}
//猜你喜欢url赋值，从配置文件中取
function setAssumeULikeUrl(url) {
    _assumeULikeUrl = url;
}
function assumeULike() {
    if (JsVilaDataNull(_assumeULikeUrl) && isLogin()) {
        url = _assumeULikeUrl.replace('{userId}', getUserId()).replace('{itemId}', getQueryString('commodityId')).replace('{datetime}', new Date().getTime());
        $.getJSON(url);
    }
}



//商品IDid，appid，商品名称，商品缩略图  ISO
function GiveIsoShare() {
    return shareData;
}
//
function GiveAndroidShare() {
    if (window.tw && window.tw.setOrderInformation) {
        window.tw.setOrderInformation(shareData);
    }
}
function removeHTMLTag(str) {
    str = str.replace(/<\/?[^>]*>/g, ''); //去除HTML tag
    str = str.replace(/[ | ]*\n/g, '\n'); //去除行尾空白
    //str = str.replace(/\n[\s| | ]*\r/g,'\n'); //去除多余空行
    str = str.replace(/&nbsp;/ig, ''); //去掉&nbsp;
    return str;
}
//分享商品
function shareCommodityDetail(promotionType) {
    var descExt = "";
    var commodityInfo = JSON.parse(sessionStorage.commodityInfo);

    var realPrice = commodityInfo.DiscountPrice > 0 ? Math.abs(commodityInfo.DiscountPrice).toFixed(2) : Math.abs(commodityInfo.Price * (commodityInfo.Intensity / 10)).toFixed(2);
    if (commodityInfo.Price - realPrice > 0) {
        //现价：89元   原价：100元
        descExt = "现价：" + realPrice + "元   原价：" + commodityInfo.Price + "元";

    } else {
        descExt = "价格：" + realPrice + "元";

        if (commodityInfo.MarketPrice > commodityInfo.Price) {
            descExt += "   市场价：" + commodityInfo.MarketPrice + "元";
        }
    }

    var tempobjons = { CommodityId: (sessionStorage.commodityId_2 || getQueryString('commodityId')), AppId: (sessionStorage.appId || getQueryString('appId')), Pic: commodityInfo.Pic, CommodityName: commodityInfo.Name + " " + descExt, Desc: descExt };
    shareData = JSON.stringify(tempobjons);

    var url = window.location.href.toLowerCase();
    if (promotionType != 3) {
        url = url.replace(/&outpromotionid=[^&]*/g, "");
        url = url.replace(/\?outpromotionid=[^&]*&/g, "\?");
    }
    shareAndroid(commodityInfo.Name + " " + descExt, commodityInfo.Name, commodityInfo.Pic, url, 8, 1);
    GiveAndroidShare();
}
function setCommodity() {
    return goutongData;
}
//设置沟通数据
function setGoutongData() {
    var commodityInfo = JSON.parse(sessionStorage.commodityInfo);
    var desc = removeHTMLTag($(".content_1").html());
    if (desc.length > 10) {
        desc = desc.substr(0, 9);
    }
    var tempgoutong = { CommodityId: (sessionStorage.commodityId_2 || getQueryString('commodityId')), AppId: (sessionStorage.appId || getQueryString('appId')), Pic: commodityInfo.Pic, CommodityName: commodityInfo.Name, CommodityDesc: desc, CommodityTzUrl: window.location.href, PaType: "1" };
    goutongData = JSON.stringify(tempgoutong);
}
function commonEvents() {
    //联系商家
    $("#contactOwner").bind("click", function () {
        sessionStorage.btntype = "contactOwner";
        DealLoginPartial.initPartialPage();
    });
}

//联系商家 
function contactToOwner() {
    var contactAppId = _contactObj ? sessionStorage.appId : (getEsAppId() || sessionStorage.appId);
    //智力圈的情况
    if (contactAppId == "630af8fc-41e9-4bd1-a436-2dd4197f076b" || contactAppId == "cf063155-e6e9-4019-ba12-6b44b704243f") {
        $("#Zhiliquan").show();
        return false;
    }
    else {
        $("#Zhiliquan").hide();
        if (JsVilaDataNull(contactUrl) && contactUrl.indexOf("http") >= 0) {
            window.location.href = contactUrl;
        } else if (sessionStorage.source == "share") {
            window.location.href = _webImUrl.format(contactAppId);
        } else {
            try {

                if ($.os.ios) {
                    window.location.href = "/Mobile/ContactAppOwner?type=goutong&appId=" + contactAppId;
                } else {
                    window.contactStoreOwner.startServiceList(contactAppId);
                }
            } catch (e) {
                toast("商家暂不支持此功能~");
            }
        }
        //行为记录->联系商家操作
        logBTP(sysname || sessionStorage.SrcType, service_type, "0x0006", sessionStorage.commodityId_2 || getQueryString('commodityId'));
    }
    
}