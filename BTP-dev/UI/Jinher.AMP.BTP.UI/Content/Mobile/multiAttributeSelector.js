//多属性商品选择器
var multiAttributeSelector = (function () {
    var regNum = new RegExp("^[1-9]\\d*$");
    var maSelector = new Object();
    //_rootElement 选择器在dom中的根元素；
    var _rootElement = new Object();
    //_commodity 商品信息；
    var _commodity = new Object();
    //_promotion 活动信息。
    var _promotion = new Object();
    //属性选择结果。
    var _seledResult = new Object();
    _seledResult.size = "";
    _seledResult.color = "";
    //操作类型（1 加入购物车 2普通活动直接购买 3拼团活动）；
    var _operType = 0;

    //设置尺寸盒子对象
    var sizeBox = null;
    //设置颜色盒子对象
    var colorBox = null;
    //最小购买数量
    var _minBuyNum = 1;

    //单价
    var _singlePrice = 0;
    //关税
    var _singleDuty = 0;
    // 每人限购数量
    var limitBuyEach = -1;
    //未选属性时价格
    var _defaultPrice = 0;
    //未选属性时关税价格
    var _defaultDuty = 0;
    //属性样式:0 无属性；1 1种属性；2 2种属性
    //双属性库存数据列表
    var _dataList = [];
    //要显示的属性列表
    var _dataShowList = {};
    //空库存列表
    var _dataDisableList = [];
    var _disableDefaultSizeList = [];
    var _disableDefaultColorList = [];

    //是否存在指定函数 
    function isFunctionExits(funcName) {
        try {
            if (typeof (eval(funcName)) == "function") {
                return true;
            }
        } catch (e) { }
        return false;
    }

    //加载商品属性。
    function GetCommodityAttrStocks(commodityId) {
        getDataAjax2({
            url: '/Mobile/GetCommodityAttribute',
            type: 'post',
            data: { commodityId: commodityId, userId: getUserId() },
            beforeSend: function () {
                //ajaxLoading(1, '');
            },
            complete: function () {
                //ajaxLoading(1, '');
            },
            callback: function (data) {
                if (data.ResultCode != 0) {
                    return;
                }
                var promotion = null;
                if (_operType == 3) {
                    _defaultPrice = data.Data.DiyGroupPromotion;
                    promotion = data.Data.DiyGroupPromotion;
                }
                else if (data.Data.PromotionTypeNew == 9999 && data.Data.VipPromotion && data.Data.VipPromotion.IsVipActive && data.Data.VipPromotion.IsVip) {
                    _defaultPrice = data.Data.Price;
                    //标记使用了会员优惠。
                    _seledResult.IsVipPromotion = true;
                    promotion = ConvertVip2Promotion(data.Data.VipPromotion);
                }
                else {
                    if (data.Data.DiscountPrice > 0 && data.Data.DiscountPrice) {
                        _defaultPrice = Math.abs(data.Data.DiscountPrice).toFixed(2);
                    } else {
                        _defaultPrice = Math.abs(data.Data.Price * (data.Data.Intensity / 10)).toFixed(2);
                    }
                    promotion = data.Data;
                }
                maSelector.show(data.Data, promotion, _operType);
            },
            error: function (date, text) {
                toast("操作失败");
            }
        });
    }

    //将vip优惠信息转换成普通活动优惠。
    function ConvertVip2Promotion(vipPromotion) {
        var p = new Object();
        p.Intensity = vipPromotion.Intensity;
        p.DiscountPrice = -1;
        p.LimitBuyEach = -1;
        return p;
    }

    function initSeledResult(sr) {
        var seledResult = new Object();
        seledResult.number = 1;
        seledResult.IsEnableSelfTake = _commodity.IsEnableSelfTake;

        if (sr != undefined) {
            _seledResult = $.extends(seledResult, sr);
        }
        else {
            _seledResult = seledResult;
        }
    }

    function loadAttibute() {
        _dataList = [];
        _dataShowList = {};
        _dataDisableList = [];
        _disableDefaultSizeList = [];
        _disableDefaultColorList = [];
        _seledResult.size = "";
        _seledResult.color = "";

        initDataList();
        setDefaultContent();
    }
    var isSingleAttrAndMultiValue = false;
    function initDataList() {
        var tmp = [];
        // 商品SKU存库新增 支持单属性多属性值的库存检查
        if (_commodity.ComAttibutes && _commodity.ComAttibutes.length > 1 && _commodity.CommodityStocks.length > 1) {
            _dataShowList.dataType = 2;
            var nameList = [];
            for (var i = 0; i < _commodity.CommodityStocks.length; i++) {
                var commodityStock = _commodity.CommodityStocks[i];
                for (var j = 0; j < commodityStock.ComAttribute.length; j++) {
                    var comAttribute = commodityStock.ComAttribute[j];
                    if (!tmp[comAttribute.Attribute]) {
                        nameList.push(comAttribute.Attribute);
                        tmp[comAttribute.Attribute] = [];
                    }
                    if (tmp[comAttribute.Attribute].indexOf(comAttribute.SecondAttribute) < 0) {
                        tmp[comAttribute.Attribute].push(comAttribute.SecondAttribute);
                    }
                }
                var model = {};
                model.FirstAttrName = commodityStock.ComAttribute[0].Attribute;
                model.FirstAttrValue = commodityStock.ComAttribute[0].SecondAttribute;
                if (commodityStock.ComAttribute.length > 1) {
                    model.SecondAttrName = commodityStock.ComAttribute[1].Attribute;
                    model.SecondAttrValue = commodityStock.ComAttribute[1].SecondAttribute;
                }
                else {
                    isSingleAttrAndMultiValue = true;
                }
                model.Stock = commodityStock.Stock;
                model.Price = commodityStock.Price;
                model.Id = commodityStock.Id;
                model.Duty = commodityStock.Duty;
                _dataList.push(model);

                if (model.Stock == 0) {
                    _dataDisableList.push(model);
                }
            }
            _dataShowList.FirstAttrName = nameList[0];
            _dataShowList.FirstAttrList = tmp[nameList[0]];
            for (var i = 0; i < _dataShowList.FirstAttrList.length; i++) {
                var firstTmp = _dataShowList.FirstAttrList[i];
                var isAllDisable = true;
                for (var j = 0; j < _dataList.length; j++) {
                    if (_dataList[j].FirstAttrValue == firstTmp && _dataList[j].Stock > 0) {
                        isAllDisable = false;
                        break;
                    }
                }
                if (isAllDisable) {
                    _disableDefaultSizeList.push(firstTmp);
                }
            }
            if (isSingleAttrAndMultiValue) {
                _dataShowList.dataType = 1;
            } else {
                _dataShowList.SecondAttrName = nameList[1];
                _dataShowList.SecondAttrList = tmp[nameList[1]];
                for (var i = 0; i < _dataShowList.SecondAttrList.length; i++) {
                    var secondTmp = _dataShowList.SecondAttrList[i];
                    var isAllDisable2 = true;
                    for (var j = 0; j < _dataList.length; j++) {
                        if (_dataList[j].SecondAttrValue == secondTmp && _dataList[j].Stock > 0) {
                            isAllDisable2 = false;
                            break;
                        }
                    }
                    if (isAllDisable2) {
                        _disableDefaultColorList.push(secondTmp);
                    }
                }
            }
        }
        // else if (_commodity.ComAttibutes && _commodity.ComAttibutes.length > 2) { //商品包含多个属性，库存、价格需要联动
        //     _dataShowList.dataType = 2;
        //     var nameList = [];
        //     for (var i = 0; i < _commodity.CommodityStocks.length; i++) {
        //         var commodityStock = _commodity.CommodityStocks[i];
        //         for (var j = 0; j < commodityStock.ComAttribute.length; j++) {
        //             var comAttribute = commodityStock.ComAttribute[j];
        //             if (!tmp[comAttribute.Attribute]) {
        //                 nameList.push(comAttribute.Attribute);
        //                 tmp[comAttribute.Attribute] = [];
        //             }
        //             if (tmp[comAttribute.Attribute].indexOf(comAttribute.SecondAttribute) < 0) {
        //                 tmp[comAttribute.Attribute].push(comAttribute.SecondAttribute);
        //             }
        //         }
        //         var model = {};
        //         model.FirstAttrName = commodityStock.ComAttribute[0].Attribute;
        //         model.FirstAttrValue = commodityStock.ComAttribute[0].SecondAttribute;
        //         model.SecondAttrName = commodityStock.ComAttribute[1].Attribute;
        //         model.SecondAttrValue = commodityStock.ComAttribute[1].SecondAttribute;
        //         model.Stock = commodityStock.Stock;
        //         model.Price = commodityStock.Price;
        //         model.Id = commodityStock.Id;
        //         model.Duty = commodityStock.Duty;
        //         _dataList.push(model);

        //         if (model.Stock == 0) {
        //             _dataDisableList.push(model);
        //         }
        //     }
        //     _dataShowList.FirstAttrName = nameList[0];
        //     _dataShowList.FirstAttrList = tmp[nameList[0]];
        //     _dataShowList.SecondAttrName = nameList[1];
        //     _dataShowList.SecondAttrList = tmp[nameList[1]];

        //     for (var i = 0; i < _dataShowList.FirstAttrList.length; i++) {
        //         var firstTmp = _dataShowList.FirstAttrList[i];
        //         var isAllDisable = true;
        //         for (var j = 0; j < _dataList.length; j++) {
        //             if (_dataList[j].FirstAttrValue == firstTmp && _dataList[j].Stock > 0) {
        //                 isAllDisable = false;
        //                 break;
        //             }
        //         }
        //         if (isAllDisable) {
        //             _disableDefaultSizeList.push(firstTmp);
        //         }
        //     }

        //     for (var i = 0; i < _dataShowList.SecondAttrList.length; i++) {
        //         var secondTmp = _dataShowList.SecondAttrList[i];
        //         var isAllDisable2 = true;
        //         for (var j = 0; j < _dataList.length; j++) {
        //             if (_dataList[j].SecondAttrValue == secondTmp && _dataList[j].Stock > 0) {
        //                 isAllDisable2 = false;
        //                 break;
        //             }
        //         }
        //         if (isAllDisable2) {
        //             _disableDefaultColorList.push(secondTmp);
        //         }
        //     }
        // }
        else if (_commodity.ComAttibutes && _commodity.ComAttibutes.length > 0) {
            _dataShowList.dataType = 1;
            var namestr = _commodity.ComAttibutes[0].Attribute;
            for (var i = 0; i < _commodity.ComAttibutes.length; i++) {
                if (_commodity.ComAttibutes[i].SecondAttribute) {
                    if (!tmp[_commodity.ComAttibutes[i].Attribute]) {
                        tmp[_commodity.ComAttibutes[i].Attribute] = [];
                        if (_seledResult.attSize == '') {
                            _seledResult.attSize = _commodity.ComAttibutes[i].Attribute;
                        } else {
                            _seledResult.attColor = _commodity.ComAttibutes[i].Attribute;
                        }
                    }
                    tmp[_commodity.ComAttibutes[i].Attribute].push(_commodity.ComAttibutes[i].SecondAttribute);
                }
            }

            isSingleAttrAndMultiValue = true;
            _dataShowList.FirstAttrName = namestr;
            _dataShowList.FirstAttrList = tmp[namestr];

            var model = {};
            var commodityStock = _commodity.CommodityStocks[0];
            model.FirstAttrName = commodityStock.ComAttribute[0].Attribute;
            model.FirstAttrValue = commodityStock.ComAttribute[0].SecondAttribute;
            if (commodityStock.ComAttribute.length > 1) {
                model.SecondAttrName = commodityStock.ComAttribute[1].Attribute;
                model.SecondAttrValue = commodityStock.ComAttribute[1].SecondAttribute;
            }
            else {
                isSingleAttrAndMultiValue = true;
            }
            model.Stock = commodityStock.Stock;
            model.Price = commodityStock.Price;
            model.Id = commodityStock.Id;
            model.Duty = commodityStock.Duty;
            _dataList.push(model);

            if (model.Stock === 0) {
                _dataDisableList.push(model);
            }
        }
        else {
            _dataShowList.dataType = 0;
        }
    }

    function setDefaultContent() {
        colorBox.empty();
        sizeBox.empty();
        var tmpHtml1 = "";
        var tmpHtml2 = "";

        _rootElement.find('.size').hide();
        _rootElement.find('.color').hide();

        //设置尺寸
        if (_dataShowList.dataType == 0) {
            //无属性
        }
        else if (_dataShowList.dataType == 1) {
            //显示内容
            _rootElement.find('.size').find('span').text(_dataShowList.FirstAttrName);
            for (var i = 0; i < _dataShowList.FirstAttrList.length; i++) {
                tmpHtml1 += "<li>" + _dataShowList.FirstAttrList[i] + "</li>";
            }
            sizeBox.append(tmpHtml1);
            _rootElement.find('.size').show();
        }
        else if (_dataShowList.dataType == 2) {
            //显示内容
            _rootElement.find('.size').find('span').text(_dataShowList.FirstAttrName);
            for (var i = 0; i < _dataShowList.FirstAttrList.length; i++) {
                tmpHtml1 += "<li>" + _dataShowList.FirstAttrList[i] + "</li>";
            }
            _rootElement.find('.color').find('span').text(_dataShowList.SecondAttrName);
            for (var i = 0; i < _dataShowList.SecondAttrList.length; i++) {
                tmpHtml2 += "<li>" + _dataShowList.SecondAttrList[i] + "</li>";
            }
            sizeBox.append(tmpHtml1);
            colorBox.append(tmpHtml2);
        }
        else {
            //其它
        }
        setDefaultHtml();
    }

    //清空时的显示
    function setDefaultHtml() {
        $("#SelectCommodityPrice").html(getCurrency() + _defaultPrice);
        setDuty(_defaultDuty, false);
        if (isSingleAttrAndMultiValue) {
            if (_commodity.Stock <= 0) {
                sizeBox.find("li").each(function () {
                    $(this).addClass('li_disable');
                });
            } else {
                if (_disableDefaultSizeList.length > 0) {
                    sizeBox.find("li").each(function () {
                        if (_disableDefaultSizeList.indexOf($(this).text()) > -1) {
                            $(this).addClass('li_disable');
                        } else {
                            $(this).removeClass('li_disable');
                        }
                    });
                } else {
                    sizeBox.find("li").each(function () {
                        $(this).removeClass('li_disable');
                    });
                }
            }
            _rootElement.find('.size').show();
            _rootElement.find('.color').hide();
        }
        else if (_dataShowList.dataType == 1 || _dataShowList.dataType == 2) {
            if (_dataShowList.dataType == 2) {
                if (_seledResult.size == "") {
                    if (_disableDefaultColorList.length > 0) {
                        colorBox.find("li").each(function () {
                            if (_disableDefaultColorList.indexOf($(this).text()) > -1) {
                                $(this).addClass('li_disable');
                            } else {
                                $(this).removeClass('li_disable');
                            }
                        });
                    }
                }
                if (_seledResult.color == "") {
                    if (_disableDefaultSizeList.length > 0) {
                        sizeBox.find("li").each(function () {
                            if (_disableDefaultSizeList.indexOf($(this).text()) > -1) {
                                $(this).addClass('li_disable');
                            } else {
                                $(this).removeClass('li_disable');
                            }
                        });
                    }
                }

                if (_seledResult.size == "" || _seledResult.color == "") {
                    _defaultDuty = 0;
                }

                _rootElement.find('.size').show();
                _rootElement.find('.color').show();
            }
            else if (_dataShowList.dataType == 1) {
                if (_commodity.Stock <= 0) {
                    sizeBox.find("li").each(function () {
                        $(this).addClass('li_disable');
                    });
                } else {
                    sizeBox.find("li").each(function () {
                        $(this).removeClass('li_disable');
                    });
                }
                _rootElement.find('.size').show();
                _rootElement.find('.color').hide();
            }
            else {
                //
            }
            var number = parseInt($('#number').find('.span_number').val());
            var text = parseInt(number);
            var dprice = (Math.abs(_defaultPrice * number) + Math.abs(_defaultDuty * number)).toFixed(2);
            _rootElement.find('span[masid="pre_money_count"]').text(getCurrency() + dprice);
            _rootElement.find('span[masid="pre_money_count"]').parent().show();
            //$('div[masid="btnOk"]').addClass("disable");
        }
        else if (_dataShowList.dataType == 0) {
            setSelectedPrice();
        }
    }

    function setSelectedHtml(item) {
        $("#SelectCommodityPrice").html(getCurrency() + _defaultPrice);
        setDuty(_defaultDuty, true);
        if (_dataShowList.dataType == 2) {
            var firstTmp = "";
            var secondTmp = "";
            var isSetDiable = false;
            var $parentItem = _rootElement.find(item).parent().parent();
            if (_seledResult.size != "") {
                firstTmp = $(item).text();
                if ($parentItem.hasClass('size')) {
                    var $parentItemColor = $parentItem.next().find('ul');
                    $parentItemColor.find("li").each(function () {
                        secondTmp = $(this).text();
                        isSetDiable = false;
                        for (var i = 0; i < _dataDisableList.length; i++) {
                            if (_dataDisableList[i].FirstAttrValue == firstTmp && _dataDisableList[i].SecondAttrValue == secondTmp) {
                                if (!$(this).hasClass("li_disable")) {
                                    $(this).addClass('li_disable');
                                }
                                isSetDiable = true;
                                break;
                            }
                        }
                        if (!isSetDiable) {
                            $(this).removeClass('li_disable');
                        }
                    });
                }
            }
            if (_seledResult.color != "") {
                secondTmp = $(item).text();
                if ($parentItem.hasClass('color')) {
                    var $parentItemSize = $parentItem.prev().find('ul');
                    $parentItemSize.find("li").each(function () {
                        firstTmp = $(this).text();
                        isSetDiable = false;
                        for (var i = 0; i < _dataDisableList.length; i++) {
                            if (_dataDisableList[i].SecondAttrValue == secondTmp && _dataDisableList[i].FirstAttrValue == firstTmp) {
                                if (!$(this).hasClass("li_disable")) {
                                    $(this).addClass('li_disable');
                                }
                                isSetDiable = true;
                                break;
                            }
                        }
                        if (!isSetDiable) {
                            $(this).removeClass('li_disable');
                        }
                    });
                }
            }

            if (_seledResult.size == "" || _seledResult.color == "") {
                _defaultDuty = 0;
            }

            _rootElement.find('.size').show();
            _rootElement.find('.color').show();
        }
        var number = parseInt($('#number').find('.span_number').val());
        var text = parseInt(number);
        var dprice = (Math.abs(_defaultPrice * number) + Math.abs(_defaultDuty * number)).toFixed(2);
        _rootElement.find('span[masid="pre_money_count"]').text(getCurrency() + dprice);
        _rootElement.find('span[masid="pre_money_count"]').parent().show();
    }

    //有效属性的结果
    function setSelectedPrice() {
        _rootElement.find('span[masid="txtMaxStock"]').hide();
        var number = parseInt(_rootElement.find('.span_number').val());

        var stock = _commodity.Stock;
        var price = _commodity.Price;
        var duty = _commodity.Duty;

        if (isSingleAttrAndMultiValue) {
            var selectedModel = {};
            var _seledResultValue = _seledResult.size ? _seledResult.size : _seledResult.color;
            for (var i = 0; i < _dataList.length; i++) {
                if (_dataList[i].FirstAttrValue == _seledResultValue) {
                    selectedModel = _dataList[i];
                    break;
                }
            }
            price = Math.abs(selectedModel.Price * (_promotion.Intensity / 10)).toFixed(2);
            duty = selectedModel.Duty.toFixed(2);
            _seledResult.CommodityStockId = selectedModel.Id;
            _seledResult.focusAttrPrice = selectedModel.Price;
            _seledResult.price = selectedModel.Price;
            _seledResult.duty = selectedModel.Duty;

            stock = selectedModel.Stock;
            if (stock > 0 && number > stock) {
                number = stock;
            }
            _seledResult.number = number;
            _rootElement.find('.span_number').val(number);
        }
        else if (_dataShowList.dataType == 2) {
            var selectedModel = {};
            for (var i = 0; i < _dataList.length; i++) {
                if (_dataList[i].FirstAttrValue == _seledResult.size && _dataList[i].SecondAttrValue == _seledResult.color) {
                    selectedModel = _dataList[i];
                    break;
                }
            }
            price = Math.abs(selectedModel.Price * (_promotion.Intensity / 10)).toFixed(2);
            duty = selectedModel.Duty.toFixed(2);
            _seledResult.CommodityStockId = selectedModel.Id;
            _seledResult.focusAttrPrice = selectedModel.Price;
            _seledResult.price = selectedModel.Price;
            _seledResult.duty = selectedModel.Duty;

            stock = selectedModel.Stock;
            if (stock > 0 && number > stock) {
                number = stock;
            }
            _seledResult.number = number;
            _rootElement.find('.span_number').val(number);
        } else {
            price = Math.abs(_commodity.Price * (_promotion.Intensity / 10)).toFixed(2);
            duty = _commodity.Duty.toFixed(2);

            _seledResult.CommodityStockId = null;
            _seledResult.focusAttrPrice = 0;
            _seledResult.price = _commodity.Price;
            _seledResult.duty = _commodity.Duty;

            if (stock > 0 && number > stock) {
                number = stock;
            }
            _seledResult.number = number;
            _rootElement.find('.span_number').val(number);
        }
        if (_promotion.DiscountPrice > 0 && _promotion.DiscountPrice) {
            price = Math.abs(_promotion.DiscountPrice).toFixed(2);
            //兼容历史数据
            if (_commodity.SkuActivityCdtos.length > 0) {
                for (var j = 0; j < _commodity.CommodityStocks.length; j++) {
                    if (_commodity.CommodityStocks[j].Id === _seledResult.CommodityStockId) {
                        price = _commodity.CommodityStocks[j].Price.toFixed(2);
                    }
                }
            } else {
                if (sessionStorage.btntype === "goumai") {
                    price = _commodity.Price.toFixed(2);
                    for (var j = 0; j < _commodity.CommodityStocks.length; j++) {
                        if (_commodity.CommodityStocks[j].Id === _seledResult.CommodityStockId) {
                            price = _commodity.CommodityStocks[j].Price.toFixed(2);
                        }
                    }
                } else {
                    price = Math.abs(_commodity.DiscountPrice).toFixed(2);
                }
            }
        } else {
            //参团按钮
            price = Math.abs(_promotion.DiscountPrice).toFixed(2);
            for (var j = 0; j < _commodity.CommodityStocks.length; j++) {
                if (_commodity.CommodityStocks[j].Id === _seledResult.CommodityStockId) {
                    price = _commodity.CommodityStocks[j].Price.toFixed(2);
                }
            }
            if (_commodity.SkuActivityCdtos) {
                for (var j = 0; j < _commodity.SkuActivityCdtos.length; j++) {
                    if (_commodity.SkuActivityCdtos[j].CommodityStockId === _seledResult.CommodityStockId) {
                        price = _commodity.SkuActivityCdtos[j].JoinPrice.toFixed(2);
                    }
                }
            }
        }
        //        if (stock && number > stock) {
        //            _rootElement.find('span[masid="txtMaxStock"]').text("库存仅剩" + stock + "件");
        //            _rootElement.find('span[masid="txtMaxStock"]').show();
        //        } else {
        //            _rootElement.find('span[masid="txtMaxStock"]').hide();
        //        }
        _seledResult.realPrice = price;
        if (stock > 0) {
            var dprice = 0;
            if (_operType == 3) {
                dprice = (Math.abs(price) + Math.abs(duty)).toFixed(2);
            }
            else {
                dprice = (Math.abs(price * number) + Math.abs(duty * number)).toFixed(2);
            }
            _rootElement.find('span[masid="pre_money_count"]').text(getCurrency() + dprice);
            _rootElement.find('span[masid="pre_money_count"]').parent().show();
            _seledResult.Stock = stock;
            _singlePrice = price;
            _singleDuty = duty;
            //$('div[masid="btnOk"]').removeClass("disable");

        } else {
            _rootElement.find('span[masid="pre_money_count"]').parent().hide();
            _seledResult.Stock = '已售完';
            //$('div[masid="btnOk"]').addClass("disable");
        }
        $("#SelectCommodityPrice").html(getCurrency() + _singlePrice);
        setDuty(_singleDuty, true);
    }

    function setNumberEvent() {
        //加减盒子对象
        var box = _rootElement.find('span[masid="numberBox"]');
        //数字对象
        var number = box.find('.span_number');
        //当前数字
        var text = parseInt(number.val());
        //获取传入参数
        var f_arguments = arguments;
        // 每人限购数量
        limitBuyEach = -1;

        //更新缓存数据
        _seledResult.number = text;

        box.on(_clientEvent, '.span_2', function () { //设置减事件,并更新缓存
            text = parseInt(number.val());
            if (text < 2) {
                text = 2;
            }
            text -= 1;
            number.val(text);
            _seledResult.number = text;

            var dprice = 0;
            //非有效属性时
            if (_dataShowList.dataType == 2 && (_seledResult.size == "" || _seledResult.color == "")) {
                dprice = Math.abs(_defaultPrice * text).toFixed(2);
            }
            else if (_dataShowList.dataType == 1 && _seledResult.size == "") {
                dprice = (Math.abs(_defaultPrice * text) + Math.abs(_defaultDuty * text)).toFixed(2);
            }
            else {
                if (_operType == 3) {
                    dprice = (Math.abs(_promotion.DiscountPrice) + Math.abs(_seledResult.duty)).toFixed(2);
                }
                else {
                    dprice = (Math.abs(_seledResult.realPrice * text) + Math.abs(_seledResult.duty * text)).toFixed(2);
                }
                if (text > _seledResult.Stock) {
                    number.val(_seledResult.Stock);
                    _seledResult.number = _seledResult.Stock;
                }
            }
            _rootElement.find('span[masid="pre_money_count"]').text(getCurrency() + dprice);
            f_arguments.length ? f_arguments[0](text) : '';
            if (isFunctionExits("calcCommodityFreight")) {
                calcCommodityFreight(_seledResult);
            }
        }).on(_clientEvent, '.span_3', function () { //设置加事件,并更新缓存
            text = parseInt(number.val()) + 1;
            number.val(text);
            _seledResult.number = text;

            var dprice = 0;
            //非有效属性时
            if (_dataShowList.dataType == 2 && (_seledResult.size == "" || _seledResult.color == "")) {
                dprice = Math.abs(_defaultPrice * text).toFixed(2);
            }
            else if (_dataShowList.dataType == 1 && _seledResult.size == "") {
                dprice = (Math.abs(_defaultPrice * text) + Math.abs(_seledResult.duty * text)).toFixed(2);
            }
            else {
                limitBuyEach = _promotion.LimitBuyEach;
                // -1 表示不限购
                if (limitBuyEach !== null && limitBuyEach !== -1 && text >= limitBuyEach) {
                    if (text > limitBuyEach) {
                        toast("每人限购" + limitBuyEach + "件，超出范围");
                        text = limitBuyEach;
                    }
                }
                if (text > _seledResult.Stock) {
                    toast("数量不足或超出限购数量.不能购买！");
                    text = _seledResult.Stock;
                }
                number.val(text);
                _seledResult.number = text;
                if (_operType == 3) {
                    dprice = (Math.abs(_promotion.DiscountPrice) + Math.abs(_seledResult.duty)).toFixed(2);
                }
                else {
                    dprice = (Math.abs(_seledResult.realPrice * text) + Math.abs(_seledResult.duty * text)).toFixed(2);
                }
            }
            _rootElement.find('span[masid="pre_money_count"]').text(getCurrency() + dprice);
            f_arguments.length ? f_arguments[0](text) : '';
            if (isFunctionExits("calcCommodityFreight")) {
                calcCommodityFreight(_seledResult);
            }
        });
        $(".span_number").on("focus", function (e) {
            if (_rootElement.find('.size').css("display") != "none") {
                $("#attrs").hide();
                eventCheck(e);
            }
        });
        $(".span_number").on("keyup", function () {
            var v = $(this).val();
            v = v.replace(/[^\d]/g, ""); //先把非数字的都替换掉，除了数字和.
            v = v.replace(/^0/g, ""); //不能以0开头。 
        }).on("change", function () {
            var v = $(this).val();
            v = v.replace(/[^\d]/g, ""); //先把非数字的都替换掉，除了数字和.
            v = v.replace(/^0/g, ""); //不能以0开头。 
        });
        $(".span_number").on("blur", function (e) {
            eventCheck(e);
            //            if (_rootElement.find('.size').css("display") == "none") {
            //                if (_dataShowList.dataType == 2) {
            //                    _rootElement.find('.size').show();
            //                    _rootElement.find('.color').show();
            //                } else if (_dataShowList.dataType == 1) {
            //                    _rootElement.find('.size').show();
            //                }
            //                if ($.os.ios) {
            //                    _myScroll = new IScroll('#attrs', { mouseWheel: true, click: true, tap: false });
            //                } else {
            //                    _myScroll = new IScroll('#attrs', { mouseWheel: true, click: true, tap: false });
            //                }
            //                _myScroll.refresh();
            //            }
            if ($.os.ios) {
                $(".addCommodity_2_ul_1").css("margin-bottom", "2.8px");
            }
            if ($("#attrs").css("display") == "none") {
                $("#attrs").show();
            }
            var v = $(this).val();
            if (!regNum.test(v)) {
                v = 1;
                $(this).val(1);
                $(this).focus();
            }
            if (v == "" || v == 0) {
                v = 1;
                $(this).val(v);
            }
            _seledResult.number = v;
            var prevNum = _seledResult.number;
            limitBuyEach = -1;
            limitBuyEach = _promotion.LimitBuyEach;

            var dprice = 0;
            //非有效属性时
            if (_dataShowList.dataType == 2 && (_seledResult.size == "" || _seledResult.color == "")) {
                dprice = (Math.abs(_defaultPrice * v) + Math.abs(_defaultDuty * v)).toFixed(2);
            }
            else if (_dataShowList.dataType == 1 && _seledResult.size == "") {
                dprice = (Math.abs(_defaultPrice * v) + Math.abs(_defaultDuty * v)).toFixed(2);
            }
            else {
                // -1 表示不限购
                if (limitBuyEach !== null && limitBuyEach !== -1 && v >= limitBuyEach) {
                    if (v > limitBuyEach) {
                        toast("每人限购" + limitBuyEach + "件，超出范围");
                        v = limitBuyEach;
                    }
                }
                if (v > _seledResult.Stock) {
                    toast("数量不足或超出限购数量.不能购买！");
                    v = _seledResult.Stock;
                }
                $(this).val(v);
                _seledResult.number = v;
                if (_operType == 3) {
                    dprice = _promotion.DiscountPrice.toFixed(2);
                }
                else {
                    dprice = (Math.abs(_seledResult.realPrice * v) + Math.abs(_seledResult.duty * v)).toFixed(2);
                }
                if (isFunctionExits("calcCommodityFreight")) {
                    calcCommodityFreight(_seledResult);
                }
            }
            _rootElement.find('span[masid="pre_money_count"]').text(getCurrency() + dprice);

        });
    }

    function setSelectedHtmlEvent() {
        sizeBox.on(_clientEvent, 'li', function () {
            var self = $(this);
            if (self.hasClass('li_disable'))
                return;
            if (self.hasClass('li_focus_1')) {
                self.removeClass('li_focus_1');
                _seledResult.size = "";
                setDefaultHtml();
            } else {
                self.parent().find('li').removeClass('li_focus_1');
                self.addClass('li_focus_1');
                _seledResult.size = self.text();
                if (_dataShowList.dataType == 2) {
                    setSelectedHtml(this);
                    if (_seledResult.color != "") {
                        setSelectedPrice();
                    }
                } else if (_dataShowList.dataType == 1) {
                    setSelectedPrice();
                }
            }
        });

        colorBox.on(_clientEvent, 'li', function () {
            var self = $(this);
            if (self.hasClass('li_disable'))
                return;

            if (self.hasClass('li_focus_1')) {
                self.removeClass('li_focus_1');
                _seledResult.color = "";
                setDefaultHtml();
            } else {
                self.parent().find('li').removeClass('li_focus_1');
                self.addClass('li_focus_1');
                //设置初始值
                _seledResult.color = self.text();
                if (_dataShowList.dataType == 2) {
                    setSelectedHtml(this);
                    if (_seledResult.size != "") {
                        setSelectedPrice();
                    }
                }
            }
        });

        //关闭按钮
        $('a[masid="clearAddCommodity"]').on(_clientEvent, function () {
            _rootElement.hide();
            maSelector.CloseCallback && maSelector.CloseCallback();
        });

        //确定
        $('div[masid="btnOk"]').on(_clientEvent, function () {
            if (_dataShowList.dataType == 2) {
                if (_seledResult.size == "" && _seledResult.color == "") {
                    toast("请选择" + _dataShowList.FirstAttrName + " " + _dataShowList.SecondAttrName);
                    return;
                }
                else if (_seledResult.size == "") {
                    toast("请选择" + _dataShowList.FirstAttrName);
                    return;
                }
                else if (_seledResult.color == "") {
                    toast("请选择" + _dataShowList.SecondAttrName);
                    return;
                }
            }
            else if (_dataShowList.dataType == 1 && _seledResult.size == "") {
                toast("请选择" + _dataShowList.FirstAttrName);
                return;
            } else {
                //
            }

            if ($('#number').find(".span_number").val() == "") {
                toast("购买数量不能为空");
                return;
            }
            if (_seledResult.Stock == '已售完') {
                toast("选择的商品数大于库存数");
                return;
            }
            var num = parseInt($('#number').find('.span_number').val());
            if (num > _seledResult.Stock) {
                toast("选择的商品数大于库存数");
                return;
            }
            if (_operType == 3) {
                _seledResult.realPrice = _promotion.DiscountPrice;
                if (_seledResult.realPrice <= 0) {
                    var price = $('#SelectCommodityPrice').text();
                    price = price.substring(1, price.length);
                    _seledResult.realPrice = parseInt(price);
                }
            }
            _seledResult.CommodityId = _commodity.Id;
            _seledResult.AppId = _commodity.AppId;
            maSelector.OkCallback && maSelector.OkCallback(_operType, _seledResult);
        });
    }

    //初始化商品属性选择器。rootElement 选择器在dom中的根元素；
    maSelector.initSelector = function (rootElement) {
        _rootElement = rootElement;
        colorBox = _rootElement.find('.color').find('ul').empty();
        sizeBox = _rootElement.find('.size').find('ul').empty();
        //initSeledResult();
        setSelectedHtmlEvent();
        setNumberEvent();
    };
    //获取已选中的商品属性。
    maSelector.getSelectedAttribute = function () {
        if (_operType == 3) {
            _seledResult.realPrice = _promotion.DiscountPrice;
        }
        return _seledResult;
    };
    //点击确定按钮的回调。
    maSelector.OkCallback = function () { };
    maSelector.CloseCallback = function () { };

    //显示 commodity 商品信息；promotion 活动信息（会员优惠信息）;operType 操作类型（1 加入购物车 2普通活动直接购买 3拼团活动）
    //活动、会员优惠信息 同时存在时，活动优先。
    //selectedResult 上次选择的结果，用于将结果重新显示到页面上。
    maSelector.show = function (commodity, promotion, operType, selectedResult) {
        _rootElement.hide();
        _commodity = commodity;
        _promotion = promotion;
        _operType = operType;
        //initSeledResult(selectedResult);
        _rootElement.find('.span_number').val(1);
        loadAttibute();

        if (operType == 1 || operType == 2) {
            _rootElement.find('li[masid="liNumber"]').show();
        }
        else if (operType == 3) {
            _rootElement.find('li[masid="liNumber"]').hide();
        } else {
            _rootElement.find('li[masid="liNumber"]').show();
        }
        $(".addCommodity_2").css("z-index", 200000);
        //加入图片
        $("#SelectCommodityPic").attr("src", _commodity.Pic);
        $("#SelectCommodityName").html(_commodity.Name);

        _rootElement.show();
        _myScroll.refresh();
    }
    //隐藏
    maSelector.hide = function () {
        _rootElement.hide();
    }
    maSelector.showByCommodityId = function (commodityId, operType, selectedResult) {
        _operType = operType;
        initSeledResult(selectedResult);
        GetCommodityAttrStocks(commodityId);
    }
    //设置默认价，当属性选不全时显示的价格
    //    maSelector.setDefault = function (defaultPrice, efaultDuty) {
    //        _defaultPrice = defaultPrice;
    //        _defaultDuty = efaultDuty;
    //    }
    maSelector.setDefault = function (defaultPrice, commodityInfo) {
        _defaultPrice = defaultPrice;
        if (commodityInfo.MinPrice > 0) {
            if (sessionStorage.btntype === "goumai") {
                _defaultPrice = commodityInfo.MinPrice.toFixed(2);
            } else {
                _defaultPrice = commodityInfo.MinSkuPrice.toFixed(2);
            }
        } else {
            //兼容老数据
            if (sessionStorage.btntype === "goumai") {
                _defaultPrice = commodityInfo.Price.toFixed(2);
            } else {
                _defaultPrice = commodityInfo.DiscountPrice.toFixed(2);
            }
        }

        //多属性商品显示关税信息
        var arrSimple = new Array();
        var le = commodityInfo.CommodityStocks.length;
        if (commodityInfo.CommodityStocks.length > 0) {
            for (var j = 0; j < le; j++) {
                arrSimple[j] = commodityInfo.CommodityStocks[j].Duty;
            }
            arrSimple.sort();
            _defaultDuty = arrSimple[0];
        }
    }
    return maSelector;
} ());

//是否显示关税
function setDuty(defaultDuty, isE) {
    if (defaultDuty > 0 && isE) {
        $("#SelectCommodityDuty").html("关税：" + getCurrency() + defaultDuty);
        $("#SelectCommodityDuty").css("margin-top", '16px');
        $("#SelectCommodityDuty").show();
    } else {
        $("#SelectCommodityDuty").hide();
    }
}

//读取cookies 
function getCookie(name) {
    var arr, reg = new RegExp("(^| )" + name + "=([^;]*)(;|$)");

    if (arr = document.cookie.match(reg))

        return unescape(arr[2]);
    else
        return null;
}
