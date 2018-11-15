define(["vue", "vueresource", "zepto", 'Commons', 'AppVm', 'ProVm', 'APP', 'PRO', 'CookieContextDTO', 'PRICE', 'swiper', 'scroll'],
    function (Vue) {

        /*
        从sessionStorage中接收店铺的总体数据
        */
        function loadSessionStorages() {

            var storeInfo = JSON.parse(sessionStorage.storeInfo_CY);

            var appId = sessionStorage.appId_CY;
            var shopId = sessionStorage.appId;
            var shopIsWorking = sessionStorage.IsWork;
            var deliveryCondition = sessionStorage.deliveryCondition == undefined ? JSON.parse(sessionStorage.storeInfo_CY).setting.CateringSetting.DeliveryAmount : sessionStorage.deliveryCondition;
            var deliveryFee = storeInfo.setting.CateringSetting.DeliveryFee;
            var freeDeliveryCondition = storeInfo.setting.CateringSetting.FreeAmount; ;
            var userId = getUserId();
            userId = Boolean(userId) ? userId : "00000000-0000-0000-0000-000000000000";

            return {
                appId: appId,
                shopId: shopId,
                shopIsWorking: shopIsWorking,
                deliveryCondition: deliveryCondition,
                deliveryFee: deliveryFee,
                freeDeliveryCondition: freeDeliveryCondition,
                userId: userId
            };
        };

        /*
        从服务器端获取商品列表，并将其map为ProVm列表
        */
        function getProVms(url, shopId, userId) {
            var response = COMMONS.postAjax(url, { appId: shopId, userId: userId }, false);
            var originDatas = response;
            if (typeof (response) == "string")
                originDatas = JSON.parse(response);
            window.originProDatas = originDatas.CommodityList;
            return PRO.mapToProductVms(originDatas);
        }

        function appendShopCartDate(cart, cartpros, proDtos) {

            var data = [];
            var appData = {
                AppId: cart.shopId,
                AppName: '', //storeInfo.StoreName,
                RealPrice: cart.amount,
                AppAmount: cart.amount,
                ShoppingCartItemSDTO: [],
                CommodityNum: cart.count,
                Price: cart.amount,
                BoxAmount: cart.boxAmount,
                DeliveryFee: cart.deliveryFee,
                RealDeliveryFee: cart.realDeliveryFee,
                AllAmount: COMMONS.formatPrice(cart.amount + cart.boxAmount, true)
            };
            for (var i = 0; i < cartpros.length; i++) {
                var cartpro = cartpros[i];
                var dto = COMMONS.findFirstByPropVal(proDtos, "Id", cartpro.proId);
                dto.CommodityNumber = cartpro.count;
                appData.ShoppingCartItemSDTO.push(dto);
            }
            data.push(appData);
            sessionStorage.ShopCartDate = JSON.stringify(data);
        }

        function sleep(d) {
            for (var t = Date.now(); Date.now() - t <= d; );
        }

        function jumpToOrder() {
            console.log("跳转至：" + window.newUrl);
            window.location.href = window.newUrl;
        }

        /*
        创建Vue对象
        */
        function createVm(appVm, cartVm, productVms) {
            var defaultPro = productVms[0];
            var defaultKpro = defaultPro.kPro;
            return new Vue({
                data: {
                    app: appVm,
                    pros: productVms,
                    deletedPros: new Array(),
                    currentKpro: defaultKpro,
                    currentPro: defaultPro,
                    cart: cartVm,
                    Wrapper: null,
                    Sorts: null,
                    shoppingCartState: false,
                    specState: false,
                    scrollState: false,
                    suctionTip: '',
                    current: 0
                },
                computed: {
                    categorys: function () {
                        return APP.mapToCategoryVms(this.pros);
                    },
                    cartpros: function () {
                        var ps = PRO.mapToCartproVms(this.pros);
                        if (this.deletedPros.length > 0) {
                            ps = ps.concat(this.deletedPros);
                            //for (var dp in this.deletedPros) {
                            //    var dPro = this.deletedPros[dp];
                            //    ps.push(this.deletedPros);
                            //}
                        }
                        return ps;
                    }
                },
                created: function () {
                    var storageData = sessionStorage.getItem("cart-" + this.app.shopId);
                    if (storageData) {
                        var storagePros = JSON.parse(storageData);
                        APP.updateProsByStorage(this.cart, this.pros, this.deletedPros, storagePros);
                    }
                },
                mounted: function () {
                    this.initWindow();
                    this.resizeWindow();
                    this.notify();
                    this.iScroll();
                },
                methods: {
                    initWindow: function () {
                        var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
                        document.documentElement.style.fontSize = deviceWidth / 6.4 + 'px'; //如果设计图是320的话就除以3.2
                    },
                    resizeWindow: function () {
                        var _this = this;
                        window.addEventListener('resize', function () {
                            _this.initWindow();
                        })
                    },
                    notify: function () {
                        if ($('.swiper-slide').length > 1) {
                            var swiper = new Swiper('.swiper-container', {
                                slidesPerView: 1,
                                direction: 'vertical',
                                paginationClickable: true,
                                spaceBetween: 30,
                                loop: true,
                                autoplay: 2500,
                                autoplayDisableOnInteraction: false
                            });
                        }

                    },
                    sortsTouch: function () {
                        this.scrollState = true;
                    },
                    iScroll: function () {
                        setTimeout(this._iScrollEvent, 100);
                        document.getElementById("wrapper").addEventListener('touchmove', function (e) { e.preventDefault(); }, false);
                        document.getElementById("sorts").addEventListener('touchmove', function (e) { e.preventDefault(); }, false);
                    },
                    _iScrollEvent: function () {
                        var map = [];
                        var _this = this;
                        var systitle = document.getElementsByClassName('systitle');
                        for (var i = 0; i < systitle.length; i++) {
                            var top = systitle[i].offsetTop;
                            var title = systitle[i].innerHTML;
                            map.push({ position: top, title: title })
                        }
                        this.Wrapper = new IScroll('#wrapper');
                        this.Sorts = new IScroll('#sorts', { probeType: 3, mouseWheel: true });
                        this.Sorts.on('scrollEnd', updatePosition);
                        this.Sorts.on('scroll', updatePosition);
                        $('#sorts').on('touchstart', function () { this.scrollState = true; })

                        function updatePosition() {
                            var current = Math.abs(this.y);
                            for (var i = 0, len = map.length; i < len; i++) {
                                if (map[i].position < current && _this.scrollState) {
                                    _this.current = i;
                                    var target = map[i].title;
                                    _this.suctionTip = target;
                                    _this.Wrapper.scrollToElement(target);
                                }
                            }
                        }
                    },
                    scrollSorts: function (id, index, name) {
                        var _this = this;
                        this.scrollState = false;
                        _this.current = index; //bug
                        this.suctionTip = name;
                        var target = document.getElementById(id);
                        this.Sorts.scrollToElement(target);
                    },
                    formatPrice: function (price) {
                        //对价格进行格式化，保留2位小数
                        //第2个参数表示：删除最末尾的0
                        return COMMONS.formatPrice(price, true);
                    },
                    //显示选规格
                    showSubpros: function (kPro) {
                        this.specState = true;
                        var pro = COMMONS.findFirstByPropVal(this.pros, "kPro", kPro);
                        PRO.printAboutSubpro(pro);
                        this.currentPro = pro;
                    },
                    //产品+、-
                    buy: function (kPro, isAdd, kSubpro) {
                        sleep(500); //停顿0.5秒
                        var pro = COMMONS.findFirstByPropVal(this.pros, "kPro", kPro);
                        PRO.buy(pro, isAdd, kSubpro);
                        var txt = PRO.printPro(pro);

                        //重新计算cart
                        APP.updateCartVm(this.cart, this.pros);

                        appendShopCartDate(this.cart, this.cartpros, window.originProDatas);
                        sessionStorage.setItem("cart-" + this.app.shopId, JSON.stringify(this.cartpros));
                    },
                    attrClick: function (kPro, kAttrVal) {
                        var pro = COMMONS.findFirstByPropVal(this.pros, "kPro", kPro);
                        PRO.attrClick(pro, kAttrVal);
                        var txt = PRO.printPro(pro);
                    },
                    payment: function () {
                        this.pros = getProVms('/Mobile/GetCateringCommodity', this.app.shopId, this.app.userId); ;
                        var storagepros = JSON.parse(sessionStorage.getItem("cart-" + this.app.shopId));
                        var errMsg = APP.updateProsByStorage(this.cart, this.pros, this.deletedPros, storagepros);
                        if (!errMsg && errMsg.length > 0) {
                            toast(errMsg);
                        } else {
                            var url = '/Mobile/CYCreateOrder?shopId=' + sessionStorage.appId + '&type=' + "gouwuche" + '&diyGroupId=' + '' + '&price=' + this.cart.amount;

                            window.newUrl = url;
                        }
                    },
                    closeSpec: function () {
                        this.specState = false;
                    },
                    onShoppingCart: function () {
                        this.shoppingCartState = this.shoppingCartState ? false : true;
                    },
                    closeShoppingCart: function () {
                        this.shoppingCartState = false;
                    }
                }
            });
        }

        /*
        入口方法
        */
        function main(vmDivId) {

            var dss = loadSessionStorages(); // dss = data SessionStorages

            //appVm包含：场馆的概要+当前分店概要
            var appVm = new AppVm(
                dss.appId, dss.shopId,
                dss.userId, dss.shopIsWorking, dss.deliveryCondition, dss.deliveryFee, dss.freeDeliveryCondition);

            //购物车总览
            var cartVm = new CartVm(dss.appId, dss.deliveryCondition, dss.deliveryFee, dss.freeDeliveryCondition);

            var proVms = getProVms('/Mobile/GetCateringCommodity', dss.shopId, dss.userId);

            window.vm = createVm(appVm, cartVm, proVms);
            window.vm.$mount('#' + vmDivId);
        }
        $(".layer-content").show();
        return { main: main };
    });
