window.APP = (function (mod) {

    mod.mapToCategoryVms = function (pros) {

        var categoryVms = new Array();

        for (var i in pros) {
            var pro = pros[i];

            var categoryVm = COMMONS.findFirstByPropVal(categoryVms, "categoryId", pro.categoryId);

            if (!categoryVm) {
                categoryVm = new CategoryVm(pro.categoryId, pro.categoryName);
                categoryVms.push(categoryVm);
            }
            categoryVm.pros.push(pro);
        }
        return categoryVms;
    };

    mod.updateCartVm = function (cart, pros) {

        var deliveryCondition = cart.deliveryCondition;
        var count = 0;
        var amount = 0;
        var boxAmount = 0;
        var proTypeCount = 0;
        var subproTypeCount = 0;

        for (var i in pros) {
            var pro = pros[i];

            //有购买
            if (pro.count > 0) {
                count += pro.count;
                amount += pro.amount;
                boxAmount += pro.boxAmount;
                proTypeCount++;
                if (pro.hasSubpro) {
                    for (var j in pro.subpros) {
                        var subpro = pro.subpros[j];
                        if (subpro.count > 0)
                            subproTypeCount++;
                    }
                } else {
                    subproTypeCount++;
                }
            }
        }

        cart.lessDeliveryCondition = deliveryCondition > amount ? deliveryCondition - amount : 0;

        cart.count = count;
        cart.amount = amount;
        cart.boxAmount = boxAmount;
        cart.proTypeCount = proTypeCount;
        cart.subproTypeCount = subproTypeCount;
        cart.realDeliveryFee = cart.freeDeliveryCondition <= amount ? 0 : cart.deliveryFee;

        console.log("amount=", cart.amount, "boxAmount=", boxAmount, "cart.deliveryFee=", cart.deliveryFee, cart.boxAmount, "免配送费金额=", cart.freeDeliveryCondition, "实际配送费=", cart.realDeliveryFee);
    };

    mod.updateProsByStorage = function (cart, pros, deletedPros, storagePros) {

        deletedPros.splice(0, deletedPros.length);  // 清空deletedPros
        var errMsg = "";

        for (var c in storagePros) {
            var storagePro = storagePros[c];

            var pro = COMMONS.findFirstByPropVal(pros, "id", storagePro.proId);
            if (!pro) {
                storagePro.isDeleted = true;
                deletedPros.push(storagePro);
                errMsg += storagePro.name + "不存在！";
                console.log(errMsg);
                continue;
            }
            if (pro.price !== storagePro.price)
                errMsg += storagePro.name + "价格已改变！";

            PRO.updateCountByCartpro(pro, storagePro);
        }

        //重新计算cart
        this.updateCartVm(cart, pros);
        return errMsg;
    };

    return mod;

})(window.APP || {});
