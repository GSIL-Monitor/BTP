window.SYSTEM = (function(mod,$,undefined){
    /**
     * 页面适配
     */
    mod.initWindow = function(){
        var deviceWidth = document.documentElement.clientWidth > 500 ? 500 : document.documentElement.clientWidth;
        document.documentElement.style.fontSize = deviceWidth / 6.4 + 'px';//如果设计图是320的话就除以3.2
    }
    $(window).resize(function(){
        mod.initWindow();
    });
    return mod;
})(window.SYSTEM || {},window.Zepto);

window.INDEX = (function(mod,$,undefined){
    /**
     * 取有效的组合属性（库存为0的数据返回null）
     * */
    mod.getCommodityAttrStockDTO = function(attSize, attColor, commodity){
        if (!commodity) {
            commodity = JSON.parse(sessionStorage.commodityInfo);
        }
        var commodityStocks = commodity.CommodityStocks;

        if (commodityStocks.length > 0) {
            for (var i = 0; i < commodityStocks.length; i++) {
                var group = commodityStocks[i].ComAttribute;
                var sizeFlag = false;
                var colorFlag = false;
                for (var j = 0; j < group.length; j++) {

                    if (group[j].Attribute == sessionStorage.attSize && group[j].SecondAttribute == attSize) {
                        sizeFlag = true;
                    }
                    if (group[j].Attribute == sessionStorage.attColor && group[j].SecondAttribute == attColor) {
                        colorFlag = true;
                    }
                    if (sizeFlag == true && colorFlag == true && commodityStocks[i].Stock > 0) {
                        return commodityStocks[i];
                    }
                }
            }
        }
        return null;
    };

    /**
     * 设置商品焦点属性
     * */
    mod.setFocusAtt = function(focusSize, focusColor, focusAttr, commodity){
        var number = parseInt($('.span_number').val());
        setSessionStorage('commodityUpInfo', 'size', focusSize);
        setSessionStorage('commodityUpInfo', 'color', focusColor);
        var stock = sessionStorage.RealStock; //组合属性库存
        var price = commodity.Price;
        if (focusAttr) {
            price = Math.abs(focusAttr.Price * (commodity.Intensity / 10)).toFixed(2);
            sessionStorage.CommodityStockId = focusAttr.Id;
            sessionStorage.focusAttrPrice = focusAttr.Price;
            setSessionStorage('commodityUpInfo', 'price', focusAttr.Price);

            stock = focusAttr.Stock;
        } else {
            price = Math.abs(commodity.Price * (commodity.Intensity / 10)).toFixed(2);
            sessionStorage.CommodityStockId = null;
            stock = commodity.Stock;
            sessionStorage.focusAttrPrice = 0;
            setSessionStorage('commodityUpInfo', 'price', price);
        }

        if (commodity.DiscountPrice > 0 && commodity.DiscountPrice) {
            price = Math.abs(commodity.DiscountPrice).toFixed(2);
        }

        if (stock && number > stock) {
            $('#txtMaxStock').text("库存仅剩" + stock + "件");
            $('#txtMaxStock').show();
        }
        setSessionStorage('commodityUpInfo', 'realPrice', price);
        sessionStorage.Price = price;
        if (stock > 0) {
            $('#pre_money_count').text(getCurrency() + Math.abs(price * number).toFixed(2));
            $('#pre_money_count').parent().show();
            sessionStorage.Stock = stock;
        } else {
            $('#pre_money_count').parent().hide();
            sessionStorage.Stock = '已售完';
        }
    };

    /**
     * 显示商品数量
     * */
    mod.displayCommodityNumber = function(){
        //缓存的购物车中已选商品。
        var cdata = new Array();
        var odjson = localStorage["orderDishes" + sessionStorage.appId];
        if (!JsVilaDataNull(odjson))
            return;
        cdata = JSON.parse(odjson);
        if (cdata == null || cdata.length == 0 || cdata.GetFirstElement == undefined) {
            $(".span_1 .span_number").val(1);
            return;
        }
        var stockInfo = cdata.GetFirstElement("CommodityStockId", sessionStorage.CommodityStockId);
        if (stockInfo == null) {
            $(".span_1 .span_number").val(1);
            return;
        }
        $(".span_1 .span_number").val(stockInfo.CommodityNumber);
        var pmc = parseFloat(stockInfo.CommodityNumber) * parseFloat(stockInfo.RealPrice);
        pmc = pmc.toFixed(2);
        $("#pre_money_count").text(getCurrency() + pmc);
        $(".span_1 .span_number").change();
    };

    /**
     *
     * */
    mod.calcComAttibutes = function(item, commodity){
        var attSize = '';
        var attColor = '';
        var focusAttr = null;
        var isSetFirst = false;
        if (!commodity)
            commodity = JSON.parse(sessionStorage.commodityInfo);
        //只有两种属性才设置联动
        if (multAttr) {
            var $parentItem = $(item).parent().parent();
            var $parentItemOther = '';
            if ($parentItem.hasClass('size')) {
                attSize = $(item).text();
                $parentItemOther = $parentItem.next();
                attColor = $parentItemOther.find('ul').find('.li_focus_1').text();
                focusAttr = mod.getCommodityAttrStockDTO(attSize, attColor, commodity);
                if (!focusAttr) {
                    isSetFirst = false;
                    $parentItemOther.find('ul').find('li').removeClass("li_focus_1");
                } else {
                    isSetFirst = true;
                }

                $parentItemOther.find('ul').find('li').each(function () {
                    var tmpvalue = $(this).text();
                    var dto = mod.getCommodityAttrStockDTO(attSize, tmpvalue, commodity);
                    if (!dto) {
                        $(this).addClass('li_disable');
                    } else {
                        $(this).removeClass('li_disable');
                        if (!isSetFirst) {
                            focusAttr = mod.getCommodityAttrStockDTO(attSize, tmpvalue, commodity);
                            attColor = tmpvalue;
                            isSetFirst = true;
                            $(this).addClass("li_focus_1");
                        }
                    }
                });
            }
            if ($parentItem.hasClass('color')) {
                $parentItemOther = $parentItem;
                attSize = $parentItemOther.prev().find('ul').find('.li_focus_1').text();
                attColor = $parentItemOther.find('ul').find('.li_focus_1').text();
                focusAttr = mod.getCommodityAttrStockDTO(attSize, attColor, commodity);
                if (!focusAttr) {
                    isSetFirst = false;
                    $parentItemOther.find('ul').find('li').removeClass("li_focus_1");
                } else {
                    isSetFirst = true;
                }
                $parentItemOther.find('ul').find('li').each(function () {
                    var tmpvalue = $(this).text();
                    var dto = mod.getCommodityAttrStockDTO(attSize, tmpvalue, commodity);
                    if (!dto) {
                        $(this).addClass('li_disable');
                    } else {
                        $(this).removeClass('li_disable');
                        if (!isSetFirst) {
                            focusAttr = mod.getCommodityAttrStockDTO(attSize, tmpvalue, commodity);
                            attColor = tmpvalue;
                            isSetFirst = true;
                            $(this).addClass("li_focus_1");
                        }
                    }
                });
            }
        }
        mod.setFocusAtt(attSize, attColor, focusAttr, commodity);
    };

    /**
     * 设置商品属性，库存等
     * */
    mod.setAttStock = function(commodity){
        sessionStorage.attSize = ''; //商品第一个属性
        sessionStorage.attColor = ''; //商品第二个属性
        var addCommodity = $('#addCommodity'); //点击购买时弹出商品尺寸与颜色盒子
        var property = $('#property'); //获取商品属性盒子
        var LiBox = $('<li></li>'); //li对象
        var tmpObj = {}; //临时obj

        //存第一个有库存的数据
        var ghcFirstAttribte = {};
        ghcFirstAttribte.Size = '';
        ghcFirstAttribte.Color = '';
        ghcFirstAttribte.Index = 0;
        var ghcDisableList = [];

        if (!commodity) {
            commodity = JSON.parse(sessionStorage.commodityInfo);
        }

        var colorAndSize = (function () { //颜色与尺寸对象.
            var tmp = {};
            var ghcFirst = false;

            if (commodity.CommodityStocks && commodity.CommodityStocks.length > 0) { //商品包含多个属性，库存、价格需要联动
                for (var i = 0; i < commodity.CommodityStocks.length; i++) {
                    var commodityStock = commodity.CommodityStocks[i];
                    if (!ghcFirst) {
                        if (commodityStock.Stock > 0) {
                            ghcFirstAttribte.Index = i;
                            ghcFirst = true;
                        }
                    }
                    for (var j = 0; j < commodityStock.ComAttribute.length; j++) {
                        var comAttribute = commodityStock.ComAttribute[j];
                        if (!tmp[comAttribute.Attribute]) {
                            tmp[comAttribute.Attribute] = [];
                            if (sessionStorage.attSize == '') {
                                sessionStorage.attSize = comAttribute.Attribute;

                            } else {
                                sessionStorage.attColor = comAttribute.Attribute;
                            }
                        }
                        if (tmp[comAttribute.Attribute].indexOf(comAttribute.SecondAttribute) < 0)
                            tmp[comAttribute.Attribute].push(comAttribute.SecondAttribute);
                    }
                    //默认值
                    for (var j = 0; j < commodityStock.ComAttribute.length; j++) {
                        var comAttribute = commodityStock.ComAttribute[j];
                        if (ghcFirstAttribte.Index == i && j == 0) {
                            ghcFirstAttribte.Size = comAttribute.SecondAttribute;
                        }
                        else if (ghcFirstAttribte.Index == i && j == 1) {
                            ghcFirstAttribte.Color = comAttribute.SecondAttribute;
                        }
                        if (commodityStock.Stock > 0) {
                            if (ghcDisableList.length == 0) {
                                if (j == 0) {
                                    ghcDisableList.push(comAttribute.SecondAttribute);
                                }
                            }
                            else {
                                var isExiste = false;
                                for (var m = 0; m < ghcDisableList.length; m++) {
                                    if (j == 0) {
                                        if (ghcDisableList[m] == comAttribute.SecondAttribute) {
                                            isExiste = true;
                                        }
                                    }
                                }
                                if (!isExiste) {
                                    if (j == 0) {
                                        ghcDisableList.push(comAttribute.SecondAttribute);
                                    }
                                }
                            }
                        }
                    }

                    multAttr = true;
                }
            } else if (commodity.ComAttibutes) {
                for (var i = 0; i < commodity.ComAttibutes.length; i++) {
                    if (!ghcFirst) {
                        if (commodity.Stock > 0) {
                            ghcFirstAttribte.Index = i;
                            ghcFirst = true;
                        }
                    }
                    if (commodity.ComAttibutes[i].SecondAttribute) {
                        if (!tmp[commodity.ComAttibutes[i].Attribute]) {
                            tmp[commodity.ComAttibutes[i].Attribute] = [];
                            if (sessionStorage.attSize == '') {
                                sessionStorage.attSize = commodity.ComAttibutes[i].Attribute;
                            } else {
                                sessionStorage.attColor = commodity.ComAttibutes[i].Attribute;
                            }
                        }
                        tmp[commodity.ComAttibutes[i].Attribute].push(commodity.ComAttibutes[i].SecondAttribute);
                    }
                    if (commodity.ComAttibutes[i].SecondAttribute) {
                        if (ghcFirstAttribte.Index == i) {
                            ghcFirstAttribte.Size = commodity.ComAttibutes[i].SecondAttribute;

                        }
                    }
                    if (commodity.Stock > 0) {
                        if (ghcDisableList.length == 0) {
                            ghcDisableList.push(commodity.ComAttibutes[i].SecondAttribute);
                        }
                        else {
                            var isExiste = false;
                            for (var m = 0; m < ghcDisableList.length; m++) {

                                if (ghcDisableList[m] == commodity.ComAttibutes[i].SecondAttribute) {
                                    isExiste = true;
                                }

                            }
                            if (!isExiste) {
                                ghcDisableList.push(commodity.ComAttibutes[i].SecondAttribute);
                            }
                        }
                    }

                }
                multAttr = false;
            }


            if (sessionStorage.attSize == '') {
                sessionStorage.attSize = ghcFirstAttribte.Size;
                sessionStorage.attColor = ghcFirstAttribte.Color;
            }
            return tmp;
        })();

        //设置尺寸盒子对象
        var sizeBox = addCommodity.find('.size').find('ul').empty().on("click", 'li', function () {
            var self = $(this);
            if (self.hasClass('li_disable'))
                return;
            self.parent().find('li').removeClass('li_focus_1');
            self.addClass('li_focus_1');
            //设置初始值
            setSessionStorage('commodityUpInfo', 'size', self.text());
            mod.calcComAttibutes(this);
            mod.displayCommodityNumber();
        });

        //设置颜色盒子对象
        var colorBox = addCommodity.find('.color').find('ul').empty().on("click", 'li', function () {
            var self = $(this);
            if (self.hasClass('li_disable'))
                return;
            self.parent().find('li').removeClass('li_focus_1');
            self.addClass('li_focus_1');
            //设置初始值
            setSessionStorage('commodityUpInfo', 'color', self.text());
            mod.calcComAttibutes(this);
            mod.displayCommodityNumber();
        });

        var focusSize = ''; //选中的商品属性
        var focusColor = ''; //选中的商品属性
        var focusAttr = null; //选中的CommodityStock对象

        //设置尺寸
        if (sessionStorage.attSize != '' && colorAndSize[sessionStorage.attSize] && colorAndSize[sessionStorage.attSize].length) {
            addCommodity.find('.size').show();
            property.find('.type_2').parent('li').show();
            addCommodity.find('.size').find('span').text(sessionStorage.attSize + "：");
            property.find('.type_2').text(colorAndSize[sessionStorage.attSize]);
            //设置购买时尺寸框
            var isSet = false;
            for (var i = 0; i < colorAndSize[sessionStorage.attSize].length; i++) {
                tmpObj = LiBox.clone().text(colorAndSize[sessionStorage.attSize][i]);
                if (!isSet) {
                    if (ghcFirstAttribte.Size != "") {
                        if (tmpObj.text() == ghcFirstAttribte.Size) {
                            focusSize = ghcFirstAttribte.Size;
                            tmpObj.addClass('li_focus_1');
                            isSet = true;
                        }
                    }
                }
                if (multAttr) {
                    var isAttrIn = false;
                    for (var j = 0; j < ghcDisableList.length; j++) {
                        if (tmpObj.text() == ghcDisableList[j]) {
                            isAttrIn = true;
                        }
                    }
                    if (!isAttrIn) {
                        tmpObj.addClass('li_disable');
                    }
                }
                sizeBox.append(tmpObj);
            }

        } else {
            property.find('.type_2').parent('li').hide();
            addCommodity.find('.size').hide();
        }

        //设置颜色
        if (sessionStorage.attColor != '' && colorAndSize[sessionStorage.attColor] && colorAndSize[sessionStorage.attColor].length) {
            addCommodity.find('.color').show();
            property.find('.type_3').parent('li').hide();
            addCommodity.find('.color').find('span').text(sessionStorage.attColor + "：");
            property.find('.type_3').text(colorAndSize[sessionStorage.attColor]);
            var hasFocusItem = false;
            var calComAtts = false;
            //设置购买时颜色框
            for (var i = 0; i < colorAndSize[sessionStorage.attColor].length; i++) {
                tmpObj = LiBox.clone().text(colorAndSize[sessionStorage.attColor][i]);
                if (multAttr) {
                    var attrStockDTO = mod.getCommodityAttrStockDTO(focusSize, colorAndSize[sessionStorage.attColor][i], commodity);
                    if (!attrStockDTO) {
                        tmpObj.addClass('li_disable');
                    } else if (!hasFocusItem) {
                        if (ghcFirstAttribte.Color != "") {
                            if (tmpObj.text() == ghcFirstAttribte.Color) {
                                tmpObj.addClass('li_focus_1');
                                focusColor = ghcFirstAttribte.Color;
                                focusAttr = attrStockDTO;
                                hasFocusItem = true;
                                calComAtts = true;
                            } else {
                                tmpObj.addClass('li_disable');
                            }
                        }
                    }
                } else {
                    if (!hasFocusItem) {
                        if (tmpObj == ghcFirstAttribte.Color) {
                            tmpObj.addClass('li_focus_1');
                        }
                        hasFocusItem = true;
                    }
                }
                colorBox.append(tmpObj);
                if (calComAtts)
                    mod.calcComAttibutes(tmpObj, commodity)
                calComAtts = false;
            }
        } else {
            property.find('.type_3').parent('li').hide();
            addCommodity.find('.color').hide();
        }

        mod.setFocusAtt(focusSize, focusColor, focusAttr, commodity);
        mod.displayCommodityNumber();
    };
    /**
     * 消息提醒
     * */
    mod.notify = function(){
        if($('.swiper-slide').length>1){
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

    };

    /**
     * 清除旧数据结构
     * */
    mod.clearOldCData = function(){
        //最初商品属于多个分类，只显示在第一个分类下，后改为每个分类下分别显示，所有旧数据结构就需要清掉
        var odjson = localStorage["orderDishes" + sessionStorage.appId];
        if (JsVilaDataNull(odjson)) {
            var cdata = JSON.parse(odjson);
            if (cdata != null && cdata.length && cdata[0].ItemId == undefined) {
                localStorage.removeItem("orderDishes" + sessionStorage.appId);
            }
        }
    };
    /**
     * 获取当前操作的商品id
     * */
    mod.getCommodityId = function(){
        if (!JsVilaDataNull(_itemId))
            return null;
        var arr = _itemId.split(',');
        if (arr.length == 2)
            return arr[0];
        return null;
    };
    mod.getCategoryId = function(){
        if (!JsVilaDataNull(_itemId))
            return null;
        var arr = _itemId.split(',');
        if (arr.length == 2)
            return arr[1];
        return null;
    };
    mod.updatePosition = function(){
        var current = Math.abs(this.y)
        for (var i = 0, len = map.length; i < len; i++) {
            if (map[i] < current && state) {
                $('#wrapper li').removeClass('current')
                $('#wrapper li').eq(i).addClass('current')
                var target = $('#wrapper li').eq(i)[0];
                myScroll.scrollToElement(target);
            }
        }
    };
    mod.formatComList = function (commodityList) {
        if (commodityList != null && commodityList.length > 0) {
            _commodityListData = commodityList;
            for (var i = 0; i < _commodityListData.length; i++) {
                _commodityListData[i].ItemId = _commodityListData[i].Id + "," + _commodityListData[i].CategoryId;
                _commodityListData[i].AttrCnt = mod.getAttrCnt(_commodityListData[i].ComAttibutes);
            }
        }
    };
    mod.getAttrCnt = function(comAttibutes){
        var arr = [];
        var cnt = 0;
        if (comAttibutes && comAttibutes.length > 0) {
            for (var i = 0; i < comAttibutes.length; i++) {
                var obj = comAttibutes[i];
                if (arr.indexOf(obj.Attribute) < 0) {
                    arr.push(obj.Attribute);
                    cnt++;
                }
            }
        }
        return cnt;
    };
    /**
     * 第一次页面加载时执行（加载分类和所有的商品）
     * */
    mod.LoadCommodity = function(){
        var userId = getUserId();
        userId = JsVilaDataNull(userId) ? userId : guidEmpty;
        getDataAjax2({
            url: '/Mobile/GetCateringCommodity',
            type: 'post',
            data: { appId: sessionStorage.appId, userId: userId },
            beforeSend: function () {
                //ajaxLoading(1, '');
            },
            complete: function () {
                //ajaxLoading(1, '');
            },
            callback: function (data) {
                _categoryListData = data.CategoryList;
                mod.formatComList(data.CommodityList);
                _appName = data.AppName;
                _vipPromotion = data.VipPromotion;

                //如果有“未分类”商品，左侧分类中添加“未分类”。
                if (_commodityListData != null && _commodityListData.length > 0) {
                    var wflc = _commodityListData.PropertyValueEqual("CategoryId", guidEmpty);
                    if (wflc.length > 0) {
                        _categoryListData.push({ Id: guidEmpty, Name: "其他" });
                    }
                }
                if (_categoryListData == null || _categoryListData.length == 0) {
                    return;
                }

                $("#sorts div.scroll").html("");
                if (_commodityListData == null || _commodityListData.length == 0) {
                    return;
                }

                //按分类来显示商品。
                for (var i = 0; i < _categoryListData.length; i++) {
                    var categoryId = _categoryListData[i].Id;
                    var commodityInCategory = _commodityListData.PropertyValueEqual("CategoryId", categoryId);
                    _categoryListData[i].cLength = commodityInCategory == null ? 0 : commodityInCategory.length;
                    mod.showCommodity(commodityInCategory, _categoryListData[i]);
                }
                mod.showCategory(_categoryListData);


                mod.initTotalCommodityCount();

                setTimeout(function () {
                    $.each($('.systitle'), function (key, val) {
                        var top = $(val).position().top;
                        map.push(top);
                    });
                    //$("body").css("min-height", "10px");

                    //滚动条
                    myScroll = new IScroll('#wrapper');

                    _sorts = new IScroll('#sorts', { probeType: 3, mouseWheel: true, click: true });
                    _sorts.on('scrollEnd', mod.updatePosition);
                    _sorts.on('scroll', mod.updatePosition);
                }, 50);

            },
            error: function (date, text) {
                toast("加载数据失败");
            }
        });
    };
    /**
     * 显示分类
     * */
    mod.showCategory = function(dataCate){
        $("#wrapper ul.goods-name").html("");
        if (dataCate == null || dataCate.length == 0) {
            return;
        }
        var htmlStr = "";
        var len = dataCate.length;
        for (var i = 0; i < len; i++) {
            if (dataCate[i].cLength <= 0) {
                continue;
            }
            htmlStr += '<li class="name-text" data-id="{Id}" data-type="sorts{Id}">{Name}</li>'.format(dataCate[i]);
        }
        $("#wrapper ul.goods-name").append(htmlStr);
        //默认选中第一个分类。
        $("#wrapper ul.goods-name li").first().addClass("current");

        //选项卡
        $('.name-text').tap(function () {
            state = false;
            $(this).addClass('current').siblings().removeClass('current');
            var id = $(this).data("id");
            var target = document.getElementById($(this).attr('data-type'));
            if (target) {
                _sorts.scrollToElement(target);
            }
        });
    };
    /**
     * 处理显示价格
     * */
    mod.dealDisplayPrice = function(cclone){
        //price_2Display 标记第二给价格是不是显示。
        if (cclone.DiscountPrice > 0) {
            cclone.price_1 = Math.abs(cclone.DiscountPrice).toFixed(2);
            cclone.price_2 = cclone.Price;
            cclone.price_2Display = "inline-block";
        }
        else {
            if (cclone.Intensity == 10) {
                cclone.price_1 = cclone.Price;
                if (cclone.MarketPrice && cclone.MarketPrice != null && cclone.MarketPrice != 'null') {
                    cclone.price_2 = cclone.MarketPrice;
                } else {
                    cclone.price_2Display = "none";
                }
            } else {
                cclone.price_1 = Math.abs(cclone.Price * (cclone.Intensity / 10)).toFixed(2);
                cclone.price_2 = cclone.Price;
            }
        }
    };
    mod.getRealPrice = function(price, discountPrice, intensity){
        if (discountPrice > 0) {
            return discountPrice;
        }
        if (0 < intensity && 10 > intensity) {
            return Math.abs(price * (intensity / 10.0)).toFixed(2);
        }
        return price;
    };
    /**
     * 显示一类商品。
     * */
    mod.showCommodity = function(commodityList, cateInfo){
        if (commodityList == null || commodityList.length == 0 || cateInfo == null) {
            return;
        }
        var categorySplit = '<div id="sorts{Id}" class="systitle">{Name}</div>';
        categorySplit = categorySplit.format(cateInfo);
        $("#sorts  div.scroll").append(categorySplit);
        //缓存的购物车中已选商品。
        var cdata = new Array();
        var odjson = localStorage["orderDishes" + sessionStorage.appId];
        if (JsVilaDataNull(odjson)) {
            cdata = JSON.parse(odjson);
            if (cdata == null) {
                cdata = new Array();
            }
        }
        var chtmlStr = "";
        var len = commodityList.length;
        for (var i = 0; i < len; i++) {
            var cclone = new Object();
            cclone = $.extend(cclone, commodityList[i]);
            cclone.srckey = "src";
            mod.dealDisplayPrice(cclone);
            //将商品实际付款价格保存起来。
            commodityList[i].price_1 = cclone.price_1;
            cclone.numberDisplay = cclone.IsMultAttribute ? "none" : "block";
            cclone.specificationDisplay = cclone.IsMultAttribute ? "block" : "none";

            //多属性商品不将缓存的“商品件数”显示到页面上。
            var lc = cdata.GetFirstElement("ItemId", cclone.ItemId);
            if (lc != null && !JsVilaDataNull(lc.CommodityStockId)) {
                cclone.CommodityNumber = lc.CommodityNumber;
                cclone.minusDisplay = lc.CommodityNumber > 0 ? "" : "hide";
            }
            else {
                cclone.CommodityNumber = 0;
                cclone.minusDisplay = "hide";
            }

            chtmlStr += _commodityTemplateHtml.format(cclone);
        }
        $("#sorts div.scroll").append(chtmlStr);
    };
    /**
     * 加入购物车
     * */
    mod.addtoshopcart = function(){
        var commodityInfo = _commodityListData.GetFirstElement("ItemId", _itemId);
        var sessionData = JSON.parse(sessionStorage.commodityUpInfo);
        var postData =
        {
            CommodityId: commodityInfo.Id,         //商品ID
            CommodityNumber: parseInt(sessionData.number), //商品数量
            SizeAndColorId: (sessionData.color ? sessionData.color : '') + ',' + (sessionData.size ? sessionData.size : ''), //尺寸颜色
            CommodityStockId: sessionStorage.CommodityStockId,
            ItemId: _itemId,
            CategoryId: null,
            CategoryName: null
        };

        if (commodityInfo.CategoryId != guidEmpty) {
            var category = _categoryListData.GetFirstElement("Id", commodityInfo.CategoryId);
            if (category != null) {
                postData.CategoryId = commodityInfo.CategoryId;
                postData.CategoryName = category.Name;
            }
        }
        if (JsVilaDataNull(sessionStorage.CommodityStockId)) {
            //从stock中取。
            postData.RealPrice = mod.getRealPrice(sessionStorage.focusAttrPrice, commodityInfo.DiscountPrice, commodityInfo.Intensity);
        } else {
            postData.RealPrice = commodityInfo.price_1;
        }
        //添加到购物车
        mod.setOrderDisheslocalStorage(postData);
        //隐藏属性选择框。
        $('#addCommodity').hide();
    };
    /**
     * */
    mod.setOrderDisheslocalStorage = function(cateItem){
        var cdata = new Array();
        var odjson = localStorage["orderDishes" + sessionStorage.appId];
        if (JsVilaDataNull(odjson)) {
            cdata = JSON.parse(odjson);
            if (cdata == null) {
                cdata = new Array();
            }
        }

        //以stockId为唯一标识。
        if (JsVilaDataNull(cateItem.CommodityStockId)) {
            if (cdata.ElementExist("CommodityStockId", cateItem.CommodityStockId)) {
                cdata.ElementReplace("CommodityStockId", cateItem.CommodityStockId, cateItem);
            }
            else {
                cdata.push(cateItem);
            }
        }
        else if (JsVilaDataNull(cateItem.ItemId)) {
            if (cdata.ElementExist("ItemId", cateItem.ItemId)) {
                cdata.ElementReplace("ItemId", cateItem.ItemId, cateItem);
            }
            else {
                cdata.push(cateItem);
            }
        }
        //清除数量为0的商品。
        var cresult = new Array();
        var sumCN = 0;
        var sumPrice = 0;
        for (var i = 0; i < cdata.length; i++) {
            if (cdata[i].CommodityNumber == 0) {
                continue;
            }
            sumCN += cdata[i].CommodityNumber;
            sumPrice += cdata[i].CommodityNumber * cdata[i].RealPrice;
            cresult.push(cdata[i]);
        }
        $("#num").text(sumCN);
        $("#spanTotalPrice").text(sumPrice.toFixed(2));
        localStorage["orderDishes" + sessionStorage.appId] = JSON.stringify(cresult);
    };
    /**
     * 初始显示购物车商品总数量
     * */
    mod.initTotalCommodityCount = function(){
        var cdata = new Array();
        var odjson = localStorage["orderDishes" + sessionStorage.appId];
        if (JsVilaDataNull(odjson)) {
            cdata = JSON.parse(odjson);
            if (cdata == null) {
                cdata = new Array();
            }
        }

        var cresult = new Array();
        var sumCN = 0;
        var sumPrice = 0;
        for (var i = 0; i < cdata.length; i++) {
            //清除已下架商品。
            var cd = _commodityListData.GetFirstElement("Id", cdata[i].CommodityId);
            if (cd == null) {
                continue;
            }

            if (cd.AttrCnt == 2) {
                //更新多属性商品价格。
                if (cd.CommodityStocks == null || cd.CommodityStocks.length == 0) {
                    continue;
                }
                var cStock = cd.CommodityStocks.GetFirstElement("Id", cdata[i].CommodityStockId);
                if (cStock == null) {
                    continue;
                }
                cdata[i].RealPrice = mod.getRealPrice(cStock.Price, cd.DiscountPrice, cd.Intensity);
            }
            else {
                //更新单属性商品价格
                cdata[i].RealPrice = cd.price_1;
            }
            sumCN += cdata[i].CommodityNumber;
            sumPrice += cdata[i].CommodityNumber * cdata[i].RealPrice;
            cresult.push(cdata[i]);
        }
        localStorage["orderDishes" + sessionStorage.appId] = JSON.stringify(cresult);
        $("#num").text(sumCN);
        $("#spanTotalPrice").text(sumPrice.toFixed(2));
    };
    mod.AddToCart = function(number){
        //清空数据。
        setSessionStorage('commodityUpInfo', 'number', number);
        //这3个属性由allevent.js来改变。
        sessionStorage.CommodityStockId = null;
        setSessionStorage('commodityUpInfo', 'color', "");
        setSessionStorage('commodityUpInfo', 'size', "");




        var commodityInfo = _commodityListData.GetFirstElement("ItemId", _itemId);
        if (commodityInfo == null) {
            return;
        }
        sessionStorage['commodityInfo'] = JSON.stringify(commodityInfo);
        var isMultAttribute = commodityInfo.IsMultAttribute;
        if (isMultAttribute) {
            //$(".footdi").hide();
            mod.setAttStock();
            //显示底部属性选择框。
            $("#addCommodity").height($(document).height()).show();
        }
        else {
            mod.addtoshopcart();
        }
    };
    /**
     * 结算
     * */
    mod.SettleAccounts = function(){
        var msg = "";
        var boprice = false;
        var bonum = false;
        var bostate = false;
        var xcdata = "";
        var preselled = false;

        var cdata = new Array();
        var odjson = localStorage["orderDishes" + sessionStorage.appId];
        if (JsVilaDataNull(odjson)) {
            cdata = JSON.parse(odjson);
            if (cdata == null) {
                cdata = new Array();
            }
        }

        var clen = cdata.length;
        if (clen == 0) {
            toast("请选择要结算的商品！");
            return;
        }
        var checkdata = { UserID: getUserId(), CommodityIdAndStockIds: cdata, diyGroupId: guidEmpty, promotionType: -1 };
        checkdata = CommLib.ObjToStringWithQuot(checkdata);
        getDataAjax2({
            url: '/Mobile/CheckCommodity',
            data: checkdata,
            callback: function (data) {
                //var msg = "";
                var commodSeled = new Array();
                for (var i = 0; i < cdata.length; i++) {
                    var cinfoOri = _commodityListData.GetFirstElement("Id", cdata[i].CommodityId);
                    var cinfo = new Object();
                    cinfo = $.extend(cinfo, cinfoOri);
                    cinfo = $.extend(cinfo, cdata[i]);


                    var crci = data.PropertyValueEqual("Id", cinfo.Id);
                    if (crci != null) {
                        if (crci.length == 1) {
                            crci = crci[0];
                        } else {
                            crci = crci.PropertyValueEqual("CommodityStockId", cinfo.CommodityStockId == guidEmpty ? null : cinfo.CommodityStockId);
                            if (crci != null && crci.length > 0)
                                crci = crci[0];
                        }
                    }
                    cinfo.UserId = getUserId();
                    cinfo.AppId = sessionStorage.appId;
                    cinfo.AppName = _appName;

                    var quantity = cinfo.CommodityNumber;

                    if (cinfo.RealPrice != crci.Price) {
                        $("#sorts div.list[data-id='" + cdata[i].CommodityId + "'] span.list-price").html(cinfo.Price);
                        cdata[i].RealPrice = crci.Price;
                        boprice = true;
                    }
                    if (crci.State == 1 || crci.State == 3) {
                        msg += cinfo.Name + " 已下架 \n";
                        bostate = true;
                    } else if (crci.IsNeedPresell && (!crci.IsPreselled)) {
                        msg += cinfo.Name + " 需要预约才可以购买 \n";
                        preselled = true;
                    } else if (crci.LimitBuyEach > 0 && quantity > crci.LimitBuyEach) {
                        if (cdata.length == 1) {
                            msg += "该商品每人限购" + crci.LimitBuyEach + "件，您已超限 \n";
                        } else {
                            msg += cinfo.Name + " 每人限购" + crci.LimitBuyEach + "件，您已超限 \n";
                        }
                        bonum = true;
                    } else if (quantity > crci.Stock || (crci.LimitBuyTotal > 0 && quantity > (crci.LimitBuyTotal - crci.SurplusLimitBuyTotal))) {
                        if (cdata.length == 1) {
                            msg += "该商品的购买数量超过了可购上限 \n";
                        } else {
                            msg += cinfo.Name + " 的购买数量超过了可购上限 \n";
                        }
                        bonum = true;
                    }

                    //                        var size = cinfo.SizeAndColorId;
                    //                        var yan = "";
                    //                        var chi = "";
                    //                        if (size != "undefined" && size != "" && size != null) {
                    //                            if (size.split(',')[0] != "" && size.split(',')[0] != null && size.split(',')[0] != "null") {
                    //                                yan = size.split(',')[0] + "";
                    //                            }
                    //                            if (size.split(',')[1] != "" && size.split(',')[1] != null && size.split(',')[1] != "null") {
                    //                                chi = size.split(',')[1] + "";
                    //                            }
                    //                        }
                    //                        cinfo.SizeAndColorId = chi + "," + yan;
                    commodSeled.push(cinfo);
                }


                if (boprice == true) {
                    msg = "购物车价格发生变化，页面即将刷新";
                    toast(msg);
                    window.location.href = window.location.href;
                }
                if (bonum == true || bostate == true || preselled == true) {
                    if (msg != "") {
                        toast(msg);
                    }
                    return false;
                }

                //start 当价格和库存都没发生改变时 进入下订单页面
                if (boprice == false && bonum == false) {
                    commodSeled = mod.buildShopCartData(commodSeled);
                    sessionStorage.ShopCartDate = JSON.stringify(commodSeled);
                    setTimeout(function () {
                        gotoCreateOrder("gouwuche", '', parseFloat($("#spanTotalPrice").html()).toFixed(2));
                    }, 1000);
                }
                //end  当价格和库存都没发生改变时 进入下订单页面
            }
        });
    };
    /**
     * 按app组织数据
     * */
    mod.buildShopCartData = function(commodSeled){
        if (commodSeled == null || commodSeled.length == 0) {
            return;
        }
        //找出选中的店铺id.
        var appIdSeled = new Array();
        for (var i = 0; i < commodSeled.length; i++) {
            commodSeled[i].Size = commodSeled[i].SizeAndColorId;
            var appid = commodSeled[i].AppId;
            if (appIdSeled.indexOf(appid) > -1) {
                continue;
            }
            appIdSeled.push(appid);
        }

        var shops = new Array();
        for (var b = 0; b < appIdSeled.length; b++) {
            var appid = appIdSeled[b];
            var cishop = commodSeled.PropertyValueEqual("AppId", appid);
            var shopOld = cishop[0];
            var shop = new Object();
            shop.AppId = shopOld.AppId;
            shop.AppName = shopOld.AppName;
            //shop.RealPrice = shopOld.RealPrice;
            shop.ShoppingCartItemSDTO = cishop;

            //计算app下总金额、总数量。
            var appAmount = 0;
            var commodityNum = 0;
            for (var d = 0; d < cishop.length; d++) {
                appAmount += cishop[d].RealPrice * cishop[d].CommodityNumber;
                commodityNum += cishop[d].CommodityNumber;
            }
            appAmount = eval(appAmount).toFixed(2);
            shop.AppAmount = appAmount;
            shop.CommodityNum = commodityNum;

            shops.push(shop);
        }
        return shops;
    };
    mod.initEvent = function(){
        //显示一个商品的模板项html
        var _commodityTemplateHtml = "";
        //当前操作的商品id.
        var _commodityId = null;
        //当前操作的商品项目id，组成结构；商品Id,商品分类Id
        var _itemId = null;
        //所有商品数据
        var _commodityListData = null;
        //所有分类数据
        var _categoryListData = null;
        //店铺名称
        var _appName = "";
        //会员优惠信息
        var _vipPromotion = null;

        var service_type = "0x0001";

        var map = [];
        var state = true;

        var _sorts = null;
        var myScroll = null;

        var guidEmpty = "00000000-0000-0000-0000-000000000000";

        $('document').ready(function () {
            //////////saveContextDTOByUrl();
            _commodityTemplateHtml = $("#divCommodityTemplate").html();
            //最小购买数量
            _minBuyNum = 0;

            /*if (DealLoginPartial != undefined) {
                //设置登录成功之后回调函数
                DealLoginPartial.setCallback(function () {
                    //去结算
                    mod.SettleAccounts();
                });
            }*/
            $('#sorts').on('touchstart', function () { state = true; });

            document.addEventListener('touchmove', function (e) { e.preventDefault(); }, false);

            //注册事件。
            $('#sorts div.scroll').on('click', '.spinner .minus', function () {
                $('#num').addClass('hide');
                _itemId = $(this).parents("div.list").data("itemid");
                var value = $(this).next().val() - 1;
                if (value <= 0) {
                    $(this).next().addClass('hide');
                    $(this).addClass('hide');
                }
                mod.AddToCart(value);
            });
            $('#sorts div.scroll').on('click', '.spinner .plus', function () {
                $('#num').removeClass('hide');
                _itemId = $(this).parents("div.list").data("itemid");
                var value = $(this).prev().val() - 0 + 1;
                if (value > 0) {
                    $(this).prev().prev().removeClass('hide');
                    $(this).prev().removeClass('hide');
                }
                mod.AddToCart(value);
            });

            $('#sorts div.scroll').on('click', '.spec', function () {
                $('.span_number').val(1);
                _itemId = $(this).parents("div.list").data("itemid");
                mod.AddToCart(1);
            });

            $("#aBalance").on("click", function () {
                DealLoginPartial.initPartialPage();
            });


            $("#btnshopcart").bind("click", function () {
                $(".span_number").change();
                if (sessionStorage.Stock == '已售完') {
                    toast("选择的商品数大于库存数");
                    return;
                }
                var num = parseInt($('#number').find('.span_number').val());
                if (num > sessionStorage.Stock) {
                    toast("选择的商品数大于库存数");
                    return;
                }
                logBTP(sessionStorage.SrcType, service_type, "0x0004", mod.getCommodityId());
                sessionStorage.btntype = "gouwuche";

                mod.addtoshopcart();
            });


            $('#clearAddCommodity').click(function () {
                $('#addCommodity').hide();
            });
            //清除旧数据结构
            mod.clearOldCData();

            //加载分类并加载第一个分类下的商品。
            mod.LoadCommodity();

            //设置加减数量按钮事件
            numberEvent();
        });
    };
    return mod;
})(window.DETAIL||{},window.Zepto);

$(document).ready(function(){
    window.SYSTEM.initWindow();
    INDEX.initEvent();
//    INDEX.notify();
});
//读取cookies 
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}