/*
App(馆)总览
*/
function AppVm(
    id, shopId,
    userId, shopIsWorking, deliveryCondition, deliveryFee, freeDeliveryCondition) {

    this.id = id;

    this.shopId = shopId;
    this.userId = userId;

    //营业中 boolean类型
    this.shopIsWorking = (typeof shopIsWorking == 'boolean') ? shopIsWorking : shopIsWorking.toLowerCase() === 'true';
    //起送条件
    this.deliveryCondition = deliveryCondition;
    //派送费
    this.deliveryFee = deliveryFee;
    //免派送费的条件
    this.freeDeliveryCondition = freeDeliveryCondition;
}

/*
购物车
购物车总览
购物车清单
*/
function CartVm(shopId, deliveryCondition, deliveryFee, freeDeliveryCondition) {
    if (!(deliveryFee >= 0))
        deliveryFee = 0;

    this.shopId = shopId;
    //起送条件
    this.deliveryCondition = deliveryCondition;
    //派送费
    this.deliveryFee = deliveryFee;
    //免派送费的条件
    this.freeDeliveryCondition = freeDeliveryCondition;
    //距离起送条件还差多少钱
    this.lessDeliveryCondition = 0;
    //真正的派送费（如果达到freeDeliveryCondition条件则为0）
    this.realDeliveryFee = deliveryFee;

    //购买数量
    this.count = 0;
    //商品总金额
    this.amount = 0;
    //餐盒总金额
    this.boxAmount = 0;

    this.proTypeCount = 0;
    this.subproTypeCount = 0;
}


function CategoryVm(categoryId, categoryName) {

    this.categoryId = categoryId;
    this.categoryName = categoryName;

    //分类下的产品列表
    this.pros = new Array();
    Object.defineProperty(this, "productCount", {
        get: function () {
            return this.pros.length;
        }
    });
}


