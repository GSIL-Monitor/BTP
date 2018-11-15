window.PRO = (function (mod) {

    function getSubprosCountAndAmount(pro) {
        if (pro.hasSubpro) {
            var count = 0;
            var amount = 0;
            for (var i in pro.subpros) {
                count += pro.subpros[i].count;
                amount += pro.subpros[i].count * pro.subpros[i].price;
            }
            return {
                count: count,
                amount: amount
            };
        } else {
            console.error(pro.hasSubpro);
            return null;
        }
    }

    //产品 +、-、点击属性时，出发，计算产品及其子产品是否可以支付，可以‘支付、+、-’。
    function refresh(pro) {
        //根据 库存、价格、数量 计算其它值

        //this.hasStockLimit = false;    //库存有限
        //this.isEnough = true;          //库存充足
        //this.isBought = false; //购买数量是否不为0

        //this.count = 0;
        //this.amount = 0;               //购买总价
        //this.boxAmount = 0;            //餐盒总价

        //this.canIncrease = true;       //还能增加
        //this.canDecrease = true;       //还能减少

        //没有子产品
        if (!pro.hasSubpro) {
            pro.hasStockLimit = pro.stock > 0 && pro.stock < 100000000;
            pro.isEnough = !pro.hasStockLimit || pro.stock >= pro.count;
            pro.isBought = pro.count > 0;
            pro.canIncrease = pro.stock > pro.count;
            pro.canDecrease = pro.count > 0;

            pro.amount = pro.count * pro.price;
            pro.boxAmount = pro.count * pro.boxPrice * pro.boxNum;

        } else { //有子产品

            //1.子产品
            for (var sub in pro.subpros) {
                var subpro = pro.subpros[sub];
                subpro.hasStockLimit = subpro.stock > 0 && subpro.stock < 100000000;
                subpro.isEnough = !subpro.hasStockLimit || subpro.stock >= subpro.count;
                subpro.isBought = subpro.count > 0;

                subpro.amount = subpro.price * subpro.count;

                subpro.canIncrease = subpro.stock > subpro.count;
                subpro.canDecrease = subpro.count > 0;

                //修改focusSubpro的状态
                if (pro.focusSubpro && pro.focusSubpro.kSubpro == subpro.kSubpro) {
                    var focusSub = pro.focusSubpro;
                    focusSub.hasStockLimit = subpro.hasStockLimit;
                    focusSub.isEnough = subpro.isEnough;
                    focusSub.isBought = subpro.isBought;
                    focusSub.count = subpro.count;
                    focusSub.amount = subpro.amount;
                    focusSub.canIncrease = subpro.canIncrease;
                    focusSub.canDecrease = subpro.canDecrease;
                }
            }

            //2.主产品
            var csps = getSubprosCountAndAmount(pro);
            pro.count = csps.count;
            pro.amount = csps.amount;
            pro.boxAmount = pro.count * pro.boxPrice * pro.boxNum;

            //有子产品时，主产品永远不能+、-
            pro.canIncrease = false;
            pro.canDecrease = false;
            pro.isEnough = pro.stock >= pro.count;
            pro.isBought = pro.count > 0;
        }
    }

    mod.updateCountByCartpro = function (pro, cartpro) {

        if (!pro.hasSubpro) {
            pro.count = cartpro.count;
        } else {
            var subpro = COMMONS.findFirstByPropVal(pro.subpros, "id", cartpro.subproId);
            if (!subpro) {
                console.error("购物车中保存的产品'" + pro.name + "'的子产品'" + cartpro.attrTxtsUnion + "'(" + cartpro.subproId + ")已不存在！");
                return;
            }
            subpro.count = cartpro.count;
        }
        refresh(pro);
    }

    mod.initPro = function (pro, categoryId, categoryName, attrs,
        subpros, intensity, discountPrice) {

        pro.categoryId = categoryId;
        pro.categoryName = categoryName;
        pro.hasSubpro = subpros && subpros.length > 0; //是否存在子产品（组合属性）
        pro.attrs = attrs; //数组 数组元素是AttrVm，AttrVm里有AttrValVms
        pro.subpros = subpros; //数组 数组元素是SubproductVm

        PRICE.alterPrice(pro, intensity, discountPrice);

        refresh(pro);
    }

    mod.printAboutSubpro = function (pro) {

        var content = "产品'" + pro.name + "'：";
        console.group(content);

        if (pro.hasSubpro) {

            content = "属性列表：";
            //console.log(content);

            for (var a in pro.attrs) {
                var attr = pro.attrs[a];
                //console.log('  ' +attr.name + '  ' + attr.kAttr);

                for (var v in attr.vals) {
                    var val = attr.vals[v];
                    //console.log('    ' +val.txt + '  ' + val.kAttrVal);
                }
            }

            content = "子产品列表：";
            //console.log(content);

            for (var i in pro.subpros) {
                var subpro = pro.subpros[i];

                //console.log('  ' +subpro.attrTxtsUnion + '  ' +subpro.kAttrsUnion);
            }
        }
        console.groupEnd();
    }
    //打印pro的主要内容（控制台打印，并返回html字符串）
    mod.printPro = function (pro) {
        var txt = "";

        var content = "产品'" + pro.name + "'：";
        txt += (content + "<br/>");
        //console.group(content);

        content = "kPro = " + pro.kPro + " , stock = " + pro.stock + " , hasDiscount = " + pro.hasDiscount + " , isEnough = " + pro.isEnough;
        txt += (content + "<br/>");
        //console.log(content);

        content = "count = " + pro.count + " , amount = " + pro.amount;
        txt += ("<font color='red'>" + content + "</font><br/>");
        //console.log(content);

        content = COMMONS.printObj({
            canIncrease: pro.canIncrease,
            canDecrease: pro.canDecrease
        }, true);
        txt += (content + "<br/>");
        //console.log(content);

        if (pro.hasSubpro) {
            var maincontent = (pro.focusSubpro ? pro.focusSubpro.kAttrsUnion + " , " + pro.focusSubpro.attrTxtsUnion : "");
            txt += ("<b>focusSubpro：" + maincontent + "</b><br/>");
            //console.log(maincontent);

            content = "子产品列表：";
            txt += ("<br/>" + content + "<br/>");
            //console.log(content);

            for (var i in pro.subpros) {
                var subpro = pro.subpros[i];

                content = subpro.attrTxtsUnion;
                txt += ("<b>" + content + "</b><br/>");
                //console.log(content);

                content = "stock = " + subpro.stock + " , isEnough = " + subpro.isEnough;
                txt += (content + "<br/>");
                //console.log(content);

                content = "count = " + subpro.count + " , amount = " + subpro.amount;
                txt += ("<font color='red'>" + content + "</font><br/>");
                //console.log(content);

                content = COMMONS.printObj({
                    canIncrease: subpro.canIncrease,
                    canDecrease: subpro.canDecrease
                }, true);
                if (content) {
                    txt += content;
                }
            }
        }
        //console.groupEnd();
        return txt;
    }

    //点击 +、-
    //如果产品没有子产品，则不需要第3个参数
    //如果产品有子产品，在”选规格“页面，不需要传第3个参数，因为购买的只能是pro.focusSubpro
    //在”购物车“里，必须传第3个参数。
    mod.buy = function (pro, isAdd, kSubpro) {
        if (!pro.hasSubpro) {
            if (isAdd) {
                if (!pro.canIncrease) {
                    console.error("产品'" + pro.name + "'canIncrease==false！");
                    return null;
                }
            } else {
                if (!pro.canDecrease) {
                    console.error("产品'" + pro.name + "'canDecrease==false！");
                    return null;
                }
            }

            pro.count = isAdd ? pro.count + 1 : pro.count - 1;
            pro.amount = pro.price * pro.count;

        } else {
            if (!kSubpro)
                kSubpro = pro.focusSubpro.kSubpro;

            if (!kSubpro) {
                console.error("产品'" + pro.name + "'没有focusSubpro，也没有传入kSubpro，请检查调用代码！");
                return null;
            }

            var theSub = COMMONS.findFirstByPropVal(pro.subpros, "kSubpro", kSubpro);

            if (isAdd) {
                if (!theSub.canIncrease) {
                    console.error("产品'" + pro.name + "'(子产品" + theSub.attrTxtsUnion + ")canIncrease==false！");
                    return null;
                }
            } else {
                if (!theSub.canDecrease) {
                    console.error("产品'" + pro.name + "'(子产品" + theSub.attrTxtsUnion + ")canDecrease==false！");
                    return null;
                }
            }

            theSub.count = isAdd ? theSub.count + 1 : theSub.count - 1;
            theSub.amount = theSub.price * theSub.count;
        }

        //主产品的数量和总价更新，交给fresh
        refresh(pro);
    }

    //点击组合属性的值
    mod.attrClick = function (pro, kAttrVal) {
        refresh(pro);

        var canCheck = true;
        var kAttr = "attr_" + kAttrVal.split('_')[1];
        var focusAttr = COMMONS.findFirstByPropVal(pro.attrs, "kAttr", kAttr);
        var focusVal = COMMONS.findFirstByPropVal(focusAttr.vals, "kAttrVal", kAttrVal);
        var brotherVals = new Array(); //获取当前属性的其它vals


        //当前选中，那么点击后，无条件变为"未选中"，然后推出
        if (focusVal.isChecked) {
            focusVal.isChecked = false;
            focusAttr.checkedValK = "";
            refresh(pro); //isChecked改变，所以更新pro的状态
            pro.focusSubpro = null;
            return null;
        }
        //选择需要改变其isCheck的兄弟val列表
        for (var i in focusAttr.vals) {
            var val = focusAttr.vals[i];
            if (val.kAttrVal != kAttrVal)
                brotherVals.push(val);
        }

        //如果当前选中一个子产品，此子产品数量是0，并且canIncrease=false时，不能选中
        //如果当前没有选中一个子产品，那就要考虑此val和另一个属性的所有val组合成的子产品列表，是否都是0且canIncrease=false
        var ralatedSubproKs = new Array();
        var len = pro.attrs.length;

        switch (len) {
            case 1:
                var kSubpro = kAttrVal;
                //var subPro = COMMONS.findFirstByPropVal(pro.subpros, "kSubpro", kSubpro);
                ralatedSubproKs.push(kSubpro);
                break;
            case 2:
                var attr0 = pro.attrs[0];
                var vals0 = attr0.vals;
                var attr1 = pro.attrs[1];
                var vals1 = attr1.vals;

                //当前点击的是attr0
                if (attr0.kAttr == kAttr) {
                    var vals1Checked = COMMONS.findFirstByPropVal(attr1.vals, "isChecked", true);

                    //另一个属性也有值被选中
                    if (vals1Checked) { //存在
                        ralatedSubproKs.push(kAttrVal + "," + vals1Checked.kAttrVal);
                    }
                        //另一个属性没有值被选中，判断全部
                    else {
                        for (var vi in vals1) {
                            var v = vals1[vi];
                            ralatedSubproKs.push(kAttrVal + "," + v.kAttrVal);
                        }
                    }
                }
                    //当前点击的是attr1
                else {
                    var vals0Checked = COMMONS.findFirstByPropVal(attr0.vals, "isChecked", true);

                    //另一个属性也有值被选中
                    if (vals0Checked) { //存在
                        ralatedSubproKs.push(vals0Checked.kAttrVal + "," + kAttrVal);
                    }
                        //另一个属性没有值被选中，判断全部
                    else {
                        for (var vi in vals0) {
                            var v = vals0[vi];
                            ralatedSubproKs.push(v.kAttrVal + "," + kAttrVal);
                        }
                    }
                }
                break;
            default:
                console.error("产品'" + pro.name + "'(" + pro.kPro + ")的组合属性数量错误！");
                break;
        }

        //如果当前选中一个子产品，此子产品数量是0，并且canIncrease=false时，不能选中
        //如果当前没有选中一个子产品，那就要考虑此val和另一个属性的所有val组合成的子产品列表，是否都是0且canIncrease=false
        canCheck = false;
        for (var k in ralatedSubproKs) {
            var rSubpro = COMMONS.findFirstByPropVal(pro.subpros, "kSubpro", ralatedSubproKs[k]);
            if ((rSubpro.count == 0 && !rSubpro.canIncrease)) {
                //console.log(rSubpro.kSubpro + "的count==" + rSubpro.count + "且canIncrease=" + rSubpro.canIncrease);
            } else {
                canCheck = true;
            }
        }

        if (canCheck) {

            //将当前属性的其他兄弟值，变为不选中
            for (var i in brotherVals) {
                var bVal = brotherVals[i];
                bVal.isChecked = false;
            }
            focusVal.isChecked = true;
            focusAttr.checkedValK = focusVal.kAttrVal;
        }

        var focusSubproK = "";
        for (var a in pro.attrs) {
            var attr = pro.attrs[a];
            if (!attr.checkedValK) {
                focusSubproK = "";
                break;
            }
            focusSubproK += "," + attr.checkedValK;
        }
        if (focusSubproK) {
            focusSubproK = focusSubproK.substring(1);
            var focusSubpro = COMMONS.findFirstByPropVal(pro.subpros, "kSubpro", focusSubproK);
            pro.focusSubpro = focusSubpro;
        } else {
            pro.focusSubpro = null;
        }

        refresh(pro);

        return {
            "brotherVals": brotherVals,
            "ralatedSubproKs": ralatedSubproKs
        };
    }

    mod.mapToProductVms = function (originDatas) {
        var productVms = new Array();

        var originProducts = originDatas.CommodityList;
        var originCategorys = originDatas.CategoryList;

        var proPrefix = "pro_";
        var proIndex = 0;

        for (var i in originProducts) {
            var originProduct = originDatas.CommodityList[i];
            var categoryId = originProduct.CategoryId;

            var originCategory = COMMONS.findFirstByPropVal(originCategorys, "Id", categoryId);
            if (!originCategory)
                originCategory = {
                    "Id": "00000000-0000-0000-0000-000000000000",
                    "Name": "其他"
                };
            var categoryName = originCategory.Name;
            var aboutSubpro = this.mapAboutSubproVm(originProduct.ComAttibutes, originProduct.CommodityStocks);

            var kPro = proPrefix + proIndex;
            proIndex++;
            var displayPrice = originProduct.MarketPrice;
            var price = originProduct.Price;

            //初始化
            var pro = new ProductVm(kPro, originProduct.Name, displayPrice, price,
                originProduct.Stock, originProduct.Id, originProduct.Pic, originProduct.MealBoxAmount, originProduct.MealBoxNum);
            this.initPro(pro, categoryId, categoryName, aboutSubpro.attrs,
                aboutSubpro.subpros, originProduct.Intensity, originProduct.DiscountPrice);

            productVms.push(pro);
        }
        return productVms;
    }

    mod.mapAboutSubproVm = function (originComAttibutes, originCommodityStocks) {

        var attrs = new Array();
        var subpros = new Array();

        var attrPrefix = "attr_";
        var attrIndex = 0;
        var attrValPrefix = "val_";
        var attrValIndex = 0;

        //originComAttibutes就是产品的 所有 组合属性值（属性名，属性值）
        for (var i in originComAttibutes) {

            var originAttr = originComAttibutes[i];

            var attr = COMMONS.findFirstByPropVal(attrs, "name", originAttr.Attribute);
            if (!attr) {
                var kComAttr = attrPrefix + attrIndex;
                attr = new AttrVm(kComAttr, originAttr.Attribute);
                attrs.push(attr);

                attrIndex++;
            }

            var kAttrVal = attrValPrefix + attr.kAttr.split('_')[1] + '_' + attrValIndex;

            attr.vals.push(new AttrValVm(kAttrVal, originAttr.SecondAttribute)); //isChecked初始值设为false
            attrValIndex++;
        }
        //console.log("属性列表:::", attrs);

        //originCommodityStocks就是产品的 子产品列表，每个子产品包括Id，价格，库存，组合属性值
        for (var j in originCommodityStocks) {

            var originSubpro = originCommodityStocks[j];

            var kAttrsUnion = "";
            var attrTxtsUnion = "";
            var isFirstAttr = true;

            //originSubpro.ComAttribute里是当前子产品的两个 组合属性值（属性名，属性值）
            for (var k in originSubpro.ComAttribute) {
                var originAttr = originSubpro.ComAttribute[k];

                var arr = COMMONS.findFirstByPropVal(attrs, "name", originAttr.Attribute);
                var val = COMMONS.findFirstByPropVal(arr.vals, "txt", originAttr.SecondAttribute);
                if (!val) {
                    console.error("数据错误！找不到txt为" + originAttr.SecondAttribute + "的组合属性值。attr.vals=", arr.vals);
                } else {
                    if (isFirstAttr) {
                        kAttrsUnion = val.kAttrVal;
                        attrTxtsUnion = val.txt;
                        isFirstAttr = false;
                    } else {
                        kAttrsUnion = kAttrsUnion + "," + val.kAttrVal;
                        attrTxtsUnion = attrTxtsUnion + "," + val.txt;
                    }
                }
            }
            //console.log("kAttrsUnion:::" + kAttrsUnion);
            var subpro = new SubproVm(
                kAttrsUnion, attrTxtsUnion,
                originSubpro.MarketPrice, originSubpro.Price, originSubpro.Stock, originSubpro.Id);
            subpros.push(subpro);
        }
        return {
            attrs: attrs,
            subpros: subpros
        };
    }

    mod.mapToCartproVms = function (pros) {

        var cartpros = new Array();

        for (var i in pros) {
            var pro = pros[i];
            if (pro.count > 0) {

                var kAttrsUnion = "";
                var attrTxtsUnion = "";

                //主要插入子产品的数据
                if (pro.hasSubpro) {
                    for (var s in pro.subpros) {
                        var subpro = pro.subpros[s];
                        kAttrsUnion = subpro.kAttrsUnion;
                        attrTxtsUnion = subpro.attrTxtsUnion;

                        if (subpro.count > 0) {
                            var subCartVm = new CartproVm(
                                pro.kPro, subpro.kSubpro, true, pro.id, subpro.id, kAttrsUnion, attrTxtsUnion,
                                pro.name, pro.pic, subpro.displayPrice, subpro.price,
                                subpro.count, subpro.amount,
                                pro.boxPrice, pro.boxNum, pro.boxAmount,
                                subpro.canIncrease, subpro.canDecrease, subpro.isEnough, subpro.isBought, false);
                            cartpros.push(subCartVm);
                        }
                    }
                }
                    //主要插入主产品的数据
                else {
                    var proCartVm = new CartproVm(
                        pro.kPro, "", false, pro.id, "", kAttrsUnion, attrTxtsUnion,
                        pro.name, pro.pic, pro.displayPrice, pro.price,
                        pro.count, pro.amount,
                        pro.boxPrice, pro.boxNum, pro.boxAmount,
                        pro.canIncrease, pro.canDecrease, pro.isEnough, pro.isBought, false);
                    cartpros.push(proCartVm);
                }
            }
        }
        return cartpros;
    };

    return mod;

})(window.PRO || {});
