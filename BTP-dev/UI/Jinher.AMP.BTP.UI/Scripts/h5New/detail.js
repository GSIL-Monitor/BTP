window.DETAIL = (function (mod, $, undefined) {
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
    mod.price = [];
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
    mod.PromotionType = -1; //活动类型
    mod.DiyGroupPromotion = null; //拼团信息
    mod.PromotionId = null; //活动id

    mod.thumImg = ''; //默认缩略图
    mod.pictures = []; //默认轮播图
    mod.carouselImgs = []; //某一属性的轮播图
    mod.picDefault = true; //当前是默认的轮播图
    mod.State = '';
    /**
    * 获取数据
    */
    mod.loadData = function () {
        /**
        * ajax获取数据,获取到数据后初始化数据
        */
        $('#mask').removeClass('hide');
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
                    DOWNLOADBANNER.setDownloadBanner(data.AppDownLoadInfo);
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
                    DOWNLOADBANNER.setDownloadBanner(data.AppDownLoadInfo);
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
    };
    /**
    * render轮播图
    */
    mod.renderSwiper = function (pictures, state, stock) {
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
                    flag && mod.upadteContentHeight($('#page1').height());
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
                        mod.initSwiper(pictures.length);
                    }
                    if (!$('#page2').hasClass('hide')) {
                        $('#summary').css('transform', 'translateY(-' + $('#page1').height() + 'px)');
                    }
                    return;
                }
                number++;
            }
        }
    };
    /**
    * 初始化轮播图
    */
    mod.initSwiper = function (number) {
        var autoplay = number == 1 ? null : 5000;
        //banner轮播
        mod.mySwiper = new Swiper('#swiper', {
            direction: 'horizontal',
            loop: true,
            autoplayDisableOnInteraction: false,
            pagination: '.swiper-pagination', // 如果需要分页器
            paginationClickable: true,
            autoplay: autoplay
        });
        number == 1 && mod.mySwiper.detachEvents();
    };
    /**
    * 更新一下显示区域的高度
    */
    mod.upadteContentHeight = function (value) {
        $('#content').height(value + 50);
    };
    /**
    * 返回到顶部
    */
    mod.backTop = function () {
        $('#page2').css('padding-top', '0');
        mod.backDetail();
    };
    /**
    * 从图文详情页返回商品详情页
    */
    mod.backDetail = function () {
        $('#summary').css('transform', 'translateY(0)');
        DETAIL.upadteContentHeight($('#page1').height()); //更新显示区域的高度
        $('.page2-header').removeClass('active');
        $('.page2-nav').addClass('hide');
        $('.backtotop-btn').removeClass('active');
        $('#page2').addClass('hide');
        $(window).scrollTop(0);
    };
    //读取新浪接口返回数据设置当前所在省
    mod.setProvince = function () {
        try {
            if (JsVilaDataNull(sessionStorage.province)) {
                DETAIL.loadData();
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
                DETAIL.loadData();
            });
        } catch (e) {
            sessionStorage.province = "北京市";
            DETAIL.loadData();
        }

    };
    /**
    * 页面的一些事件绑定
    */
    mod.event = function () {
        //加入购物车
        $('#add').on('click', function () {
            if ($(this).hasClass('disabled')) {
                return;
            }
            //判断是否登录，未登录跳转到登录页
            if (isLogin()) {
                if (mod.hasAttribute) {
                    $('#modal-wrap').data('state', 'add');
                    /*初始化属性列表*/
                    mod.initColorSize();
                    //显示选择属性弹窗
                    $('#modal-wrap').removeClass('hide');
                    mod.currentColor = '';
                    mod.currentSize = '';
                    mod.currentSpecifications = '';
                    mod.selectCartNum = 1;
                    $('body').addClass('lock');
                } else {
                    mod.selectCartNum = 1;
                    mod.addShopCart();
                }
            } else {
                DealLoginPartial.initPartialPage();
            }
        });
        $('#buyBtn,#reserver').on('click', function () {
            if ($(this).hasClass('disabled')) {
                return;
            }
            //判断是否登录，未登录跳转到登录页
            if (isLogin()) {
                var urlCommodityStockId = getQueryString("commodityStockId");
                if (mod.hasAttribute && urlCommodityStockId === null) {
                    $('#modal-wrap').data('state', 'buy');
                    /*初始化属性列表*/
                    mod.initColorSize();
                    //显示选择属性弹窗
                    $('#modal-wrap').removeClass('hide');
                    mod.currentColor = '';
                    mod.currentSize = '';
                    mod.currentSpecifications = '';
                    mod.selectCartNum = 1;
                    $('body').addClass('lock');
                } else {
                    mod.checkCommodity();
                }
            } else {
                DealLoginPartial.initPartialPage();
            }
        });
        //立即预约按钮事件绑定
        $('#appointment').on('click', function () {
            //判断是否登录,未登录跳转到登录页
            DealLoginPartial.initPartialPage();
            if (mod.hasAttribute) {
                $('#modal-wrap').data('state', 'presell');
                /*初始化属性列表*/
                mod.initColorSize();
                //显示选择属性弹窗
                $('#modal-wrap').removeClass('hide');
                mod.currentColor = '';
                mod.currentSize = '';
                mod.currentSpecifications = '';
                mod.selectCartNum = 1;
                $('body').addClass('lock');
                mod.initColorSize();
            } else {
                $.ajax({
                    url: '/mobile/CheckMyPresellComdty',
                    dataType: "json",
                    data: {
                        userId: getUserId(),
                        presellComdtyId: DETAIL.outPromotionId,
                        commodityId: getQueryString('commodityId'),
                        commodityStockId: mod.commodityStockId
                    }
                }).done(function (data) {
                    if (data.isSuccess === false) { //已经预约过
                        mod.isPresell = true;
                    } else {
                        mod.isPresell = false;
                    }
                    //判断是否已经预约过
                    if (mod.isPresell) { //已经预约过
                        $('#appointment-modal-text').text('您已经成功预约过了，无需重复预约，请关注抢购时间~~~');
                        $('#appointment-modal').removeClass('hide');
                        $('body').addClass('lock');
                    } else { //未预约过
                        $('#identify-modal').removeClass('hide');
                        $('body').addClass('lock');
                        $("#identify-img").attr("src", getBtpDomain() + "mobile/GetVerifyCodeZPH?r=" + Math.random());
                    }
                });
            }
        });
        //关闭弹窗
        $('.modal-wrap').on('click', function (event) {
            var target = event.target;
            if ($(target).hasClass('modal-wrap') || $(target).hasClass('modal-content-close')) {
                mod.closeModal();
            }
        });
        //属性弹窗 -- 选择商品数量
        $('#modal-wrap-increase').on('click', function () {
            //预约默认为1
            if (mod.PromotionType !== 2) {
                var number = $('#modal-wrap-count').val() - 0;
                number = number + 1;
                mod.updateNumber(number, true);
            }
        });
        $('#modal-wrap-decrease').on('click', function () {
            if (mod.PromotionType !== 2) {
                var number = $('#modal-wrap-count').val() - 0;
                number = number < 2 ? 1 : number - 1;
                $('#modal-wrap-count').val(number);
            }
        });
        $('#modal-wrap-count').on('change', function () {
            if (mod.PromotionType !== 2) {
                var number = parseInt($(this).val() - 0);
                if (number < 1) {
                    number = 1;
                };
                mod.updateNumber(number, true);
            } else {
                $('#modal-wrap-count').val(1);
            }
        });
        //属性选择 -- 确定按钮事件绑定
        $('#modal-wrap').on('click', '.modal-content-btn', function () {

            if (mod.attributeTitle.length >= 2) {

                if (!mod.currentColor) {
                    toast('请选择' + mod.attributeTitle[0]);
                    return;
                }
                if (mod.attributeTitle.length >= 2) { //多属性
                    if (!mod.currentSize) {
                        toast('请选择' + mod.attributeTitle[1]);
                        return;
                    }
                    var obj = mod.getCommodity(mod.currentColor, mod.currentSize, false);
                    if (obj && obj.Id) {
                        mod.commodityStockId = obj.Id;
                    }
                }
                if (mod.allSpecifications.length > 0) {

                    if (!mod.currentSpecifications) {
                        toast('请选择规格设置');
                        return;
                    }
                }
                else if (mod.attributeTitle.length == 1) {
                    var obj = mod.getCommodity(mod.currentColor, mod.currentSize, true);
                    if (obj && obj.Id) {
                        mod.commodityStockId = obj.Id;
                    }
                }
            }
            if (mod.attributeTitle.length == 0 && mod.allSpecifications.length > 0) {

                if (mod.allSpecifications.length > 0) {

                    if (!mod.currentSpecifications) {
                        toast('请选择规格设置');
                        return;
                    }
                }
            }

            if (mod.attributeTitle.length == 1) {

                if (!mod.currentColor) {
                    toast('请选择' + mod.attributeTitle[0]);
                    return;
                }
                if (mod.attributeTitle.length >= 2) { //多属性
                    if (!mod.currentSize) {
                        toast('请选择' + mod.attributeTitle[1]);
                        return;
                    }
                    var obj = mod.getCommodity(mod.currentColor, mod.currentSize, false);
                    if (obj && obj.Id) {
                        mod.commodityStockId = obj.Id;
                    }
                }
                if (mod.allSpecifications.length > 0) {

                    if (!mod.currentSpecifications) {
                        toast('请选择规格设置');
                        return;
                    }
                }
                else if (mod.attributeTitle.length == 1) {
                    var obj = mod.getCommodity(mod.currentColor, mod.currentSize, true);
                    if (obj && obj.Id) {
                        mod.commodityStockId = obj.Id;
                    }
                }
            }



            mod.selectCartNum = $('#modal-wrap-count').val() - 0;
            var state = $('#modal-wrap').data('state');
            if (state == 'buy') {
                mod.checkCommodity();
            } else if (state == 'add') {
                mod.addShopCart();
            } else if (state == 'presell') {
                //l
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
                    CommodityIdAndStockIds: [{
                        CommodityId: getQueryString('commodityId'),
                        CommodityStockId: mod.commodityStockId,
                        OutPrommotionId: mod.outPrommotionId,
                        SizeAndColorId: (mod.currentColor ? mod.currentColor : '') + ',' + (mod.currentSize ? mod.currentSize : ''), //color属性 size属性
                        Specifications: specifications
                    }]
                };
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
                    if (data[0].Stock <= 0) {
                        toast('抱歉，该商品已经售完');
                        return;
                    }
                }).fail(function (e) {
                    $('#mask').addClass('hide');
                    toast('服务器繁忙,请稍候再试');
                });

                $.ajax({
                    url: '/mobile/CheckMyPresellComdty',
                    dataType: "json",
                    data: {
                        userId: getUserId(),
                        presellComdtyId: DETAIL.outPromotionId,
                        commodityId: getQueryString('commodityId'),
                        commodityStockId: mod.commodityStockId
                    }
                }).done(function (data) {
                    if (data.isSuccess === false) { //已经预约过
                        mod.isPresell = true;
                    } else {
                        mod.isPresell = false;
                    }
                    //判断是否已经预约过
                    if (mod.isPresell) { //已经预约过
                        $('#appointment-modal-text').text('您已经成功预约过了，无需重复预约，请关注抢购时间~~~');
                        $('#appointment-modal').removeClass('hide');
                        $('body').addClass('lock');
                    } else { //未预约过
                        $('#identify-modal').removeClass('hide');
                        $('body').addClass('lock');
                        $("#identify-img").attr("src", getBtpDomain() + "mobile/GetVerifyCodeZPH?r=" + Math.random());
                    }
                });
            }
        });
        /**
        * 价格发生变化提示弹窗
        */
        $('#modal-footer').on('click', '.btn-default', function () {
            $('#myModal').hide();
            document.location.reload();
        }).on('click', '.btn-primary', function () {
            mod.commodityUpInfo.number = mod.selectCartNum;
            mod.commodityUpInfo.size = mod.currentColor;
            mod.commodityUpInfo.color = mod.currentSize;
            mod.commodityUpInfo.commodityStockId = mod.commodityStockId;
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
            window.location.href = urlAppendCommonParams('/Mobile/CreateOrder?type=goumai');
        });
        //分销
        $('.page1').on('click', '.distribution-content', function () {
            //判断是否登录
            var login = true;
            if (login) { //已登录
                window.location.href = "#"; //跳转到我要分销页面
            } else { //未登录
                window.location.href = "login.html"; //跳转到登录页面
            }
        });
        /**
        * 进店逛逛
        */
        $('#shop,.store-info-message').on('click', function () {
            var hostname = window.location.hostname;

            if (hostname.substr(0, 4) == 'test') {
                window.location.href = 'http://testyjmobile.iuoooo.com/shop?shopId=' + sessionStorage.appId + '&appId=' + getQueryString("appId");
            } else {
                window.location.href = 'http://yjmobile.iuoooo.com/shop?shopId=' + sessionStorage.appId + '&appId=' + getQueryString("appId");
            }
        });
        /**
        * 联系客服
        */
        $('.customer').on('click', function () {

            var appid = mod.contactObj ? sessionStorage.appId : (getEsAppId() || sessionStorage.appId);
            //智力圈的情况
            if (appid == "630af8fc-41e9-4bd1-a436-2dd4197f076b" || appid == "cf063155-e6e9-4019-ba12-6b44b704243f") {
                $("#Zhiliquan").show();
                return false;
            }
            else {
                $("#Zhiliquan").hide();
                if (isLogin()) {
                    var contactAppId = mod.contactObj ? sessionStorage.appId : (getEsAppId() || sessionStorage.appId);
                    if (JsVilaDataNull(mod.contactUrl) && mod.contactUrl.indexOf("http") >= 0) {
                        window.location.href = mod.contactUrl;
                    } else if (sessionStorage.source == "share") {
                        window.location.href = "http://bjzxkf.ejoy.sinopec.com:9000/public/views/framework/chat/app/app_index.html?source=bjyj&chatInlet=" + appid + "&openId=" + getUserId() + "&commodityId=" + getQueryString("commodityId")+"&hideShare=1";
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
                } else {
                    DealLoginPartial.initPartialPage();
                }
            }

        });
        /**
        * 点击购物车事件绑定
        */
        $('#shopCart').on('click', function () {
            var r;
            if (arguments.length > 1) {
                r = arguments[1].split('?')[1];
            } else {
                r = window.location.href.replace(/\&amp;/g, "&").substr(1);
            }
            //	var r = str ? str.split('?')[1] : '' || window.location.search.substr(1);
            var reg = new RegExp("(^|&)appId=([^&]*)(&|$)", "i");
            r = r.match(reg);
            window.location.href = urlAppendCommonParams('/Mobile/ShoppongCartList') + "&esAppId=" + unescape(r[2]);
        });
        /**
        * 我要分销事件绑定
        */
        $('#page1').on('click', '#distribution-wrap', function () {
            if (isLogin()) {
                document.location.href = urlAppendCommonParams("/Distribute/CommodityDistribute?commodityId=" + getQueryString('commodityId') + "&distributorId=" + sessionStorage.distributorId);
            } else {
                DealLoginPartial.initPartialPage();
            }
        });
        /**
        * 优惠券点击事件
        */
        $('.coupon-wrap').on('click', '.coupon-content', function () {
            $('#modal-coupon-wrap').removeClass('hide');
            $('#modal-coupon-wrap').find('.modal-coupon-content').addClass('fade');
            $('body').addClass('lock');
        });
        //关闭弹窗
        $('.modal-coupon').on('click', function (event) {
            var target = event.target;
            if ($(target).hasClass('modal-coupon') || $(target).hasClass('modal-coupon-content-close')) {
                $('#modal-coupon-wrap').addClass('hide');
                $('#modal-coupon-wrap').find('.modal-coupon-content').removeClass('fade');
                $('body').removeClass('lock');
            }
        });
        /**
        * 切换tab
        */
        $('.page2-nav').on('click', '.nav-list', function () {
            if (!$(this).hasClass('selected')) {
                $(this).parent().find('.selected').removeClass('selected');
                $(this).addClass('selected');
                $('.shoppingInfo>div').addClass('hide');
                $('.shoppingInfo>div').eq($(this).index()).removeClass('hide');
                DETAIL.upadteContentHeight($('#page2').height() + 43); //更新显示区域的高度
            }
        });
        /**
        * 开启服务说明弹窗
        */
        $('#d-info').on('click', function () {
            $('#server-wrap').removeClass('hide');
            $('#server-wrap').find('.modal-server-content').addClass('fade');
            $('body').addClass('lock');
        });
        /**
        * 关闭服务说明弹窗
        */
        $('#server-wrap').on('click', function (event) {
            var target = event.target;
            if ($(target).hasClass('modal-server') || $(target).hasClass('modal-server-content-close') || $(target).hasClass('modal-server-content-bt')) {
                $('#server-wrap').addClass('hide');
                $('#server-wrap').find('.modal-server-content').removeClass('fade');
                $('body').removeClass('lock');
            }
        });
        /**
        * 开启优惠套餐弹窗
        */
        $('#package').on('click', function () {
            $('#mask').removeClass('hide');
            $.ajax({
                url: '/Mobile/GetSetMealActivitysByCommodityId?r=' + Math.random(),
                type: 'GET',
                dataType: 'json',
                data: {                    
                    commodityId: getQueryString("commodityId"),
                    appId: getEsAppId()
                },
                async: true
            }).done(function (data) {
                if (data.Result) {
                    if (data.Result.length) {
                        $('#package-price').text(parseFloat(data.Result[0].PreferentialPrice).toFixed(2) + "元");
                        $('#package-num').text(data.Result.length);
                        var clientWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
                        var maxCount = parseInt((clientWidth) / 93);
                        var html = '';
                        for (var b = 0; b < data.Result.length; b++) {
                            var commodityIds = new Array();
                            var item = data.Result[b];
                            var zwI = "";
                            if (b === 0) {
                                zwI = "一";
                            } else if (b === 1) {
                                zwI = "二";
                            } else if (b === 2) {
                                zwI = "三";
                            } else if (b === 3) {
                                zwI = "四";
                            } else if (b === 4) {
                                zwI = "五";
                            } else if (b === 5) {
                                zwI = "六";
                            } else if (b === 6) {
                                zwI = "七";
                            } else if (b === 7) {
                                zwI = "八";
                            } else if (b === 8) {
                                zwI = "九";
                            } else if (b === 9) {
                                zwI = "十";
                            } else if (b === 10) {
                                zwI = "十一";
                            } else if (b === 11) {
                                zwI = "十二";
                            } else if (b === 12) {
                                zwI = "十三";
                            } else if (b === 13) {
                                zwI = "十四";
                            } else if (b === 14) {
                                zwI = "十五";
                            } else if (b === 15) {
                                zwI = "十六";
                            } else if (b === 16) {
                                zwI = "十七";
                            } else if (b === 17) {
                                zwI = "十八";
                            } else if (b === 18) {
                                zwI = "十九";
                            } else if (b === 19) {
                                zwI = "二十";
                            }
                            html += '<div class="packgae-list-title">套餐' + zwI + '</div><div class="packgae-list-item" onclick="rMealItems(' + b + ')">';
                            var count = 0;
                            for (var j = 0; j < item.SetMealItemsCdtos.length; j++) {
                                if ($.inArray(item.SetMealItemsCdtos[j].CommodityId, commodityIds) === -1) {
                                    var child = item.SetMealItemsCdtos[j].CommodityDto.Pic;
                                    if (count >= maxCount) {
                                        html += '<span class="more">...</span>';
                                        break;
                                    } else {
                                        html += '<div class="pic"><img src="' + child + '"></div>';
                                    }
                                    commodityIds.push(item.SetMealItemsCdtos[j].CommodityId);
                                    count++;
                                }
                            }
                            html += '</div>';
                        }
                        $('#package-list-content').html(html);

                        if (DETAIL.Intensity !== 10 || DETAIL.DiscountPrice !== -1 || DETAIL.PromotionType === 1 || DETAIL.PromotionType === 2 || DETAIL.PromotionType === 3 || DETAIL.PromotionType === 5) {
                            $('.package-tips').removeClass('hide');
                            $("#tip").text("此价格不与套装优惠同时享受");
                        } else {
                            /*设置优惠套餐*/
                            var isMealActivity = true;
                            /*优惠券、易捷币、加油卡兑换劵、优惠套装等n选1*/
                            var couponExtraText = "";
                            var couponEnable = $("#coupon-text").is(":visible");
                            var yjbEnable = $("#yjb-text").is(":visible");
                            var youkaEnable = $("#oil-coupon").is(":visible");

                            if (couponEnable && yjbEnable && youkaEnable && isMealActivity) {
                                couponExtraText = "优惠券、易捷币、加油卡兑换券不与套餐优惠同时享受";
                            } else if (couponEnable && yjbEnable && youkaEnable) {
                                couponExtraText = "优惠券、易捷币、加油卡兑换券不能同时享受";
                            } else if (couponEnable && youkaEnable && isMealActivity) {
                                couponExtraText = "优惠券、易捷币、套餐优惠不能同时享受";
                            } else if (yjbEnable && youkaEnable && isMealActivity) {
                                couponExtraText = "易捷币、加油卡兑换券不与套餐优惠同时享受";
                            }
                            else if (couponEnable && yjbEnable) {
                                couponExtraText = "优惠券、易捷币不能同时享受";
                            } else if (couponEnable && youkaEnable) {
                                couponExtraText = "优惠券、加油卡兑换券不能同时享受";
                            } else if (couponEnable && isMealActivity) {
                                couponExtraText = "优惠券、套餐优惠不能同时享受";
                            } else if (youkaEnable && yjbEnable) {
                                couponExtraText = "易捷币、加油卡兑换券不能同时享受";
                            }
                            else if (yjbEnable && isMealActivity) {
                                couponExtraText = "易捷币、套餐优惠不能同时享受";
                            }
                            else if (youkaEnable && isMealActivity) {
                                couponExtraText = "加油卡兑换券、套餐优惠不能同时享受";
                            }
                            if (couponExtraText.length > 0) {
                                $('.package-tips').removeClass('hide');
                                $("#tip").text(couponExtraText);
                            }
                        }
                    }

                    $('#package-wrap').removeClass('hide');
                    $('#package-wrap').find('.modal-server-content').addClass('fade');
                    $('body').addClass('lock');
                }
                $('#mask').addClass('hide');
            }).fail(function (err) {
                //获取数据失败的操作
                toast('服务器繁忙，请稍后重试!');
                $('#mask').addClass('hide');
            });
        });
        /**
        * 关闭优惠套餐弹窗
        */
        $('#package-wrap').on('click', function (event) {
            var target = event.target;
            if ($(target).hasClass('modal-server') || $(target).hasClass('modal-server-content-close') || $(target).hasClass('modal-server-content-bt')) {
                $('#package-wrap').addClass('hide');
                $('#package-wrap').find('.modal-server-content').removeClass('fade');
                $('body').removeClass('lock');
            }
        });
        /**
        * 开启赠品弹窗
        */
        $('#present').on('click', function () {
            $('#present-wrap').removeClass('hide');
            $('#present-wrap').find('.modal-present-content').addClass('fade');
            $('body').addClass('lock');
        });
        /**
        * 关闭赠品弹窗
        */
        $('#present-wrap').on('click', function (event) {
            var target = event.target;
            if ($(target).hasClass('modal-present') || $(target).hasClass('modal-present-content-close') || $(target).hasClass('modal-present-content-bt')) {
                $('#present-wrap').addClass('hide');
                $('#present-wrap').find('.modal-present-content').removeClass('fade');
                $('body').removeClass('lock');
            }
        });
        /**
        * 打开分享有礼规则页面
        */
        $('#showRule').on('click', function () {
            window.location.href = "/ShareRedEnvelope/ShowRuleDescription?appId=" + getEsAppId();
        });
    };

    /**
    * 获取到数据后初始化数据
    */
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
        mod.DiscountPrice = data.DiscountPrice; //优惠价
        mod.Intensity = data.Intensity; //折扣
        mod.outPromotionId = data.OutPromotionId;
        mod.commodityStockId = data.CommodityStockId;
        mod.limitBuyEach = data.limitBuyEach;
        mod.stock = data.Stock;
        mod.SkuActivityCdtos = data.SkuActivityCdtos;
        mod.JCActivityItemsListCdtos = data.JCActivityItemsListCdtos;
        mod.PromotionType = data.PromotionType;
        mod.DiyGroupPromotion = data.DiyGroupPromotion;
        mod.PromotionId = data.PromotionId;
        if (data.CommodityStocks && data.CommodityStocks.length) {
            mod.commodityStocks = data.CommodityStocks;
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
        data.Pictures.length && this.renderSwiper(data.Pictures, data.State, data.Stock);
        /*设置价格*/
        this.setPriceRange(data);
        mod.thumImg = data.Pic;
        $('#pic').attr('src', data.Pic);
        $('#title').text(data.Name);
        /*销量*/
        if (mod.resultIsVolume == true) {
            $('#d-adds').append('<span class="sales">销量: ' + data.Total + '</span>');
        }
        /*运费*/
        $('#d-adds').append('<span class="freight">运费: ￥' + Math.abs(data.Freight || 0) + (data.PostAge ? ',' + data.PostAge + '</span>' : '</span>'));
        /*设置佣金*/
        if (data.SharePercent != null && parseFloat(data.SharePercent) > 0) {
            $('#showRule').removeClass('hide');
            /*var commission = (data.Price * data.SharePercent).toFixed(2);
            var html = '<span class="commission">' +
            '<span class="clearfix" style="display: inline-block;">' +
            '<em id="commission">佣金: ' + getCurrency() + commission + '</em>' +
            '<a href="/ShareRedEnvelope/ShowRuleDescription?appId=' + getQueryString("appId") + '" class="pic"><img src="/Content/Mobile/wenhao.png"></a>' +
            '</span>' +
            '</span>';
            $('#d-adds').append(html);*/
        }
        /*设置标签*/
        if (data.ServiceSettings && data.ServiceSettings.length > 0) {
            var dInfoHtml = '';
            var html = "";
            data.ServiceSettings.forEach(function (v) {
                dInfoHtml += '<span>' + v.Title + '</span>';
                html += '<div class="server-list">' +
                            '<div class="server-list-title">' +
                                '<span>' + v.Title + '</span>' +
                            '</div>' +
                            '<div class="server-list-info">' + v.Content + '</div>' +
                        '</div>';
            });
            $("#d-info").html(dInfoHtml).removeClass('hide');
            $('#server-list-content').html(html);
        }
        /*设置优惠券*/
        if (data.Coupons && data.Coupons.length > 0) {
            $("#coupon-text").text(data.Coupons.join(",")).parent().removeClass('hide');
        }
        /*设置优惠券领取列表*/
        if (data.CouponList && data.CouponList.length > 0) {
            var couhtml = '';
            data.CouponList.forEach(function (v) {
                couhtml +=
                    '<div class="lists-wrap">' +
                    '<div class="list">' +
                    (v.IsDraw ? '<div class="list-left used">' : '<div class="list-left">') +
                    '<div class="name">' + v.Name + '</div>' +
                    '<div class="content">' +
                    '<div class="text">' + (v.LimitCondition > 0 ? ('满' + v.LimitCondition + '元使用') : '无门槛优惠券') + '</div>' +
                    '<div class="date">有效期至' + mod.transformTime(v.EndTime) + '</div>' +
                    ' </div>' +
                    '</div>' +
                    ' <div class="list-right">' +
                    '<div class="title">' + (v.Type == 0 ? '店铺' : '商品') + '优惠券</div>' +
                    '<div class="price">&#165;<span class="f30">' + v.Cash + '</span></div>' +
                (v.LimitUse - v.UseNum == 0 ? '<a class="btn disabled">已领取</a>' : '<a class="btn" onclick=\'DETAIL.bindCoupon("' + v.Id + '",this)\'>点击领取</a>') +
                    '</div>' +
                    '</div>' +
                    '</div>';
            });
            $("#coupon-list-content").html(couhtml);
        }
        /*设置易捷币*/
        if (data.YJBInfo && data.YJBInfo.Enabled) {
            $("#yjb-text").text('可使用易捷币折现' + data.YJBInfo.InsteadCashAmount + '元').removeClass('hide');
            $('#coupon-wrap').removeClass('hide');
        } else {
            $("#yjb-text").addClass('hide');
        }
        /*设置加油卡兑换券*/
        if (data.YouKaInfo && data.YJBInfo.GiveMoney > 0) {
            $("#oil-coupon").text(data.YJBInfo.Message).removeClass('hide');
            $('#coupon-wrap').removeClass('hide');
        } else {
            $("#oil-coupon").addClass('hide');
        }
        /*金采团购活动 == 设置加油卡兑换券*/
        if (data.JCActivityItemsListCdtos && data.JCActivityItemsListCdtos !== null) {
            var a = new Array();
            var ctos = data.JCActivityItemsListCdtos;
            for (var j = 0; j < ctos.length; j++) {
                a.push(ctos[j].GiftGardScale * ctos[j].GroupPrice / 100);
            }
            a.sort();
            if (a[ctos.length - 1] > a[0]) {
                $("#oil-coupon").text(a[0] + "~" + a[ctos.length - 1]).removeClass('hide');
                $('#coupon-wrap').removeClass('hide');

                $("#yjb-text").addClass('hide');
            } else {
                $("#oil-coupon").text((a[0])).removeClass('hide');
                if (a[0] > 0) {
                    $('#coupon-wrap').removeClass('hide');

                    $("#yjb-text").addClass('hide');
                }
            }
        }
        /*设置赠品*/
        if (data.Present) {
            if (data.Present.IsAll) {
                $('#present').text('购买即送超值赠品 (赠完即止)').removeClass('hide');
            } else {
                $('#present').text('购买指定型号送超值赠品 (赠完即止)').removeClass('hide');
            }
            $('#coupon-wrap').removeClass('hide');
            mod.showPresent(data.Present);
        }
        /*设置优惠套餐*/
        var isMealActivity = false;
        if (data.MealActivityInfo && data.MealActivityInfo !== null) {
            $("#package").text(data.MealActivityInfo).removeClass('hide');
            $('#coupon-wrap').removeClass('hide');
            isMealActivity = true;
        } else {
            $("#package-content").addClass('hide');
        }

        /*优惠券、易捷币、加油卡兑换劵、优惠套装等n选1*/
        var couponExtraText = "";
        var couponEnable = $("#coupon-text").is(":visible");
        var yjbEnable = $("#yjb-text").is(":visible");
        var youkaEnable = $("#oil-coupon").is(":visible");

        if (couponEnable && yjbEnable && youkaEnable && isMealActivity) {
            couponExtraText = "优惠券、易捷币、加油卡兑换券不与套餐优惠同时享受";
        } else if (couponEnable && yjbEnable && youkaEnable) {
            couponExtraText = "优惠券、易捷币、加油卡兑换券不能同时享受";
        } else if (couponEnable && youkaEnable && isMealActivity) {
            couponExtraText = "优惠券、易捷币、套餐优惠不能同时享受";
        } else if (yjbEnable && youkaEnable && isMealActivity) {
            couponExtraText = "易捷币、加油卡兑换券不与套餐优惠同时享受";
        }
        else if (couponEnable && yjbEnable) {
            couponExtraText = "优惠券、易捷币不能同时享受";
        } else if (couponEnable && youkaEnable) {
            couponExtraText = "优惠券、加油卡兑换券不能同时享受";
        } else if (couponEnable && isMealActivity) {
            couponExtraText = "优惠券、套餐优惠不能同时享受";
        } else if (youkaEnable && yjbEnable) {
            couponExtraText = "易捷币、加油卡兑换券不能同时享受";
        }
        else if (yjbEnable && isMealActivity) {
            couponExtraText = "易捷币、套餐优惠不能同时享受";
        }
        else if (youkaEnable && isMealActivity) {
            couponExtraText = "加油卡兑换券、套餐优惠不能同时享受";
        }
        if (couponExtraText.length > 0) $("#extra-text").text(couponExtraText);
        /*设置关税*/
        mod.setDuty(data);
        data.IsDistribute && $('#distribution-wrap').removeClass('hide');
        data.AppName && $('#store-name').text(data.AppName); //设置店名称
        data.AppIcon && $('#store-logo').attr('src', data.AppIcon); //设置店铺logo

        /*设置评价*/
        this.setEvaluate(data.HasReviewFunction, data.Score);
        /*设置直播和720云景按钮*/
        this.setBtn(data.EquipmentUrl, data.CloudviewUrl);
        // 展现 活动状态
        this.showSateByPromotion(data);
        //相关商品区
        mod.setRelationCommodity(data.RelationCommoditys);
        /*获取购物车数量*/
        isLogin() && this.getShoppingCartNum();
        /*设置 下载APP 三级分销*/
        DOWNLOADBANNER.setDistributeBanner(data.IsDistribute, data.AppId);

        //视频内容
        if (data.VideoUrl) { //已经上传视频组件
            $('#vedio').attr('src', data.VideoUrl);
            $('#vedio-wrap').removeClass('hide');
        } else {
            $('#vedio-wrap').addClass('hide');
        }
        //图文内容
        if (data.Description) {
            $("#shoppingInfo").append(data.Description);
            if (data.Description.indexOf('JD-goods') !== -1) {
                var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
                var scale = (deviceWidth - 20) / 640;

                $('.JD-goods').css({
                    'transform': 'scale(' + scale + ')',
                    'width': '640px',
                    'transform-origin': '0 0 0'
                })

            }
        }

        if (data.TechSpecs) {
            $("#techSpecs").append(data.TechSpecs);
        } else {
            $("#techSpecs").append('<p>没有更多了~</p>');
        }
        if (data.SaleService) {
            $("#saleService").append(data.SaleService);
        } else {
            $("#saleService").append('<p>没有更多了~</p>');
        }
        var h = document.documentElement.clientHeight;
        $("#page2").css('min-height', h - 88);
        //页面数据加载完成后显示区域的高度设置成第一屏页面的高度
        this.initEvent();
        //微信自定义分享内容
        WxShare.config(DETAIL.commodityUpInfo.name, DETAIL.commodityUpInfo.name, DETAIL.commodityUpInfo.pic);
    };
    /**
    * 设置关税
    */
    mod.setDuty = function (data) {
        if (data.Duty != null && data.Duty > 0) {
            mod.Duty.push(data.Duty.toFixed(2));
        }
        if (data.CommodityStocks.length) {
            var dutyArr = [];
            mod.Duty = [];
            for (var j = 0; j < data.CommodityStocks.length; j++) {
                var commodity = data.CommodityStocks[j];
                if (commodity.Duty != null && commodity.Duty != 'null') {
                    dutyArr.push(commodity.Duty);
                }
            }
            if (dutyArr.length) {
                dutyArr.sort();
                if (dutyArr[0] < dutyArr[dutyArr.length - 1]) {
                    mod.Duty.push(dutyArr[0].toFixed(2));
                    mod.Duty.push(dutyArr[dutyArr.length - 1].toFixed(2));
                } else {
                    (dutyArr[0] > 0) && mod.Duty.push(dutyArr[0].toFixed(2));
                }
            }
        }
        if (mod.Duty.length) {
            $('#duty').removeClass('hide');
            $('#duty-value').text('￥' + mod.Duty.join('~'));
            if (mod.Duty[0] > 0) {
                $('#modal-duty').text('关税：￥' + mod.Duty[0]);
            }
        }
    };
    /**
    * 更新关税
    */
    mod.updateDuty = function () {
        var duty = mod.Duty[0];
        //判断价格是否是区间值：如果是并且两种属性已经选择了，则显示对应的值
        if (mod.Duty.length == 2) { //价格是区间值
            if (mod.currentColor && mod.currentSize) { //两种属性已经选择了
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    var commodity = mod.commodityStocks[i];
                    if ((commodity.ComAttribute[0].SecondAttribute == mod.currentColor && commodity.ComAttribute[1].SecondAttribute == mod.currentSize) || (commodity.ComAttribute[1].SecondAttribute == mod.currentColor && commodity.ComAttribute[0].SecondAttribute == mod.currentSize)) {
                        duty = commodity.Duty.toFixed(2);
                    }
                }
            }
        }
        duty > 0 ? $('#modal-duty').text('关税：￥' + duty) : $('#modal-duty').text('');
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
    * 设置价格区间
    */
    mod.setPriceRange = function (data) {
        var minPrice = data.Price,
            maxPrice = data.Price,
            minMarketPrice = data.MarketPrice,
            maxMarketPrice = data.MarketPrice,
            skuPrices = new Array(),
            skuIds = new Array(),
            skuJcPrices = new Array(),
            skuJcIds = new Array();
        if (data.CommodityStocks && data.CommodityStocks.length) { //多属性
            minPrice = 0;
            maxPrice = 0;
            minMarketPrice = 0;
            maxMarketPrice = 0;
            //获取参与sku活动以及不参与的价格集合
            if (data.SkuActivityCdtos) {
                for (var l = 0; l < data.SkuActivityCdtos.length; l++) {
                    skuPrices.push(data.SkuActivityCdtos[l].JoinPrice);
                    skuIds.push(data.SkuActivityCdtos[l].CommodityStockId);
                }
            }
            //获取参与sku金采团购活动以及不参与的价格集合
            if (data.JCActivityItemsListCdtos) {
                for (var l = 0; l < data.JCActivityItemsListCdtos.length; l++) {
                    skuJcPrices.push(data.JCActivityItemsListCdtos[l].GroupPrice);
                    skuJcIds.push(data.JCActivityItemsListCdtos[l].ComdtyStockId);
                }
            }
            for (var j = 0; j < data.CommodityStocks.length; j++) {
                var commodity = data.CommodityStocks[j];
                if (commodity.Price < minPrice || minPrice == 0) {
                    minPrice = commodity.Price;
                }
                if (commodity.Price > maxPrice) {
                    maxPrice = commodity.Price;
                }
                if (commodity.MarketPrice && commodity.MarketPrice != null && commodity.MarketPrice != 'null' && (commodity.MarketPrice < minMarketPrice || minMarketPrice == 0)) {
                    minMarketPrice = commodity.MarketPrice;
                }
                if (commodity.MarketPrice && commodity.MarketPrice != null && commodity.MarketPrice != 'null' && commodity.MarketPrice > maxMarketPrice) {
                    maxMarketPrice = commodity.MarketPrice;
                }

                //获取参与sku活动以及不参与的价格集合
                if ($.inArray(data.CommodityStocks[j].Id, skuIds) === -1) {
                    skuPrices.push(data.CommodityStocks[j].Price);
                }
                skuPrices.sort();
                //获取参与sku金采团购活动以及不参与的价格集合
                if ($.inArray(data.CommodityStocks[j].Id, skuJcIds) === -1) {
                    skuJcPrices.push(data.CommodityStocks[j].Price);
                }
                skuJcPrices.sort();
            }
        }

        if (data.DiscountPrice > 0) { //优惠价
            //秒杀显示价格区间
            if (data.PromotionType === 1 || data.PromotionType === 0) {
                //判断是否为拼团商品
                if (data.DiyGroupPromotion != null && data.PromotionId == null) {
                    mod.price.push(minPrice.toFixed(2));
                    mod.price.push(maxPrice.toFixed(2));
                    if (minMarketPrice < maxMarketPrice) {
                        mod.oldPrice.push(minMarketPrice.toFixed(2));
                        mod.oldPrice.push(maxMarketPrice.toFixed(2));
                    }
                } else {
                    //如果存在sku属性值
                    if (data.MinSkuPrice > 0 && data.MinSkuPrice < data.MaxSkuPrice) {
                        mod.price.push(skuPrices[0].toFixed(2));
                        mod.price.push(skuPrices[skuPrices.length - 1].toFixed(2));
                        if (minMarketPrice < maxMarketPrice) {
                            mod.oldPrice.push(minMarketPrice.toFixed(2));
                            mod.oldPrice.push(maxMarketPrice.toFixed(2));
                        } else {
                            //minMarketPrice && mod.oldPrice.push(minMarketPrice.toFixed(2));
                            if (minPrice < maxPrice) {
                                mod.oldPrice.push(minPrice.toFixed(2));
                                mod.oldPrice.push(maxPrice.toFixed(2));
                            }
                        }
                    } else {
                        mod.price.push(data.DiscountPrice.toFixed(2));
                        if (minPrice < maxPrice) {
                            mod.oldPrice.push(minPrice.toFixed(2));
                            mod.oldPrice.push(maxPrice.toFixed(2));
                        } else {
                            mod.oldPrice.push(minPrice.toFixed(2));
                        }
                    }
                }
            } else if (data.PromotionType === 2) {//预约显示最小价格
                //如果存在sku属性值
                if (data.MinSkuPrice > 0 && data.MinSkuPrice < data.MaxSkuPrice) {
                    mod.price.push(data.MinSkuPrice.toFixed(2));
                    if (minMarketPrice < maxMarketPrice) {
                        mod.oldPrice.push(minMarketPrice.toFixed(2));
                    } else {
                        //minMarketPrice && mod.oldPrice.push(minMarketPrice.toFixed(2));
                        if (minPrice < maxPrice) {
                            mod.oldPrice.push(minPrice.toFixed(2));
                        }
                    }
                } else {
                    mod.price.push(data.DiscountPrice.toFixed(2));
                    if (minPrice < maxPrice) {
                        mod.oldPrice.push(minPrice.toFixed(2));
                    } else {
                        mod.oldPrice.push(minPrice.toFixed(2));
                    }
                }

                //我的预约 带属性值过来 把价格直接赋值
                var commodityStockId = getQueryString("commodityStockId");
                if (commodityStockId !== null && commodityStockId !== "00000000-0000-0000-0000-000000000000") {
                    //清空价格赋值
                    mod.price = [];
                    mod.oldPrice = [];
                    for (var k = 0; k < data.SkuActivityCdtos.length; k++) {
                        if (data.SkuActivityCdtos[k].CommodityStockId === commodityStockId) {
                            mod.price.push(data.SkuActivityCdtos[k].JoinPrice.toFixed(2));
                        }
                    }
                    for (var m = 0; m < data.CommodityStocks.length; m++) {
                        if (data.CommodityStocks[m].Id === commodityStockId) {
                            if (data.CommodityStocks[m].MarketPrice) {
                                mod.oldPrice.push(data.CommodityStocks[m].MarketPrice.toFixed(2));
                            }
                        }
                    }
                }
            } else if (data.PromotionType === 5) {//预售价格（显示最小价格）
                //如果存在sku属性值
                if (data.MinSkuPrice > 0 && data.MinSkuPrice < data.MaxSkuPrice) {
                    mod.price.push(data.MinSkuPrice.toFixed(2));
                    minMarketPrice && (data.MinSkuPrice.toFixed(2) != minMarketPrice.toFixed(2)) && mod.oldPrice.push(minMarketPrice.toFixed(2));
                } else {
                    mod.price.push(data.DiscountPrice.toFixed(2));
                    (minPrice.toFixed(2) != data.DiscountPrice.toFixed(2)) && mod.oldPrice.push(minPrice.toFixed(2));
                }
            }
        } else if (data.Intensity < 10) { //折扣
            mod.price.push((minPrice * (data.Intensity / 10)).toFixed(2));
            if (minPrice < maxPrice) {
                mod.price.push((maxPrice * (data.Intensity / 10)).toFixed(2));
                mod.oldPrice.push(minPrice.toFixed(2));
                mod.oldPrice.push(maxPrice.toFixed(2));
            } else {
                mod.oldPrice.push(minPrice.toFixed(2));
            }
        } else {
            //如果存在sku属性值
            if (data.MinSkuPrice > 0 && data.MinSkuPrice < data.MaxSkuPrice) {
                mod.price.push(data.MinSkuPrice.toFixed(2));
                if (data.PromotionType === 2 || data.PromotionType === 5) { //预约、预售显示最小价格
                    minMarketPrice && mod.oldPrice.push(minMarketPrice.toFixed(2));
                } else {
                    mod.price.push(data.MaxSkuPrice.toFixed(2));
                    if (minMarketPrice < maxMarketPrice) {
                        mod.oldPrice.push(minMarketPrice.toFixed(2));
                        mod.oldPrice.push(maxMarketPrice.toFixed(2));
                    } else {
                        minMarketPrice && mod.oldPrice.push(minMarketPrice.toFixed(2));
                    }
                }
            } //如果存在sku金采团购活动属性值
            if (data.MinJcSkuPrice > 0 && data.MinJcSkuPrice < data.MaxJcSkuPrice) {
                mod.price.push(data.MinJcSkuPrice.toFixed(2));
                mod.price.push(data.MaxJcSkuPrice.toFixed(2));
                if (minMarketPrice < maxMarketPrice) {
                    mod.oldPrice.push(minMarketPrice.toFixed(2));
                    mod.oldPrice.push(maxMarketPrice.toFixed(2));
                } else {
                    minMarketPrice && mod.oldPrice.push(minMarketPrice.toFixed(2));
                }
            } else {
                mod.price.push(minPrice.toFixed(2));
                if (data.PromotionType === 2 || data.PromotionType === 5) { //预约、预售显示最小价格
                    minMarketPrice && mod.oldPrice.push(minMarketPrice.toFixed(2));
                } else {
                    (minPrice < maxPrice) && mod.price.push(maxPrice.toFixed(2));
                    if (minMarketPrice < maxMarketPrice) {
                        mod.oldPrice.push(minMarketPrice.toFixed(2));
                        mod.oldPrice.push(maxMarketPrice.toFixed(2));
                    } else {
                        minMarketPrice && mod.oldPrice.push(minMarketPrice.toFixed(2));
                    }
                    //金采团购活动
                    if (data.MinJcSkuPrice > 0) {
                        mod.price = [];
                        mod.price.push(data.MinJcSkuPrice.toFixed(2));
                    }
                }
            }
        }
        var price_text = [];
        for (var i = 0; i < mod.price.length; i++) {
            price_text.push(mod.price[i].split('.')[0] + '.<span class="font11">' + mod.price[i].split('.')[1] + '</span>');
        }
        $('#price').html(price_text.join('~'));
        $('#appointment-price').text(mod.price.join('~'));
        if (mod.oldPrice.length) {
            $('#olderPrice').text('￥' + mod.oldPrice.join('~'));
            $('#appointment-older-price').text('￥' + mod.oldPrice.join('~'));
        }
        mod.realPrice = mod.price[0];
        mod.commodityUpInfo.realPrice = mod.realPrice;
        $('#money').html(price_text[0]);
    };
    /**
    * 更新属性选择弹窗中的价格
    */
    mod.updatePrice = function () {
        var price = mod.price[0];
        var thumImg = mod.thumImg;
        //        //判断价格是否是区间值：如果是并且两种属性已经选择了，则显示对应的值
        //        if (mod.price.length == 2) { //价格是区间值
        if (this.attributeTitle.length == 2) {
            if (mod.currentColor && mod.currentSize) { //两种属性已经选择了
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    var commodity = mod.commodityStocks[i];
                    if ((commodity.ComAttribute[0].SecondAttribute == mod.currentColor && commodity.ComAttribute[1].SecondAttribute == mod.currentSize) || (commodity.ComAttribute[1].SecondAttribute == mod.currentColor && commodity.ComAttribute[0].SecondAttribute == mod.currentSize)) {
                        mod.commodityUpInfo.price = commodity.Price;
                        thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                        mod.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : [];
                        if (mod.vipIntensity != null) {
                            price = (commodity.Price * (mod.vipIntensity / 10)).toFixed(2);
                        } else if (mod.Intensity < 10) { //有折扣
                            price = (commodity.Price * (mod.Intensity / 10)).toFixed(2);
                        } else {
                            price = commodity.Price.toFixed(2);
                        }
                        if (mod.DiyGroupPromotion != null && mod.PromotionId == null) {
                        } else {
                            if (mod.SkuActivityCdtos && mod.SkuActivityCdtos.length > 0) {
                                for (var j = 0; j < mod.SkuActivityCdtos.length; j++) {
                                    if (mod.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                        price = mod.SkuActivityCdtos[j].JoinPrice.toFixed(2);
                                    }
                                }
                            } else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    price = mod.DiscountPrice.toFixed(2);
                                }
                            }
                            //金采团购活动价格
                            if (mod.JCActivityItemsListCdtos && mod.JCActivityItemsListCdtos.length > 0) {
                                for (var j = 0; j < mod.JCActivityItemsListCdtos.length; j++) {
                                    if (mod.JCActivityItemsListCdtos[j].ComdtyStockId === commodity.Id) {
                                        price = mod.JCActivityItemsListCdtos[j].GroupPrice.toFixed(2);
                                    }
                                }
                            } else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    price = mod.DiscountPrice.toFixed(2);
                                }
                            }
                        }
                    }
                }
            }
        } else if (this.attributeTitle.length == 1) {
            if (mod.currentColor && mod.commodityStocks.length) {
                for (var i = 0; i < mod.commodityStocks.length; i++) {
                    var commodity = mod.commodityStocks[i];
                    if (commodity.ComAttribute[0].SecondAttribute == mod.currentColor) {
                        mod.commodityUpInfo.price = commodity.Price;
                        thumImg = commodity.ThumImg ? commodity.ThumImg : thumImg;
                        mod.carouselImgs = commodity.CarouselImgs.length ? commodity.CarouselImgs : [];
                        if (mod.vipIntensity != null) {
                            price = (commodity.Price * (mod.vipIntensity / 10)).toFixed(2);
                        } else if (mod.Intensity < 10) { //有折扣
                            price = (commodity.Price * (mod.Intensity / 10)).toFixed(2);
                        } else {
                            price = commodity.Price.toFixed(2);
                        }
                        if (mod.DiyGroupPromotion != null && mod.PromotionId == null) {
                        } else {
                            if (mod.SkuActivityCdtos && mod.SkuActivityCdtos.length > 0) {
                                for (var j = 0; j < mod.SkuActivityCdtos.length; j++) {
                                    if (mod.SkuActivityCdtos[j].CommodityStockId === commodity.Id) {
                                        price = mod.SkuActivityCdtos[j].JoinPrice.toFixed(2);
                                    }
                                }
                            }
                            else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    price = mod.DiscountPrice.toFixed(2);
                                }
                            }
                            //金采团购活动价格
                            if (mod.JCActivityItemsListCdtos && mod.JCActivityItemsListCdtos.length > 0) {
                                for (var j = 0; j < mod.JCActivityItemsListCdtos.length; j++) {
                                    if (mod.JCActivityItemsListCdtos[j].ComdtyStockId === commodity.Id) {
                                        price = mod.JCActivityItemsListCdtos[j].GroupPrice.toFixed(2);
                                    }
                                }
                            }
                            else {
                                if (mod.DiscountPrice.toFixed(2) > -1) {
                                    price = mod.DiscountPrice.toFixed(2);
                                }
                            }
                        }
                    }
                }
            }
        }
        //}
        $('#pic').attr('src', thumImg);
        $('#money').html(price.split('.')[0] + '.<span class="font11">' + price.split('.')[1] + '</span>');
        mod.realPrice = price;
        mod.commodityUpInfo.realPrice = mod.realPrice;
    };
    /**
    * 更新属性弹窗中的商品选择的数量
    */
    mod.updateNumber = function (number, isInput) {
        var number = number || $('#modal-wrap-count').val();
        var stock = mod.stock || 0;
        var falg = false;
        //判断是否超出限购数量
        if (mod.limitBuyEach !== null && mod.limitBuyEach !== -1 && number > mod.limitBuyEach) {
            number = mod.limitBuyEach;
            falg = true;
        }
        if (mod.attributeTitle.length == 2 && mod.currentColor && mod.currentSize) {
            var obj = mod.getCommodity(mod.currentColor, mod.currentSize, false);
            if (obj) {
                stock = obj.Stock;
            }
        }
        if (mod.attributeTitle.length == 1 && mod.currentColor) {
            var obj = mod.getCommodity(mod.currentColor, mod.currentSize, true);
            if (obj) {
                stock = obj.Stock;
            }
        }
        if (stock < number) {
            number = stock;
            falg = true;
        }
        if (falg && isInput) {
            toast("购买数量超出范围");
        }
        $('#modal-wrap-count').val(number);
    };
    /**
    * 设置评价组件
    */
    mod.setEvaluate = function (HasReviewFunction, Score) {
        if (HasReviewFunction) { //配置了“商品评价”组件
            if (Score.Records.length > 0) {
                $('#recevied').text(100 * (Score.Evaluate.GoodCount / Score.TotalCount).toFixed(2) + "%");
                $('#reviewCount').text(Score.TotalCount);
                $('#comment-name').text(Score.Records[0].UserName);
                $('#comment-content').text(Score.Records[0].Content);
                $("#detail-evaluate").removeClass('hide');
                $("#unevaluate").addClass('hide');
            } else {
                $("#detail-evaluate").addClass('hide');
                $("#unevaluate").removeClass('hide');
            }
        } else { //定制平台没有配置“商品评价”组件
            $("#detail-evaluate").addClass('hide');
            $("#unevaluate").addClass('hide');
        }
        var commodityId = sessionStorage.commodityId_2 || getQueryString('commodityId');
        var url = SNSUrl + "/Evaluate/List?appId=" + sessionStorage.appId + "&productId=" + commodityId;
        $('#more').attr('href', url);
    };
    /**
    * 会员价促销
    */
    mod.setVipActive = function (VipPromotion) {
        if (VipPromotion.IsVipActive && VipPromotion.Intensity < 10) { //有促销活动
            $('#discount-wrap').removeClass('hide'); //显示促销活动
            $('#coupon-wrap').removeClass('hide');
            if (!isLogin()) { //登录
                if (VipPromotion.IsVip) { //是会员并且有会员折扣
                    var vipPrice = [];
                    for (var i = 0; i < mod.price.length; i++) {
                        vipPrice.push((mod.price[i] * (VipPromotion.Intensity / 10)).toFixed(2));
                    }
                    mod.vipIntensity = VipPromotion.Intensity;
                    $('#discount-wrap').text('您可享受至尊VIP会员价￥' + vipPrice.join('~'));
                    $('#discount-wrap').addClass('vip');
                } else { //不是会员
                    $('#discount-wrap').text('您不是会员');
                }
            } else { //未登录
                $('#discount-wrap').text('登录可查看会员优惠信息');
            }
        } else {
            $('#discount-wrap').addClass('hide'); //隐藏促销活动
        }
    };
    /**
    * 设置直播和720云景按钮
    */
    mod.setBtn = function (EquipmentUrl, CloudviewUrl) {
        EquipmentUrl && $('#store-info').append('<a href="' + EquipmentUrl + '" class="btn">直播</a>');
        CloudviewUrl && $('#store-info').append('<a href="' + CloudviewUrl + '" class="btn">720°云景</a>');
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
        var html = '<div class="modal-content-bd-title">' + title + '</div><ul id="' + id + '" class="clearfix">';
        for (var j = 0; j < data.length; j++) {
            var flag = true;
            for (var k = 0; k < disAttribute.length; k++) {
                if (data[j] == disAttribute[k]) {
                    html += '<li class="modal-content-bd-item disable">' + data[j] + '</li>';
                    flag = false;
                    break;
                }
            }
            if (flag) {
                if (currentSelect == data[j]) {
                    html += '<li class="modal-content-bd-item active">' + data[j] + '</li>';
                } else {
                    html += '<li class="modal-content-bd-item">' + data[j] + '</li>';
                }
            }
        }
        return html + '</ul>';
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
        var Specificationshtml = '';
        if (mod.allColor.length) {
            html += mod.renderColorSize('color', mod.attributeTitle[0], mod.allColor, '', []);
        }
        if (mod.allSize.length) {
            html += mod.renderColorSize('size', mod.attributeTitle[1], mod.allSize, '', []);
        }
        if (mod.allSpecifications) {

            Specificationshtml += mod.renderSpecifications(mod.allSpecifications, '');
        }
        $('#attribute').empty().append(html);
        $('#Specifications').empty().append(Specificationshtml);
        mod.updatePrice();
        mod.updateDuty();
        //属性选择事件绑定
        $('#modal-wrap').on('click', '.modal-content-bd-item', function (event) {
            var target = event.target;
            if ($(target).hasClass('disable')) {
                return;
            } else {
                if ($(target).parent()[0].id == 'color') {
                    mod.currentClick = $(target).text();
                    mod.updateColorSize(target, 'color');
                } else if ($(target).parent()[0].id == 'size') {
                    mod.currentClick = $(target).text();
                    mod.updateColorSize(target, 'size');
                }
                mod.updatePrice();
                mod.updateDuty();
                mod.updateNumber(null, false);
            }
        });


        //规格设置选择事件绑定
        $('#Specifications').on('click', '.modal-content-bd-item', function (event) {
            var target = event.target;
            if ($(target).hasClass('disable')) {
                return;
            } else {
                //规格设置
                mod.currentSpecifications = $(target).text();
                mod.updateSpecifications(target);
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
            if ($(target).hasClass('active')) { //取消选中的属性
                if (mod.allColor.length) {
                    if (selectAttr == 'color') {
                        mod.currentColor = '';
                        html += this.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, this.getDisableAttr('color'));
                    } else {
                        html += this.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, []);
                    }
                }
                if (mod.allSize.length) {
                    if (selectAttr == 'size') {
                        mod.currentSize = '';
                        html += this.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, this.getDisableAttr('size'));
                    } else {
                        html += this.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, []);
                    }
                }
            } else { //选择属性
                if (mod.allColor && mod.allColor.length) {
                    if (selectAttr == 'color') {
                        mod.currentColor = $(target).text();
                        html += this.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, this.getDisableAttr('color'));
                    } else {
                        html += this.renderColorSize('color', mod.attributeTitle[0], mod.allColor, mod.currentColor, this.getDisableAttr(mod.commodityStocks, mod.attributeTitle[1]));
                    }
                }
                if (mod.allSize && mod.allSize.length) {
                    if (selectAttr == 'size') {
                        mod.currentSize = $(target).text();
                        html += this.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, this.getDisableAttr('size'));
                    } else {
                        html += this.renderColorSize('size', mod.attributeTitle[1], mod.allSize, mod.currentSize, this.getDisableAttr(mod.commodityStocks, mod.attributeTitle[0]));
                    }
                }
            }
            $('#attribute').empty();
            $('#attribute').append(html);
        } catch (e) { };
        return;
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
                    html += this.renderSpecifications(mod.allSpecifications, '');
                }

            } else { //选择属性
                if (mod.allSpecifications.length) {
                    mod.currentSpecifications = $(target).text();
                    html += this.renderSpecifications(mod.allSpecifications, mod.currentSpecifications);
                }

            }
            $('#Specifications').empty();
            $('#Specifications').append(html);
        } catch (e) { };
        return;
    };




    mod.showSateByPromotion = function (data) {
        var nowDate = data.CurrentTime ? data.CurrentTime.split("(")[1].split(")")[0] : (new Date()).getTime(); //(new Date()).getTime();
        var lastTime = 0;
        // 距离预约结束还剩的时间
        sessionStorage.presellEndTime = this.getRemainingTime(data.PresellEndTime, lastTime, nowDate);
        lastTime = sessionStorage.presellEndTime == 0 ? 0 : data.PresellEndTime;
        // 距离抢购开始(距秒杀开始) 还剩的时间
        sessionStorage.promotionStartTime = this.getRemainingTime(data.PromotionStartTime, lastTime, nowDate);
        lastTime = sessionStorage.promotionStartTime == 0 ? 0 : data.PromotionStartTime;
        // 距离抢购结束（距秒杀结束）还剩的时间
        sessionStorage.promotionEndTime = this.getRemainingTime(data.PromotionEndTime, lastTime, nowDate);
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
                this.setVipActive(data.VipPromotion);
                break;
            case 1: // 秒杀
                SECKILL.updateSeckillState(data.PromotionState, hasStock);
                break;
            case 2: //预约
                var preselledNum = data.PreselledNum || 0;
                $('#preselled-num').text(preselledNum + '人已预约');
                PRESELL.updateAppointmentState(data.PromotionState, hasStock);
                break;
            case 3: //团购（先不做）
                break;
            case 5: //预售
                $('#reserver-wrap').removeClass('hide').text('[预售] 发货时间：' + data.DeliveryTime);
                var surplusLimitBuyTotal = data.SurplusLimitBuyTotal || 0;
                $('#preselled-num').text(surplusLimitBuyTotal + '人已预定');
                RESERVER.updateReserverState(data.PromotionState, hasStock);
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
    * 相关商品
    */
    mod.setRelationCommodity = function (comdtyList) {
        if (comdtyList && comdtyList.length) {
            var html = '';
            for (var i = 0, len = comdtyList.length; i < len; i++) {
                var item = comdtyList[i];
                var price = 0;
                var oldPrice = 0;
                if (item.DiscountPrice > 0) {
                    price = item.DiscountPrice.toFixed(2);
                    oldPrice = item.Price.toFixed(2);
                } else if (item.Intensity < 10) {
                    price = (item.Price * (data.Intensity / 10)).toFixed(2);
                    oldPrice = item.Price.toFixed(2);
                } else {
                    if (item.MarketPrice && item.MarketPrice != null && item.MarketPrice != 'null') {
                        price = item.Price.toFixed(2);
                        oldPrice = item.MarketPrice.toFixed(2);
                    } else {
                        price = item.Price.toFixed(2);
                    }
                }
                html += '<a href="javascript:void(0);" class="recommendPortraitTwo-lists-item" data-id="' + item.RelationCommodityId + '">' +
                    '<div class="pic"><img src="' + item.CommodityPicturesPath + '"></div>' +
                    '<div class="pro_info">' +
                    '<p class="text-overflow-multi">' + item.Name + '</p>';
                if (price) {
                    html += '<span class="actual_price"><span class="font11">￥</span>' + price.split('.')[0] + '.<span class="font11">' + price.split('.')[1] + '</span></span>';
                }
                if (oldPrice) {
                    html += '<span class="old_price" style="margin-left: 10px;">￥' + oldPrice + '</span>';
                }
                html += '</div></a>';
            }
            $('#comdty-list').append(html);
            $('#comdty-wrap').removeClass('hide');
        } else {
            $('#comdty-wrap').addClass('hide');
        }
        $('#comdty-wrap').on('click', '.recommendPortraitTwo-lists-item', function () {
            window.location.href = urlAppendCommonParams('/Mobile/CommodityDetail?commodityId=' + $(this).data('id') + '&type=show')
        })
    };
    /**
    * 倒计时
    */
    mod.countdown = function (time, timeElement, callback) {
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
                timeElement.text(day + '天');
            } else {
                timeElement.text(hour + ':' + minute + ':' + second);
            }
        }, 1000);
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
                toast('您没有获得预约码，不能购买');
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
                $(el).addClass('disabled').text('已领取');
                $(el).parent().prev().addClass("used");
            } else {
                toast(data.Info);
            }
        }).fail(function (err) {
            toast('优惠券领取失败，请稍后重试!');
            $('#mask').addClass('hide');
        });
    };
    /**
    * 初始化赠品
    */
    mod.showPresent = function (data) {
        $('#present-tips').text(data.Title);
        var begin = new Date(parseInt(data.BeginTime.split('Date(')[1].split(')/')[0]));
        var end = new Date(parseInt(data.EndTime.split('Date(')[1].split(')/')[0]));
        $('#startTime').text(dateFormat(begin, 1));
        $('#endTime').text(dateFormat(end, 1));
        var html = '';
        for (var i = 0, len = data.Items.length; i < len; i++) {
            var item = data.Items[i];
            html += '<div class="present-list" data-id="' + item.Id + '">' +
                        '<div class="present-list-text">' +
                            '<div class="present-list-title">' + item.Name + '</div>' +
                            '<div class="present-list-size">' +
                                '<span>' + showSize(item.SKU) + '</span>' +
                                '<span>x' + item.Number + '</span>' +
                            '</div>' +
                        '</div>' +
                    '</div>';
        }
        $('#present-list-content').html(html);
        function showSize(size) {
            var str = '';
            if (size) {
                for (var i = 0; i < size.length; i++) {
                    str += size[i].SecondAttribute + ' '
                }
            }
            return str;
        }
    };
    /**
    * 关闭属性弹窗
    */
    mod.closeModal = function () {
        $('#modal-wrap').addClass('hide');
        $('body').removeClass('lock');
        $('#modal-wrap-count').val(1);
    };
    mod.transformTime = function (time) {
        var date = new Date(parseInt(time.split('Date(')[1].split(')/')[0]));
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        return year + '年' + month + '月' + day + '日';
    }
    mod.initEvent = function () {
        //页面数据加载完成后显示区域的高度设置成第一屏页面的高度
        this.upadteContentHeight($('#page1').height());
        this.event();
        SCROLLANIMATE.initEvent();
    };
    return mod;
})(window.DETAIL || {}, window.Zepto);
/**
 * 下拉查看商品详情滚动动画
 */
var SCROLLANIMATE = (function (mod, $) {
    var _getPoint = function (e) {
        var point = e.touches ? e.touches[0] : e;
        return {
            x: point.pageX,
            y: point.pageY
        };
    };
    mod.initEvent = function () {
        /**
         * 上拉查看详情
         */
        $('#page1').on('touchstart', function () {
            mod.startPos = _getPoint(event);
            mod.state = false;
        });
        $('#page1').on('touchmove', function () {
            var childrenHeight = $(document).height();
            var meHeight = $(window).height();
            var scrollTop = $(window).scrollTop();
            mod.endPos = _getPoint(event);
            var moveY = mod.startPos.y - mod.endPos.y;
            if (childrenHeight <= (meHeight + scrollTop)) {
                mod.state = true;
                mod.offsetY = moveY * 0.2;
                $('#content').height($('#content').height() - 0 + mod.offsetY);
            }
        });
        $('#page1').on('touchend', function (event) {
            if (mod.state) {
                var offsetBottom = $('#content').height() - $(window).scrollTop() - $(window).height(); //底部剩余距离
                var offset = $('#content').height() - $('#page1').height() - 41 - offsetBottom;
                DETAIL.upadteContentHeight($('#page1').height());
                if (offset > 20) {
                    $('.page2-header').addClass('active'); //显示header
                    $('.page2-nav').removeClass('hide');
                    $('.backtotop-btn').addClass('active'); //显示header
                    $('#page2').removeClass('hide'); //显示第二屏内容
                    setTimeout(function () {
                        $('#summary').css('transform', 'translateY(-' + ($('#page1').height() - 36) + 'px)');
                        DETAIL.upadteContentHeight($('#page2').height() + 43); //更新显示区域的高度
                        $(window).scrollTop(0);
                    })
                }
                mod.offsetY = 0;
            }
        });
        if ((navigator.platform.indexOf('Win') == 0) || (navigator.platform.indexOf('Mac') == 0)) {
            $('.pull-detail').on('click', function () {
                $('#summary').css('transform', 'translateY(-' + ($('#page1').height() - 36) + 'px)');
                $('.page2-header').addClass('active'); //显示header
                $('.page2-nav').removeClass('hide');
                $('.backtotop-btn').addClass('active'); //显示header
                $('#page2').removeClass('hide'); //显示第二屏内容
                setTimeout(function () {
                    DETAIL.upadteContentHeight($('#page2').height() + 43); //更新显示区域的高度
                }, 1000);
                $(window).scrollTop(0);
            });
        }
        /**
         * 下拉返回商品详情
         */
        $('#page2').on('touchstart', function (event) {
            mod.startPos = _getPoint(event);
        });
        $('#page2').on('touchmove', function (event) {
            mod.endPos = _getPoint(event);
            var moveY = mod.endPos.y - mod.startPos.y;
            if (moveY > 0 && ($(window).scrollTop() <= 0)) { //向下拉动
                mod.offsetY = moveY * 0.2;
                if (mod.offsetY > 50) {
                    $('#pullDown').text('释放返回商品详情');
                } else {
                    $('#pullDown').text('下拉返回商品详情');
                }
                $('#page2').css('padding-top', mod.offsetY)
            }
        });
        $('#page2').on('touchend', function (event) {
            $('#page2').css('padding-top', '0');
            if (mod.offsetY > 10) {
                DETAIL.backDetail();
            }
            mod.offsetY = 0;
        });
        $('#present-list-content').on('click','.present-list',function(){
            window.location.href = urlAppendCommonParams('/Mobile/CommodityDetail?commodityId=' + $(this).attr("data-id") + '&shopId=' + getEsAppId());
        });
    };
    return mod;
})(SCROLLANIMATE || {}, window.Zepto);
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
    mod.updateSeckillState = function (promotionState,hasStock) {
        switch (promotionState) {
            case 2: //2：等待抢购：
                //显示倒计时
                $('#seckill').removeClass('hide');
                $('#remind').removeClass('hide');
                $('#add').addClass('hide');
                $('#buyBtn').addClass('hide');
                DETAIL.countdown(sessionStorage.promotionStartTime, $('#seckill-time'), function () {
                    mod.updateSeckillState(3, hasStock);
                });
                break;
            case 3: //3：活动进行中，
                if(hasStock){
                    $('#add').removeClass('hide');
                    $('#buyBtn').removeClass('hide');
                    $('#seckill').addClass('hide');
                    $('#remind').addClass('hide');
                }else{
                    $('#add').removeClass('hide').addClass('disabled');
                    $('#buyBtn').removeClass('hide').addClass('disabled');
                    $('#robbed').removeClass('hide').text('已抢光');
                }
                DETAIL.countdown(sessionStorage.promotionEndTime, $('#seckill-time'), function () {
                    mod.updateSeckillState(4);
                });
                break;
            case 4:
                $('#add').removeClass('hide').addClass('disabled');
                $('#buyBtn').removeClass('hide').addClass('disabled');
                $('#robbed').removeClass('hide').text('已抢光');
                break;
        }
    };
    return mod;
})(window.SECKILL || {}, window.Zepto);

/**
 * 预约详情
 */
window.PRESELL = (function (mod, $) {
    mod.isPresell = false; //是否预约过
    /**
     * 预售活动状态更新
     */
    mod.updateAppointmentState = function (promotionState,hasStock) {
        $('#nav').find('.btn').addClass('hide');
        $('#appointment-wrap').removeClass('hide');
        $('#d-price').addClass('hide');
        switch (promotionState) {
            case 1: //1:预约预售进行中，（预约开始）
                $('#appointment-text').text('距预约结束');
                $('#appointment').removeClass('hide');
                DETAIL.countdown(sessionStorage.presellEndTime, $('#appointment-day'), function () {
                    mod.updateAppointmentState(2);
                });
                break;
            case 2: //2：等待抢购：
                $('#appointment-text').text('距抢购开始');
                DETAIL.countdown(sessionStorage.promotionStartTime, $('#appointment-day'), function () {
                    mod.updateAppointmentState(3);
                });
                $('#buyBtn').removeClass('hide');
                break;
            case 3: //3：活动进行中，
                $('#appointment-text').text('距抢购结束');
                DETAIL.countdown(sessionStorage.promotionEndTime, $('#appointment-day'), function () {
                    mod.updateAppointmentState(4);
                });
                $('#buyBtn').removeClass('hide');
                if(hasStock){
                    $('#buyBtn').removeClass('hide');
                }else{ //抢光了，但活动未结束
                    $('#buyBtn').removeClass('hide').addClass('disabled');
                    $('#robbed').removeClass('hide').text('已抢光');
                }
                break;
            case 4:
                $('#appointment-text').text('活动已结束');
                $('#buyBtn').removeClass('hide').addClass('disabled');
                $('#robbed').removeClass('hide').text('已抢光');
                break;
        }
        mod.initEvent();
    };
    mod.initEvent = function () {
        //        if (isLogin()) {
        //            $.ajax({
        //                url: '/mobile/CheckMyPresellComdty',
        //                dataType: "json",
        //                data: {
        //                    userId: getUserId(),
        //                    presellComdtyId: DETAIL.outPromotionId
        //                }
        //            }).done(function (data) {
        //                if (data.isSuccess == false) { //已经预约过
        //                    mod.isPresell = true;
        //                }
        //            });
        //        }
        //        //立即预约按钮事件绑定
        //        $('#appointment').on('click', function () {
        //            //判断是否登录,未登录跳转到登录页
        //            DealLoginPartial.initPartialPage();
        //            $("#identify-img").attr("src", getBtpDomain() + "mobile/GetVerifyCodeZPH?r=" + Math.random());
        //            //判断是否已经预约过
        //            if (mod.isPresell) { //已经预约过
        //                $('#appointment-modal-text').text('您已经成功预约过了，无需重复预约，请关注抢购时间~~~');
        //                $('#appointment-modal').removeClass('hide');
        //                $('body').addClass('lock');
        //            } else { //未预约过
        //                $('#identify-modal').removeClass('hide');
        //                $('body').addClass('lock');
        //            }
        //        });
        $('.identify-btn-cancel').on('click', function () {
            $('#identify-modal').addClass('hide');
            $('body').removeClass('lock');
        });
        $('.appointment-btn-cancel').on('click', function () {
            $('#appointment-modal').addClass('hide');
            $('body').removeClass('lock');
        });
        //我的预约按钮
        $('.appointment-btn-submit').on('click', function () {
            window.location = zphUrl + "Presell/MyPresellComdty?userId=" + getUserId() + "&sessionId=" + getSessionId() + "&ChangeOrg=00000000-0000-0000-0000-000000000000&btpdomain=" + window.location.host;
        });
        //点击验证码弹窗中的提交按钮
        $('.identify-btn-submit').on('click', function () {
            var value = $.trim($('#identify-val').val()),
                outPromotionId = DETAIL.outPromotionId;
            if (!value) {
                toast('请输入验证码');
                return;
            }
            $('#mask').removeClass('hide').find('p').text('正在提交。。。');
            // 验证码
            $.ajax({
                url: '/mobile/SaveMyPresellComdtyZPH',
                data: {
                    outPromotionId: outPromotionId,
                    userId: getUserId(),
                    sessionId: getSessionId(),
                    verifyCode: value,
                    esAppId: getEsAppId(),
                    commodityId: getQueryString('commodityId'),
                    commodityStockId: DETAIL.commodityStockId
                },
                success: function (result) {
                    $('#mask').addClass('hide');
                    $("#identify-img").attr("src", getBtpDomain() + "mobile/GetVerifyCodeZPH?r=" + Math.random());
                    $('#identify-val').val("");
                    //0，成功，1 已预约过，2 失败
                    if (result.ResultCode === 2) {
                        toast(result.Message);
                    } else if (result.ResultCode === 0) {
                        $('#identify-modal').addClass('hide');
                        $('#appointment-modal-text').text('商品预约成功，您已经获得抢购资格，请关注抢购时间~~~');
                        $('#appointment-modal').removeClass('hide');
                    }
                },
                error: function () {
                    $('#mask').addClass('hide');
                    toast('服务器繁忙，请稍后重试！');
                }
            });
        });
    };
    return mod;
})(window.PRESELL || {}, window.Zepto);
/**
 * 预售
 */
window.RESERVER = (function(mod,undefined){
    /**
     * 更新预售状态
     */
    mod.updateReserverState = function(promotionState,hasStock){
        $('#nav').find('.btn').addClass('hide');
        $('#appointment-wrap').removeClass('hide');
        $('#d-price').addClass('hide');
        $('#add').addClass('hide');
        $('#buyBtn').addClass('hide');
        $('#reserver').removeClass('hide');
        switch (promotionState) {
            case 3: //1:预售进行中，（预售开始）
                $('#appointment-text').text('距预定结束');
                DETAIL.countdown(sessionStorage.promotionEndTime, $('#appointment-day'), function () {
                    mod.updateReserverState(4);
                });
                if(hasStock){
                    $('#reserver').removeClass('disabled').text('立即预定');
                }else{//库存不足或到达限购数量
                    $('#reserver').addClass('disabled').text('已抢光');
                    $('#robbed').removeClass('hide').text('已抢光');
                }
                break;
            case 4: //活动结束
                $('#appointment-text').text('活动已结束');
                $('#reserver').addClass('disabled').text('已抢光');
                $('#robbed').removeClass('hide').text('已抢光');
                break;
        }
    };
    return mod;
})(window.RESERVER || {}, window.Zepto);
/**
 * 入口
 */
$(document).ready(function () {
    FastClick.attach(document.body);
    //校验h5的参数
    checkMobileParams();
    sessionStorage.commodityId_2 = getQueryString('commodityId');
    DETAIL.setProvince();
});
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
        } else {
            for (var i = 0; i < arguments.length; i++) {
                if (arguments[i] != undefined) {
                    var reg = new RegExp("({[" + i + "]})", "g");
                    result = result.replace(reg, arguments[i]);
                }
            }
        }
    }
    return;
}
