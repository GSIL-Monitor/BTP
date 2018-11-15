window.PRICE = (function (mod) {

    //修饰price字符串，首先保留两位小数，其次忽略结尾的0
    function alterPriceTxt(price) {
        if (typeof(price) === 'number')//包括整数和浮点数
            price += "";

        //price已经是字符串，这时候Boolean('0'),Boolean('0.0')都是true
        //Boolean(price)=false说明price没有值
        if (!price)
            return null;

        price = Number(price).toFixed(2);
        if (price.charAt(price.length - 1) === '0') {
            price = price.substring(0, price.length - 1);
            if (price.charAt(price.length - 1) === '0') {
                price = price.substring(0, price.length - 2);
            }
        }
        return price;
    }

    function runDiscount(price,intensity, discountPrice) {
        if (discountPrice > 0) {
            price = discountPrice;
        } else {
            price = price * intensity / 10;
        }
        return price;
    }

    mod.alterPrice = function(pro, intensity, discountPrice) {

        pro.hasDiscount = (0 < intensity && 10 > intensity) || discountPrice > 0;

        if (pro.hasDiscount) {
            pro.displayPrice = pro.price;
            pro.price = runDiscount(pro.price, intensity, discountPrice);

            for (var i in pro.subpros) {
                var subproVm = pro.subpros[i];
                subproVm.displayPrice = alterPriceTxt(subproVm.price);
                subproVm.price = runDiscount(subproVm.price, intensity, discountPrice);
            }
        }

        //格式化
        pro.displayPrice = alterPriceTxt(pro.displayPrice);
        pro.price = alterPriceTxt(pro.price);
        for (var i in pro.subpros) {
            var subpro = pro.subpros[i];
            subpro.displayPrice = alterPriceTxt(subpro.displayPrice);
            subpro.price = alterPriceTxt(subpro.price);
        }
    }
    return mod;
})(window.PRICE || {});