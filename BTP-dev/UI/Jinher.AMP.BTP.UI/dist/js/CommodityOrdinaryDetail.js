var flex = function () {
    var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
    document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
};
flex();
window.onresize = function () {
    flex();
};
var DETAIL = (function (mod, $, undefined) {
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
    mod.DiscountPrice = null; //优惠价
    mod.Intensity = null; //折扣
    mod.defaultPrice = null;
    mod.price = null;
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
    mod.SkuActivityCdtos = null; //会员折扣
    mod.JCActivityItemsListCdtos = null; //金采团购
    mod.PromotionTypeNew = -1; //活动类型
    mod.DiyGroupPromotion = null; //拼团信息
    mod.PromotionId = null; //活动id

    mod.thumImg = ''; //默认缩略图
    mod.pictures = []; //默认轮播图
    mod.carouselImgs = []; //某一属性的轮播图
    mod.picDefault = true; //当前是默认的轮播图
    mod.State = '';
    mod.InsteadCashAmount = 0;
    mod.DefaultCommodityList = null;//默认选中的数据

	/**
	 * 加载数据
	 */
    mod.isScroll = true;

    mod.loadData = function () {
        //数据在在完后 隐藏加载动画
        $('#mask').addClass('hide');

        //金采团购活动商品
        if (getQueryString("jcActivityId") !== null) {
            $.ajax({
                url: '/Mobile/GetCommodityDetailsZPHII?r=' + Math.random(),
                type: 'GET',
                dataType: 'json',
                data: {
                    appId: getEsAppId(),
                    userId: getUserId(),
                    commodityId: getQueryString("commodityId"),
                    freightTo: sessionStorage.province,
                    jcActivityId: getQueryString("jcActivityId"),
                    source: ''
                },
                async: true
            }).done(function (data) {
                if (data && data.ResultCode == 0) {
                    mod.contactObj = data.ContactObj;
                    mod.contactUrl = data.ContactUrl;
                    mod.resultIsVolume = data.ResultIsVolume;
                    //DOWNLOADBANNER.setDownloadBanner(data.AppDownLoadInfo);
                    //获取数据成功的操作
                    data.CommodityInfo && DETAIL.initData(data.CommodityInfo);
                } else {
                    $('#noCommodity').removeClass('hide');
                    $('#summary').addClass('hide');
                    $('#nav').addClass('hide');
                }
                $('#mask').addClass('hide');
            }).fail(function (err) {
                //获取数据失败的操作
                toast('服务器繁忙，请稍后重试!');
                $('#mask').addClass('hide');
            });
        } else {
            $.ajax({
                url: '/Mobile/GetCommodityDetailsZPH?r=' + Math.random(),
                type: 'GET',
                dataType: 'json',
                data: {
                    appId: getEsAppId(),
                    userId: getUserId(),
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
                    //DOWNLOADBANNER.setDownloadBanner(data.AppDownLoadInfo);
                    //获取数据成功的操作
                    data.CommodityInfo && DETAIL.initData(data.CommodityInfo);
                } else {
                    $('#noCommodity').removeClass('hide');
                    $('#summary').addClass('hide');
                    $('#nav').addClass('hide');
                }
                $('#mask').addClass('hide');
            }).fail(function (err) {
                //获取数据失败的操作
                toast('服务器繁忙，请稍后重试!');
                $('#mask').addClass('hide');
            });
        }

        this.initEvent();
        this.initBanner('#banner');
        this.initEvaluate('#evaluate');
        this.intRecommend('#recommend');
        //有易捷币抵现的情况 更新到手价的字体大小
        this.updateFontSize();
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
        $('.modal-wrap').on('click', function (event) {
            var target = event.target;
            var flag = $(target).hasClass('modal-wrap');
            if (flag) {
                var id = $(target).attr('id');
                mod.hideModal('#' + id);
            }
        });

        //点击底部加入购物车
        $('#addShop').on('click', function () {
            if ($(this).hasClass('disabled')) {
                return;
            }
            $('#iFBuy').val(1);
            if ($('.sku').hasClass('hide')) {
                //判断是否登录，未登录跳转到登录页
                if (isLogin()) {

                    if ($('FastBuy').hasClass('hide')) {
                        mod.realPrice = mod.price;
                        mod.commodityUpInfo.realPrice = mod.price;
                    }

                    mod.selectCartNum = $('#modal-wrap-count').val();
                    mod.addShopCart();
                } else {
                    DealLoginPartial.initPartialPage();
                }
            } else {
                $('#SkuaddShop').addClass('hide');
                $('#SkuBuy').addClass('hide');
                $('#SkuDetermine').removeClass('hide');

            }
        });

        //点击底部立即购买
        $('#FastBuy').on('click', function () {
            if ($(this).hasClass('disabled')) {
                return;
            }
            $('#iFBuy').val(0);
            if ($('.sku').hasClass('hide')) {
                //判断是否登录，未登录跳转到登录页
                if (isLogin()) {
                    var urlCommodityStockId = getQueryString("commodityStockId");
                    mod.selectCartNum = $('#modal-wrap-count').val();
                    mod.checkCommodity();
                } else {
                    DealLoginPartial.initPartialPage();
                }
            } else {
                $('#SkuaddShop').addClass('hide');
                $('#SkuBuy').addClass('hide');
                $('#SkuDetermine').removeClass('hide');
            }
        });

        //点击属性中确定
        $('#SkuDetermine').on('click', function () {
            if (isLogin()) {
                if ($('#iFBuy').val() == 0) {
                    //立即购买
                    if (mod.hasAttribute && urlCommodityStockId === null) {
                        mod.selectCartNum = $('#modal-wrap-count').val(1);
                        /*初始化属性列表*/
                        mod.initColorSize();
                        //显示选择属性弹窗
                        $('#modal-wrap').removeClass('hide');
                        mod.currentColor = '';
                        mod.currentSize = '';
                        mod.currentSpecifications = '';
                        $('body').addClass('lock');
                    } else {
                        mod.checkCommodity();
                    }
                } else {
                    //加入购物车
                    if ($('FastBuy').hasClass('hide')) {
                        mod.realPrice = mod.price;
                        mod.commodityUpInfo.realPrice = mod.price;
                    }
                    if (mod.hasAttribute) {
                        mod.selectCartNum = $('#modal-wrap-count').val(1);
                        /*初始化属性列表*/
                        mod.initColorSize();
                        //显示选择属性弹窗
                        $('#modal-wrap').removeClass('hide');
                        mod.currentColor = '';
                        mod.currentSize = '';
                        mod.currentSpecifications = '';
                        $('body').addClass('lock');
                    } else {
                        mod.selectCartNum = $('#modal-wrap-count').val();
                        mod.addShopCart();
                    }
                }
            } else {
                DealLoginPartial.initPartialPage();
            }
        });

        //分销
        $('.parameters').on('click', function () {
            if (isLogin()) {
                document.location.href = urlAppendCommonParams("/Distribute/CommodityDistribute?commodityId=" + getQueryString('commodityId') + "&distributorId=" + sessionStorage.distributorId);
            } else {
                DealLoginPartial.initPartialPage();
            }
        });

        $('#shop').on('click', function () {
            window.location.href = urlAppendCommonParams("/Mobile/CommodityList?type=" + sessionStorage.AppType + "&shopId=" + sessionStorage.appId);
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
	 * 实例化顶部商品banner
	 * @param el
	 */
    mod.initBanner = function (el) {
        new Swiper(el, {
            direction: 'horizontal',
            loop: true,
            autoplayDisableOnInteraction: false,
            pagination: '.swiper-pagination',// 如果需要分页器
            paginationType: 'custom',//分页器样式类型 1."bullets": 圆点（默认）2."fraction"：分式 3. "progressbar":进度条 4."custom" :自定义
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
	 * 实例化更多推荐滑动组件
	 */
    mod.intRecommend = function (el) {
        new Swiper(el, {
            spaceBetween: 0,
            pagination: '.swiper-pagination',// 如果需要分页器
            paginationType: 'custom',//分页器样式类型 1."bullets": 圆点（默认）2."fraction"：分式 3. "progressbar":进度条 4."custom" :自定义
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
            paginationClickable: true
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

    mod.initData = function (data) {
        document.title = data.Name;
        //保存商品信息
        mod.commodityUpInfo = {
            name: data.Name,
            pic: data.Pic,
            number: 1,
            price: data.Price,
            Intensity: data.Intensity,
            IsEnableSelfTake: data.IsEnableSelfTake,
            marketPrice: Math.abs(data.MarketPrice).toFixed(2),
            CommodityType: data.CommodityType,
            orderType: data.CommodityType,
            OutPromotionId: data.OutPromotionId || ''
        };
        mod.price = data.Price.toFixed(2);
        mod.DiscountPrice = data.DiscountPrice.toFixed(2); //优惠价
        mod.Intensity = data.Intensity; //折扣
        mod.outPromotionId = data.OutPromotionId;
        mod.commodityStockId = data.CommodityStockId;
        mod.limitBuyEach = data.limitBuyEach;
        mod.stock = data.Stock;
        mod.SkuActivityCdtos = data.SkuActivityCdtos;
        mod.JCActivityItemsListCdtos = data.JCActivityItemsListCdtos;
        mod.PromotionTypeNew = data.PromotionTypeNew;
        mod.DiyGroupPromotion = data.DiyGroupPromotion;
        mod.PromotionId = data.PromotionId;

        if (data.CommodityStocks && data.CommodityStocks.length) {
            mod.commodityStocks = data.CommodityStocks;
            for (var i = 0; i < data.CommodityStocks.length; i++) {
                if (mod.commodityStocks[i].Stock > 0) {
                    mod.DefaultCommodityList = mod.commodityStocks[i];
                    break;
                }
            }
        }
        else
            $('.sku').addClass('hide');


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

        //获取规格属性
        if (data.Specifications && data.Specifications.length > 0) {
            mod.hasAttribute = true;
            for (var i = 0; i < data.Specifications.length; i++) {
                var obj = {};
                obj.Id = data.Specifications[i].Id;
                obj.Name = data.Specifications[i].Name;
                obj.Attribute = data.Specifications[i].Attribute;
                obj.strAttribute = data.Specifications[i].strAttribute;
                mod.allSpecifications.push(obj);
            }
        }
        /*设置轮播组件*/
        mod.pictures = data.Pictures;
        mod.State = data.State;
        data.Pictures.length && mod.renderSwiper(data.Pictures, data.State, data.Stock);

        for (var i = 0; i < data.Pictures.length; i++) {
            $("#swiper-wrapper").append('<div class="swiper-slide"><a href="javascript:void(0);"><img src=' + data.Pictures[i].PicturesPath + '/></a></div>');
        }

        /*设置价格*/
        //this.setPriceRange(data);

        /*设置直播和720云景按钮*/
        mod.setBtn(data.EquipmentUrl, data.CloudviewUrl);
        // 展现 活动状态
        mod.showSateByPromotion(data);
        mod.thumImg = data.Pic;
        $('#pic').attr('src', data.Pic);
        data.AppName && $('#store-name').text(data.AppName);
        data.AppIcon && $('#store-logo').attr('src', data.AppIcon);

        /*设置 下载APP 三级分销*/
        DOWNLOADBANNER.setDistributeBanner(data.IsDistribute, data.AppId);

        //易捷币折扣
        if (data.YJBInfo && data.YJBInfo.Enabled) {
            mod.InsteadCashAmount = data.YJBInfo.InsteadCashAmount.toFixed(2);
        }
        else
            $('.activity-wrap').addClass('hide');

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

        /*销量*/
        //if (mod.resultIsVolume == true) {
        //    $('#d-adds').append('<span class="sales">销量: ' + data.Total + '</span>');
        //}

        /*价格+运费*/
        $('.goods-new-price').append('￥<em class="num">' + data.Price + '</em>');
        if (data.DiscountPrice > 0)
            $('.goods-old-price').text('￥' + data.DiscountPrice);
        else
            $('.goods-old-price').addClass('hide');

        if (data.freight < 0)
            $('.freight').html('快递:包邮');
        else
            $('.freight').html('运费: ￥' + Math.abs(data.Freight || 0) + (data.PostAge ? ',' + data.PostAge : ''));

        //秒杀详情隐藏优惠券和领券
        if (data.PromotionType != 0) {
            $('#promotion-modal').addClass('hide');
            $('#Voucher').addClass('hide');
        }
        else {
            var Promotion = '<li class="item">'
                + '     <span class="item-tag">{Category}</span>'
                + '     <span class="item-num">{Detailed}</span>'
                + '     <span class="item-text">{Conten}</span>'
                + '  </li>';
            var Promotion1 = "";
            var Promotion2 = "";

            //油卡data.YouKaInfo.Title
            if (data.YouKaInfo && data.YouKaInfo.GiveMoney > 0) {
                Promotion1 += Promotion.replace('{category}', "油卡券").replace('{Detailed}', data.YouKaInfo.GiveMoney + '元油卡兑换券:').replace('{Conten}', '购买并支付本商品后 即赠');//data.YouKaInfo.Message
                Promotion2 += Promotion.replace('{category}', "油卡券").replace('item-num', 'item-num hide').replace('{Conten}', data.YouKaInfo.Message);
                $('.promotion').removeClass('hide');
                $('#promotion-modal').removeClass('hide');
            }

            //易捷币
            if (data.YJBInfo && data.YJBInfo.Enabled) {
                Promotion1 += Promotion.replace('{category}', '易捷币').replace('{Detailed}', '折现' + data.YJBInfo.InsteadCashAmount + '元:').replace('{Conten}', '使用易捷币即可折现');
                Promotion2 += Promotion.replace('{category}', '易捷币').replace('item-num', 'item-num hide').replace('{Conten}', '可使用易捷币抵现' + data.YJBInfo.InsteadCashAmount + '元');
                $('.promotion').removeClass('hide');
                $('#promotion-modal').removeClass('hide');
            }

            //赠品
            if (data.Present) {
                Promotion1 += Promotion.replace('{category}', '赠品').replace('item-num', 'item-num hdie').replace('{Conten}', data.Present);
                Promotion2 += Promotion.replace('{category}', '赠品').replace('item-num', 'item-num hdie').replace('{Conten}', data.Present);
                $('.promotion').removeClass('hide');
                $('#promotion-modal').removeClass('hide');
            }
            $('#Viewpromotion').append(Promotion1);
            $('#Popuppromotion').append(Promotion2);
        }

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
        if (data.ServiceSettings !== null && data.ServiceSettings.length > 0) {
            for (var i = 0; i < data.ServiceSettings.length; i++) {

                $('.service-list-wrap').append(service.replace("{servicelist}", data.ServiceSettings[i].Title));

                $('.server-list').append(service1.replace("{servicetitle}", data.ServiceSettings[i].Title).replace("{servicecontent}", data.ServiceSettings[i].Content));
            }
        }
        else {
            $('.service-list-wrap').append(service.replace("{servicelist}", "暂无服务！"));
        }

        //优惠券
        //var coupon = '<span class="coupon-list-item">{content}</span>';
        if (data.Coupons.length > 0) {
            $.each(data.Coupons, function (i, j) {
                $('.coupon-lists').append('<span class="coupon-list-item">' + j + '</span>');
            });
        }
        else
            $('#Voucher').addClass('hide');

        //领取优惠券
        if (data.CouponList) {
            var coupon = '';
            data.CouponList.forEach(function (v) {
                coupon +=
                    '<li class="item">'
                    + '< div class="item-left" >'
                    + '<p class="price">￥<span class="num">' + v.Cash + '</span></p>'
                    + '<div class="text">' + v.Description + '</div>'
                    + '<div class="date">有效期至：' + mod.transformTime(v.EndTime) + '</div>'
                    + '</div >'
                    + '<div class="item-right">' +
                    (v.IsDraw ? '<a class="item-btn">已领取</a>' : '<a class="item-btn active" onclick=\'DETAIL.bindCoupon("' + v.Id + '",this)\' >领取</a>')
                    + '</div>'
                    + '</li >';
            });
            $('.coupon-list').append(coupon);
        }

        //商品详情售后
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

        /*设置优惠套餐*/
        var isMealActivity = false;
        if (data.MealActivityInfo && data.MealActivityInfo !== null) {
            $(".package-bottom").text(data.MealActivityInfo).removeClass('hide');
            isMealActivity = true;
        } else {
            $(".package-wrap").addClass('hide');
        }
        mod.setEvaluate(data.HasReviewFunction, data.Score);

        /*获取购物车数量*/
        isLogin() && mod.getShoppingCartNum();

        mod.initColorSize();

        //更多推荐
        //mod.MoreRecommend();

    };

    mod.renderSwiper = function (Pictures, State, Stock) {
        var html = "";
        var lunbopic = '<div class="swiper-slide"><a href="javascript:void(0);"><img src="{pic}"/></a></div>';
        for (var i = 0; i < Pictures.length; i++) {
            html += lunbopic.replace("{pic}", Pictures[i].PicturesPath);
        }
        $("#swiper-wrapper").html(html);
    };

    mod.setEvaluate = function (HasReviewFunction, Score) {
        var commodityId = sessionStorage.commodityId_2 || getQueryString('commodityId');
        var url = SNSUrl + "/Evaluate/List?appId=" + sessionStorage.appId + "&productId=" + commodityId;
        //商品评价
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
                        evaluateconfig += evaluate.replace("evaluate-pic", "evaluate-pic hide");
                    else
                        evaluateconfig += evaluate.replace("{compic}", Score.Records[i].PhotosArr[0]);

                    if (i > 4)
                        break;//评价显示5条
                }
                $('.swiper-wrapper').html(evaluateconfig + href.replace("{href}", url));
            } else {
                $("#evaluate").addClass('hide');
                $("#evalshow").addClass('hide');
                $("#evalhide").removeClass('hide');
            }
        } else { //定制平台没有配置“商品评价”组件
            $("#eval").addClass('hide');
        }
    };

    /**
    * 会员价促销
    */
    mod.setVipActive = function (VipPromotion) {
        if (VipPromotion.IsVipActive && VipPromotion.Intensity < 10) { //有促销活动
            $('.promotion').removeClass('hide'); //显示促销活动
            var promotion = '<li class="item">'
                + ' < span class="item-tag" >VIP</span >'
                + ' <span class="item-text" >{content}</span>'
                + '</li >';
            if (!isLogin()) { //登录
                if (VipPromotion.IsVip) { //是会员并且有会员折扣
                    var vipPrice = [];

                    vipPrice.push((mod.price * (VipPromotion.Intensity / 10)).toFixed(2));

                    mod.vipIntensity = VipPromotion.Intensity;
                    promotion.replace('{content}', '您可享受至尊VIP会员价￥' + vipPrice.join('~'));
                } else { //不是会员
                    promotion.replace('{content}', '您不是会员');
                }
            } else { //未登录
                promotion.replace('{content}', '登录可查看会员优惠信息');
            }
            $('#Viewpromotion').append(promotion);
            $('#Popuppromotion').append(promotion);
        }
    };

    mod.showSateByPromotion = function (data) {
        var nowDate = data.CurrentTime ? data.CurrentTime.split("(")[1].split(")")[0] : (new Date()).getTime(); //(new Date()).getTime();
        var lastTime = 0;
        // 距离抢购开始(距秒杀开始) 还剩的时间
        sessionStorage.promotionStartTime = mod.getRemainingTime(data.PromotionStartTime, lastTime, nowDate);
        lastTime = sessionStorage.promotionStartTime == 0 ? 0 : data.PromotionStartTime;
        // 距离抢购结束（距秒杀结束）还剩的时间
        sessionStorage.promotionEndTime = mod.getRemainingTime(data.PromotionEndTime, lastTime, nowDate);
        sessionStorage.appId = data.AppId;
        //PromotionState: 0：没有活动或已失效 ,1:预约预售进行中，2：等待抢购：3：活动进行中，4：活动已结束
        // 库存为0 表示抢光了
        var hasStock = true;
        if (data.Stock == 0 || (data.LimitBuyTotal > 0 && (data.LimitBuyTotal - data.SurplusLimitBuyTotal <= 0))) { //达到限购总数量
            hasStock = false;
        }
        switch (data.PromotionTypeNew) {
            case 9999: //无优惠
                /*促销-会员价*/ //只有无优惠（PromotionTypeNew == 9999）时才显示会员促销价
                mod.setVipActive(data.VipPromotion);
                break;
            case 1: // 秒杀
                SECKILL.updateSeckillState(data.PromotionState, hasStock);
                break;
        }
    };
    // 获取倒计时 剩余时间
    mod.getRemainingTime = function (serverTime, preTime, nowDate) {
        var resTime = 0;
        if (serverTime) {
            if (preTime == 0) {
                resTime = (serverTime.split("(")[1].split(")")[0] - nowDate) / 1000;
            } else {
                resTime = (serverTime.split("(")[1].split(")")[0] - (preTime.split("(")[1].split(")"))[0]) / 1000;
            }
            resTime = (resTime < 0) ? 0 : resTime;
        }
        return resTime;
    };
	/**
	 * 显示弹窗
	 * @param el
	 */
    mod.showModal = function (el) {
        $(el).removeClass('hide');
        $('body').addClass('clock');
    };
    /**
    * 更新属性选择弹窗中的价格
    */
    mod.updatePrice = function () {
        //var price = mod.price;
        var thumImg = mod.thumImg;
        //        //判断价格是否是区间值：如果是并且两种属性已经选择了，则显示对应的值
        //        if (mod.price.length == 2) { //价格是区间值
        if (mod.attributeTitle.length == 2) {
            if (mod.currentColor && mod.currentSize) { //两种属性已经选择了
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    var commodity = mod.commodityStocks[i];
                    if ((commodity.ComAttribute[0].SecondAttribute == mod.currentColor && commodity.ComAttribute[1].SecondAttribute == mod.currentSize) || (commodity.ComAttribute[1].SecondAttribute == mod.currentColor && commodity.ComAttribute[0].SecondAttribute == mod.currentSize)) {
                        mod.commodityUpInfo.price = commodity.Price;
                        mod.Stock = commodity.Stock;
                        if (commodity.Stock > 10)
                            $('#StockText').text('库存充足');
                        else
                            $('#StockText').text('库存紧张');

                        thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                        mod.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : [];
                        if (mod.vipIntensity != null) {
                            mod.price = (commodity.Price * (mod.vipIntensity / 10)).toFixed(2);
                        } else if (mod.Intensity < 10) { //有折扣
                            mod.price = (commodity.Price * (mod.Intensity / 10)).toFixed(2);
                        } else {
                            mod.price = commodity.Price.toFixed(2);
                        }
                        if (mod.DiyGroupPromotion != null && mod.PromotionId == null) {
                        } else {
                            if (mod.SkuActivityCdtos && mod.SkuActivityCdtos.length > 0) {
                                for (var j = 0; j < mod.SkuActivityCdtos.length; j++) {
                                    if (mod.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                        mod.DiscountPrice = mod.SkuActivityCdtos[j].JoinPrice.toFixed(2);
                                    }
                                }
                            } else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    mod.DiscountPrice;
                                }
                            }
                            //金采团购活动价格
                            if (mod.JCActivityItemsListCdtos && mod.JCActivityItemsListCdtos.length > 0) {
                                for (var j = 0; j < mod.JCActivityItemsListCdtos.length; j++) {
                                    if (mod.JCActivityItemsListCdtos[j].ComdtyStockId === commodity.Id) {
                                        mod.DiscountPrice = mod.JCActivityItemsListCdtos[j].GroupPrice.toFixed(2);
                                    }
                                }
                            } else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    mod.DiscountPrice;
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
                        mod.Stock = commodity.Stock;
                        if (commodity.Stock > 10)
                            $('#StockText').text('库存充足');
                        else
                            $('#StockText').text('库存紧张');
                        thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                        mod.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : [];
                        if (mod.vipIntensity != null) {
                            mod.price = (commodity.Price * (mod.vipIntensity / 10)).toFixed(2);
                        } else if (mod.Intensity < 10) { //有折扣
                            mod.price = (commodity.Price * (mod.Intensity / 10)).toFixed(2);
                        } else {
                            mod.price = commodity.Price.toFixed(2);
                        }
                        if (mod.DiyGroupPromotion != null && mod.PromotionId == null) {
                        } else {
                            if (mod.SkuActivityCdtos && mod.SkuActivityCdtos.length > 0) {
                                for (var j = 0; j < mod.SkuActivityCdtos.length; j++) {
                                    if (mod.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                        mod.DiscountPrice = mod.SkuActivityCdtos[j].JoinPrice.toFixed(2);
                                    }
                                }
                            }
                            else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    mod.DiscountPrice;
                                }
                            }
                            //金采团购活动价格
                            if (mod.JCActivityItemsListCdtos && mod.JCActivityItemsListCdtos.length > 0) {
                                for (var j = 0; j < mod.JCActivityItemsListCdtos.length; j++) {
                                    if (mod.JCActivityItemsListCdtos[j].ComdtyStockId === commodity.Id) {
                                        mod.DiscountPrice = mod.JCActivityItemsListCdtos[j].GroupPrice.toFixed(2);
                                    }
                                }
                            }
                            else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    mod.DiscountPrice;
                                }
                            }
                        }
                    }
                }
            }
        }
        //}

        if (mod.PromotionTypeNew == 0) {
            if (mod.DiscountPrice > 0) {
                $('.goods-new-price').html('￥<em class="num">' + mod.DiscountPrice + '</em>');
                $('.goods-old-price').text(mod.Price);
            }
            else {
                $('.goods-new-price').html('￥<em class="num">' + mod.Price + '</em>');
                $('.goods-old-price').addClass('hide');
            }

            if (!$('.activity-wrap').hasClass('hide')) {
                $('#OriginalPrice').text('<i>￥</i>' + mod.Price);
                $('#YJBPrice').text('<i>￥</i>' + mod.InsteadCashAmount);
                var daoshou = (mod.Price - mod.InsteadCashAmount).toFixed(2);
                $('#total-price').text(daoshou.substr(0, daoshou.indexOf('.')));
                $('#HandPrice').text('.' + daoshou.replace(/\d+\.(\d*)/, "$1"));
            }
        }
        else {
            $('#SeckillPrice').html('<i>￥</i>' + mod.DiscountPrice.substr(0, mod.DiscountPrice.indexOf('.')) + '.<em>' + mod.DiscountPrice.replace(/\d+\.(\d*)/, "$1") + '</em>');
            $('#OldSeckillPrice').text(mod.Price);
        }

        $('#DityPic').attr('src', thumImg);
        $('#AttributePrice').html('<span>' + mod.DiscountPrice + '</span>');

        $('.sku-list-item').text(mod.currentColor + '，' + mod.currentSize + '，' + $('#modal-wrap-count').val() + '件');
        mod.realPrice = mod.DiscountPrice;
        mod.commodityUpInfo.realPrice = mod.realPrice;
    };

    /**
    * 渲染属性列表
    * @param id 属性类别id
    * @param title 属性类别名称
    * @param data 属性列表
    * @param currentSelect 当前选中的属性值
    * @param disAttribute 禁用状态的属性
    * @returns {string}
    */
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

    //属性设置
    mod.renderSpecifications = function (data, currentSelect) {

        var html = '<div class="modal-content-bd-title">规格设置</div><ul  class="clearfix">';
        for (var j = 0; j < data.length; j++) {

            if (currentSelect == data[j].strAttribute) {
                html += '<li class="modal-content-bd-item active">' + data[j].strAttribute + '</li>';
            }
            else {
                html += '<li class="modal-content-bd-item">' + data[j].strAttribute + '</li>';
            }
        }
        return html + '</ul>';

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
    * 初始化属性列表
    */
    mod.initColorSize = function () {
        var html = '';
        if (mod.DefaultCommodityList) {
            for (var i = 0; i < mod.DefaultCommodityList.ComAttribute.length; i++) {
                mod.currentColor = mod.DefaultCommodityList.ComAttribute[0].SecondAttribute;
                if (i == 1) {
                    mod.currentSize = mod.DefaultCommodityList.ComAttribute[1].SecondAttribute;
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

    /**
   * 校验商品
   * @param color
   * @param size
   * @param count
   */
    mod.checkCommodity = function () {
        mod.closeModal();
        $('#mask').removeClass('hide').find('p').text('提交中...');
        var size = [];
        if (mod.currentColor) {
            size.push(mod.currentColor);
        }
        if (mod.currentSize) {
            size.push(mod.currentSize);
        }
        var specifications = 0;
        var arr = [];
        if (mod.currentSpecifications != null || mod.currentSpecifications != "") {
            var str = mod.currentSpecifications;
            arr = str.split('*');
            specifications = Number(arr[1]);
        }
        var checkdata = {
            diyGroupId: '00000000-0000-0000-0000-000000000000',
            promotionType: -1,
            UserID: getUserId(),
            jcActivityId: getQueryString('jcActivityId'),
            CommodityIdAndStockIds: [{
                CommodityId: getQueryString('commodityId'),
                CommodityStockId: mod.commodityStockId,
                OutPrommotionId: mod.outPrommotionId,
                SizeAndColorId: (mod.currentColor ? mod.currentColor : '') + ',' + (mod.currentSize ? mod.currentSize : ''), //color属性 size属性
                Specifications: specifications
            }]
        };
        if (checkdata.jcActivityId == null) {
            checkdata.jcActivityId = '00000000-0000-0000-0000-000000000000';
        }
        if (checkdata.CommodityIdAndStockIds[0].CommodityStockId === "00000000-0000-0000-0000-000000000000") {
            checkdata.CommodityIdAndStockIds[0].CommodityStockId = getQueryString("commodityStockId");
        }
        //获取数据.
        $.ajax({
            url: '/Mobile/CheckCommodity',
            dataType: 'json',
            contentType: "application/json",
            type: 'POST',
            data: JSON.stringify(checkdata)
        }).done(function (data) {
            $('#mask').addClass('hide');
            if (!data.length) {
                toast('服务器繁忙,请稍候再试');
                return;
            }
            if (data[0].State == 1) {
                toast('抱歉，该商品已下架');
                return;
            }
            if (data[0].State == 3) {
                toast('商品已删除，不能购买');
                return;
            }
            if (data[0].IsNeedPresell && !data[0].IsPreselled) {
                toast('未预约，不能购买');
                return;
            }
            if (data[0].Stock <= 0) {
                toast('购买数量超出范围~');
                return;
            }
            if (mod.selectCartNum > data[0].Stock || (data[0].LimitBuyEach > 0 && mod.selectCartNum > data[0].LimitBuyEach) || (data[0].LimitBuyTotal > 0 && mod.selectCartNum > (data[0].LimitBuyTotal - data[0].SurplusLimitBuyTotal))) {
                //1.库存不足 2.单人购买超出限购数量 3.限购数量不足
                toast('购买数量超出范围');
                return;
            }
            if (data[0].Price.toFixed(2) != mod.realPrice) { //商品价格发生变化
                $('#myModal').show();
                $('#modal-body').text('商品价格变成￥' + data[0].Price.toFixed(2) + ',是否继续购买');
                mod.commodityUpInfo.realPrice = data[0].Price.toFixed(2);
                return;
            }
            sessionStorage.Stock = data[0].Stock;
            mod.commodityUpInfo.number = mod.selectCartNum;
            mod.commodityUpInfo.size = mod.currentColor;
            mod.commodityUpInfo.color = mod.currentSize;
            mod.commodityUpInfo.commodityStockId = mod.commodityStockId;
            mod.commodityUpInfo.currentSpecifications = mod.currentSpecifications;
            mod.commodityUpInfo.Specifications = specifications;
            if (mod.commodityStockId === "00000000-0000-0000-0000-000000000000") {
                mod.commodityStockId = getQueryString("commodityStockId");
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    if (mod.commodityStocks[i].Id === mod.commodityStockId) {
                        if (mod.commodityStocks[i].ComAttribute.length > 1) {
                            mod.commodityUpInfo.size = mod.commodityStocks[i].ComAttribute[0].SecondAttribute;
                            mod.commodityUpInfo.color = mod.commodityStocks[i].ComAttribute[1].SecondAttribute;
                        } else {
                            mod.commodityUpInfo.size = mod.commodityStocks[i].ComAttribute[0].SecondAttribute;
                        }
                    }
                }
            }
            sessionStorage.setItem('commodityUpInfo', JSON.stringify(mod.commodityUpInfo)); //缓存商品信息，下订单页面需要
            var reg = new RegExp("(^|&)appId=([^&]*)(&|$)", "i");
            var a = window.location.href.replace(/\&amp;/g, "&");
            var r = a.substr(1).match(reg);
            var url = urlAppendCommonParams('/Mobile/CreateOrder?esAppId=' + (r ? unescape(r[2]) : '') + '&type=goumai');
            if (getQueryString("jcActivityId") != null) {
                url = urlAppendCommonParams('/Mobile/CreateOrder?jcActivityId=' + getQueryString("jcActivityId") + '&esAppId=' + (r ? unescape(r[2]) : '') + '&type=goumai');
            }
            window.location.href = url;
        }).fail(function (e) {
            $('#mask').addClass('hide');
            toast('服务器繁忙,请稍候再试');
        });
    };

    /**
    * 加入购物车
    * @param color color属性
    * @param size  size数字能够
    * @param count 数量
    */
    mod.addShopCart = function () {
        mod.closeModal();
        $('#mask').removeClass('hide').find('p').text('提交中。。。');

        var specifications = 0;
        var arr = [];
        if (mod.currentSpecifications != null || mod.currentSpecifications != "") {
            var str = mod.currentSpecifications;
            arr = str.split('*');
            specifications = Number(arr[1]);
        }
        var checkdata = {
            CommodityId: getQueryString('commodityId'), //商品ID
            CommodityNumber: mod.selectCartNum, //商品数量
            SizeAndColorId: (mod.currentColor ? mod.currentColor : '') + ',' + (mod.currentSize ? mod.currentSize : ''), //color属性 size属性
            UserId: getUserId(),
            AppId: sessionStorage.appId,
            CommodityStockId: mod.commodityStockId,
            EsAppId: getEsAppId(),
            JcActivityId: getQueryString('jcActivityId'),
            Specifications: specifications
        }
        $.ajax({
            url: '/Mobile/SaveShoppingCart',
            type: 'POST',
            dataType: 'json',
            contentType: "application/json",
            data: JSON.stringify(checkdata)
        }).done(function (data) {
            $('#mask').addClass('hide');
            if (data.ResultCode == 0) {
                mod.shoppingCartNum = mod.shoppingCartNum - 0 + mod.selectCartNum;
                var num = mod.shoppingCartNum;
                if (num > 99) {
                    num = '99+';
                }
                $('#shoppingCount').text(num);
                $('#shoppingCount').hasClass('hide') && $('#shoppingCount').removeClass('hide');
                if (mod.carouselImgs.length) {
                    mod.picDefault = true;
                    mod.renderSwiper(mod.carouselImgs, mod.State, mod.stock);
                } else if (mod.picChange) {
                    //显示默认的轮播图
                    mod.renderSwiper(mod.Pictures, mod.State, mod.stock);
                    mod.picDefault = false;
                }
                toast('加入购物车成功');
            } else {
                toast(data.Message);
            }
        }).fail(function (e) {
            $('#mask').addClass('hide');
            toast('加入购物车失败,请重试!');
        });
    };

    /**
    * 更新规格设置状态
    * @param target 点击的目标元素
    * @param selectAttr 点击的目标元素所在的属性分类
    */
    mod.updateSpecifications = function (target) {
        try {
            var html = '';
            if ($(target).hasClass('active')) { //取消选中的属性
                if (mod.allSpecifications.length) {
                    html += mod.renderSpecifications(mod.allSpecifications, '');
                }

            } else { //选择属性
                if (mod.allSpecifications.length) {
                    mod.currentSpecifications = $(target).text();
                    html += mod.renderSpecifications(mod.allSpecifications, mod.currentSpecifications);
                }

            }
            $('#Specifications').empty();
            $('#Specifications').append(html);
        } catch (e) { };
        return;
    };

    /**
    * 联系客服
    */
    $('#customer').on('click', function () {

        var appid = mod.contactObj ? sessionStorage.appId : (getEsAppId() || sessionStorage.appId);
        //智力圈的情况
        if (appid == "630af8fc-41e9-4bd1-a436-2dd4197f076b" || appid == "cf063155-e6e9-4019-ba12-6b44b704243f") {
            return false;
        }
        else {
            DealLoginPartial.initPartialPage();
            var contactAppId = mod.contactObj ? sessionStorage.appId : (getEsAppId() || sessionStorage.appId);
            if (JsVilaDataNull(mod.contactUrl) && mod.contactUrl.indexOf("http") >= 0) {
                window.location.href = mod.contactUrl;
            } else if (sessionStorage.source == "share") {
                var href = webImUrl.replace('{0}', contactAppId);
                window.location.href = href;
            } else {
                try {
                    if ($.os.ios) { //ios
                        window.location.href = "/Mobile/ContactAppOwner?type=goutong&appId=" + contactAppId;
                    } else { //andorid
                        window.contactStoreOwner.startServiceList(contactAppId);
                    }
                } catch (e) {
                    toast("商家暂不支持此功能~");
                }
            }
        }

    });

    /**
    * 领取优惠券
    */
    mod.bindCoupon = function (conponId, el) {
        $('#mask').removeClass('hide').find('p').text('领取中...');
        $.ajax({
            url: '/Mobile/BindCouponInCommodityDetails',
            type: 'post',
            dataType: 'json',
            data: {
                EsAppId: getEsAppId(),
                BindUserId: getUserId(),
                ConponTemplateId: conponId
            },
            async: true
        }).done(function (data) {
            $('#mask').addClass('hide');
            if (data.IsSuccess) {
                toast('优惠券领取成功。');
                $(el).removeClass('active').text('已领取');
                $(el).parent().prev().addClass("used");
            } else {
                toast(data.Info);
            }
        }).fail(function (err) {
            toast('优惠券领取失败，请稍后重试!');
            $('#mask').addClass('hide');
        });
    };

    mod.transformTime = function (time) {
        var date = new Date(parseInt(time.split('Date(')[1].split(')/')[0]));
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        return year + '年' + month + '月' + day + '日';
    }

    /**
    * 设置直播和720云景按钮
    */
    mod.setBtn = function (EquipmentUrl, CloudviewUrl) {
        
        EquipmentUrl && $('#store-info').append('<a href="' + EquipmentUrl + '" class="opera-btn">直播</a>');
        CloudviewUrl && $('#store-info').append('<a href="' + CloudviewUrl + '" class="opera-btn">720°云景</a>');
    };

    /**
     * 获取购物车数量
     */
    mod.getShoppingCartNum = function () {
        $.ajax({
            url: '/Mobile/GetShoppingCartNum',
            dataType: 'json',
            type: 'GET',
            data: {
                userId: getUserId(),
                esAppId: getEsAppId()
            }
        }).done(function (data) {
            if (data.Code == 0 && data.Data > 0) {
                mod.shoppingCartNum = data.Data;
                var count = data.Data;
                if (count > 99) {
                    count = '99+';
                }
                $('#shoppingCount').text(count);
                $('#shoppingCount').hasClass('hide') && $('#shoppingCount').removeClass('hide');
            }
        })
    };

    /**
    * 倒计时
    */
    mod.countdown = function (time, callback) {
        var day = 0,
            hour = 0,
            minute = 0,
            second = 0; //时间默认值
        var interval = window.setInterval(function () {
            var intDiff = parseInt(time);
            if (intDiff >= 1) {
                day = Math.floor(intDiff / (60 * 60 * 24));
                hour = Math.floor(intDiff / (60 * 60)) - (day * 24);
                minute = Math.floor(intDiff / 60) - (day * 24 * 60) - (hour * 60);
                second = Math.floor(intDiff) - (day * 24 * 60 * 60) - (hour * 60 * 60) - (minute * 60);
                if (minute <= 9) minute = '0' + minute;
                if (second <= 9) second = '0' + second;
                --time;
            } else {
                clearInterval(interval);
                callback();
                return;
            }
            if (day) {
                //timeElement.text(day + '天');
                $('#hour').text(day);
                $('#minute').text('天');
                $('#second').addClass('hide');
                $('.countdown').replace(':', '');

            } else {
                //timeElement.text(hour + ':' + minute + ':' + second);
                $('#hour').text(hour);
                $('#minute').text(minute);
                $('#second').text(second);
            }
        }, 1000);
    };

    /**
     * 更多推荐
     */
    //mod.MoreRecommend = function () {
    //    $.ajax({
    //        url: '/Mobile/GetMoreRecommend',
    //        dataType: 'json',
    //        type: 'post',
    //        data: {
    //            Fanye: 0,
    //            RankNo: 0,
    //            ItemId: getQueryString("commodityId"),
    //            esAppId: getEsAppId()
    //        }
    //    }).done(function (data) {
    //        if (data) {
    //            var more = '<li class="recommend-item" onclick="{setMgr}">'
    //                + '< img class="item-pic" src = "{pic}" >'
    //                + '<p class="item-title">{title}</p>'
    //                + '<div class="item-price">￥<span class="price">{price}</span></div>'
    //                + '</li >';
    //            var recommend = "";
    //            var j = 0;
    //            for (var i = 0; i < data.length; i++) {
    //                if (j == 6) {
    //                    $('#MoreR').append('<ul class="swiper-slide">' + recommend + '</ul>');
    //                    j = 0;
    //                }
    //                recommend += more.replace('{setMgr}', "GoCommodityDetail('" + data[i].Id + "','" + appId + "')").more.replace('{pic}', data[i].Pic).replace('{title}', data[i].Name).replace('{pic}', data[i].Price);
    //                j++;
    //            }
    //        }
    //        else {
    //            $('.recommend-wrap').addClass('hide');
    //        }
    //    })
    //};

	/**
	 * 隐藏躺床
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
    mod.updateSpinner = function (count, stock) {
        stock = mod.stock || 9999;
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
    mod.updateFontSize = function () {
        var num = $('#total-price').text().length || 0;
        var f;
        switch (num) {
            case 0:
            case 1:
            case 2:
                f = 'f70';
                break;
            case 3:
                f = 'f60';
                break;
            case 4:
                f = "f50";
                break;
            case 5:
                f = "f40";
                break;
            default:
                f = "f30";
                break;
        }
        document.getElementById('total-price').className = f;
    };
    return mod;
})(DETAIL || {}, Zepto);

/**
* 下载 APP Banner
*/
var DOWNLOADBANNER = (function (mod, $) {
    /**
     * app download banner
     */
    mod.setDownloadBanner = function (AppDownLoadInfo) {
        if (sessionStorage.source == "share" && AppDownLoadInfo && JsVilaDataNull(AppDownLoadInfo.Icon)) {
            $("#esAppLogo").attr("src", AppDownLoadInfo.Icon);
            if (JsVilaDataNull(AppDownLoadInfo.PromotionDownGuide)) {
                $("#esAppDesc").html(AppDownLoadInfo.PromotionDownGuide);
            }
            $("#dwEsAppId").show();
        }
        $('#closeDwEsAppId').on('click', function () {
            $('#dwEsAppId').hide();
        });
        $('#dolEsAppId').on('click', function () {
            window.location.href = BTPAppresUrl + '/app/GetAppDetail?appId=' + getEsAppId();
        });
    };
    /**
     * app download banner (三级分销)
     * @param IsDistribute
     * @param AppId
     */
    mod.setDistributeBanner = function (IsDistribute, AppId) {
        if (IsDistribute && AppId == getEsAppId() && sessionStorage.source == 'share') {
            $("#divRegOrDownloadTip").show();
            $("#dwEsAppId").hide();
            $("#divRegOrDownloadTip").on("click", function (event) {
                if ($(event.srcElement).is("a")) {
                    $("#divRegOrDownloadTip").hide();
                } else {
                    if (JsVilaDataNull(getUserId())) {
                        document.location.href = "/Distribute/AppDetail?appId=" + getEsAppId() + "&distributorId=";
                    }
                }
            });
        }
    };
    return mod;
})(DOWNLOADBANNER || {}, window.Zepto);

/**
 * 秒杀详情
 */
window.SECKILL = (function (mod, $) {
    /**
     * 秒杀状态设置
     */
    mod.updateSeckillState = function (promotionState, hasStock) {
        switch (promotionState) {
            case 2: //2：等待抢购：
                //显示倒计时
                $('#SoonBuy').removeClass('hide');
                $('#FastBuy').addClass('hide');
                DETAIL.countdown(sessionStorage.promotionStartTime, function () {
                    mod.updateSeckillState(3, hasStock);
                });
                break;
            case 3: //3：活动进行中，
                if (hasStock) {
                    $('#FastBuy').removeClass('hide');
                    $('#SoonBuy').addClass('hide');
                } else {
                    $('#FastBuy').addClass('hide');
                    $('#addShop').addClass('gray,disabled');
                    $('#SoonBuy').addClass('hide');
                }
                DETAIL.countdown(sessionStorage.promotionEndTime, function () {
                    mod.updateSeckillState(4);
                });
                break;
            case 4:
                $('#FastBuy').addClass('hide');
                $('#addShop').addClass('gray,disabled');
                $('#SoonBuy').addClass('hide');
                break;
        }
    };
    return mod;
})(window.SECKILL || {}, window.Zepto);

$(document).ready(function () {
    DETAIL.loadData();
});