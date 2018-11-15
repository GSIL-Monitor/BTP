(function ($) {
    // "use strict";
    //商品字段
    var comdityField = {
        ServiceSettings: null,//店铺服务
        PinCommodityList: null,//默认选中的sku属性
        hasAttributes: false, //商品是否有属性
        attributeTitle: [], //属性类别名称
        allColor: [], //color属性所有值
        allSize: [], //size属性所有值
        allSpecifications: [], //规格设置
        currentColor: "", //color属性
        currentSize: "", //size 属性
        currentSpecifications: "", //规格属性
        commodityStocks: [], //多属性数据
        Duty: [], //关税
        DiscountPrice: null, //优惠价
        Intensity: null, //折扣
        defaultPrice: null,
        price: [],
        oldPrice: [],
        outPromotionId: null, //外部优惠活动id
        commodityStockId: null, //属性ID ,多属性商品中的属性组合ID，单属性和无属性商品中的属性ID（都是00000000-0000-0000-0000-000000000000）
        limitBuyEach: null,
        stock: 0, //库存
        shoppingCartNum: 0, //购物车数量
        selectCartNum: 1,
        realPrice: null,
        contactObj: null,
        contactUrl: null,
        commodityUpInfo: {}, //商品信息
        resultIsVolume: false, //是否配置了销量
        vipIntensity: null, //会员折扣
        SkuActivityCdtos: null, //会员折扣
        JCActivityItemsListCdtos: null, //金采团购
        PromotionType: -1, //活动类型
        DiyGroupPromotion: null, //拼团信息
        PromotionId: null, //活动id

        thumImg: '', //默认缩略图
        pictures: [], //默认轮播图
        carouselImgs: [], //某一属性的轮播图
        picDefault: true, //当前是默认的轮播图
        State: '',
    };
    var Page = {
        //获取校验码
        GetValidCode: function () {
            $("#validCodeImg").attr("src", "/Mobile/GetVerifyCodeZPH?" + Math.random());
        },
        flex: function () {
            var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
            document.documentElement.style.fontSize = deviceWidth / 7.5 + 'px';
        },//控制页面大小
        isScroll: true,
        setProvince: function () {
            try {
                if (JsVilaDataNull(sessionStorage.province)) {
                    return;
                }
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
                });
            } catch (e) {
                sessionStorage.province = "北京市";
            }

        },  //读取新浪接口返回数据设置当前所在省
        updateNav: function (index) {
            $('.detail-nav').removeClass('selected');
            $('.detail-nav').eq(index).addClass('selected');
        },
        showModal: function (el) {
            $(el).removeClass('hide');
            $('body').addClass('clock');
        },
        hideModal: function (el) {
            $(el).addClass('hide');
            $('body').removeClass('clock');
        },
        updateSpinner: function (count, stock) {
            stock = stock || 9999;
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
        },
        renderSwiper: function (pictures, state, stock) {
            var html = '';
            var paddingtop = '75%';
            for (var i = 0; i < pictures.length; i++) {
                var flag = false;
                var number = 1;
                var image = new Image();
                if (typeof pictures[i] == 'string') {
                    image.src = pictures[i];
                } else if (pictures[i].PicturesPath) {
                    image.src = pictures[i].PicturesPath;
                }
                html += '<div class="swiper-slide"><a class="swiper-slide-pic" href="javascript:void(0);"><img src="' + image.src + '"/></a></div>';
                image.onload = function () {
                    if (this.src == pictures[0].PicturesPath && (this.height == this.width)) {
                        paddingtop = '100%';
                        flag = true;
                    }
                    if (number == pictures.length) {
                        $('#swiper-wrapper').html(html);
                        $('.swiper-slide-pic').css('padding-top', paddingtop);
                        flag && Page.upadteContentHeight($('#page1').height());
                        if (state == 1) { //0：上架；1: 下架
                            $("#robbed").removeClass('hide').text("已下架");
                            $('#add,#buyBtn').addClass('disabled');
                        } else {
                            if (stock <= 0) {
                                $("#robbed").removeClass('hide').text("已售罄");
                                $('#add,#buyBtn').addClass('disabled');
                            }
                        }
                        if (stock >= 1 && state == 0) {
                            Page.initSwiper('#swiper', pictures.length);
                        }
                        if (!$('#page2').hasClass('hide')) {
                            $('#summary').css('transform', 'translateY(-' + $('#page1').height() + 'px)');
                        }
                        return;
                    }
                    number++;
                }
            }
        },//商品图片连接拼装成img标签插入页面
        setEvaluate: function (HasReviewFunction, Score) {
            var commodityId = sessionStorage.commodityId_2 || getQueryString('commodityId');
            var url = SNSUrl + "/Evaluate/List?appId=" + sessionStorage.appId + "&productId=" + commodityId;
            //$('#more').attr('href', url);
            var evaluate = '<div class="swiper-slide">'
                + '  <div class="evaluate-text">'
                + '      <div class="evaluate-text-hd">'
                + '          <img class="head" src="{userpic}"  >'
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
                    $('#evaluate').html('<div class="swiper-wrapper">' + evaluateconfig + href.replace("{href}", url) + '</div>');
                } else {
                    $("#evaluate").addClass('hide');
                    $("#evalshow").addClass('hide');
                    $("#evalhide").removeClass('hide');
                }
            } else { //定制平台没有配置“商品评价”组件
                $("#eval").addClass('hide');
            }
        },//商品评价
        init: function () {
            Page.flex();
            window.onresize = function () {
                Page.flex();
            };
            Page.loadData();
        },//初始化
        initEvent: function () {
            $('.header-content').on('click', '.detail-nav', function () {
                var state = $(this).data('type');
                var header = $('#header').height();
                var top = $('#' + state).offset().top - header + 2;
                Page.isScroll = false;
                document.documentElement.scrollTop = top;
                Page.isScroll = true;
            });
            $(window).on('scroll', function () {
                if (!Page.isScroll) {
                    return;
                }
                var header = $('#header').height();
                var height = document.documentElement.scrollTop;
                var detail = $('#detail').offset().top;//详情
                var eval = $('#eval').offset().top;//评价
                if (height > detail - header) {
                    Page.updateNav(2);
                } else if (height < detail - header && height >= eval - header) {
                    Page.updateNav(1);
                } else {
                    Page.updateNav(0);
                }
            });
            $('.modal-wrap').on('click', function (event) {
                var target = event.target;
                var flag = $(target).hasClass('modal-wrap');
                if (flag) {
                    var id = $(target).attr('id');
                    Page.hideModal('#' + id);
                }
            });
            $('#modal-wrap-increase').on('click', function () {
                var number = $('#modal-wrap-count').val() - 0;
                number++;
                Page.updateSpinner(number);
            });
            $('#modal-wrap-decrease').on('click', function () {
                var number = $('#modal-wrap-count').val() - 0;
                if (number > 1) {
                    number--;
                    Page.updateSpinner(number);
                }
            });
            $('#modal-wrap-count').on('change', function () {
                var number = parseInt($(this).val() - 0);
                if (number < 1) {
                    number = 1;
                }
                Page.updateSpinner(number);
            });
            //立即预约
            $(".LJYY").on('click', function () {
                if (!sessionStorage.commodityStockId) {//没选择商品属性
                    DETAIL.showModal('#size-modal')//弹出属性选择窗口
                    return;
                }
                if (isLogin()) {
                    if (Page.isPresale() == true) {
                        //已经预约过
                        $("#confirm-text").text("您已经成功预约过了，无需重复预约，请关注抢购时间~~~");
                        $("#confirm").removeClass('hide');
                        return;
                    } else {
                        Page.showModal('#identify-modal');//弹出验证码窗口
                    }
                } else {
                    DealLoginPartial.initPartialPage();
                }
            });
            //我的预约按钮
            $('#appointment-btn-submit').on('click', function () {
                window.location = zphUrl + "Presell/MyPresellComdty?userId=" + getUserId() + "&sessionId=" + getSessionId() + "&ChangeOrg=00000000-0000-0000-0000-000000000000&btpdomain=" + window.location.host;
            });
            //提交
            $("#Submit").on('click', function () {
                if ($("#validCodeText").val() != "") {
                    debugger
                    //检查输入的验证码
                    $.ajax({
                        url: '/mobile/SaveMyPresellComdtyZPH',
                        data: {
                            outPromotionId: getQueryString("outPromotionId"),
                            userId: getUserId(),
                            sessionId: getSessionId(),
                            verifyCode: $("#validCodeText").val(),
                            esAppId: getEsAppId(),
                            commodityId: getQueryString('commodityId'),
                            commodityStockId: sessionStorage.commodityStockId
                        },
                        async: false,
                        success: function (result) {
                            //0，成功，1 已预约过，2 失败
                            if (result.ResultCode === 2) {
                                toast(result.Message);
                            } else if (result.ResultCode === 0) {
                                //已经预约过
                                Page.hideModal('#identify-modal');//弹出验证码窗口
                                $("#confirm-text").text("商品预约成功，您已经获得抢购资格，请关注抢购时间~~~");
                                $("#confirm").removeClass('hide');

                            }
                        },
                        error: function () {
                            $('#mask').addClass('hide');
                            toast('服务器繁忙，请稍后重试！');
                        }
                    });
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
            $('#customer').on('click', function () {
                var appid = comdityField.contactObj ? sessionStorage.appId : (getEsAppId() || sessionStorage.appId);
                //智力圈的情况
                if (appid == "630af8fc-41e9-4bd1-a436-2dd4197f076b" || appid == "cf063155-e6e9-4019-ba12-6b44b704243f") {
                    $("#Zhiliquan").show();
                    return false;
                }
                else {
                    $("#Zhiliquan").hide();
                    DealLoginPartial.initPartialPage();
                    var contactAppId = comdityField.contactObj ? sessionStorage.appId : (getEsAppId() || sessionStorage.appId);
                    if (JsVilaDataNull(comdityField.contactUrl) && comdityField.contactUrl.indexOf("http") >= 0) {
                        window.location.href = comdityField.contactUrl;
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
            //店铺服务
            $('#dpfw').on('click', function () {
                if (comdityField.ServiceSettings !== null && comdityField.ServiceSettings.length > 0) {
                    DETAIL.showModal('#server-modal')
                }
            });
            ////验证码
            $("#validCodeImg").click(function () {
                Page.GetValidCode();
            });
        },//绑定事件
        initEvaluate: function (el) {
            new Swiper(el, {
                slidesPerView: 'auto',
                spaceBetween: 0,
                resistanceRatio: 0.5,
                freeMode: true
            });
        },//评价滚动
        initSwiper: function (el, number) {
            var autoplay = number == 1 ? null : 5000;
            Page.mySwiper = new Swiper(el, {
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
                autoplay: autoplay
            });
            number == 1 && Page.mySwiper.detachEvents();
        },//商品图片轮播
        upadteContentHeight: function (value) {
            $('#content').height(value + 50);
        },//更新显示区域的高度
        renderColorSize: function (id, title, data, currentSelect, disAttribute) {
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
        },
        initColorSize: function () {
            var html = '';
            if (comdityField.allColor.length) {
                html += Page.renderColorSize('color', comdityField.attributeTitle[0], comdityField.allColor, comdityField.currentColor, []);
            }
            if (comdityField.allSize.length) {
                html += Page.renderColorSize('size', comdityField.attributeTitle[1], comdityField.allSize, comdityField.currentSize, []);
            }
            $('#modal-bd').append(html);
            Page.updatePrice();
            //属性选择事件绑定
            $('#size-modal').on('click', '.size-item', function (event) {
                var target = event.target;
                if ($(target).hasClass('disable')) {
                    return;
                } else {
                    if ($(target).parent().parent()[0].id == 'color') {
                        comdityField.currentClick = $(target).text();
                        Page.updateColorSize(target, 'color');
                    } else if ($(target).parent().parent()[0].id == 'size') {
                        comdityField.currentClick = $(target).text();
                        Page.updateColorSize(target, 'size');
                    }
                    Page.updatePrice();
                }
            });
        },
        updateColorSize: function (target, selectAttr) {
            try {
                var html = '';
                if ($(target).hasClass('selected')) { //取消选中的属性
                    if (comdityField.allColor.length) {
                        if (selectAttr == 'color') {
                            comdityField.currentColor = '';
                            html += comdityField.renderColorSize('color', comdityField.attributeTitle[0], comdityField.allColor, comdityField.currentColor, Page.getDisableAttr('color'));
                        } else {
                            html += comdityField.renderColorSize('color', comdityField.attributeTitle[0], comdityField.allColor, comdityField.currentColor, []);
                        }
                        $('#color').empty();
                        $('#color').append(html);
                    }
                    var html = '';
                    if (comdityField.allSize.length) {
                        if (selectAttr == 'size') {
                            comdityField.currentSize = '';
                            html += comdityField.renderColorSize('size', comdityField.attributeTitle[1], comdityField.allSize, comdityField.currentSize, Page.getDisableAttr('size'));
                        } else {
                            html += comdityField.renderColorSize('size', comdityField.attributeTitle[1], comdityField.allSize, comdityField.currentSize, []);
                        }
                        $('#size').empty();
                        $('#size').append(html);
                    }
                } else { //选择属性
                    if (comdityField.allColor && comdityField.allColor.length) {
                        if (selectAttr == 'color') {
                            comdityField.currentColor = $(target).text();
                            html += Page.renderColorSize('color', comdityField.attributeTitle[0], comdityField.allColor, comdityField.currentColor, Page.getDisableAttr('color'));
                        } else {
                            html += Page.renderColorSize('color', comdityField.attributeTitle[0], comdityField.allColor, comdityField.currentColor, Page.getDisableAttr(comdityField.commodityStocks, comdityField.attributeTitle[1]));
                        }
                        $('#color').empty();
                        $('#color').append(html);
                    }
                    var html = '';
                    if (comdityField.allSize && comdityField.allSize.length) {
                        if (selectAttr == 'size') {
                            comdityField.currentSize = $(target).text();
                            html += Page.renderColorSize('size', comdityField.attributeTitle[1], comdityField.allSize, comdityField.currentSize, Page.getDisableAttr('size'));
                        } else {
                            html += Page.renderColorSize('size', comdityField.attributeTitle[1], comdityField.allSize, comdityField.currentSize, Page.getDisableAttr(comdityField.commodityStocks, comdityField.attributeTitle[0]));
                        }
                        $('#size').empty();
                        $('#size').append(html);
                    }
                }
            } catch (e) { };
            return;
        },
        getDisableAttr: function (param, attributeTitle) {
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
        },
        updatePrice: function () {
            var thumImg = comdityField.thumImg;
            //如果是并且两种属性已经选择了，则显示对应的值
            if (comdityField.attributeTitle.length == 2) {
                if (comdityField.currentColor && comdityField.currentSize) { //两种属性已经选择了
                    for (var i = 0; i < comdityField.commodityStocks.length; i++) {
                        var commodity = comdityField.commodityStocks[i];
                        if ((commodity.ComAttribute[0].SecondAttribute == comdityField.currentColor && commodity.ComAttribute[1].SecondAttribute == comdityField.currentSize) || (commodity.ComAttribute[1].SecondAttribute == comdityField.currentColor && commodity.ComAttribute[0].SecondAttribute == comdityField.currentSize)) {
                            comdityField.commodityUpInfo.price = commodity.Price;
                            thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                            comdityField.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : [];
                            commodity.Price = commodity.Price;
                            comdityField.stock = commodity.Stock;
                            comdityField.commodityUpInfo.stock = comdityField.stock;
                            comdityField.commodityUpInfo.duty = commodity.Duty;
                            sessionStorage.commodityStockId = commodity.Id;

                            if (commodity.Stock < 10)
                                $('#IsStock').html('库存紧张');
                            else
                                $('#IsStock').html('库存充足');

                            if (comdityField.DiyGroupPromotion != null && comdityField.PromotionId == null) {
                                if (comdityField.SkuActivityCdtos && comdityField.SkuActivityCdtos.length > 0) {
                                    for (var j = 0; j < comdityField.SkuActivityCdtos.length; j++) {
                                        if (comdityField.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                            sessionStorage.commodityStockId = commodity.Id;
                                            comdityField.HDPrice = mod.SkuActivityCdtos[j].JoinPrice;
                                            comdityField.commodityUpInfo.CommodityStockId = comdityField.SkuActivityCdtos[j].CommodityStockId;

                                        }
                                    }
                                } else {
                                    if (comdityField.DiscountPrice > -1) {
                                    }
                                }
                            }
                        }
                    }
                    $(".sku-list-item").html(comdityField.currentColor + "," + comdityField.currentSize);
                }
            } else if (comdityField.attributeTitle.length == 1) {
                if (comdityField.currentColor && comdityField.commodityStocks.length) {
                    for (var i = 0; i < comdityField.commodityStocks.length; i++) {
                        var commodity = comdityField.commodityStocks[i];
                        if (commodity.Stock < 10)
                            $('#IsStock').html('库存紧张');
                        else
                            $('#IsStock').html('库存充足');
                        comdityField.stock = commodity.Stock;
                        comdityField.commodityUpInfo.duty = commodity.Duty;
                        comdityField.commodityUpInfo.stock = comdityField.stock;
                        if (commodity.ComAttribute[0].SecondAttribute == comdityField.currentColor) {
                            comdityField.commodityUpInfo.price = commodity.Price;
                            thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                            comdityField.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : []
                            if (comdityField.DiyGroupPromotion != null && comdityField.PromotionId == null) {
                                if (comdityField.SkuActivityCdtos && comdityField.SkuActivityCdtos.length > 0) {
                                    for (var j = 0; j < comdityField.SkuActivityCdtos.length; j++) {
                                        if (comdityField.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                            comdityField.commodityUpInfo.CommodityStockId = comdityField.SkuActivityCdtos[j].CommodityStockId;
                                        }
                                    }
                                }
                                else {
                                    if (comdityField.DiscountPrice > -1) {
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var disprice = '<i>￥</i>{money}<em>{point}</em>';
            $('#money').html(comdityField.HDPrice);
            //comdityField.commodityUpInfo.Price = YuanPeice;
            comdityField.commodityUpInfo.color = comdityField.currentColor;
            comdityField.commodityUpInfo.size = comdityField.currentSize;
            comdityField.commodityUpInfo.number = $('#modal-wrap-count').val();
            comdityField.commodityUpInfo.pic = thumImg;

            sessionStorage.Color = comdityField.currentColor;
            sessionStorage.Size = comdityField.currentSize;


        },
        isPresale: function () {
            var flag = false;
            $.ajax({
                url: '/mobile/CheckMyPresellComdty',
                dataType: "json",
                async: false,
                data: {
                    userId: getUserId(),
                    presellComdtyId: getQueryString("outPromotionId"),
                    commodityId: getQueryString('commodityId'),
                    commodityStockId: sessionStorage.commodityStockId
                }
            }).done(function (data) {
                if (data.isSuccess === false) { //已经预约过
                    flag = true;
                } else {
                    flag = false;
                }
            });
            return flag;
        },
        setPrice: function (data) {
            var Price = data.DiscountPrice;
            Price += "";
            var _Price = Price.split(".");
            var _Price1 = _Price[0];
            var _Price2 = _Price[1] || "00";
            $('#price').html("<span>￥</span>" + _Price1 + ". <span>" + _Price2 + "</span>");
            $('#olderPrice').text(data.Price && "￥" + data.Price);
        },//设置价格
        loadData: function () {
            Page.initEvent();//绑定事件
            Page.initSwiper('#swiper', 5000);//轮播
            Page.setProvince();
            //请求后台获取商品信息json对象
            (function () {
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
                        Page.contactObj = data.ContactObj;
                        Page.contactUrl = data.ContactUrl;
                        Page.resultIsVolume = data.ResultIsVolume;
                        //DOWNLOADBANNER.setDownloadBanner(data.AppDownLoadInfo);
                        //获取数据成功的操作
                        data.CommodityInfo && Page.BindData(data.CommodityInfo);
                    } else {
                        //$('#noCommodity').removeClass('hide');
                        //$('#summary').addClass('hide');
                        //$('#nav').addClass('hide');
                    }
                    $('#mask').addClass('hide');
                }).fail(function (err) {
                    //获取数据失败的操作
                    toast('服务器繁忙，请稍后重试!');
                    $('#mask').addClass('hide');
                });
            })()
        },//加载数据
        BindData: function (data) {
            window.CommodityInfo = data;
            //保存商品信息
            comdityField.Price = data.Price;//原价
            comdityField.HDPrice = data.DiscountPrice;//活动价
            document.title = data.Name;
            //保存商品信息
            comdityField.commodityUpInfo = {
                name: data.Name,
                pic: data.Pic,
                number: 1,
                price: data.Price,
                Intensity: data.Intensity,
                IsEnableSelfTake: data.IsEnableSelfTake,
                marketPrice: Math.abs(data.MarketPrice),
                CommodityType: data.CommodityType,
                orderType: data.CommodityType,
                OutPromotionId: data.OutPromotionId || ''
            };
            comdityField.DiscountPrice = data.DiscountPrice; //优惠价
            comdityField.Intensity = data.Intensity; //折扣
            comdityField.outPromotionId = data.OutPromotionId;
            comdityField.commodityStockId = data.CommodityStockId;
            comdityField.limitBuyEach = data.limitBuyEach;
            comdityField.stock = data.Stock;
            comdityField.SkuActivityCdtos = data.SkuActivityCdtos;
            comdityField.JCActivityItemsListCdtos = data.JCActivityItemsListCdtos;
            comdityField.PromotionType = data.PromotionType;
            comdityField.DiyGroupPromotion = data.DiyGroupPromotion;
            comdityField.PromotionId = data.PromotionId;
            if (data.CommodityStocks && data.CommodityStocks.length) {
                comdityField.commodityStocks = data.CommodityStocks;
                comdityField.SkuActivityCdtos = data.SkuActivityCdtos;
            }
            else {
                $('.sku').addClass('hide');
            }
            if (data.ComAttibutes && data.ComAttibutes.length) {
                for (var i = 0; i < data.ComAttibutes.length; i++) {
                    if (comdityField.attributeTitle.indexOf(data.ComAttibutes[i].Attribute) < 0) {
                        comdityField.attributeTitle.push(data.ComAttibutes[i].Attribute);
                    }
                }
            }
            if (comdityField.attributeTitle.length == 2) { //商品包含多个属性
                comdityField.hasAttribute = true;
                for (var i = 0; i < data.ComAttibutes.length; i++) {
                    if (comdityField.attributeTitle[0].indexOf(data.ComAttibutes[i].Attribute) < 0) {
                        comdityField.allSize.push(data.ComAttibutes[i].SecondAttribute);
                    } else {
                        comdityField.allColor.push(data.ComAttibutes[i].SecondAttribute);
                    }
                }
            } else if (comdityField.attributeTitle.length == 1) { //商品只有一个属性
                comdityField.hasAttribute = true;
                for (var i = 0; i < data.ComAttibutes.length; i++) {
                    comdityField.allColor.push(data.ComAttibutes[i].SecondAttribute);
                }
            }
            comdityField.thumImg = data.Pic;
            //获取规格属性
            if (data.Specifications && data.Specifications.length > 0) {
                comdityField.hasAttribute = true;
                for (var i = 0; i < data.Specifications.length; i++) {
                    var obj = {};
                    obj.Id = data.Specifications[i].Id;
                    obj.Name = data.Specifications[i].Name;
                    obj.Attribute = data.Specifications[i].Attribute;
                    obj.strAttribute = data.Specifications[i].strAttribute;
                    comdityField.allSpecifications.push(obj);
                }
            }
            /*设置轮播组件*/
            Page.renderSwiper(data.Pictures, data.State, data.Stock);


            /*设置价格*/
            Page.setPrice(data);
            $('#title').text(data.Name);//商品标题
            $('#pic').attr('src', data.Pic);//已选页面 商品小图标
            //运费 
            if (data.Freight > 0)
                $('#freight').text('运费：￥' + Math.abs(data.Freight || 0) + (data.PostAge ? ',' + data.PostAge : ''));

            /*设置评价*/
            Page.setEvaluate(data.HasReviewFunction, data.Score);
            Page.initEvaluate('#evaluate');

            //分销
            if (!data.IsActiveCrowdfunding) {
                $('.parameters').addClass('hide');
            }
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

            //服务 begin
            var ServiceTitle = '';
            var ServiceContent = ''
            if (data.ServiceSettings !== null && data.ServiceSettings.length > 0) {
                for (var i = 0; i < data.ServiceSettings.length; i++) {
                    ServiceTitle += "<span class=\"service-list\">" + data.ServiceSettings[i].Title + "</span>";
                    ServiceContent += "<li class=\"item\"><img class=\"item-pic\" src=\"/Content/Mobile/CommodityPresale/images/yj-service-icon.png\"><div class=\"item-context\"><div class=\"title\">" + data.ServiceSettings[i].Title + "</div><div class=\"text\">" + data.ServiceSettings[i].Content + "</div></div></li>";
                }
                $('#ServiceTitle').html(ServiceTitle);
                $('#ServiceContent').html(ServiceContent);
            }
            else {
                $("#Service").addClass('hide')
            }
            //服务 end
            Page.initColorSize();
            //if (data.ComAttibutes && data.ComAttibutes.length > 0) {//库存商品属性集合
            //    debugger
            //    data.ComAttibutes.forEach(function (x, index, a) {
            //        console.log(x + '|' + index + '|' + (a === arr));
            //    });
            //    //商品属性（1无属性，2单属性，3，两组属性组合）
            //    if (data.ComAttrType == 2) {
            //        var attr1 = '';
            //        attr1 += "<section class=\"sku-list\">                            "
            //        attr1 += "    <h2 class=\"sku-title\">颜色</h2>                   "
            //        attr1 += "    <div class=\"sku-content size\">                    "
            //        attr1 += "        <span class=\"size-item\">宇宙黑</span>"
            //        attr1 += "        <span class=\"size-item\">中国红</span>         "
            //        attr1 += "    </div>                                            "
            //        attr1 += "</section>                                            ";
            //    }


            //    if (data.ComAttrType == 3) {
            //        var attr1 = '';
            //        attr1 += "<section class=\"sku-list\">                            "
            //        attr1 += "    <h2 class=\"sku-title\">颜色</h2>                   "
            //        attr1 += "    <div class=\"sku-content size\">                    "
            //        attr1 += "        <span class=\"size-item selected\">宇宙黑</span>"
            //        attr1 += "        <span class=\"size-item\">中国红</span>         "
            //        attr1 += "    </div>                                              "
            //        attr1 += "</section>                                              ";



            //    }
            //} else {
            //    $("#Selected").addClass('hide');
            //}
        }
    };
    $(function () {
        Page.init();
    });
    window.DETAIL = Page;
})(window.Zepto)


