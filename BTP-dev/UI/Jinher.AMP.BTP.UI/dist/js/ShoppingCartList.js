new Vue({
    el: '#app',
    data: function(){
        return{
            hearderTitle: '购物车',
            editText: '编辑',
            isEmpty: null,
            currentValue: [],
            checkboxAll: [],
            host: '',
            commodifyList: [],//商品列表
            totalPrice: 0,
            allSelectFlag: false,
            bottomMenus: {},
            isShowMenu: false,
            buttonText: '结算',
            isEdit: false,
            popupShow: false,
            attributes: {},//商品属性
            selectSize: {},//选择的商品属性
            selectSizeIndex: null, //选择商品属性元素所在的索引值
            selectSizeParentIndex: null, //选择商品属性元素的店铺所在的索引值
            curShopCartItemId: null,
            sliderConf: {distance:63},
            allowClick: true,
            isUpdateCount: true
        }
    },
    mounted: function(){
        this.$nextTick(function(){
            this.host = window.location.host.substring(0,window.location.host.indexOf('btp'));
            //判断用户是否已登录
            if (!JsVilaDataNull(getUserId()) || !JsVilaDataNull(getSessionId())) {
                DealLoginPartial.initPartialPage();
            }
            this.loadData();
        });
    },
    methods:{
        editClick:function(){
            this.isEdit = !this.isEdit;
            this.editText = this.isEdit? "完成": "编辑";
            this.buttonText = this.isEdit? "删除": "结算";
        },
        loadData:function(){
            this.$http({
                url:'/Mobile/GetShoppongCartList',
                method: 'GET',
                params:{
                    appId: getEsAppId(),
                    userId: getUserId()
                }
            }).then(function(response){
                if(response.data.CommodifyList.length){
                    this.isEmpty = false;
                    this.tansformData(response.data.CommodifyList);
                }else{
                    this.isEmpty = true;
                }
            },function(err){
                this.$toast('服务器繁忙，请稍后再试！');
            });
        },
        tansformData:function(data){
            var shop = [];
            this.commodifyList = [];
            for(var i = 0; i < data.length; i++){
                var index = shop.indexOf(data[i].AppId);
                if(index != -1){
                    this.commodifyList[index].Lists.push(data[i]);
                }else{
                    var commodify = {
                        AppId : data[i].AppId,
                        AppName: data[i].AppName,
                        Lists:[data[i]]
                    };
                    this.commodifyList.push(commodify);
                    shop.push(data[i].AppId);
                }
            }
        },
        handlerFun:function(lists,$index,index){
            var _this = this;
            this.$confirm('确定要删除此商品吗？',function(){
                _this.deleteCommoditys(lists[$index].ShopCartItemId,function(){
                    lists.splice($index,1);
                    if(lists.length == 0){
                        _this.commodifyList.splice(index,1);//删除dom元素
                    }
                });
            });
        },
        //单个商品checkbox 选中/取消
        selectChange:function(index,state){
            if(!this.isEdit){
                switch(state){
                    case 1:
                        this.$toast('此商品已售罄');
                        return;
                        break;
                    case 2:
                        this.$toast('此商品已下架');
                        return;
                        break;
                    case 3:
                    case 4:
                    case 9999:
                        this.$toast('此商品已失效');
                        return;
                        break;
                }
            }
            var _this = this;
            setTimeout(function(){
                var lists = _this.commodifyList[index].Lists;
                var flag = false;
                var disabled = [];
                if(lists && lists.length){
                    for(var i = 0; i < lists.length; i++){
                        if(!_this.isEdit && lists[i].Disabled){
                            disabled.push({id:'checkbox_' + lists[i].AppId + '_' + i,data: lists[i]});
                        }
                        if(!_this.isHasCheckbox("checkbox_" + lists[i].AppId + "_" + i) && (_this.isEdit || !lists[i].Disabled)){
                            flag = true;
                        }
                    }
                }
                if(flag){
                    var $index = _this.checkboxAll.indexOf(lists[0].AppId);
                    if($index !== -1){
                        _this.checkboxAll.splice($index,1);
                        for(var i = 0 ; i < disabled.length; i++){
                            for(var k = 0; k < _this.currentValue.length; k++){
                                if(_this.currentValue[k].id == disabled[i].id){
                                    _this.currentValue.splice(k,1);
                                    break;
                                }
                            }
                        }
                    }
                }else{
                    if(_this.checkboxAll.indexOf(lists[0].AppId) == -1){
                        _this.checkboxAll.push(lists[0].AppId);
                        for(var j = 0; j < disabled.length; j++){
                            if(!_this.isHasCheckbox(disabled[j].id)){
                                _this.currentValue.push(disabled[j]);
                            }
                        }
                    }
                }
                _this.setSselectAllstate();
            });
        },
        // 店铺checkbox 选中/取消
        shopSelectChange:function(id){
            var _this = this;
            setTimeout(function(){
                if(_this.checkboxAll.indexOf(id) == -1){//取消选中
                    for(var j = 0; j < _this.currentValue.length; j++){
                        if(_this.currentValue[j].id.indexOf(id) !== -1){
                            _this.currentValue.splice(j,1);
                        }
                    }
                }else{//选中
                    for(var k = 0; k < _this.commodifyList.length; k++){
                        if(_this.commodifyList[k].AppId == id){
                            var lists = _this.commodifyList[k].Lists;
                            for(var i =0; i < lists.length; i++){
                                if(!_this.isHasCheckbox('checkbox_' + id + "_" + i)){
                                    _this.currentValue.push({id:'checkbox_' + id + "_" + i,data:lists[i]});
                                }
                            }
                        }
                    }
                }
                _this.setSselectAllstate();
            });
        },
        //全选/反选
        allSelect:function(){
            var _this = this;
            setTimeout(function(){
                if(_this.allSelectFlag){
                    for(var i = 0; i < _this.commodifyList.length; i++){
                        if(_this.checkboxAll.indexOf(_this.commodifyList[i].AppId) == -1){
                            _this.checkboxAll.push(_this.commodifyList[i].AppId);
                            _this.shopSelectChange(_this.commodifyList[i].AppId);
                        }
                    }
                }else{
                    _this.checkboxAll = [];
                    _this.currentValue = [];
                }
            });
        },
        setSselectAllstate:function(){
            if(this.checkboxAll.length == this.commodifyList.length){
                this.allSelectFlag = true;
            }else{
                this.allSelectFlag = false;
            }
        },
        setAttribute:function(data,state){
            if(state == 'RealPrice'){
                var realPrice = data.DiscountPrice > -1 ? DiscountPrice.toFixed(2) : (data.Price * (data.Intensity / 10)).toFixed(2);
                this.$set(data,'RealPrice',realPrice);
            }else if(state == 'RealStock'){
                var realStock = data.Stock;
                if(data.LimitBuyEach > 0 && realStock > data.LimitBuyEach){
                    realStock = data.LimitBuyEach;
                }
                if(data.LimitBuyTotal > 0 && realStock > (data.LimitBuyTotal - data.SurplusLimitBuyTotal)){
                    realStock = data.LimitBuyTotal - data.SurplusLimitBuyTotal;
                }
                this.$set(data,'RealStock',realStock);
            }else if(state == 'disabled'){
                if(',1,2,3,4,9999'.indexOf(',' + data.ShopCartState) !== -1){
                    this.$set(data,'Disabled', true);
                }
            }
        },
        //计算总价格
        calculatePrice:function(value){
            var totalPrice = 0;
            for(var i = 0; i < value.length; i++){
                if(!value[i].data.Disabled){
                    totalPrice += value[i].data.RealPrice * value[i].data.CommodityNumber;
                }
            }
            return '￥' + totalPrice.toFixed(2);
        },
        //点击 结算/删除 按钮
        buttonClick:function(){
            if(this.isEdit){//删除
                var selectCartItemId = [];
                if(this.currentValue.length){
                    for(var i = 0; i < this.currentValue.length; i++){
                        selectCartItemId.push(this.currentValue[i].data.ShopCartItemId);
                    }
                    var _this = this;
                    this.$confirm('确定要删除选中的商品？',function(){
                        //回复默认滑动删除组件状态
                        for(var i = 0; i < _this.$children.length; i++){
                            if(/left-delete/ig.test(_this.$children[i].$el.className) && _this.$children[i].$data.transformX !== 0){
                                _this.$children[i].$data.transformX = 0;
                            }
                        }
                        _this.dialogShow = false;
                        _this.deleteCommoditys(selectCartItemId.join(','),function(){
                            for(var i = 0; i < selectCartItemId.length; i++){
                                for(var j = 0 ;j < _this.commodifyList.length; j++){
                                    var lists = _this.commodifyList[j].Lists;
                                    for(var k = 0; k < lists.length; k++){
                                        if(lists[k].ShopCartItemId == selectCartItemId[i]){
                                            var appId = lists[k].AppId;
                                            lists.splice(k,1);
                                            if(lists.length == 0){
                                                for(var key = 0; key < _this.commodifyList.length; key++){
                                                    if(_this.commodifyList[key].AppId == appId){
                                                        _this.commodifyList.splice(key,1);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        });
                    })
                }else{
                    this.$toast('你还没有选商品哦~');
                }
            }else{//结算
                if(!this.currentValue.length){
                    this.$toast('请选择商品');
                    return;
                }
                var xcdata = '';
                for(var i =0; i < this.currentValue.length; i++){
                    if(!this.currentValue[i].data.Disabled){
                        xcdata += "{\"CommodityId\":\"" + this.currentValue[i].data.Id + "\" ,\"CommodityStockId\":\"" + this.currentValue[i].data.CommodityStockId + "\"},";
                    }
                }
                var checkdata = "{\"UserID\":\"" + getUserId() + "\" ,\"CommodityIdAndStockIds\":[" + xcdata.substr(0, xcdata.length - 1) + "],\"promotionType\":-1,\"diyGroupId\":\"00000000-0000-0000-0000-000000000000\"}";
                this.$http.post('/Mobile/CheckCommodity',checkdata,{emulateJSON: true}).then(function(response){
                    if(!response.data.length){
                        this.$toast('服务器繁忙,请稍候再试');
                        return;
                    }
                    var commodSelected = [];
                    for(var i = 0 ; i < response.data.length; i++){
                        var item = response.data[i];
                        var oldItem = this.getCommoditysById(item.CommodityStockId) || {};
                        if(item.State == 1 || item.State == 3){
                            this.$toast(oldItem.Name + ' 已下架');
                            return;
                        }
                        if(item.IsNeedPresell == 1 && !item.IsPreselled){
                            this.$toast(oldItem.Name + ' 需要预约才可以购买');
                            return;
                        }
                        if((item.LimitBuyEach > 0 && oldItem.CommodityNumber > item.LimitBuyEach) || oldItem.CommodityNumber > item.Stock || (item.LimitBuyTotal > 0 && oldItem.CommodityNumber > (item.LimitBuyTotal - item.SurplusLimitBuyTotal))){
                            this.$toast(oldItem.Name + ' 数量超出范围');
                            return;
                        }
                        if(item.Price.toFixed(2) != oldItem.RealPrice){
                            var _this = this;
                            this.$confirm(oldItem.Name + '商品价格发生变化，是否仍然购买？',function(){
                                var commodSeled = _this.buildCartData(commodSelected);
                                sessionStorage.ShopCartDate = JSON.stringify(commodSeled);
                                var totalPrice = _this.calculatePrice(_this.currentValue);
                                setTimeout(function () {
                                    gotoCreateOrder("gouwuche",'',totalPrice.split('￥')[1]);
                                }, 1000);
                            });
                            this.loadData();
                            return;
                        }
                        oldItem.UserId = getUserId();
                        oldItem.SizeAndColorId = oldItem.Size;
                        commodSelected.push(oldItem);
                    }
                    if(commodSelected.length){
                        var commodSeled = this.buildCartData(commodSelected);
                        sessionStorage.ShopCartDate = JSON.stringify(commodSeled);
                        var totalPrice = this.calculatePrice(this.currentValue);
                        setTimeout(function () {
                            gotoCreateOrder("gouwuche",'',totalPrice.split('￥')[1]);
                        }, 1000);
                    }
                },function(err){
                    this.$toast('服务器繁忙，请稍后再试！');
                });
            }
        },
        //删除商品
        deleteCommoditys:function(shopCartItemId,callback){
            this.$loading.open('删除中。。。');
            this.$http({
                url: '/Mobile/DeleteCommoditysFromShoppingCart',
                method: 'GET',
                params:{
                    shopCartItemIds: shopCartItemId,
                    userId: getUserId(),
                    appId: getEsAppId()
                }
            }).then(function(response){
                this.$loading.close();
                if(response.data.ResultCode == 0){
                    this.$toast("删除商品成功");
                    if(typeof callback == 'function'){
                        callback();
                    }
                    if(this.commodifyList.length == 0){
                        this.isEmpty = true;
                    }
                }else{
                    this.$toast("删除商品失败");
                }
            },function(err){
                this.$toast('删除商品失败');
                this.$loading.close();
            })
        },
        //判断某个元素是否选中
        isHasCheckbox:function(id){
            for(var i = 0; i < this.currentValue.length; i++){
                if(this.currentValue[i].id == id){
                    return true;
                }
            }
            return false;
        },
        //获取某个元素
        getCommoditysById:function(id){
            for(var j = 0; j < this.currentValue.length; j++){
                var lists = this.currentValue[j].data;
                if(lists.CommodityStockId == id){
                    return lists;
                }
            }
            return false;
        },
        //构建购物车商品下订单的数据
        buildCartData:function(data){
            var appIdAll = [];
            var shops = [];
            for(var i = 0 ; i< data.length; i++){
                var item = data[i];
                if(appIdAll.indexOf(item.AppId) == -1){
                    appIdAll.push(item.AppId);
                    var shop = {
                        AppId: item.AppId,
                        AppName: item.AppName,
                        AppAmount: Number((item.RealPrice * item.CommodityNumber).toFixed(2)),
                        CommodityNum: item.CommodityNumber,
                        ShoppingCartItemSDTO: [item]
                    };
                    shops.push(shop);
                }else{
                    for(var j = 0; j < shops.length; j++){
                        if(shops[j].AppId == item.AppId){
                            shops[j].AppAmount = (item.RealPrice * item.CommodityNumber).toFixed(2) - 0 + shops[j].AppAmount;
                            shops[j].CommodityNum = shops[j].CommodityNum - 0 + item.CommodityNumber;
                            shops[j].ShoppingCartItemSDTO.push(item);
                            break;
                        }
                    }
                }
            }
            return shops;
        },
        changeCountFunc:function(shopCartItemId,appId,commodityNumber,edit){
            //更新商品数量
            if(edit !== this.isEdit || !commodityNumber){;return;}
            if(!this.isUpdateCount){
                this.isUpdateCount = true;
                return;
            }
            var comnum = "{\"Number\":" + commodityNumber + ",\"ShopCartItemId\":\"" + shopCartItemId + "\"}";
            var strjson = "{\"shopCartCommodityUpdateDTOs\":[" + comnum + "],\"userId\":\"" + getUserId() + "\",\"appId\":\"" + appId + "\"}";
            this.$http.post('/Mobile/UpdateShoppingCart',strjson,{emulateJSON: true}).then(function(response){
                if (response.data.ResultCode !== 0) {
                    this.$toast(response.data.Message);
                }
            },function(err){
                this.$toast('服务器繁忙,请稍候再试');
            });
        },
        //显示属性弹窗
        showAttribute:function(list,index,index2){
            if(!this.allowClick){
                this.allowClick = true;
                return false;
            }
            if(list.ShopCartState == 2 || list.ShopCartState == 3 || list.ShopCartState == 4 || list.ShopCartState == 9999|| list.IsDel){return;}
            //获取属性接口
            this.$http({
                url:'/Mobile/GetShoppongCartItemAttribute',
                method:'GET',
                params:{
                    commodityId: list.Id,
                    userId: getUserId()
                }
            }).then(function(response){
                this.attributes = response.data.Data;
                this.curShopCartItemId = list.ShopCartItemId;
                this.selectSize = list;
                this.selectSizeIndex = index;
                this.selectSizeParentIndex = index2;
                this.popupShow = true;
            },function(err){
            });
        },
        //点击属性弹窗确定按钮
        attrDialogOk:function(msg){
            var price = msg.price;
            this.$http.post('/Mobile/UpdateShoppingAttribute',{
                ShopCartItemId:this.selectSize.ShopCartItemId,
                userId: getUserId(),
                appId: getEsAppId(),
                StrComAttributes: msg.size.join(',')
            },{emulateJSON: true}).then(function(response){
                if(response.body.ResultCode == 0){
                    this.loadData();
                    this.popupShow = false;
                }else{
                    var message = response.body.Message || '服务器繁忙，请稍后重试！';
                    this.$toast(message);
                }
            },function(err){
                this.$toast('服务器繁忙，请稍后重试！');
            });
        },
        bottomMenuFun: function(){
            this.isShowMenu = true;
        },
        fractionlFilter:function(value){
            if(value == undefined || value == null){
                return;
            }
            return '<span class="font11">￥</span>' + value.split('.')[0] + '<span class="font11">.'+ value.split('.')[1] +'</span>';
        },
        goPage:function(state,id){
            if(state == "home"){
                window.location.href = 'http://' + this.host + "wap.iuoooo.com/appphonepage?appId=" + getEsAppId();
            }
            if(state == "shop"){
                window.location.href = urlAppendCommonParams("/Mobile/CommodityList?shopId=" + getEsAppId());
            }
            if(state == 'detail'){
                if(!this.allowClick){
                    this.allowClick = true;
                    return false;
                }
                window.location.href = urlAppendCommonParams('/Mobile/CommodityDetail?commodityId=' + id + '&shopId=' + getEsAppId());
            }
        },
        /**
         * 设置赠品属性
         */
        presentSize: function(sku){
            var str = [];
            for(var i = 0; i < sku.length; i++){
                str.push(sku[i].SecondAttribute);
            }
            return str.length > 0 ? str.join(' ') : '';
        },
        /**
         * 设置赠品数量
         * @param CommodityNumber 购买本商品数量
         * @param Limit 赠品需要达到购买本商品的购买件数(达到设置的件数才可以赠送商品)
         * @param Number 每达到limit件数后可获得曾品的件数
         */
        presentNum: function(CommodityNumber,Limit,Number){
            //获得赠送赠品的次数
            var count = parseInt(CommodityNumber / Limit);
            //获得赠品的总数
            var prensetnNum = parseInt(count * Number);
            return '数量 ' + prensetnNum;
        }
    },
    filters:{
        priceFilter:function(value){
            if(value == undefined || value == null){
                return;
            }
            return '￥' + value.toFixed(2);
        },
        sizeFilter:function(value){
            if(value == "") return;
            var arr = value.split(',');
            return arr.join(' ');
        }
    }
});