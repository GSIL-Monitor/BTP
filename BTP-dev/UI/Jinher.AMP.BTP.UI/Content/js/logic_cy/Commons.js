
window.COMMONS = (function (mod) {

    /*
    同步ajax(使用zepto)
    */
    mod.postAjax = function (url, data, isAsync) {
        if (isAsync !== false)
            isAsync = true; //默认采用异步调用
        var result;

        if (data && $.type(data) != "string") {
            data = JSON.stringify(data);
        }
        $.ajax({
            url: url,
            type: 'post',
            async: isAsync,
            contentType: "application/json",
            data: data,
            success: function(response) {
                result = response;
            },
            error: function(response) {
                console.error(response);
            },
            dataType: 'json'
        });

        return result;
    }

    /*
    对价格进行格式化，保留2位小数
    第2个参数表示：是否删除最末尾的0
    */
    mod.formatPrice = function (number, skipEndZero) {

        var newNumber = Number(number).toFixed(2);

        if (skipEndZero) {
            var len = newNumber.length;
            if (newNumber[len - 1] === '0') {
                newNumber = newNumber.substring(0, len - 1);

                len = newNumber.length;
                if (newNumber[len - 1] === '0') {
                    newNumber = newNumber.substring(0, len - 2);
                }
            }
        }
        return newNumber;
    }

    /*
    在数组中寻找 第一个 属性propName的值=propVal 的元素
    */
    mod.findFirstByPropVal = function (arr, propName, propVal) {

        for (var i in arr) {
            if (arr.hasOwnProperty(i)) {
                var obj = arr[i];
                //只判断对象的本地属性
                if (obj.hasOwnProperty(propName) && obj[propName] == propVal) {
                    return obj;
                }
            }
        }
        return null;
    };

    /*
    根据数组的某个值，创建一个索引表
    */
    mod.createIndexlist = function (arr, propName) {
        var indexList = {};
        for (var i in arr) {
            if (arr.hasOwnProperty(i)) {
                var obj = arr[i];
                if (obj.hasOwnProperty(propName)) {
                    indexList[obj[propName]] = obj;
                }
            }
        }
        return indexList;
    };

    /*
    根据属性值 过滤数组
    */
    mod.filterArrByEqual = function (arr, propName, propVal) {
        var result = new Array();
        for (var i in arr) {
            if (arr.hasOwnProperty(i)) {
                var obj = arr[i];
                if (propName in obj && obj[propName] == propVal)
                    result.push(obj);
            }
        }
        return result;
    };

    /*
    根据属性的最小值 过滤数组
    注意：不包含最小值
    */
    mod.filterArrByGreater = function (arr, propName, minPropVal) {
        var result = new Array();
        for (var i in arr) {
            if (arr.hasOwnProperty(i)) {
                var obj = arr[i];
                if (propName in obj && obj[propName] > minPropVal)
                    result.push(obj);
            }
        }
        return result;
    };

    /*
    打印对象，遇到prop值==specialVal时，加亮显示
    */
    mod.printObj = function (obj, specialVal, printNormal) {

        var txt = "";
        for (var k in obj) {
            var val = obj[k];
            var content = k + " = " + val;
            if (val == specialVal) {
                txt += ("<b>" + content + "</b><br/>");
                console.log('%c' + content, 'color: violet');
            }
            else if (printNormal) {
                txt += (content + "<br/>");
                console.log(content);
            }
        }
        return txt;
    }

    /*
    对arr执行过滤（select），目的是对arr里的对象执行投影，只保留需要的prop
    */
    mod.arrSelect = function (arr, propNames) {
        var result = new Array();

        for (var i in arr) {
            var obj = arr[i];
            var newObj = {}
            for (var n in propNames) {
                var propName = propNames[n];
                newObj[propName] = obj[propName];
            }
            result.push(newObj);
        }
        return result;
    }

    /*
    判断arr中是否存在符合要求（属性和值等于入参）的元素
    */
    mod.arrExistSomeitem = function (arr, propName, propVal) {

        for (var i in arr) {
            var obj = arr[i];
            var val = obj['"' + propName + '"'];
            if (val == propVal) 
                return true;
        }
        return false;
    }

    function findFirstProVm(productVms, kPro) {
        var pro = COMMONS.findFirstByPropVal(productVms, "kPro", kPro);
        if (!pro) {
            console.error("产品" + kPro + "不存在！");
            return null;
        }
        else {
            return pro;
        }
    };

    function findFirstSubproVm(productVm, kSubpro) {
        var pro = productVm;
        if (!pro.hasSubpro) {
            console.error("产品" + pro.kPro + "没有子产品！");
            return null;
        }
        else {
            var subpro = COMMONS.findFirstByPropVal(pro.subpros, "kSubpro", kSubpro);
            if (!subpro) {
                console.error("产品" + pro.kPro + "没有子产品" + kSubpro + "！");
                return null;
            } else {
                return { pro: pro, subpro: subpro };
            }
        }
    };

    function findFirstProInCartVm(productInCarts, kProInCart) {
        var proInCart = COMMONS.findFirstByPropVal(productInCarts, "kProInCart", kProInCart);
        if (!proInCart) {
            console.error("购物车产品" + kProInCart + "不存在！");
            return null;
        }
        else {
            return proInCart;
        }
    };

    return mod;
})(window.COMMONS || {});