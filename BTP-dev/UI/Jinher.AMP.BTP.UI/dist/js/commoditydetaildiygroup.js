var flex = function () {
    var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function () {
    flex();
};

var DETAIL = (function (mod, $, undefined) {
    /**
    * 加载数据
    */
    mod.hasAttribute = false; //商品是否有属性
    mod.attributeTitle = []; //属性类别名称
    mod.allColor = []; //color属性所有值
    mod.allSize = []; //size属性所有值
    mod.allSpecifications = []; //规格设置
    mod.currentColor = ""; //color属性
    mod.currentSize = ""; //size 属性
    mod.currentSpecifications = ""; //规格属性
    mod.commodityStocks = []; //多属性数据
    mod.Duty = []; //关税
    mod.Intensity = null; //折扣
    mod.defaultPrice = null;
    mod.oldPrice = [];
    mod.outPromotionId = null; //外部优惠活动id
    mod.commodityStockId = null; //属性ID ,多属性商品中的属性组合ID，单属性和无属性商品中的属性ID（都是00000000-0000-0000-0000-000000000000）
    mod.limitBuyEach = null;
    mod.stock = 0; //库存
    mod.shoppingCartNum = 0; //购物车数量
    mod.selectCartNum = 1;
    mod.realPrice = null;
    mod.contactObj = null;
    mod.contactUrl = null;
    mod.commodityUpInfo = {}; //商品信息
    mod.resultIsVolume = false; //是否配置了销量
    mod.vipIntensity = null; //会员折扣
    mod.SkuActivityCdtos = []; //会员折扣
    mod.JCActivityItemsListCdtos = null; //金采团购
    mod.PromotionType = -1; //活动类型
    mod.DiyGroupPromotion = null; //拼团信息
    mod.PromotionId = null; //活动id
    mod.PinCommodityList = null;//默认选中的sku属性
    mod.YuanPeice = null; //原价
    mod.PinPrice = null;//拼团价
    mod.thumImg = ''; //默认缩略图
    mod.pictures = []; //默认轮播图
    mod.carouselImgs = []; //某一属性的轮播图
    mod.picDefault = true; //当前是默认的轮播图
    mod.State = '';
    mod.isScroll = true;

    mod.loadData = function () {
        var userid = getUserId() == null ? '00000000-0000-0000-0000-000000000000' : getUserId();
        $.ajax({
            url: '/Mobile/GetCommodityDetailsZPH?r=' + Math.random(),
            type: 'GET',
            dataType: 'json',
            data: {
                appId: getEsAppId(),
                userId: userid,
                commodityId: getQueryString("commodityId"),
                freightTo: sessionStorage.province,
                outPromotionId: getQueryString("outPromotionId"),
                source: ''
            },
            async: true
        }).done(function (data) {
            if (data && data.ResultCode == 0) {
                mod.contactObj = data.ContactObj;
                mod.contactUrl = data.ContactUrl;
                mod.resultIsVolume = data.ResultIsVolume;
                //sDOWNLOADBANNER.setDownloadBanner(data.AppDownLoadInfo);                
                //获取数据成功的操作   
                data.CommodityInfo && DETAIL.initData(data.CommodityInfo);
                var data = data.CommodityInfo;
                DETAIL.setEvaluate(data.HasReviewFunction, data.Score);
                DETAIL.GetDiyGrouplist();
                //轮播图
                var html = "";
                var lunbopic = '<div class="swiper-slide"><a href="javascript:void(0);"><img src="{pic}"/></a></div>';
                for (var i = 0; i < data.Pictures.length; i++) {
                    html += lunbopic.replace("{pic}", data.Pictures[i].PicturesPath);
                }
                $("#swiper-wrapper").html(html);
                //轮播图

                //运费 
                if (data.Freight > 0)
                    $('.freight').text('运费：￥' + Math.abs(data.Freight || 0) + (data.PostAge ? ',' + data.PostAge : ''));
                else
                    $('.freight').text('快递:包邮');

                //分销
                if (!data.IsActiveCrowdfunding) {
                    $('.parameters').addClass('hide');
                }

                //标签+商品名称
                var tag = '<span class="tag">{tags}</span>';
                var title = '<span class="title">{title}</span>';
                var tag1 = "";
                var title1 = "";
                if (data.SelfSupport) {
                    tag1 += tag.replace("{tags}", data.SelfSupport);
                }
                else {
                    tag1 += tag.replace("tag", "tag hide");
                }
                title1 += title.replace("{title}", data.Name);
                $('.goods-title').html(tag1 + title1);

                //标签+商品名称

                //服务项
                //页面服务标题
                var service = '<span class="service-list">{servicelist}</span>';
                //弹窗服务内容
                var service1 = '<li class="item">'
                    + '<img class="item-pic" src="../../Images/yj-service-icon.png">'
                    + '  <div class="item-context">'
                    + '    <div class="title">{servicetitle}</div>'
                    + '    <div class="text">{servicecontent}</div>'
                    + '  </div>'
                    + '</li>';
                var servermod = "";
                if (data.ServiceSettings !== null && data.ServiceSettings.length > 0) {
                    for (var i = 0; i < data.ServiceSettings.length; i++) {

                        $('.service-list-wrap').append(service.replace("{servicelist}", data.ServiceSettings[i].Title));

                        //$('.server-list').append(service1.replace("{servicetitle}", data.ServiceSettings[i].Title).replace("{servicecontent}", data.ServiceSettings[i].Content));
                        servermod += service1.replace("{servicetitle}", data.ServiceSettings[i].Title).replace("{servicecontent}", data.ServiceSettings[i].Content);
                    }
                    $('#servermod').html('<ul class="server-list">' + servermod + '</ul>');
                }
                else {
                    $('.service-list-wrap').append(service.replace("{servicelist}", "暂无服务！"));
                }
                //服务项

                //商品详情+商品售后
                if (data.Description === null) {
                    $("#DetailsH").addClass('hide');
                    $("#DetailsD").addClass('hide');
                }
                else {
                    $("#DetailsD").html(data.Description);
                }
                if (data.SaleService === null) {
                    $("#AftersaleH").addClass('hide');
                    $("#AftersaleD").addClass('hide');
                } else {
                    var SaleService = data.SaleService.split("。");

                    var cont = '<span class="context">{context}</span>';
                    var text = "";
                    for (var i = 0; i < SaleService.length; i++) {
                        text += cont.replace("{context}", SaleService[i]);
                    }
                    $("#AftersaleD").html("<p>" + text + "</p>");
                }
                //商品详情+商品售后

                mod.initBanner('#banner');
                mod.initEvent();
                mod.initEvaluate('#evaluate');

            }
            $('#mask').addClass('hide');
        }).fail(function (err) {
            //获取数据失败的操作
            toast('服务器繁忙，请稍后重试!');
            $('#mask').addClass('hide');
        });

        //数据在在完后 隐藏加载动画
        $('#mask').addClass('hide');
    };
    mod.initEvent = function () {
        $('.header-content').on('click', '.detail-nav', function () {
            var state = $(this).data('type');
            var header = $('#header').height();
            var top = $('#' + state).offset().top - header + 2;
            mod.isScroll = false;
            document.documentElement.scrollTop = top;
            mod.isScroll = true;
        });
        $(window).on('scroll', function () {
            if (!mod.isScroll) {
                return;
            }
            var header = $('#header').height();
            var height = document.documentElement.scrollTop;
            var eval = $('#eval').offset().top;//评价
            var detail = $('#detail').offset().top;//详情
            if (height >= detail - header) {
                mod.updateNav(2);
            } else if (height < detail - header && height >= eval - header) {
                mod.updateNav(1);
            } else {
                mod.updateNav(0);
            }
        });

        $("#TogetherBuy").on('click', function () {
            $('#ifPin').val(0);
            if ($(this).hasClass('disabled')) {
                return;
            }
            mod.updateSpinner(1);
            if ($('.sku').hasClass('hide')) {
                if (isLogin()) {
                    mod.GoOrder();
                } else {
                    DealLoginPartial.initPartialPage();
                }
            } else {
                $('#size-modal').removeClass('hide');
                $('#attributeBuy').addClass('hide');
                $('#attributeSpelling').addClass('hide');
                $('#Determine').removeClass('hide');
                $('#count').addClass('hide');
                $('#money').text(PinPrice);                
            }
        });

        $('#Determine').on('click', function () {
            //判断是否登录，未登录跳转到登录页
            if (isLogin()) {
                mod.GoOrder();
            } else {
                DealLoginPartial.initPartialPage();
            }
        });

        $('.desc-btn').on('click', function () {
            //判断是否登录，未登录跳转到登录页
            if (isLogin()) {
                if ($(this).hasClass("desc-btn")) {
                    var groupId = $(this).data("group-id");

                    if (location.href.indexOf("diyGroupId") == -1) {
                        if (location.href.indexOf("?") != -1) {
                            window.history.pushState({}, 0, location.href + "&diyGroupId=" + groupId);
                        }
                        else {
                            window.history.pushState({}, 0, location.href + "&diyGroupId=" + groupId);
                        }
                    }
                }
                //开 || 参
                var action = $(this).children().eq(1).children().first().text();
                if (action == "参" || $(this).hasClass("desc-btn")) {
                    //检测拼团状态
                    $.getJSON("/mobile/CheckDiyGroup", { diyGroupId: groupId }, function (json) {
                        if (json.ResultCode === 0 && !json.Data.IsCompleted) {
                            $('#ifPin').val(0);
                            DraddEventListener(mod.commodityUpInfo);
                        }
                        else {
                            alert("哎呀，下手慢了。再看看其他人的团吧");
                            location.reload(true);
                        }
                    });
                }
                else {
                    mod.GoOrder();
                }
            } else {
                DealLoginPartial.initPartialPage();
            }
        });

        $("#AloneBuy").on('click', function () {
            $('#ifPin').val(1);

            if ($(this).hasClass('disabled')) {
                return;
            }
            if ($('.sku').hasClass('hide')) {
                if (isLogin()) {
                    mod.GoOrder();
                } else {
                    DealLoginPartial.initPartialPage();
                }
            }
            else {
                $('#size-modal').removeClass('hide');
                $('#attributeBuy').addClass('hide');
                $('#attributeSpelling').addClass('hide');
                $('#Determine').removeClass('hide');
                $('#count').removeClass('hide');
                $('#money').text(YuanPeice);

            }
        });

        //自己买
        $("#attributeBuy").bind("click", function () {
            //判断是否登录，未登录跳转到登录页
            if ($(this).hasClass('disabled')) {
                return;
            }
            if (isLogin()) {
                mod.GoOrder();
            } else {
                DealLoginPartial.initPartialPage();
            }
        });

        $('#attributeSpelling').on('click', function () {
            //判断是否登录，未登录跳转到登录页
            if ($('#attributeSpelling').hasClass('disabled')) {
                return;
            }
            mod.updateSpinner(1);
            if (isLogin()) {
                mod.GoOrder();
            } else {
                DealLoginPartial.initPartialPage();
            }
        });

        $('#evalhide').on('click', function () {
            var commodityId = sessionStorage.commodityId_2 || getQueryString('commodityId');
            var url = SNSUrl + "/Evaluate/List?appId=" + sessionStorage.appId + "&productId=" + commodityId;
            document.location.href = url;
        });

        //分销
        $('.parameters').on('click', function () {
            if (isLogin()) {
                document.location.href = urlAppendCommonParams("/Distribute/CommodityDistribute?commodityId=" + getQueryString('commodityId') + "&distributorId=" + sessionStorage.distributorId);
            } else {
                DealLoginPartial.initPartialPage();
            }
        });


        $('.modal-wrap').on('click', function (event) {
            var target = event.target;
            var flag = $(target).hasClass('modal-wrap');
            if (flag) {
                var id = $(target).attr('id');
                mod.hideModal('#' + id);
            }
        });
        /**
        * mins
        */
        $('#modal-wrap-increase').on('click', function () {
            var number = $('#modal-wrap-count').val() - 0;
            number++;
            mod.updateSpinner(number);
        });
        /**
        * plus
        */
        $('#modal-wrap-decrease').on('click', function () {
            var number = $('#modal-wrap-count').val() - 0;
            if (number > 1) {
                number--;
                mod.updateSpinner(number);
            }
        });
        $('#modal-wrap-count').on('change', function () {
            var number = parseInt($(this).val() - 0);
            if (number < 1) {
                number = 1;
            }
            mod.updateSpinner(number);
        });
    };

    /**
    * 更新顶部导航
    * @param index
    */
    mod.updateNav = function (index) {
        $('.detail-nav').removeClass('selected');
        $('.detail-nav').eq(index).addClass('selected');
    };

    /**
    * 获取指定属性的商品（多属性商品）
    * @param color color属性
    * @param size  size属性
    * @returns {*}
    */
    mod.getCommodity = function (color, size, isSingle) {
        var result = null;
        if (isSingle) { //单属性
            if (mod.commodityStocks.length) {
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    var commodity = mod.commodityStocks[i];
                    if (commodity.ComAttribute[0].SecondAttribute == color) {
                        result = commodity;
                        break;
                    }
                }
            }
        } else { //多属性
            for (var i = 0; i < mod.commodityStocks.length; i++) {
                var commodity = mod.commodityStocks[i];
                if ((commodity.ComAttribute[0].SecondAttribute == color && commodity.ComAttribute[1].SecondAttribute == size) || (commodity.ComAttribute[1].SecondAttribute == color && commodity.ComAttribute[0].SecondAttribute == size)) {
                    result = commodity;
                    break;
                }
            }
        }
        return result;
    };
    /**
    * 实例化顶部商品banner
    * @param el
    */
    mod.initBanner = function (el) {
        new Swiper(el, {
            direction: 'horizontal',
            loop: true,
            autoplayDisableOnInteraction: false,
            pagination: '.swiper-pagination', // 如果需要分页器
            paginationType: 'custom', //分页器样式类型 1."bullets": 圆点（默认）2."fraction"：分式 3. "progressbar":进度条 4."custom" :自定义
            paginationCustomRender: function (swiper, current, total) { //生成我们自定义的分页器到页面上
                var customPaginationHtml = "";
                for (var i = 0; i < total; i++) {
                    //判断哪个分页器此刻应该被激活
                    if (i == (current - 1)) {
                        customPaginationHtml += '<span class="swiper-pagination-customs swiper-pagination-customs-active"></span>';
                    } else {
                        customPaginationHtml += '<span class="swiper-pagination-customs"></span>';
                    }
                }
                return customPaginationHtml;
            },
            paginationClickable: true,
            autoplay: 5000
        });
    };
    /**
    * 实例化商品评价 滑动组件
    */
    mod.initEvaluate = function (el) {
        new Swiper(el, {
            slidesPerView: 'auto',
            spaceBetween: 0,
            resistanceRatio: 0.5,
            freeMode: true
        });
    };
    /**
    * 显示弹窗
    * @param el
    */
    mod.showModal = function (el) {
        $(el).removeClass('hide');
        $('body').addClass('clock');
        $('#ifPin').val(0);
        $('#money').html(PinPrice);
        if ($('#attributeBuy').hasClass('hide')) {
            $('#attributeBuy').removeClass('hide');
            $('#attributeSpelling').removeClass('hide');
            $('#count').removeClass('hide');
        }
        $('#Determine').addClass('hide');
        $('#modal-wrap-increase').removeClass('disable');
    };
    /**
    * 隐藏弹窗
    * @param el
    */
    mod.hideModal = function (el) {
        $(el).addClass('hide');
        $('body').removeClass('clock');
    };
    /**
    * 更新spinner
    *
    */
    mod.initData = function (data) {
        //保存商品信息
        YuanPeice = data.Price.toFixed(2);
        PinPrice = data.DiscountPrice.toFixed(2);
        mod.stock = data.Stock;
        mod.DiyGroupPromotion = data.DiyGroupPromotion;
        mod.PromotionId = data.PromotionId;
        mod.thumImg = data.Pic;
        mod.commodityUpInfo = {
            Appid: data.DiyGroupPromotion.AppId,
            CommodityId: data.DiyGroupPromotion.CommodityId,
            CommodityStockId: data.CommodityStockId,
            name: data.Name,
            pic: data.Pic,
            Stock: mod.stock,
            color: '',
            size: '',
            duty: data.Duty,
            focusAttrPrice: 0,//属性原价
            number: $('#modal-wrap-count').val(),
            price: YuanPeice,//属性原价
            realPrice: PinPrice,//属性活动价
            Intensity: data.Intensity,
            IsEnableSelfTake: data.IsEnableSelfTake,
            marketPrice: Math.abs(data.MarketPrice).toFixed(2),
            CommodityType: data.CommodityType,
            orderType: data.CommodityType,
            PromotionType: data.DiyGroupPromotion.PromotionType,
            OutPromotionId: data.OutPromotionId || ''
        };

        $('#pic').attr('src', data.Pic);
        $('#groupMinVolume').text(data.DiyGroupPromotion.GroupMinVolume - 1);

        if (data.CommodityStocks && data.CommodityStocks.length) {

            mod.commodityStocks = data.CommodityStocks;
            mod.SkuActivityCdtos = data.SkuActivityCdtos;
            for (var i = 0; i < data.CommodityStocks.length; i++) {
                if (mod.commodityStocks[i].Stock > 0) {
                    if (mod.commodityStocks[i].Stock < 10)
                        $('#IsStock').html('库存紧张');
                    else
                        $('#IsStock').html('库存充足');

                    mod.PinCommodityList = mod.commodityStocks[i];
                    break;
                }
                if (i == data.CommodityStocks.length - 1) {
                    mod.PinCommodityList = mod.commodityStocks[0];
                    $('#AloneBuy').removeClass('yellow');
                    $('#AloneBuy').addClass('disabled').addClass('gray');
                    $('#TogetherBuy').addClass('black').addClass('disabled');
                    $('#attributeBuy').removeClass('yellow');
                    $('#attributeBuy').addClass('disabled').addClass('gray');
                    $('#attributeSpelling').addClass('black').addClass('disabled');
                }
            }
        }
        else { 
            $('.sku').addClass('hide');
            if (data.Stock <= 0) {
                $('#AloneBuy').removeClass('yellow');
                $('#AloneBuy').addClass('disabled').addClass('gray');
                $('#TogetherBuy').addClass('black').addClass('disabled');
                $('#attributeBuy').removeClass('yellow');
                $('#attributeBuy').addClass('disabled').addClass('gray');
                $('#attributeSpelling').addClass('black').addClass('disabled');
            }
        }

        if (data.ComAttibutes && data.ComAttibutes.length) {
            for (var i = 0; i < data.ComAttibutes.length; i++) {
                if (mod.attributeTitle.indexOf(data.ComAttibutes[i].Attribute) < 0) {
                    mod.attributeTitle.push(data.ComAttibutes[i].Attribute);
                }
            }
        }
        if (mod.attributeTitle.length == 2) { //商品包含多个属性
            mod.hasAttribute = true;
            for (var i = 0; i < data.ComAttibutes.length; i++) {
                if (mod.attributeTitle[0].indexOf(data.ComAttibutes[i].Attribute) < 0) {
                    mod.allSize.push(data.ComAttibutes[i].SecondAttribute);
                } else {
                    mod.allColor.push(data.ComAttibutes[i].SecondAttribute);
                }
            }
        } else if (mod.attributeTitle.length == 1) { //商品只有一个属性
            mod.hasAttribute = true;
            for (var i = 0; i < data.ComAttibutes.length; i++) {
                mod.allColor.push(data.ComAttibutes[i].SecondAttribute);
            }
        }
        mod.initColorSize();
    };
    mod.renderColorSize = function (id, title, data, currentSelect, disAttribute) {
        var html = '<section id="' + id + '" class="sku-list"><h2 class="sku-title">' + title + '</h2><div class="sku-content size">';
        for (var j = 0; j < data.length; j++) {
            var flag = true;
            for (var k = 0; k < disAttribute.length; k++) {
                if (data[j] == disAttribute[k]) {
                    html += '<span class="size-item disable">' + data[j] + '</span>';
                    flag = false;
                    break;
                }
            }

            if ($('#AloneBuy').hasClass('disabled') && $('#TogetherBuy').hasClass('disabled')) {
                html += '<span class="size-item selected">' + data[j] + '</span>';
                flag = false;
            }

            if (flag) {
                if (currentSelect == data[j]) {
                    html += '<span class="size-item selected">' + data[j] + '</span>';
                } else {
                    html += '<span class="size-item">' + data[j] + '</span>';
                }
            }
        }
        return html + '</section>';
    };
    mod.initColorSize = function () {
        var html = '';
        if (mod.PinCommodityList) {
            for (var i = 0; i < mod.PinCommodityList.ComAttribute.length; i++) {
                mod.currentColor = mod.PinCommodityList.ComAttribute[0].SecondAttribute;
                if (i == 1) {
                    mod.currentSize = mod.PinCommodityList.ComAttribute[1].SecondAttribute;
                }
            }
        }

        if (mod.allColor.length) {
            html += mod.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, []);
        }
        if (mod.allSize.length) {
            html += mod.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, []);
        }
        $('.modal-bd').append(html);

        mod.updatePrice();
        //属性选择事件绑定
        $('#size-modal').on('click', '.size-item', function (event) {
            var target = event.target;
            if ($(target).hasClass('disable')) {
                return;
            } else {
                if ($(target).parent().parent()[0].id == 'color') {
                    mod.currentClick = $(target).text();
                    mod.updateColorSize(target, 'color');
                } else if ($(target).parent().parent()[0].id == 'size') {
                    mod.currentClick = $(target).text();
                    mod.updateColorSize(target, 'size');
                }
                mod.updatePrice();
            }
        });
    };

    /**
   * 获取禁止状态的属性
   */
    mod.getDisableAttr = function (param, attributeTitle) {
        var disAttribute = [];
        if ((typeof param) == 'string') {
            var disableNum = $('#' + param).find('.disable');
            for (var k = 0; k < disableNum.length; k++) {
                disAttribute.push(disableNum.eq(k).text());
            }
        } else if (param instanceof Array) {
            for (var i = 0; i < param.length; i++) {
                var commodity = param[i];
                if (commodity.Stock <= 0) {
                    if (commodity.ComAttribute[0].Attribute == attributeTitle) {
                        if (commodity.ComAttribute[0].SecondAttribute == mod.currentClick) {
                            disAttribute.push(commodity.ComAttribute[1].SecondAttribute)
                        }
                    } else {
                        if (commodity.ComAttribute[1].SecondAttribute == mod.currentClick) {
                            disAttribute.push(commodity.ComAttribute[0].SecondAttribute)
                        }
                    }
                }
            }
        }
        return disAttribute;
    };

    /**
    * 更新属性状态
    * @param target 点击的目标元素
    * @param selectAttr 点击的目标元素所在的属性分类
    */
    mod.updateColorSize = function (target, selectAttr) {
        try {
            var html = '';
            if ($(target).hasClass('selected')) { //取消选中的属性
                if (mod.allColor.length) {
                    if (selectAttr == 'color') {
                        mod.currentColor = '';
                        html += mod.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, mod.getDisableAttr('color'));
                    } else {
                        html += mod.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, []);
                    }
                    $('#color').empty();
                    $('#color').append(html);
                }
                var html = '';
                if (mod.allSize.length) {
                    if (selectAttr == 'size') {
                        mod.currentSize = '';
                        html += mod.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, mod.getDisableAttr('size'));
                    } else {
                        html += mod.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, []);
                    }
                    $('#size').empty();
                    $('#size').append(html);
                }
            } else { //选择属性
                if (mod.allColor && mod.allColor.length) {
                    if (selectAttr == 'color') {
                        mod.currentColor = $(target).text();
                        html += mod.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, mod.getDisableAttr('color'));
                    } else {
                        html += mod.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, mod.getDisableAttr(mod.commodityStocks, mod.attributeTitle[1]));
                    }
                    $('#color').empty();
                    $('#color').append(html);
                }
                var html = '';
                if (mod.allSize && mod.allSize.length) {
                    if (selectAttr == 'size') {
                        mod.currentSize = $(target).text();
                        html += mod.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, mod.getDisableAttr('size'));
                    } else {
                        html += mod.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, mod.getDisableAttr(mod.commodityStocks, mod.attributeTitle[0]));
                    }
                    $('#size').empty();
                    $('#size').append(html);
                }
            }
        } catch (e) { };
        return;
    };

    mod.updatePrice = function () {
        var thumImg = mod.thumImg;
        //如果是并且两种属性已经选择了，则显示对应的值
        if (mod.attributeTitle.length == 2) {
            if (mod.currentColor && mod.currentSize) { //两种属性已经选择了
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    var commodity = mod.commodityStocks[i];

                    if ((commodity.ComAttribute[0].SecondAttribute == mod.currentColor && commodity.ComAttribute[1].SecondAttribute == mod.currentSize) || (commodity.ComAttribute[1].SecondAttribute == mod.currentColor && commodity.ComAttribute[0].SecondAttribute == mod.currentSize)) {
                        mod.commodityUpInfo.price = commodity.Price;
                        mod.commodityUpInfo.duty = commodity.Duty;
                        mod.commodityUpInfo.stock = mod.stock;

                        thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                        mod.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : [];
                        YuanPeice = commodity.Price.toFixed(2);
                        mod.stock = commodity.Stock;                        
                        if (commodity.Stock < 10)
                            $('#IsStock').html('库存紧张');
                        else
                            $('#IsStock').html('库存充足');

                        if (mod.DiyGroupPromotion != null && mod.PromotionId == null) {
                            if (mod.SkuActivityCdtos && mod.SkuActivityCdtos.length > 0) {
                                for (var j = 0; j < mod.SkuActivityCdtos.length; j++) {
                                    if (mod.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                        PinPrice = mod.SkuActivityCdtos[j].JoinPrice.toFixed(2);
                                        mod.commodityUpInfo.CommodityStockId = mod.SkuActivityCdtos[j].CommodityStockId;
                                    }
                                }
                            } else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    PinPrice = mod.DiscountPrice.toFixed(2);
                                }
                            }
                        }
                    }
                }
            }
        } else if (mod.attributeTitle.length == 1) {
            if (mod.currentColor && mod.commodityStocks.length) {
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    var commodity = mod.commodityStocks[i];                    
                    
                    if (commodity.ComAttribute[0].SecondAttribute == mod.currentColor) {
                        mod.commodityUpInfo.price = commodity.Price;                        
                        mod.commodityUpInfo.duty = commodity.Duty;
                        mod.commodityUpInfo.stock = mod.stock;
                        
                        thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                        mod.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : []
                        YuanPeice = commodity.Price.toFixed(2);
                        mod.stock = commodity.Stock;
                        if (commodity.Stock < 10)
                            $('#IsStock').html('库存紧张');
                        else
                            $('#IsStock').html('库存充足');

                        if (mod.DiyGroupPromotion != null && mod.PromotionId == null) {
                            if (mod.SkuActivityCdtos && mod.SkuActivityCdtos.length > 0) {
                                for (var j = 0; j < mod.SkuActivityCdtos.length; j++) {
                                    if (mod.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                        PinPrice = mod.SkuActivityCdtos[j].JoinPrice.toFixed(2);
                                        mod.commodityUpInfo.CommodityStockId = mod.SkuActivityCdtos[j].CommodityStockId;
                                    }
                                }
                            }
                            else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    PinPrice = mod.DiscountPrice.toFixed(2);
                                }
                            }
                        }
                    }
                }
            }
        }
        var disprice = '<i>￥</i>{money}<em>{point}</em>';

        $('#PinTuanPrice').html(disprice.replace("{money}", PinPrice.substr(0, PinPrice.indexOf('.'))).replace("{point}", '.' + PinPrice.replace(/\d+\.(\d*)/, "$1")));
        $('#YiQiPin').html(disprice.replace("{money}", PinPrice.substr(0, PinPrice.indexOf('.'))).replace("{point}", '.' + PinPrice.replace(/\d+\.(\d*)/, "$1")));
        $('#ZiJiMai').html(disprice.replace("{money}", YuanPeice.substr(0, YuanPeice.indexOf('.'))).replace("{point}", '.' + YuanPeice.replace(/\d+\.(\d*)/, "$1")));
        $('#PinTuanOld').text("￥" + YuanPeice);

        $('#SXYQ').html(disprice.replace("{money}", PinPrice.substr(0, PinPrice.indexOf('.'))).replace("{point}", '.' + PinPrice.replace(/\d+\.(\d*)/, "$1")));
        $('#SXZJ').html(disprice.replace("{money}", YuanPeice.substr(0, YuanPeice.indexOf('.'))).replace("{point}", '.' + YuanPeice.replace(/\d+\.(\d*)/, "$1")));

        //原价与拼团价相同不显示原价 
        if (YuanPeice === PinPrice) {
            $('.old-price').replace("old-price", "old-price hide");
        }

        $('.sku-list-item').text(mod.currentColor + '，' + mod.currentSize + '，' + $('#modal-wrap-count').val() + '件');

        $('#DityPic').attr('src', thumImg);
        if ($('#ifPin').val() == 0) {
            $('#money').html(PinPrice);
        }
        else {
            $('#money').html(YuanPeice);
        }

        mod.realPrice = PinPrice;
        mod.commodityUpInfo.Price = YuanPeice;
        mod.commodityUpInfo.color = mod.currentColor;
        mod.commodityUpInfo.size = mod.currentSize;
        mod.commodityUpInfo.number = $('#modal-wrap-count').val();
        mod.commodityUpInfo.pic = thumImg;

    };

    mod.updateSpinner = function (count, stock) {
        stock = mod.stock || 999;
        $('#modal-wrap-increase').removeClass('disable');
        $('#modal-wrap-decrease').removeClass('disable');
        if (count == 1) {
            $('#modal-wrap-decrease').addClass('disable');
            $('#modal-wrap-count').val(1);
        }
        if (stock < count) { //数量小于库存
            $('#modal-wrap-increase').addClass('disable');
            $('#modal-wrap-count').val(stock);
        }
        if (count > 1 && count <= stock) {
            $('#modal-wrap-count').val(count);
        }

        $('.sku-list-item').text(mod.currentColor + '，' + mod.currentSize + '，' + $('#modal-wrap-count').val() + '件');
    };
    mod.setEvaluate = function (HasReviewFunction, Score) {
        var commodityId = sessionStorage.commodityId_2 || getQueryString('commodityId');
        var url = SNSUrl + "/Evaluate/List?appId=" + sessionStorage.appId + "&productId=" + commodityId;
        //$('#more').attr('href', url);

        var evaluate = '<div class="swiper-slide">'
            + '  <div class="evaluate-text">'
            + '      <div class="evaluate-text-hd">'
            + '          <img class="head" src="{userpic}" onerror="../../Images/yj-service-icon.pn">'
            + '          <span class="name">{username}</span>'
            + '      </div>'
            + '      <div class="evaluate-text-bd">{evacontent}</div>'
            + '  </div>'
            + '  <img class="evaluate-pic" src="{compic}">'
            + '</div>';
        var href = '<a href="{href}" class="more-evaluate">更多评价</a>';
        var evaluateconfig = "";
        if (HasReviewFunction) { //商品评价
            if (Score.Records.length > 0) {
                $('#recevied').text(100 * (Score.Evaluate.GoodCount / Score.TotalCount).toFixed(2) + "%好评");
                $('.count').text("(" + Score.TotalCount + ")");
                for (var i = 0; i < Score.Records.length; i++) {
                    evaluateconfig += evaluate.replace("{userpic}", Score.Records[i].Icon).replace("{username}", Score.Records[i].UserName).replace("{evacontent}", Score.Records[i].Content);
                    if (Score.Records[i].PhotosArr.length <= 0)
                        evaluateconfig += evaluate.replace('evaluate-pic', 'evaluate-pic hide');
                    else
                        evaluateconfig += evaluate.replace("{compic}", Score.Records[i].PhotosArr[0]);

                    if (i > 4)
                        break;//评价显示5条
                }
                $('#evaluate').html('<div class="swiper-wrapper">' + evaluateconfig + href.replace("{href}", url) + '</div>');
            } else {
                $("#evaluate").addClass('hide');
                $("#evalshow").addClass('hide');
                $("#evalhide").removeClass('hide');
            }
        } else { //定制平台没有配置“商品评价”组件
            $("#eval").addClass('hide');
        }
    };
    mod.GetDiyGrouplist = function () {
        //已开团信息
        var obj = {
            appId: getEsAppId(),
            commodityId: getQueryString("commodityId"),
            outsidePromoId: getQueryString("outPromotionId")
        };

        $.ajax({
            type: "POST",
            async: false,
            url: "/Mobile/UnfinishedDiyGrouplist",
            data: obj,
            success: function (data) {
                if (data.length > 0) {
                    var pin = '<div class="desc-bt">'
                        + '      <img class="pic" src="{Headportrait}">'
                        + '      <span class="name">{Nickname}</span>'
                        + '      <div class="desc-bt-right">'
                        + '          <div class="info">还差 <span class="person">{number}</span> 人成团</div>'
                        //<div class="time countdown" data-seconds="' + data[i].SpareSeconds + '">剩余 <span></span>结束</div>
                        + '          <div class="time countdown" data-seconds={Remainingtime}>剩余 <span class="date"></span> 结束</div>'
                        + '      </div>'
                        + '      <a href="javascript:;" class="desc-btn" data-group-id="{groupid}">去参团</a>'
                        + '  </div>';
                    var html = "";
                    for (var i = 0; i < data.length; i++) {
                        html += pin.replace("{Headportrait}", data[i].OwnerIcon).replace("{Nickname}", data[i].OwnerName).replace("{number}", data[i].LackMember).replace("{Remainingtime}", data[i].SpareSeconds).replace("{groupid}", data[i].GroupId);
                        if (i > 9)
                            break;
                    }

                    $(".desc-wrap").append(html);
                    startCountdown();
                }
            }
        });
    };

    mod.GoOrder = function () {
        if ($('.sku').hasClass('hide')) {
            if ($('#ifPin').val() == 0)
                mod.commodityUpInfo.realPrice = PinPrice;
            else
                mod.commodityUpInfo.realPrice = YuanPeice;
        }
        else
            mod.commodityUpInfo.realPrice = $('#money').text();
        //直接购买
        if (mod.attributeTitle.length == 2) {
            if (mod.currentSize == "" && mod.currentColor == "") {
                toast("请选择" + mod.allSize + " " + mod.allColor);
                return;
            }
            else if (mod.currentSize == "") {
                toast("请选择" + mod.allSize);
                return;
            }
            else if (mod.currentColor == "") {
                toast("请选择" + mod.allColor);
                return;
            }
        }
        else if (mod.attributeTitle.length == 1 && mod.currentSize == "") {
            toast("请选择" + mod.allSize);
            return;
        } else {
            //
        }

        if ($("#modal-wrap-count").val() == "") {
            toast("购买数量不能为空");
            return;
        }
        if (mod.stock == 0) {
            toast("选择的商品数大于库存数");
            return;
        }
        var num = parseInt($("#modal-wrap-count").val());
        if (num > mod.stock) {
            toast("选择的商品数大于库存数");
            return;
        }
        if ($('#ifPin').val() == 0) {
            mod.realPrice = PinPrice;
            if (mod.realPrice <= 0) {
                if (!$('.sku').hasClass('hide')) {
                    var price = $('#money').text();
                    price = price.substring(1, price.length);
                    mod.realPrice = parseInt(price);
                }
                else
                    toast("");
            }
        }
        DraddEventListener(mod.commodityUpInfo);
    };
    return mod;
})(DETAIL || {}, Zepto);

$(document).ready(function () {
    DETAIL.loadData();
    checkMobileParams();
});