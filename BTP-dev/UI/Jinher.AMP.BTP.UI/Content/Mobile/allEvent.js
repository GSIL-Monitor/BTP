
////string.format.
////两种调用方式
//var template1 = "我是{0}，今年{1}了";
//var template2 = "我是{name}，今年{age}了";
//var result1 = template1.format("loogn", 22);
//var result2 = template2.format({ name: "loogn", age: 22 });
////两个结果都是"我是loogn，今年22了"
String.prototype.format = function (args) {
    var result = this;
    if (arguments.length > 0) {
        if (arguments.length == 1 && typeof (args) == "object") {
            for (var key in args) {
                if (args[key] != undefined) {
                    var reg = new RegExp("({" + key + "})", "g");
                    result = result.replace(reg, args[key]);
                }
            }
        }
        else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return result;
}

$.fn.clicks = function () {
    var tag;
    var callback;
    if (arguments.length > 1) {
        tag = arguments[0];
        callback = arguments[1];
    } else {
        tag = '';
        callback = arguments[0];
    }

    var startSize;
    this.on('touchstart', tag, function (e) {
        startSize = e.changedTouches[0].pageY;
        e.stopPropagation();
        e.stopPropagation();
        //		ajaxLoading('111', '');
    }).on('touchend', tag, function (e) {
        e.stopPropagation();
        e.preventDefault();
        var self = $(this);
        //			if(self.hasClass('clicks_lock')) {
        ////				return this;
        //			} else {
        if (Math.abs(startSize - e.changedTouches[0].pageY) < 10) {
            //					self.addClass('clicks_lock');
            callback(this, e);
        }
        //			}
        //			ajaxLoading('111', '');
    });
    return this;
};

var appId = '11111111-1111-1111-1111-111111111111';
!sessionStorage.page ? sessionStorage.page = 1 : '';
sessionStorage.categoryId = '';
sessionStorage.categoryLock = '';
sessionStorage.want = '';
sessionStorage.wantLock = '';
var advance = 500; //距离底部30px时开始加载
var multAttr = false;   //商品多个属性，默认为false
var maxHeight = 0;
var scrollLoading = false;
var regNum = new RegExp("^0$|^[1-9]\\d*$");
var isComHasVipInfo = false;
$(function () {
    //设置元素盒子最小高度
    $('#main').css({
        minHeight: $(window).height() + $('#footer').height()
    });
});


/**
* ajax loading 图片
* @param id                默认基础上添加的区别ID
* @param insertElement     插入那个Element元素名称.可输入   #id   .class  不传入该值时.默认插入body内.并生成一个蒙版.
*/

function ajaxLoading(id, insertElement) {
    //ajaxLoading盒子ID对象
    var loading = $('#ajaxLoading_' + id);
    //蒙版
    var blind = $('<div></div>');
    //是否有插入对象.有插入对象则不显示蒙版
    var insertElements = insertElement ? $(insertElement) : blind;

    //蒙版相关css
    blind.css({
        'position': 'fixed',
        'z-index': '10000',
        'opacity': 0.2,
        'backgroundColor': '#ccc',
        'height': '100%',
        'width': '100%',
        'top': 0,
        'left': 0
    });

    //蒙版ID值
    blind.attr('id', 'ajaxLoadBlind');

    //判断是否有自定义插入对象.当没有是插入body最后位置
    if (!insertElement) {
        !$('#ajaxLoadBlind')[0] ? $('body').append(blind) : '';
    }

    //生成loading图片对象
    if (!loading.attr('id')) {
        loading = $('<div></div>');
        loading.attr('id', 'ajaxLoading_' + id);
        loading.css({
            'position': 'absolute',
            'z-index': '99',
            'left': '50%',
            'margin-left': '-16px',
            'top': '50%',
            'margin-top': '-16px'
        });

        loading.append('<img src="/Content/images/ajax-loader.gif" />');

        insertElement ? insertElements.css({ 'position': 'relative' }) : '';
        insertElements.append(loading);
        var windowParent = $(window.parent);
        //		!insertElement ? loading.css({
        //			top: loading.css({ top: (windowParent.height() / 2) - 16 + windowParent.scrollTop() })
        //		}) : '';

    } else {
        !insertElement ? $('#ajaxLoadBlind').remove() : loading.remove();
    }
}

var ajaxLoadingSingle =(function() {
    function initLoading()
    {
        var blind = $('body').find("#ajaxLoadBlind");
        if(blind.length > 0)
        {
            return;
        }
        //蒙版
        blind = $('<div></div>'); 
        //蒙版相关css
        blind.css({
            'position': 'fixed',
            'z-index': '10000',
            'opacity': 0.2,
            'backgroundColor': '#ccc',
            'height': '100%',
            'width': '100%',
            'top': 0,
            'left': 0
        });
        //蒙版ID值
        blind.attr('id', 'ajaxLoadBlind');
       
        //ajaxLoading盒子ID对象
        var loading = $('#ajaxLoading_img');
        loading = $('<div></div>');
        loading.attr('id', 'ajaxLoading_img');
        loading.css({
            'position': 'absolute',
            'z-index': '99',
            'left': '50%',
            'margin-left': '-16px',
            'top': '50%',
            'margin-top': '-16px'
        });
        loading.append('<img src="/Content/images/ajax-loader.gif" />');
        //blind.css({ 'position': 'relative' });
        blind.append(loading);
        $('body').append(blind); 
    }

    function show()
    {
        initLoading();
        var blind = $('body').find("#ajaxLoadBlind");
        if(blind.length == 0)
        {
            return;
        }
        blind.show();
    }
    function hide()
    {
        var blind = $('body').find("#ajaxLoadBlind");
        if(blind.length == 0)
        {
            return;
        }
        blind.hide();
    }

    var loadingSingle = new Object();
    loadingSingle.show = show;
    loadingSingle.hide = hide;
    return loadingSingle;
}())
 




//获取URL某值

    function getQueryString(name) {
        var r;
        if (arguments.length > 1) {
            r = arguments[1].split('?')[1];
        } else {
            r = window.location.search.substr(1);
        }
        //	var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
        r = r.match(reg);
        if (r != null) return unescape(r[2]);
        return null;
    }

    function GoShop(id) {
        var str = location.href; //取得整个地址栏
        var tempStr = str;
        if (str.indexOf("?") != "-1") {
            var num = str.indexOf("?")
            str = str.substr(num + 1);
            if (str.indexOf("commodityId=") != -1) {
                tempStr = tempStr.replace(str.substr(str.indexOf("commodityId=") + 12, 36), id);
                window.location.href = tempStr;
            } else {
                tempStr += "&commodityId=" + id;
            }
        } else {
            window.location.href = str + "?commodityId=" + id;
        }

    }

//商品列表中设置各个商品相关属性
function setItemData1(obj) {
    var clone_item = obj.element;
    var data = obj.data;
    clone_item.css({
    }).attr('data-id', data.RelationCommodityId);    
    clone_item.find('img').parent().css({ 'height': Math.round(sessionStorage.img_height) + 'px', 'overflow': 'hidden' }).
		find('img').css('height', Math.round(sessionStorage.img_height) + 'px').attr('src', data.CommodityPicturesPath)
		.addClass('LazyLoad')[0].onerror = function () {
		    console.log(self.css({ background: 'none' }).attr('src', '/Content/Mobile/b_2.png'));
		};
    if(data.IsEnableSelfTake==1){
        clone_item.find('.selfTakeImg').attr('src','/Images/selftake.png').show();
    }
    else{
        clone_item.find('.selfTakeImg').attr('src','').hide();
    }
    clone_item.find('.item_title').text(data.Name);
    var isyou = false;
    if (data.DiscountPrice > 0) {
        clone_item.find('.price_1').text(getCurrency() + Math.abs(data.DiscountPrice).toFixed(2));
        clone_item.find('.price_2').text(getCurrency() + data.Price);
        clone_item.find('.zk').text('优惠价')
        isyou = true;
    }
    else {
        if (data.Intensity == 10) {
            clone_item.find('.price_1').text(getCurrency() + data.Price);
            if (data.MarketPrice && data.MarketPrice != null && data.MarketPrice != 'null')
            {
                 clone_item.find('.price_2').text(getCurrency() + data.MarketPrice);
            }
            else
            {
                clone_item.find('.price_2').hide();
            }
        } else {
            clone_item.find('.price_1').text(getCurrency() + Math.abs(data.Price * (data.Intensity / 10)).toFixed(2));
            clone_item.find('.price_2').text(getCurrency() + data.Price);
            isyou = true;
        }
        data.Intensity != 10 ? clone_item.find('.zk').text(data.Intensity + '折') : '';
    }
        clone_item.bind("click", function() {
            GoShop($(this).attr('data-id'));
        });
    var lim = "";
    if (isyou == true
    && sessionStorage.ProductType != "appcjzy" 
    && sessionStorage.ProductType != "webcjzy") {
        if (data.LimitBuyEach > -1 && data.LimitBuyEach) {
            lim += "每人限购" + data.LimitBuyEach + "件,";
        }
        else {
            lim += "不限购,";
        }

        if (data.LimitBuyTotal > -1 && data.LimitBuyTotal) {
            lim += "还剩" + (parseInt(data.LimitBuyTotal) - parseInt(data.SurplusLimitBuyTotal)) + "件";
        }
        else {
            lim += "还剩" + data.Stock + "件";
        }
        clone_item.find('.yhzknum').text(lim);
    }
    !(data.Stock > 0) ? clone_item.find('.mb_box').removeClass('noDisplay') : '';
    data.State == 1 ? clone_item.find('.mb_box').removeClass('noDisplay').find('.mb_1').text('已下架') : '';

   // clone_item.css({ height: sessionStorage.item_height + 'px', overflow: 'hidden' });
    return clone_item;
}



//相关商品填充

function setRelationCommoditys(relationCommoditys) {
    var items = $('#items');
    var item = $('#parent_item');
    var height = item.height();
    var img_height = item.find('img').height();
    var clone_item;
    $("#items").html("");
     $('#itemsListMode').html("");
     _commodityListData = [];
    if (relationCommoditys != null && relationCommoditys.length > 0) {

        for (var i = 0; i < relationCommoditys.length; i++) {
            clone_item = item.clone().removeClass('noDisplay').attr('id', '');
            //toast(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
            items.append(setItemData1({ element: clone_item, data: relationCommoditys[i], height: height, img_height: img_height }));
        }
    } else {
        $("#RelationProduct").hide();
    }
}

function noCommodityDetail() {
    $("#ajaxLoadBlind").remove();
    $("#box").html($("#divNotLoginTemplate").html());
}

//获取商品详细信息

function getCommodityInfo(document, callback) {
    sessionStorage.commodityUpInfo = '';
    sessionStorage.isSecKill = ''; //是否秒杀
    sessionStorage.startSecKillTime = '';
     
    sessionStorage.commodityId_2 = getQueryString('commodityId');

    //设置获取商品详情请求参数
    //appId已改变语义，实际传递esAppId
    var data = {
        appId: getEsAppId(),
        commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId'),
        freightTo: sessionStorage.province,
        outPromotionId: getQueryString('outPromotionId'),
        source: sessionStorage.source
    };
    if (isLogin()) {
        data.userId = getUserId();
    }
    //获取数据.
    getDataAjax({
        url: '/Mobile/GetCommodityDetailsZPH?r=' + Math.random(),
        data: data,
        callback: function(data) {
            if (!data || data.ResultCode != 0) {
                noCommodityDetail();
            }
            setData(data);
           
//            // 获取众筹状态
//            if (GetCrowdfundingState(sessionStorage.appId, "crowdwin")) {
//                $("#crowdwin").bind("click", function() {
//                    CrowdfundingMessage();
//                });
//                $('#gotoContent').click(function() {
//                    var psource = sessionStorage.source == "share" ? "&source=share" : "";
//                    window.location.href = '/Mobile/CrowdfundingDesc?shopId=' + sessionStorage.appId + psource;
//                    CloseFade();
//                });
//            }
        },
        beforeSend: function() {
            ajaxLoading('22', '');
        }
    });

    //设置商品价格区间
    //commodity -商品详情

    function setPriceRange(commodity) {
        $('.u-detail-pricebox').show();
        if (!commodity) {
            commodity = JSON.parse(sessionStorage.commodityInfo);
        }

        //设置价格 优惠价显示优惠价
        sessionStorage.DiscountPrice = -1;

        //价格区间显示条件：1、商品存在多个属性 2、商品没有设置折扣价   
        //所以默认隐藏最小价格
        $('#minPrice').hide();
        $(".oldPrice").hide();
        var minPriceShow = false;
        var minMarketPriceShow = false;

        var minPrice = commodity.Price;
        var maxPrice = commodity.Price;
        var minMarketPrice = commodity.MarketPrice;
        var maxMarketPrice = commodity.MarketPrice;
        if (commodity.CommodityStocks && commodity.CommodityStocks.length > 0) { //商品包含多个属性，库存、价格需要联动
            minPrice = 999999999;
            maxPrice = 0;
            minMarketPrice = 999999999;
            maxMarketPrice = 0;
            for (var j = 0; j < commodity.CommodityStocks.length; j++) {

                if (commodity.CommodityStocks[j].Price < minPrice) {
                    minPrice = commodity.CommodityStocks[j].Price;
                }
                if (commodity.CommodityStocks[j].Price > maxPrice) {

                    maxPrice = commodity.CommodityStocks[j].Price;
                }

                if (commodity.CommodityStocks[j].MarketPrice && commodity.CommodityStocks[j].MarketPrice != null && commodity.CommodityStocks[j].MarketPrice != 'null' && commodity.CommodityStocks[j].MarketPrice < minMarketPrice) {
                    minMarketPrice = commodity.CommodityStocks[j].MarketPrice;
                }
                if (commodity.CommodityStocks[j].MarketPrice && commodity.CommodityStocks[j].MarketPrice != null && commodity.CommodityStocks[j].MarketPrice != 'null' && commodity.CommodityStocks[j].MarketPrice > maxMarketPrice) {
                    maxMarketPrice = commodity.CommodityStocks[j].MarketPrice;
                }
            }
            if (minPrice < maxPrice) {
                minPriceShow = true;
            }
            if (minMarketPrice < maxMarketPrice) {
                minMarketPriceShow = true;
            }
        }
        setSessionStorage.minPriceShow = minPriceShow;
        sessionStorage.minPrice = minPrice;
        sessionStorage.maxPrice = maxPrice;
        
        if (commodity.DiscountPrice > 0) {
            $(".oldPrice").show();
            sessionStorage.DiscountPrice = commodity.DiscountPrice;
            $("#maxPrice").text(Math.abs(commodity.DiscountPrice).toFixed(2));
            if (!minPriceShow) {
                $("#minOldPrice").hide();
            } else {
                $('#minOldPrice').show().text(Math.abs(minPrice).toFixed(2) + '-');
            }
            $('#maxOldPrice').show().text(Math.abs(maxPrice).toFixed(2));
        } else if (commodity.Intensity < 10) {
            $(".oldPrice").show();

            if (!minPriceShow) {
                $("#minPrice").hide();
                $("#minOldPrice").hide();
            } else {
                $('#minPrice').show().text(Math.abs(minPrice * (commodity.Intensity / 10)).toFixed(2) + '-');
                $('#minOldPrice').show().text(Math.abs(minPrice).toFixed(2) + '-');
            }
            $("#maxPrice").text(Math.abs(maxPrice * (commodity.Intensity / 10)).toFixed(2));
            $('#maxOldPrice').show().text(Math.abs(maxPrice).toFixed(2));
        } else {

            if (minPriceShow) {
                $("#minPrice").show().text(Math.abs(minPrice).toFixed(2) + '-');
            }
            $("#maxPrice").text(Math.abs(maxPrice).toFixed(2));
            if (commodity.MarketPrice && commodity.MarketPrice != null && commodity.MarketPrice != 'null') {
                $(".oldPrice").show();
                if (minMarketPriceShow) {
                    $('#minOldPrice').show().text(Math.abs(minMarketPrice).toFixed(2) + '-');
                }
                $('#maxOldPrice').show().text(Math.abs(maxMarketPrice).toFixed(2));
            }
        }
    }


    function setFreightToDetails(comId) {
        $(".u-link-select").show();
        var req = { commodityId: comId };
        getDataAjax({
            url: '/Mobile/GetFreightDetails',
            data: req,
            callback: function(result) {
                $("#provinceSel").empty();
                if (result && result.ResultCode == 0 && result.FreightList.length > 0) {
                    for (var i = 0; i < result.FreightList.length; i++) {
                        var optionVal = result.FreightList[i].FreightTo + "_" + result.FreightList[i].Freitht;
                        var option = $("<option>").val(optionVal).text(result.FreightList[i].FreightTo);
                        $("#provinceSel").append(option);
                        if (result.FreightList[i].FreightTo == sessionStorage.province) {
                            $("#provinceSel").val(optionVal);
                        }
                    }
                }
            },
        });
    }

    //设置商品详情使用函数
    //商品属性：以size标示第一个商品属性，以color标示第二个商品属性

    function setData(comDetailResult) {
        var data = comDetailResult.CommodityInfo;
        var appDownLoadInfo = comDetailResult.AppDownLoadInfo;
        contactUrl = comDetailResult.ContactUrl;
        appName = comDetailResult.EsAppName;
        _contactObj = comDetailResult.ContactObj;
        var isShowvolume = comDetailResult.ResultIsVolume;
        var img = $('<img/>'); //生成一个img对象
        var imgBox = document.find('#img'); //获取传入对象内的img对象.
        var imgList = imgBox.find('ul'); //获取ul
        var property = document.find('#property'); //获取商品属性盒子
        var content_box = document.find('#content_box'); //商品详情与留言盒子
        var content_2_clone = document.find('#content_2_clone'); //留言盒子单个对象
        var nowDate = data.CurrentTime ? data.CurrentTime.split("(")[1].split(")")[0] : (new Date()).getTime(); //(new Date()).getTime();
        var tmpImg; //临时img对象
        var lastTime = "";
        

        //缓存当前数据.
        sessionStorage.commodityInfo = JSON.stringify(data);
        sessionStorage.appId = data.AppId;
        sessionStorage.State = data.State;
        isAppSet = data.IsAppSet; //是否厂家直营
        // 距离预约结束 还剩
        sessionStorage.PresellEndTime = getRemainingTime(data.PresellEndTime, "0", nowDate);
        lastTime = sessionStorage.PresellEndTime === "0" ? sessionStorage.PresellEndTime : data.PresellEndTime;
        // 距离抢购开始 还剩
        sessionStorage.PromotionStartTime = getRemainingTime(data.PromotionStartTime, lastTime, nowDate);
        lastTime = sessionStorage.PromotionStartTime === "0" ? sessionStorage.PromotionStartTime : data.PromotionStartTime;
        // 距离抢购结束 还剩
        sessionStorage.PromotionEndTime = getRemainingTime(data.PromotionEndTime, lastTime, nowDate);

        // 预约人数
        sessionStorage.PreselledNum = data.PreselledNum || 0;
        sessionStorage.PromotionState = data.PromotionState;
        sessionStorage.CommidotyId = getQueryString('commodityId');
        sessionStorage.OutPromotionId = data.OutPromotionId;
        sessionStorage.PromotionType = data.PromotionType;
        sessionStorage.total = data.Total;
        $('#total').find('span').text(data.Total);

        //是否显示销量
        if (isShowvolume == false) {
            $("#total").hide();
        } else {
            $("#total").show();
        }
        //商品参加促销，只显示活动的库存和销量
        sessionStorage.RealStock = data.Stock;
        if ((data.DiscountPrice > -1 || data.Intensity < 10) && data.LimitBuyTotal > 0) {
            var realStock = parseInt(data.LimitBuyTotal) - parseInt(data.SurplusLimitBuyTotal)
            property.find('.type_4').text(realStock);
            sessionStorage.RealStock = realStock;
            //设置出售数量
            property.find('.type_5').text(parseInt(data.SurplusLimitBuyTotal));
        } else {
            property.find('.type_4').text(data.Stock);
            //设置出售数量
            property.find('.type_5').text(data.Total);
        }
        if (data.Stock <= 0) {
            if(JsVilaDataNull(data.DiyGroupPromotion))
            {
                $("#promotionMsg").children("div").html("已售完");
                $("#btnDirectBuy").addClass("disabled");
                $("#btnDiyGroup").addClass("disabled");
            }
            else
            {
                $("#promotionMsg").children("div").html("已抢光");
            }
            $("#promotionMsg").show();
        }

        // 展现 活动状态
        showSateByPromotion(data.PromotionState || 0, data.PromotionType || 0, sessionStorage.RealStock || 0);

        //设置商品价格区间
        setPriceRange(data);

        //设置商品名称
        $('#title').text(data.Name);
        if (data.VideoWebUrl) {
            $("#Mvideo").show();
            $("#MVPVideo").attr("poster",data.VideoPicUrl);
            $("#MVPVideo").attr("src",data.VideoWebUrl); 
        } 
        
        setSessionStorage('commodityUpInfo', 'name', data.Name);


        $("#freight").text(Math.abs(data.Freight || 0).toFixed(2));
        if (data.IsSetMulti) {
            setFreightToDetails(data.Id);
        } else {
            $("#provinceSel").hide();
        }
        setSessionStorage('commodityUpInfo', 'price', Math.abs(data.Price).toFixed(2));
        

        //设置收藏数
        property.find('.type_6').text(data.CollectNum);
        //设置商品详情
        var contents = $(data.Description);
        contents.find('img').attr('width', '').attr('height', '').css('width', '100%');
        content_box.find('.content_1').html(contents);
        //设置评价数
        $("#reviewCount").text(data.ReviewNum);
        window.ReviewNum = data.ReviewNum;

        imgList.find("li").remove();
        //遍历头部图片.
        if (data.Pictures) {
            for (var i = 0; i < data.Pictures.length; i++) {
                imgList.append(img.clone().attr('src', data.Pictures[i].PicturesPath));
            }
        }

        if (data.RelationCommoditys != null) {
            $("#RelationProduct").show();
            for (var i = 0; i < data.RelationCommoditys.length; i++) {
                var tempImg = '<img style="width: 23%; height: auto;" src="' + data.RelationCommoditys[i].CommodityPicturesPath + '" onclick="GoShop(\'' + data.RelationCommoditys[i].RelationCommodityId + '\')" />';
                $("#RelationProductImg").append(tempImg);
            }
        }
        //设置图片高度与宽度
        tmpImg = img.clone().attr('src', '/Content/Mobile/2.jpg?' + new Date().getTime());
        tmpImg[0].onload = function() {
            callback();
        };

        //将商品名称.价格.折扣.图片缓存到commodityUpInfo内


        setSessionStorage('commodityUpInfo', 'Intensity', data.Intensity);
        setSessionStorage('commodityUpInfo', 'pic', data.Pic);
        setSessionStorage('commodityUpInfo', 'marketPrice', Math.abs(data.MarketPrice).toFixed(2));
        setSessionStorage('commodityUpInfo', 'IsEnableSelfTake', data.IsEnableSelfTake);
        setSessionStorage('commodityUpInfo', 'number', "1");
        setSessionStorage('commodityUpInfo', 'CommodityType', data.CommodityType);
        setSessionStorage('commodityUpInfo', 'orderType', data.CommodityType);
        //设置加减数量按钮事件
        numberEvent();
        //ComOrderList(2, 0, 1);
        setRelationCommoditys(data.RelationCommoditys);
        data.IsEnableSelfTake == 1 ? $('.selfTake').removeClass('noDisplay') : '';
        data.IsEnableSelfTake == 1 ? $("#selfTakeFlag").val(1) : $("#selfTakeFlag").val(0);

        //多属性商品显示关税信息
        var arrSimple = new Array();
        var le = data.CommodityStocks.length;
        var length = 0;
        if (data.CommodityStocks.length > 0) {
            for (var j = 0; j < le; j++) {
                if (data.CommodityStocks[j].Duty > 0) {
                    arrSimple.push(data.CommodityStocks[j].Duty);
                    length ++;
                }
            }
            arrSimple.sort(sortNumber);
            if (length > 0) {
                if (length > 1) {
                    $("#divDuty").html(arrSimple[0].toFixed(2) + " ~ " + arrSimple[length - 1].toFixed(2));
                } else {
                    $("#divDuty").html(arrSimple[0].toFixed(2));
                }
                $("#DivDutys").show();
            } else {
                $("#DivDutys").hide();
            }
        } else {
            //显示关税信息
            if (data.Duty != null && parseFloat(data.Duty) > 0) {
                $("#divDuty").html(data.Duty);
                $("#DivDutys").show();
            } else {
                $("#DivDutys").hide();
            }
        }

        //显示部分包邮信息
        if (data.FreeFreightStandard != null) {
            var partial = "";
            if (data.FreeFreightStandard.length == 1) {
                partial += data.FreeFreightStandard[0] + "<br />";
            } else {
                for (var i = 0; i < data.FreeFreightStandard.length; i++) {
                    partial += (i + 1) + ". " + data.FreeFreightStandard[i] + "<br />";
                }
            }
            partial += "具体运费以下订单时显示为准";
            $("#partialFree").html(partial);
        }
        //计算佣金
        if (data.SharePercent != null
            && parseFloat(data.SharePercent) > 0) {
            var commission = (data.Price * data.SharePercent).toFixed(2);
            $("#spanCommission .commission").html(getCurrency() + commission);
            $("#spanCommission").show();
        } else {
            $("#spanCommission").hide();
        }

        //应用名称
        $("#spanAppName").text(data.AppName);

//        //商品本身不参与分销
//        //或 若A供应商嵌入了B供应商的商品，通过A供应商的APP或者链接购买了B供应商的分销商品，此时B供应商的商品不参与三级分销。
//        //都不显示“我要分销”按钮。
//        if (data.IsDistribute && data.AppId == getEsAppId()) {
//            $("#divDistribute51").show();
//        } else {
//            $("#divDistribute51").hide();
//        }
        //商品本身不参与分销
        //都不显示“我要分销”按钮。
        if (data.IsDistribute) {
            $("#divDistribute51").show();
        } else {
            $("#divDistribute51").hide();
        }
        if (sessionStorage.source == "share" && appDownLoadInfo && JsVilaDataNull(appDownLoadInfo.Icon) ) {
            $("#esAppLogo").attr("src",appDownLoadInfo.Icon);
            if (JsVilaDataNull(appDownLoadInfo.PromotionDownGuide)) {
                $("#esAppDesc").html(appDownLoadInfo.PromotionDownGuide);
            }
            $("#dwEsAppId").show();
        }

        //显示“下载、注册、登录”提示。
        if (data.IsDistribute && data.AppId == getEsAppId()
            && sessionStorage.source == 'share') {
            $("#divRegOrDownloadTip").show();
            $("#dwEsAppId").hide();
        } else {
            $("#divRegOrDownloadTip").hide();
        }
        if (isLogin()) {
            $("#pDownload").show();
            $("#pRegist").hide();
        } else {
            $("#pDownload").hide();
            $("#pRegist").show();
        }

        if (JsVilaDataNull(data.DiyGroupPromotion)) { //拼团
            showDiyGroupDetail(data);
            $("#divVipMember").hide();
        } 
        else {  //非拼团的处理优惠处理
            if (data.VipPromotion && data.VipPromotion.IsVipActive) {
                if (isLogin()) {
                    showVipPriceLogin(data.VipPromotion);
                } else {
                    var commodityInfo = JSON.parse(sessionStorage.commodityInfo);
                    var promotionType = commodityInfo.PromotionTypeNew;
                    if (promotionType == 9999) {
                        $("#divVipMember").show();
                    } else {
                        $("#divVipMember").hide();
                    }
                }
            }
        }
        if (isLogin()) {
            isComHasVipInfo = true;
        }
    }
    //显示“拼团”的商品详情页。
    function showDiyGroupDetail(data)
    {
       var diyg = data.DiyGroupPromotion; 
       $("#divPriceDiy").show();
       $("#promotionDesc").html(diyg.Description);

        var minMarketPrice = 0,
            maxMarketPrice = 0;
        if (data.CommodityStocks && data.CommodityStocks.length) { //多属性
            for (var j = 0; j < data.CommodityStocks.length; j++) {
                var commodity = data.CommodityStocks[j];
                if (commodity.MarketPrice && commodity.MarketPrice != null && commodity.MarketPrice != 'null' && (commodity.MarketPrice < minMarketPrice || minMarketPrice == 0)) {
                    minMarketPrice = commodity.MarketPrice;
                }
                if (commodity.MarketPrice && commodity.MarketPrice != null && commodity.MarketPrice != 'null' && commodity.MarketPrice > maxMarketPrice) {
                    maxMarketPrice = commodity.MarketPrice;
                }
            }
        }
        if (data.MaxSkuPrice > data.MinSkuPrice) {
            $("#spanPriceDiy").html(getCurrency() + data.MinSkuPrice + "~" + data.MaxSkuPrice);
            $("#btnDiyGroup div.open-group-price").html(getCurrency() + data.MinSkuPrice);
            if (minMarketPrice < maxMarketPrice) {
                $("#spanPriceOri").html(getCurrency() + minMarketPrice + "~" + maxMarketPrice);
            } else {
                $("#spanPriceOri").html(getCurrency() +  data.MinPrice + "~" +  data.MaxPrice);
            }
            $("#btnDirectBuy div.alone-buy-price").html(getCurrency() + data.MinPrice);
        } else {
            $("#spanPriceDiy").html(getCurrency() + data.DiscountPrice);
            $("#btnDiyGroup div.open-group-price").html(getCurrency() + data.DiscountPrice);
            $("#spanPriceOri").html(getCurrency() + data.Price);
            $("#btnDirectBuy div.alone-buy-price").html(getCurrency() + data.Price);
        }

       $("#spanJoinPersonCount").html(data.AlreadyJoinCount);
       $("#btnDiyGroup div.open-group-go span:last").html(data.DiyGroupPromotion.GroupMinVolume);

       if(JsVilaDataNull(getQueryString('diyGroupId')))
       { 
            $("#btnDiyGroup div.open-group-go span:first").html("参");
       }
       else
       {
            $("#btnDiyGroup div.open-group-go span:first").html("开");
       }
       //已参团人数 已超过 总的限购数量，不可再有人参团。
       if(diyg.SurplusLimitBuyTotal >= diyg.LimitBuyTotal)
       {
          $("#btnDiyGroup").addClass("disabled");
       }
        if(diyg.PromotionState == 0 || diyg.PromotionState == 4)
        {
            $("#promotionMsg").children("div").html("已结束");
            $("#promotionMsg").show();
            $("#btnDiyGroup").addClass("disabled");
        }
    }

   
    // 获取倒计时 剩余时间
    function getRemainingTime(serverTime, preTime, nowDate) {
        var resTime = 0;
        if (serverTime) {
            if (preTime === "0") {
                resTime = (serverTime.split("(")[1].split(")")[0] - nowDate) / 1000;
            } else {
                resTime = (serverTime.split("(")[1].split(")")[0] - (preTime.split("(")[1].split(")"))[0]) / 1000;
            }

            if (resTime < 0) {
                resTime = 0;
            }
        } else {
            resTime = 0;
        }

        return resTime;
    }

    // 根据活动状态 进行不同展示 以及相应的事件处理

    function showSateByPromotion(promotionState, promotionType, stock) {
        var promotionTemplate = "";
        // 库存为0 表示抢光了
        if (stock === "0") {
            promotionState = 5;
        }
        // 活动类型 0：普通活动，1：秒杀，2：预约，3：预售
        if (promotionType === 0) {
            normalEvent();
        } else if (promotionType === 1) {
            seckillEvent(promotionState);
        } else if (promotionType === 2) {
            presellEvent();
        } else if (promotionType === 3) {
            // 此版本没有
        } else {
            // 
        }

        // 普通活动 还是原来的逻辑 只给展示价格模版

        function normalEvent() {
            var cmdtyProperties = $("#cmdtyProperties");
            if (cmdtyProperties.children().length <= 0) {
                promotionTemplate = '<p class="u-detail-pricebox clearFloat">' +
                    '<span class="u-detail-price">'+getCurrency() +'</span> <span id="minPrice" class="u-detail-price u-detail-price2">' +
                    '</span><span id="maxPrice" class="u-detail-price u-detail-price2"></span><span class="u-detail-mprice oldPrice">' +
                    '<span>'+getCurrency() +'</span> <span id="minOldPrice"></span><span id="maxOldPrice"></span></span>' +
                    '<span class="u-detail-discount" class="color_1 type_1 " id="crowdwin" style="display: none;">' +
                    '众筹中</span>' +
                    '</p>';
                cmdtyProperties.html(promotionTemplate);
            }
        }

        // 秒杀

        function seckillEvent(promotionState) {
            //processPmtTemplate(promotionState);
            switch (promotionState) {
            case 0:
                normalEvent();
                break;
            //1:预约预售进行中，
            //case 1:
            //2：等待抢购：
            case 2:
            //3：活动进行中，
            case 3:                    
                normalEvent();
                procSeckillCommodity(promotionState);
                break;
            //4：活动已结束
            case 4:
            //5(当库存为0时) 抢光了
            case 5:
                normalEvent();
                // 购买 灰掉
                procPromotion45(promotionType, promotionState);
                break;
            }

            // 设置提醒状态
            setNotiSate();
        }

        // 预约

        function presellEvent() {
            processPmtTemplate(promotionState);
        }

        // 秒杀模版处理

        function procSeckillCommodity(promotionState) {
            var pTpt = ""; // 各个活动状态模版 辅助
            var pTptBtn = "";

            var promotionTemplate = ""; // 各个活动状态模版
            var footer = $("#footer");
            var timestamp = 0;
            var cmdtyProperties = $("#cmdtyProperties");

            if (promotionState === 2) {
                // 距离 抢购开始
                timestamp = sessionStorage.PromotionStartTime;

                promotionTemplate = '<div id="prmTimerDiv" style="padding-left: 16px;">' +
                    '<span>' +
                    '<span>距开抢时间：</span><span id="prm_day_show"></span><span>天</span>' +
                    '<span id="prm_hour_show"></span><span>：</span>' +
                    '<span id="prm_minute_show"></span><span>：</span>' +
                    '<span id="prm_second_show"></span>' +
                    '</span>' +
                    '<span id="sendNotifications" style="margin-left: 10px;width: 80px;height: 27px;line-height: 27px;display: inline-block;border-radius: 3px;">设置提醒</span>' +
                    '</div>';
                footer.html(promotionTemplate);
                

            } else if (promotionState === 3) {
                promotionTemplate = '<div class="footer_4" id="addshopcart" style="margin-right: 5px; margin-left: 0;padding : 3.5% 8%;opacity: .8;">' +
                    '加入购物车</div>' +
                    '<div class="footer_2" style="padding: 3.5% 8% !important;   border-radius: 4px; background-color: rgb(255, 0, 84);  text-align: center; color: #fff; font-size: 1.3em; letter-spacing: 3px; display: inline-block;">' +
                    '立即抢购</div>';
                // 距离 抢购开始
                timestamp = sessionStorage.PromotionEndTime;

                footer.html(promotionTemplate);
            }
            // 倒计时
            prmTimer(timestamp,
                {
                    day: $("#prm_day_show") || null,
                    hour: $("#prm_hour_show") || null,
                    minute: $("#prm_minute_show") || null,
                    second: $("#prm_second_show") || null
                },
                function() {
                    // 展示该活动的下一个状态
                    if (promotionState === 2) {
                        seckillEvent(3);
                    } else if (promotionState === 3) {

                        procPromotion45(2, 4);
                    }
                }
            );
        }

        // 预约 处理模版

        function processPmtTemplate(promotionState) {
            switch (promotionState) {
            case 0:
                normalEvent();
                break;
            //1:预约预售进行中，
            case 1:
            //2：等待抢购：
            case 2:
            //3：活动进行中，
            case 3:
                procPromotion123(promotionState);
                break;
            //4：活动已结束 或者 抢光了
            case 4:
            //5(当库存为0时) 抢光了
            case 5:
                normalEvent();
                procPromotion45(promotionType, promotionState);
                break;
            }
        }

        // 活动状态为 1、2、3时

        function procPromotion123(promotionState) {
            var pTpt = ""; // 各个活动状态模版 辅助
            var pTptBtn = "";

            var promotionTemplate = ""; // 各个活动状态模版
            var footer = $("#footer");
            var timestamp = 0;
            var cmdtyProperties = $("#cmdtyProperties");

            // 抢购开始的时候 还是原来 购买的逻辑
            if (promotionState !== 3) {
                promotionTemplate = '<div class="footer_4" id="addshopcart" style="margin-right: 5px; margin-left: 0;padding : 3.5% 8%;opacity: .8;">' +
                    '加入购物车</div>' +
                    '<div id="prmPresell" style="padding: 3.5% 8% !important;   border-radius: 4px; background-color: rgb(255, 0, 84);  text-align: center; color: #fff; font-size: 1.3em; letter-spacing: 3px; display: inline-block;">' +
                    '购买</div>';

                footer.html(promotionTemplate);
            }

            if (promotionState === 1) {
                pTpt = "距预约结束";
                pTptBtn = "立即预约";

                timestamp = sessionStorage.PresellEndTime;

                subscribeNow();
            } else if (promotionState === 2) {
                pTpt = "距抢购开始";
                pTptBtn = "立即抢购";

                timestamp = sessionStorage.PromotionStartTime;

                footer.children("div").css({ "background-color": "#E0E0E0", "color": "#505050" });
            } else if (promotionState === 3) {
                pTpt = "距抢购结束";
                pTptBtn = "立即抢购";

                timestamp = sessionStorage.PromotionEndTime;

                promotionTemplate = '<div class="footer_4" id="addshopcart" style="margin-right: 5px; margin-left: 0px; color: gray;padding: 3.5% 8%;opacity: .8;">加入购物车</div>' +
                    '<div class="footer_2" style="padding: 3.5% 8% !important;">立即抢购</div>';

                footer.children("div").filter(".footer_2").attr("style", "padding: 3.5% 12% !important;");
                footer.html(promotionTemplate);
            }

            if (footer.children("div").filter("#prmPresell").length !== 0) {
                footer.children("div").filter("#prmPresell")[0].innerHTML = pTptBtn;
            } else {
                footer.children("div").filter(".footer_2")[0].innerHTML = pTptBtn;
            }

             if (cmdtyProperties.children("div").length > 0) {

            } else {
                promotionTemplate = '<div style="padding-left: 0;  margin-bottom: .3rem;">' +
                    '<span style=" clear:both;">预售价：</span><span style="color: #FF0054;" class="yuan">'+getCurrency() +'</span> <span id="minPrice" class="u-detail-price2" style="color: #FF0054;">' +
                    '</span><span id="maxPrice" class="u-detail-price2" style="color: #FF0054;"></span><span style="margin: -1rem 1rem 0 2rem; color: #999; text-decoration: line-through; font-size: .9rem;">' +
                    '<span>'+getCurrency() +'</span> <span id="minOldPrice"></span><span id="maxOldPrice"></span></span>' +
                    '<span class="color_1 type_1" id="crowdwin" style="display: none;">' +
                    '众筹中</span>' +
                    '</div>' ;

                $("#cmdtyProperties").html(promotionTemplate);
            }
			var	prmTimerDiv = '<span>' + pTpt + '</span><span style=" color:#ff0054;" class="remain_t">（' +
                    '<span>还剩</span><span id="prm_day_show"></span><span>天</span>' +
                    '<span id="prm_hour_show"></span><span>小时</span>' +
                    '<span id="prm_minute_show"></span><span>分</span>' +
                    '<span id="prm_second_show"></span><span>秒</span>' +
                    '）</span>';
                $("#prmTimerDiv").html(prmTimerDiv).show();
            $("#spanCommission").css("top", -5);

            // 倒计时
            prmTimer(timestamp,
                {
                    day: $("#prm_day_show"),
                    hour: $("#prm_hour_show"),
                    minute: $("#prm_minute_show"),
                    second: $("#prm_second_show")
                },
                function() {
                    // 展示该活动的下一个状态
                    if (promotionState === 1) {
                        sessionStorage.PromotionState = 2;

                        processPmtTemplate(2);
                    } else if (promotionState === 2) {
                        sessionStorage.PromotionState = 3;

                        processPmtTemplate(3);
                    } else if (promotionState === 3) {
                        sessionStorage.PromotionState = 4;
                        procPromotion45(1, 4);
                    }
                }
            );


        }

        // 秒杀 、 预约 抢光和结束的情况(活动类型，活动状态)

        function procPromotion45(promotionType, promotionState) {
            var footer = $("#footer"),
                promotionTemplate = "",
                proMsg = $("#promotionMsg"),
                msg = "";

            if (promotionType === 1) {
                // 秒杀
                pTptBtn = "立即抢购";
            } else if (promotionType === 2) {
                // 预约
                pTptBtn = "立即抢购";
            }

            if (promotionState === 5) {
                msg = "已抢光";
            } else if (promotionState === 4) {
                msg = "已结束";
            }

            proMsg.show().children("div").html(msg);
            promotionTemplate = '<div class="footer_4" id="addshopcart" style="margin-right: 5px; margin-left: 0;padding : 3.5% 8%;opacity: .8;">' +
                '加入购物车</div>' +
                '<div style="padding: 3.5% 8%; border-radius: 4px; background-color: rgb(255, 0, 84);  text-align: center; color: #fff; font-size: 1.3em; letter-spacing: 3px; display: inline-block;  opacity: .8;">' +
                '' + pTptBtn + '</div>';
            footer.html(promotionTemplate);
            footer.children("div").css({ "background-color": "#E0E0E0", "color": "#505050" });
        }

        // 事件处理

        function subscribeNow() {
            var txtVali = $("#val-txtVali"),
                valValImg = $("#val-valImg");
            txtVali.focus(function() {
                if (valValImg.attr("src").indexOf("/mobile/GetVerifyCodeZPH") < 0) {
                    valValImg.attr("src", getBtpDomain() + "mobile/GetVerifyCodeZPH?r=" + Math.random());
                }

            });

            valValImg.on("click", function() {
                $("#val-valImg").attr("src", getBtpDomain() + "mobile/GetVerifyCodeZPH?r=" + Math.random());
            });

            // 取消
            $("#val-cancel").on("click", function() {
                closeVali();
            });

            // 我的预约
            $("#btnGotoMyPresell").on("click", function() {
                window.location = zphUrl + "Presell/MyPresellComdty?userId=" + getUserId() + "&sessionId=" + getSessionId() + "&ChangeOrg=00000000-0000-0000-0000-000000000000&btpdomain=" + window.location.host;
            });

            // 
            $(".okMsgClose").on("click", function() {
                closeVali();
            });

            // 预约
            $("#val-submit").on("click", function() {
                var valVali = $.trim(txtVali.val()),
                    outPromotionId = sessionStorage.OutPromotionId;
                if (promotionState !== 1 || valVali === "" || outPromotionId === "") {
                    return false;
                }

                var sendData = {
                    outPromotionId: outPromotionId,
                    userId: getUserId(),
                    sessionId: getSessionId(),
                    verifyCode: valVali,
                    esAppId: getEsAppId()
                };

                // 验证码
                getDataAjax({
                    url: '/mobile/SaveMyPresellComdtyZPH',
                    data: sendData,
                    callback: function(result) {
                        $("#val-valImg").attr("src", getBtpDomain() + "mobile/GetVerifyCodeZPH?r=" + Math.random());
                        txtVali.val("");
                        //0，成功，1 已预约过，2 失败 
                        // 验证码 错误 接口返回值不对设置加事件,并更新缓存
                        if (result.ResultCode === 2) {
                            toast(result.Message);
                        } else if (result.ResultCode === 0) {
                            $("#valGetDiv").hide();

                            $("#okMsgDiv").show().children("p").html("商品预约成功，您已经获得抢购资格，请关注抢购时间~~~");
                        } else if (result.ResultCode === 1) {
                            $("#valGetDiv").hide();

                            $("#okMsgDiv").show().children("p").html("您已成功预约过了，无需重复预约，请关注抢购时间~~~");
                            //closeVali();
                        }

                        $("#ajaxLoadBlind").remove();
                    },
                    beforeSend: function() {
                        ajaxLoading('22', '');
                    }
                });

                return false;
            });


        }

        function closeVali() {
            // 关闭层
            $("#val-overlay").hide();
            $("#val-txtVali").val("");
            $("#val-valImg").attr("src", "");
        }

        // 设置提醒状态

        function setNotiSate() {
            if (!JsVilaDataNull(getUserId())) {
                $("#sendNotifications").text("设置提醒");
                return;
            }


            var sendData = {
                skId: sessionStorage.OutPromotionId,
                uId: getUserId()
            };

            // 设置状态
            getDataAjax({
                url: '/mobile/SetNotificationState',
                data: sendData,
                callback: function(result) {
                    if (result.state) {

                        $("#sendNotifications").text("取消提醒");
                    } else {

                        $("#sendNotifications").text("设置提醒");
                    }

                    $("#ajaxLoadBlind").remove();
                },
                beforeSend: function() {
                    ajaxLoading('22', '');
                }
            });
        }
    }
}

function prmTimer(timestamp, timeElement,callback) {
    var day = 0,
        hour = 0,
        minute = 0,
        second = 0; //时间默认值        

    var prm = window.setInterval(function () {
        var intDiff = parseInt(timestamp);
        if (intDiff >= 1) {
            day = Math.floor(intDiff / (60 * 60 * 24));
            hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
            minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
            second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
            if (minute <= 9) minute = '0' + minute;
            if (second <= 9) second = '0' + second;
            if(timeElement.day !== null){
                timeElement.day.html('<s></s>' + day);
                timeElement.hour.html('<s></s>' + hour);
                timeElement.minute.html('<s></s>' + minute);
                timeElement.second.html('<s></s>' + second);                
            }
            --timestamp;
            
        } else {
            if(timeElement.day !== null){
                timeElement.day.html('<s></s>' + "0");
                timeElement.hour.html('<s></s>' + "00");
                timeElement.minute.html('<s></s>' + "00");
                timeElement.second.html('<s></s>' + "00");
            }
            clearInterval(prm);
            callback();
            return;
        }

    }, 1000);
}

//设置.sessionStorage值
function setSessionStorage(key, p_key, value) {
    //临时对象
    var data;
    //判断是否有当前缓存值
    if (!sessionStorage[key]) {
        //没有则生成一个空对象
        sessionStorage[key] = '{}';
        //将data赋值为空对象
        data = {};
    } else {
        //有则将缓存的值赋值为data
        data = JSON.parse(sessionStorage[key]);
    }

    //重新赋值
    data[p_key] = value;
    //更新缓存
    sessionStorage[key] = JSON.stringify(data);
}

//最小购买数量
var _minBuyNum = 1;
//数量加减事件
function numberEvent() {
    //加减盒子对象
    var box = $('#number');
    //数字对象
    var number = box.find('.span_number');
    //当前数字
    var text = parseInt(number.val());
    //获取传入参数
    var f_arguments = arguments;
    // 每人限购数量
    var limitBuyEach = -1;

    //更新缓存数据
    setSessionStorage('commodityUpInfo', 'number', text);

    box.on("click",'.span_2', function() { //设置减事件,并更新缓存
        text = parseInt(number.val());
        if (text - _minBuyNum <= 0) {
            //text = 1;
            return;
        }
		if($('.plus').hasClass('hide')){
			$('.plus').removeClass('hide');
			$('.dis-plus').addClass('hide');
		}
        text -= 1;

        text = sessionStorage.Stock != '已售完' ? text : '已售完';

        number.val(text);
        setSessionStorage('commodityUpInfo', 'number', text);

        $('#pre_money_count').text(getCurrency() + Math.abs(sessionStorage.Price * text).toFixed(2));
        if (text <= sessionStorage.Stock) {
            $('#txtMaxStock').hide();
        }
        f_arguments.length ? f_arguments[0](text) : '';
        JsCrowdfund();
        calcCommodityFreight();
        specBtn();/*判断选规格按钮状态*/
    }).on("click",'.span_3', function() { //设置加事件,并更新缓存
        var commodityInfo = JSON.parse(sessionStorage.commodityInfo);
        text = parseInt(number.val());
        limitBuyEach = commodityInfo ? commodityInfo.LimitBuyEach : limitBuyEach;
        // -1 表示不限购
        if (limitBuyEach !== null && limitBuyEach !== -1 && text >= limitBuyEach) {
            return;
        }

        number.val(text < sessionStorage.Stock ? text += 1 : sessionStorage.Stock);

        if (text >= sessionStorage.Stock) {
			$(this).addClass('hide');
			$('.dis-plus').removeClass('hide');
            return;
        }

        setSessionStorage('commodityUpInfo', 'number', text);

        $('#pre_money_count').text(getCurrency() + Math.abs(sessionStorage.Price * text).toFixed(2));

        $('#txtMaxStock').text("库存仅剩" + sessionStorage.Stock + "件");
        if (text > sessionStorage.Stock) {
            $('#txtMaxStock').show();
        }

        f_arguments.length ? f_arguments[0](text) : '';
        specBtn();/*判断选规格按钮状态*/
        JsCrowdfund();
        calcCommodityFreight();
    });
    $(".span_number").on("keyup", function () {
                var v = $(this).val();
                v = v.replace(/[^\d]/g, ""); //先把非数字的都替换掉，除了数字和.
                v = v.replace(/^0/g, ""); //不能以0开头。 
            });
    $(".span_number").on("change", function() {
        var v = $(this).val();
        if (!regNum.test(v)) {
            $(this).val(_minBuyNum);
            $(this).focus();
            return;
        }
        if (v == "" || v == 0) {
            v = _minBuyNum;
            $(this).val(v);
        }

        var commodityInfo = JSON.parse(sessionStorage.commodityInfo);

        var commodityUpInfo = JSON.parse(sessionStorage.commodityUpInfo);
        var prevNum = commodityUpInfo.number;

        var limitBuyEach = -1;
        limitBuyEach = commodityInfo ? commodityInfo.LimitBuyEach : limitBuyEach;
        // -1 表示不限购
        if (limitBuyEach !== null && limitBuyEach !== -1 && v >= limitBuyEach) {
            $(this).val(prevNum);
            return;
        }

        //大于库存，只显示库存的量。
        v = parseInt(v) < parseInt(sessionStorage.Stock) ? v : sessionStorage.Stock;
        $(this).val(v);

        setSessionStorage('commodityUpInfo', 'number', v);

        $('#pre_money_count').text(getCurrency() + Math.abs(sessionStorage.Price * v).toFixed(2));

        $('#txtMaxStock').text("库存仅剩" + sessionStorage.Stock + "件");
        if (parseInt(v) > parseInt(sessionStorage.Stock)) {
            $('#txtMaxStock').show();
        }

        JsCrowdfund();
        calcCommodityFreight();
    });


}

 function calcCommodityFreight(cinfo) {
        if($("#freightTo").length ==0)
        {
            return;
        }
        var str = "";
        var commodityUpInfo = null;
        if(JsVilaDataNull(cinfo))
        {
            commodityUpInfo = cinfo;
        }
        else
        {
            commodityUpInfo = JSON.parse(sessionStorage.commodityUpInfo);
        }
        var subData = new Array();
        subData.push({ AppId: sessionStorage.appId, CommodityId: sessionStorage.commodityId_2, Count: commodityUpInfo.number, Price: commodityUpInfo.realPrice });
  
        var cparam = {};
        cparam.templateCounts = subData;
        cparam.freightTo = $("#freightTo").html();
        cparam.isSelfTake = commodityUpInfo.IsEnableSelfTake;
        var strjson = JSON.stringify(cparam);

        getDataAjax2({
            url: "/Mobile/CalFreight",
            data: strjson,
            callback: function(result) {
                if (result.ResultCode == 0) {
                    var freight = Math.abs(result.Freight).toFixed(2);
                    $('#freight').html(freight);
                    setSessionStorage('commodityUpInfo', 'freight', freight);
                }
            }
        });
    }

function getFormatDate(day) {
    var Year = 0;
    var Month = 0;
    var Day = 0;
    var Hour = 0;
    var Minute = 0;
    var Second = 0;
    var CurrentDate = "";

//初始化时间 
    Year = day.getFullYear(); //ie火狐下都可以 
    Month = day.getMonth() + 1;
    Day = day.getDate();
    Hour = day.getHours();
    Minute = day.getMinutes();
    Second = day.getSeconds();
    CurrentDate += Year + "-";
    if (Month >= 10) {
        CurrentDate += Month + "-";
    } else {
        CurrentDate += "0" + Month + "-";
    }
    if (Day >= 10) {
        CurrentDate += Day;
    } else {
        CurrentDate += "0" + Day;
    }
    CurrentDate += " ";
    if (Hour >= 10) {
        CurrentDate += Hour + ":";
    } else {
        CurrentDate += "0" + Hour + ":";
    }

    if (Minute >= 10) {
        CurrentDate += Minute + ":";
    } else {
        CurrentDate += "0" + Minute + ":";
    }
    if (Second >= 10) {
        CurrentDate += Second;
    } else {
        CurrentDate += "0" + Second;
    }
    return CurrentDate;
}

//商品详情评价滑动事件
function reviewScrollEvent() {
    if (window.location.hash != "#review")
        return;
    if (scrollLoading == false) {

        var scrollHeight = $(document).height() > $(window).height() ? $(document).height() - $(window).height() : 0;
        var scrollTop = $(window).scrollTop();
        var scrollBottom = scrollHeight - scrollTop;

        //滚动加载
        if (scrollBottom <= advance && maxHeight < scrollHeight) {
            maxHeight = scrollHeight;
            scrollLoading = true;
            getCommodityInfoReplays({ type: false, loadFromCache: false, limit: 0 });
        }
    }
}

var lastReviewTime = '';

//获取商品留言
//obj.type:true清空
//obj.loadFromCache:true,清空并加载缓存的数据
//obj.limit: 大于0，则只加载obj.limit条数据
function getCommodityInfoReplays(obj) {
    !window.reviewByCommodity ? window.reviewByCommodity = [] : '';
    var content_2 = $('#reviewList');                    //留言盒子父对象
    var content_2_clone = $('#review_clone');            //留言盒子克隆对象
    //var replay_clone = content_2_clone.find('.replays__');  //留言盒子留言条数对象
    var tmpClone;                                           //临时克隆对象
    //var tmpReplayClone;                                     //临时留言条目对象
    var limit = obj.limit;                               //最多加载数量
    if (obj.type) {
        content_2.empty();
    }
    //提交查询数据
    var data = {
        appId: sessionStorage.appId || getQueryString('shopId'),
        commodityId: sessionStorage.commodityId_2 || getQueryString('commodityId'),
        lastReviewTime: lastReviewTime
    };

    //是否有缓存过当前商品评价对象
    //没有则获服务器数据
    if (obj.loadFromCache && window.reviewByCommodity.length >0) {
        setData(window.reviewByCommodity);
        scrollLoading = false;
    } else {
        if (window.getAjax_2) {
            window.getAjax_2.abort();
            getAjax();
        } else {
            getAjax();
        }
    }

    function getAjax() {
        window.getAjax_2 = getDataAjax({
            url: '/Mobile/GetReviewByCommodityId',
            data: obj.inData || data,
            beforeSend: function () {
            },
            callback: function (data) {
                if (data.length) {
                    var time = data[data.length - 1].SubTime.match(/\d/g).join('');
                    lastReviewTime = getFormatDate(new Date(Number(time)));
                    window.reviewByCommodity = window.reviewByCommodity.concat(data);
                    setData(data);
                }
                scrollLoading = false;
            }
        });
    }

    //设置盒子内容方法
    function setData(data) {
        var dataCnt = data.length;
        if (limit && limit < data.length) {
            dataCnt = limit;
        }

        //遍历设置留言数据
        for (var i = 0; i < dataCnt; i++) {
            tmpClone = content_2_clone.clone().removeClass('noDisplay').removeAttr('id');
            tmpClone.find('.user').text(data[i].Name.replace(/null/gi, ''));
            tmpClone.find('.cmt_cnt').text(data[i].Details);
            tmpClone.find('.date').text(data[i].ShowTime);
            tmpClone.find('.cmt_sku1').text( data[i].Size ? data[i].Size.replace(/null/gi, '') : '');
//            for (var b = 0; b < data[i].Replays.length; b++) {
//                tmpReplayClone = replay_clone.clone().removeClass('noDisplay').removeAttr('id');
//                tmpReplayClone.find('.replays_5').text(data[i].Replays[b].ReplyerName + ': ');
//                tmpReplayClone.find('.replays_6').text(data[i].Replays[b].Details);
//                tmpReplayClone.find('.replays_7').find('span').text(data[i].Replays[b].ShowTime);
//                tmpClone.find('.left_3').append(tmpReplayClone);
//            }
//            data[i].UserHead ?
//				data[i].UserHead != 'null' ? tmpClone.find('.replays_8').attr('src', data[i].UserHead) : ''
//			: '';
            
            var li = $('<li></li>');
            li.append(tmpClone);
            content_2.append(li);
        }
    }
}

//封装ajax方法 get
function getDataAjax(obj) {
    var requestAsync = true;
    if (obj.async === false) {
        requestAsync = false;
    }
    return $.ajax({
        url: obj.url,
        type: 'get',
        async: requestAsync,
        contentType: "application/json",
        data: obj.data,
        success: obj.callback,
        beforeSend: obj.beforeSend,
        error: function(e) {
            if (e.status) {
                toast("网络异常，请重试~");
                obj.error && obj.error();    
            }
        },
        complete:obj.complete,
        dataType: 'json'
    })
}

//复制        post
function getDataAjax2(obj) {
    var requestAsync = true;
    if (obj.async === false) {
        requestAsync = false;
    }
    if (obj.data && $.type(obj.data) != "string") {
        obj.data = JSON.stringify(obj.data);
    }
    $.ajax({
        url: obj.url,
        type: 'post',
        async: requestAsync,
        contentType: "application/json",
        data: obj.data,
        success: obj.callback,
        beforeSend: obj.beforeSend,
        error: function(e) {
            if (e.status) {
                toast("网络异常，请重试~");
                obj.error && obj.error();    
            }
        },
        complete:obj.complete,
        dataType: 'json'
    })
}

function getDataAjax3(obj) {
    return $.ajax({
        url: obj.url,
        type: 'post',
        //		contentType: "application/json",
        data: obj.data,
        success: obj.success,
        beforeSend: obj.beforeSend,
        error: function(e) {
            if (e.status) {
                toast("网络异常，请重试~");
                obj.error && obj.error();    
            }
        },
        complete:obj.complete,
        dataType: 'json'
    })
}

//获取商品列表 ajax
function getCommodity(obj) {

    csson($("#cdefault"));
    var data = {
        appId: obj.appId,
        pageIndex: obj.pageIndex,
        pageSize: obj.pageSize,
        promotionId: getQueryString('promotionId')
    };
    if (!obj.appId || obj.appId == null || obj.appId == 'null' || obj.appId == 'undefined' || obj.appId == undefined)
        return;
    if ((sessionStorage.ProductType == "appcjzy" || sessionStorage.ProductType == "webcjzy") && JsVilaDataNull(getCookie("selectCityCode"))) {
        data.areaCode = getCookie("selectCityCode")||'';
    }
    return getDataAjax({
        url: '/Mobile/GetCommodity',
        data: data,
        beforeSend: obj.beforeSend,
        callback: obj.callback
    });
}

//商品列表中设置各个商品相关属性
function setItemData(obj) {
    var clone_item = obj.element;
    var data = obj.data;
    if (window.itemIdList) {
        if (window.itemIdList[data.Id]) {
            return '';
        }
        window.itemIdList[data.Id] = 'on'
    } else {
        window.itemIdList = {};
        window.itemIdList[data.Id] = 'on'
    }
    clone_item.css({
        //		height: obj.height
    }).attr('data-id', data.Id);
    //	clone_item.find('a')[0].href += '?id=' + data.Id;
    clone_item.find('.img').parent().css({ 'height': Math.round(sessionStorage.img_height) + 'px', 'overflow': 'hidden' }).
		find('.img').css('height', Math.round(sessionStorage.img_height) + 'px').attr('src', data.Pic)
		.addClass('LazyLoad')[0].onerror = function () {
		    console.log(self.css({ background: 'none' }).attr('src', '/Content/Mobile/b_2.png'));
		};
     clone_item.find('.selfTakeImg').parent().css({ 'height': Math.round(sessionStorage.img_height) + 'px', 'overflow': 'hidden' }).
        find('.selfTakeImg').css('height', Math.round(sessionStorage.img_height) + 'px');
    clone_item.find('.item_title').text(data.Name, 8, "...");
    var isyou = false;
    if (data.DiscountPrice > 0) {
        clone_item.find('.price_1').text(getCurrency() + Math.abs(data.DiscountPrice).toFixed(2));
        clone_item.find('.price_2').text(getCurrency() + data.Price);
        clone_item.find('.zk').text('优惠价')
        isyou = true;
    }
    else {
        if (data.Intensity == 10) {
            clone_item.find('.price_1').text(getCurrency() + data.Price);
            if (data.MarketPrice && data.MarketPrice != null && data.MarketPrice != 'null') {
                clone_item.find('.price_2').text(getCurrency() + data.MarketPrice);
            } else {
            clone_item.find('.price_2').hide();
            }
        } else {
            clone_item.find('.price_1').text(getCurrency() + Math.abs(data.Price * (data.Intensity / 10)).toFixed(2));
            clone_item.find('.price_2').text(getCurrency() + data.Price);
            isyou = true;
        }
        data.Intensity != 10 ? clone_item.find('.zk').text(data.Intensity + '折') : '';
    }

    var lim = ""; 

    //正品会不显示限购信息。
    if (isyou == true
    && sessionStorage.ProductType != "appcjzy" 
    && sessionStorage.ProductType != "webcjzy") {
        if (data.LimitBuyEach > -1 && data.LimitBuyEach) {
            lim += "每人限购" + data.LimitBuyEach + "件,";
        }
        else {
            lim += "不限购,";
        }

        if (data.LimitBuyTotal > -1 && data.LimitBuyTotal) {
            lim += "还剩" + (parseInt(data.LimitBuyTotal) - parseInt(data.SurplusLimitBuyTotal)) + "件";
        }
        else {
            lim += "还剩" + data.Stock + "件";
        }
        clone_item.find('.yhzknum').text(lim);
    }
    !(data.Stock > 0) ? clone_item.find('.mb_box').removeClass('noDisplay') : '';
    data.State == 1 ? clone_item.find('.mb_box').removeClass('noDisplay').find('.mb_1').text('已下架') : '';
    data.IsEnableSelfTake==1? clone_item.find('.selfTake').removeClass('noDisplay') : clone_item.find('.selfTake').addClass('noDisplay');
   
    //	if(!window.itemHeight) {
    //		var clones = clone_item.clone();
    //		$('body').append(clones.css({
    //			position: 'absolute',
    //			zIndex: '-1',
    //			left: -3000 + 'px'
    //		}));
    //		setTimeout(function () {
    //			window.itemHeight = sessionStorage.item_height + 'px';
    //		}, 1000);
    //	}

    //clone_item.css({ height: sessionStorage.item_height + 'px', overflow: 'hidden' });

    return clone_item;
}

//生成一个商品项（列表模式下的）
function getCommodityItem_ListMode(obj)
{
    var alen = $("#itemsListMode a[data-id='"+obj.data.Id+"']").length;
    if(alen > 0)
    {
        return "";
    }
    //{ element: clone_item, data: data[i], height: height, img_height: img_height }
    var data ={};
    $.extend(data,obj.data); 

    data.Price_1 = "0.00";
    data.Price_2 = "0.00";
    data.Price_2_Display = "inlline-block";
    data.LimitBuy = "";
    data.FloatMsg = "已售完";
    data.FloatMsg_Display = "block";

     
     var isyou = false;
    if (data.DiscountPrice > 0) {
        data.Price_1 =  Math.abs(data.DiscountPrice).toFixed(2);
        data.Price_2 =  data.Price;
        //clone_item.find('.zk').text('优惠价')
        isyou = true;
    }
    else {
        if (data.Intensity == 10) {
            data.Price_1 =  data.Price;
            if (data.MarketPrice && data.MarketPrice != null && data.MarketPrice != 'null') {
                data.Price_2 =  data.MarketPrice;
            } else {
             data.Price_2_Display = "none";
            }
        } else {
            data.Price_1 =  Math.abs(data.Price * (data.Intensity / 10)).toFixed(2);
            data.Price_2 = data.Price;
            isyou = true;
        }
        //data.Intensity != 10 ? clone_item.find('.zk').text(data.Intensity + '折') : '';
    }

    var lim = ""; 
    //正品会不显示限购信息。
    if (isyou == true
    && sessionStorage.ProductType != "appcjzy" 
    && sessionStorage.ProductType != "webcjzy") {
        if (data.LimitBuyEach > -1 && data.LimitBuyEach) {
            lim += "每人限购" + data.LimitBuyEach + "件,";
        }
        else {
            lim += "不限购,";
        }

        if (data.LimitBuyTotal > -1 && data.LimitBuyTotal) {
            lim += "还剩" + (parseInt(data.LimitBuyTotal) - parseInt(data.SurplusLimitBuyTotal)) + "件";
        }
        else {
            lim += "还剩" + data.Stock + "件";
        }
    }
    data.LimitBuy = lim;

    if(data.Stock <= 0)
    {
        data.FloatMsg_Display = "block";
        if(data.State == 1)
        { 
            data.FloatMsg = "已下架";
        }
        else
        {
            data.FloatMsg = "已售完";
        }
    }
    else
    {
         data.FloatMsg_Display = "none";
    }
     if(data.IsEnableSelfTake==1)
     {
        data.SelfTakeDisplay = "inline-block";
     }
     else
     {
        data.SelfTakeDisplay = "none";
     } 
     data.srckey = "src";
    

    //已下架、折扣、自提

    var cItemHtml =  $("#divCommodityItemListMode").html();
    cItemHtml = cItemHtml.format(data); 
    return cItemHtml;
}

 

function csson(obj) {

    $(".toporder ul li").each(function (i) {
        $(this).removeClass("topon");
    });
    $(obj).addClass("topon");
};

//保存商品列表数据。
var _commodityListData = new Array();

//商品列表按销量排序
function ComOrderList(fieldSort, order, state) {

    var data = { categoryId: "11111111-1111-1111-1111-111111111111", appId: sessionStorage.appId, pageIndex: sessionStorage.page, pageSize: 10, fieldSort: fieldSort, order: order };
    var areaCode = "";
    if ((sessionStorage.ProductType == "appcjzy" || sessionStorage.ProductType == "webcjzy") && JsVilaDataNull(getCookie("selectCityCode"))) {
        data.areaCode = getCookie("selectCityCode") || '';
    }
    getDataAjax({
        url: '/Mobile/GetOrByCommodity',
        data: data,
        callback: function (data) {
            window.itemIdList = {};
            if (state == 1) {
                $("#items").html("");
                $('#itemsListMode').html("");
                _commodityListData = [];
            }
            //toast( $("#parent_item").html());
            //                    $("#parent_item").html("");
            //                    $("#parent_item").html($(".yingc").html());

            sessionStorage.data = JSON.stringify(data);

            //sessionStorage.commodityList = JSON.stringify(data);
            _commodityListData = _commodityListData.concat(data);



            var items = $('#items');
            var item = $('#parent_item');
            var height = item.height();
            var img_height = item.find('img').height();
            var clone_item;

            var itemsListMode =  $('#itemsListMode');

            for (var i = 0; i < data.length; i++) {
                clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                //toast(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                var ghParam = { element: clone_item, data: data[i], height: height, img_height: img_height };
                items.append(setItemData(ghParam));
                var html = getCommodityItem_ListMode(ghParam);
                itemsListMode.append(html);
            }
            //					items.append(div);
            $("#ajaxLoadBlind").remove();

            showLazyLoadImg();
            if (data.length >= 10)
                $('#footer_loading').show().find('span').text('获取更多信息');
        },
        beforeSend: function () {
            //                    ajaxLoading('22', '');
        },
        error: function () {

            $("#ajaxLoadBlind").remove();
        }
    });
}
//商品列表获取分页
function getCommodityByCategory(categoryId) {
    window.itemIdList = {};
    var items = $('#items');
    var item = $('#parent_item');
    var height = item.height();
    var img_height = item.find('img').height();
    var clone_item;
    var $footer_loading = $('#footer_loading');
    var $top_loading = $('#top_loading');
    var type = false;

     var itemsListMode =  $('#itemsListMode');

    if (!sessionStorage.categoryLock) {
        items.empty();
        itemsListMode.empty();
        _commodityListData = [];
    }

    if (categoryId) {
        sessionStorage.categoryId = categoryId;
        var data = {
            categoryId: categoryId,
            appId: sessionStorage.appId,
            pageIndex: sessionStorage.page,
            pageSize: 10
        };


        getDataAjax({
            url: '/Mobile/GetCommodityByCategory',
            data: data,
            beforeSend: function () {
                $footer_loading.find('span').text('正在获取信息...');
            },
            callback: function (data) {
                if (data.length) {
                    
                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').removeAttr('id');
                        var ghParam ={ element: clone_item, data: data[i], height: height, img_height: img_height };
                        items.append(setItemData(ghParam));
                        
                         var html = getCommodityItem_ListMode(ghParam);
                         itemsListMode.append(html);
                    }
                    showLazyLoadImg();
                    sessionStorage.page++;
                } else {
                    if (data.length > 10) {
                        $footer_loading.removeClass('clicks_lock').find('span').text('没有更多商品');
                    }
                }

                if (data.length < 10) {
                    $footer_loading.hide();
                } else {
                    $footer_loading.removeClass('clicks_lock').show().find('span').text('获取更多信息');
                }

                $top_loading.find('span').text('已更新');

                type = true;
            }
        });
    } else {
        $footer_loading.hide();
        items.html('<div style="text-align: center;line-height: 20em;">暂无信息</div>');
    }

    $("#Commodity_nav").css("left", "-" + $("#Commodity_nav").width() + "px");
    return type;
}

//获取商品分类列表
function getNavList() {
    var l_length = 0;
    var data = {
        appId: sessionStorage.appId
    };
    var li = $('<li></li>');
    var nav_list = $('#nav_list');
    nav_list.html("");
    if (nav_list.length) {
        getNavObj();
    }

    function getNavObj() {
        nav_list = $('#nav_list');
        setTimeout(function () {
            if (nav_list.length) {
                getNavObj();
            }
        }, 500);
    }
    if (sessionStorage.commodityNavList) {
        if (sessionStorage.commodityNavList.length) {
            setItem(JSON.parse(sessionStorage.commodityNavList));
        } else {
            getAjax();
        }
    } else {
        getAjax();
    }

    function getAjax() {
        getDataAjax({
            url: '/Mobile/GetCategory',
            data: data,
            callback: function (data) {
                sessionStorage.commodityNavList = JSON.stringify(data);
                localStorage.commodityNavList = JSON.stringify(data);
                setItem(data);
            },
            error: function () {
                if (l_length < 3) {
                    l_length++;
                    getAjax();
                } else {
                    setItem(JSON.parse(localStorage.commodityNavList));
                }
            }
        });
    }

    function setItem(data) {
        var show_item_key = 0;
        for (var i = 0; i < data.length; i++) {
            nav_list.append(li.clone().addClass('nav_one').
				text(data[i].Name).data('category-id', data[i].Id));

            for (var b = 0; b < data[i].SecondCategory.length; b++) {
                nav_list.append(li.clone().addClass('nav_two nav_inner').
					text(data[i].SecondCategory[b].Name).data('parent-name', data[i].Name).data('show-item', show_item_key)
					.data('category-id', data[i].SecondCategory[b].Id).append($('<span class="nav_inners"></span>')));

                for (var c = 0; c < data[i].SecondCategory[b].ThirdCategory.length; c++) {
                    nav_list.append(li.clone().addClass('nav_two hidden').
						text(data[i].SecondCategory[b].ThirdCategory[c].Name).data('show-key', show_item_key)
						.data('category-id', data[i].SecondCategory[b].ThirdCategory[c].Id));
                }

                show_item_key++;
            }
        }

        nav_list.append(li.clone().addClass('nav_one').
			text('未分类').data('category-id', '00000000-0000-0000-0000-000000000000'));
    }
}

//关键字查询商品
function getWantCommodityData(want) {
    var items = $('#items');
    var item = $('#parent_item');
    var height = item.height();
    var img_height = item.find('img').height();
    var clone_item;
    var $footer_loading = $('#footer_loading');
    var $top_loading = $('#top_loading');
    var type = false;

    var itemsListMode =  $('#itemsListMode');

    window.itemIdList = {};

    if (!sessionStorage.wantLock) {
        items.empty();
        itemsListMode.empty();
        _commodityListData = [];
    }

    sessionStorage.want = want;
    var data = {
        want: want,
        appId: sessionStorage.appId,
        pageIndex: sessionStorage.page,
        pageSize: 10
    };

    getDataAjax({
        url: '/Mobile/GetWantCommodity',
        data: data,
        callback: function (data) {
            if (data.length) {
                for (var i = 0; i < data.length; i++) {
                    clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                    items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                }

                showLazyLoadImg();
                sessionStorage.page++;
            } else {
                if (data.length > 10) {
                    $footer_loading.removeClass('clicks_lock').find('span').text('没有更多商品');
                }
            }

            if (data.length < 10) {
                $footer_loading.hide();
            } else {
                $footer_loading.removeClass('clicks_lock').show().find('span').text('获取更多信息');
            }
            type = true;

            $top_loading.find('span').text('已更新');
        }
    });

    return type;
}

//商品列表页首次加载使用函数
//涉及到页面加载时获取第一分页信息.
//设置侧边栏分类信息.
//侧边栏相关事件.
function CommodityList() {
    $(function () {
        sessionStorage.page = 1;
        new TouchMoveEvent();
        //		newTouchMoveEvent();
        navEvent();

        if (sessionStorage.appId && sessionStorage.appId != "undefined" && sessionStorage.appId != "null") {
        }
        else {
            sessionStorage.appId = getQueryString('shopId') || appId;
        }

        //getNavList();

        //        if (sessionStorage.commodityList) {
        //            if (sessionStorage.commodityList.length) {
        //                (function () {
        //                    var items = $('#items');
        //                    var item = $('#parent_item');
        //                    var height = item.height();
        //                    var img_height = item.find('img').height();
        //                    var clone_item;
        //                    var data = JSON.parse(sessionStorage.commodityList);

        //                    for (var i = 0; i < data.length; i++) {
        //                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
        //                        items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
        //                    }
        //                    showLazyLoadImg();
        //                    if (data.length >= 10)
        //                        $('#footer_loading').show().find('span').text('获取更多信息');
        //                })();
        //            } else {
        //                getAjaxData();
        //            }
        //        } else {

        //        }
        if (getQueryString("sortType") == "New" && sessionStorage.ComTypeSearch != 1) {
            sessionStorage.appId = getQueryString('shopId');
            csson($("#cnewcom"));
            ajaxLoading('22', '')
            sessionStorage.page = 1;
            sessionStorage.ComTypeSearch = 4;
            ComOrderList(2, 0, 1);
        }
        else {
            getAjaxData();
        }
        function getAjaxData() {
            getCommodity({
                appId: sessionStorage.appId,
                pageIndex: sessionStorage.page,
                pageSize: 10,
                callback: function (data) {   
                    sessionStorage.data = JSON.stringify(data);
                    //sessionStorage.commodityList = JSON.stringify(data);
                    _commodityListData = _commodityListData.concat(data);

                    if (!data || data.length == 0) {
                        $("#findNone").show();
                        return;
                    } else {
                        $("#findNone").hide();
                    }

                    var items = $('#items');
                    var item = $('#parent_item');
                    var height = item.height();
                    var img_height = item.find('img').height();
                    var clone_item;
                     var itemsListMode =  $('#itemsListMode');

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');

                        var ghParam = { element: clone_item, data: data[i], height: height, img_height: img_height };
                        items.append(setItemData(ghParam));

                         var html = getCommodityItem_ListMode(ghParam);
                         itemsListMode.append(html);
                    }
                    //					items.append(div);
                    showLazyLoadImg();
                    if (data.length >= 10)
                        $('#footer_loading').show().find('span').text('获取更多信息');
                    if (getQueryString('promotionId') != '' && getQueryString('promotionId') != undefined) {
                        $('.toporder').addClass('noDisplay');
                    }
                }
            });
        }
    });

    var touchLock = true;

    /**
    * 手势事件
    *
    */
    function newTouchMoveEvent() {
        var nav = document.getElementById('Commodity_nav'),
			$nav = $(nav),
			navStartLeft,
			navWidth = $nav.css({ position: "absolute", visibility: "hidden", display: "block" }).width(),
			navMoveAutoMinSize = 20,
			starSize;

        $nav.css({ position: "fixed", visibility: "inherit", display: "none", left: -navWidth,height: $(window).height() });

        touch.config({
            tap: true,                  //tap类事件开关, 默认为true
            doubleTap: true,            //doubleTap事件开关， 默认为true
            hold: true,                 //hold事件开关, 默认为true
            holdTime: 650,              //hold时间长度
            swipe: true,                //swipe事件开关
            swipeTime: 300,             //触发swipe事件的最大时长
            swipeMinDistance: 18,       //swipe移动最小距离
            swipeFactor: 5,             //加速因子, 值越大变化速率越快
            drag: true,                 //drag事件开关
            pinch: true                //pinch类事件开关
        });

        touch.on('body', 'swipestart', function (ev) {
            starSize = ev.x;
            navStartLeft = parseInt(nav.style.left);
        });

        touch.on('body', 'touchmove', function (e) {
            //			e.preventDefault();
        });

        touch.on('body', 'swiping', function (ev) {
            if (Math.abs(ev.x) > navMoveAutoMinSize) {
                nav.style.left = navStartLeft + ev.x - navMoveAutoMinSize + 'px';
                nav.style.display = 'block';
                nav.style.zIndex = '3';
            }
        });

        touch.on('body', 'swipeend', function (ev) {
            var left = Math.abs(parseInt(nav.style.left));

            if (starSize - ev.x < 0) {
                if (left > navWidth - navMoveAutoMinSize) {
                    nav.style.left = -navWidth + 'px';
                } else {
                    nav.style.left = '0px';
                }
            } else {
                if (left < navMoveAutoMinSize) {
                    nav.style.left = '0px';
                } else {
                    nav.style.left = -navWidth + 'px';
                }
            }
        });




        //获取样式表信息
        function getStyle(obj, styleName) {
            return document.defaultView.getComputedStyle(obj, false)[styleName];
        }
    }

    function TouchMoveEvent() {
        var body = document.getElementsByTagName('body')[0]; //缓存body元素
        var main = document.getElementById('main'); //缓存main元素
        var box = document.getElementById('box'); //缓存box元素
        var nav = document.getElementById('Commodity_nav'); //缓存nav元素
        var top_loading = document.getElementById('top_loading'); //头部加载文字盒子
        var footer_loading = document.getElementById('footer_loading'); //底部加载文字盒子
        var $main = $(main);
        var $body = $(body);
        var $nav = $(nav);
        var $box = $(box);
        var $top_loading = $(top_loading);
        var $footer_loading = $(footer_loading);
        var startPageX = 0; //横轴开始值
        var startPageY = 0; //纵轴开始值
        var top_loading_start_size = 0;
        var moveSize = 0; //横轴移动量
        var moveSizeY = 0; //纵轴移动量
        var moveYMax = 10; //纵轴阀值
        var moveXMax = 10; //横轴阀值
        var startY = document.body.scrollTop; //记录滚动条距离
        var startX = document.body.scrollLeft;
        var startNavLeft = 0;
        var boxLeft = 0; //盒子左边距
        var touchMoveMinSize = 20; //最小移动距离.当到达这个值时将自动移动到touchMoveMaxSize设置的值
        var touchMoveMaxSize = parseInt($nav.width());
        //		var touchMoveMaxSize = $nav.css({position: "absolute", visibility: "hidden", display: "block" }).width(); //最大移动距离.
        //		$nav.css({position: "fixed", visibility: "inherit", display: "block" });
        var lockY = true; //纵轴锁
        var lockX = false; //横走锁
        var lockInnerX = true; //纵轴内部锁.该值为真时才能横向移动
        var mainLock = true; //main元素横向锁
        var navScrollStart = 0; //nav开始值
        var navScrollTop = 0; //nav滚动值
        var navMove = 0; //nav移动量
        var footerLoadingLock = false;
        var topLoadingLock = false;
        var type = {};

        var lock = true;

        var testBox = $('<div id="testBox"></div>').css({
            position: 'fixed',
            top: 0,
            height: '200px',
            width: '300px',
            left: 0,
            zIndex: 999,
            backgroundColor: '#fff'
        }
		);

        $nav.css({
            //			left: '-' + touchMoveMaxSize + 'px',
            zIndex: 3
        });

        body.addEventListener('touchstart', function (e) {
 
            var touch = e.changedTouches[0];
            startPageX = touch.screenX;
            startPageY = touch.screenY;
            boxLeft = parseInt($(box).css('left'));
            box.style.left = boxLeft + 'px';
            startNavLeft = parseInt($nav.css('left'));
            lockX = false;
            lockY = true;
            startY = body.scrollTop;
            top_loading_start_size = parseInt($top_loading.css('margin-top'));
            footerLoadingLock = false;
            topLoadingLock = false;
            $nav.css({ height: $(window).height() });

            if (parseInt($nav.css('left')) > -20) {
                lockY = false;
                //				e.preventDefault();
                lockX = true;
            }

            if ($(document).height() > $(window).height()) {
                type.h = $(document).height() - $(window).height();
            } else {
                type.h = $(window).height() - $(document).height();
            }

            touchMoveMaxSize = parseInt($nav.width());
        });
        body.addEventListener('touchmove', function (e) {
 
            var touch = e.changedTouches[0];
            moveSize = touch.screenX - startPageX;
            moveSizeY = touch.screenY - startPageY;

            //判断当前纵轴移动量是否超过阀值.则将横轴锁打开
            //				if (Math.abs(moveSizeY) > moveYMax && lockX && lockY) {
            //					lockX = false;
            //				}

            //判断当前横轴移动量是否超过阀值.则将纵轴锁打开
            if (Math.abs(moveSize) > moveXMax && Math.abs(moveSizeY) < moveYMax) {
                lockY = false;
                e.preventDefault();
                lockX = true;
            }

            //纵移动相关
            if (lockY) {
                if (startNavLeft == 0) {
                    $nav.css({
                        left: 0
                    });
                } else {
                    $nav.css({
                        left: -touchMoveMaxSize + 'px'
                    });
                }

                if (startY == 0 && moveSizeY > 0) {
                    var top_move = top_loading_start_size + moveSizeY;
                    if (top_move <= 0) {
                        e.preventDefault();
                        $top_loading.css({
                            marginTop: top_move
                        })
                    } else {
                        $top_loading.css({
                            marginTop: 0
                        }).find('span').text('松开可更新');
                        topLoadingLock = true;
                    }
                }

                if ($body.scrollTop() == type.h) {
                    if (moveSizeY < -10 && $footer_loading.find('span').text() != '正在获取信息...') {
                        $footer_loading.find('span').text('松开可更新');
                        e.preventDefault();
                        footerLoadingLock = true;
                    }
                } else {
                    //						$footer_loading.find('span').text('获取更多信息');
                }
            }

            //横移动相关
            if (lockX) {
                if (Math.abs(moveSizeY) < 10) {
                    e.preventDefault();
                    $nav.show();
                    var ll = startNavLeft + moveSize;
                    if (ll < 0) {
                        $nav.css({
                            left: startNavLeft + moveSize + 'px'
                        })
                    }
                    body.scrollTop = startY;
                }
            }

        });
        body.addEventListener('touchend', function (e) {
 
            touchMoveMaxSize = parseInt($nav.width());
            type.moveEnd = new Date().getTime();

            var touch = e.changedTouches[0];
            var mo = startPageX - touch.screenX;
            //				moveSizeY = startPageY - touch.screenY;

            //				if (lockY) {
            //					var item = $('#main').find('.item_1').eq(0);
            var items = $('#items');

            if (topLoadingLock) {
                //sessionStorage.commodityList = '';
                _commodityListData = [];
                setUpListItem();
            } else {
                $top_loading.css('margin-top', '-' + $top_loading.css('height'));
            }

            if (footerLoadingLock) {
                var bnext = false;
                switch (sessionStorage.ComTypeSearch) {
                    case "2":
                        bnext = true;
                        sessionStorage.page++;
                        ComOrderList(1, sessionStorage.PriceState, 0);
                        break;
                    case "3":
                        bnext = true;
                        sessionStorage.page++;
                        ComOrderList(0, 0, 0);
                        break;
                    case "4":
                        bnext = true;
                        sessionStorage.page++;
                        ComOrderList(2, 0, 0);
                        break;
                }
                if (bnext == false) {
                    setDownListItem();
                }
            }
            //				}

            //横移动相关  nav相关
            if (lockX) {
                var n_left = parseInt($nav.css('left'));

                if (mo > 0) {
                    if (Math.abs(n_left) > 20) {
                        $nav.css('left', -touchMoveMaxSize + 'px')
                    } else {
                        $nav.css('left', 0);
                    }
                } else {
                    if (Math.abs(n_left) > touchMoveMaxSize - 20) {
                        $nav.css('left', -touchMoveMaxSize + 'px')
                    } else {
                        $nav.css('left', 0)
                    }
                }
            }

            moveSizeY = 0;
            moveSize = 0;
        });

        //向上获取数据
        function setUpListItem() {
            if (sessionStorage.categoryId) {
                $top_loading.find('span').text('正在获取信息...');
                $('#items').empty();
                $('#itemsListMode').empty();
                _commodityListData = [];
                sessionStorage.page = 1;
                sessionStorage.categoryLock = true;
                getCommodityByCategory(sessionStorage.categoryId);

                setTimeout(function () {
                    $top_loading.css('margin-top', '-' + $top_loading.css('height'))
						.find('span').text('下拉可刷新');
                }, 1000);

            } else if (sessionStorage.want) {
                $top_loading.find('span').text('正在获取信息...');
                $('#items').empty();
                $('#itemsListMode').empty();
                _commodityListData = [];
                sessionStorage.page = 1;
                sessionStorage.wantLock = true;
                getWantCommodityData(sessionStorage.want);

                setTimeout(function () {
                    $top_loading.css('margin-top', '-' + $top_loading.css('height'))
						.find('span').text('下拉可刷新');
                }, 1000);

            } else {
                getCommodity({
                    appId: sessionStorage.appId,
                    pageIndex: 1,
                    pageSize: 10,
                    beforeSend: function () {
                        $top_loading.find('span').text('正在获取信息...');
                    },
                    callback: function (data) {
                        //					if (sessionStorage.data != JSON.stringify(data)) {
                        var items = $('#items').empty();
                        var itemsListMode = $('#itemsListMode').empty();
                        var item = $('#parent_item');
                        var height = item.height();
                        var img_height = item.find('img').height();
                        var clone_item;

                        _commodityListData = [];

                        window.itemIdList = {};

                        for (var i = 0; i < data.length; i++) {
                            clone_item = item.clone().removeClass('noDisplay').attr('id', '');

                            var ghParam = {
                                element: clone_item,
                                data: data[i],
                                height: height,
                                img_height: img_height
                            };


                            items.append(setItemData(ghParam));


                              var html = getCommodityItem_ListMode(ghParam);
                                itemsListMode.append(html);
                        }

                        $top_loading.find('span').text('已更新');
                        sessionStorage.data = JSON.stringify(data);
                        //sessionStorage.commodityList = JSON.stringify(data);
                        _commodityListData = _commodityListData.concat(data);
                        sessionStorage.page = 1;
                        setTimeout(function () {
                            $top_loading.css('margin-top', '-' + $top_loading.css('height'))
								.find('span').text('下拉可刷新');
                        }, 1000);
                        $footer_loading.show();
                        //					} else {
                        //						$top_loading.find('span').text('暂无更新');
                        //						setTimeout(function () {
                        //							$top_loading.css('margin-top', '-' + $top_loading.css('height'))
                        //								.find('span').text('下拉可刷新');
                        //						}, 1000);
                        //					}
                    }
                });
            }
        }

        $footer_loading.on("click",function () {
            if ($footer_loading.find('span').text() != '正在获取信息...') {
                var bnext = false;
                switch (sessionStorage.ComTypeSearch) {
                    case "2":
                        bnext = true;
                        sessionStorage.page++;
                        ComOrderList(1, sessionStorage.PriceState, 0);
                        break;
                    case "3":
                        bnext = true;
                        sessionStorage.page++;
                        ComOrderList(0, 0, 0);
                        break;
                    case "4":
                        bnext = true;
                        sessionStorage.page++;
                        ComOrderList(2, 0, 0);
                        break;
                }
                if (bnext == false) {
                    setDownListItem();
                }
            }
        });

        //向下设置列表分页数据
        function setDownListItem() {
            var w = $('#Commodity_nav').css('left');
            w = w.indexOf("%") > -1 ? parseInt(w.replace("%","")) /100 * $(window).width() : parseInt(w);
            if (Math.abs(w) + 5 < Math.round($(window).width() * 0.6)) {
                return 0;
            }
            window.ajax;
            if (!sessionStorage.categoryId && !sessionStorage.want) {
                sessionStorage.page == 1 ? sessionStorage.page++ : '';
                if (lock) {
                    if (window.ajax) {
                        window.ajax.abort();
                        sendAjax();
                    } else {
                        sendAjax();
                    }
                }
                function sendAjax() {
                    window.ajax = getCommodity({
                        appId: sessionStorage.appId,
                        pageIndex: sessionStorage.page,
                        pageSize: 10,
                        beforeSend: function () {
                            $footer_loading.find('span').text('正在获取信息...');
                        },
                        callback: function (data) {
                            lock = true;
                            var items = $('#items');
                            var item = $('#parent_item');
                            var itemsListMode = $('#itemsListMode')
                            var height = item.height();
                            var img_height = item.width() / item.find('img').width();
                            var clone_item;
                            //sessionStorage.commodityList =
							//	JSON.stringify(JSON.parse(sessionStorage.commodityList).concat(data));
                            _commodityListData = _commodityListData.concat(data);

                            setTimeout(function () {
                                if (data.length) {
                                    for (var i = 0; i < data.length; i++) {

                                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');

                                        var ghParam = {
                                            element: clone_item,
                                            data: data[i],
                                            height: height,
                                            img_height: img_height
                                        }

                                        items.append(setItemData(ghParam));

                                        var html = getCommodityItem_ListMode(ghParam);
                                        itemsListMode.append(html);
                                    }

                                    showLazyLoadImg();

                                    sessionStorage.page++;
                                    if(data.length >= 10)
                                    {
                                        $footer_loading.removeClass('clicks_lock').find('span').text('获取更多信息');
                                    }
                                    else
                                    {
                                         $footer_loading.removeClass('clicks_lock').find('span').text('没有更多商品');
                                    }
                                } else {
                                    $footer_loading.removeClass('clicks_lock').find('span').text('没有更多商品');
                                }
                            }, 500);
                        }
                    })
                }

                lock = false;

            } else if (sessionStorage.categoryId) {
                sessionStorage.categoryLock = true;
                getCommodityByCategory(sessionStorage.categoryId);
            } else if (sessionStorage.want) {
                sessionStorage.wantLock = true;
                getWantCommodityData(sessionStorage.want);
            }
        }

        /**
        * nav触摸开始事件
        * 设置初始值
        */
        nav.addEventListener('touchstart', function (e) {
            var touch = e.changedTouches[0];
            navScrollStart = touch.pageY;
        });

        /**
        * nav触摸移动事件.
        * 将浏览器默认事件重置.
        * 并模拟内部滚动.
        */
        nav.addEventListener('touchmove', function (e) {
            e.preventDefault();
            var touch = e.changedTouches[0];
            navMove = touch.pageY - navScrollStart;

            nav.scrollTop = navScrollTop - navMove;
        });

        /**
        * nav触摸结束时.将当前高度记录.供下次移动时使用.
        * 并设置全局touchLock锁.
        */
        nav.addEventListener('touchend', function (e) {
            navScrollTop = nav.scrollTop;
            touchLock = !navMove;
            navMove = 0;
        });

        $box.on('click', '.item_1,a.item', function (e) {
            if ($(e.target).is(".zcz")) {
                tanchucrowd();
            }
            else if($(e.target).is("span.shopping-cart"))
            { 
               _commodityItemRootElement = $(this);
                 DealLoginPartial.initPartialPage();
              
            }
            else {
                if (Math.abs(moveSizeY) < 10 && !lockX) {
                    ajaxLoading('22', '');
                    var self = $(this);
                    var box_2 = $('#box_2');
                    sessionStorage.commodityId_2 = self.data('id');
                    //				sessionStorage.commodityListScrollTop = document.body.scrollTop;
                    window.location.href = urlAppendCommonParams('/Mobile/CommodityDetail?commodityId=' + self.data('id') + '&type=show');
                }
            }
        });


    }

    /**
    * nav相关事件
    */
    function navEvent() {
        var parentUl;
        var parentUlClone = {};
        $('#Commodity_nav').on('click', '.nav_inner', function () {
            if (touchLock) {
                var self = $(this);
                parentUl = self.parents('ul');
                parentUlClone = parentUl.clone();
                var self_name = self.data('parent-name');
                var item_key = self.data('show-item');
                var one_li = '<li class="nav_one" data-category-id="{id}">{text}</li>';
                var two_li = '<li class="nav_two" data-category-id="{id}">{text}</li>';

                parentUl.empty();

                parentUl.append(one_li.replace('{text}', self_name + '<span>></span>' + self.text())
					.replace('{id}', self.data('category-id')));

                parentUlClone.find('.hidden').each(function () {
                    if ($(this).data('show-key') == item_key) {
                        parentUl.append(two_li.replace('{text}', $(this).text()).replace('{id}', $(this).data('category-id')));
                    }
                });
            }
        }).on('click', '.title', function () {
            window.itemIdList = {};
            if (parentUl) {
                var ul_parent = parentUl.parent();
                ul_parent.find('ul').remove();
                parentUlClone.find('li').removeClass('nav_focus');
                ul_parent.append(parentUlClone);
            } else {
                $(this).parent().find('li').removeClass('nav_focus');
            }
            var items = $('#items').empty();
            var itemsListMode = $('#itemsListMode').empty();
            _commodityListData = [];

            getCommodity({
                appId: sessionStorage.appId,
                pageIndex: 1,
                pageSize: 10,
                callback: function (data) {
                    sessionStorage.data = JSON.stringify(data);
                    sessionStorage.categoryId = '';
                    sessionStorage.categoryLock = '';
                    sessionStorage.want = '';
                    sessionStorage.wantLock = '';
                    //sessionStorage.commodityList = JSON.stringify(data);
                    _commodityListData = _commodityListData.concat(data);
                    sessionStorage.page = 1;
                    var item = $('#parent_item');
                    var height = item.height();
                    var img_height = item.find('img').height();
                    var clone_item;

                    for (var i = 0; i < data.length; i++) {
                        clone_item = item.clone().removeClass('noDisplay').attr('id', '');
                        items.append(setItemData({ element: clone_item, data: data[i], height: height, img_height: img_height }));
                    }

                    showLazyLoadImg();

                    if (data.length >= 10)
                        $('#footer_loading').show().find('span').text('获取更多信息');
                    parentUl = '';
                }
            });


        }).on('click', 'li', function () {
            if (touchLock) {
                var self = $(this);
                var parent = self.parent();
                if (!self.hasClass('no_nav_focus')) {
                    parent.find('li').removeClass('nav_focus');
                    if (!self.hasClass('no_nav_focus')) {
                        self.addClass('nav_focus');
                    }

                    if (!self.hasClass('nav_inner')) {
                        sessionStorage.page = 1;
                        sessionStorage.ComTypeSearch = 0;
                        sessionStorage.categoryLock = '';
                        getCommodityByCategory(self.data('category-id'));
                    }
                }
            }
        }).on('keydown', 'input', function (e) {
            var self = $(this);
            if (e.keyCode == 13) {
                sessionStorage.page = 1;
                sessionStorage.categoryLock = '';
                getWantCommodityData(self.val());
                self[0].blur();
            }
        });

        $('#nav_search_click').on("click",function () {
            sessionStorage.ComTypeSearch = 0;
            sessionStorage.page = 1;
            getWantCommodityData($('#Commodity_nav').find('input').val());
        })
    }
}

function showLazyLoadImg() {
    //	$('.LazyLoad').each(function () {
    //		var self = $(this);
    //		self.attr('src', self.attr('data-src')).removeClass('LazyLoad')[0].onerror = function () {
    //			console.log(self.css({background: 'none'}).attr('src', '/Content/Mobile/b_2.png'));
    //		};
    //	});
}



function setSessionStorageItemHeightAndImgHeight() {
    var imgs = document.createElement('img');
    imgs.src = '/Content/Mobile/1.png?' + new Date().getTime();
    imgs.onload = function () {
        var parent_item = $('#parent_item').clone().show();
        $('body').append(parent_item);
        sessionStorage.img_height = Math.round(parent_item.width() / this.width * this.height);
        parent_item.find('.position_r').append(imgs);
        sessionStorage.item_height = parent_item.height() - 15;
        parent_item.remove();
    };
}

//获取订单列表 ajax
function getOrder(obj) { 
   
    var data = {
        userId: obj.userId,
        pageIndex: obj.pageIndex,
        pageSize: obj.pageSize,
        state:obj.orderState,
        esAppId:getEsAppId()
        
    };
    return getDataAjax({
        url: '/Mobile/GetOrder',
        data: data,
        beforeSend: obj.beforeSend,
        callback: obj.callback
    });
}

//生成整个订单列表html.
function getOrderListHtml(data)
{
    if(data == null || data.length == 0)
    {
        return "";
    }
    var len = data.length;
    var allHtml = "";
    for(var i = 0 ;i < len; i++)
    {
        allHtml += getOrderItemHtml(data[i]);
    }
    return allHtml;
}
//生成单个订单列表html.
  function getOrderItemHtml(data) {
    var ciHtml = "";

    if (data.ShoppingCartItemSDTO && data.ShoppingCartItemSDTO.length > 0) {
    var len = data.ShoppingCartItemSDTO.length;
    var reviewLength = 0;
    for (var i = 0; i < len; i++) {
            var cartItem = data.ShoppingCartItemSDTO[i];
           //看规规格的参数判断显示和隐藏
            if(Number(cartItem.Specifications)>0){
                cartItem.CommodityNumber=Number(cartItem.CommodityNumber/cartItem.Specifications);
                cartItem.Specifications=String("包装规格: 1*"+cartItem.Specifications+"");
            }
            else {
                cartItem.Specifications="";
            }
            cartItem.end = (i == len - 1) ? "end":"";
            cartItem.SelfTakeDisplay = data.SelfTakeFlag == 1 ? "block":"none";
            cartItem.srckey = "src";
            ciHtml += commodityItemTemplate.format(cartItem);
            if(cartItem.HasReview)
            {
                reviewLength++;
            }
        }
    } 
    //订单中有一个已评价，则整个订单为已评价。
    var isReview = reviewLength > 0 ? true:false;
    data.CommodityCount = len;
    data.CommodityItemsHtml = ciHtml;
    data.OperateButtonsHtml = getOneOrderButtonHtml(data,isReview);
    data.StateText = getOrderStateText(data.State,data.StateAfterSales);
    data.AppName = data.AppName == undefined ? "":data.AppName;
     
    var html = shopItemTemplate.format(data); 
    return html;
 }

 //获取售后状态显示文本。
function getAfterSaleState(afterSaleState)
{
     //售后-订单状态（必填）：确认收货=3，售后退款中=5,已退款=7，商家未收到货=10 ,金和处理退款中=12,售后交易成功=15
     var text = "";
     if(afterSaleState == 3)
     {
        text = "交易成功";
     }
     else if(afterSaleState == 5)
     {
          text = "退款中";
     }
     else if(afterSaleState == 7)
     {
        text = "已退款";
     }
     else if(afterSaleState == 10)
     {
         text = "退款中";
     }
     else if(afterSaleState == 12)
     {
        text = "金和处理退款中";
     }
     else if(afterSaleState == 15)
     {
         text = "交易成功";
     }
     return text;

}
//获取状态的文本内容。
function getOrderStateText(state,afterSaleState)
{  
    //最新请参见Model.CommodityOrder.
    //订单状态（必填）：未支付=0，未发货=1，已发货=2，确认收货=3，商家取消订单=4，客户取消订单=5，超时交易关闭=6，已退款=7，待发货退款中=8，已发货退款中=9,已发货退款中商家同意退款，商家未收到货=10,付款中=11,金和处理退款中=12,出库中=13，出库中退款中=14
    var text = "";
    switch (state) {
        case 0:
            text = "待付款";
            break;
        case 1:
            text = "待发货";
            break;
        case 13:
            text = "出库中";
            break;
        case 2:
            text = "已发货";
            break;
        case 3:
            text = "交易成功";
            //已开始售后，显示售后状态。
            if(JsVilaDataNull(afterSaleState))
            {
                var asText = getAfterSaleState(afterSaleState);
                if(JsVilaDataNull(asText))
                {
                    text = asText;
                }
            }
            break;
        case 4:
            text = "交易失败";
            break;
        case 5:
            text = "交易失败";
            break;
        case 6:
            text = "交易关闭";
            break;
        case 7:
            text = "已退款";
            break;
        case 8: 
        case 9:
        case 10:
        case 14:
            text = "退款中";
            break;
        case 11:
            text = "待发货";
            break;
        case 12:
                text = "金和处理退款中";
            break;
        default:
            text = "";
            break;
    }
    return text;
}

//根据订单的当前状态显示可操作按钮列表。
function getButtonByState(data,state,pay,orderItemState)
{
     //最新请参见Model.CommodityOrder.
    //订单状态（必填）：未支付=0，未发货=1，已发货=2，确认收货=3，商家取消订单=4，客户取消订单=5，超时交易关闭=6，已退款=7，待发货退款中=8，已发货退款中=9,已发货退款中商家同意退款，商家未收到货=10,付款中=11,金和处理退款中=12,出库中=13，出库中退款中=14
        //永远不会“货到付款”
     //售后-订单状态（必填）：确认收货=3，售后退款中=5,已退款=7，商家未收到货=10 ,金和处理退款中=12,售后交易成功=15
     var btnArr = new Array();
    //0、金币支付;1、货到付款;2、支付宝;3、支付宝担保交易;4、U付
    //可显示的操作按钮：0、支付;1、取消订单;2、退款;3、申请退款/退货;4、确认收货;5、撤销退款申请;6、撤销退款/退货申请;7、评价;8、查看退款详情; 9、删除; 10、退货方式；11、延长收货时间(3天) 12、查看退款方式
    //23、售后-申请退款/退货;24、售后-撤销退款/退货申请; 25、售后-查看退款详情； 26、售后-退货方式；  27、售后-查看退货方式；
    //30、撤销退款/退货申请 31、退款详情 32、退货方式 33、查看退货方式
     switch (state) {
        //=================当前"待付款",显示"支付","取消订单"
        case 0:
            btnArr.push(0);
            btnArr.push(1);
            break;
            //================当前"待付款",显示""
        case 1:
            if (pay != 1) {
                    //退款
                     btnArr.push(2);
             }
            else{
                //取消订单
                btnArr.push(1);
            }
            //商品自提，显示“确认收货”。
            if(data.SelfTakeFlag == 1)
            {
                btnArr.push(4);
            }
            //单品退款中
            if (orderItemState == 1) {
                btnArr.push(30);
            }
            break;
            //============当前"出库中",显示""
        case 13:
            //确认收货
            btnArr.push(4);
            if (pay != 1) {
                    //申请退款/退货
                    btnArr.push(3);
            }
            break;
            //=============当前"已发货" 
        case 2:
            //确认收货
             btnArr.push(4);
           if (pay != 1) {
                    //申请退款/退货
                    btnArr.push(3);
            }

              //转换时间格式 为了计算日期时间差
            var myDate = new Date();
            var month = myDate.getMonth() + 1 < 10 ? "0" + (myDate.getMonth() + 1) : myDate.getMonth() + 1;
            var currentDate = myDate.getDate() < 10 ? "0" + myDate.getDate() : myDate.getDate();
            var eDate = myDate.getFullYear() + "-" + month + "-" + currentDate;

            //计算时间差
            var sDate = ChangeDateFormat(data.ShipmentsTime, 2);
            var sArr = sDate.split("-");
            var eArr = eDate.split("-");
            var sRDate = new Date(sArr[0], sArr[1], sArr[2]);
            var eRDate = new Date(eArr[0], eArr[1], eArr[2]);
            var result = (eRDate - sRDate) / (24 * 60 * 60 * 1000) + 1;
            if (result >= 6 && result <= 9 && !data.IsDelayConfirmTime) {
                  btnArr.push(11);
            }

            //单品退货 
            if (orderItemState === 1) {
                btnArr.push(30);
            }
            //单品退货 达成协议
            if (orderItemState === 3) {
                btnArr.push(30);
                btnArr.push(32);  
            }
            break;
            //=============当前"交易成功" 
        case 3:
            //售后-订单状态（必填）：确认收货=3，售后退款中=5,已退款=7，已发货退款中商家同意退款，商家未收到货=10 ,金和处理退款中=12 ,买家发货超时，商家未收到货=13,售后交易成功=15
            //单品退货 
            if (orderItemState === 1 || orderItemState === 3) {
                btnArr.push(30);
            }
            if(data.StateAfterSales == 3)
            {
                //评价
                btnArr.push(7);
                //单品退货 达成协议
                if (orderItemState === 3) {
                    btnArr.push(31);
                    btnArr.push(32);
                } else {
                    if (data.OrderType == 0 || data.OrderType == 1 || data.OrderType == 3) {
                        btnArr.push(23);
                    }
                }
            }
            else  if(data.StateAfterSales == 5)
            {
                //单品退货 达成协议
                if (orderItemState === 3) {
                    btnArr.push(32);
                }
                //评价
                btnArr.push(24);
                btnArr.push(25);
            }
            else  if(data.StateAfterSales == 7)
            {
//                //评价
//                btnArr.push(7);
              btnArr.push(25);
              //退款退货订单，且有退货物流单号，显示物流详情。
              if(data.RefundType == 1 && JsVilaDataNull(data.RefundExpCo))
              {
                    btnArr.push(27);
              }
            }
            else  if(data.StateAfterSales == 10)
            { 
//                //评价
//                btnArr.push(7);
                if (JsVilaDataNull(data.RefundExpCo)) {
                    //已发货，不能再撤销退款\退货
                    btnArr.push(27);
                 }
                 else {
                    btnArr.push(24);
                    btnArr.push(26);
                 }
                 btnArr.push(25);
            }
            else  if(data.StateAfterSales == 12)
            {
//                //评价
//                btnArr.push(7);
            }
            //7天后商家还未收到买家的货，不在显示“退货方式”
            else if(data.StateAfterSales == 13)
            {
                //评价
                btnArr.push(7);
            }
            else  if(data.StateAfterSales == 15)
            {
                //评价
                btnArr.push(7);
            }
            break;
            //=============当前"交易失败" 
        case 4:
             btnArr.push(9);
            break;
            //==============当前"交易失败" 
        case 5:
            btnArr.push(9);
            break;
            //==============当前"交易关闭" 
        case 6:
            btnArr.push(9);
            break;
            //==============当前"已退款" 
        case 7:
            if(data.SelfTakeFlag != 1)
            {
                btnArr.push(31);
                   //退款退货订单，且有退货物流单号，显示物流详情。
              if(data.RefundType == 1 && JsVilaDataNull(data.RefundExpCo))
              {
                    btnArr.push(33);
              }
            }
            break;
            //==============当前"退款中" 
        case 8:
                //撤销退款申请
                btnArr.push(30);
                //查看退款详情
                btnArr.push(31);
            break;
            //===============当前"退款中" 
        case 9:
                //撤销退款申请
                btnArr.push(30);
                //查看退款详情
                btnArr.push(31);
                if (orderItemState == 3) {
                    btnArr.push(32);
                }
            break;
        case 14:
                btnArr.push(30);
                 //查看退款详情
                btnArr.push(31);
            break;
            //==============当前"退款中" 
        case 10:
                 //查看退款详情
                 btnArr.push(31);
                 if (JsVilaDataNull(data.RefundExpCo)) {
                    btnArr.push(33);
                 }
                 else {
                      if(data.OrderRefundState == 11){
                          //买已发货时，不需要显示撤销退款申请与退货方式
                      }
                      else{
                         //撤销退款申请
                           btnArr.push(30);
                           //退货方式
                           btnArr.push(32);
                      }
                 }
            break;
            //==============当前"待发货" 
        case 11:
                btnArr.push(2);
            break;
            //==============当前"金和处理退款中"
        case 12:
            break;
        default:
            break;
    }
     return btnArr;
}
////根据订单的当前状态显示可操作按钮列表。
//function getButtonByState(state)
//{
//        //永远不会“货到付款”
//     var btnArr = new Array();
//    //0、金币支付;1、货到付款;2、支付宝;3、支付宝担保交易;4、U付
//    //可显示的操作按钮：0、支付;1、取消订单;2、退款;3、申请退款/退货;4、确认收货;5、撤销退款申请;6、撤销退款/退货申请;
//     switch (state) {
//        //=================当前"待付款",显示"支付","取消订单"
//        case 0:
//            btnArr.push(0);
//            btnArr.push(1);
//            break;
//            //================当前"待付款",显示""
//        case 1:
//             //退款
//             btnArr.push(2);
//            break;
//            //============当前"出库中",显示""
//        case 13:
//            //确认收货
//            btnArr.push(4);
//            //申请退款/退货
//            btnArr.push(3);
//            break;
//            //=============当前"已发货" 
//        case 2:
//        //确认收货
//        btnArr.push();
//            //确认收货
//             btnArr.push(4);
//            //申请退款/退货
//            btnArr.push(3);
//            break;
//            //=============当前"交易成功" 
//        case 3:
//            break;
//            //=============当前"交易失败" 
//        case 4:
//            break;
//            //==============当前"交易失败" 
//        case 5:
//            break;
//            //==============当前"交易关闭" 
//        case 6:
//            break;
//            //==============当前"已退款" 
//        case 7:
//            break;
//            //==============当前"退款中" 
//        case 8:
//        //撤销退款申请
//            btnArr.push(5);
//            break;
//            //===============当前"退款中" 
//        case 9:
//        case 14:
//            btnArr.push(6);
//            break;
//            //==============当前"退款中" 
//        case 10:
//            break;
//            //==============当前"待发货" 
//        case 11:
//            btnArr.push(2);
//            break;
//            //==============当前"金和处理退款中"
//        case 12:
//            break;
//        default:
//            break;
//    }
//     return btnArr;
//}

//生成一个订单所能显示的button按钮的html.
function getOneOrderButtonHtml(data,isReview)
{ 
    var btnArr = getButtonByState(data,data.State,data.PayType);
    if(btnArr.length == 0)
    {
        return "";
    }
     //如果已评价，则不再评价。
    if (isReview) {
        var ind = btnArr.indexOf(7);
        if (ind > -1) {
            btnArr.splice(ind, 1);
        }
    } 
    //如果是服务订单，则也不需要评价
     if (data.OrderType == 1) {
         //服务订单不需要评价，如果数据有问题也不需要显示已评价
        isReview = false;
        var ind = btnArr.indexOf(7);
        if (ind > -1) {
            btnArr.splice(ind, 1);
        }
    } 
    //列表里如果有（;2、退款;3、申请退款/退货; 9、删除;10、退货方式；12、查看退款方式 23、售后-申请退款/退货; 26、售后-退货方式； 27、售后-查看退货方式；）操作按钮不显示。
    var ind01 = btnArr.indexOf(2);
    if (ind01 > -1) {
        btnArr.splice(ind01, 1);
    }
    var ind02 = btnArr.indexOf(3);
    if (ind02 > -1) {
        btnArr.splice(ind02, 1);
    }

    var ind03 = btnArr.indexOf(9);
    if (ind03 > -1) {
        btnArr.splice(ind03, 1);
    }
    var ind04 = btnArr.indexOf(10);
    if (ind04 > -1) {
        btnArr.splice(ind04, 1);
    }
    var ind05 = btnArr.indexOf(12);
    if (ind05 > -1) {
        btnArr.splice(ind05, 1);
    }

    var ind06 = btnArr.indexOf(23);
    if (ind06 > -1) {
        btnArr.splice(ind06, 1);
    }
     var ind07 = btnArr.indexOf(26);
    if (ind07 > -1) {
        btnArr.splice(ind07, 1);
    }
     var ind08 = btnArr.indexOf(27);
    if (ind08 > -1) {
        btnArr.splice(ind08, 1);
    }

     var ind09 = btnArr.indexOf(24);
    if (ind09 > -1) {
        btnArr.splice(ind09, 1);
    }
    
     
    if (btnArr.length == 0) {
        return "";
    }
    var btnHtml = "";
    for(var i = 0 ; i < btnArr.length ;i++)
    {
        btnHtml += getOneButtonHtml(btnArr[i],data);
    }
    return btnHtml;
}
//生成一个操作按钮的html.
function getOneButtonHtml(btnType,data)
{
      //可显示的操作按钮：0、支付;1、取消订单;2、退款;3、申请退款/退货;4、确认收货;5、撤销退款申请;6、撤销退款/退货申请;7、评价;8、查看退款详情; 9、删除; 10、退货方式；11、延长收货时间(3天) 12、查看退款方式
    //23、售后-申请退款/退货;24、售后-撤销退款/退货申请; 25、售后-查看退款详情； 26、售后-退货方式；  27、售后-查看退货方式；

    var btnText = "";
    switch(btnType)
    {
        case 0:
        btnText = "付款";
        break;
         case 1:
        btnText = "取消订单";
        break;
         case 2:
        btnText = "退款";
        break;
         case 3:
        btnText = "申请退款/退货";
        break;
         case 4:
        btnText = "确认收货";
        break;
         case 5:
        btnText = "撤销退款申请";
        break;
        case 6:
        btnText = "撤销退款/退货申请";
        break;
        case 7:
        btnText = "评价";
        break;
        case 8:
            btnText = "查看退款详情";
            break;
        case 9:
            btnText = "删除";
            break;
        case 10:
            btnText = "退货方式";
            break;
        case 11:
            btnText = "延长收货时间(3天)";
            break;
        case 12:
            btnText = "查看退货方式";
            break; 
        case 23:
             btnText = "申请退款/退货";
            break;
        case 24:
              btnText = "撤销退款/退货申请";
            break;
        case 25:
            btnText = "查看退款详情";
            break;
        case 26:
            btnText = "退货方式";
            break;
        case 27:
            btnText = "查看退货方式";
            break; 
        case 30:
            btnText = "撤销退款/退货申请";
            break; 
        case 31:
            btnText = "退款详情";
            break; 
        case 32:
            btnText = "退货方式";
            break; 
        case 33:
            btnText = "查看退货方式";
            break; 

    //30、撤销退款/退货申请 31、退款详情 32、退货方式 33、查看退货方式

    }
   
   
    var fparam = new Object();
    fparam.btnText = btnText;
    fparam.orderId = data.CommodityOrderId;
    fparam.otag = btnType;
    fparam.appId = data.AppId;
    fparam.btnPrimary = btnType == 0 ? "btn-primary" : "";

    var btnTempHtml = '<a href="javascript:void(0)" class="button {btnPrimary}" orderId="{orderId}" otag="{otag}" appId="{appId}" >{btnText}</a>';
    btnTempHtml = btnTempHtml.format(fparam);
   return btnTempHtml;
}

//注册每个(所有)订单的点击事件。
function registOrderEvent() {
    $('div[ctag="shopItem"]').off("click", "**");
    $('div[ctag="shopItem"]').on("click", function(e) {
        var orderId = $(e.target).attr("orderId");
        orderId = orderId == null ? $(e.currentTarget).attr("orderId") : orderId;
        var appId = $(e.target).attr("appId");
        appId = appId == null ? $(e.currentTarget).attr("appId") : appId;
        var url = urlAppendCommonParams('MyOrderDetail?orderId=' + orderId + '&shopId=' + appId);
        var sessionId = null;
        if(typeof getSessionId == "function") sessionId = getSessionId();
        if(!sessionId && typeof getQueryString == "function") sessionId = getQueryString("sessionId");
        if(sessionId) url += "&sessionId=" + sessionId
        var orderState = getQueryString("orderState");
        if (JsVilaDataNull(orderState)) {
            url += "&orderState=" + orderState;
        }
        window.location.href = url;
    });
}

//订单项模板。
var shopItemTemplate = "";
var commodityItemTemplate = "";
$(function () {
    shopItemTemplate = $("#shopItemTemplate").html();
    commodityItemTemplate = $("#commodityItemTemplate").html();
});
//商品列表中设置各个商品相关属性
function setOrderItemData(obj) {
    var clone_item = obj.element;
    var clone_orderItem = obj.ordeItem;
    var data = obj.data;
    //if (window.itemIdList) {
    //    if (window.itemIdList[data.Id]) {
    //        return '';
    //    }
    //    window.itemIdList[data.Id] = 'on'
    //} else {
    //    window.itemIdList = {};
    //    window.itemIdList[data.Id] = 'on'
    //}
    clone_item.css({
        //		height: obj.height
    }).attr('data-id', data.CommodityOrderId);
    //	clone_item.find('a')[0].href += '?id=' + data.Id;
    //clone_item.find('img').parent().css({'height': Math.round(sessionStorage.img_height) + 'px','overflow': 'hidden'}).
    //    find('img').css('height', Math.round(sessionStorage.img_height) + 'px').attr('src', data.Pic)
    //    .addClass('LazyLoad')[0].onerror = function () {
    //    console.log(self.css({background: 'none'}).attr('src', '/Content/Mobile/b_2.png'));
    //};
    clone_item.find('a').attr('href', 'MyOrderDetail?orderId=' + data.CommodityOrderId + '&shopId=' + data.AppId);


    if (data.ShoppingCartItemSDTO && data.ShoppingCartItemSDTO.length) {
        var html = "";
        for (var i = 0; i < data.ShoppingCartItemSDTO.length; i++) {

            var cartItem = data.ShoppingCartItemSDTO[i];

            if (i == 0) {
                html += "<li>";
            }
            html += "<div style=\"margin-top:5px;\"><img src='" + cartItem.Pic + "' alt=\"\" width=\"60\" height=\"60\">";
            html += " <div class=\"title\">" + cartItem.Name + "</div></div><div style=\"clear:both;\"></div>";
            if (i == data.ShoppingCartItemSDTO.length - 1) {
                if (data.State == 0) {
                    html += "<span  class=\"con\">待付款</span>";
                    // clone_item.find('.con').text("待付款");
                }
                else if (data.State == 1) {
                    html += "<span  class=\"con\">待发货</span>";
                    //clone_item.find('.con').text("待发货");
                }
                else if (data.State == 2) {
                    html += "<span  class=\"con\">已发货</span>";
                    //clone_item.find('.con').text("已发货");
                }
                else if (data.State == 3) {
                    html += "<span  class=\"con\">交易成功</span>";
                    //clone_item.find('.con').text("交易成功");
                }
                else if (data.State == 4) {
                    html += "<span  class=\"con\">交易失败</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 5) {
                    html += "<span  class=\"con\">交易失败</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 6) {
                    html += "<span  class=\"con\">交易关闭</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 7) {
                    html += "<span  class=\"con\">已退款</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 8) {
                    html += "<span  class=\"con\">退款中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 9 || data.State == 14) {
                    html += "<span  class=\"con\">退款中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 10) {
                    html += "<span  class=\"con\">退款中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 11) {
                    html += "<span  class=\"con\">待发货</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 12) {
                    html += "<span  class=\"con\">金和处理退款中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                else if (data.State == 13) {
                    html += "<span  class=\"con\">出库中</span>";
                    //clone_item.find('.con').text("交易失败");
                }
                html += "</li>";
            }


        }
        clone_item.find('.list').prepend(html)
    }


    return clone_item;
}


function getOrderNew(fn) {
    var orderState =JsVilaDataNull(getQueryString('orderState')) ? getQueryString('orderState') : null;
    getOrder({
        userId: getUserId(),
        pageIndex: sessionStorage.page,
        pageSize: 10,
        orderState: orderState,
        callback: function (data) {
            $("#ajaxLoadBlind").remove();
            sessionStorage.OrderState = getQueryString('orderState');

            ////没有订单。
            //if (sessionStorage.page == 1
            //&& data.length <= 0) {
            //    var h = ($(window).height() / 2) - 50;
            //    $("#main").append("<div style=\"color: #d7d7d7;margin: auto;text-align: center;margin-top: " + h + "px;font-size: 1rem;\">暂无订单<div>");
            //    $('#noOrder').show();
            //     return;
            // }

            $('#noOrder').hide();
            var items = $('#orderItems');

            //加载第一页时清除历史数据。
            if (sessionStorage.page == 1) {
                items.empty();
                sessionStorage.orderList = false;
            }

            //将订单数据缓存起来。
            var prevlocalData = JSON.parse(sessionStorage.orderList);
            if (prevlocalData == false
            || prevlocalData.length == 0) { 
                sessionStorage.orderList = JSON.stringify(data);
            } else {
                sessionStorage.orderList = JSON.stringify(prevlocalData.concat(data));
            }
           

            //用加载的数据生成订单html.
            var html = getOrderListHtml(data);
            items.append(html);
            //注册每个订单项点击事件。
            registOrderEvent();

            showLazyLoadImg();

            if (sessionStorage.page == 1) {
                var $top_loading = $('#top_loading');
                $top_loading.find('span').text('已更新');
                setTimeout(function () {
                    $top_loading.css('margin-top', '-' + $top_loading.css('height'))
                            .find('span').text('下拉可刷新');
                }, 1000);
            }

            if (data.length >= 10) {
                $('#footer_loading').show().find('span').text('正在获取数据...');
            }
            else {
                $('#footer_loading').show().find('span').text('暂无更多订单数据!');
            }

            isOrderDataEnd = data.length < 10;
            //topLoadingLock = false;

            fn && fn();
        }
    });
}


var isOrderDataEnd = false;
function OrderList() {
    $(function () {
        sessionStorage.page = 1;
        if (getQueryString('type') == "shuaxin") {
            sessionStorage.orderList = false;
        }
        if (sessionStorage.OrderState != getQueryString('orderState')) {
            sessionStorage.orderList = false;
        }
        if (sessionStorage.orderList && sessionStorage.orderList != "false") {
            var data = JSON.parse(sessionStorage.orderList);
            if (data && data.length) {
                (function () {
                    var items = $('#orderItems');
                    sessionStorage.page =data.length % 10 == 0 ? parseInt(data.length / 10): parseInt(data.length / 10) + 1;
                    var html = getOrderListHtml(data);
                    items.append(html);
                    registOrderEvent();

                    showLazyLoadImg();
                    if (data.length >= 10)
                    {
                        $('#footer_loading').show().find('span').text('正在获取数据...');
                    }
                    else
                    {
                        if(data.length>0)
                         $('#footer_loading').show().find('span').text('暂无更多订单数据!');
                    }
                    isOrderDataEnd = data.length < 10;
                })();
            } else {
                getOrderNew(function(){});
            }
        } else {
            getOrderNew(function(){});
        } 
    });
}

function round2(number, fractionDigits) {
    with (Math) {
        return round(number * pow(10, fractionDigits)) / pow(10, fractionDigits);
    }
}

function JsCrowdfund() {
    return;
    getDataAjax({
        url: "/Mobile/GetUserCrowdfundingBuy",
        data: { userId: getUserId(), appId: sessionStorage.appId },
        callback: function (data) {
            $("#ajaxLoadBlind").remove();
            showCrowdfund(data);
        },
        beforeSend: function () {
            ajaxLoading('22', '');
        },
        error: function () {
            $("#ajaxLoadBlind").remove();
        }
    });
}

//显示众销信息。
function showCrowdfund(data)
{
    if(data == null)
    {
        return;
    }
    //众筹每股金额
    var perShareMoney = data.PerShareMoney;
    //用户已购买金额
    var money = data.Money;
    //用户已持有股数
    var currentShareCount = data.CurrentShareCount;
    //是否是进行中的众筹 true为活动中 
    var isActiveCrowdfunding = data.IsActiveCrowdfunding;
    //众筹可购买股数
    var shareCountRemain = data.ShareCountRemain;
    //用户将要够买的金额
    var usergou = $(".span_4").html();

    if (isActiveCrowdfunding == "true" || isActiveCrowdfunding) {
        $(".crowli").show();
        //如果用户将要够买 产生的股点数超过了此App剩余的股点数
        if ((usergou / perShareMoney) >= shareCountRemain) {
            $(".crowinfor").html("您已拥有全部剩余股点" + shareCountRemain + "股！");

        }
        else {
            //向上取整,有小数就整数部分加1
            var jianggu = parseInt(usergou / perShareMoney) + 1;
            var chaprice = ((perShareMoney * jianggu) - usergou).toFixed(2);
            $(".crowinfor").html("您再购买" + chaprice + "元商品就能获得" + jianggu + "股，坐等分红啦！");
        }
    }
}

//var _hmt = _hmt || [];
////百度埋码
//(function () {
//    var htmpro = (("https:" == document.location.protocol) ? "https://" : "http://");
//    var hj = "", url = document.location.href;
//    if (url.indexOf(htmpro + "dev") >= 0) {
//        return;
//    }
//    else if (url.indexOf(htmpro + "test") >= 0) {
//        return;
//    }
//    else if (url.indexOf(htmpro + "pre") >= 0) {
//        return;
//    }
//    else if (url.indexOf("localhost") >= 0) {
//        return;
//    }
//    var hm = document.createElement("script");
//    hm.src = "//hm.baidu.com/hm.js?b2605c4e1512bf83f1abadd4ba614c70";
//    var s = document.getElementsByTagName("script")[0];
//    s.parentNode.insertBefore(hm, s);
//})();



function timer(timestamp, callback) {
    $(".time-item").show();
    var day = 0,
        hour = 0,
        minute = 0,
        second = 0; //时间默认值        
    var intDiff = parseInt((sessionStorage.startSecKillTime.split("(")[1].split(")")[0] - (new Date()).getTime()) / 1000);
    if (intDiff >= 0) {
        day = Math.floor(intDiff / (60 * 60 * 24));
        hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
        minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
        second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
        if (minute <= 9) minute = '0' + minute;
        if (second <= 9) second = '0' + second;
        //        $('#hour_show').html('<s id="h"></s>' + hour);
        $('#minute_show').html('<s></s>' + minute);
        $('#second_show').html('<s></s>' + second);
    } else {
        $('#minute_show').html('<s></s>' + "00");
        $('#second_show').html('<s></s>' + "00");
        return;
    }
    if (!intDiff) {
        callback();
        return;
    }


    window.setInterval(function () {
        intDiff = parseInt((sessionStorage.startSecKillTime.split("(")[1].split(")")[0] - (new Date()).getTime()) / 1000);
        if (intDiff >= 0) {
            day = Math.floor(intDiff / (60 * 60 * 24));
            hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
            minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
            second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
            if (minute <= 9) minute = '0' + minute;
            if (second <= 9) second = '0' + second;
            //        $('#hour_show').html('<s id="h"></s>' + hour);
            $('#minute_show').html('<s></s>' + minute);
            $('#second_show').html('<s></s>' + second);
            if (!intDiff) {
                callback();
                return;
            }
        }
        else {
            $('#minute_show').html('<s></s>' + "00");
            $('#second_show').html('<s></s>' + "00");
            return;
        }




    }, 1000);

}
 
var iosShareData = '';
var iosShareDataNew = '';

function shareAndroid(content, title, imgUrl, url, sourceType, shareType) {
    var startWebviewVersion = "1.0.0";
    if (!shareType)
        shareType = 0;
    try {
        var shareurl = "";
        var shareSquareUrl = "";
        url = url.toLowerCase();

        url = url.replace(/&userid=[^&]*/g, "");
        url = url.replace(/\?userid=[^&]*&/g, "\?");
        url = url.replace(/&sessionid=[^&]*/g, "");
        url = url.replace(/\?sessionid=[^&]*&/g, "\?");
        url = url.replace(/&srcappid=[^&]*/g, "");
        url = url.replace(/\?srcappid=[^&]*&/g, "\?");
        url = url.replace(/\?srctype=[^&]*&/g, "\?");
        url = url.replace(/&srctype=[^&]*/g, "");
        url = url.replace(/\?srctagid=[^&]*&/g, "\?");
        url = url.replace(/&srctagid=[^&]*/g, "");
        url = url.replace(/&user=[^&]*/g, "");
        url = url.replace(/\?user=[^&]*&/g, "\?");
        url = url.replace(/&speader=[^&]*/g, "");
        url = url.replace(/\?speader=[^&]*&/g, "\?");

        url = url.replace("&source=share&", "&").replace("?source=share&", "?").replace("&source=share", "").replace("?source=share", "").replace('?&', '?');

        var tmpIndex = url.indexOf("?");
        if (tmpIndex < 0) {
            shareurl = url + "?source=share";
            shareSquareUrl = url;
        } else if (tmpIndex == url.length - 1) {
            shareurl = url + "source=share";
            shareSquareUrl = url.substr(0, url.length - 1);
        } else {
            shareurl = url + "&source=share";
            shareSquareUrl = url;
        }
        //原应用Id.
        if (JsVilaDataNull(sessionStorage.srcAppId)) {
            var appIdUrl = "srcAppId=" + sessionStorage.srcAppId;
            shareurl += "&" + appIdUrl;
        }
        //推广主id.
        if (url.indexOf("speader") < 0 && JsVilaDataNull(sessionStorage.speader)) {
            shareurl += "&speader=" + sessionStorage.speader;
        }
        iosShareData = '{\"shareurl\":\"' + shareurl + '",\"shareSquareUrl\":"' + shareSquareUrl + '",\"content\":"' + (content ? content : '') + '",\"title\":"' + (title ? title : '') + '",\"imgUrl\":"' + (imgUrl ? imgUrl : '') + '",\"sourceType\":"' + (sourceType ? sourceType : '') + '"}';
        iosShareDataNew = '{\"shareurl\":\"' + shareurl + '",\"shareSquareUrl\":"' + shareSquareUrl + '",\"content\":"' + (content ? content : '') + '",\"title\":"' + (title ? title : '') + '",\"imgUrl\":"' + (imgUrl ? imgUrl : '') + '",\"sourceType\":"' + (sourceType ? sourceType : '') + '",\"shareType\":"' + shareType + '"}';
        if (isNewerJhWebview(startWebviewVersion)) {
            var base64 = new Base64();
            var para = "{\"businessJson\":\"{\\\"shareUrl\\\":\\\"" + shareurl + "\\\",\\\"shareSquareUrl\\\":\\\"" + shareSquareUrl + "\\\",\\\"shareContent\\\":\\\"" + content + "\\\",\\\"imgUrl\\\":\\\"" + imgUrl + "\\\",\\\"message\\\":null,\\\"actionName\\\":null,\\\"shareTitle\\\":\\\"" + title + "\\\",\\\"sourceType\\\":8,\\\"shareType\\\":" + shareType + "}\",\"businessType\":0}";
            if (!$.os.ios) {
                window.location.href = "jhoabrowser://integralTypeUrl?args=" + base64.encode(para) + "&tag=" + base64.encode(Math.random().toString());    
            }
        } else {
            window.appshare.getShareInfo(shareurl, shareSquareUrl, content, title, imgUrl, null, null, sourceType);
        }
    } catch(e) {
    }
}

function shareIOS() {
    return iosShareData;
}
function shareIOSNew() {
    return iosShareDataNew;
}

 
//获取vip优惠信息。
function getVipPrice() {
    if (isComHasVipInfo) {
        return;
    }
    getDataAjax({
        url: '/Mobile/GetVipInfo',
        type: 'get',
        contentType: "application/json",
            dataType: 'json',
        async:false,
        data: {appId:sessionStorage.appId},
        callback: function(data) {
            ajaxLoadingSingle.hide();
            if (!data || data.ResultCode != "0") {
                toast(data.Message);
                return;
            }
            showVipPriceLogin(data.Data);
        },
        beforeSend:function(){
            ajaxLoadingSingle.show();
        },
        error: function() {
            ajaxLoadingSingle.hide();
        }
    });
}

//显示vip优惠信息。
function showVipPriceLogin(vipData) {
    if (!vipData || !vipData.IsVipActive) {
         $("#divVipMember").hide();
        return;
    }
    $("#divVipMember").show();
    var commodityInfo = JSON.parse(sessionStorage.commodityInfo);
    commodityInfo.VipPromotion = vipData;
    sessionStorage.commodityInfo = JSON.stringify(commodityInfo);
    var price = commodityInfo.Price;
    var promotionType = commodityInfo.PromotionTypeNew;
    $("#noLoginText").hide();
    var tmpPrice = 0;
    if (promotionType == 9999) {
        if (JsVilaDataNull(vipData) && vipData.IsVip) {
            var tmpPriceTest = "";
            var minPriceShow =  setSessionStorage.minPriceShow ;
            var maxPrice = sessionStorage.maxPrice;
            if (minPriceShow) {
                 var minPrice = sessionStorage.minPrice;
                 tmpPriceTest = (Number(vipData.Intensity) * Number(minPrice) / 10).toFixed(2) +"-";
            }
            var vipMaxPrice = (Number(vipData.Intensity) * Number(maxPrice) / 10).toFixed(2);
            tmpPriceTest = tmpPriceTest + vipMaxPrice;
            $("#vipPrice2 .spanPrice").html(tmpPriceTest);
            $("#divVipMember").show();
            $("#vipPrice").show();
            $("#vipPrice2").show();
            $("#notVipText").hide();
        }else if (JsVilaDataNull(vipData) && !vipData.IsVip) {
            $("#divVipMember").show();
            $("#vipPrice").hide();
            $("#vipPrice2").hide();
            $("#notVipText").show();
        }
        else {
            $("#divVipMember").hide();
        }
    } else {
       $("#divVipMember").hide();
    }
}

function gotoCreateOrder(createOrderType, diyGroupId,totalPrice) {
    sessionStorage.PicturesPathModel = "";
    sessionStorage.PicturesPathModelMobile = "";
    sessionStorage.CreateOrderInfo = '';
    sessionStorage.userSelfTake = '';
    sessionStorage.removeItem("PicturesPathModel");
    sessionStorage.removeItem("PicturesPathModelMobile");
    sessionStorage.removeItem("CreateOrderInfo");
    sessionStorage.removeItem("userSelfTake");
    var url = urlAppendCommonParams('/Mobile/CreateOrder?esAppId=' + getQueryString("esAppId") + '&type=' + createOrderType);
    if (JsVilaDataNull(diyGroupId)) {
        url = addParam("diyGroupId", diyGroupId, url);
    }
    if (JsVilaDataNull(totalPrice)) {
        url = addParam("price", totalPrice, url);
    }
    if (JsVilaDataNull(sessionStorage.JcActivityId)) {
        url = addParam("jcActivityId", sessionStorage.JcActivityId, url);
    }
    window.location.href = url;
}
function gotoCreateOrder3(createOrderType, setMealId,totalPrice) {
    sessionStorage.PicturesPathModel = "";
    sessionStorage.PicturesPathModelMobile = "";
    sessionStorage.CreateOrderInfo = '';
    sessionStorage.userSelfTake = '';
    sessionStorage.removeItem("PicturesPathModel");
    sessionStorage.removeItem("PicturesPathModelMobile");
    sessionStorage.removeItem("CreateOrderInfo");
    sessionStorage.removeItem("userSelfTake");
    var url = urlAppendCommonParams('/Mobile/CreateOrder?type=' + createOrderType);
    if (JsVilaDataNull(setMealId)) {
        url = addParam("setMealId", setMealId, url);
    }
    if (JsVilaDataNull(totalPrice)) {
        url = addParam("price", totalPrice, url);
    }
    window.location.href = url;
}
function gotoCreateOrder2(createOrderType, diyGroupId,totalPrice) {
    sessionStorage.PicturesPathModel = "";
    sessionStorage.PicturesPathModelMobile = "";
    sessionStorage.CreateOrderInfo = '';
    sessionStorage.userSelfTake = '';
    sessionStorage.removeItem("PicturesPathModel");
    sessionStorage.removeItem("PicturesPathModelMobile");
    sessionStorage.removeItem("CreateOrderInfo");
    sessionStorage.removeItem("userSelfTake");
    var shopid = sessionStorage.appId;
    var url = urlAppendCommonParams('/Mobile/CYCreateOrder?shopId='+shopid+'&type=' + createOrderType);
    console.log("diyGroupId=",diyGroupId,"url=",url);
    if (JsVilaDataNull(diyGroupId)) {
        url = addParam("diyGroupId", diyGroupId, url);
    }
    if (JsVilaDataNull(totalPrice)) {
        url = addParam("price", totalPrice, url);
    }
    window.location.href = url;
}


//读取cookies 
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}
function sortNumber(a,b) 
{ 
    return a - b 
} 
