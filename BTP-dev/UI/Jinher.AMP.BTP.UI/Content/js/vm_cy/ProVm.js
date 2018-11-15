
/*
商品VM
*/
function ProductVm(kPro, name, displayPrice, price, stock, id, pic, boxPrice, boxNum) {

    if (arguments.length !== 9)
        console.error("构造函数实参数目不对！");

    //--------------构造函数初始化---------------
    this.kPro = kPro;
    this.name = name;
    this.displayPrice = displayPrice;
    this.price = price;
    this.stock = stock;
    this.id = id;
    this.pic = pic;
    //盒子单价
    this.boxPrice = boxPrice || 0;
    //餐盒数量(一个产品)
    this.boxNum = boxNum || 0;

    //--------------initPro函数初始化(initPro内部调用PRICELOGIC.alterPrice、refresh)---------------
    this.categoryId = null;
    this.categoryName = null;
    this.hasSubpro = false; //是否存在子产品（组合属性）
    this.attrs = null; //数组 数组元素是AttrVm，AttrVm里有AttrValVms
    this.subpros = null; //数组 数组元素是SubproductVm

    //--------------PRICELOGIC.alterPrice函数初始化并修改所有价格---------------
    this.hasDiscount = false; //是否有优惠活动

    //--------------通过refresh计算---------------
    this.hasStockLimit = false; //库存有限
    this.isEnough = true; //库存充足
    this.isBought = false; //购买数量是否不为0

    this.count = 0; //购买数量
    this.amount = 0; //购买总价

    //有餐盒费
    this.hasBoxFee = boxPrice > 0 && boxNum > 0;
    //盒子总金额
    this.boxAmount = 0;

    this.canIncrease = true; //还能增加
    this.canDecrease = true; //还能减少

    //--------------通过attrClick计算---------------
    this.focusSubpro = null; //当前选中的子产品
}

/*
子商品VM（一个有效的comattr组合）
*/
function SubproVm(kAttrsUnion, attrTxtsUnion, displayPrice, price, stock, id) {

    if (arguments.length !== 6)
        console.error("构造函数实参数目不对！");

    this.kSubpro = kAttrsUnion;
    this.displayPrice = displayPrice;
    this.price = price;
    this.stock = stock;
    this.id = id;

    this.kAttrsUnion = kAttrsUnion; //字符串，kAttr1,kAttr2
    this.attrTxtsUnion = attrTxtsUnion; //字符串，attrTxt1,attrTxt2

    //--------------通过refresh计算---------------
    this.hasStockLimit = false; //库存有限
    this.isEnough = true; //库存充足
    this.isBought = false; //购买数量是否不为0

    this.count = 0;
    this.amount = 0; //购买总价

    this.canIncrease = true; //还能增加
    this.canDecrease = true; //还能减少
}

/*
购物车里的商品VM
购物车里的商品，和ProVm的主要区别是：购物车里的商品可能是主商品，也可能是子商品
*/
function CartproVm(
            kPro, kSubpro, isSubpro, proId, subproId, kAttrsUnion, attrTxtsUnion,
            name, pic, displayPrice, price,
            count, amount,
            boxPrice, boxNum, boxAmount,
            canIncrease, canDecrease, isEnough, isBought, isDeleted) {

    this.kPro = kPro;
    this.kSubpro = kSubpro;
    this.isSubpro = isSubpro;
    this.proId = proId;
    this.subproId = subproId;

    this.kAttrsUnion = kAttrsUnion;
    this.attrTxtsUnion = attrTxtsUnion;

    this.name = name;
    this.pic = pic;

    this.displayPrice = displayPrice;
    this.price = price;

    //购买数量
    this.count = count;
    //商品总金额
    this.amount = amount;

    //盒子单价
    this.boxPrice = boxPrice;
    //餐盒数量(一个产品)
    this.boxNum = boxNum;
    //盒子总金额
    this.boxAmount = boxAmount;

    //还能增加
    this.canIncrease = canIncrease;
    //还能减少
    this.canDecrease = canDecrease;
    //数量足够
    this.isEnough = isEnough;
    //购买数量是否不为0
    this.isBought = isBought;
    //商品已经被删除
    this.isDeleted = isDeleted;
} 

/*
组合属性VM
一个商品最多两个
*/
function AttrVm(kAttr, name) {
    this.kAttr = kAttr;
    this.name = name;

    this.vals = new Array(); //每个val是一个 AttrValVm 对象
    this.checkedValK = null;
}

/*
组合属性的值VM
*/
function AttrValVm(kAttrVal, txt) {
    this.kAttrVal = kAttrVal;
    this.txt = txt;
    this.isChecked = false; //是否选中
}